// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using UnityEngine.Android;

namespace UnityEngine.XR.MagicLeap
{
    [Obsolete("Use MagicLeap.Android.Permissions instead.")]
    public struct MLPermission
    {
        [Obsolete("Use UnityEngine.Android.Permission.Camera")]
        public const string Camera = Permission.Camera;

        [Obsolete("Use UnityEngine.Android.Permission.Microphone")]
        public const string RecordAudio = Permission.Microphone;
        
        [Obsolete("Use UnityEngine.Android.Permission.ExternalStorageRead")]
        public const string ReadExternalStorage = Permission.ExternalStorageRead;

        [Obsolete("Use UnityEngine.Android.Permission.ExternalStorageWrite")]
        public const string WriteExternalStorage = Permission.ExternalStorageWrite;

        public const string Internet = "android.permission.INTERNET";

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
    }
}
