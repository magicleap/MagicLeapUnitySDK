using System;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.SystemInfo
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
    
    internal unsafe class SystemInfoNativeFunctions : NativeFunctionsBase
    {
        internal delegate* unmanaged [Cdecl] <ulong, in XrSystemGetInfo, out XrSystemId, XrResult> XrGetSystem;
        internal delegate* unmanaged [Cdecl] <ulong, ulong, out XrSystemProperties, XrResult> XrGetSystemProperties;
        protected override void LocateNativeFunctions()
        {
            XrGetSystem = (delegate* unmanaged[Cdecl]<ulong, in XrSystemGetInfo, out XrSystemId, XrResult>)LocateNativeFunction("xrGetSystem");
            XrGetSystemProperties = (delegate* unmanaged[Cdecl]<ulong, ulong, out XrSystemProperties, XrResult>)LocateNativeFunction("xrGetSystemProperties");
        }

        internal XrResult GetSystemId(out XrSystemId systemId, XrSystemFormFactor formFactor = XrSystemFormFactor.HeadMountedDisplay)
        {
            systemId = 0;
            var systemGetInfo = new XrSystemGetInfo
            {
                Type = XrSystemInfoTypes.XrTypeSystemGetInfo,
                Next = default,
                FormFactor = formFactor
            };
            return XrGetSystem(XrInstance, in systemGetInfo, out systemId);
        }
    }
}
