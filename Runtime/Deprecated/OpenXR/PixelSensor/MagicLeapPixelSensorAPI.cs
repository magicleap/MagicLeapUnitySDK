using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapPixelSensorFeature
    {
        /// <summary>
        /// Get all the supported sensors on the device.
        /// <para>Note: This will return all the sensors the device supports, not sensors that are currently available</para>
        /// </summary>
        /// <returns>List of the supported sensors on the platform</returns>
        public List<PixelSensorId> GetSupportedSensors()
        {
            Debug.Log("Trying to fetch available sensors!");
            if (!pathsFetched)
            {
                GetAvailableSensorsInternal();
            }

            return supportedSensors.Where(sensorType => !unavailableSensors.Contains(sensorType)).ToList();
        }
        
        /// <summary>
        /// Create a pixel sensor of the given type
        /// </summary>
        /// <param name="sensorType">The type of the sensor to create</param>
        /// <returns>True if the sensor was created</returns>
        public bool CreatePixelSensor(PixelSensorId sensorType)
        {
            if (!pathsFetched)
            {
                GetAvailableSensorsInternal();
            }

            if (!supportedSensors.Contains(sensorType))
            {
                Debug.LogError($"{sensorType} was not found.");
                return false;
            }

            var pixelSensorOperations = new PixelSensor(sensorType, sensorType, nativeFunctions, this);
            var creationResult = pixelSensorOperations.CreatePixelSensor();
            if (!creationResult)
            {
                return false;
            }

            connectedSensors[sensorType] = pixelSensorOperations;
            return true;
        }
        
        /// <summary>
        /// Destroys a created pixel sensor
        /// </summary>
        /// <param name="sensorType">The type of the sensor to create</param>
        /// <returns>True if the sensor was destroyed</returns>
        public bool DestroyPixelSensor(PixelSensorId sensorType)
        {
            if (!IsSensorConnected(sensorType, out var sensorOperation))
            {
                return false;
            }

            var result = sensorOperation.DestroyPixelSensor();
            if (result)
            {
                connectedSensors.Remove(sensorType);
            }
            return result;
        }
        
        /// <summary>
        /// Get all the capabilities that a pixel sensor supports
        /// </summary>
        /// <param name="sensorType">The type of the pixel sensor</param>
        /// <param name="streamIndex">The stream index for which to get the supported capabilities for</param>
        /// <param name="capabilities">The array of capabilities supported</param>
        /// <returns>True if the capabilities were fetched</returns>
        public bool GetPixelSensorCapabilities(PixelSensorId sensorType, uint streamIndex, out PixelSensorCapability[] capabilities)
        {
            capabilities = Array.Empty<PixelSensorCapability>();
            if (!IsSensorConnected(sensorType, out var sensorOperation))
            {
                return false;
            }
            sensorOperation.EnumerateSensorCapabilities(streamIndex, out capabilities);
            capabilities = capabilities.OrderByDescending(capability => IsCapabilityRequired(capability.CapabilityType)).ToArray();
            return true;
        }
        
        /// <summary>
        /// Query a particular capability of a sensor to get its valid range
        /// </summary>
        /// <param name="sensorType">The type of the sensor to query the capability for</param>
        /// <param name="capabilityType">The type of the capability to query</param>
        /// <param name="streamIndex">The stream for which to query the capability</param>
        /// <param name="capabilityRange">The range of the capability. Use the return value to determine if this value is accurate or not</param>
        /// <returns>True if the query was successful</returns>
        public bool QueryPixelSensorCapability(PixelSensorId sensorType, PixelSensorCapabilityType capabilityType, uint streamIndex, out PixelSensorCapabilityRange capabilityRange)
        {
            capabilityRange = default;
            return IsSensorConnected(sensorType, out var sensorOperation) && sensorOperation.QuerySensorCapability(capabilityType, streamIndex, out capabilityRange);
        }
        
        /// <summary>
        /// Get a configured value for a sensor.
        /// <para>A configured value is the value that is applied to the sensor by calling <see cref="ApplySensorConfig(UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapPixelSensorFeature.PixelSensorId,UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapPixelSensorFeature.PixelSensorConfigData)"/></para>
        /// </summary>
        /// <param name="sensorType">The type of the sensor</param>
        /// <param name="streamIndex">The stream index to get the configured value for</param>
        /// <param name="capabilityType">The type of the capability</param>
        /// <param name="result">The configured value</param>
        /// <returns>True if a valid configuration was found</returns>
        public bool GetConfiguredValueForSensor(PixelSensorId sensorType, uint streamIndex, PixelSensorCapabilityType capabilityType, out PixelSensorConfigData result)
        {
            result = null;
            if (!IsSensorConnected(sensorType, out var sensor))
            {
                return false;
            }
            return sensor.GetAppliedConfigForCapability(streamIndex, capabilityType, out result);
        }
        
        /// <summary>
        /// Gets the capability range type and the capability data type for a sensor's capability
        /// </summary>
        /// <param name="sensorType">The type of the sensor</param>
        /// <param name="streamIndex">The stream index to query the capability for</param>
        /// <param name="capabilityType">The capability type</param>
        /// <returns>The range type and the data type for the capability</returns>
        public (PixelSensorCapabilityRangeType, PixelSensorCapabilityDataType) GetPixelSensorRangeAndDataTypeForCapability(PixelSensorId sensorType, uint streamIndex, PixelSensorCapabilityType capabilityType)
        {
            if (!IsSensorConnected(sensorType, out var sensor))
            {
                return default;
            }

            return sensor.GetRangeAndDataType(capabilityType, streamIndex);
        }
        
        /// <summary>
        /// Apply a configuration to a sensor
        /// <para>To configure a sensor, capabilities have to be configured iteratively</para>
        /// Each applied configuration can impact other capabilities.
        /// <para>So, when applying a sensor config, query for the other capabilities that have not been applied to get the updated range and to check if this applied config can configure the sensor</para>
        /// <para>Note: Every subsequent application of the same capability for the same stream will overwrite the previous value</para>
        /// </summary>
        /// <param name="sensorType">The type of the sensor</param>
        /// <param name="configData">The config data to apply</param>
        public void ApplySensorConfig(PixelSensorId sensorType, PixelSensorConfigData configData)
        {
            if (!IsSensorConnected(sensorType, out var sensorOperation))
            {
                return;
            }

            sensorOperation.ApplySensorConfig(configData);
        }
        
        /// <summary>
        /// An overloaded ApplySensorConfig that applies a float value for a capability
        /// </summary>
        /// <param name="sensorType">The type of the sensor</param>
        /// <param name="capabilityType">The capability type</param>
        /// <param name="value">The float value to be applied</param>
        /// <param name="streamIndex">The stream index</param>
        public void ApplySensorConfig(PixelSensorId sensorType, PixelSensorCapabilityType capabilityType, float value, uint streamIndex)
        {
            var config = new PixelSensorConfigData(capabilityType, streamIndex)
            {
                FloatValue = value
            };
            ApplySensorConfig(sensorType, config);
        }
        
        /// <summary>
        /// An overloaded ApplySensorConfig that applies a bool value for a capability
        /// </summary>
        /// <param name="sensorType">The type of the sensor</param>
        /// <param name="capabilityType">The capability type</param>
        /// <param name="value">The bool value to be applied</param>
        /// <param name="streamIndex">The stream index</param>
        public void ApplySensorConfig(PixelSensorId sensorType, PixelSensorCapabilityType capabilityType, bool value, uint streamIndex)
        {
            var config = new PixelSensorConfigData(capabilityType, streamIndex)
            {
                BoolValue = value
            };
            ApplySensorConfig(sensorType, config);
        }
        
        /// <summary>
        /// An overloaded ApplySensorConfig that applies a unsigned int value for a capability
        /// </summary>
        /// <param name="sensorType">The type of the sensor</param>
        /// <param name="capabilityType">The capability type</param>
        /// <param name="value">The unsigned int value to be applied</param>
        /// <param name="streamIndex">The stream index</param>
        public void ApplySensorConfig(PixelSensorId sensorType, PixelSensorCapabilityType capabilityType, uint value, uint streamIndex)
        {
            var config = new PixelSensorConfigData(capabilityType, streamIndex)
            {
                IntValue = value
            };
            ApplySensorConfig(sensorType, config);
        }
        
        /// <summary>
        /// An overloaded ApplySensorConfig that applies a Vector2int value for a capability
        /// </summary>
        /// <param name="sensorType">The type of the sensor</param>
        /// <param name="capabilityType">The capability type</param>
        /// <param name="value">The Vector2Int value to be applied</param>
        /// <param name="streamIndex">The stream index</param>
        public void ApplySensorConfig(PixelSensorId sensorType, PixelSensorCapabilityType capabilityType, Vector2Int value, uint streamIndex)
        {
            var config = new PixelSensorConfigData(capabilityType, streamIndex)
            {
                VectorValue = value
            };
            ApplySensorConfig(sensorType, config);
        }
        
        /// <summary>
        /// Clears an applied config for a capability
        /// </summary>
        /// <param name="sensorType">The type of the sensor</param>
        /// <param name="capabilityType">The capability type</param>
        /// <param name="streamIndex">The stream index of the capability</param>
        public void ClearSensorConfig(PixelSensorId sensorType, PixelSensorCapabilityType capabilityType, uint streamIndex)
        {
            if (!IsSensorConnected(sensorType, out var sensorOperation))
            {
                return;
            }

            sensorOperation.ClearAppliedConfig(capabilityType, streamIndex);
        }
        
        /// <summary>
        /// Clears all the applied configs for a sensor
        /// </summary>
        /// <param name="sensorType">The type of the sensor</param>
        public void ClearAllAppliedConfigs(PixelSensorId sensorType)
        {
            if (!IsSensorConnected(sensorType, out var sensorOperation))
            {
                return;
            }

            sensorOperation.ClearAllAppliedConfigs();
        }
        
        /// <summary>
        /// Configures a sensor with default capabilities
        /// </summary>
        /// <param name="sensorType">The type of the sensor</param>
        /// <param name="streams">The streams to configure</param>
        /// <returns>An async operation object for this operation (See: <see cref="PixelSensorAsyncOperationResult"/>)</returns>
        public PixelSensorAsyncOperationResult ConfigureSensorWithDefaultCapabilities(PixelSensorId sensorType, params uint[] streams)
        {
            if (!IsSensorConnected(sensorType, out var sensorOperation))
            {
                return PixelSensorAsyncOperationResult.FailedOperationResult;
            }

            Debug.Log($"---Configuring {sensorType} with default configuration-----");

            foreach (var streamIndex in streams)
            {
                if (streamIndex >= sensorOperation.StreamCount)
                {
                    Debug.LogError($"Invalid stream index {streamIndex}");
                    return PixelSensorAsyncOperationResult.FailedOperationResult;
                }

                sensorOperation.EnumerateSensorCapabilities(streamIndex, out var capabilities);
                foreach (var capability in capabilities)
                {
                    if (sensorOperation.QuerySensorCapability(capability.CapabilityType, streamIndex, out var range))
                    {
                        if (!range.IsValid)
                        {
                            Debug.LogError($"Invalid Setting for {capability.CapabilityType}");
                            continue;
                        }

                        var config = range.GetDefaultConfig(streamIndex);
                        sensorOperation.ApplySensorConfig(config);
                    }
                    else
                    {
                        return PixelSensorAsyncOperationResult.FailedOperationResult;
                    }
                }
            }

            return sensorOperation.ConfigureSensor(streams);
        }
        
        /// <summary>
        /// Gets the number of streams supported by a sensor
        /// </summary>
        /// <param name="sensorType">The type of the sensor</param>
        /// <returns>The number of streams supported by the sensor</returns>

        public uint GetStreamCount(PixelSensorId sensorType)
        {
            return !IsSensorConnected(sensorType, out var sensorOperation) ? 0 : sensorOperation.GetStreamCount();
        }
        
        /// <summary>
        /// Returns if a given capability is required to configure a sensor
        /// </summary>
        /// <param name="capabilityType">The type of capability to check</param>
        /// <returns>True if the capability is required for a successful configuration of the sensor</returns>
        public bool IsCapabilityRequired(PixelSensorCapabilityType capabilityType) => RequiredCapabilities.Contains(capabilityType);


        #region Async Operations

        /// <summary>
        /// Start a configured sensor
        /// </summary>
        /// <param name="sensorType">The type of the sensor to start</param>
        /// <param name="streams"> The streams to start for the sensor</param>
        /// <param name="metaDataTypes">The types of the metadata for each stream</param>
        /// <returns>An awaitable operation result to indicate the progress of this operation. (See <see cref="PixelSensorAsyncOperationResult"/>)</returns>
        public PixelSensorAsyncOperationResult StartSensor(PixelSensorId sensorType, IEnumerable<uint> streams, Dictionary<uint, PixelSensorMetaDataType[]> metaDataTypes = null)
        {
            if (!IsSensorConnected(sensorType, out var sensor))
            {
                return PixelSensorAsyncOperationResult.FailedOperationResult;
            }

            return sensor.StartSensor(streams, metaDataTypes);
        }
        
        /// <summary>
        /// Configure a given sensor for the passed streams
        /// </summary>
        /// <param name="sensorType">The type of the sensor to configure</param>
        /// <param name="streams">The streams to configure</param>
        /// <returns>An awaitable operation result to indicate the progress of this operation. (See <see cref="PixelSensorAsyncOperationResult"/>)</returns>
        public PixelSensorAsyncOperationResult ConfigureSensor(PixelSensorId sensorType, IEnumerable<uint> streams)
        {
            if (!IsSensorConnected(sensorType, out var sensor))
            {
                return PixelSensorAsyncOperationResult.FailedOperationResult;
            }

            return sensor.ConfigureSensor(streams);
        }
        
        /// <summary>
        /// Stops a given sensor for the passed streams
        /// </summary>
        /// <param name="sensorType">The type of the sensor to stop</param>
        /// <param name="streams">The streams to stop</param>
        /// <returns>An awaitable operation result to indicate the progress of this operation. (See <see cref="PixelSensorAsyncOperationResult"/>)</returns>
        public PixelSensorAsyncOperationResult StopSensor(PixelSensorId sensorType, IEnumerable<uint> streams)
        {
            if (!IsSensorConnected(sensorType, out var sensor))
            {
                return PixelSensorAsyncOperationResult.FailedOperationResult;
            }

            return sensor.StopSensor(streams);
        }
        
        /// <summary>
        /// Get the sensor data of a started sensor
        /// </summary>
        /// <param name="sensorType">The type of the sensor to get the data for</param>
        /// <param name="streamIndex">The stream to get the data for</param>
        /// <param name="frame">The current frame data of the sensor</param>
        /// <param name="metaData">The metadata for this frame</param>
        /// <param name="allocator">An allocator handle that will be used to allocate the memory for the underlying raw data</param>
        /// <param name="timeOut">The expected timeout for the data fetch</param>
        /// <param name="shouldFlipTexture"> Whether to fip the obtained texture. The data stored in the sensor could be flipped. Pass in false if the raw data is needed</param>
        /// <returns>True if a frame was obtained</returns>
        public bool GetSensorData(PixelSensorId sensorType, uint streamIndex, out PixelSensorFrame frame, out PixelSensorMetaData[] metaData, Allocator allocator, long timeOut = 10, bool shouldFlipTexture = true)
        {
            frame = default;
            metaData = Array.Empty<PixelSensorMetaData>();
            if (!IsSensorConnected(sensorType, out var sensor))
            {
                return false;
            }

            if (sensor.Status != PixelSensorStatus.Started)
            {
                Debug.LogError($"{sensorType} was not started. Please start the sensor to get its data");
                return false;
            }

            if (sensor.StreamStatus[streamIndex] != PixelSensorStatus.Started)
            {
                Debug.LogError($"{streamIndex} was not started. Please start the stream of this sensor to get its data");
                return false;
            }

            unsafe
            {
                if (!sensor.GetSensorData(streamIndex, out var sensorFrameInternal, out metaData, allocator, timeOut, shouldFlipTexture))
                {
                    return false;
                }

                frame.CreateFromNativeFrame(sensorFrameInternal, allocator, shouldFlipTexture);
                return true;
            }
        }
        
        /// <summary>
        /// Get the status of the sensor
        /// </summary>
        /// <param name="sensorType">The type of the sensor</param>
        /// <returns>The status of the sensor</returns>
        public PixelSensorStatus GetSensorStatus(PixelSensorId sensorType)
        {
            return !IsSensorConnected(sensorType, out var sensor, false) ? PixelSensorStatus.Undefined : sensor.Status;
        }
        
        /// <summary>
        /// Get the sensor pose
        /// </summary>
        /// <param name="sensorType">The type of the sensor</param>
        /// <param name="offset">The offset that should be considered for the pose calculation</param>
        /// <returns>The pose of the sensor</returns>
        public Pose GetSensorPose(PixelSensorId sensorType, Pose offset = default)
        {
            if (!IsSensorConnected(sensorType, out var sensor))
            {
                return default;
            }

            return sensor.GetSensorPose(offset);
        }
        
        /// <summary>
        /// The sensor data stored can have differing orientations.
        /// </summary>
        /// <param name="sensorType">The type of the sensor to get the rotation for</param>
        /// <returns>The rotation of the frame data</returns>
        public Quaternion GetSensorFrameRotation(PixelSensorId sensorType)
        {
            return sensorType.XrPathString switch
            {
                "/pixelsensor/eye/nasal/left" or "/pixelsensor/eye/nasal/right" => Quaternion.Euler(0, 0, 90),
                "/pixelsensor/eye/temple/left" or "/pixelsensor/eye/temple/right" => Quaternion.Euler(0, 0, -90),
                _ => Quaternion.identity
            };
        }
        
        /// <summary>
        /// Gets the available metadata types for a sensor's stream.
        /// <para>Note: A sensor has to be configured in order to be able to get its supported metadata types</para>
        /// </summary>
        /// <param name="sensorType">The type of the sensor</param>
        /// <param name="streamIndex">The stream to get the metadata for</param>
        /// <param name="metaDataTypes">The types of the metadata that are supported</param>
        /// <returns>True if the supported metadata types were fetched</returns>
        public bool EnumeratePixelSensorMetaDataTypes(PixelSensorId sensorType, uint streamIndex, out PixelSensorMetaDataType[] metaDataTypes)
        {
            metaDataTypes = Array.Empty<PixelSensorMetaDataType>();
            if (!IsSensorConnected(sensorType, out var sensor))
            {
                return false;
            }

            return sensor.EnumeratePixelSensorMetadata(streamIndex, out metaDataTypes);
        }

        #endregion
    }
}
