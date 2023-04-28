// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2023) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine.XR.MagicLeap.Native;
namespace UnityEngine.XR.MagicLeap
{
    using System.Runtime.InteropServices;
    /// <summary>
    /// MLHeadsetFit description goes here.
    /// </summary>
    public partial class MLNotifications
    {
        /// <summary>
        /// See ml_system_notification_manager.h for additional comments.
        /// </summary>
        private class NativeBindings : Native.MagicLeapNativeBindings
        {

            /// <summary>
            /// Create System Notification manager client.
            /// </summary>
            [DllImport(MLNotificationsManagerDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSystemNotificationManagerCreate(out ulong handle);


            /// <summary>
            /// Destroy System Notification manager client.
            /// </summary>
            [DllImport(MLNotificationsManagerDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSystemNotificationManagerDestroy(ulong handle);


            /// <summary>
            /// Suppress/Unsuppress System Notifications.
            /// </summary>
            [DllImport(MLNotificationsManagerDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSystemNotificationManagerSetNotifications(ulong handle, bool suppress);

        }
    }
}
