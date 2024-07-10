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
using System.Globalization;
using System.Linq;
using MagicLeap.OpenXR.NativeDelegates;
using MagicLeap.OpenXR.Spaces;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.XR.OpenXR.NativeTypes;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace MagicLeap.OpenXR.Features.PixelSensors
{
    /// <summary>
    /// Enables the Magic Leap OpenXR Loader for Android, and modifies the AndroidManifest to be compatible with ML2.
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(
        UiName = "Magic Leap 2 Pixel Sensor", 
        Desc = "Necessary to obtain data from pixel sensors available in a Magic Leap device", 
        Company = "Magic Leap", 
        Version = "1.0.0", 
        Priority = -2, 
        FeatureId = FeatureId, 
        BuildTargetGroups = new[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        OpenxrExtensionStrings = "XR_ML_pixel_sensor XR_EXT_future"
    )]
#endif
    public partial class MagicLeapPixelSensorFeature : MagicLeapOpenXRFeatureWithInterception<MagicLeapPixelSensorFeature>
    {
        public event Action<PixelSensorId, bool> OnSensorAvailabilityChanged;
        
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_pixel_sensor";
        
        private readonly Dictionary<PixelSensorId, PixelSensor> connectedSensors = new();
        private readonly HashSet<PixelSensorId> supportedSensors = new();
        private PixelSensorNativeFunctions nativeFunctions;
        
        internal SpacesNativeFunctions SpaceInfoNativeFunctions;
        private bool pathsFetched;

        private readonly HashSet<PixelSensorId> unavailableSensors = new();
        private PlayerLoopSystem pixelSensorFeatureLoop;

        public HashSet<PixelSensorCapabilityType> RequiredCapabilities { get; } = new()
        {
            PixelSensorCapabilityType.Resolution,
            PixelSensorCapabilityType.Format,
            PixelSensorCapabilityType.UpdateRate
        };

        protected override bool UsesExperimentalExtensions => true;

        protected override void OnInstanceDestroy(ulong xrInstance)
        {
            base.OnInstanceDestroy(xrInstance);
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopUtil.RemoveFromPlayerLoop(ref playerLoop, pixelSensorFeatureLoop, PlayerLoopUtil.InstallPath);
            
            foreach (var (_, sensor) in connectedSensors)
            {
                sensor.DestroyPixelSensor();
            }
            unavailableSensors.Clear();
            connectedSensors.Clear();
            pathsFetched = false;
        }

        protected override void MarkFunctionsToIntercept()
        {
            InterceptPollEvent = true;
        }

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            var creationResult = base.OnInstanceCreate(xrInstance);
            if (!creationResult)
            {
                return false;
            }

            nativeFunctions = CreateNativeFunctions<PixelSensorNativeFunctions>();
            SpaceInfoNativeFunctions = CreateNativeFunctions<SpacesNativeFunctions>();
            
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            pixelSensorFeatureLoop = new PlayerLoopSystem
            {
                type = typeof(PixelSensorUpdateType),
                subSystemList = Array.Empty<PlayerLoopSystem>(),
                updateDelegate = PixelSensorTick
            };
            if (!PlayerLoopUtil.InstallIntoPlayerLoop(ref playerLoop, pixelSensorFeatureLoop, PlayerLoopUtil.InstallPath))
            {
                Debug.LogError("Unable to install Pixel Sensor Player Loop");
                return false;
            }

            PlayerLoop.SetPlayerLoop(playerLoop);
            return true;
        }

        internal override XrResult OnPollEvent(ulong instance, IntPtr eventBuffer, XrPollEvent originalPollEvent)
        {
            var result = base.OnPollEvent(instance, eventBuffer, originalPollEvent);
            if (result != XrResult.Success)
            {
                return result;
            }
            unsafe
            {
                var eventData = (XrEventBuffer*)eventBuffer;
                if (eventData->Type != (ulong)XrPixelSensorStructTypes.XrTypeEventDataPixelSensorAvailabilityChangedML)
                {
                    return result;
                }
                var sensorEventData = (XrEventDataPixelSensorAvailabilityChanged*)eventBuffer;
                SensorAvailabilityChanged(in *sensorEventData);
            }
            return result;
        }

        private void SensorAvailabilityChanged(in XrEventDataPixelSensorAvailabilityChanged eventData)
        {
            var sensorId = new PixelSensorId
            {
                XrPath = eventData.Sensor,
            };
            var isAvailable = eventData.Available > 0;
            unavailableSensors.Remove(sensorId);
            if (!isAvailable)
            {
                unavailableSensors.Add(sensorId);
            }
            OnSensorAvailabilityChanged?.Invoke(sensorId, isAvailable);
        }
        
        private void PixelSensorTick()
        {
            foreach (var (_, sensor) in connectedSensors)
            {
                if (!sensor.ShouldPoll)
                {
                    continue;
                }

                sensor.PollSensorStatus();
            }
        }

        internal static bool IsRangeTypeDiscrete(PixelSensorCapabilityRangeType rangeType)
        {
            return rangeType switch
            {
                PixelSensorCapabilityRangeType.Boolean => true,
                PixelSensorCapabilityRangeType.Continuous => false,
                PixelSensorCapabilityRangeType.Discrete => true,
                _ => throw new ArgumentOutOfRangeException(nameof(rangeType), rangeType, null)
            };
        }

        private bool IsSensorConnected(PixelSensorId sensorType, out PixelSensor sensor, bool shouldPrintLog = true)
        {
            if (connectedSensors.TryGetValue(sensorType, out sensor))
            {
                return true;
            }

            if (shouldPrintLog)
            {
                Debug.LogError($"{sensorType} was not connected");
            }

            return false;
        }

        private void GetAvailableSensorsInternal()
        {
            unsafe
            {
                var xrResult = nativeFunctions.XrEnumeratePixelSensors(AppSession, 0, out var sensorCoutOutput, null);
                if (!Utils.DidXrCallSucceed(xrResult, nameof(PixelSensorNativeFunctions.XrEnumeratePixelSensors)))
                {
                    return;
                }

                var sensorArray = new NativeArray<XrPath>((int)sensorCoutOutput, Allocator.Temp);
                xrResult = nativeFunctions.XrEnumeratePixelSensors(AppSession, sensorCoutOutput, out sensorCoutOutput, (XrPath*)sensorArray.GetUnsafePtr());
                if (!Utils.DidXrCallSucceed(xrResult, nameof(PixelSensorNativeFunctions.XrEnumeratePixelSensors)))
                {
                    return;
                }

                foreach (var sensor in sensorArray)
                {
                    var path = PathToString(sensor);
                    var sensorType = new PixelSensorId()
                    {
                        XrPath = sensor,
                        XrPathString = path,
                        SensorName = GetDisplayNameOfSensor(path),
                    };
                    supportedSensors.Add(sensorType);
                }

                pathsFetched = true;
            }
        }
        
        private static string GetDisplayNameOfSensor(string sensorPath)
        {
            var splitPath = sensorPath.Replace("/pixelsensor/", "").Split("/").Select(str => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str));
            var result = string.Join(" ", splitPath);
            return result;
        }

        private struct PixelSensorUpdateType
        {
        }
    }
}
