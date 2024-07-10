

using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    using MagicLeapConvertTimeSpecNativeTypes;
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MagicLeapConvertTimeSpecNativeFunctions")]
    internal unsafe class MagicLeapConvertTimeSpecNativeFunctions : MagicLeapNativeFunctionsBase
    {
        internal delegate* unmanaged [Cdecl] <ulong, in TimeSpec, out long, XrResult> XrConvertTimeSpecTimeToTime;
        internal delegate* unmanaged [Cdecl] <ulong, long, out TimeSpec, XrResult> XrConvertTimeToTimeSpecTime;
        protected override void LocateNativeFunctions()
        {
            XrConvertTimeSpecTimeToTime = (delegate* unmanaged[Cdecl]<ulong, in TimeSpec, out long, XrResult>)LocateNativeFunction("xrConvertTimespecTimeToTimeKHR");
            XrConvertTimeToTimeSpecTime = (delegate* unmanaged[Cdecl]<ulong, long, out TimeSpec, XrResult>)LocateNativeFunction("xrConvertTimeToTimespecTimeKHR");
        }
    }
}
