// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLPermissionNames.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2021 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine.Android;

namespace UnityEngine.XR.MagicLeap
{
    public struct MLPermission
    {
        public const string Camera = Permission.Camera;

        public const string RecordAudio = Permission.Microphone;
        
        public const string ReadExternalStorage = Permission.ExternalStorageRead;
        
        public const string WriteExternalStorage = Permission.ExternalStorageWrite;

        public const string Internet = "android.permission.INTERNET";

        public const string EyeTracking = "com.magicleap.permission.EYE_TRACKING";

        public const string PupilSize = "com.magicleap.permission.PUPIL_SIZE";

        public const string ControllerPose = "com.magicleap.permission.CONTROLLER_POSE";

        public const string HeadPose = "com.magicleap.permission.HEAD_POSE";

        public const string SpatialAnchors = "com.magicleap.permission.SPATIAL_ANCHOR";

        public const string HandTracking = "com.magicleap.permission.HAND_TRACKING";

        public const string WebView = "com.magicleap.permission.WEBVIEW";

        public const string MarkerTracking = "com.magicleap.permission.MARKER_TRACKING";

        public const string VoiceInput = "com.magicleap.permission.VOICE_INPUT";

        public const string SpatialMapping = "com.magicleap.permission.SPATIAL_MAPPING";
    }
}
