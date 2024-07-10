using System;
using UnityEngine.XR.OpenXR.NativeTypes;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapOpenXRFeatureNativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    namespace MagicLeapSpatialAnchorsNativeTypes
    {
        internal enum XrSpatialAnchorsStructTypes : ulong
        {
            XrTypeSpatialAnchorsCreateInfoFromPose = 1000140000U,
            XrTypeCreateSpatialAnchorsCompletion = 1000140001U,
            XrTypeSpatialAnchorState = 1000140002U
        }

        internal enum XrSpatialAnchorConfidence
        {
            Low = 0,
            Medium = 1,
            High = 2
        }

        internal struct XrSpatialAnchorsCreateInfoBaseHeader
        {
            internal XrSpatialAnchorsStructTypes Type;
            internal IntPtr Next;
        }

        internal struct XrSpatialAnchorsCreateInfoFromPose
        {
            internal XrSpatialAnchorsStructTypes Type;
            internal IntPtr Next;
            internal ulong BaseSpace;
            internal XrPose PoseInBaseSpace;
            internal long Time;
        }

        internal unsafe struct XrCreateSpatialAnchorsCompletion
        {
            internal XrSpatialAnchorsStructTypes Type;
            internal IntPtr Next;
            internal XrResult FutureResult;
            internal uint SpaceCount;
            internal ulong* Spaces;
        }

        internal struct XrSpatialAnchorState
        {
            internal XrSpatialAnchorsStructTypes Type;
            internal IntPtr Next;
            internal XrSpatialAnchorConfidence Confidence;
        }
    }
}
