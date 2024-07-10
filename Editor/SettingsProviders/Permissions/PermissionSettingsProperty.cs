// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2022-2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using MagicLeap.Settings.Android;
using System;
using System.IO;
using UnityEngine.UIElements;

namespace MagicLeap.Settings
{
    [Serializable]
    internal class PermissionSettingsProperty
    {
        internal bool Default;

        private Permission permission;

        internal Toggle Toggle { get; set; }

        private AndroidManifestXml androidManifest;
        private bool allowToggle = false;

        internal PermissionSettingsProperty(Permission setting, bool isMandatory)
        {
            permission = setting;
            if (File.Exists(AndroidManifestXml.AssetPath))
            {
                androidManifest = new AndroidManifestXml(AndroidManifestXml.AssetPath);
            }
            allowToggle = (androidManifest != null);
            if (isMandatory)
            {
                allowToggle = false;
            }
        }

        internal void RenderProperty(VisualElement root)
        {
            Toggle = new Toggle { label = permission.Name, tooltip = permission.Description };
            Toggle.AddToClassList("settings-permission-label");
            Toggle.AddToClassList("extended-width");
            Toggle.SetEnabled(allowToggle);
            if (allowToggle)
            {
                Toggle.RegisterValueChangedCallback(OnValueChange);
            }
            root.Add(Toggle);
        }

        private void OnValueChange(ChangeEvent<bool> evt)
        {
            bool shouldInclude = evt.newValue;
            if (androidManifest != null)
            {
                if (shouldInclude)
                {
                    androidManifest.AddPermission(permission.Name);
                }
                else
                {
                    androidManifest.RemovePermission(permission.Name);
                }
                androidManifest.Save();
            }
        }
    }
}
