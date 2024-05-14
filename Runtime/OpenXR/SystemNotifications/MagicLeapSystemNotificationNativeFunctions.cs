
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    using MagicLeapOpenXRSystemNotificationNativeTypes;
    internal unsafe class MagicLeapSystemNotificationNativeFunctions : MagicLeapNativeFunctionsBase
    {
        internal delegate* unmanaged [Cdecl] <ulong, in XrSystemNotificationSetInfo, XrResult> XrSetSystemNotifications;
        protected override void LocateNativeFunctions()
        {
            XrSetSystemNotifications = (delegate* unmanaged[Cdecl]<ulong, in XrSystemNotificationSetInfo, XrResult>)LocateNativeFunction("xrSetSystemNotificationsML");
        }
    }
}
