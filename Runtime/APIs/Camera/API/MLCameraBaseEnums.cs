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

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// APIs for accessing Camera Device and to do Camera Capture.
    /// </summary>
    public partial class MLCameraBase
    {
        /// <summary>
        /// Camera errors
        /// </summary>
        public enum ErrorType
        {
            /// <summary>
            /// No error
            /// </summary>
            None = 0,

            /// <summary>
            /// Invalid state
            /// </summary>
            Invalid,

            /// <summary>
            /// Camera disabled
            /// </summary>
            Disabled,

            /// <summary>
            /// Camera device failed
            /// </summary>
            DeviceFailed,

            /// <summary>
            /// Camera service failed
            /// </summary>
            ServiceFailed,

            /// <summary>
            /// Capture failed
            /// </summary>
            CaptureFailed
        }

        /// <summary>
        /// Capture operation type
        /// </summary>
        public enum CaptureType
        {
            /// <summary>
            ///  To capture an image
            /// </summary>
            Image,

            /// <summary>
            ///  To capture a video
            /// </summary>
            Video,

            /// <summary>
            ///  To capture a video and and access the raw buffer of the frames.
            /// </summary>
            Preview,
        }

        /// <summary>
        /// Captured output format
        /// </summary>
        public enum OutputFormat
        {
            /// <summary>
            ///  YUV planar format.
            /// </summary>
            YUV_420_888 = 1,

            /// <summary>
            ///  Compressed output stream.
            /// </summary>
            JPEG,

            /// <summary>
            ///  RGB32 format.
            /// </summary>
            RGBA_8888,
        }

        /// <summary>
        /// Client can implement polling mechanism to retrieve device status
        /// and use these masks to view device status.
        /// </summary>
        [Flags]
        public enum DeviceStatusFlag
        {
            /// <summary>
            ///  Connected
            /// </summary>
            Connected = 1 << 0,

            /// <summary>
            ///  Idle
            /// </summary>
            Idle = 1 << 1,

            /// <summary>
            ///  Opened.
            /// </summary>
            Streaming = 1 << 2,

            /// <summary>
            ///  Disconnected.
            /// </summary>
            Disconnected = 1 << 3,

            /// <summary>
            ///  Error.  Call MLCameraGetErrorCode() to obtain the error code.
            /// </summary>
            Error = 1 << 4,
        }

        /// <summary>
        /// The metadata for the control AE mode.
        /// </summary>
        public enum MetadataControlAEMode
        {
            /// <summary>
            /// The control AE mode: Off.
            /// </summary>
            Off = 0,

            /// <summary>
            /// The control AE mode: On.
            /// </summary>
            On
        }

        /// <summary>
        /// The metadata for the color correction aberration mode.
        /// </summary>
        public enum MetadataColorCorrectionAberrationMode
        {
            /// <summary>
            /// The color correction aberration mode: Off.
            /// </summary>
            Off = 0,

            /// <summary>
            /// The color correction aberration mode: Fast.
            /// </summary>
            Fast,

            /// <summary>
            /// The color correction aberration mode: High Quality.
            /// </summary>
            HighQuality,
        }

        /// <summary>
        /// The metadata for the control AE lock.
        /// </summary>
        public enum MetadataControlAELock
        {
            /// <summary>
            /// The control AE lock: Off
            /// </summary>
            Off = 0,

            /// <summary>
            /// The control AE lock: On
            /// </summary>
            On,
        }

        /// <summary>
        /// The metadata for the control AWB mode.
        /// </summary>
        public enum MetadataControlAWBMode
        {
            /// <summary>
            /// The control AWB mode: Off
            /// </summary>
            Off = 0,

            /// <summary>
            /// The control AWB mode: Auto
            /// </summary>
            Auto,

            /// <summary>
            /// The control AWB mode: Incandescent
            /// </summary>
            Incandescent,

            /// <summary>
            /// The control AWB mode: Fluorescent
            /// </summary>
            Fluorescent,

            /// <summary>
            /// The control AWB mode: Warm Fluorescent
            /// </summary>
            WarmFluorescent,

            /// <summary>
            /// The control AWB mode: Daylight
            /// </summary>
            Daylight,

            /// <summary>
            /// The control AWB mode: Cloudy Day Light
            /// </summary>
            CloudyDaylight,

            /// <summary>
            /// The control AWB mode: Twilight
            /// </summary>
            Twilight,

            /// <summary>
            /// The control AWB mode: Shade
            /// </summary>
            Shade,
        }

        /// <summary>
        /// The metadata for the control AWB lock.
        /// </summary>
        public enum MetadataControlAWBLock
        {
            /// <summary>
            /// The control AWB lock: Off
            /// </summary>
            Off = 0,

            /// <summary>
            /// The control AWB lock: On
            /// </summary>
            On,
        }

        /// <summary>
        /// The metadata for the color correction mode.
        /// </summary>
        public enum MetadataColorCorrectionMode
        {
            /// <summary>
            /// The color correction mode: Transform Matrix
            /// </summary>
            TransformMatrix = 0,

            /// <summary>
            /// The color correction mode: Fast
            /// </summary>
            Fast,

            /// <summary>
            /// The color correction mode: High Quality
            /// </summary>
            HighQuality,
        }

        /// <summary>
        /// The metadata for the control AE anti banding mode.
        /// </summary>
        public enum MetadataControlAEAntibandingMode
        {
            /// <summary>
            /// The control AE anti banding mode: Off
            /// </summary>
            Off = 0,

            /// <summary>
            /// The control AE anti banding mode: 50hz
            /// </summary>
            FiftyHz,

            /// <summary>
            /// The control AE anti banding mode: 60hz
            /// </summary>
            SixtyHz,

            /// <summary>
            /// The control AE anti banding mode: Auto
            /// </summary>
            Auto,
        }

        /// <summary>
        /// The metadata for <c>scaler</c> available formats.
        /// </summary>
        public enum MetadataScalerAvailableFormats
        {
            /// <summary>
            /// RAW16 Format
            /// </summary>
            RAW16 = 0x20,

            /// <summary>
            /// RAW OPAQUE Format
            /// </summary>
            RAW_OPAQUE = 0x24,

            /// <summary>
            /// YV12 Format
            /// </summary>
            YV12 = 0x32315659,

            /// <summary>
            /// <c>YCrCb 420 SP Format</c>
            /// </summary>
            YCrCb_420_SP = 0x11,

            /// <summary>
            /// Implementation Defined
            /// </summary>
            IMPLEMENTATION_DEFINED = 0x22,

            /// <summary>
            /// <c>YCbCr 420 888 Format</c>
            /// </summary>
            YCbCr_420_888 = 0x23,

            /// <summary>
            /// BLOB Format
            /// </summary>
            BLOB = 0x21,
        }

        /// <summary>
        /// The metadata for <c>scaler</c> available stream configurations.
        /// </summary>
        public enum MetadataScalerAvailableStreamConfigurations
        {
            /// <summary>
            /// The <c>scaler</c> available stream configuration: Output
            /// </summary>
            OUTPUT = 0,

            /// <summary>
            /// The <c>scaler</c> available stream configuration: Input
            /// </summary>
            INPUT,
        }

        /// <summary>
        /// The metadata for the control AE state.
        /// </summary>
        public enum MetadataControlAEState
        {
            /// <summary>
            /// The control AE state: Inactive
            /// </summary>
            Inactive = 0,

            /// <summary>
            /// The control AE state: Searching
            /// </summary>
            Searching,

            /// <summary>
            /// The control AE state: Converged
            /// </summary>
            Converged,

            /// <summary>
            /// The control AE state: Locked
            /// </summary>
            Locked,

            /// <summary>
            /// The control AE state: Flash Required
            /// </summary>
            FlashRequired,

            /// <summary>
            /// The control AE state: Pre-capture
            /// </summary>
            PreCapture,
        }

        /// <summary>
        /// The metadata for the control AWB state.
        /// </summary>
        public enum MetadataControlAWBState
        {
            /// <summary>
            /// The control AWB state: Inactive
            /// </summary>
            Inactive = 0,

            /// <summary>
            /// The control AWB state: Searching
            /// </summary>
            Searching,

            /// <summary>
            /// The control AWB state: Converged
            /// </summary>
            Converged,

            /// <summary>
            /// The control AWB state: Locked
            /// </summary>
            Locked,
        }

        /// <summary>
        /// MR Video Quality enumeration
        /// </summary>
        public enum MRQuality
        {
            /// <summary>
            /// Specifies 648 x 720 resolution.
            /// Aspect ratio: 9x10.
            /// </summary>
            _648x720 = 1,
            
            /// <summary>
            /// Specifies 972 x 1080 resolution.
            /// Aspect ratio: 9x10.
            /// </summary>
            _972x1080 = 2,
            
            /// <summary>
            /// Specifies 1944 x 2160 resolution.
            /// Aspect ratio: 9x10.
            /// CaptureFrameRate._60FPS is not supported for this quality mode.
            /// </summary>
            _1944x2160 = 3,
            
            /// <summary>
            /// Specifies 960 x 720 resolution.
            /// Aspect ratio: 4x3.
            /// </summary>
            _960x720 = 4,
            
            /// <summary>
            /// Specifies 1440 x 1080 resolution.
            /// Aspect ratio: 4x3.
            /// </summary>
            _1440x1080 = 5,
            
            /// <summary>
            /// Specifies 2880 x 2160 resolution.
            /// Aspect ratio: 4x3.
            /// CaptureFrameRate._60FPS is not supported for this quality mode.
            /// </summary>
            _2880x2160 = 6,
        };

        /// <summary>
        /// Capture Frame Rate
        /// Call MLCameraPrepareCapture to configure frame rate
        /// use FrameRate_None when configuring only Image capture
        /// FrameRate_60fps only supported when resolution of captures <= 1080P.
        /// </summary>
        public enum CaptureFrameRate
        {
            /// <summary>
            ///  None Still Capture
            /// </summary>
            None,

            /// <summary>
            ///  Specified 15FPS
            /// </summary>
            _15FPS,

            /// <summary>
            ///  Specified 30FPS
            /// </summary>
            _30FPS,

            /// <summary>
            ///  Specified 60FPS
            /// </summary>
            _60FPS,
        };

        /// <summary>
        /// Camera Disconnect Reason.
        /// </summary>
        public enum DisconnectReason
        {
            /// <summary>
            ///  Device Lost.
            /// </summary>
            DeviceLost,

            /// <summary>
            ///  Priority Lost.
            /// </summary>
            PriorityLost,
        };

        /// <summary>
        /// Logical Camera identifiers available for access.
        /// </summary>
        public enum Identifier
        {
            /// <summary>
            ///  x86 logical camera
            /// </summary>
            Main = 0,

            /// <summary>
            ///  CV logical camera
            /// </summary>
            CV,
        };

        /// <summary>
        /// Flags to describe various modules in camera pipeline.
        /// </summary>
        public enum ConnectFlag
        {
            /// <summary>
            ///  Camera only frame capture
            /// </summary>
            CamOnly,

            /// <summary>
            ///  virtual only capture
            /// </summary>
            VirtualOnly,

            /// <summary>
            ///  Mixed Reality capture
            /// </summary>
            MR,
        };

        /// <summary>
        /// Comment Needed!
        /// </summary>
        public enum MRBlendType
        {
            /// <summary>
            ///  Additive Blend Type. It simply adds pixel values of real world and virtual layer.
            /// </summary>
            Additive = 1
        };
    }
}
