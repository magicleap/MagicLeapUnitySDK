using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Time
{
    internal struct TimeSpec
    {
        internal long Seconds;
        internal long NanoSeconds;
    }
    
    internal unsafe class TimeSpecNativeFunctions : NativeFunctionsBase
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
