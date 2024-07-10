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
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor.XR.Management;
using System.Linq;
using UnityEngine.XR.MagicLeap;
#if UNITY_OPENXR_1_9_0_OR_NEWER
using UnityEngine.XR.OpenXR;
#endif

namespace MagicLeap
{
    // Pre-build checks for certain project configurations before generating a Magic Leap APK
    public class BuildPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.Android && IsMagicLeapOrOpenXRLoaderEnabled())
            {
                SetLinearColorSpace();
                SetAndroidMinSdkVersion();
            }
        }
        
        /// <summary>
        /// Displays Dialog box at build time about incorrect color space setting.
        /// </summary>
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
                        "PlayerSettings.colorSpace is set to Gamma which is not recommended for Magic Leap 2.",
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
#if UNITY_XR_MAGICLEAP_PROVIDER
            var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
            return settings != null && settings.Manager != null && settings.Manager.activeLoaders.Any(l => l is MagicLeapLoader);
#else
            return false;
#endif
        }

        private bool IsOpenXRLoaderEnabled()
        {
#if UNITY_OPENXR_1_9_0_OR_NEWER
            var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
            if (settings != null && settings.Manager != null && settings.Manager.activeLoaders.Where(l => l is OpenXRLoader).Count() > 0)
            {
                return true;
            }
#endif
            return false;
        }

        private bool IsMagicLeapOrOpenXRLoaderEnabled()
        {
            var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
            if (settings != null && settings.Manager != null)
            {
                bool isOpenXRLoaderActive = IsOpenXRLoaderEnabled();
                bool isMagicLeapLoaderActive = IsMagicLeapLoaderEnabled();
                
                return isOpenXRLoaderActive || isMagicLeapLoaderActive;
            }

            return false;
        }
    }
}
