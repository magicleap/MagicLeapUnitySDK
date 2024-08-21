// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;

    public partial class MLCameraBase
    {
        /// <summary>
        /// MLCameraMetadata Summary placeholder.
        /// </summary>
        public partial class Metadata
        {
            /// <summary>
            /// Color correction mode
            /// </summary>
            public enum ColorCorrectionMode
            {
                /// <summary>
                /// Color correction by matrix transformation
                /// </summary>
                TransformMatrix = 0,
                /// <summary>
                /// Fast color correction
                /// </summary>
                Fast,
                /// <summary>
                /// High quality color correction
                /// </summary>
                HighQuality,
            }

            /// <summary>
            /// Color correction aberration mode. 
            /// </summary>
            public enum ColorCorrectionAberrationMode
            {
                /// <summary>
                /// Disable color correction aberration
                /// </summary>
                Off = 0,
                /// <summary>
                /// Fast color correction aberration
                /// </summary>
                Fast,
                /// <summary>
                /// High quality color correction aberration
                /// </summary>
                HighQuality,
            }

            /// <summary>
            /// Control AE anti-banding mode. 
            /// </summary>
            public enum ControlAEAntibandingMode
            {
                /// <summary>
                /// Disable AE anti-banding
                /// </summary>
                Off = 0,
                /// <summary>
                /// 50Hz AE anti-banding
                /// </summary>
                Mode_50Hz,
                /// <summary>
                /// 60Hz AE anti-banding
                /// </summary>
                Mode_60Hz,
                /// <summary>
                /// Automatic AE anti-banding
                /// </summary>
                Auto,
            }

            /// <summary>
            /// Control AE Lock
            /// </summary>
            public enum ControlAELock
            {
                Off = 0,
                On,
            }

            /// <summary>
            ///  Control AE mode. 
            /// </summary>
            public enum ControlAEMode
            {
                /*! Off. */
                Off = 0,
                /*! On. */
                On,
            }

            /// <summary>
            /// Control AWB lock. 
            /// </summary>
            public enum ControlAWBLock
            {
                /*! Off. */
                Off = 0,
                /*! On. */
                On,
            }

            /// <summary>
            /// Control AWB mode. 
            /// </summary>
            public enum ControlAWBMode
            {
                /*! Off. */
                Off = 0,
                /*! Auto. */
                Auto,
                /*! Incandescent. */
                Incandescent,
                /*! Fluorescent. */
                Fluorescent,
                /*! Warm fluorescent. */
                WarmFluorescent,
                /*! Daylight. */
                Daylight,
                /*! Cloudy daylight. */
                CloudyDaylight,
                /*! Twilight. */
                Twilight,
                /*! Shade. */
                Shade,
            }

            /// <summary>
            /// The current auto-focus (AF) mode controls.
            /// </summary>
            public enum ControlAFMode
            {
                /// <summary>
                /// Disables the camera device's auto-focus routine. 
                /// </summary>
                Off = 0,

                /// <summary>
                /// Sets the camera device's auto-focus routine to automatic.
                /// </summary>
                Auto,

                /// <summary>
                /// Sets the camera device's auto-focus routine to close-up focusing mode.
                /// </summary>
                Macro,

                /// <summary>
                /// Sets the camera device's auto-focus routine to Continuous Video Mode. 
                /// The focusing behavior should be suitable for good quality video recording.
                /// </summary>
                ContinuousVideo,

                /// <summary>
                /// Sets the camera device's auto-focus routine to Continuous Picture Mode. 
                /// The focusing behavior should be suitable for still image capture.
                /// </summary>
                ContinuousPicture
            }

            /// <summary>
            /// The current auto-focus (AF) trigger.
            /// </summary>
            public enum ControlAFTrigger
            {
                /// <summary>
                /// AF trigger is idle.
                /// </summary>
                Idle = 0,

                /// <summary>
                /// AF will trigger now.
                /// </summary>
                Start,

                /// <summary>
                /// Cancel any currently active AF trigger.
                /// </summary>
                Cancel
            }

            /// <summary>
            /// Control mode. 
            /// </summary>
            public enum ControlMode
            {
                /*! Off. */
                Off = 0,
                /*! Auto. */
                Auto,
                /*! Use Scene Mode */
                UseSceneMode,
                /*! no update of 2A state */
                OffKeepState,
            }

            /// <summary>
            /// Scene mode. 
            /// </summary>
            public enum ControlSceneMode
            {
                /*! Action. */
                Action = 2,
                /*! Portrait. */
                Portrait = 3,
                /*! Landscape. */
                Landscape = 4,
                /*! Theatre. */
                Theatre = 7,
                /*! Sports. */
                Sports = 13,
                /*! Party. */
                Party = 14,
                /*! CandleLight. */
                CandleLight = 15,
                /*! Barcode. */
                Barcode = 16,
                /*! Medical. */
                Medical = 100,
            }

            /// <summary>
            /// Effect mode. 
            /// </summary>
            public enum ControlEffectMode
            {
                /*! Off. */
                Off = 0,
                /*! GrayScale. */
                Grayscale,
                /*! Negative. */
                Negative,
                /*! Sepia. */
                Sepia,
                /*! ColorSelection. */
                ColorSelection,
                /*! Sharpening. */
                Sharpening,
                /*! Emboss. */
                Emboss,
                /*! Sketch. */
                Sketch,
            }

            /// <summary>
            /// Control AE state. 
            /// </summary>
            public enum ControlAEState
            {
                /*! Inactive. */
                Inactive = 0,
                /*! Searching. */
                Searching,
                /*! Converged. */
                Converged,
                /*! Locked. */
                Locked,
                /*! Flash required. */
                FlashRequired,
                /*! Pre-capture. */
                PreCapture,
            }

            /// <summary>
            /// Control AWB state. 
            /// </summary>
            public enum ControlAWBState
            {
                /*! Inactive. */
                Inactive = 0,
                /*! Searching. */
                Searching,
                /*! Converged. */
                Converged,
                /*! Locked. */
                Locked,
            }

            /// <summary>
            /// Current state of auto-focus (AF) routine.
            /// </summary>
            public enum ControlAFState
            {
                /// <summary>
                /// AF is off or has not yet tried to scan.
                /// </summary>
                Inactive = 0,

                /// <summary>
                /// AF is currently performing an AF scan initiated by the camera device in a continuous autofocus mode.
                /// </summary>
                PassiveScan,

                /// <summary>
                /// AF currently is in focus, but may restart scanning at any time
                /// </summary>
                PassiveFocused,

                /// <summary>
                /// AF is performing an AF scan triggered by AF trigger
                /// </summary>
                ActiveScan,

                /// <summary>
                /// AF is focused correctly and has locked focus.
                /// </summary>
                FocusedLocked,

                /// <summary>
                /// AF has failed to focus successfully and has locked focus.
                /// </summary>
                NotFocusedLocked,

                /// <summary>
                /// AF finished a passive scan without finding focus,and may restart scanning at any time.
                /// </summary>
                PassiveUnFocused
            }

            /// <summary>
            /// Whether a significant scene change is detected by AF algorithm.
            /// </summary>
            public enum ControlAFSceneChange
            {
                /// <summary>
                /// No scene change detected by AF.
                /// </summary>
                NotDetected = 0,

                /// <summary>
                /// Scene change detected by AF.
                /// </summary>
                Detected
            }

            /// <summary>
            ///  Current Lens status.
            /// </summary>
            public enum LensState
            {
                /// <summary>
                /// Lens focal length is not changing.
                /// </summary>
                Stationary = 0,

                /// <summary>
                /// Lens focal length is changing.
                /// </summary>
                Moving
            }

            /// <summary>
            /// Scaler available formats. 
            /// </summary>
            public enum ScalerAvailableFormats
            {
                /*! Raw16. */
                RAW16 = 0x20,
                /*! Raw opaque. */
                RAW_OPAQUE = 0x24,
                /*! TV12. */
                YV12 = 0x32315659,
                /*! YCrCb 420 SP. */
                YCrCb_420_SP = 0x11,
                /*! Implementation defined. */
                IMPLEMENTATION_DEFINED = 0x22,
                /*! YCrCb 420 888. */
                YCbCr_420_888 = 0x23,
                /*! BLOB. */
                BLOB = 0x21,
            }

            /// <summary>
            /// Scaler available stream configuration. 
            /// </summary>
            public enum ScalerAvailableStreamConfigurations
            {
                /*! Output. */
                OUTPUT = 0,
                /*! Input. */
                INPUT,
            }

            /// <summary>
            /// Jpeg Thumbnail Size. 
            /// </summary>
            public enum JpegThumbnailSize
            {
                /*! 160x120. */
                Size_160x120 = 1,
                /*! 240x135. */
                Size_240x135 = 2,
                /*! 256x135. */
                Size_256x135 = 3,
            }

            /// <summary>
            /// Force Apply Metadata Settings. 
            /// </summary>
            public enum ControlForceApplyMode
            {
                /*! Off. */
                Off = 0,
                /*! On. */
                On = 1
            }
        }
    }
}
