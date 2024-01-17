// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using System;
using System.Collections.Generic;
using UnityEngine.XR.MagicLeap;
#if UNITY_OPENXR_1_9_0_OR_NEWER
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.NativeTypes;
#endif
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
#if UNITY_EDITOR
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
#if UNITY_OPENXR_1_9_0_OR_NEWER
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
        
        internal static bool DidXrCallSucceed(XrResult result, string functionName = "A native function", Predicate<XrResult> successCase = null, bool showError = true)
        {
            var success = successCase?.Invoke(result) ?? result == XrResult.Success;

            if (!success && showError)
                MLPluginLog.ErrorFormat($"{functionName} in the Magic Leap API failed. Reason: {result} ");
            return success;
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
#endif // UNITY_OPENXR_1_9_0_OR_NEWER

        internal static Type[] InstallPath = {
            typeof(Initialization),
            typeof(Initialization.XREarlyUpdate)
        };

        internal static bool InstallIntoPlayerLoop(ref PlayerLoopSystem topLevelPlayerLoop, PlayerLoopSystem systemToInstall, params Type[] installPath)
        {
            installPath ??= Array.Empty<Type>();

            ref var current = ref topLevelPlayerLoop;
            foreach (var path in installPath)
            {
                var idx = Array.FindIndex(current.subSystemList, s => s.type == path);
                if (idx == -1)
                    return false;
                current = ref current.subSystemList[idx];
            }

            InstallSystem(ref current, systemToInstall);
            return true;
        }

        private static void InstallSystem(ref PlayerLoopSystem parentSystem, PlayerLoopSystem targetSystem)
        {
            var subsystems = parentSystem.subSystemList ?? Array.Empty<PlayerLoopSystem>();
            var length = subsystems.Length;
            Array.Resize(ref subsystems, length + 1);
            subsystems[length] = targetSystem;
            parentSystem.subSystemList = subsystems;
        }
    }
    
     
}
