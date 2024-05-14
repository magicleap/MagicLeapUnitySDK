﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.LowLevel;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    using global::MagicLeap;
    /// <summary>
    ///     Enables the Magic Leap OpenXR Loader for Android, and modifies the AndroidManifest to be compatible with ML2.
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Pixel Sensor", Desc = "Necessary to obtain data from pixel sensors available in a Magic Leap device", Company = "Magic Leap", Version = "1.0.0", Priority = -2, FeatureId = FeatureId, BuildTargetGroups = new[]
    {
        BuildTargetGroup.Android,
        BuildTargetGroup.Standalone
    }, OpenxrExtensionStrings = PixelSensorExtensionName)]
#endif
    public partial class MagicLeapPixelSensorFeature : MagicLeapOpenXRFeatureWithEvents<MagicLeapPixelSensorFeature>
    {
        public event Action<PixelSensorId, bool> OnSensorAvailabilityChanged;
        
        private const string FeatureId = "com.magicleap.openxr.feature.ml2_pixel_sensor";
        private const string PixelSensorExtensionName = "XR_ML_pixel_sensor XR_EXT_future";
        
        private readonly Dictionary<PixelSensorId, PixelSensor> connectedSensors = new();
        private readonly HashSet<PixelSensorId> supportedSensors = new();
        private PixelSensorNativeFunctions nativeFunctions;
        private MagicLeapSpaceInfoNativeFunctions spaceInfoNativeFunctions;
        private bool pathsFetched;

        private readonly HashSet<PixelSensorId> unavailableSensors = new();
        private PlayerLoopSystem pixelSensorFeatureLoop;

        public HashSet<PixelSensorCapabilityType> RequiredCapabilities { get; } = new()
        {
            PixelSensorCapabilityType.Resolution,
            PixelSensorCapabilityType.Format,
            PixelSensorCapabilityType.UpdateRate
        };
        
        protected override void OnInstanceDestroy(ulong xrInstance)
        {
            base.OnInstanceDestroy(xrInstance);
            var playerLoop = LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopUtil.RemoveFromPlayerLoop(ref playerLoop, pixelSensorFeatureLoop, PlayerLoopUtil.InstallPath);
            
            foreach (var (_, sensor) in connectedSensors)
            {
                sensor.DestroyPixelSensor();
            }
            unavailableSensors.Clear();
            connectedSensors.Clear();
            pathsFetched = false;
        }

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            var creationResult = base.OnInstanceCreate(xrInstance);
            if (!creationResult)
            {
                return false;
            }

            nativeFunctions = CreateNativeFunctions<PixelSensorNativeFunctions>();
            spaceInfoNativeFunctions = CreateNativeFunctions<MagicLeapSpaceInfoNativeFunctions>();
            
            var playerLoop = LowLevel.PlayerLoop.GetCurrentPlayerLoop();
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

            LowLevel.PlayerLoop.SetPlayerLoop(playerLoop);
            return true;
        }

        internal override void OnPollEvent(IntPtr eventBuffer)
        {
            base.OnPollEvent(eventBuffer);
            unsafe
            {
                var eventData = (XrEventBuffer*)eventBuffer;
                if (eventData->Type != (ulong)XrPixelSensorStructTypes.XrTypeEventDataPixelSensorAvailabilityChangedML)
                {
                    return;
                }

                var sensorEventData = (XrEventDataPixelSensorAvailabilityChanged*)eventBuffer;
                SensorAvailabilityChanged(in *sensorEventData);
            }
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
        

        private static bool IsRangeTypeDiscrete(PixelSensorCapabilityRangeType rangeType)
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
                var xrResult = nativeFunctions.XrEnumeratePixelSensors(XRSession, 0, out var sensorCoutOutput, null);
                if (!Utils.DidXrCallSucceed(xrResult, nameof(PixelSensorNativeFunctions.XrEnumeratePixelSensors)))
                {
                    return;
                }

                var sensorArray = new NativeArray<ulong>((int)sensorCoutOutput, Allocator.Temp);
                xrResult = nativeFunctions.XrEnumeratePixelSensors(XRSession, sensorCoutOutput, out sensorCoutOutput, (ulong*)sensorArray.GetUnsafePtr());
                if (!Utils.DidXrCallSucceed(xrResult, nameof(PixelSensorNativeFunctions.XrEnumeratePixelSensors)))
                {
                    return;
                }

                foreach (var sensor in sensorArray)
                {
                    var sensorType = new PixelSensorId()
                    {
                        XrPath = sensor,
                        SensorName = GetDisplayNameOfSensor(PathToString(sensor))
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
        
        /// <summary>
        /// Represents the progress of an async Pixel sensor operation
        /// <para>Can be yielded inside a coroutine or has events that can be subscribed to</para>
        /// </summary>
        public class PixelSensorAsyncOperationResult : CustomYieldInstruction
        {
            private static PixelSensorAsyncOperationResult FailedOperation;
            public bool DidOperationFinish { get; private set; }
            public bool DidOperationFail { get; private set; }
            public bool DidOperationSucceed => DidOperationFinish && !DidOperationFail;

            public override bool keepWaiting => !DidOperationFinish;

            internal static PixelSensorAsyncOperationResult FailedOperationResult
            {
                get
                {
                    FailedOperation ??= new PixelSensorAsyncOperationResult();
                    FailedOperation.DidOperationFail = true;
                    FailedOperation.DidOperationFinish = true;
                    return FailedOperation;
                }
            }

            public event Action<PixelSensorId, PixelSensorStatus, IReadOnlyList<uint>> OnOperationFinishedSuccessfully;
            public event Action<PixelSensorId, PixelSensorStatus, IReadOnlyList<uint>> OnOperationStarted;
            public event Action<PixelSensorId, PixelSensorStatus, IReadOnlyList<uint>> OnOperationFailed;

            internal void StartOperation(PixelSensorId sensorType, PixelSensorStatus status, IReadOnlyList<uint> streams)
            {
                DidOperationFinish = false;
                DidOperationFail = false;
                OnOperationStarted?.Invoke(sensorType, status, streams);
            }

            internal void OperationFailed(PixelSensorId sensorType, PixelSensorStatus status, IReadOnlyList<uint> streams)
            {
                DidOperationFinish = true;
                DidOperationFail = true;
                OnOperationFailed?.Invoke(sensorType, status, streams);
            }

            internal void FinishOperation(PixelSensorId sensorType, PixelSensorStatus outputStatus, IReadOnlyList<uint> streams)
            {
                DidOperationFinish = true;
                DidOperationFail = false;
                OnOperationFinishedSuccessfully?.Invoke(sensorType, outputStatus, streams);
            }
        }

        private struct PixelSensorUpdateType
        {
        }
    }
}
