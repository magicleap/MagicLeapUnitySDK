// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

// Disabling deprecated warning for the internal project
#pragma warning disable 618

using UnityEngine.XR.MagicLeap.Native;
namespace UnityEngine.XR.MagicLeap
{
    public partial class MLNotifications : MLAutoAPISingleton<MLNotifications>
    {
        #region MLAutoAPISingleton
        /// <summary>
        /// Start the API.
        /// </summary>
        protected override MLResult.Code StartAPI() => Instance.InternalMLNotificationCreateClient();

        /// <summary>
        /// Stop the API.
        /// </summary>
        protected override MLResult.Code StopAPI() => Instance.InternalMLNotificationStopClient();
        #endregion


        public static MLResult SetNotifications(bool suppress) => MLResult.Create(Instance.InternalMLNotificationsSuppress(suppress));


        #region internal
        /// <summary>
        /// Creates system notification manager client.
        /// <para>API Level 24</para>
        /// permissions com.magicleap.permission.SYSTEM_NOTIFICATION (protection level: normal)
        /// </summary>
        /// <returns>
        /// MLResult.Code.InvalidParam: Failed due to an invalid parameter.<br/>
        /// MLResult.Code.Ok: System Notification Manager handle was successfully created.<br/>
        /// MLResult.Code.PermissionDenied: Necessary permission is missing.<br/>
        /// MLResult.Code.UnspecifiedFailure: System Notification Manager handle failed to be created.
        /// </returns>
        private MLResult.Code InternalMLNotificationCreateClient()
        {
            if (!MLResult.DidNativeCallSucceed(MLPermissions.CheckPermission(MLPermission.SystemNotification).Result))
            {
                MLPluginLog.Error($"{nameof(MLNotifications)} requires missing permission {MLPermission.SystemNotification}");
                return MLResult.Code.PermissionDenied;
            }

            var resultCode = NativeBindings.MLSystemNotificationManagerCreate(out Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLSystemNotificationManagerCreate));
            return resultCode;
        }

        /// <summary>
        /// Destroy system notification manager client.
        /// <para>API Level 24</para>
        /// permissions None
        /// </summary>
        /// <returns>
        /// MLResult.Code.InvalidParam: The handle passed in was not valid.<br/>
        /// MLResult.Code.Ok: System Notification Manager handle was successfully stopped.
        /// </returns>
        private MLResult.Code InternalMLNotificationStopClient()
        {
            var resultCode = NativeBindings.MLSystemNotificationManagerDestroy(Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLSystemNotificationManagerDestroy));
            return resultCode;
        }

        /// <summary>
        /// Set system notification suppressions.
        /// <para>API Level 24</para>
        /// permissions None
        /// </summary>
        /// <returns>
        /// MLResult.Code.InvalidParam: The handle passed in was not valid.<br/>
        /// MLResult.Code.Ok: All system notifications were successfully suppressed/unsuppressed.<br/>
        /// MLResult.Code.UnspecifiedFailure: Suppression/Unsuppression of system notifications failed.
        /// </returns>
        private MLResult.Code InternalMLNotificationsSuppress(bool suppress)
        {
            var resultCode = NativeBindings.MLSystemNotificationManagerSetNotifications(Handle, suppress);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLSystemNotificationManagerSetNotifications));
            return resultCode;
        }
        #endregion
    }
}

