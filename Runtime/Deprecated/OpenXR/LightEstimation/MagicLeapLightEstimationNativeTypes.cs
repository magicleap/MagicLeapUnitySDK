using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.MagicLeap.Unsafe;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapLightEstimationFeature
    {
        internal const int TotalHarmonics = 27;

        internal const int SpecVersion = 1;

        internal enum XrLightEstimationHDRCubemapFaceResolution
        {
            Resolution_64x64,
            Resolution_128x128,
            Resolution_256x256
        }

        internal enum XrLightEstimationObjectTypes : ulong
        {
            XrObjectTypeLightEstimationML = 1000481000U,
            XrObjectTypeLightEstimationEstimate = 1000481001U
        }

        internal enum XrLightEstimationStructTypes : ulong
        {
            XrTypeLightEstimationCreateInfoML = 1000481000U,
            XrTypeLightEstimationStateML = 1000481001U,
            XrTypeLightEstimationHDRCubemapML = 1000481002U,
            XrTypeLightEstimationMainDirectionalLightML = 1000481003U,
            XrTypeLightEstimationSphericalHarmonicsML = 1000481004U
        }

        internal struct XrLightEstimationCreateInfo
        {
            internal XrLightEstimationStructTypes Type;
            internal IntPtr Next;
            internal ulong BaseSpace;
            internal long Time;
            internal XrLightEstimationHDRCubemapFaceResolution CubemapFaceResolution;
        }

        internal struct XrLightEstimationState
        {
            internal XrLightEstimationStructTypes Type;
            internal IntPtr Next;
            internal long LastUpdate;
        }

        internal unsafe struct XrLightEstimationHDRCubemap
        {
            internal XrLightEstimationStructTypes Type;
            internal IntPtr Next;
            internal uint PixelCountInput;
            internal uint PixelCountOutput;
            internal float* Pixels;
            internal uint FaceDim;
        }

        internal struct XrLightEstimationMainDirectionalLight
        {
            internal XrLightEstimationStructTypes Type;
            internal IntPtr Next;
            internal Vector3 Direction;
            internal Color Color;
        }

        internal unsafe struct XrLightEstimationSphericalHarmonics
        {
            internal XrLightEstimationStructTypes Type;
            internal IntPtr Next;
            internal fixed float HarmonicsCoefficients[TotalHarmonics];
        }
    }
}
