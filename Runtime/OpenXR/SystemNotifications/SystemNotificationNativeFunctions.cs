
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Features.SystemNotifications
{
    internal unsafe class SystemNotificationNativeFunctions : NativeFunctionsBase
    {
        internal delegate* unmanaged [Cdecl] <ulong, in XrSystemNotificationSetInfo, XrResult> XrSetSystemNotifications;
        protected override void LocateNativeFunctions()
        {
            XrSetSystemNotifications = (delegate* unmanaged[Cdecl]<ulong, in XrSystemNotificationSetInfo, XrResult>)LocateNativeFunction("xrSetSystemNotificationsML");
        }
    }
}
