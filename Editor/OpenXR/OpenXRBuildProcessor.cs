using System;
using System.IO;
using System.Xml;
using UnityEditor.Build.Reporting;
using UnityEditor.XR.MagicLeap;

#if UNITY_OPENXR_1_4_0_OR_NEWER
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport;

namespace UnityEditor.XR.OpenXR.Features.MagicLeapSupport
{
    internal class OpenXRBuildProcessor : OpenXRFeatureBuildHooks
    {
        public override int callbackOrder => 1;

        public override Type featureType => typeof(MagicLeapFeature);

        private static readonly string kHaveAndroidWindowSupportBootSettingsKey = "android-device-have-window-support";
        private static readonly string kUseNullDisplayManagerBootSettingsKey = "android-device-use-null-display-manager";
        private static readonly string kAndroidAudioUseMLAudio = "android-audio-use-MLAudio";
        private static readonly string kVulkanForceDisableETCSupport = "vulkan-force-disable-ETC-support";
        private static readonly string kVulkanForceDisableASTCSupport = "vulkan-force-disable-ASTC-support";
        private static readonly string kVulkanDisablePreTransform = "vulkan-disable-pre-transform";

        protected override void OnPreprocessBuildExt(BuildReport report)
        {
            AndroidBootConfig bootConfig = new AndroidBootConfig(report);

            if (report.summary.platform == BuildTarget.Android)
            {
                bootConfig.SetValueForKey(kHaveAndroidWindowSupportBootSettingsKey, "0", true);
                bootConfig.SetValueForKey(kUseNullDisplayManagerBootSettingsKey, "1", true);
                bootConfig.SetValueForKey(kVulkanForceDisableETCSupport, "1", true);
                bootConfig.SetValueForKey(kVulkanForceDisableASTCSupport, "1", true);
                bootConfig.SetValueForKey(kVulkanDisablePreTransform, "1", true);
                MagicLeapFeature magicLeapFeature = FeatureHelpers.GetFeatureWithIdForBuildTarget(BuildTargetGroup.Android, MagicLeapFeature.featureId) as MagicLeapFeature;
                if (magicLeapFeature != null)
                {
                    bootConfig.SetValueForKey(kAndroidAudioUseMLAudio, magicLeapFeature.IsMLAudioEnabled ? "1" : "0" , true);
                }
                else
                {
                    bootConfig.SetValueForKey(kAndroidAudioUseMLAudio, "0", true);
                }
            }
            else
            {
                bootConfig.ClearEntryForKey(kHaveAndroidWindowSupportBootSettingsKey);
                bootConfig.ClearEntryForKey(kUseNullDisplayManagerBootSettingsKey);
                bootConfig.ClearEntryForKey(kVulkanForceDisableETCSupport);
                bootConfig.ClearEntryForKey(kVulkanForceDisableASTCSupport);
                bootConfig.ClearEntryForKey(kVulkanDisablePreTransform);
                bootConfig.ClearEntryForKey(kAndroidAudioUseMLAudio);
            }

            bootConfig.WriteBootConfig();
        }

        protected override void OnPostGenerateGradleAndroidProjectExt(string path)
        {
            string manifestPath = Path.Combine(path, "src", "main", "AndroidManifest.xml");
            AndroidManifestXml manifest = new AndroidManifestXml(manifestPath);

            manifest.AddPermission("org.khronos.openxr.permission.OPENXR");
            manifest.AddPermission("org.khronos.openxr.permission.OPENXR_SYSTEM");
            AddOpenXRRuntimeBrokerInfo(manifest);
            AddOpenXRImmersiveHmdIntent(manifest);
            manifest.Save();
        }

        protected override void OnPostprocessBuildExt(BuildReport report) { }

        private void AddOpenXRRuntimeBrokerInfo(AndroidManifestXml manifest)
        {
            const string queryElem = "queries";
            const string providerElem = "provider";
            const string providerAttrib = "org.khronos.openxr.runtime_broker;org.khronos.openxr.system_runtime_broker";

            // Get all child nodes that match the tag and see if value already exists
            XmlNode queriesNode = manifest.ManifestElement.SelectSingleNode(queryElem);
            if (queriesNode == null)
            {
                queriesNode = manifest.ManifestElement.AppendChild(manifest.CreateElement(queryElem));
            }

            manifest.UpdateOrCreateAttribute(queriesNode as XmlElement, providerElem, "authorities", providerAttrib);
        }

        private void AddOpenXRImmersiveHmdIntent(AndroidManifestXml manifest)
        {
            XmlElement activityIntentFilterElement = manifest.SelectSingleNode("/manifest/application/activity/intent-filter") as XmlElement;
            manifest.UpdateOrCreateAttribute(activityIntentFilterElement,
                    "category",
                    "name",
                    "org.khronos.openxr.intent.category.IMMERSIVE_HMD");
        }
    }
}
#endif
