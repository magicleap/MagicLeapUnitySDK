using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapPixelSensorFeature
    {
        internal partial class PixelSensor
        {
            /// <summary>
            ///     A class representing a buffer associated with a pixel sensor's stream
            /// </summary>
            internal class PixelSensorStreamBuffer
            {
                private readonly PixelSensorNativeFunctions nativeFunctions;
                public readonly List<PixelSensorMetaDataType> RequestedMetaDataTypes = new();
                private readonly PixelSensor sensor;
                private readonly uint streamIndex;
                internal XrPixelSensorBuffer SensorBuffer;

                public PixelSensorStreamBuffer(uint streamIndex, PixelSensor sensor)
                {
                    this.streamIndex = streamIndex;
                    this.sensor = sensor;
                    nativeFunctions = this.sensor.NativeFunctions;
                    IsValid = false;
                }

                public bool IsValid { get; private set; }

                private IntPtr Buffer { get; set; } = IntPtr.Zero;

                private uint BufferSize { get; set; }

                public long LastCaptureTime { get; set; } = 1;

                /// <summary>
                ///     Allocate the required internal buffers for the stream. Calls the native function to get the buffer size and then
                ///     allocates a buffer
                /// </summary>
                public void AllocateBuffer()
                {
                    unsafe
                    {
                        var metadataTypeArray = new NativeArray<PixelSensorMetaDataType>(RequestedMetaDataTypes.ToArray(), Allocator.Temp);
                        var bufferPropertiesInfo = new XrPixelSensorBufferPropertiesInfo
                        {
                            Type = XrPixelSensorStructTypes.XrTypePixelSensorBufferPropertiesInfoML,
                            Stream = streamIndex,
                            MetaDataCount = (uint)metadataTypeArray.Length,
                            MetaDatas = metadataTypeArray.Length > 0 ? (PixelSensorMetaDataType*)metadataTypeArray.GetUnsafePtr() : null
                        };

                        var bufferProperties = new XrPixelSensorBufferProperties
                        {
                            Type = XrPixelSensorStructTypes.XrTypePixelSensorBufferPropertiesML
                        };

                        var xrResult = nativeFunctions.XrGetPixelSensorBufferProperties(sensor.Handle, ref bufferPropertiesInfo, out bufferProperties);
                        if (!Utils.DidXrCallSucceed(xrResult, nameof(PixelSensorNativeFunctions.XrGetPixelSensorBufferProperties)))
                        {
                            return;
                        }

                        BufferSize = bufferProperties.BufferSize;
                        Buffer = (IntPtr)UnsafeUtility.MallocTracked(BufferSize, UnsafeUtility.AlignOf<byte>(), Allocator.Persistent, 1);
                        SensorBuffer = new XrPixelSensorBuffer
                        {
                            Type = XrPixelSensorStructTypes.XrTypePixelSensorBufferML,
                            BufferSize = BufferSize,
                            Buffer = Buffer
                        };
                        IsValid = true;
                    }
                }

                private void FreeBuffer()
                {
                    if (Buffer == IntPtr.Zero)
                    {
                        return;
                    }

                    unsafe
                    {
                        UnsafeUtility.FreeTracked(Buffer.ToPointer(), Allocator.Persistent);
                        BufferSize = 0;
                        Buffer = IntPtr.Zero;
                    }
                }

                public void Dispose()
                {
                    FreeBuffer();
                    IsValid = false;
                }
            }

            /// <summary>
            ///     A class that represents the buffer information of all the streams supported by a pixel sensor
            /// </summary>
            internal class PixelSensorStreamBuffers : IDisposable, IEnumerable<PixelSensorStreamBuffer>
            {
                private readonly Dictionary<uint, PixelSensorStreamBuffer> bufferTable = new();

                private readonly PixelSensor sensor;

                public PixelSensorStreamBuffers(PixelSensor sensor)
                {
                    this.sensor = sensor;
                }

                public PixelSensorStreamBuffer this[uint streamIndex]
                {
                    get
                    {
                        if (bufferTable.TryGetValue(streamIndex, out var buffer))
                        {
                            return buffer;
                        }

                        buffer = bufferTable[streamIndex] = new PixelSensorStreamBuffer(streamIndex, sensor);
                        return buffer;
                    }
                }

                public void Dispose()
                {
                    foreach (var buffer in bufferTable)
                    {
                        buffer.Value.Dispose();
                    }
                }

                public IEnumerator<PixelSensorStreamBuffer> GetEnumerator()
                {
                    return bufferTable.Values.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }
            }
        }
    }
}
