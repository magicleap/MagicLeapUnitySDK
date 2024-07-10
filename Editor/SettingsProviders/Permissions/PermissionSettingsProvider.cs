// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEditor;

namespace MagicLeap.Settings
{
    internal class PermissionSettingsProvider
    {
        [SettingsProvider]
        internal static SettingsProvider RenderSettingProvider()
        {
            return new PermissionsSettingsProviderPresenter(
                "MagicLeap/Permissions",
                @"Permissions");
        }
    }
}
