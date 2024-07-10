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
        public const string EyeTracking = "com.magicleap.permission.EYE_TRACKING";
        public const string PupilSize = "com.magicleap.permission.PUPIL_SIZE";
        public const string SpatialAnchors = "com.magicleap.permission.SPATIAL_ANCHOR";
        public const string HandTracking = "com.magicleap.permission.HAND_TRACKING";
        public const string WebView = "com.magicleap.permission.WEBVIEW";
        public const string MarkerTracking = "com.magicleap.permission.MARKER_TRACKING";
        public const string VoiceInput = "com.magicleap.permission.VOICE_INPUT";
        public const string SpatialMapping = "com.magicleap.permission.SPATIAL_MAPPING";
        public const string DepthCamera = "com.magicleap.permission.DEPTH_CAMERA";
        public const string SystemNotification = "com.magicleap.permission.SYSTEM_NOTIFICATION";
        public const string SpaceManager = "com.magicleap.permission.SPACE_MANAGER";
        public const string SpaceImportExport = "com.magicleap.permission.SPACE_IMPORT_EXPORT";
        public const string EyeCamera = "com.magicleap.permission.EYE_CAMERA";
        public const string FacialExpression = "com.magicleap.permission.FACIAL_EXPRESSION";
        
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
