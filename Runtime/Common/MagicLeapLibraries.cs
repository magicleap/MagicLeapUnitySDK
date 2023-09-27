// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%


namespace UnityEngine.XR.MagicLeap.Native
{
    public partial class MagicLeapNativeBindings
    {
        // Use SDK loader lib for features that will work in ML App Sim
        public const string MLSdkLoaderDll = "ml_sdk_loader";

#if UNITY_EDITOR
        /// <summary>
        /// Perception library name string.
        /// </summary>
        public const string MLPerceptionClientDll = MLSdkLoaderDll;

        /// <summary>
        /// Platform level library name string.
        /// </summary>
        public const string MLPlatformDll = MLSdkLoaderDll;

        /// <summary>
        /// MLAudio library name.
        /// </summary>
        protected const string AudioPlayerDLL = MLSdkLoaderDll;

        /// <summary>
        /// Internal DLL used by the API.
        /// </summary>
        protected const string MLGazeRecognitionDll = MLSdkLoaderDll;

        /// <summary>
        /// ZI permissions library name
        /// </summary>
        protected const string MLZIPermissionsDll = MLSdkLoaderDll;

        /// <summary>
        /// Internal DLL used by the API.
        /// </summary>
        protected const string MLWebRTCDLL = MLSdkLoaderDll;

        /// <summary>
        /// The MLCamera library name.
        /// </summary>
        protected const string MLCameraDll = MLSdkLoaderDll;

        /// <summary>
        /// Internal DLL used by the API.
        /// </summary>
        protected const string MLCameraMetadataDll = MLSdkLoaderDll;

        /// <summary>
        /// Internal DLL used by the Input API.
        /// </summary>
        protected const string MLInputDll = MLSdkLoaderDll;

        /// <summary>
        /// Internal DLL used by the Notifications API.
        /// </summary>
        protected const string MLNotificationsManagerDll = MLSdkLoaderDll;

        /// <summary>
        /// Internal DLL used by the API.
        /// </summary>
        protected const string MLVoiceDll = MLSdkLoaderDll;

        /// <summary>
        /// Internal DLL used by the Space API.
        /// </summary>
        protected const string MLSpaceDll = MLSdkLoaderDll;

        /// <summary>
        /// Internal DLL used by the API.
        /// </summary>
        protected const string MLPowerManagerDll = MLSdkLoaderDll;

        /// <summary>
        /// MLMediaError library name.
        /// </summary>
        protected const string MLMediaErrorDLL = MLSdkLoaderDll;
        
#else
        /// <summary>
        /// Perception library name string.
        /// </summary>
        public const string MLPerceptionClientDll = "perception.magicleap";

        /// <summary>
        /// Platform level library name string.
        /// </summary>
        public const string MLPlatformDll = "platform.magicleap";

        /// <summary>
        /// MLAudio library name.
        /// </summary>
        protected const string AudioPlayerDLL = "audio.magicleap";

        /// <summary>
        /// Internal DLL used by the API.
        /// </summary>
        protected const string MLGazeRecognitionDll = "perception.magicleap";

        /// <summary>
        /// ZI permissions library name
        /// </summary>
        protected const string MLZIPermissionsDll = "zi.magicleap";

        /// <summary>
        /// Internal DLL used by the API.
        /// </summary>
        protected const string MLWebRTCDLL = "webrtc.magicleap";

        /// <summary>
        /// The MLCamera library name.
        /// </summary>
        protected const string MLCameraDll = "camera.magicleap";

        /// <summary>
        /// Internal DLL used by the API.
        /// </summary>
        protected const string MLCameraMetadataDll = "camera_metadata.magicleap";

        /// <summary>
        /// Internal DLL used by the Input API.
        /// </summary>
        protected const string MLInputDll = "input.magicleap";

        /// <summary>
        /// Internal DLL used by the Notifications API.
        /// </summary>
        protected const string MLNotificationsManagerDll = "system_notification_manager.magicleap";
        
        /// <summary>
        /// Internal DLL used by the API.
        /// </summary>
        protected const string MLVoiceDll = "input.magicleap";
        
        /// <summary>
        /// Internal DLL used by the Space API.
        /// </summary>
        protected const string MLSpaceDll = "space.magicleap";

        /// <summary>
        /// Internal DLL used by the API.
        /// </summary>
        protected const string MLPowerManagerDll = "power_manager.magicleap";

        /// <summary>
        /// MLMediaError library name.
        /// </summary>
        protected const string MLMediaErrorDLL = "media_error.magicleap";
        
#endif
        /// <summary>
        /// Unity's XR provider library in com.unity.xr.magicleap
        /// </summary>
        public const string UnityMagicLeapDll = "UnityMagicLeap";

        /// <summary>
        /// Internal DLL used by the API.
        /// </summary>
        protected const string MLMediaFormatDll = "media_format.magicleap";

        /// <summary>
        /// Internal DLL used by the API.
        /// </summary>
        protected const string MediaMuxerDll = "media_muxer.magicleap";

        /// <summary>
        /// Internal DLL used by the API.
        /// </summary>
        protected const string MLMediaDRMDll = "media_drm.magicleap";

        /// <summary>
        /// Media CC parser library name
        /// </summary>
        protected const string MLMediaCEA608DLL = "media_ccparser.magicleap";

        /// <summary>
        /// Media CEA-708 library name
        /// </summary>
        protected const string MLMediaCEA708DLL = "media_cea708parser.magicleap";

        /// <summary>
        /// Internal DLL used by the API.
        /// </summary>
        protected const string MLMediaPlayerDll = "media_player.magicleap";

        /// <summary>
        /// Internal DLL used by the API.
        /// </summary>
        protected const string MLMediaRecorderDll = "media_recorder.magicleap";

        /// <summary>
        /// Internal DLL used by the API.
        /// </summary>
        protected const string MLNativeSurfaceDll = "native_surface.magicleap";

        /// <summary>
        /// Internal DLL used by the API.
        /// </summary>
        protected const string MLWebViewDll = "webview.magicleap";

        /// <summary>
        /// Internal DLL used to get Java VM Pointer.
        /// </summary>
        protected const string CUtilsDLL = "ml_c_utils";

        /// <summary>
        /// Internal DLL used to send unity audio buffers to MLAudio api.
        /// </summary>
        protected const string AudioOutputPluginDLL = "MLAudioOutput";
    }
}
