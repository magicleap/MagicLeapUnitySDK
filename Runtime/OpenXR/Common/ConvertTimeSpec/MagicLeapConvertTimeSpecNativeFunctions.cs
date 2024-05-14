

using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    using MagicLeapConvertTimeSpecNativeTypes;
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
