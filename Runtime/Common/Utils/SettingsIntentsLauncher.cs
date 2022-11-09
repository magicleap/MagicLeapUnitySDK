// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    public static class SettingsIntentsLauncher
    {
        private const string UnityPlayerClassName = "com.unity3d.player.UnityPlayer";
        private const string UnityPlayerActivityRefName = "currentActivity";
        private const string AndroidIntentClassName = "android.content.Intent";
        private const string AndroidIntentStartActivityFuncName = "startActivity";

        private const string DisplaySettingsIntentName = "android.settings.DISPLAY_SETTINGS";
        private const string VoiceInputSettingsIntentName = "com.magicleap.intent.action.VOICE_INPUT_SETTINGS";
        private const string ARCloudSettingsIntentName = "com.magicleap.intent.action.AR_CLOUD_SETTINGS";

        /// <summary>
        ///     Utility function to launch the Magic Leap System Settings application and 
        ///     navigate to the page of the provided intent name. This uses the Android Intents
        ///     API to invoke the proper application intent.
        /// </summary>
        /// <param name="intentName">Name of the intent to launch the settings page for.</param>
        public static void LaunchSystemSettings(string intentName)
        {
            using (var unityClass = new AndroidJavaClass(UnityPlayerClassName))
            using (AndroidJavaObject currentActivityObject =
                    unityClass.GetStatic<AndroidJavaObject>(UnityPlayerActivityRefName))
            using (var intentObject = new AndroidJavaObject(
                        AndroidIntentClassName, intentName))
            {
                currentActivityObject.Call(AndroidIntentStartActivityFuncName, intentObject);
            }
        }

        /// <summary>
        ///     Utility function to launch the Magic Leap System Settings application and 
        ///     navigate to the display page. This uses the Android Intents API to invoke
        ///     the proper application intent. 
        /// </summary>
        public static void LaunchSystemDisplaySettings()
        {
            LaunchSystemSettings(DisplaySettingsIntentName);
        }

        /// <summary>
        ///     Utility function to launch the Magic Leap System Settings application and 
        ///     navigate to the voice input page. This uses the Android Intents API to invoke
        ///     the proper application intent. 
        /// </summary>
        public static void LaunchSystemVoiceInputSettings()
        {
            LaunchSystemSettings(VoiceInputSettingsIntentName);
        }

        /// <summary>
        ///     Utility function to launch the Magic Leap System Settings application and 
        ///     navigate to the AR Cloud page. This uses the Android Intents API to invoke
        ///     the proper application intent. 
        /// </summary>
        public static void LaunchSystemARCloudSettings()
        {
            LaunchSystemSettings(ARCloudSettingsIntentName);
        }
    }
}
