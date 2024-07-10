// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2023-2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using MagicLeap.Settings;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.XR.Management;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR;

namespace MagicLeap.OpenXR.Features
{
    internal class OpenXRBuildProcessor : OpenXRFeatureBuildHooks
    {
        public override int callbackOrder => 1;

        public override Type featureType => typeof(MagicLeapFeature);

        private static readonly string haveAndroidWindowSupportBootSettingsKey = "android-device-have-window-support";
        private static readonly string useNullDisplayManagerBootSettingsKey = "android-device-use-null-display-manager";
        private static readonly string androidAudioUseMLAudio = "android-audio-use-MLAudio";
        private static readonly string vulkanForceDisableETCSupport = "vulkan-force-disable-ETC-support";
        private static readonly string vulkanForceDisableASTCSupport = "vulkan-force-disable-ASTC-support";
        private static readonly string vulkanDisablePreTransform = "vulkan-disable-pre-transform";

        protected override void OnPreprocessBuildExt(BuildReport report)
        {
            AndroidBootConfig bootConfig = new AndroidBootConfig(report);

            if (report.summary.platform == BuildTarget.Android)
            {
                bootConfig.SetValueForKey(haveAndroidWindowSupportBootSettingsKey, "1", true);
                bootConfig.SetValueForKey(useNullDisplayManagerBootSettingsKey, "0", true);
                bootConfig.SetValueForKey(vulkanForceDisableETCSupport, "1", true);
                bootConfig.SetValueForKey(vulkanForceDisableASTCSupport, "1", true);
                bootConfig.SetValueForKey(vulkanDisablePreTransform, "1", true);
                bootConfig.SetValueForKey(androidAudioUseMLAudio, MagicLeapProjectSettings.Instance.EnableMLAudio? "1" : "0", true);
            }
            else
            {
                bootConfig.ClearEntryForKey(haveAndroidWindowSupportBootSettingsKey);
                bootConfig.ClearEntryForKey(useNullDisplayManagerBootSettingsKey);
                bootConfig.ClearEntryForKey(vulkanForceDisableETCSupport);
                bootConfig.ClearEntryForKey(vulkanForceDisableASTCSupport);
                bootConfig.ClearEntryForKey(vulkanDisablePreTransform);
                bootConfig.ClearEntryForKey(androidAudioUseMLAudio);
            }

            bootConfig.WriteBootConfig();
        }

        protected override void OnPostGenerateGradleAndroidProjectExt(string path)
        {
            // don't bother if the OpenXR loader isn't actually enabled for the build
            if (!IsOpenXRLoaderEnabled())
                return;

            string manifestPath = Path.Combine(path, "src", "main", "AndroidManifest.xml");
            AndroidManifestXml manifest = new AndroidManifestXml(manifestPath);

            manifest.AddPermission("org.khronos.openxr.permission.OPENXR_SYSTEM");
            AddOpenXRImmersiveHmdIntent(manifest);
            manifest.Save();
        }

        protected override void OnPostprocessBuildExt(BuildReport report) { }

        private void AddOpenXRImmersiveHmdIntent(AndroidManifestXml manifest)
        {
            XmlElement activityIntentFilterElement = manifest.SelectSingleNode("/manifest/application/activity/intent-filter") as XmlElement;
            manifest.UpdateOrCreateAttribute(activityIntentFilterElement,
                    "category",
                    "name",
                    "org.khronos.openxr.intent.category.IMMERSIVE_HMD");
        }

        private bool IsOpenXRLoaderEnabled()
        {
            var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
            return settings.Manager.activeLoaders.Any(l => l is OpenXRLoader);
        }
    }
}
