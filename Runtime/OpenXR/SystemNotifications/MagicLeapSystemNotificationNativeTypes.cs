using System;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    namespace MagicLeapOpenXRSystemNotificationNativeTypes
    {
        internal enum XrSystemNotificationStructTypes : ulong
        {
            XrTypeSystemNotificationsSetInfo = 1000473000U,
            XrTypeSystemNotificationsProperties = 1000473001U
        }

        [Flags]
        internal enum XrSystemNotificationsCapabilityFlags : ulong
        {
            XrSystemNotificationCapabilitySuppression = 0x00000001
        }

        internal struct XrSystemNotificationSetInfo
        {
            internal XrSystemNotificationStructTypes Type;
            internal IntPtr Next;
            internal uint SuppressNotifications;
        }

        internal struct XrSystemNotificationProperties
        {
            internal XrSystemNotificationStructTypes Type;
            internal IntPtr Next;
            internal XrSystemNotificationsCapabilityFlags SupportedFeatures;
        }
    }
}
