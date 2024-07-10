using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    using MagicLeapSystemInfoNativeTypes;
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MagicLeapSystemInfoNativeFunctions")]
    internal unsafe class MagicLeapSystemInfoNativeFunctions : MagicLeapNativeFunctionsBase
    {
        internal delegate* unmanaged [Cdecl] <ulong, in XrSystemGetInfo, out ulong, XrResult> XrGetSystem;
        internal delegate* unmanaged [Cdecl] <ulong, ulong, out XrSystemProperties, XrResult> XrGetSystemProperties;
        protected override void LocateNativeFunctions()
        {
            XrGetSystem = (delegate* unmanaged[Cdecl]<ulong, in XrSystemGetInfo, out ulong, XrResult>)LocateNativeFunction("xrGetSystem");
            XrGetSystemProperties = (delegate* unmanaged[Cdecl]<ulong, ulong, out XrSystemProperties, XrResult>)LocateNativeFunction("xrGetSystemProperties");
        }

        internal XrResult GetSystemId(out ulong systemId, XrSystemFormFactor formFactor = XrSystemFormFactor.HeadMountedDisplay)
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
