// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLCameraBase
    {
        internal partial class NativeBindings
        {
            /// <summary>
            /// Structure to encapsulate connection settings for MR capture.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLCameraMRConnectInfo
            {
                /// <summary>
                /// MR video quality
                /// </summary>
                public MLCamera.MRQuality MrQuality;

                /// <summary>
                /// blending type for mixed reality capture
                /// </summary>
                public MLCamera.MRBlendType MrBlendType;

                /// <summary>
                /// capture frame rate
                /// </summary>
                public MLCamera.CaptureFrameRate FrameRate;

                /// <summary>
                /// Create and return an initialized version of this struct.
                /// </summary>
                /// <returns>A new instance of this struct.</returns>
                public static MLCameraMRConnectInfo Create(MLCamera.MRConnectInfo connectInfo)
                {
                    return new MLCameraMRConnectInfo()
                    {
                        MrQuality = connectInfo.MRQuality,
                        MrBlendType = connectInfo.MRBlendType,
                        FrameRate = connectInfo.FrameRate
                    };
                }
            }

            /// <summary>
            /// Structure to encapsulate context for a CameraConnect Request.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLCameraConnectContext
            {
                /// <summary>
                /// version contains the version number for this structure.
                /// </summary>
                public uint Version;

                /// <summary>
                /// logical camera idenitifier
                /// </summary>
                public  MLCamera.Identifier CamId;

                /// <summary>
                /// The Context in which the camera should operate in.
                /// </summary>
                public MLCamera.ConnectFlag Flags;

                /// <summary>
                /// Video Stabilization enable/disable 1 - enable 0 - disabled
                /// </summary>
                [MarshalAs(UnmanagedType.I1)]
                public bool EnableVideoStab;

                /// <summary>
                /// MR capture connection settings
                /// </summary>
                public MLCameraMRConnectInfo MrInfo;

                /// <summary>
                /// Create and return an initialized version of this struct.
                /// </summary>
                /// <returns>A new instance of this struct.</returns>
                public static MLCameraConnectContext Create(MLCamera.ConnectContext cameraContext)
                {
                    return new MLCameraConnectContext()
                    {
                        Version = 1,
                        CamId = cameraContext.CamId,
                        Flags = cameraContext.Flags,
                        EnableVideoStab = cameraContext.EnableVideoStabilization,
                        MrInfo = MLCameraMRConnectInfo.Create(cameraContext.MixedRealityConnectInfo)
                    };
                }
            }

            /// <summary>
            /// The capabilities for each stream supported by logical camera device * can be queried with MLCameraGetDeviceCaps
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLCameraCaptureStreamCaps
            {
                /// <summary>
                /// capture_type Video, Image ,Preview
                /// </summary>
                public MLCamera.CaptureType CaptureType;

                /// <summary>
                /// Capture Resolution
                /// </summary>
                public int Width;

                /// <summary>
                /// Resolution height
                /// </summary>
                public int Height;

                /// <summary>
                /// Create and return an initialized version of this struct.
                /// </summary>
                /// <returns>A new instance of this struct.</returns>
                public static MLCameraCaptureStreamCaps Create()
                {
                    return new MLCameraCaptureStreamCaps()
                    {
                    };
                }
            }

            /// <summary>
            /// Structure to encapsulate stream configurations
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLCameraCaptureStreamConfig
            {

                /// <summary>
                /// capture_type Image Preview
                /// </summary>
                public MLCamera.CaptureType CaptureType;

                /// <summary>
                /// Capture Resolution
                /// </summary>
                public int Width;

                /// <summary>
                /// Resolution height
                /// </summary>
                public int Height;

                /// <summary>
                /// output Format
                /// </summary>
                public MLCamera.OutputFormat OutputFormat;

                /// <summary>
                /// media recorder surface, only valid for capture type video * set to ML_INVALID_HANDLE for yuv/rgba video capture
                /// </summary>
                public ulong MediaRecorderSurfaceHandle;

                /// <summary>
                /// Create and return an initialized version of this struct.
                /// </summary>
                /// <returns>A new instance of this struct.</returns>
                public static MLCameraCaptureStreamConfig Create()
                {
                    return new MLCameraCaptureStreamConfig()
                    {
                    };
                }

                public static MLCameraCaptureStreamConfig Create(MLCamera.CaptureStreamConfig config)
                {
                    return new MLCameraCaptureStreamConfig()
                    {
                        Height = config.Height,
                        Width = config.Width,
                        CaptureType = config.CaptureType,
                        OutputFormat = config.OutputFormat,
                        MediaRecorderSurfaceHandle = config.Surface?.Handle ?? InvalidHandle,
                    };
                }
            }

            /// <summary>
            /// Structure to encapsulate capture configuration
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLCameraCaptureConfig
            {
                /// <summary>
                /// version contains the version number for this structure.
                /// </summary>
                public uint Version;

                /// <summary>
                /// capture frame rate
                /// </summary>
                public MLCamera.CaptureFrameRate CaptureFrameRate;

                /// <summary>
                /// no of capture streams.
                /// </summary>
                public uint NumStreams;

                /// <summary>
                /// Stream configurations
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = MLCameraMaxStreams)]
                public MLCameraCaptureStreamConfig[] StreamConfig;

                /// <summary>
                /// Create and return an initialized version of this struct.
                /// </summary>
                /// <returns>A new instance of this struct.</returns>
                public static MLCameraCaptureConfig Create()
                {
                    return new MLCameraCaptureConfig()
                    {
                        Version = 1,
                        StreamConfig = new MLCameraCaptureStreamConfig[MLCameraMaxStreams]
                    };
                }

                /// <summary>
                /// Create and initialize this native struct based on values exposed in <seealso cref="MLCameraCaptureConfig"/>.
                /// </summary>
                /// <returns>A new instance of this struct.</returns>
                public static MLCameraCaptureConfig Create(MLCamera.CaptureConfig config)
                {
                    MLCameraCaptureConfig nativeConfig = MLCameraCaptureConfig.Create();
                    nativeConfig.NumStreams = (uint)config.StreamConfigs.Length;
                    nativeConfig.CaptureFrameRate = config.CaptureFrameRate;
                    for (var i = 0; i < config.StreamConfigs.Length; i++)
                        nativeConfig.StreamConfig[i] = MLCameraCaptureStreamConfig.Create(config.StreamConfigs[i]);

                    return nativeConfig;
                }
            }

            /// <summary>
            /// Per plane info for captured output
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLCameraPlaneInfo
            {
                /// <summary>
                /// version contains the version number for this structure.
                /// </summary>
                public uint Version;

                /// <summary>
                /// Width of the output image in pixels
                /// </summary>
                public uint Width;

                /// <summary>
                /// Height of the output image in pixels
                /// </summary>
                public uint Height;

                /// <summary>
                /// Stride of the output image in pixels
                /// </summary>
                public uint Stride;

                /// <summary>
                /// Number of bytes used to represent a pixel
                /// </summary>
                public uint BytesPerPixel;

                public uint PixelsStride;

                /// <summary>
                /// Image data
                /// </summary>
                public IntPtr Data;

                /// <summary>
                /// Number of bytes in the image output data
                /// </summary>
                public uint Size;

                /// <summary>
                /// Create and return an initialized version of this struct.
                /// </summary>
                /// <returns>Returns a new MLCamera.PlaneInfo structure.</returns>
                public static MLCameraPlaneInfo Create()
                {
                    return new MLCameraPlaneInfo
                    {
                        Version = 0u,
                        Width = 0u,
                        Height = 0u,
                        Stride = 0u,
                        BytesPerPixel = 0u,
                        PixelsStride = 0u,
                        Data = IntPtr.Zero,
                        Size = 0u
                    };
                }

                public MLCamera.PlaneInfo CreatePlaneInfo(bool copyToManagedMemory, byte[] byteArrayToUse = null)
                {
                    if (copyToManagedMemory && byteArrayToUse == null)
                    {
                        byteArrayToUse = new byte[Size];
                    }

                    var planeInfo = new MLCamera.PlaneInfo()
                    {
                        IsValid = true,
                        Width = this.Width,
                        Height = this.Height,
                        Stride = this.Stride,
                        BytesPerPixel = this.BytesPerPixel,
                        PixelStride = this.PixelsStride,
                        DataPtr = this.Data,
                        Size = this.Size,
                        Data = (copyToManagedMemory) ? byteArrayToUse : null
                    };

                    if (planeInfo.Data != null)
                    {
                        uint width = planeInfo.Stride == 0 ? planeInfo.Width : planeInfo.Stride;

                        planeInfo.Data = new byte[width * planeInfo.Height];
                        if (planeInfo.PixelStride == 2)
                        {
                            Marshal.Copy(Data, planeInfo.Data, 0, (int)((width * (planeInfo.Height - 1)) + (planeInfo.Width * planeInfo.PixelStride) - 1));
                        }
                        else
                        {
                            Marshal.Copy(Data, planeInfo.Data, 0, (int)((width * (planeInfo.Height - 1)) + (planeInfo.Width * planeInfo.PixelStride)));
                        }

                        if (planeInfo.Stride != 0)
                        {
                            planeInfo.Size = planeInfo.Stride * planeInfo.Height;
                        }
                    }

                    return planeInfo;
                }
            }

            /// <summary>
            /// Captured output
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLCameraOutput
            {
                /// <summary>
                /// version contains the version number for this structure.
                /// </summary>
                public uint Version;

                /// <summary>
                /// Number of output image planes:
                /// - 1 for compressed output such as JPEG stream,
                /// - 3 for separate color component output such as <c>YUV/YCbCr/RGB.</c>
                /// </summary>
                public byte PlaneCount;

                /// <summary>
                /// Output image plane info.
                /// The number of output planes is specified by PlaneCount.
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeBindings.MLCameraMaxImagePlanes)]
                public MLCameraPlaneInfo[] Planes;

                /// <summary>
                /// Supported output format specified by MLCamera.OutputFormat
                /// </summary>
                public MLCamera.OutputFormat Format;

                /// <summary>
                /// Create and return an initialized version of this struct.
                /// </summary>
                /// <returns>Returns a new MLCamera.Output structure.</returns>
                public static MLCameraOutput Create()
                {
                    return new MLCameraOutput
                    {
                        Version = 0,
                        PlaneCount = 0,
                        Planes = Enumerable.Repeat(MLCameraPlaneInfo.Create(), NativeBindings.MLCameraMaxImagePlanes).ToArray(),
                        Format = MLCamera.OutputFormat.JPEG
                    };
                }

                public MLCamera.CameraOutput CreateFrameInfo(bool copyToManagedMemory, byte[][] byteArraysToUse = null)
                {
                    var frameInfo = new MLCamera.CameraOutput()
                    {
                        Planes = new MLCamera.PlaneInfo[this.PlaneCount],
                        Format = this.Format
                    };

                    for (int i = 0; i < this.PlaneCount; ++i)
                    {
                        frameInfo.Planes[i] = Planes[i].CreatePlaneInfo(copyToManagedMemory, byteArraysToUse?[i]);
                    }

                    return frameInfo;
                }
            }

            /// <summary> 
            /// IntrinsicCalibrationParameters is a structure to provide instrisic calibration information
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLCameraIntrinsicCalibrationParameters
            {
                /// <summary>
                /// version contains the version number for this structure.
                /// </summary>
                public readonly uint Version;

                /// <summary>
                /// Camera width. 
                /// </summary>
                public readonly uint Width;

                /// <summary>
                /// Camera height.
                /// </summary>
                public readonly uint Height;

                /// <summary>
                /// Camera focal length.
                /// </summary>
                public readonly MLVec2f FocalLength;

                /// <summary>
                /// Camera principal point. 
                /// </summary>
                public readonly MLVec2f PrincipalPoint;

                /// <summary>
                /// Field of view in degrees
                /// </summary>
                public readonly float FOV;

                /// <summary>
                /// Distortion vector.
                /// The distortion co-efficients are in the following order:
                /// [k1, k2, p1, p2, k3]
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = MLCameraMaxDistortionCoefficients)]
                public readonly double[] Distortion;
            }

            /// <summary>
            /// ResultExtras is a structure to encapsulate various indices for a capture result.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLCameraResultExtras
            {
                /// <summary>
                /// version contains the version number for this structure.
                /// </summary>
                public readonly uint Version;

                /// <summary>
                /// A 64bit integer to index the frame number associated with this result
                /// </summary>
                public readonly long FrameNumber;

                /// <summary>
                /// VCam exposure timestamp in microseconds (us)
                /// </summary>
                public readonly long VcamTimestamp;

                /// <summary>
                /// Camera intrinsic parameter.
                /// Only valid within callback scope.
                /// The Library allocates and maintains the lifetime of intrinsics. 
                /// Only valid for on_image_buffer_available, on_video_buffer_available, on_preview_buffer_available callbacks.
                /// NULL for on_capture_completed, on_capture_failed callbacks 
                public readonly IntPtr Intrinsics;
            }

            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLCameraDeviceAvailabilityInfo
            {
                public readonly MLCamera.Identifier CamId;
                public readonly IntPtr UserData;
            }

            /// <summary>
            /// Device availability status callbacks to be implemented by client to receive device availability status.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLCameraDeviceAvailabilityStatusCallbacks
            {
                /// <summary>
                /// Version contains the version number for this structure.
                /// </summary>
                public uint Version;

                /// <summary>
                /// Callback is invoked when the camera becomes available.
                /// </summary>
                public DeviceAvailabilityStatusDelegate OnDeviceAvailable;

                /// <summary>
                /// Callback is invoked when the camera becomes unavailable.
                /// </summary>
                public DeviceAvailabilityStatusDelegate OnDeviceUnavailable;

                /// <summary>
                /// Create and return an initialized version of this struct.
                /// </summary>
                /// <returns>A new instance of this struct.</returns>
                public static MLCameraDeviceAvailabilityStatusCallbacks Create()
                {
                    return new MLCameraDeviceAvailabilityStatusCallbacks()
                    {
                        Version = 1,
                        OnDeviceAvailable = OnDeviceAvailableCallback,
                        OnDeviceUnavailable = OnDeviceUnavailableCallback
                    };
                }

                public static MLCameraDeviceAvailabilityStatusCallbacks CreateUninitialized()
                {
                    return new MLCameraDeviceAvailabilityStatusCallbacks()
                    {
                        Version = 1,
                        OnDeviceAvailable = null,
                        OnDeviceUnavailable = null
                    };
                }
            }

            /// <summary>
            /// Device status callbacks to be implemented by client to receive device status if callback mechanism is used.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLCameraDeviceStatusCallbacks
            {

                /// <summary>
                /// Version contains the version number for this structure.
                /// </summary>
                public uint Version;

                /// <summary>
                /// Callback is invoked when the camera is streaming.
                /// </summary>
                public OnDeviceStreamingDelegate OnDeviceStreaming;

                /// <summary>
                /// Callback is invoked when the camera stops streaming.
                /// </summary>
                public OnDeviceIdleDelegate OnDeviceIdle;

                /// <summary>
                /// Callback is invoked when the camera is disconnected.
                /// </summary>
                public OnDeviceDisconnectedDelegate OnDeviceDisconnected;

                /// <summary>
                /// Callback is invoked when the camera encountered errors.  Invalid: The camera device is not available.  Disabled: The
                /// camera device can't be opened due to a device policy.  Device error: The camera device encountered a fatal error such as
                /// the Lightwear device has lost connection or the hardware comm bus is busy and not be able to response and caused a
                /// timeout.  Service error: The camera service has encountered a fatal error and could not provide service.  Capture failed: The
                /// capure request has failed.  on_capture_failed or on_capture_buffer_lost callbacks will be invoked when this error
                /// happens.
                /// </summary>
                public OnDeviceErrorDelegate OnDeviceError;

                /// <summary>
                /// Create and return an initialized version of this struct.
                /// </summary>
                /// <returns>A new instance of this struct.</returns>
                public static MLCameraDeviceStatusCallbacks Create()
                {
                    return new MLCameraDeviceStatusCallbacks()
                    {
                        Version = 1,
                        OnDeviceStreaming = OnDeviceStreamingCallback,
                        OnDeviceIdle = OnDeviceIdleCallback,
                        OnDeviceDisconnected = OnDeviceDisconnectCallback,
                        OnDeviceError = OnDeviceErrorCallback
                    };
                }
            }

            /// <summary>
            /// Capture callbacks to be implemented by client to receive capture status if callback mechanism is used.  This structure
            /// must be initialized by calling MLCameraCaptureCallbacksInit() before use.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLCameraCaptureCallbacks
            {
                /// <summary>
                /// version contains the version number for this structure.
                /// </summary>
                public uint Version;

                /// <summary>
                /// Callback is invoked when a capture has failed when the camera device failed to produce a capture result for the request.
                /// </summary>
                public OnCaptureFailedDelegate OnCaptureFailed;

                /// <summary>
                /// Callback is invoked when an ongoing video or preview capture or both are aborted due to an error.
                /// </summary>
                public OnCaptureAbortedDelegate OnCaptureAborted;

                /// <summary>
                /// Callback is invoked when capturing single frame is completed and result is available.
                /// </summary>
                public OnCaptureCompletedDelegate OnCaptureCompleted;

                /// <summary>
                /// Callback is invoked when a captured image buffer is available with #MLCameraCaptureType_ImageRaw.
                /// </summary>
                public OnImageBufferAvailableDelegate OnImageBufferAvailable;

                /// <summary>
                /// Callback is invoked when a captured raw/compressed video frame buffer is available with #MLCameraCaptureType_VideoRaw.
                /// </summary>
                public OnVideoBufferAvailableDelegate OnVideoBufferAvailable;

                /// <summary>
                /// Callback is invoked when a preview video frame buffer is available with #MLCameraCaptureType_Preview.
                /// </summary>
                public OnPreviewBufferAvailableDelegate OnPreviewBufferAvailable;

                /// <summary>
                /// Create and return an initialized version of this struct.
                /// </summary>
                /// <returns>A new instance of this struct.</returns>
                public static MLCameraCaptureCallbacks Create()
                {
                    return new MLCameraCaptureCallbacks()
                    {
                        Version = 1,
                        OnCaptureFailed = OnCaptureFailCallback,
                        OnCaptureAborted = OnCaptureAbortCallback,
                        OnCaptureCompleted = OnCaptureCompleteCallback,
                        OnImageBufferAvailable = OnImageBufferAvailableCallback,
                        OnVideoBufferAvailable = OnVideoBufferAvailableCallback,
                        OnPreviewBufferAvailable = OnPreviewBufferAvailableCallback
                    };
                }
            }
        }
    }
}
