// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

namespace MagicLeap.Android
{
    public static class Permissions
    {
        public static void RequestPermission(string permission,
            Action<string> onPermissionGranted = null,
            Action<string> onPermissionDenied = null,
            Action<string> onPermissionDeniedDontAskAgain = null)
        {
            if (Application.isEditor)
            {
                onPermissionGranted?.Invoke(permission);
            }
            else
            {
                RequestPermissions(new string[] { permission }, onPermissionGranted, onPermissionDenied, onPermissionDeniedDontAskAgain);
            }
        }

        public static void RequestPermissions(string[] permissions,
            Action<string> onPermissionGranted = null,
            Action<string> onPermissionDenied = null,
            Action<string> onPermissionDeniedDontAskAgain = null)
        {
            var permissionsToCheck = new List<string>();
            foreach (string permission in permissions)
            {
                if (CheckPermission(permission))
                {
                    onPermissionGranted?.Invoke(permission);
                }
                else
                {
                    permissionsToCheck.Add(permission);
                }
            }

            if (Application.isEditor || permissionsToCheck.Count == 0)
            {
                return;
            }

            var callbacks = new PermissionCallbacks();
            callbacks.PermissionGranted += onPermissionGranted;
            callbacks.PermissionDenied += onPermissionDenied;
            callbacks.PermissionDeniedAndDontAskAgain += onPermissionDeniedDontAskAgain;
            Permission.RequestUserPermissions(permissionsToCheck.ToArray(), callbacks);
        }

        public static bool CheckPermission(string permission) => Application.isEditor || Permission.HasUserAuthorizedPermission(permission);
    }
}
