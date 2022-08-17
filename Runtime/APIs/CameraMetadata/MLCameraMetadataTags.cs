// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLCameraMetadataTags.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
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
				Off  = 0,
				/*! On. */
				On  = 1
			}
		}
	}
}
