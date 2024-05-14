﻿using System;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapOpenXRFeatureNativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    namespace MagicLeapFacialExpressionNativeTypes
    {
        internal enum XrFacialExpressionStructTypes : ulong
        {
            XrTypeSystemFacialExpressionProperties = 1000482004U,
            XrTypeFacialExpressionClientCreateInfo = 1000482005U,
            XrTypeFacialExpressionBlendShapeGetInfo = 1000482006U,
            XrTypeFacialExpressionBlendShapeProperties = 1000482007U
        }

        internal enum XrFacialBlendShapeML
        {
            BrowLowerer_L = 0,
            BrowLowerer_R = 1,
            CheekRaiser_L = 2,
            CheekRaiser_R = 3,
            ChinRaiser = 4,
            Dimpler_L = 5,
            Dimpler_R = 6,
            EyesClosed_L = 7,
            EyesClosed_R = 8,
            InnerBrowRaiser_L = 9,
            InnerBrowRaiser_R = 10,
            JawDrop = 11,
            LidTightener_L = 12,
            LidTightener_R = 13,
            LipCornerDepressor_L = 14,
            LipCornerDepressor_R = 15,
            LipCornerPuller_L = 16,
            LipCornerPuller_R = 17,
            LipFunneler_LB = 18,
            LipFunneler_LT = 19,
            LipFunneler_RB = 20,
            LipFunneler_RT = 21,
            LipPressor_L = 22,
            LipPressor_R = 23,
            LipPucker_L = 24,
            LipPucker_R = 25,
            LipStretcher_L = 26,
            LipStretcher_R = 27,
            LipSuck_LB = 28,
            LipSuck_LT = 29,
            LipSuck_RB = 30,
            LipSuck_RT = 31,
            LipTightener_L = 32,
            LipTightener_R = 33,
            LipsToward = 34,
            LowerLipDepressor_L = 35,
            LowerLipDepressor_R = 36,
            NoseWrinkler_L = 37,
            NoseWrinkler_R = 38,
            OuterBrowRaiser_L = 39,
            OuterBrowRaiser_R = 40,
            UpperLidRaiser_L = 41,
            UpperLidRaiser_R = 42,
            UpperLipRaiser_L = 43,
            UpperLipRaiser_R = 44,
            TongueOut = 45,
        }

        [Flags]
        internal enum XrFacialExpressionBlendShapePropertiesFlags : ulong
        {
            Valid = 0x01,
            Tracked = 0x02
        }

        internal struct XrSystemFacialExpressionProperties
        {
            internal XrFacialExpressionStructTypes Type;
            internal IntPtr Next;
            internal XrBool32 SupportsFacialExpression;
        }

        internal unsafe struct XrFacialExpressionClientCreateInfo
        {
            internal XrFacialExpressionStructTypes Type;
            internal IntPtr Next;
            internal uint RequestedCount;
            internal XrFacialBlendShapeML* RequestedFacialBlendShapes;
        }

        internal struct XrFacialExpressionShapeGetInfo
        {
            internal XrFacialExpressionStructTypes Type;
            internal IntPtr Next;
        }

        internal struct XrFacialExpressionBlendShapeProperties
        {
            internal XrFacialExpressionStructTypes Type;
            internal IntPtr Next;
            internal XrFacialBlendShapeML RequestedFacialBlendShape;
            internal float Weight;
            internal XrFacialExpressionBlendShapePropertiesFlags Flags;
            internal long Time;

            internal static XrFacialExpressionBlendShapeProperties CreateFromBlendShapeProperties(in MagicLeapFacialExpressionFeature.BlendShapeProperties properties, long time)
            {
                var result = new XrFacialExpressionBlendShapeProperties()
                {
                    Type = XrFacialExpressionStructTypes.XrTypeFacialExpressionBlendShapeProperties,
                    RequestedFacialBlendShape = (XrFacialBlendShapeML)properties.FacialBlendShape,
                    Weight = properties.Weight,
                    Flags = (XrFacialExpressionBlendShapePropertiesFlags)properties.Flags,
                    Time = time
                };
                return result;
            }

            internal void AssignBlendShapeProperties(ref MagicLeapFacialExpressionFeature.BlendShapeProperties properties)
            {
                properties.FacialBlendShape = (MagicLeapFacialExpressionFeature.FacialBlendShape)RequestedFacialBlendShape;
                properties.Weight = Weight;
                properties.Flags = (MagicLeapFacialExpressionFeature.BlendShapePropertiesFlags)Flags;
            }
        }
    }
}
