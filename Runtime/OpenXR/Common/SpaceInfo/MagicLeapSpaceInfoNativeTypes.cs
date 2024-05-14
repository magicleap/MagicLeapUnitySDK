using System;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    using MagicLeapOpenXRFeatureNativeTypes;
    namespace MagicLeapSpaceInfoNativeTypes
    {
        internal enum XrSpaceLocationFlagsML : ulong
        {
            OrientationValid = 0x00000001,
            PositionValid = 0x00000002,
            OrientationTracked = 0x00000004,
            PositionTracked = 0x00000008,
        }
        
        internal struct XrSpaceLocation
        {
            internal const ulong XrSpaceLocationStructType = 42;
            internal ulong Type;
            internal IntPtr Next;
            internal XrSpaceLocationFlagsML SpaceLocationFlags;
            internal XrPose Pose;
        }
    }
}
