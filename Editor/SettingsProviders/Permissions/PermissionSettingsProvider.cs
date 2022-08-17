// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2022 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEditor;

namespace UnityEditor.XR.MagicLeap
{
    internal class PermissionSettingsProvider
    {
        [SettingsProvider]
        internal static UnityEditor.SettingsProvider RenderSettingProvider()
        {
            return new PermissionsSettingsProviderPresenter(
                "MagicLeap/Manifest Settings",
                @"Permissions");
        }
    }
}
