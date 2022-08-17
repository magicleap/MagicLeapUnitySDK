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

using System.IO;
using System.Text;
using UnityEngine;

namespace UnityEditor.XR.MagicLeap
{
    public class PermissionSettingsLoader
    {
        private string FilePath => Path.Combine(MagicLeapSDKUtil.SdkPath, "data", "ml_permissions.json");
        internal PermissionsListJson settingJson { get; private set; }

        public PermissionSettingsLoader() => Initialize();
      
        private void Initialize()
        {
            try
            {
                var json = File.ReadAllText(FilePath, Encoding.UTF8);

                settingJson = JsonUtility.FromJson<PermissionsListJson>($"{{\"Settings\":{json}}}");
            }
            catch
            {
                Debug.LogError("Failed to load permissions data. Do you have the newest Magic Leap SDK?");
            }
        }
    }
}
