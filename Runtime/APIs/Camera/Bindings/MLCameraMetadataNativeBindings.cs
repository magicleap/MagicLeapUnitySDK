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
    using System.Runtime.InteropServices;

    public partial class MLCameraBase
    {
        /// <summary>
        /// MLCameraMetadata description goes here.
        /// </summary>
        public partial class Metadata
        {
            /// <summary>
            /// See ml_camera_metadata_v2.h for additional comments.
            /// </summary>
            private class NativeBindings : Native.MagicLeapNativeBindings
            {
                /// <summary>
                /// Rational data type.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLCameraMetadataRational
                {

                    /// <summary>
                    /// Numerator.
                    /// </summary>
                    public int Numerator;

                    /// <summary>
                    /// Denominator.
                    /// </summary>
                    public int Denominator;

                    /// <summary>
                    /// Create and return an initialized version of this struct.
                    /// </summary>
                    /// <returns>A new instance of this struct.</returns>
                    public static MLCameraMetadataRational Create()
                    {
                        return new MLCameraMetadataRational()
                        {
                        };
                    }
                };

                /// <summary>
                /// Gets sensor sensitivity.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetSensorSensitivityRequestMetadata(ulong Handle, out int OutData);

                /// <summary>
                /// Sets AE lock.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetControlAELock(
                    ulong Handle, ref ControlAELock Data);

                /// <summary>
                /// Gets AE Max Regions.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAEMaxRegions(ulong Handle, out int OutData);

                /// <summary>
                /// Gets AE mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAEModeResultMetadata(
                    ulong Handle, out ControlAEMode OutData);

                /// <summary>
                /// Gets AE antibanding mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAEAntibandingModeResultMetadata(
                    ulong Handle, out ControlAEAntibandingMode OutData);

                /// <summary>
                /// Sets force apply mode.  Main camera and CV Camera share the same camera hardware resources.  When both the cameras are
                /// streaming, request metadata properties for both cameras are merged and then applied.  While merging, the metadata
                /// properties from Main Camera take precedence over CV camera metadata properties.  The force apply mode property can be used to
                /// override this.  If CV Camera metadata has force apply mode on, the CV Camera metadata properties take precedence over
                /// Main Camera metadata properties.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetControlForceApplyMode(
                    ulong Handle,
                    ControlForceApplyMode Data);

                /// <summary>
                /// Gets AE compensation step.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAECompensationStep(ulong Handle, IntPtr OutData);

                /// <summary>
                /// Gets color correction transform.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetColorCorrectionTransformRequestMetadata(ulong Handle, IntPtr OutData);

                /// <summary>
                /// Gets sensor info sensitivity range.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetSensorInfoSensitivityRange(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] int[] OutData);

                /// <summary>
                /// Sets Effect mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetControlEffectMode(
                    ulong Handle, ref ControlEffectMode Data);

                /// <summary>
                /// Sets color correction aberration.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetColorCorrectionAberrationMode(
                    ulong Handle, ref ColorCorrectionAberrationMode Data);

                /// <summary>
                /// Gets color correction aberration modes.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetColorCorrectionAvailableAberrationModes(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.LPArray)] out IntPtr OutData,
                    out int OutCount);

                /// <summary>
                /// Gets color correction aberration.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetColorCorrectionAberrationModeRequestMetadata(
                    ulong Handle, out ColorCorrectionAberrationMode OutData);

                /// <summary>
                /// Gets AE lock.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAELockAvailable(
                    ulong Handle, out ControlAELock OutData);

                /// <summary>
                /// Gets jpeg GPS coordinates.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetJpegGPSCoordinatesRequestMetadata(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] OutData);

                /// <summary>
                /// Gets Effect modes.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAvailableEffectModes(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] out ControlEffectMode[] OutData,
                    [MarshalAs(UnmanagedType.SysUInt)] out int OutCount);

                /// <summary>
                /// Gets Scene modes.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAvailableSceneModes(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] out ControlSceneMode[] OutData,
                    [MarshalAs(UnmanagedType.SysUInt)] out int OutCount);

                /// <summary>
                /// Gets AE modes.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAEAvailableModes(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] out ControlAEMode[] OutData,
                    [MarshalAs(UnmanagedType.SysUInt)] out int OutCount);

                /// <summary>
                /// Gets AE compensation range.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAECompensationRange(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.LPArray, SizeConst = 2, ArraySubType = UnmanagedType.SysInt)] out int[] OutData);

                /// <summary>
                /// Gets AWB modes.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAWBAvailableModes(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] out ControlAWBMode[] OutData,
                    [MarshalAs(UnmanagedType.SysUInt)] out int OutCount);

                /// <summary>
                /// Gets AF modes
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAFAvailableModes(
                    ulong Handle, 
                    [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] out ControlAFMode[] OutData, 
                    [MarshalAs(UnmanagedType.SysUInt)] out int OutCount);

                /// <summary>
                /// Gets lens hyperfocal distance.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetLensInfoHyperfocalDistance(ulong Handle, out float OutData);

                /// <summary>
                /// Gets lens minimum focus distance
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetLensInfoMinimumFocusDistance(ulong Handle, out float OutData);

                /// <summary>
                /// Gets lens minimum focus distance increment.
                /// The lens position can be set in multiples of distance increments between hyperfocal distance and minimum focus distance.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetLensInfoMinimumFocusDistanceIncrement(ulong Handle, out float OutData);

                /// <summary>
                /// Gets AF mode
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAFModeRequestMetadata(ulong Handle, out ControlAFMode OutData);

                /// <summary>
                /// Gets AF trigger
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAFTriggerRequestMetadata(ulong Handle, out ControlAFTrigger OutData);

                /// <summary>
                /// Gets AF distance range. Both Min and Max are specified as focus distance in diopters.
                /// The min value specifies the lower bound for focus distance used by Autofocus algorithm and should be
                /// more than min focus distance which can be queried using <see cref="MLCameraMetadataGetLensInfoMinimumFocusDistance"/>.
                /// The max value specifies the upper bound for focus distance used by Autofocus algorithm and should be
                /// less than hyperfocal distance which can be queried using api <see cref="MLCameraMetadataGetLensInfoHyperfocalDistance"/>.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetAFDistanceRangeRequestMetadata(ulong Handle, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] out float[] OutData);

                /// <summary>
                /// Gets Lens focus distance in diopters.
                /// The distance is between minimum focus distance and hyperfocal distance.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetLensFocusDistanceRequestMetadata(ulong Handle, out float OutData);

                /// <summary>
                /// Sets AF mode
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetControlAFMode(ulong Handle, in ControlAFMode InData);

                /// <summary>
                /// Sets AF trigger
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetControlAFTrigger(ulong Handle, in ControlAFTrigger InData);

                /// <summary>
                /// Sets AF distance range. Both min and max are specified as focus distance in diopters.
                /// The min value specifies the lower bound for focus distance used by Autofocus algorithm and should be
                /// more than min focus distance which can be queried using <see cref="MLCameraMetadataGetLensInfoMinimumFocusDistance"/>.
                /// The max value specifies the upper bound for focus distance used by Autofocus algorithm and should be
                /// less than hyperfocal distance which can be queried using api <see cref="MLCameraMetadataGetLensInfoHyperfocalDistance"/>.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetAFDistanceRange(ulong Handle, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] in float[] InData);

                /// <summary>
                /// Sets Lens focus distance
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetLensFocusDistance(ulong Handle, in float InData);

                /// <summary>
                /// Gets AF mode
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAFModeResultMetadata(ulong Handle, out ControlAFMode OutData);

                /// <summary>
                /// Gets AF trigger
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAFTriggerResultMetadata(ulong Handle, out ControlAFTrigger OutData);

                /// <summary>
                /// Gets AF distance range. Both Min and Max are specified as focus distance in diopters.
                /// The min value specifies the lower bound for focus distance used by Autofocus algorithm and should be
                /// more than min focus distance which can be queried using <see cref="MLCameraMetadataGetLensInfoMinimumFocusDistance"/>.
                /// The max value specifies the upper bound for focus distance used by Autofocus algorithm and should be
                /// less than hyperfocal distance which can be queried using api <see cref="MLCameraMetadataGetLensInfoHyperfocalDistance"/>.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetAFDistanceRangeResultMetadata(ulong Handle, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] out float[] OutData);

                /// <summary>
                /// Gets AF state
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAFStateResultMetadata(ulong Handle, out ControlAFState OutData);

                /// <summary>
                /// Gets AF scene change detection status
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAFSceneChangeResultMetadata(ulong Handle, out ControlAFSceneChange OutData);

                /// <summary>
                /// Gets Lens focus distance
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetLensFocusDistanceResultMetadata(ulong Handle, out float OutData);

                /// <summary>
                /// Gets Lens state
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetLensStateResultMetadata(ulong Handle, out LensState OutData);

                /// <summary>
                /// Gets AE regions.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAERegionsRequestMetadata(
                    ulong Handle,
                    IntPtr OutData,
                    out int OutCount);

                /// <summary>
                /// Gets Available Modes.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAvailableModes(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] out ControlMode[] OutData,
                    [MarshalAs(UnmanagedType.SysUInt)] out int OutCount);

                /// <summary>
                /// Sets color correction transform.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetColorCorrectionTransform(
                    ulong Handle,
                    IntPtr Data);

                /// <summary>
                /// Gets AWB lock.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAWBLockAvailable(
                    ulong Handle,
                    out ControlAWBLock OutData);

                /// <summary>
                /// Gets sensor info active array sizes.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetSensorInfoActiveArraySize(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] int[] OutData);

                /// <summary>
                /// Gets color correction mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetColorCorrectionModeRequestMetadata(
                    ulong Handle,
                    out ColorCorrectionMode OutData);

                /// <summary>
                /// Gets sensor orientation degree.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetSensorOrientation(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.SysInt)] out int OutData);

                /// <summary>
                /// Gets color correction gains.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetColorCorrectionGainsRequestMetadata(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] float[] OutData);

                /// <summary>
                /// Gets color correction transform.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetColorCorrectionTransformResultMetadata(
                    ulong Handle,
                    IntPtr OutData);

                /// <summary>
                /// Gets AE antibanding mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAEAntibandingModeRequestMetadata(
                    ulong Handle,
                    out ControlAEAntibandingMode OutData);

                /// <summary>
                /// Gets AE exposure compensation.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAEExposureCompensationRequestMetadata(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.SysInt)] out int OutData);

                /// <summary>
                /// Gets AE lock.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAELockRequestMetadata(
                    ulong Handle,
                    out ControlAELock OutData);

                /// <summary>
                /// Gets AE mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAEModeRequestMetadata(
                    ulong Handle,
                    out ControlAEMode OutData);

                /// <summary>
                /// Gets AWB lock.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAWBLockRequestMetadata(
                    ulong Handle,
                    out ControlAWBLock OutData);

                /// <summary>
                /// Gets AWB mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAWBModeRequestMetadata(
                    ulong Handle,
                    out ControlAWBMode OutData);

                /// <summary>
                /// Gets mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlModeRequestMetadata(
                    ulong Handle,
                    out ControlMode OutData);

                /// <summary>
                /// Gets Scene mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlSceneModeRequestMetadata(
                    ulong Handle,
                    out ControlSceneMode OutData);

                /// <summary>
                /// Gets sensor exposure time.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetSensorExposureTimeRequestMetadata(
                    ulong Handle,
                    out long OutData);

                /// <summary>
                /// Effect mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlEffectModeRequestMetadata(
                    ulong Handle,
                    out ControlEffectMode OutData);

                /// <summary>
                /// Gets AE lock.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAELockResultMetadata(
                    ulong Handle,
                    out ControlAELock OutData);

                /// <summary>
                /// Gets exposure time upper limit.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlExposureUpperTimeLimitRequestMetadata(
                    ulong Handle,
                    out long OutData);

                /// <summary>
                /// Gets jpeg timestamp.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetJpegGPSTimestampRequestMetadata(
                    ulong Handle,
                    out long OutData);

                /// <summary>
                /// Gets jpeg thumbnail size.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetJpegThumbnailSizeRequestMetadata(
                    ulong Handle,
                    out JpegThumbnailSize OutData);

                /// <summary>
                /// Gets force apply mode. Main camera and CV Camera share the same camera hardware resources. When both the cameras are
                /// streaming, request metadata properties for both cameras are merged and then applied. While merging, the metadata
                /// properties from Main Camera take precedence over CV camera metadata properties. The force apply mode property can be used to
                /// override this. If CV Camera metadata has force apply mode on, the CV Camera metadata properties take precedence over
                /// Main Camera metadata properties.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlForceApplyModeRequestMetadata(
                    ulong Handle,
                    out ControlForceApplyMode OutData);

                /// <summary>
                /// Gets jpeg quality.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetJpegQualityRequestMetadata(
                    ulong Handle,
                    out byte OutData);

                /// <summary>
                /// Sets color correction mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetColorCorrectionMode(
                    ulong Handle,
                    ref ColorCorrectionMode Data);

                /// <summary>
                /// Gets color correction gains.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetColorCorrectionGainsResultMetadata(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] float[] OutData);

                /// <summary>
                /// Sets color correction gains.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetColorCorrectionGains(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] float[] Data);

                /// <summary>
                /// Sets AE antiband mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetControlAEAntibandingMode(
                    ulong Handle,
                    ref ControlAEAntibandingMode Data);

                /// <summary>
                /// Sets AE exposure compensation.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetControlAEExposureCompensation(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.SysInt)] ref int Data);

                /// <summary>
                /// Sets AE mode.  MLCameraMetadataControlAEMode_Off is not supported if camera is configured for 15FPS
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetControlAEMode(
                    ulong Handle,
                    ref ControlAEMode Data);

                /// <summary>
                /// set AE regions.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetControlAERegions(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.LPArray, SizeConst = 15)] IntPtr Data,
                    int Count);

                /// <summary>
                /// Sets AWB lock.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetControlAWBLock(
                    ulong Handle,
                    ref ControlAWBLock Data);

                /// <summary>
                /// Sets AWB mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetControlAWBMode(
                    ulong Handle,
                    ref ControlAWBMode Data);

                /// <summary>
                /// Sets mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetControlMode(
                    ulong Handle,
                    ref ControlMode Data);

                /// <summary>
                /// Sets Scene mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetControlSceneMode(
                    ulong Handle,
                    ref ControlSceneMode Data);

                /// <summary>
                /// Gets AWB lock.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAWBLockResultMetadata(
                    ulong Handle,
                    out ControlAWBLock OutData);

                /// <summary>
                /// Sets sensor exposure time.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetSensorExposureTime(
                    ulong Handle,
                    ref long Data);

                /// <summary>
                /// Sets sensor sensitivity.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetSensorSensitivity(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.SysInt)] ref int Data);

                /// <summary>
                /// Sets exposure time upper limit.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetControlExposureUpperTimeLimit(
                    ulong Handle,
                    long Data);

                /// <summary>
                /// Sets jpeg GPS coordinates.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetJpegGPSCoordinates(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] Data);

                /// <summary>
                /// Sets jpeg timestamp.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetJpegGPSTimestamp(
                    ulong Handle,
                    long Data);

                /// <summary>
                /// Sets jpeg thumbnail size.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetJpegThumbnailSize(
                    ulong Handle,
                    ref JpegThumbnailSize Data);

                /// <summary>
                /// Sets jpeg quality.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataSetJpegQuality(
                    ulong Handle,
                    ref byte Data);

                /// <summary>
                /// Gets color correction.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetColorCorrectionModeResultMetadata(
                    ulong Handle,
                    out ColorCorrectionMode OutData);

                /// <summary>
                /// Gets color correction aberration.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetColorCorrectionAberrationModeResultMetadata(
                    ulong Handle,
                    out ColorCorrectionAberrationMode OutData);

                /// <summary>
                /// Gets AE exposure compensation.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAEExposureCompensationResultMetadata(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.SysInt)] out int OutData);

                /// <summary>
                /// Gets AE regions.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAERegionsResultMetadata(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.LPArray, SizeConst = 15)] IntPtr OutData,
                    [MarshalAs(UnmanagedType.SysInt)] out int OutCount);

                /// <summary>
                /// Gets AE target FPS range.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAETargetFPSRangeResultMetadata(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] int[] OutData);

                /// <summary>
                /// Gets AE state.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAEStateResultMetadata(
                    ulong Handle,
                    out ControlAEState OutData);

                /// <summary>
                /// Gets AWB state.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAWBStateResultMetadata(
                    ulong Handle,
                    out ControlAWBState OutData);

                /// <summary>
                /// Gets AWB mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlAWBModeResultMetadata(
                    ulong Handle,
                    out ControlAWBMode OutData);

                /// <summary>
                /// Gets mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlModeResultMetadata(
                    ulong Handle,
                    out ControlMode OutData);

                /// <summary>
                /// Gets Scene mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlSceneModeResultMetadata(
                    ulong Handle,
                    out ControlSceneMode OutData);

                /// <summary>
                /// Gets sensor exposure time.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetSensorExposureTimeResultMetadata(
                    ulong Handle,
                    out long OutData);

                /// <summary>
                /// Gets sensor sensitivity.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetSensorSensitivityResultMetadata(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.SysInt)] out int OutData);

                /// <summary>
                /// Gets frame captured timestamp.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetSensorTimestampResultMetadata(
                    ulong Handle,
                    out long OutData);

                /// <summary>
                /// Gets sensor frame duration.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetSensorFrameDurationResultMetadata(
                    ulong Handle,
                    out long OutData);

                /// <summary>
                /// Gets Effect mode.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlEffectModeResultMetadata(
                    ulong Handle,
                    out ControlEffectMode OutData);

                /// <summary>
                /// Gets exposure time upper limit.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlExposureUpperTimeLimitResultMetadata(
                    ulong Handle,
                    out long OutData);

                /// <summary>
                /// Gets jpeg GPS coordinates.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetJpegGPSCoordinatesResultMetadata(
                    ulong Handle,
                    [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] out double[] OutData);

                /// <summary>
                /// Gets jpeg timestamp.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetJpegGPSTimestampResultMetadata(
                    ulong Handle,
                    out long OutData);

                /// <summary>
                /// Gets jpeg thumbnail size.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetJpegThumbnailSizeResultMetadata(
                    ulong Handle,
                    out JpegThumbnailSize OutData);

                /// <summary>
                /// Gets force apply mode.  Main camera and CV Camera share the same camera hardware resources.  When both the cameras are
                /// streaming, request metadata properties for both cameras are merged and then applied.  While merging, the metadata
                /// properties from Main Camera take precedence over CV camera metadata properties.  The force apply mode property can be used to
                /// override this.  If CV Camera metadata has force apply mode on, the CV Camera metadata properties take precedence over
                /// Main Camera metadata properties.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetControlForceApplyModeResultMetadata(
                    ulong Handle,
                    out ControlForceApplyMode OutData);

                /// <summary>
                /// Gets jpeg quality.
                /// </summary>
                [DllImport(MLCameraMetadataDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLCameraMetadataGetJpegQualityResultMetadata(
                    ulong Handle,
                    out byte OutData);
            }
        }
    }
}
