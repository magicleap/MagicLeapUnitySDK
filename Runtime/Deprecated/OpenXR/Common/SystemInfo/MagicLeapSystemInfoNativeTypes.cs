using System;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    namespace MagicLeapSystemInfoNativeTypes
    {
        internal enum XrSystemInfoTypes : ulong
        {
            XrTypeSystemGetInfo = 4,
            XrTypeSystemProperties = 5,
        }

        internal enum XrSystemFormFactor : uint
        {
            HeadMountedDisplay = 1,
            HandheldDisplay = 2,
        }
        
        internal struct XrSystemGraphicsProperties
        {
            internal uint MaxSwapchainImageHeight;
            internal uint MaxSwapchainImageWidth;
            internal uint MaxLayerCount;
        }

        internal struct XrSystemTrackingProperties
        {
            internal uint OrientationTracking;
            internal uint PositionTracking;
        }

        internal unsafe struct XrSystemProperties
        {
            private const int SystemNameLength = 256;
            internal XrSystemInfoTypes Type;
            internal IntPtr Next;
            internal ulong SystemId;
            internal uint VendorId;
            internal fixed byte SystemName[SystemNameLength];
            internal XrSystemGraphicsProperties GraphicsProperties;
            internal XrSystemTrackingProperties TrackingProperties;
        }

        internal struct XrSystemGetInfo
        {
            internal XrSystemInfoTypes Type;
            internal IntPtr Next;
            internal XrSystemFormFactor FormFactor;
        }
    }
}
