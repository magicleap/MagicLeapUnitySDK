// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLCamera
    {
        /// <summary>
        /// MLCameraMetadata Summary placeholder.
        /// </summary>
        public partial class Metadata
        {
            public enum ColorCorrectionMode
            {
                /*! Transform Matrix. */
                TransformMatrix = 0,
                /*! Fast. */
                Fast,
                /*! High Quality. */
                HighQuality,
            }

            /*! Color correction aberration mode. */
            public enum ColorCorrectionAberrationMode
            {
                /*! Off. */
                Off = 0,
                /*! Fast. */
                Fast,
                /*! High Quality. */
                HighQuality,
            }

            /*! Control AE anti-banding mode. */
            public enum ControlAEAntibandingMode
            {
                /*! Off. */
                Off = 0,
                /*! 50Hz. */
                Mode_50Hz,
                /*! 60Hz. */
                Mode_60Hz,
                /*! Auto. */
                Auto,
            }

            /*! Control AE lock. */
            public enum ControlAELock
            {
                /*! Off. */
                Off = 0,
                /*! On. */
                On,
            }

            /*!  Control AE mode. */
            public enum ControlAEMode
            {
                /*! Off. */
                Off = 0,
                /*! On. */
                On,
            }

            /*! Control AWB lock. */
            public enum ControlAWBLock
            {
                /*! Off. */
                Off = 0,
                /*! On. */
                On,
            }

            /*! Control AWB mode. */
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

            /*! Control mode. */
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

            /*! Scene mode. */
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

            /*! Effect mode. */
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

            /*! Control AE state. */
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

            /*! Control AWB state. */
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

            /*! Scaler available formats. */
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

            /*! Scaler available stream configuration. */
            public enum ScalerAvailableStreamConfigurations
            {
                /*! Output. */
                OUTPUT = 0,
                /*! Input. */
                INPUT,
            }

            /*! Jpeg Thumbnail Size. */
            public enum JpegThumbnailSize
            {
                /*! 160x120. */
                Size_160x120 = 1,
                /*! 240x135. */
                Size_240x135 = 2,
                /*! 256x135. */
                Size_256x135 = 3,
            }

            /*! Force Apply Metadata Settings. */
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
