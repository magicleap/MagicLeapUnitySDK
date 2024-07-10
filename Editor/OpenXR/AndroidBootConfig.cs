// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2023-2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace MagicLeap
{
    internal class AndroidBootConfig
    {
        private static readonly string xrBootSettingsKey = "xr-boot-settings";
        Dictionary<string, string> bootConfigSettings;

        private BuildReport buildReport;

        public AndroidBootConfig(BuildReport report)
        {
            buildReport = report;
            ReadBootConfg();
        }

        public void SetValueForKey(string key, string value, bool replace = false)
        {
            if (bootConfigSettings.ContainsKey(key))
            {
                bootConfigSettings[key] = value;
            }
            else
            {
                bootConfigSettings.Add(key, value);
            }
        }

        public void ClearEntryForKey(string key)
        {
            if (bootConfigSettings.ContainsKey(key))
            {
                bootConfigSettings.Remove(key);
            }
        }

        public void WriteBootConfig()
        {
            // boot settings string format
            // <boot setting>:<value>[;<boot setting>:<value>]*
            bool firstEntry = true;
            var sb = new System.Text.StringBuilder();
            foreach (var kvp in bootConfigSettings)
            {
                if (!firstEntry)
                {
                    sb.Append(";");
                }
                sb.Append($"{kvp.Key}:{kvp.Value}");
                firstEntry = false;
            }

            string buildTargetName = BuildPipeline.GetBuildTargetName(buildReport.summary.platform);
            EditorUserBuildSettings.SetPlatformSettings(buildTargetName, xrBootSettingsKey, sb.ToString());
        }

        private void ReadBootConfg()
        {
            bootConfigSettings = new Dictionary<string, string>();

            string buildTargetName = BuildPipeline.GetBuildTargetName(buildReport.summary.platform);
            string xrBootSettings = EditorUserBuildSettings.GetPlatformSettings(buildTargetName, xrBootSettingsKey);
            if (!string.IsNullOrEmpty(xrBootSettings))
            {
                // boot settings string format
                // <boot setting>:<value>[;<boot setting>:<value>]*
                var bootSettings = xrBootSettings.Split(';');
                foreach (var bootSetting in bootSettings)
                {
                    var setting = bootSetting.Split(':');
                    if (setting.Length == 2 && !String.IsNullOrEmpty(setting[0]) && !String.IsNullOrEmpty(setting[1]))
                    {
                        bootConfigSettings.Add(setting[0], setting[1]);
                    }
                }
            }
        }
    }
}
