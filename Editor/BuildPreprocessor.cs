// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.IO;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor.XR.MagicLeap;
using UnityEditor.XR.Management;
using System.Linq;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    /// <summary>
    /// Displays Dialog box at build time about incorrect color space setting.
    /// </summary>
    public class ColorSpaceBuildPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.Android && IsMagicLeapLoaderEnabled())
            {
                SetLinearColorSpace();
                SetAndroidMinSdkVersion();
            }
        }

        private void SetLinearColorSpace()
        {
            if (PlayerSettings.colorSpace != ColorSpace.Linear
                && !Application.isBatchMode)
            {
                const string assetPath = "Assets/Plugins/Android/x86_64";
                const string assetName = "EditorProjectData.asset";
                EditorProjectData editorProjectData = AssetDatabase.LoadAssetAtPath($"{assetPath}/{assetName}", typeof(EditorProjectData)) as EditorProjectData;

                if (editorProjectData == null)
                {
                    editorProjectData = ScriptableObject.CreateInstance<EditorProjectData>();

                    Directory.CreateDirectory(assetPath);
                    AssetDatabase.CreateAsset(editorProjectData, $"{assetPath}/{assetName}");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                if (editorProjectData.EnableColorSpaceWarning)
                {
                    int option = EditorUtility.DisplayDialogComplex("Invalid Color Space",
                        "PlayerSettings.colorSpace is set to Gamma for Relish which is not recommended for that platform.",
                        "Switch to Linear Space",
                        "Ignore and continue always",
                        "Ignore and continue"
                    );

                    if (option == 0)
                    {
                        PlayerSettings.colorSpace = ColorSpace.Linear;
                    }
                    else if (option == 1)
                    {
                        editorProjectData.EnableColorSpaceWarning = false;
                        EditorUtility.SetDirty(editorProjectData);
                    }
                }
            }
        }

        private void SetAndroidMinSdkVersion()
        {
            if (PlayerSettings.Android.minSdkVersion != AndroidSdkVersions.AndroidApiLevel29)
            {
                Debug.LogWarning($"Min SDK version for this project is {PlayerSettings.Android.minSdkVersion} but ML2 requires {AndroidSdkVersions.AndroidApiLevel29}. Updating the project setting.");
                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
            }
        }

        private bool IsMagicLeapLoaderEnabled()
        {
            var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
            if (settings != null && settings.Manager != null && settings.Manager.activeLoaders.Where(l => l is MagicLeapLoader).Count() > 0)
            {
                return true;
            }
            return false;
        }
    }
}
