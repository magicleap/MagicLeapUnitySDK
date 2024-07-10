using System.Text;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapPixelSensorFeature
    {
        /// <summary>
        /// The capability range for a sensor capability
        /// </summary>
        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.PixelSensorCapabilityRange")]
        public class PixelSensorCapabilityRange
        {
            /// <summary>
            /// The type of the capability this range is for
            /// </summary>
            public PixelSensorCapabilityType CapabilityType { get; internal set; }
            
            /// <summary>
            /// The stream index this range is for
            /// </summary>
            public uint Stream { get; internal set; }

            /// <summary>
            /// The range type for this capability
            /// </summary>
            public PixelSensorCapabilityRangeType RangeType { get; internal set; }
            
            /// <summary>
            /// The data type for this capability
            /// </summary>
            public PixelSensorCapabilityDataType DataType { get; internal set; }
            
            /// <summary>
            /// Whether the range is valid or not. This is used when configuring a sensor
            /// <para>Use this to filter out invalid configurations from being applied to a sensor</para>
            /// </summary>
            public bool IsValid { get; internal set; }
            
            /// <summary>
            /// The array of discrete booleans
            /// </summary>
            public bool[] BoolValues { get; internal set; }
            
            /// <summary>
            /// The array of discrete uints
            /// </summary>
            public uint[] IntValues { get; internal set; }
            
            /// <summary>
            /// The array of discrete extents
            /// </summary>
            public Vector2Int[] ExtentValues { get; internal set; }
            
            /// <summary>
            /// The IntRange with a min and max
            /// </summary>
            public PixelSensorCapabilityMinMax<uint>? IntRange { get; internal set; }
            
            /// <summary>
            /// The float range with a min and max
            /// </summary>
            public PixelSensorCapabilityMinMax<float>? FloatRange { get; internal set; }
            
            /// <summary>
            /// The array of frame formats
            /// </summary>
            public PixelSensorFrameFormat[] FrameFormats { get; internal set; }
            
            /// <summary>
            /// The array of auto exposure modes
            /// </summary>
            public PixelSensorAutoExposureMode[] ExposureModes { get; internal set; }
            
            public override string ToString()
            {
                StringBuilder str = new();

                void CheckAndAppendArr<T>(in T[] arr)
                {
                    if (arr != null)
                    {
                        str.Append($"[{string.Join(',', arr)}]");
                    }
                }

                str.Append($" Stream: {Stream}; For {CapabilityType}, DataType: {DataType}, RangeType: {RangeType}, IsValid: {IsValid}, Value(s): ");

                CheckAndAppendArr(BoolValues);
                CheckAndAppendArr(IntValues);
                CheckAndAppendArr(ExtentValues);
                CheckAndAppendArr(ExposureModes);
                CheckAndAppendArr(FrameFormats);

                if (IntRange != null)
                {
                    str.Append($"{IntRange}");
                }

                if (FloatRange != null)
                {
                    str.Append($"{FloatRange}");
                }

                return str.ToString();
            }
            
            /// <summary>
            /// Get a default configuration from the given range for this capability
            /// </summary>
            /// <param name="streamIndex">The stream to get the config for</param>
            /// <returns>The default config</returns>
            public PixelSensorConfigData GetDefaultConfig(uint streamIndex)
            {
                void SetDefaultValue<T>(out T? target, T[] values) where T : unmanaged
                {
                    if (values != null)
                    {
                        target = values[0];
                    }
                    else
                    {
                        target = null;
                    }
                }

                var result = new PixelSensorConfigData(CapabilityType, streamIndex);
                SetDefaultValue(out result.BoolValue, BoolValues);
                SetDefaultValue(out result.IntValue, IntValues);
                SetDefaultValue(out result.VectorValue, ExtentValues);
                SetDefaultValue(out result.ExposureMode, ExposureModes);
                SetDefaultValue(out result.FrameFormat, FrameFormats);

                if (FloatRange != null)
                {
                    result.FloatValue = FloatRange.Value.Min;
                }

                if (IntRange != null)
                {
                    result.IntValue = IntRange.Value.Min;
                }

                return result;
            }
            
            /// <summary>
            /// Represents a range of values ranging from Min to Max
            /// </summary>
            /// <typeparam name="T">The type of the range</typeparam>
            public struct PixelSensorCapabilityMinMax<T>
            {
                public T Min { get; internal set; }

                public T Max { get; internal set; }

                public override string ToString()
                {
                    return $"Min: {Min}, Max: {Max}";
                }
            }
        }
    }
}
