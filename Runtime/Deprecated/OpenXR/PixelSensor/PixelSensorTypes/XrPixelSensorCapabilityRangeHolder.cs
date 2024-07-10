using System;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.MagicLeap.Unsafe;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapPixelSensorFeature
    {
        /// <summary>
        /// Represents a container for holding on to a capability range
        /// </summary>
        internal struct XrPixelSensorCapabilityRangeHolder : IDisposable
        {
            private IntPtr internalBuffer;
            private IntPtr discreteBuffer;
            private readonly PixelSensorCapabilityType capabilityType;
            internal PixelSensorCapabilityRangeType RangeType;
            internal PixelSensorCapabilityDataType DataType;

            internal XrPixelSensorCapabilityRangeHolder(PixelSensorCapabilityRangeType rType, PixelSensorCapabilityDataType dType, PixelSensorCapabilityType cType)
            {
                internalBuffer = IntPtr.Zero;
                discreteBuffer = IntPtr.Zero;
                RangeType = rType;
                DataType = dType;
                capabilityType = cType;
            }

            internal unsafe PixelSensorCapabilityRange GetRangeFromInternal(uint streamIndex)
            {
                var result = new PixelSensorCapabilityRange
                {
                    RangeType = RangeType,
                    DataType = DataType,
                    CapabilityType = capabilityType,
                    Stream = streamIndex
                };

                switch (result.RangeType)
                {
                    case PixelSensorCapabilityRangeType.Continuous:
                        switch (result.DataType)
                        {
                            case PixelSensorCapabilityDataType.UnsignedInt32:
                            {
                                var range = new PixelSensorCapabilityRange.PixelSensorCapabilityMinMax<uint>();
                                var intValue = (XrPixelSensorCapabilityRangeContinuousUint*)internalBuffer;
                                range.Min = intValue->MinValue;
                                range.Max = intValue->MaxValue;
                                result.IntRange = range;
                                result.IsValid = intValue->Valid > 0;
                                break;
                            }
                            case PixelSensorCapabilityDataType.Float:
                            {
                                var range = new PixelSensorCapabilityRange.PixelSensorCapabilityMinMax<float>();
                                var floatValue = (XrPixelSensorCapabilityRangeContinuousFloat*)internalBuffer;
                                range.Min = floatValue->MinValue;
                                range.Max = floatValue->MaxValue;
                                result.FloatRange = range;
                                result.IsValid = floatValue->Valid > 0;
                                break;
                            }
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case PixelSensorCapabilityRangeType.Discrete:
                    {
                        switch (DataType)
                        {
                            case PixelSensorCapabilityDataType.Boolean:
                            {
                                var rangeValue = (XrPixelSensorCapabilityRangeDiscreteBoolean*)internalBuffer;
                                result.BoolValues = new bool[rangeValue->ValueCountOutput];
                                result.IsValid = rangeValue->Valid > 0;
                                for (var i = 0; i < result.BoolValues.Length; i++)
                                {
                                    var currentValue = rangeValue->Buffer[i];
                                    result.BoolValues[i] = currentValue > 0;
                                }

                                break;
                            }
                            case PixelSensorCapabilityDataType.UnsignedInt32:
                            {
                                var rangeValue = (XrPixelSensorCapabilityRangeDiscreteUint*)internalBuffer;
                                result.IntValues = UnsafeUtilityEx.GetManagedArray(rangeValue->Buffer, (int)rangeValue->ValueCountOutput);
                                result.IsValid = rangeValue->Valid > 0;
                                switch (capabilityType)
                                {
                                    //Have to make exceptions for format and exposure modes
                                    case PixelSensorCapabilityType.Format:
                                        result.FrameFormats = result.IntValues.Select(val => (PixelSensorFrameFormat)val).ToArray();
                                        break;
                                    case PixelSensorCapabilityType.AutoExposureMode:
                                        result.ExposureModes = result.IntValues.Select(val => (PixelSensorAutoExposureMode)val).ToArray();
                                        break;
                                }

                                break;
                            }
                            case PixelSensorCapabilityDataType.Extent2D:
                            {
                                var rangeValues = (XrPixelSensorCapabilityRangeDiscreteExtents*)internalBuffer;
                                result.ExtentValues = UnsafeUtilityEx.GetManagedArray(rangeValues->Buffer, (int)rangeValues->ValueCountOutput);
                                result.IsValid = rangeValues->Valid > 0;
                                break;
                            }
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return result;
            }

            private unsafe void AllocateInternalBuffer(int size)
            {
                internalBuffer = (IntPtr)UnsafeUtility.MallocTracked(size, UnsafeUtility.AlignOf<byte>(), Allocator.Persistent, 1);
                UnsafeUtility.MemClear(internalBuffer.ToPointer(), size);
            }

            internal unsafe IntPtr GetBaseHeader()
            {
                var bufferSize = RangeType switch
                {
                    PixelSensorCapabilityRangeType.Continuous => DataType switch
                    {
                        PixelSensorCapabilityDataType.UnsignedInt32 => sizeof(XrPixelSensorCapabilityRangeContinuousUint),
                        PixelSensorCapabilityDataType.Float => sizeof(XrPixelSensorCapabilityRangeContinuousFloat),
                        _ => throw new ArgumentOutOfRangeException(nameof(DataType), DataType, null)
                    },
                    PixelSensorCapabilityRangeType.Discrete => DataType switch
                    {
                        PixelSensorCapabilityDataType.Boolean => sizeof(XrPixelSensorCapabilityRangeDiscreteBoolean),
                        PixelSensorCapabilityDataType.UnsignedInt32 => sizeof(XrPixelSensorCapabilityRangeDiscreteUint),
                        PixelSensorCapabilityDataType.Extent2D => sizeof(XrPixelSensorCapabilityRangeDiscreteExtents),
                        _ => throw new ArgumentOutOfRangeException(nameof(DataType), DataType, null)
                    },
                    PixelSensorCapabilityRangeType.Boolean => sizeof(XrPixelSensorCapabilityRangeDiscreteBoolean),
                    _ => throw new ArgumentOutOfRangeException(nameof(RangeType), RangeType, null)
                };

                if (bufferSize == 0)
                {
                    return IntPtr.Zero;
                }

                AllocateInternalBuffer(bufferSize);
                var headerPtr = (XrPixelSensorCapabilityRangeBaseHeader*)internalBuffer;
                headerPtr->Type = GetStructTypeFromRangeAndData(RangeType, DataType);
                return internalBuffer;
            }
            
            //Need to get the header again but with allocated discrete values
            internal unsafe IntPtr GetBaseHeaderForDiscreteValues()
            {
                if (!IsRangeTypeDiscrete(RangeType))
                {
                    throw new ArgumentException($"The {RangeType} is not discrete. Invalid function call");
                }

                switch (DataType)
                {
                    case PixelSensorCapabilityDataType.Boolean:
                    {
                        var ptr = (XrPixelSensorCapabilityRangeDiscreteBoolean*)internalBuffer;
                        ptr->AllocateBuffer(out discreteBuffer);
                        break;
                    }
                    case PixelSensorCapabilityDataType.UnsignedInt32:
                    {
                        var ptr = (XrPixelSensorCapabilityRangeDiscreteUint*)internalBuffer;
                        ptr->AllocateBuffer(out discreteBuffer);
                        break;
                    }
                    case PixelSensorCapabilityDataType.Extent2D:
                    {
                        var ptr = (XrPixelSensorCapabilityRangeDiscreteExtents*)internalBuffer;
                        ptr->AllocateBuffer(out discreteBuffer);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var headerPtr = (XrPixelSensorCapabilityRangeBaseHeader*)internalBuffer;
                headerPtr->Type = GetStructTypeFromRangeAndData(RangeType, DataType);
                return internalBuffer;
            }

            public void Dispose()
            {
                unsafe
                {
                    UnsafeUtility.FreeTracked(internalBuffer.ToPointer(), Allocator.Persistent);
                    if (discreteBuffer != IntPtr.Zero)
                    {
                        UnsafeUtility.FreeTracked(discreteBuffer.ToPointer(), Allocator.Persistent);
                    }
                }
            }
        }
    }
}
