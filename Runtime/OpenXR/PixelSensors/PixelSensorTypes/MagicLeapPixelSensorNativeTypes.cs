using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.MagicLeap.Unsafe;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Features.PixelSensors
{
    /// <summary>
    /// The exposure mode of the pixel sensor
    /// </summary>
    public enum PixelSensorAutoExposureMode : ulong
    {
        /// <summary>
        /// Exposure mode optimized for environmental tracking
        /// </summary>
        EnvironmentTracking,

        /// <summary>
        /// Exposure mode optimized for proximity IR light source.
        /// </summary>
        ProximityIrTracking
    }

    /// <summary>
    /// Represents the data types supported by a pixel sensor capability.
    /// </summary>
    public enum PixelSensorCapabilityDataType : uint
    {
        Boolean,
        UnsignedInt32 = 100,
        Float = 101,
        Extent2D = 200
    }

    /// <summary>
    /// Represents the range type of pixel sensor capability.
    /// </summary>
    public enum PixelSensorCapabilityRangeType : uint
    {
        /// <summary>
        /// A boolean value
        /// </summary>
        Boolean,

        /// <summary>
        /// Represents a range with a min and a max value
        /// </summary>
        Continuous,

        /// <summary>
        /// Represents a series of discrete values
        /// </summary>
        Discrete
    }

    /// <summary>
    /// The configurable capability types of a pixel sensor
    /// </summary>
    public enum PixelSensorCapabilityType : uint
    {
        /// <summary>
        /// Data Rate Per second of the sensor. This must be specified for a sensor to be configured
        /// </summary>
        UpdateRate,

        /// <summary>
        /// The resolution of the sensor data. This must be specified for a sensor to be configured
        /// </summary>
        Resolution,

        /// <summary>
        /// The data format for the sensor. This must be specified for a sensor to be configured
        /// </summary>
        Format = 3,

        /// <summary>
        /// The range of a depth sensor
        /// </summary>
        Depth,

        /// <summary>
        /// The camera frame and the digital content will be blended into a single frame
        /// </summary>
        MixedReality,

        /// <summary>
        /// Exposure time in milliseconds. If this is not specified then AutoExposureMode and AutoExposureTargetBrightness must be specified
        /// </summary>
        ///
        ManualExposureTime = 100,

        /// <summary>
        /// Higher gain is useful in low light conditions but may introduce noise.
        /// </summary>
        AnalogGain,

        /// <summary>
        /// Higher gain is useful in low light conditions but may introduce noise.
        /// </summary>            
        DigitalGain,

        /// <summary>
        /// Enable/disable auto exposure mode.
        /// </summary>
        AutoExposureMode = 200,

        /// <summary>
        /// Set target brightness for auto exposure mode.
        /// </summary>
        AutoExposureTargetBrightness
    }


    /// <summary>
    /// The flags associated with <see cref="PixelSensorDepthFlagBuffer"/> each pixel 
    /// </summary>
    public enum PixelSensorDepthFlags
    {
        Valid = 1,
        Invalid = 2,
        Saturated = 4,
        Inconsistent = 8,
        LowSignal = 0x10,
        FlyingPixel = 0x20,
        MaskedBit = 0x40,
        Sbi = 0x80,
        StrayLight = 0x100,
        ConnectedComponents = 0x200
    }

    /// <summary>
    /// The status of the depth frame projector when the frame is obtained
    /// </summary>
    public enum PixelSensorDepthFrameIlluminationType
    {
        /// <summary>
        /// Depth camera frame projector is on
        /// </summary>
        On,

        /// <summary>
        /// Depth camera frame projector is off
        /// </summary>
        Off
    }
    
    /// <summary>
    /// The pixel sensor's reality mode
    /// </summary>
    public enum PixelSensorRealityMode
    {
        /// <summary>
        /// Camera frame and digital content will be blended into a single frame
        /// </summary>
        Mixed,
        /// <summary>
        /// Only camera frame will be captured
        /// </summary>
        Camera,
        /// <summary>
        /// Only virtual content will be captured
        /// </summary>
        Virtual
    }
    
    /// <summary>
    /// <para>The frame format capabilities of a sensor.</para>
    /// Note: This is not the same as PixelSensorFrameType.
    /// <see cref="PixelSensorFrameFormat"/> represents the frame capabilities supported by a sensor while <see cref="PixelSensorFrameType"/> indicates the type of frame
    /// when the sensor data is fetched
    /// </summary>
    public enum PixelSensorFrameFormat : uint
    {
        /// <summary>
        /// Each pixel is 1 byte and represents a grayscale value
        /// </summary>
        Grayscale,

        /// <summary>
        /// Each pixel is 4 bytes and represents R,G,B, and A channels in that order
        /// </summary>
        Rgba8888,

        /// <summary>
        /// Frame is represented in the YUV_420_888 planar format
        /// </summary>
        Yuv420888,

        /// <summary>
        /// Frame is JPEG encoded
        /// </summary>
        Jpeg,

        /// <summary>
        /// Represents the depth. Depth is the radial distance (in meters) of the real world location with respect to the depth camera. 
        /// </summary>
        Depth32,

        /// <summary>
        /// <para>Raw pixel data representing light captured by the sensor.</para>
        /// For depth cameras that have a projector this raw frame will include frames captured both when the projector is on and off.
        /// </summary>
        DepthRaw
    }

    /// <summary>
    /// <para>The frame format types of a sensor's data.</para>
    /// Note: This is not the same as PixelSensorFrameFormat.
    /// <see cref="PixelSensorFrameFormat"/> represents the frame capabilities supported by a sensor while <see cref="PixelSensorFrameType"/> indicates the type of frame
    /// when the sensor data is fetched
    /// </summary>
    public enum PixelSensorFrameType : uint
    {
        /// <summary>
        /// Each pixel is 1 byte and represents a grayscale value
        /// </summary>
        Grayscale,

        /// <summary>
        /// Each pixel is 4 bytes and represents R,G,B, and A channels in that order
        /// </summary>
        Rgba8888,

        /// <summary>
        /// Frame is represented in the YUV_420_888 planar format
        /// </summary>
        Yuv420888,

        /// <summary>
        /// Frame is JPEG encoded
        /// </summary>
        Jpeg,

        /// <summary>
        /// Represents the depth. Depth is the radial distance (in meters) of the real world location with respect to the depth camera. 
        /// </summary>
        Depth32,

        /// <summary>
        /// <para>Raw pixel data representing light captured by the sensor.</para>
        /// For depth cameras that have a projector this raw frame will include frames captured both when the projector is on and off.
        /// </summary>
        DepthRaw,

        /// <summary>
        /// The confidence data for each pixel in the sensor frame buffer
        /// <para>The confidence score is derived from the sensor noise and it is not normalized</para>
        /// </summary>
        DepthConfidence,

        /// <summary>
        /// The depth sensor flags for each pixel in the sensor frame buffer. See <see cref="PixelSensorDepthFlags"/>
        /// </summary>
        DepthFlags
    }

    /// <summary>
    /// The metadata types that can be supported by a sensor
    /// </summary>
    public enum PixelSensorMetaDataType : uint
    {
        /// <summary>
        /// Exposure time in milliseconds used to capture the frame.
        /// </summary>
        ExposureTime,

        /// <summary>
        /// Analog gain used to capture the frame
        /// </summary>
        AnalogGain,

        /// <summary>
        /// Digital gain used to capture the frame
        /// </summary>
        DigitalGain,

        /// <summary>
        /// Pinhole camera model
        /// </summary>
        PinholeCameraModel,

        /// <summary>
        /// Fisheye camera model
        /// </summary>
        FishEyeCameraModel,

        /// <summary>
        /// Illumination type used for the depth frame
        /// </summary>
        DepthFrameIllumination,

        /// <summary>
        /// Confidence values for each pixel in the camera frame.
        /// The confidence score is derived from the sensor noise. It is not normalized.
        /// <para>The higher the value the higher the confidence.</para>
        /// Applications can determine what confidence threshold to use based on their use case
        /// </summary>
        DepthConfidenceBuffer,

        /// <summary>
        /// Flag bits for each pixel in the depth camera frame. See <see cref="PixelSensorDepthFlags"/>
        /// </summary>
        DepthFlagBuffer
    }

    internal struct XrEventBuffer
    {
        internal ulong Type;
        internal IntPtr Next;
        internal IntPtr Varying;
    }

    internal enum XrPixelSensorStructTypes : ulong
    {
        XrTypePixelSensorCreateInfoML = 1000475001U,
        XrTypeEventDataPixelSensorAvailabilityChangedML = 1000475004U,
        XrTypePixelSensorCapabilityML = 1000475005U,
        XrTypePixelSensorCapabilityQueryInfoML = 1000475006U,
        XrTypePixelSensorConfigInfoML = 1000475035U,
        XrTypePixelSensorCapabilityConfigXrBool32ML = 1000475030U,
        XrTypePixelSensorCapabilityConfigUint32ML = 1000475007U,
        XrTypePixelSensorCapabilityConfigFloatML = 1000475008U,
        XrTypePixelSensorCapabilityConfigXrExtent2DiML = 1000475009U,
        XrTypePixelSensorCapabilityRangeContinuousFloatML = 1000475010U,
        XrTypePixelSensorCapabilityRangeContinuousUint32ML = 1000475011U,
        XrTypePixelSensorCapabilityRangeDiscreteUint32ML = 1000475012U,
        XrTypePixelSensorCapabilityRangeDiscreteXrBool32ML = 1000475032U,
        XrTypePixelSensorCapabilityRangeDiscreteXrExtent2DiML = 1000475013U,
        XrTypePixelSensorBufferPropertiesInfoML = 1000475014U,
        XrTypePixelSensorBufferPropertiesML = 1000475031U,
        XrTypePixelSensorBufferML = 1000475015U,
        XrTypePixelSensorDataGetInfoML = 1000475016U,
        XrTypePixelSensorDataML = 1000475017U,
        XrTypePixelSensorExposureTimeML = 1000475025U,
        XrTypePixelSensorAnalogGainML = 1000475038U,
        XrTypePixelSensorDigitalGainML = 1000475039U,
        XrTypePixelSensorPinholeIntrinsicsML = 1000475018U,
        XrTypePixelSensorFisheyeIntrinsicsML = 1000475024U,
        XrTypePixelSensorDepthConfidenceBufferML = 1000475026U,
        XrTypePixelSensorDepthFlagBufferML = 1000475027U,
        XrTypePixelSensorCreateSpaceInfoML = 1000475019U,
        XrTypePixelSensorConfigureCompletionML = 1000475020U,
        XrTypePixelSensorStartCompletionML = 1000475036U,
        XrTypePixelSensorStopCompletionML = 1000475037U,
        XrTypePixelSensorFrameML = 1000475021U,
        XrTypePixelSensorPlaneML = 1000475033U,
        XrTypePixelSensorStartInfoML = 1000475022U,
        XrTypePixelSensorStopInfoML = 1000475023U,
        XrTypePixelSensorDepthFrameIlluminationML = 1000475034U
    }

    internal struct XrPixelSensor
    {
        internal ulong Handle;

        internal XrPixelSensor(ulong handle = default)
        {
            Handle = handle;
        }
        public static implicit operator XrPixelSensor(ulong handle) => new(handle);
    }

    internal struct XrPixelSensorCreateInfo
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal ulong Sensor;
    }

    internal struct XrEventDataPixelSensorAvailabilityChanged
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal ulong Sensor;
        internal uint Available;
        internal long ChangeTime;
    }

    internal struct XrPixelSensorCapability
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal PixelSensorCapabilityType CapabilityType;
        internal PixelSensorCapabilityDataType CapabilityDataType;
        internal PixelSensorCapabilityRangeType CapabilityRangeType;
    }

    internal struct XrPixelSensorCapabilityQueryInfo
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint Stream;
        internal PixelSensorCapabilityType CapabilityType;
    }

    internal struct XrPixelSensorCapabilityConfigBaseHeader
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint Stream;
        internal PixelSensorCapabilityType CapabilityType;
    }

    internal unsafe struct XrPixelSensorConfigInfo
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint StreamCount;
        internal uint* Streams;
        internal uint ConfigurationCount;
        internal IntPtr* Configurations;
    }

    internal struct XrPixelSensorCapabilityConfigBool32
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint Stream;
        internal PixelSensorCapabilityType CapabilityType;
        internal uint Value;
    }

    internal struct XrPixelSensorCapabilityConfigUint32
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint Stream;
        internal PixelSensorCapabilityType CapabilityType;
        internal uint Value;
    }

    internal struct XrPixelSensorCapabilityConfigFloat
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint Stream;
        internal PixelSensorCapabilityType CapabilityType;
        internal float Value;
    }

    internal struct XrPixelSensorCapabilityConfigExtent2D
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint Stream;
        internal PixelSensorCapabilityType CapabilityType;
        internal Vector2Int Value;
    }

    internal struct XrPixelSensorCapabilityRangeBaseHeader
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint Valid;
        internal bool IsValid => Valid > 0;
    }

    internal struct XrPixelSensorCapabilityRangeContinuousFloat
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint Valid;
        internal float MinValue;
        internal float MaxValue;
    }

    internal struct XrPixelSensorCapabilityRangeContinuousUint
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint Valid;
        internal uint MinValue;
        internal uint MaxValue;
    }

    internal unsafe struct XrPixelSensorCapabilityRangeDiscreteBoolean
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint Valid;
        internal uint ValueCapacityInput;
        internal uint ValueCountOutput;
        internal uint* Buffer;

        internal void AllocateBuffer(out IntPtr buffer)
        {
            ValueCapacityInput = ValueCountOutput;
            buffer = (IntPtr)UnsafeUtility.MallocTracked((int)ValueCountOutput * sizeof(uint), UnsafeUtility.AlignOf<uint>(), Allocator.Persistent, 1);
            Buffer = (uint*)buffer.ToPointer();
            UnsafeUtility.MemClear(buffer.ToPointer(), sizeof(uint) * ValueCountOutput);
        }
    }

    internal unsafe struct XrPixelSensorCapabilityRangeDiscreteExtents
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint Valid;
        internal uint ValueCapacityInput;
        internal uint ValueCountOutput;
        internal Vector2Int* Buffer;

        internal void AllocateBuffer(out IntPtr buffer)
        {
            ValueCapacityInput = ValueCountOutput;
            buffer = (IntPtr)UnsafeUtility.MallocTracked((int)ValueCountOutput * sizeof(Vector2Int), UnsafeUtility.AlignOf<Vector2Int>(), Allocator.Persistent, 1);
            Buffer = (Vector2Int*)buffer.ToPointer();
            UnsafeUtility.MemClear(buffer.ToPointer(), sizeof(uint) * ValueCountOutput);
        }
    }

    internal unsafe struct XrPixelSensorCapabilityRangeDiscreteUint
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint Valid;
        internal uint ValueCapacityInput;
        internal uint ValueCountOutput;
        internal uint* Buffer;

        internal void AllocateBuffer(out IntPtr buffer)
        {
            ValueCapacityInput = ValueCountOutput;
            buffer = (IntPtr)UnsafeUtility.MallocTracked((int)ValueCountOutput * sizeof(uint), UnsafeUtility.AlignOf<uint>(), Allocator.Persistent, 1);
            Buffer = (uint*)buffer.ToPointer();
            UnsafeUtility.MemClear(buffer.ToPointer(), sizeof(uint) * ValueCountOutput);
        }
    }

    internal unsafe struct XrPixelSensorBufferPropertiesInfo
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint Stream;
        internal uint MetaDataCount;
        internal PixelSensorMetaDataType* MetaDatas;
    }

    internal struct XrPixelSensorBufferProperties
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint Stream;
        internal uint BufferSize;
    }

    internal struct XrPixelSensorBuffer
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint BufferSize;
        internal IntPtr Buffer;
    }

    internal struct XrPixelSensorDataGetInfo
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint Stream;
        internal long LastCaptureTime;
        internal long Timeout;
    }

    internal unsafe struct XrPixelSensorPlane
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint Width;
        internal uint Height;
        internal uint Stride;
        internal uint BytesPerPixel;
        internal uint PixelStride;
        internal uint BufferSize;
        internal byte* Buffer;
    }

    internal unsafe struct XrPixelSensorFrame
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal PixelSensorFrameType FrameType;
        internal uint PlaneCount;
        internal XrPixelSensorPlane* Planes;
    }

    internal struct XrPixelSensorData
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal long CaptureTime;
        internal IntPtr Frame;
    }

    internal struct XrPixelSensorExposureTime
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal float ExposureTime;

        public readonly PixelSensorMetaData GetMetaData(Allocator allocator)
        {
            return new PixelSensorExposureTime
            {
                ExposureTime = ExposureTime
            };
        }
    }

    internal struct XrPixelSensorBaseHeader
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
    }


    internal struct XrPixelSensorAnalogGain
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint AnalogGain;

        public PixelSensorMetaData GetMetaData(Allocator allocator)
        {
            return new PixelSensorAnalogGain
            {
                AnalogGain = AnalogGain
            };
        }
    }

    internal struct XrPixelSensorDigitalGain
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint DigitalGain;

        public PixelSensorMetaData GetMetaData(Allocator allocator)
        {
            return new PixelSensorDigitalGain
            {
                DigitalGain = DigitalGain
            };
        }
    }

    internal unsafe struct XrPixelSensorPinholeIntrinsics
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal Vector2 FocalLength;
        internal Vector2 PrincipalPoint;
        internal Vector2 FOV;
        private fixed double distortion[5];

        public PixelSensorMetaData GetMetaData(Allocator allocator)
        {
            var result = new PixelSensorPinholeIntrinsics
            {
                FocalLength = FocalLength,
                PrincipalPoint = PrincipalPoint,
                FOV = FOV
            };
            fixed (double* distortions = distortion)
            {
                result.Distortion = UnsafeUtilityEx.GetManagedArray(distortions, 5);
            }

            return result;
        }
    }

    internal unsafe struct XrPixelSensorFisheyeIntrinsics
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal Vector2 FocalLength;
        internal Vector2 PrincipalPoint;
        internal Vector2 FOV;

        private fixed double radialDistortion[5];
        private fixed double tangentialDistortion[2];

        public PixelSensorMetaData GetMetaData(Allocator allocator)
        {
            var result = new PixelSensorFisheyeIntrinsics
            {
                FocalLength = FocalLength,
                PrincipalPoint = PrincipalPoint,
                FOV = FOV
            };
            fixed (double* radialDistortions = radialDistortion)
            {
                result.RadialDistortion = UnsafeUtilityEx.GetManagedArray(radialDistortions, 5);
            }

            fixed (double* tangentialDistortions = tangentialDistortion)
            {
                result.TangentialDistortion = UnsafeUtilityEx.GetManagedArray(tangentialDistortions, 2);
            }

            return result;
        }
    }

    internal struct XrPixelSensorDepthFrameIllumination
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal PixelSensorDepthFrameIlluminationType IlluminationType;

        public PixelSensorMetaData GetMetaData(Allocator allocator)
        {
            return new PixelSensorDepthFrameIllumination
            {
                IlluminationType = IlluminationType
            };
        }
    }

    internal unsafe struct XrPixelSensorDepthConfidenceBuffer
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal XrPixelSensorFrame* Frame;

        public PixelSensorMetaData GetMetaData(Allocator allocator, bool shouldFlipTexture, long captureTime)
        {
            var buffer = new PixelSensorDepthConfidenceBuffer();
            buffer.Frame.CreateFromNativeFrame(Frame, allocator, shouldFlipTexture, captureTime);
            return buffer;
        }
    }

    internal unsafe struct XrPixelSensorDepthFlagBuffer
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal XrPixelSensorFrame* Frame;

        public PixelSensorMetaData GetMetaData(Allocator allocator, bool shouldFlipTexture, long captureTime)
        {
            var buffer = new PixelSensorDepthFlagBuffer();
            buffer.Frame.CreateFromNativeFrame(Frame, allocator, shouldFlipTexture, captureTime);
            return buffer;
        }
    }

    internal struct XrPixelSensorCreateSpaceInfo
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal XrPixelSensor Sensor;
        internal XrPose Offset;
    }

    internal struct XrPixelSensorConfigureCompletion
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal XrResult FutureResult;
    }

    internal unsafe struct XrPixelSensorStartInfo
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint StreamCount;
        internal uint* Streams;
    }

    internal struct XrPixelSensorStartCompletion
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal XrResult FutureResult;
    }

    internal unsafe struct XrPixelSensorStopInfo
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal uint StreamCount;
        internal uint* Streams;
    }

    internal struct XrPixelSensorStopCompletion
    {
        internal XrPixelSensorStructTypes Type;
        internal IntPtr Next;
        internal XrResult FutureResult;
    }

    internal unsafe struct XrPixelSensorConfigData : IDisposable
    {
        private IntPtr internalBuffer;
        private PixelSensorCapabilityType capabilityType;
        private PixelSensorCapabilityDataType dataType;
        private XrPixelSensorStructTypes targetType;
        private uint streamIndex;

        internal void AssignFromConfig(PixelSensorConfigData configData)
        {
            dataType = configData.ActiveDataType;
            capabilityType = configData.CapabilityType;
            streamIndex = configData.StreamIndex;
            var bufferSize = dataType switch
            {
                PixelSensorCapabilityDataType.Boolean => sizeof(XrPixelSensorCapabilityConfigBool32),
                PixelSensorCapabilityDataType.UnsignedInt32 => sizeof(XrPixelSensorCapabilityConfigUint32),
                PixelSensorCapabilityDataType.Float => sizeof(XrPixelSensorCapabilityConfigFloat),
                PixelSensorCapabilityDataType.Extent2D => sizeof(XrPixelSensorCapabilityConfigExtent2D),
                _ => throw new ArgumentOutOfRangeException()
            };

            internalBuffer = (IntPtr)UnsafeUtility.MallocTracked(bufferSize, UnsafeUtility.AlignOf<byte>(), Allocator.Persistent, 1);
            UnsafeUtility.MemClear(internalBuffer.ToPointer(), bufferSize);
            switch (dataType)
            {
                case PixelSensorCapabilityDataType.Boolean:
                {
                    var ptr = (XrPixelSensorCapabilityConfigBool32*)internalBuffer;
                    ptr->CapabilityType = capabilityType;
                    targetType = ptr->Type = XrPixelSensorStructTypes.XrTypePixelSensorCapabilityConfigXrBool32ML;
                    ptr->Value = 0;
                    if (configData.BoolValue.HasValue)
                    {
                        ptr->Value = configData.BoolValue.Value ? 1U : 0;
                    }

                    ptr->Stream = streamIndex;
                    break;
                }
                case PixelSensorCapabilityDataType.UnsignedInt32:
                {
                    var ptr = (XrPixelSensorCapabilityConfigUint32*)internalBuffer;
                    ptr->CapabilityType = capabilityType;
                    targetType = ptr->Type = XrPixelSensorStructTypes.XrTypePixelSensorCapabilityConfigUint32ML;
                    ptr->Value = configData.IntValue ?? 0;
                    ptr->Stream = streamIndex;
                    break;
                }
                case PixelSensorCapabilityDataType.Float:
                {
                    var ptr = (XrPixelSensorCapabilityConfigFloat*)internalBuffer;
                    ptr->CapabilityType = capabilityType;
                    targetType = ptr->Type = XrPixelSensorStructTypes.XrTypePixelSensorCapabilityConfigFloatML;
                    ptr->Value = configData.FloatValue ?? 0;
                    ptr->Stream = streamIndex;
                    break;
                }
                case PixelSensorCapabilityDataType.Extent2D:
                {
                    var ptr = (XrPixelSensorCapabilityConfigExtent2D*)internalBuffer;
                    ptr->CapabilityType = capabilityType;
                    targetType = ptr->Type = XrPixelSensorStructTypes.XrTypePixelSensorCapabilityConfigXrExtent2DiML;
                    ptr->Value = configData.VectorValue ?? Vector2Int.zero;
                    ptr->Stream = streamIndex;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal IntPtr GetBaseHeader()
        {
            var header = (XrPixelSensorCapabilityConfigBaseHeader*)internalBuffer;
            header->Type = targetType;
            header->Stream = streamIndex;
            header->CapabilityType = capabilityType;
            header->Next = IntPtr.Zero;
            return (IntPtr)header;
        }

        public void Dispose()
        {
            UnsafeUtility.FreeTracked(internalBuffer.ToPointer(), Allocator.Persistent);
        }
    }

    internal struct XrMetadataContainer
    {
        //When requesting metadata, persistent values have to passed in.
        //Can't simulate a union as multiple metadata can be requested at once
        internal XrPixelSensorAnalogGain AnalogGain;
        internal XrPixelSensorDepthConfidenceBuffer DepthConfidenceBuffer;
        internal XrPixelSensorDepthFlagBuffer DepthFlagBuffer;
        internal XrPixelSensorDepthFrameIllumination DepthFrameIllumination;
        internal XrPixelSensorDigitalGain DigitalGain;
        internal XrPixelSensorExposureTime ExposureTime;
        internal XrPixelSensorFisheyeIntrinsics FisheyeIntrinsics;
        internal XrPixelSensorPinholeIntrinsics PinholeIntrinsics;

        internal static XrMetadataContainer Create()
        {
            var result = new XrMetadataContainer();
            result.AnalogGain.Type = XrPixelSensorStructTypes.XrTypePixelSensorAnalogGainML;
            result.DepthConfidenceBuffer.Type = XrPixelSensorStructTypes.XrTypePixelSensorDepthConfidenceBufferML;
            result.DepthFlagBuffer.Type = XrPixelSensorStructTypes.XrTypePixelSensorDepthFlagBufferML;
            result.DepthFrameIllumination.Type = XrPixelSensorStructTypes.XrTypePixelSensorDepthFrameIlluminationML;
            result.DigitalGain.Type = XrPixelSensorStructTypes.XrTypePixelSensorDigitalGainML;
            result.ExposureTime.Type = XrPixelSensorStructTypes.XrTypePixelSensorExposureTimeML;
            result.FisheyeIntrinsics.Type = XrPixelSensorStructTypes.XrTypePixelSensorFisheyeIntrinsicsML;
            result.PinholeIntrinsics.Type = XrPixelSensorStructTypes.XrTypePixelSensorPinholeIntrinsicsML;
            return result;
        }
    }
}
