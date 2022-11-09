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

[Obsolete("SettingsIntentsLauncher class has been deprecated. Use UnityEngine.XR.MagicLeap.SettingsIntentsLauncher instead.")]
public static class SettingsIntentsLauncher
{
    public static void LaunchSystemDisplaySettings()
    {
        UnityEngine.XR.MagicLeap.SettingsIntentsLauncher.LaunchSystemDisplaySettings();
    }

    public static void LaunchSystemVoiceInputSettings()
    {
        UnityEngine.XR.MagicLeap.SettingsIntentsLauncher.LaunchSystemVoiceInputSettings();
    }

    public static void LaunchSystemARCloudSettings()
    {
        UnityEngine.XR.MagicLeap.SettingsIntentsLauncher.LaunchSystemARCloudSettings();
    }
}
