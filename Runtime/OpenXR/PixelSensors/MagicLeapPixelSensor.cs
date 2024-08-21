using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Features.PixelSensors
{
    internal class PixelSensor
    {
        internal readonly List<PixelSensorConfigData> AppliedConfigs = new();
        internal readonly PixelSensorStreamStatus StreamStatus = new();
        internal PixelSensorStreamBuffers StreamBuffers;
        internal PixelSensorId SensorType { get; set; }
        private bool IsConnected { get; set; }
        public uint StreamCount { get; private set; }
        public bool ShouldPoll => activeOperations.Count > 0 || queuedOperations.Count > 0;
        public PixelSensorStatus Status { get; internal set; }


        private readonly List<PixelSensorAsyncOperation> activeOperations = new();
        private readonly List<PixelSensorAsyncOperation> queuedOperations = new();
        private XrSpace sensorSpace;

        private MagicLeapPixelSensorFeature PixelSensorFeature { get; }
        internal PixelSensorNativeFunctions NativeFunctions { get; }
        private ulong Path { get; }
        internal XrPixelSensor Handle { get; private set; }

        private bool IsSensorConfigured =>
            IsConnected && Status switch
            {
                PixelSensorStatus.Configured or PixelSensorStatus.Started or PixelSensorStatus.Stopped => true,
                _ => false
            };

        public PixelSensor(ulong path, PixelSensorId sensorType, PixelSensorNativeFunctions nativeFunctions, MagicLeapPixelSensorFeature pixelSensorFeature)
        {
            Path = path;
            NativeFunctions = nativeFunctions;
            PixelSensorFeature = pixelSensorFeature;
            SensorType = sensorType;
        }

        public bool CreatePixelSensor()
        {
            unsafe
            {
                Reset();
                var createInfo = new XrPixelSensorCreateInfo
                {
                    Type = XrPixelSensorStructTypes.XrTypePixelSensorCreateInfoML,
                    Next = default,
                    Sensor = Path
                };

                var xrResult = NativeFunctions.XrCreatePixelSensor(PixelSensorFeature.AppSession, ref createInfo, out var sensorHandle);
                if (!Utils.DidXrCallSucceed(xrResult, nameof(PixelSensorNativeFunctions.XrCreatePixelSensor)))
                {
                    return false;
                }

                Handle = sensorHandle;
                IsConnected = true;
                StreamCount = GetStreamCount();
                AllocateStreamBasedArrays();
                Status = PixelSensorStatus.NotConfigured;
                return true;
            }
        }

        public Pose GetSensorPose(Pose offset)
        {
            unsafe
            {
                var convertedOffsetPose = XrPose.GetFromPose(offset);
                if (sensorSpace == 0)
                {
                    var createSpaceInfo = new XrPixelSensorCreateSpaceInfo
                    {
                        Type = XrPixelSensorStructTypes.XrTypePixelSensorCreateSpaceInfoML,
                        Sensor = Handle,
                        Offset = convertedOffsetPose
                    };
                    var xrResult = NativeFunctions.XrCreatePixelSensorSpace(PixelSensorFeature.AppSession, ref createSpaceInfo, out sensorSpace);
                    if (!Utils.DidXrCallSucceed(xrResult, nameof(PixelSensorNativeFunctions.XrCreatePixelSensorSpace)))
                    {
                        return default;
                    }
                }

                var spaceInfoFunctions = PixelSensorFeature.SpaceInfoNativeFunctions;
                var pose = spaceInfoFunctions.GetUnityPose(sensorSpace, PixelSensorFeature.AppSpace, PixelSensorFeature.NextPredictedDisplayTime);
                return pose;
            }
        }

        private void AllocateStreamBasedArrays()
        {
            StreamBuffers = new PixelSensorStreamBuffers(this);
            StreamStatus.Initialize((int)StreamCount);
        }

        public bool DestroyPixelSensor()
        {
            unsafe
            {
                var xrResult = NativeFunctions.XrDestroyPixelSensor(Handle);
                if (!Utils.DidXrCallSucceed(xrResult, nameof(PixelSensorNativeFunctions.XrDestroyPixelSensor)))
                {
                    return false;
                }

                Reset();
                return true;
            }
        }

        public bool GetAppliedConfigForCapability(uint streamIndex, PixelSensorCapabilityType capabilityType, out PixelSensorConfigData configData)
        {
            configData = null;
            var config = AppliedConfigs.FirstOrDefault(appliedConfig => appliedConfig.StreamIndex == streamIndex && appliedConfig.CapabilityType == capabilityType);
            if (config == null)
            {
                return false;
            }

            configData = new PixelSensorConfigData(config);
            return true;
        }

        public uint GetStreamCount()
        {
            unsafe
            {
                var result = NativeFunctions.XrGetPixelSensorStreamCount(Handle, out var streamCount);
                return !Utils.DidXrCallSucceed(result, nameof(PixelSensorNativeFunctions.XrGetPixelSensorStreamCount)) ? default : streamCount;
            }
        }

        public void EnumerateSensorCapabilities(uint streamIndex, out PixelSensorCapability[] capabilities)
        {
            if (streamIndex >= StreamCount)
            {
                throw new IndexOutOfRangeException($"{SensorType} does not have {streamIndex} streams");
            }

            var nativeCapabilities = GetSupportedCapabilities(streamIndex);
            capabilities = new PixelSensorCapability[nativeCapabilities.Length];
            for (var i = 0; i < nativeCapabilities.Length; i++)
            {
                capabilities[i].CapabilityType = nativeCapabilities[i].CapabilityType;
                capabilities[i].CapabilityRange = nativeCapabilities[i].CapabilityRangeType;
                capabilities[i].CapabilityDataType = nativeCapabilities[i].CapabilityDataType;
            }
        }

        public void ApplySensorConfig(PixelSensorConfigData configData)
        {
            ClearAppliedConfig(configData.CapabilityType, configData.StreamIndex);
            configData.ValidateConfig();
            AppliedConfigs.Add(configData);
        }

        public void ClearAppliedConfig(PixelSensorCapabilityType capabilityType, uint streamIndex)
        {
            AppliedConfigs.RemoveAll(config => config.CapabilityType == capabilityType && config.StreamIndex == streamIndex);
        }

        public void ClearAllAppliedConfigs()
        {
            AppliedConfigs.Clear();
        }

        public unsafe bool GetSensorData(uint streamIndex, out XrPixelSensorFrame* sensorFrame, out PixelSensorMetaData[] metaData, Allocator allocator, long timeout, bool shouldFlipTexture)
        {
            bool FrameErrorResultCheck(XrResult result)
            {
                return result is XrResult.Success or XrResult.TimeoutExpored;
            }

            sensorFrame = null;
            metaData = Array.Empty<PixelSensorMetaData>();
            var buffer = StreamBuffers[streamIndex];
            var getInfo = new XrPixelSensorDataGetInfo
            {
                Type = XrPixelSensorStructTypes.XrTypePixelSensorDataGetInfoML,
                Stream = streamIndex,
                LastCaptureTime = buffer.LastCaptureTime,
                Timeout = timeout
            };
            var data = new XrPixelSensorData
            {
                Type = XrPixelSensorStructTypes.XrTypePixelSensorDataML,
                Next = IntPtr.Zero
            };
            var requestedMetaData = buffer.RequestedMetaDataTypes;

            var chainStart = &data.Next;
            var metaDataContainer = XrMetadataContainer.Create();
            foreach (var currentMetadata in requestedMetaData)
            {
                switch (currentMetadata)
                {
                    case PixelSensorMetaDataType.ExposureTime:
                        *chainStart = new IntPtr(&metaDataContainer.ExposureTime);
                        chainStart = &metaDataContainer.ExposureTime.Next;
                        break;
                    case PixelSensorMetaDataType.AnalogGain:
                        *chainStart = new IntPtr(&metaDataContainer.AnalogGain);
                        chainStart = &metaDataContainer.AnalogGain.Next;
                        break;
                    case PixelSensorMetaDataType.DigitalGain:
                        *chainStart = new IntPtr(&metaDataContainer.DigitalGain);
                        chainStart = &metaDataContainer.DigitalGain.Next;
                        break;
                    case PixelSensorMetaDataType.PinholeCameraModel:
                        *chainStart = new IntPtr(&metaDataContainer.PinholeIntrinsics);
                        chainStart = &metaDataContainer.PinholeIntrinsics.Next;
                        break;
                    case PixelSensorMetaDataType.FishEyeCameraModel:
                        *chainStart = new IntPtr(&metaDataContainer.FisheyeIntrinsics);
                        chainStart = &metaDataContainer.FisheyeIntrinsics.Next;
                        break;
                    case PixelSensorMetaDataType.DepthFrameIllumination:
                        *chainStart = new IntPtr(&metaDataContainer.DepthFrameIllumination);
                        chainStart = &metaDataContainer.DepthFrameIllumination.Next;
                        break;
                    case PixelSensorMetaDataType.DepthConfidenceBuffer:
                        *chainStart = new IntPtr(&metaDataContainer.DepthConfidenceBuffer);
                        chainStart = &metaDataContainer.DepthConfidenceBuffer.Next;
                        break;
                    case PixelSensorMetaDataType.DepthFlagBuffer:
                        *chainStart = new IntPtr(&metaDataContainer.DepthFlagBuffer);
                        chainStart = &metaDataContainer.DepthFlagBuffer.Next;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var xrResult = NativeFunctions.XrGetPixelSensorData(Handle, ref getInfo, out buffer.SensorBuffer, out data);
            buffer.LastCaptureTime = data.CaptureTime;
            if (!Utils.DidXrCallSucceed(xrResult, nameof(PixelSensorNativeFunctions.XrGetPixelSensorData), FrameErrorResultCheck))
            {
                return false;
            }

            sensorFrame = (XrPixelSensorFrame*)data.Frame;
            metaData = new PixelSensorMetaData[requestedMetaData.Count];
            for (var i = 0; i < requestedMetaData.Count; i++)
            {
                metaData[i] = requestedMetaData[i] switch
                {
                    PixelSensorMetaDataType.ExposureTime => metaDataContainer.ExposureTime.GetMetaData(allocator),
                    PixelSensorMetaDataType.AnalogGain => metaDataContainer.AnalogGain.GetMetaData(allocator),
                    PixelSensorMetaDataType.DigitalGain => metaDataContainer.DigitalGain.GetMetaData(allocator),
                    PixelSensorMetaDataType.PinholeCameraModel => metaDataContainer.PinholeIntrinsics.GetMetaData(allocator),
                    PixelSensorMetaDataType.FishEyeCameraModel => metaDataContainer.FisheyeIntrinsics.GetMetaData(allocator),
                    PixelSensorMetaDataType.DepthFrameIllumination => metaDataContainer.DepthFrameIllumination.GetMetaData(allocator),
                    PixelSensorMetaDataType.DepthConfidenceBuffer => metaDataContainer.DepthConfidenceBuffer.GetMetaData(allocator, shouldFlipTexture, data.CaptureTime),
                    PixelSensorMetaDataType.DepthFlagBuffer => metaDataContainer.DepthFlagBuffer.GetMetaData(allocator, shouldFlipTexture, data.CaptureTime),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return true;
        }

        private XrPixelSensorCapability[] GetSupportedCapabilities(uint streamIndex)
        {
            unsafe
            {
                var xrResult = NativeFunctions.XrEnumeratePixelSensorCapabilities(Handle, streamIndex, 0, out var capabilityCount, null);
                if (!Utils.DidXrCallSucceed(xrResult, nameof(NativeFunctions.XrEnumeratePixelSensorCapabilities)))
                {
                    return Array.Empty<XrPixelSensorCapability>();
                }

                var capabilities = new NativeArray<XrPixelSensorCapability>((int)capabilityCount, Allocator.Temp);
                for (var i = 0; i < capabilityCount; i++)
                {
                    var capability = capabilities[i];
                    capability.Type = XrPixelSensorStructTypes.XrTypePixelSensorCapabilityML;
                    capabilities[i] = capability;
                }

                xrResult = NativeFunctions.XrEnumeratePixelSensorCapabilities(Handle, streamIndex, capabilityCount, out capabilityCount, (XrPixelSensorCapability*)capabilities.GetUnsafePtr());
                if (!Utils.DidXrCallSucceed(xrResult, nameof(NativeFunctions.XrEnumeratePixelSensorCapabilities)))
                {
                    return Array.Empty<XrPixelSensorCapability>();
                }

                return capabilities.ToArray();
            }
        }

        public (PixelSensorCapabilityRangeType, PixelSensorCapabilityDataType) GetRangeAndDataType(PixelSensorCapabilityType capabilityType, uint streamIndex)
        {
            var capabilities = GetSupportedCapabilities(streamIndex);
            var targetCapability = capabilities.FirstOrDefault(capability => capability.CapabilityType == capabilityType);
            var result = (targetCapability.CapabilityRangeType, targetCapability.CapabilityDataType);
            return result;
        }

        public bool QuerySensorCapability(PixelSensorCapabilityType capabilityType, uint streamIndex, out PixelSensorCapabilityRange capabilityRange)
        {
            if (streamIndex >= StreamCount)
            {
                throw new IndexOutOfRangeException($"{SensorType} does not have {streamIndex} streams.");
            }

            var operationResult = false;
            capabilityRange = default;
            var queryInfo = new XrPixelSensorCapabilityQueryInfo
            {
                Type = XrPixelSensorStructTypes.XrTypePixelSensorCapabilityQueryInfoML,
                Next = default,
                Stream = streamIndex,
                CapabilityType = capabilityType
            };
            var (capabilityRangeType, capabilityDataType) = GetRangeAndDataType(capabilityType, streamIndex);
            unsafe
            {
                var appliedConfigurations = AppliedConfigs.ToArray();
                var configArray = new NativeArray<XrPixelSensorConfigData>(appliedConfigurations.Length, Allocator.Temp);
                var configHeaders = new NativeArray<IntPtr>(appliedConfigurations.Length, Allocator.Temp);
                using var rangeHolder = new XrPixelSensorCapabilityRangeHolder(capabilityRangeType, capabilityDataType, capabilityType);
                if (appliedConfigurations.Length > 0)
                {
                    for (var i = 0; i < appliedConfigurations.Length; i++)
                    {
                        var currentConfig = new XrPixelSensorConfigData();
                        currentConfig.AssignFromConfig(appliedConfigurations[i]);
                        configHeaders[i] = currentConfig.GetBaseHeader();
                        configArray[i] = currentConfig;
                    }
                }

                var xrResult = NativeFunctions.XrQueryPixelSensorCapabilityRange(Handle, ref queryInfo, (uint)configArray.Length, (IntPtr*)configHeaders.GetUnsafePtr(), rangeHolder.GetBaseHeader());
                if (Utils.DidXrCallSucceed(xrResult, $"{nameof(PixelSensorNativeFunctions.XrQueryPixelSensorCapabilityRange)}WithoutBuffer"))
                {
                    if (MagicLeapPixelSensorFeature.IsRangeTypeDiscrete(capabilityRangeType))
                    {
                        //If it is a discrete type then we have to call the buffer again
                        xrResult = NativeFunctions.XrQueryPixelSensorCapabilityRange(Handle, ref queryInfo, (uint)configArray.Length, (IntPtr*)configHeaders.GetUnsafePtr(), rangeHolder.GetBaseHeaderForDiscreteValues());
                        if (!Utils.DidXrCallSucceed(xrResult, nameof(PixelSensorNativeFunctions.XrQueryPixelSensorCapabilityRange)))
                        {
                        }
                    }

                    capabilityRange = rangeHolder.GetRangeFromInternal(streamIndex);
                    operationResult = true;
                }

                //Free allocated buffers
                for (var i = 0; i < configArray.Length; i++)
                {
                    configArray[i].Dispose();
                }

                return operationResult;
            }
        }

        private T GetOperation<T>() where T : PixelSensorAsyncOperation, new()
        {
            var operation = new T
            {
                NativeFunctions = NativeFunctions,
                PixelSensorFeature = PixelSensorFeature,
                Sensor = this
            };
            queuedOperations.Add(operation);
            return operation;
        }

        public void PollSensorStatus()
        {
            var finishedOperations = new HashSet<PixelSensorAsyncOperation>();
            foreach (var operation in activeOperations)
            {
                if (operation.DidOperationFinish)
                {
                    finishedOperations.Add(operation);
                    continue;
                }

                operation.PollOperation();
            }

            activeOperations.RemoveAll(operation => finishedOperations.Contains(operation));
            //If there are still active operations performed, finish them first
            if (activeOperations.Count != 0)
            {
                return;
            }

            activeOperations.AddRange(queuedOperations);
            queuedOperations.Clear();
        }

        private PixelSensorMetaDataType[] GetSupportedMetadata(uint streamIndex)
        {
            var result = Array.Empty<PixelSensorMetaDataType>();
            unsafe
            {
                var xrResult = NativeFunctions.XrEnumeratePixelSensorMetadata(Handle, streamIndex, 0, out var metaDataCount, null);
                if (!Utils.DidXrCallSucceed(xrResult, nameof(NativeFunctions.XrEnumeratePixelSensorMetadata)))
                {
                    return result;
                }

                var metaDataTypes = new NativeArray<PixelSensorMetaDataType>((int)metaDataCount, Allocator.Temp);
                xrResult = NativeFunctions.XrEnumeratePixelSensorMetadata(Handle, streamIndex, metaDataCount, out metaDataCount, (PixelSensorMetaDataType*)metaDataTypes.GetUnsafePtr());
                if (!Utils.DidXrCallSucceed(xrResult, nameof(NativeFunctions.XrEnumeratePixelSensorMetadata)))
                {
                    return result;
                }

                return metaDataTypes.ToArray();
            }
        }

        public bool EnumeratePixelSensorMetadata(uint streamIndex, out PixelSensorMetaDataType[] metaDataTypes)
        {
            metaDataTypes = Array.Empty<PixelSensorMetaDataType>();
            if (!IsSensorConfigured)
            {
                return false;
            }

            metaDataTypes = GetSupportedMetadata(streamIndex);
            return true;
        }

        public bool IsMetaDataTypeSupported(PixelSensorMetaDataType metaDataType, uint streamIndex)
        {
            var supportedMetaDataTypes = GetSupportedMetadata(streamIndex);
            return supportedMetaDataTypes.Any(metaData => metaData == metaDataType);
        }


        private void Reset()
        {
            sensorSpace = 0;
            Status = PixelSensorStatus.Undefined;
            IsConnected = false;
            AppliedConfigs.Clear();
            activeOperations.Clear();
            queuedOperations.Clear();
            StreamBuffers?.Dispose();
        }

        public class PixelSensorStreamStatus
        {
            private readonly Dictionary<uint, PixelSensorStatus> statusTable = new();

            public PixelSensorStatus this[uint stream]
            {
                get
                {
                    if (statusTable.TryGetValue(stream, out var status))
                    {
                        return status;
                    }

                    statusTable[stream] = PixelSensorStatus.Undefined;
                    return PixelSensorStatus.Undefined;
                }

                set => statusTable[stream] = value;
            }

            public void SetStatusForStreams(PixelSensorStatus status, IEnumerable<uint> streams)
            {
                foreach (var stream in streams)
                {
                    statusTable[stream] = status;
                }
            }

            public void Initialize(int streamCount, PixelSensorStatus startingStatus = PixelSensorStatus.Undefined)
            {
                for (var i = 0U; i < streamCount; i++)
                {
                    statusTable[i] = startingStatus;
                }
            }
        }

        public PixelSensorAsyncOperationResult StartSensor(IEnumerable<uint> streams, Dictionary<uint, PixelSensorMetaDataType[]> metaDataTypes)
        {
            //If we are trying to start streams that have not been configured
            var streamArr = streams as uint[] ?? streams.ToArray();
            var startOperation = GetOperation<PixelSensorStartOperation>();
            if (metaDataTypes != null)
            {
                foreach (var (stream, requestedMetadata) in metaDataTypes)
                {
                    if (requestedMetadata == null)
                    {
                        continue;
                    }

                    startOperation.MetadataTypesForStream.Remove(stream);
                    startOperation.MetadataTypesForStream.Add(stream, new HashSet<PixelSensorMetaDataType>(requestedMetadata));
                }
            }

            startOperation.Start(streamArr);
            return startOperation.OperationResult;
        }

        public PixelSensorAsyncOperationResult ConfigureSensor(IEnumerable<uint> streams)
        {
            var configOperation = GetOperation<PixelSensorConfigureOperation>();
            configOperation.Start(streams);
            return configOperation.OperationResult;
        }

        public PixelSensorAsyncOperationResult StopSensor(IEnumerable<uint> streams)
        {
            var stopOperation = GetOperation<PixelSensorStopOperation>();
            stopOperation.Start(streams);
            return stopOperation.OperationResult;
        }
    }
}
