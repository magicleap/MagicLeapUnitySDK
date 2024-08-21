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
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.OpenXR
{
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.Utils")]
    public static class Utils
    {
        public enum MLXrResult
        {
            GraphicsRequirementsCallMissing = -50,
            RuntimeUnavailable = -51,
            ControllerModelKeyInvalidMSFT = -1000055000,
            ReprojectionModeUnsupportedMSFT = -1000066000,
            ComputeNewSceneNotCompletedMSFT = -1000097000,
            SceneComponentIdInvalidMSFT = -1000097001,
            SceneComponentTypeMismatchMSFT = -1000097002,
            SceneMeshBufferIdInvalidMSFT = -1000097003,
            SceneComputefeatureincompatibleMSFT = -1000097004,
            SceneComputeConsistencyMismatchMSFT = -1000097005,
            DisplayRefreshRateUnsupportedFB = -1000101000,
            ColorSpaceUnsupportedFB = -1000108000,
            SpaceComponentNotSupportedFB = -1000113000,
            SpaceComponentNotEnabledFB = -1000113001,
            SpaceComponentStatusPendingFB = -1000113002,
            SpaceComponentStatusAlreadySetFB = -1000113003,
            UnexpectedStatePassthroughFB = -1000118000,
            FeatureAlreadyCreatedPassthroughFB = -1000118001,
            FeatureRequiredPassthroughFB = -1000118002,
            NotPermittedPassthroughFB = -1000118003,
            InsufficientResourcesPassthroughFB = -1000118004,
            UnknownPassthroughFB = -1000118050,
            RenderModelKeyInvalidFB = -1000119000,
            RenderModelUnavailableFB = 1000119020,
            MarkerNotTrackedVarjo = -1000124000,
            MarkerIdInvalidVarjo = -1000124001,
            SpatialAnchorNameNotFoundMSFT = -1000142001,
            SpatialAnchorNameInvalidMSFT = -1000142002,
            SpaceMappingInsufficientFB = -1000169000,
            SpaceLocalizationFailedFB = -1000169001,
            SpaceNetworkTimeoutFB = -1000169002,
            SpaceNetworkRequestFailedFB = -1000169003,
            SpaceCloudStorageDisabledFB = -1000169004,
            PassthroughColorLutBufferSizeMismatchMETA = -1000266000,
            HintAlreadySetQCOM = -1000306000,
            SpaceNotLocatableEXT = -1000429000,
            PlaneDetectionPermissionDeniedEXT = -1000429001,
            FuturePendingEXT = -1000469001,
            FutureInvalidEXT = -1000469002,
            FacialExpressionPermissionDeniedML = 1000482000,
            LocalizationMapIncompatibleML = -1000139000,
            LocalizationMapUnavailableML = -1000139001,
            LocalizationMapFailML = -1000139002,
            LocalizationMapImportExportPermissionDeniedML = -1000139003,
            LocalizationMapPermissionDeniedML = -1000139004,
            LocalizationMapAlreadyExistsML = -1000139005,
            LocalizationMapCannotExportCloudMapML = -1000139006,
            MarkerDetectorPermissionDeniedML = -1000138000,
            MarkerDetectorLocateFailedML = -1000138001,
            MarkerDetectorInvalidDataQueryML = -1000138002,
            MarkerDetectorInvalidCreateInfoML = -1000138003,
            MarkerInvalidML = -1000138004,
            SpatialAnchorsPermissionDeniedML = -1000140000,
            SpatialAnchorsNotLocalizedML = -1000140001,
            SpatialAnchorsOutOfMapBoundsML = -1000140002,
            SpatialAnchorsAnchorNotFoundML = -1000141000,
            SystemNotificationPermissionDeniedML = -1000473000,
            SystemNotificationIncompatibleSKUML = -1000473001,
            WorldMeshDetectorPermissionDeniedML = -1000474000,
            WorldMeshDetectorSpaceNotLocatableML = -1000474001,
            PixelSensorPermissionDeniedML = -1000475000,
            PixelSensorNotSupportedML = -1000475001,
            PixelSensorNotAvailableML = 1000475002,
            PixelSensorCapabilityNotSupportedML = 1000475003,
            PixelSensorSpaceNotSupportedML = 1000475004,
            LightEstimationPermissionDeniedML = -1000481000,
            LightEstimationRGBSensorNotAvailableML = -1000481001
        }

        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.XrConstants")]

        internal static class XrConstants
        {
            internal const ulong NullHandle = 0U;
            internal const long NoDuration = 0U;
            internal const long InfiniteDuration = 0x7fffffffffffffffL;
        }

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
            {
                string logResult = result.ToString();
                if (!Enum.IsDefined(typeof(XrResult), result))
                {
                    logResult = ((MLXrResult)result).ToString();
                }
                MLPluginLog.ErrorFormat($"{functionName} in the Magic Leap API failed. Reason: {logResult} ");
            }
            return success;
        }
        
        internal static Vector3 ConvertBetweenUnityOpenXr(this Vector3 input)
        {
            return new Vector3(input.x, input.y, -input.z);
        }

        internal static Quaternion ConvertBetweenUnityOpenXr(this Quaternion quaternion)
        {
            return new Quaternion(-quaternion.x, -quaternion.y, quaternion.z, quaternion.w);
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
    }
}
