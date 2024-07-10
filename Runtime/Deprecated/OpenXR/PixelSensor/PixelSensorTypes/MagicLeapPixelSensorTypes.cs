using System;
using System.Diagnostics;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapPixelSensorFeature
    {
        /// <summary>
        /// A Pixel Sensor Identifier
        /// </summary>
        public struct PixelSensorId : IEquatable<PixelSensorId>
        {
            /// <summary>
            /// The internal XrPath of the pixel sensor
            /// </summary>
            public ulong XrPath
            {
                get;
                internal set;
            }

            internal string XrPathString
            {
                get;
                set;
            }

            /// <summary>
            /// The name of the sensor
            /// </summary>
            public string SensorName
            {
                get;
                internal set;
            }
            
            public bool Equals(PixelSensorId other)
            {
                return XrPath == other.XrPath;
            }

            public override bool Equals(object obj)
            {
                return obj is PixelSensorId other && Equals(other);
            }

            public override int GetHashCode()
            {
                return XrPath.GetHashCode();
            }

            public override string ToString()
            {
                return SensorName;
            }

            public static implicit operator ulong(PixelSensorId sensor) => sensor.XrPath;
        }
        
        /// <summary>
        ///     The status of a sensor
        /// </summary>
        public enum PixelSensorStatus : uint
        {
            /// <summary>
            ///     Sensor has not been created
            /// </summary>
            Undefined,

            /// <summary>
            ///     Sensor has been created but not configured
            /// </summary>
            NotConfigured,

            /// <summary>
            ///     Sensor is being configured
            /// </summary>
            Configuring,

            /// <summary>
            ///     Sensor has been configured
            /// </summary>
            Configured,

            /// <summary>
            ///     Starting a configured sensor
            /// </summary>
            Starting,

            /// <summary>
            ///     Sensor has been started
            /// </summary>
            Started,

            /// <summary>
            ///     Stopping a sensor
            /// </summary>
            Stopping,

            /// <summary>
            ///     Sensor has been stopped
            /// </summary>
            Stopped
        }

        private static XrPixelSensorStructTypes GetStructTypeFromRangeAndData(PixelSensorCapabilityRangeType rangeType, PixelSensorCapabilityDataType dataType)
        {
            switch (rangeType)
            {
                case PixelSensorCapabilityRangeType.Continuous:
                    return dataType switch
                    {
                        PixelSensorCapabilityDataType.UnsignedInt32 => XrPixelSensorStructTypes.XrTypePixelSensorCapabilityRangeContinuousUint32ML,
                        PixelSensorCapabilityDataType.Float => XrPixelSensorStructTypes.XrTypePixelSensorCapabilityRangeContinuousFloatML,
                        _ => throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null)
                    };
                case PixelSensorCapabilityRangeType.Discrete:
                    return dataType switch
                    {
                        PixelSensorCapabilityDataType.Boolean => XrPixelSensorStructTypes.XrTypePixelSensorCapabilityRangeDiscreteXrBool32ML,
                        PixelSensorCapabilityDataType.UnsignedInt32 => XrPixelSensorStructTypes.XrTypePixelSensorCapabilityRangeDiscreteUint32ML,
                        PixelSensorCapabilityDataType.Extent2D => XrPixelSensorStructTypes.XrTypePixelSensorCapabilityRangeDiscreteXrExtent2DiML,
                        _ => throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null)
                    };
                case PixelSensorCapabilityRangeType.Boolean: return XrPixelSensorStructTypes.XrTypePixelSensorCapabilityRangeDiscreteXrBool32ML;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rangeType), rangeType, null);
            }
        }
        
        /// <summary>
        /// Represents the capability data's type
        /// </summary>
        public struct PixelSensorCapability
        {
            /// <summary>
            /// The capability type
            /// </summary>
            public PixelSensorCapabilityType CapabilityType;
            /// <summary>
            /// The data type of the capability
            /// </summary>
            public PixelSensorCapabilityDataType CapabilityDataType;
            /// <summary>
            /// The range type of the capability
            /// </summary>
            public PixelSensorCapabilityRangeType CapabilityRange;

            public override string ToString()
            {
                return $"{CapabilityType}, {CapabilityRange}, {CapabilityDataType}";
            }
        }
        
        /// <summary>
        /// Represents a configuration to be applied for a sensor
        /// </summary>
        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.PixelSensorConfigData")]
        public class PixelSensorConfigData
        {
            /// <summary>
            /// The capability type this config supplies the value for
            /// </summary>
            public PixelSensorCapabilityType CapabilityType
            {
                get;
            }
            
            /// <summary>
            /// The stream index this config supplies the value for
            /// </summary>
            public uint StreamIndex
            {
                get;
            }
            
            /// <summary>
            /// The boolean value.
            /// </summary>
            public bool? BoolValue;
            /// <summary>
            /// The exposure mode value
            /// </summary>
            public PixelSensorAutoExposureMode? ExposureMode;
            /// <summary>
            /// The floating point value
            /// </summary>
            public float? FloatValue;
            /// <summary>
            /// The frame format value
            /// </summary>
            public PixelSensorFrameFormat? FrameFormat;
            /// <summary>
            /// The integer value
            /// </summary>
            public uint? IntValue;
            /// <summary>
            /// The vector2 value
            /// </summary>
            public Vector2Int? VectorValue;
            
            /// <summary>
            /// Create a PixelSensorConfigData for the given capability and stream index
            /// </summary>
            /// <param name="capabilityType">The type of the capability this config will hold a value for</param>
            /// <param name="streamIndex">The stream index associated with this capability config</param>
            public PixelSensorConfigData(PixelSensorCapabilityType capabilityType, uint streamIndex)
            {
                CapabilityType = capabilityType;
                StreamIndex = streamIndex;
            }

            internal PixelSensorConfigData(PixelSensorConfigData source)
            {
                BoolValue = source.BoolValue;
                CapabilityType = source.CapabilityType;
                ExposureMode = source.ExposureMode;
                FloatValue = source.FloatValue;
                IntValue = source.IntValue;
                StreamIndex = source.StreamIndex;
                VectorValue = source.VectorValue;
            }

            internal PixelSensorCapabilityDataType ActiveDataType
            {
                get
                {
                    if (BoolValue.HasValue)
                    {
                        return PixelSensorCapabilityDataType.Boolean;
                    }

                    if (FloatValue.HasValue)
                    {
                        return PixelSensorCapabilityDataType.Float;
                    }

                    if (IntValue.HasValue || FrameFormat.HasValue || ExposureMode.HasValue)
                    {
                        return PixelSensorCapabilityDataType.UnsignedInt32;
                    }

                    if (VectorValue.HasValue)
                    {
                        return PixelSensorCapabilityDataType.Extent2D;
                    }

                    throw new ArgumentException("None of the values were set. Please set a value for the config");
                }
            }
            
            /// <summary>
            /// Clear all the assigned values
            /// </summary>
            public void ClearValues()
            {
                BoolValue = null;
                FloatValue = null;
                VectorValue = null;
                IntValue = null;
                FrameFormat = null;
                ExposureMode = null;
            }

            [Conditional("DEVELOPMENT_BUILD")]
            internal void ValidateConfig()
            {
                if (BoolValue != null || FloatValue != null || VectorValue != null || IntValue != null || FrameFormat != null || ExposureMode != null)
                {
                    return;
                }
                throw new NullReferenceException("None of the values of the config were set");
            }
        }
    }
}
