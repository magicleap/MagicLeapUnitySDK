// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections.Generic;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    [Obsolete("MLPermissions is deprecated. Use MagicLeap.Android.Permissions instead.")]
    public sealed partial class MLPermissions : MLAutoAPISingleton<MLPermissions>
    {
        public delegate void OnPermissionGrantedDelegate(string permission);
        public delegate void OnPermissionDeniedDelegate(string permission);
        public delegate void OnPermissionDeniedAndDontAskAgainDelegate(string permission);

        public class Callbacks
        {
            public event OnPermissionGrantedDelegate OnPermissionGranted = delegate { };
            public event OnPermissionDeniedDelegate OnPermissionDenied = delegate { };
            public event OnPermissionDeniedAndDontAskAgainDelegate OnPermissionDeniedAndDontAskAgain = delegate { };

            internal void TriggerGranted(string permission)
            {
                OnPermissionGranted(permission);
            }

            internal void TriggerDenied(string permission)
            {
                OnPermissionDenied(permission);
            }

            internal void TriggerDeniedAndDontAskAgain(string permission)
            {
                OnPermissionDeniedAndDontAskAgain(permission);
            }
        }

        private readonly Queue<string> permissionRequests = new Queue<string>();
        private readonly Dictionary<string, List<Callbacks>> requestData = new Dictionary<string, List<Callbacks>>();
        private readonly HashSet<string> deniedPermissions = new HashSet<string>();
        private readonly HashSet<string> dontAskAgainPermissions = new HashSet<string>();

        private bool currentlyRequestingPermission = false;

        public static MLResult CheckPermission(string permission) => MLResult.Create(Instance.CheckPermissionInternal(permission));

        public static MLResult RequestPermission(string permission, Callbacks callbacks) => MLResult.Create(Instance.RequestPermissionInternal(permission, callbacks));

        private static bool NativeCallSuccess(MLResult.Code resultCode)
        {
            switch (resultCode)
            {
                case MLResult.Code.Ok:
                case MLResult.Code.Pending:
                    return true;

                default:
                    return false;
            }
        }

        protected override MLResult.Code StartAPI() => MLResult.Code.Ok;

        protected override MLResult.Code StopAPI() => MLResult.Code.Ok;

        protected override void Update()
        {
            base.Update();
            ProcessRequestQueue();
        }

        private void ProcessRequestQueue()
        {
            if (permissionRequests.Count > 0)
            {
                if (Application.isEditor)
                    return;

                // a request has been issued and is waiting for a callback. during this time,
                // the application doesn't have focus due to the popup. we need to wait until a callback
                // clears this flag after the user makes their choice and focus returns.
                if (currentlyRequestingPermission)
                {
                    return;
                }

                var permissionName = permissionRequests.Dequeue();
                // if the next permission in the queue has already been granted or the user previously denied and
                // told us not to ask about it again, don't do anything.
                if (Android.Permission.HasUserAuthorizedPermission(permissionName) || dontAskAgainPermissions.Contains(permissionName))
                {
                    return;
                }

                // request the next permission in the queue
                currentlyRequestingPermission = true;
                var callbacks = new Android.PermissionCallbacks();
                callbacks.PermissionGranted += OnPermissionGranted;
                callbacks.PermissionDenied += OnPermissionDenied;
                callbacks.PermissionDeniedAndDontAskAgain += OnPermissionDeniedDontAskAgain;
                Android.Permission.RequestUserPermission(permissionName, callbacks);
            }
        }

        private MLResult.Code CheckPermissionInternal(string permission)
        {
            if (string.IsNullOrEmpty(permission))
            {
                return MLResult.Code.InvalidParam;
            }
            MLResult.Code result = MLResult.Code.Ok;

            if (Application.isEditor)
                return result;

            nativeMLPermissionsCheckPermissionPerfMarker.Begin();
            if (Android.Permission.HasUserAuthorizedPermission(permission))
            {
                result = MLResult.Code.Ok;
            }
            else
            {
                if (dontAskAgainPermissions.Contains(permission))
                {
                    result = MLResult.Code.PermissionDenied;
                }
                else if (!deniedPermissions.Contains(permission))
                {
                    result = MLResult.Code.Pending;
                }
                else
                {
                    result = MLResult.Code.PermissionDenied;
                }
            }

            MLResult.DidNativeCallSucceed(result, nameof(Android.Permission.HasUserAuthorizedPermission), NativeCallSuccess);
            nativeMLPermissionsCheckPermissionPerfMarker.End();
            return result;
        }

        private MLResult.Code RequestPermissionInternal(string permission, Callbacks callbacks)
        {
            if (Application.isEditor)
                return MLResult.Code.Ok;

            if (string.IsNullOrEmpty(permission))
            {
                Debug.LogError($"MLPermissions: requested permission name is blank");
                return MLResult.Code.InvalidParam;
            }
            nativeMLPermissionsRequestPermissionPerfMarker.Begin();
            MLResult.Code result = MLResult.Code.Ok;

            if (!requestData.ContainsKey(permission))
            {
                requestData.Add(permission, new List<Callbacks>());
            }
            if (callbacks != null)
            {
                requestData[permission].Add(callbacks);
            }

            // If permission has already been granted, immediately trigger the
            // granted callbacks and dont enqueue the request.
            if (MLResult.IsOK(CheckPermissionInternal(permission)))
            {
                foreach (Callbacks cb in requestData[permission])
                {
                    cb.TriggerGranted(permission);
                }
                requestData[permission].Clear();
            }
            else
            {
                result = MLResult.Code.Pending;
                if (!permissionRequests.Contains(permission))
                {
                    permissionRequests.Enqueue(permission);
                }
            }

            nativeMLPermissionsRequestPermissionPerfMarker.End();
            return result;
        }

        private void OnPermissionDeniedDontAskAgain(string permission)
        {
            dontAskAgainPermissions.Add(permission);
            deniedPermissions.Add(permission);

            MLPluginLog.Debug($"MLPermissions: User denied {permission}, DON'T ASK AGAIN.");
            MLThreadDispatch.ScheduleMain(() =>
            {
                foreach (Callbacks cb in requestData[permission])
                {
                    cb.TriggerDeniedAndDontAskAgain(permission);
                }
                requestData[permission].Clear();
            });

            currentlyRequestingPermission = false;
        }

        private void OnPermissionDenied(string permission)
        {
            deniedPermissions.Add(permission);

            MLPluginLog.Debug($"MLPermissions: User denied {permission}");
            MLThreadDispatch.ScheduleMain(() =>
            {
                foreach (Callbacks cb in requestData[permission])
                {
                    cb.TriggerDenied(permission);
                }
                requestData[permission].Clear();
            });

            currentlyRequestingPermission = false;
        }

        private void OnPermissionGranted(string permission)
        {
            MLPluginLog.Debug($"MLPermissions: User granted {permission}");
            MLThreadDispatch.ScheduleMain(() =>
            {
                foreach (Callbacks cb in requestData[permission])
                {
                    cb.TriggerGranted(permission);
                }
                requestData[permission].Clear();
            });
            currentlyRequestingPermission = false;
        }
    }
}
