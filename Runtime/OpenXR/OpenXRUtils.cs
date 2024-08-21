// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MagicLeap.OpenXR.Constants;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.NativeTypes;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MagicLeap.OpenXR
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

            if (success || !showError)
            {
                return success;
            }

            var logResult = result.ToString();
            if (!Enum.IsDefined(typeof(XrResult), result))
            {
                logResult = ((MLXrResult)result).ToString();
            }
            MLPluginLog.ErrorFormat($"{functionName} in the Magic Leap API failed. Reason: {logResult} ");
            return false;
        }
        
        internal static Vector3 InvertZ(this Vector3 input)
        {
            return new Vector3(input.x, input.y, -input.z);
        }

        internal static Quaternion InvertXY(this Quaternion quaternion)
        {
            return new Quaternion(-quaternion.x, -quaternion.y, quaternion.z, quaternion.w);
        }
        
        /// <summary>
        /// An OpenXRStruct helper class
        /// </summary>
        /// <typeparam name="T">The type of the OpenXR struct</typeparam>
        internal static class OpenXRStructHelpers<T> where T : unmanaged 
        {
            
            /// <summary>
            /// Create an OpenXR struct with the type initialized
            /// </summary>
            /// <param name="type">The type of the OpenXr struct (that should be written to the Type field)</param>
            /// <param name="result">The resulting openxr struct</param>
            /// <param name="typeFieldName">The name of the Type field (default is "Type")</param>
            /// <param name="defaultValue">The value to be initialized with for the field</param>
            /// <typeparam name="TType">The underlying type of the field (can be ulong or an enum for example)</typeparam>
            internal static void Create<TType>(TType type, out T result, string typeFieldName = "Type", T defaultValue = default) 
                where TType: unmanaged
            {
                var nativeResult = defaultValue;
                unsafe
                {
                    var nativePtr = new IntPtr(&nativeResult);
                    var offset = Marshal.OffsetOf<T>(typeFieldName);
                    var sourceType = type;
                    UnsafeUtility.MemCpy((nativePtr + offset.ToInt32()).ToPointer(), &sourceType, sizeof(TType));
                    result = nativeResult;
                }
            }
        }
        
      
        
#if UNITY_EDITOR
        internal static string GetNiceTypeName(Type type)
            => ObjectNames.NicifyVariableName(type.Name);

        internal static OpenXRFeature.ValidationRule GetDepthSubmissionValidationRule(OpenXRFeature feature, BuildTargetGroup targetGroup, OpenXRSettings.DepthSubmissionMode validDepthSubmissionMode = OpenXRSettings.DepthSubmissionMode.Depth16Bit)
        {
            var depthSubmissionModeValidationRule = new OpenXRFeature.ValidationRule(feature)
            {
                message = $"{nameof(OpenXRSettings.DepthSubmissionMode)} must be set to {validDepthSubmissionMode} to support {GetNiceTypeName(feature.GetType())}",
                checkPredicate = () =>
                {
                    var settings = GetSettings(targetGroup);
                    return settings.depthSubmissionMode == validDepthSubmissionMode;
                },
                fixIt = () =>
                {
                    var settings = GetSettings(targetGroup);
                    settings.depthSubmissionMode = validDepthSubmissionMode;
                },
                error = true,
            };
            return depthSubmissionModeValidationRule;
        }
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
            return feature != null && (feature.enabled = true);
        }

        internal static bool TryEnableFeature<TFeature>(BuildTargetGroup group) where TFeature : OpenXRFeature
        {
            var feature = GetFeatureForBuildTarget<TFeature>(group);
            return feature != null && (feature.enabled = true);
        }
#endif // UNITY_EDITOR
    }
}
