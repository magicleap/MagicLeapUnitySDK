using System.Collections.Generic;
#if UNITY_OPENXR_1_7_0_OR_NEWER
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR.Features;
#endif

#if UNITY_EDITOR
using System;
using UnityEditor;
#endif

namespace UnityEngine.XR.OpenXR
{
    public static class Utils
    {
        /// <summary>
        /// Finds an appropriate Magic Leap Controller registered through OpenXR which is properly named and is valid.
        /// </summary>
        /// <param name="inputDeviceCharacteristics">The characteristics required of the input device to be found.</param>
        /// <returns>A Magic Leap Controller InputDevice registered through OpenXR. If none is found then a new InputDevice is returned.</returns>
        public static InputDevice FindMagicLeapController(InputDeviceCharacteristics inputDeviceCharacteristics)
        {
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(inputDeviceCharacteristics, devices);

            // TODO: Consider changing this for loop if the controller bug is fixed.
            // Currently, the input device is registered twice and the first element returns zero values.
            // This loop searches the list in reverse to get the last element which should have the proper data.
            for (int i = devices.Count - 1; i > 0; i--)
            {
                if (devices[i].name == "Magic Leap Controller OpenXR" && devices[i].isValid)
                {
                    return devices[i];
                }
            }

            return new InputDevice();
        }
#if UNITY_OPENXR_1_7_0_OR_NEWER
        internal static bool TryGetOpenXRFeature<TFeature>(out TFeature feature) where TFeature : OpenXRFeature
        {
            feature = null;

            var settings = OpenXRSettings.Instance;
            if (settings == null)
                return false;

            feature = settings.GetFeature<TFeature>();
            return feature != null;
        }

        internal static bool TryGetOpenXRLoader(out OpenXRLoader loader)
        {
            loader = null;
            var settings = XRGeneralSettings.Instance;
            if (settings == null)
                return false;
            var mgr = settings.Manager;
            if (mgr == null)
                return false;
            loader = mgr.activeLoader as OpenXRLoader;
            return loader != null;
        }

        internal static bool TryGetSubsytemFromOpenXRLoader<TSubsystem>(out TSubsystem subsystem)
            where TSubsystem : class, ISubsystem
        {
            subsystem = null;
            if (!TryGetOpenXRLoader(out var loader))
                return false;
            subsystem = loader.GetLoadedSubsystem<TSubsystem>();
            return subsystem != null;
        }
        
#if UNITY_EDITOR
        internal static string GetNiceTypeName(System.Type type)
            => UnityEditor.ObjectNames.NicifyVariableName(type.Name);

        internal static string GetNiceTypeName<T>()
            => GetNiceTypeName(typeof(T));
        
        private static OpenXRSettings GetSettings(BuildTargetGroup group)
            => OpenXRSettings.GetSettingsForBuildTargetGroup(group);

        private static OpenXRFeature GetFeatureForBuildTarget(BuildTargetGroup group, Type featureType)
        {
            var settings = GetSettings(group);
            if (settings == null)
                return null;
            return settings.GetFeature(featureType);
        }

        private static TFeature GetFeatureForBuildTarget<TFeature>(BuildTargetGroup group) where TFeature: OpenXRFeature
        {
            var settings = GetSettings(group);
            if (settings == null)
                return null;
            return settings.GetFeature<TFeature>();
        }

        internal static bool IsFeatureEnabled(BuildTargetGroup group, Type featureType)
        {
            var feature = GetFeatureForBuildTarget(group, featureType);
            return feature != null && feature.enabled;
        }
        
        internal static bool IsFeatureEnabled<TFeature>(BuildTargetGroup group) where TFeature: OpenXRFeature
        {
            var feature = GetFeatureForBuildTarget<TFeature>(group);
            return feature != null && feature.enabled;
        }

        internal static bool TryEnableFeature(BuildTargetGroup group, Type featureType)
        {
            var feature = GetFeatureForBuildTarget(group, featureType);
            return feature != null && (feature.enabled = true) || false;
        }

        internal static bool TryEnableFeature<TFeature>(BuildTargetGroup group) where TFeature : OpenXRFeature
        {
            var feature = GetFeatureForBuildTarget<TFeature>(group);
            return feature != null && (feature.enabled = true) || false;
        }
#endif // UNITY_EDITOR
#endif // UNITY_OPENXR_1_7_0_OR_NEWER
    }
    
     
}
