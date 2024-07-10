// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.IO;
using System.Text;
using UnityEngine;

namespace MagicLeap.Settings
{
    internal class PermissionSettingsLoader
    {
        private const string manifestFileName = "ml_permissions.json";
        public string ManifestFilePath => Path.Combine("Packages/com.magicleap.unitysdk/Editor/SettingsProviders/Permissions/data~", manifestFileName);
        internal PermissionsListJson settingJson { get; private set; }

        public PermissionSettingsLoader() => Initialize();
      
        private void Initialize()
        {
            try
            {
                var json = File.ReadAllText(ManifestFilePath, Encoding.UTF8);

                settingJson = JsonUtility.FromJson<PermissionsListJson>($"{{\"Settings\":{json}}}");
            }
            catch
            {
                Debug.LogError("Failed to load permissions data. Do you have the newest Magic Leap SDK?");
            }
        }
    }
}
