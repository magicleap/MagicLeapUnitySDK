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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// APIs for accessing Camera Device and to do Camera Capture.
    /// </summary>
    public partial class MLCameraBase
    {
        /// <summary>
        /// Connection settings for MR capture.
        /// </summary>
        public struct MRConnectInfo
        {
            /// <summary>
            /// video quality
            /// </summary>
            public MRQuality MRQuality;

            /// <summary>
            /// blending type for mixed reality capture
            /// </summary>
            public MRBlendType MRBlendType;

            /// <summary>
            /// capture frame rate
            /// </summary>
            public CaptureFrameRate FrameRate;

            /// <summary>
            /// Create and return an initialized version of this struct.
            /// </summary>
            /// <returns>A new instance of this struct.</returns>
            public static MRConnectInfo Create()
            {
                return new MRConnectInfo()
                {
                    MRQuality = MRQuality._1440x1080,
                    MRBlendType = MRBlendType.Additive,
                    FrameRate = CaptureFrameRate._30FPS
                };
            }

            public override string ToString()
            {
                return $"Quality : {MRQuality}, BlendType : {MRBlendType}, FrameRate : {FrameRate}";
            }
        }

        /// <summary>
        /// Camera connection settings.
        /// </summary>
        public struct ConnectContext
        {
            /// <summary>
            /// logical camera idenitifier
            /// </summary>
            public Identifier CamId;

            /// <summary>
            /// The Context in which the camera should operate in.
            /// </summary>
            public ConnectFlag Flags;

            /// <summary>
            /// Video Stabilization enable/disable,
            /// true - enable; false - disabled
            /// </summary>
            public bool EnableVideoStabilization;

            /// <summary>
            /// MR capture connection settings.
            /// Ignored for camera only capture.
            /// </summary>
            public MRConnectInfo MixedRealityConnectInfo;

            /// <summary>
            /// Create and return an initialized version of this struct.
            /// </summary>
            /// <returns>A new instance of this struct.</returns>
            public static ConnectContext Create()
            {
                return new ConnectContext()
                {
                    CamId = Identifier.Main,
                    Flags = ConnectFlag.CamOnly,
                    EnableVideoStabilization = false,
                    MixedRealityConnectInfo = MRConnectInfo.Create()
                };
            }

            public override string ToString()
            {
                return $"CamId : {CamId}, Flags : {Flags}, Stabilization : {EnableVideoStabilization}, MRConnectInfo : {MixedRealityConnectInfo}";
            }
        }

        public struct StreamCapability
        {
            /// <summary>
            /// Capture Type
            /// </summary>
            public CaptureType CaptureType;

            /// <summary>
            /// Resolution height
            /// </summary>
            public int Width;

            /// <summary>
            /// Resolution height 
            /// </summary>
            public int Height;

            public override string ToString()
            {
                return $"CaptureType : {CaptureType}, Width : {Width}, Height : {Height}";
            }
        }


        public struct StreamCapabilitiesInfo
        {
            public StreamCapability[] StreamCapabilities;
        }

        public struct CaptureStreamConfig
        {
            /// <summary>
            /// Capture Type
            /// </summary>
            public CaptureType CaptureType;

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
            public OutputFormat OutputFormat;

            /// <summary>
            /// Media recorder surface, only valid for capture type video * set to ML_INVALID_HANDLE for yuv/rgba video capture
            /// </summary>
            public MLNativeSurface Surface;

            public static CaptureStreamConfig Create(StreamCapability streamCapability, OutputFormat outputFormat, MLNativeSurface recorderSurface = null)
            {
                return new CaptureStreamConfig()
                {
                    Width = streamCapability.Width,
                    Height = streamCapability.Height,
                    CaptureType = streamCapability.CaptureType,
                    OutputFormat = outputFormat,
                    Surface = recorderSurface
                };
            }

            public override string ToString()
            {
                return $"CaptureType : {CaptureType}, Width : {Width}, Height : {Height}, OutputFormat : {OutputFormat}, MediaRecorder : {(Surface == null ? "NULL" : Surface.Handle.ToString())}";
            }
        }

        public struct CaptureConfig
        {
            /// <summary>
            /// capture frame rate
            /// </summary>
            public CaptureFrameRate CaptureFrameRate;

            /// <summary>
            /// Stream configurations
            /// </summary>
            public CaptureStreamConfig[] StreamConfigs;
        }

        /// <summary>
        /// Camera intrinsic parameters.
        /// </summary>
        public readonly struct IntrinsicCalibrationParameters
        {
            public readonly uint Width;
            public readonly uint Height;
            public readonly Vector2 FocalLength;
            public readonly Vector2 PrincipalPoint;
            public readonly float FOV;

            /// <summary>
            /// Distortion vector.
            /// The distortion co-efficients are in the following order:
            /// [k1, k2, p1, p2, k3]
            /// </summary>
            public readonly double[] Distortion;

            public IntrinsicCalibrationParameters(uint width, uint height, in Vector2 focalLength, in Vector2 principalPoint, float fov, double[] distortion)
            {
                Width = width;
                Height = height;
                FocalLength = focalLength;
                PrincipalPoint = principalPoint;
                FOV = fov;
                Distortion = distortion;
            }

            public override string ToString()
            {
                return $"Width : {Width}, Height : {Height}, FocalLength : {FocalLength}, PrincipalPoint : {PrincipalPoint}, FOV : {FOV}, Distortion : {string.Join(", ", Distortion)}";
            }
        }

        public readonly struct ResultExtras
        {
            /// <summary>
            /// Frame number associated with this result.
            /// </summary>
            public readonly long FrameNumber;

            /// <summary>
            /// Frame timestamp in microseconds.
            /// </summary>
            public readonly MLTime VCamTimestamp;

            /// <summary> 
            /// Intrinsic calibration parameters.
            /// </summary>
            public readonly IntrinsicCalibrationParameters? Intrinsics;

            internal ResultExtras(long frameNumber, long vcamTimestampUs, in IntrinsicCalibrationParameters? intrinsics)
            {
                FrameNumber = frameNumber;
                VCamTimestamp = vcamTimestampUs;
                Intrinsics = intrinsics;
            }
        }

        /// <summary>
        /// Contains both the data and information necessary to read the data for a specific buffer in a capture
        /// </summary>
        public struct PlaneInfo
        {
            /// <summary>
            /// Indicate if this structure contains valid data
            /// </summary>
            public bool IsValid;

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

            /// <summary>
            /// Distance between 2 consecutive pixels in bytes.
            /// </summary>
            public uint PixelStride;

            /// <summary>
            /// Image Data (in managed memory)
            /// </summary>
            public byte[] Data;

            /// <summary>
            /// Pointer to the unmanaged memory where the actual image data is found.
            /// Only valid when received via the OnRawVideoFrameAvailableYUV_NativeCallbackThread event.
            /// </summary>
            public IntPtr DataPtr;

            /// <summary>
            /// Number of bytes in the image output data
            /// </summary>
            public uint Size;

            /// <summary>
            /// Create and return an initialized version of this struct.
            /// </summary>
            /// <returns>Returns a new YUVBuffer structure.</returns>
            public static PlaneInfo Create()
            {
                return new PlaneInfo
                {
                    IsValid = false,
                    Width = 0u,
                    Height = 0u,
                    Stride = 0u,
                    BytesPerPixel = 0u,
                    PixelStride = 0u,
                    Data = new byte[0],
                    Size = 0u
                };
            }

            public override string ToString()
            {
                return $"IsValid: {IsValid}, Width: {Width}, Height: {Height}, Stride: {Stride}, BytesPerPixel: {BytesPerPixel} " +
                    $"PixelStride: {PixelStride}, ByteBufferLength: {Data?.Length}, DataPtr: {DataPtr}, Size: {Size}";
            }
        }

        /// <summary>
        /// Contains the information and data of each of the available buffers/planes in a capture
        /// Number of planes for JPEG and RGBA captures = 1
        /// Number of planes for YUV captures = 3
        /// </summary>
        public struct CameraOutput
        {
            public PlaneInfo[] Planes;

            public OutputFormat Format;

            public override string ToString()
            {
                string result = $"Format: {Format}\n";
                for (int i = 0; i < Planes.Length; ++i)
                {
                    result += $"Plane[{i}]: {Planes[i]}\n";
                }
                return result;
            }
        }
    }
}
