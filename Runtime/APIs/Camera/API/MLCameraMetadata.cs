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
    using UnityEngine.XR.MagicLeap.Native;

    public partial class MLCameraBase
    {
        /// <summary>
        /// MLCameraMetadata Summary placeholder.
        /// </summary>
        public partial class Metadata
        {
            /// <summary>
            /// Rational data type.
            /// </summary>
            public struct Rational
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
                /// Provides a string representation of the numerator and denominator.
                /// </summary>
                /// <returns>A string representation of the numerator and denominator.</returns>
                public override string ToString()
                {
                    return this.Numerator + "/" + this.Denominator;
                }

                /// <summary>
                /// Returns the rational value as a decimal.
                /// </summary>
                /// <returns>The rational value.</returns>
                public float ToFloat()
                {
                    if (this.Denominator == 0)
                    {
                        Debug.LogError("MLCameraMetadataRationalNative has a Denominator of 0. Cannot divide by zero!");
                        return 0.0f;
                    }

                    return (float)this.Numerator / (float)this.Denominator;
                }

                /// <summary>
                /// Sets the numerator and denominator based on the decimal rational and with the provided denominator.
                /// </summary>
                /// <param name="value">The decimal rational value.</param>
                /// <param name="denominator">The denominator used in the rational.</param>
                public void FromFloat(float value, int denominator)
                {
                    this.Numerator = (int)(value * denominator);
                    this.Denominator = denominator;
                }
            };

            public ulong Handle { get; private set; }

            internal Metadata(ulong handle)
            {
                Handle = handle;
            }

            /// <summary>
            /// Gets sensor sensitivity.
            /// </summary>
            public MLResult GetSensorSensitivityRequestMetadata(out int OutData)
                => MLResult.Create(InternalGetSensorSensitivityRequestMetadata(out OutData));

            /// <summary>
            /// Sets AE lock.
            /// </summary>
            public MLResult SetControlAELock(ControlAELock Data)
                => MLResult.Create(InternalSetControlAELock(Data));

            /// <summary>
            /// Gets AE Max Regions.
            /// </summary>
            public MLResult GetControlAEMaxRegions(out int OutData)
                => MLResult.Create(InternalGetControlAEMaxRegions(out OutData));

            /// <summary>
            /// Gets AE mode.
            /// </summary>
            public MLResult GetControlAEModeResultMetadata(out ControlAEMode OutData)
                => MLResult.Create(InternalGetControlAEModeResultMetadata(out OutData));

            /// <summary>
            /// Gets AE antibanding mode.
            /// </summary>
            public MLResult GetControlAEAntibandingModeResultMetadata(out ControlAEAntibandingMode OutData)
                => MLResult.Create(InternalGetControlAEAntibandingModeResultMetadata(out OutData));

            /// <summary>
            /// Gets AE compensation step.
            /// </summary>

            public MLResult GetControlAECompensationStep(out Rational OutData)
                => MLResult.Create(InternalGetControlAECompensationStep(out OutData));

            /// <summary>
            /// Gets color correction transform.
            /// </summary>
            public MLResult GetColorCorrectionTransformRequestMetadata(out Rational[][] OutData)
                => MLResult.Create(InternalGetColorCorrectionTransformRequestMetadata(out OutData));

            /// <summary>
            /// Gets sensor info sensitivity range.
            /// </summary>
            public MLResult GetSensorInfoSensitivityRange(out int[] OutData)
                => MLResult.Create(InternalGetSensorInfoSensitivityRange(out OutData));

            /// <summary>
            /// Sets Effect mode.
            /// </summary>
            public MLResult SetControlEffectMode(ControlEffectMode Data)
                => MLResult.Create(InternalSetControlEffectMode(Data));

            /// <summary>
            /// Sets color correction aberration.
            /// </summary>
            public MLResult SetColorCorrectionAberrationMode(ColorCorrectionAberrationMode Data)
                => MLResult.Create(InternalSetColorCorrectionAberrationMode(Data));

            /// <summary>
            /// Gets color correction aberration modes.
            /// </summary>
            public MLResult GetColorCorrectionAvailableAberrationModes(
                out ColorCorrectionAberrationMode[] OutData)
                => MLResult.Create(InternalGetColorCorrectionAvailableAberrationModes(out OutData));

            /// <summary>
            /// Gets color correction aberration.
            /// </summary>
            public MLResult GetColorCorrectionAberrationModeRequestMetadata(
                out ColorCorrectionAberrationMode OutData)
                => MLResult.Create(InternalGetColorCorrectionAberrationModeRequestMetadata(out OutData));

            /// <summary>
            /// Gets AE lock.
            /// </summary>
            public MLResult GetControlAELockAvailable(out ControlAELock OutData)
                => MLResult.Create(InternalGetControlAELockAvailable(out OutData));

            /// <summary>
            /// Gets jpeg GPS coordinates.
            /// </summary>
            public MLResult GetJpegGPSCoordinatesRequestMetadata(out double[] OutData)
                => MLResult.Create(InternalGetJpegGPSCoordinatesRequestMetadata(out OutData));

            /// <summary>
            /// Gets Effect modes.
            /// </summary>
            public MLResult GetControlAvailableEffectModes(out ControlEffectMode[] OutData)
                => MLResult.Create(InternalGetControlAvailableEffectModes(out OutData));

            /// <summary>
            /// Gets Scene modes.
            /// </summary>
            public MLResult GetControlAvailableSceneModes(out ControlSceneMode[] OutData)
                => MLResult.Create(InternalGetControlAvailableSceneModes(out OutData));

            /// <summary>
            /// Gets AE modes.
            /// </summary>
            public MLResult GetControlAEAvailableModes(out ControlAEMode[] OutData)
                => MLResult.Create(InternalGetControlAEAvailableModes(out OutData));

            /// <summary>
            /// Gets AE compensation range.
            /// </summary>
            public MLResult GetControlAECompensationRange(out int[] OutData)
                => MLResult.Create(InternalGetControlAECompensationRange(out OutData));

            /// <summary>
            /// Gets AWB modes.
            /// </summary>
            public MLResult GetControlAWBAvailableModes(out ControlAWBMode[] OutData)
                => MLResult.Create(InternalGetControlAWBAvailableModes(out OutData));

            /// <summary>
            /// Gets AF modes
            /// </summary>
            /// <param name="OutData">API mode</param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to obtain result due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully obtained result<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to obtain result due to an unknown error.<br/></returns>
            public MLResult GetControlAFAvailableModes(out ControlAFMode[] OutData)
                => MLResult.Create(InternalGetControlAFAvailableModes(out OutData));

            /// <summary>
            /// Gets lens hyperfocal distance in diopters
            /// </summary>
            /// <param name="OutData">Lens hyperfocal distance in diopters</param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to obtain result due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully obtained result<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to obtain result due to an unknown error.<br/></returns>
            public MLResult GetLensInfoHyperfocalDistance(out float OutData)
                => MLResult.Create(InternalGetLensInfoHyperfocalDistance(out OutData));

            /// <summary>
            /// Gets lens minimum focus distance in diopters
            /// </summary>
            /// <param name="OutData">Lens minimum focus distance in diopters</param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to obtain result due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully obtained result<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to obtain result due to an unknown error.<br/></returns>
            public MLResult GetLensInfoMinimumFocusDistance(out float OutData)
                => MLResult.Create(InternalGetLensInfoMinimumFocusDistance(out OutData));

            /// <summary>
            /// Gets lens minimum focus distance increment.
            /// The lens position can be set in multiples of distance increments between hyperfocal distance and minimum focus distance.
            /// </summary>
            /// <param name="OutData">Minimum focus distance increment in diopters</param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to obtain result due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully obtained result<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to obtain result due to an unknown error.<br/></returns>
            public MLResult GetLensInfoMinimumFocusDistanceIncrement(out float OutData)
                => MLResult.Create(InternalGetLensInfoMinimumFocusDistanceIncrement(out OutData));

            /// <summary>
            /// Gets AF mode
            /// </summary>
            /// <param name="OutData">AF Mode</param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to obtain result due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully obtained result<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to obtain result due to an unknown error.<br/></returns>
            public MLResult GetControlAFModeRequestMetadata(out ControlAFMode OutData)
                => MLResult.Create(InternalGetControlAFModeRequestMetadata(out OutData));

            /// <summary>
            /// Gets AF trigger
            /// </summary>
            /// <param name="OutData">AF trigger</param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to obtain result due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully obtained result<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to obtain result due to an unknown error.<br/></returns>
            public MLResult GetControlAFTriggerRequestMetadata(out ControlAFTrigger OutData)
                => MLResult.Create(InternalGetControlAFTriggerRequestMetadata(out OutData));

            /// <summary>
            /// Gets AF distance range. Both Min and Max are specified as focus distance in diopters.
            /// The min value specifies the lower bound for focus distance used by Autofocus algorithm and should be
            /// more than min focus distance which can be queried using <see cref="GetLensInfoMinimumFocusDistance"/>.
            /// The max value specifies the upper bound for focus distance used by Autofocus algorithm and should be
            /// less than hyperfocal distance which can be queried using API <see cref="GetLensInfoHyperfocalDistance"/>.
            /// </summary>
            /// <param name="Min">Min AF distance</param>
            /// <param name="Max">Max AF distance</param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to obtain result due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully obtained result<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to obtain result due to an unknown error.<br/></returns>
            public MLResult GetControlAFDistanceRangeRequestMetadata(out float Min, out float Max)
                => MLResult.Create(InternalGetControlAFDistanceRangeRequestMetadata(out Min, out Max));

            /// <summary>
            /// Gets Lens focus distance in diopters.
            /// The distance is between minimum focus distance and hyperfocal distance.
            /// </summary>
            /// <param name="OutData"></param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to obtain result due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully obtained result<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to obtain result due to an unknown error.<br/></returns>
            public MLResult GetLensFocusDistanceRequestMetadata(out float OutData)
                => MLResult.Create(InternalGetLensFocusDistanceRequestMetadata(out OutData));

            /// <summary>
            /// Sets AF mode
            /// </summary>
            /// <param name="Mode">AF mode</param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to set value due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully set value<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to set value due to an unknown error.<br/></returns>
            public MLResult SetControlAFMode(ControlAFMode Mode)
                => MLResult.Create(InternalSetControlAFMode(Mode));

            /// <summary>
            /// Sets AF trigger
            /// </summary>
            /// <param name="Trigger">AF trigger</param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to set value due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully obtained result<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to set value due to an unknown error.<br/></returns>
            public MLResult SetControlAFTrigger(ControlAFTrigger Trigger)
                => MLResult.Create(InternalSetControlAFTrigger(Trigger));

            /// <summary>
            /// Sets AF distance range. Both min and max are specified as focus distance in diopters.
            /// The min value specifies the lower bound for focus distance used by Autofocus algorithm and should be
            /// more than min focus distance which can be queried using <see cref="GetLensInfoMinimumFocusDistance"/>.
            /// The max value specifies the upper bound for focus distance used by Autofocus algorithm and should be
            /// less than hyperfocal distance which can be queried using api <see cref="GetLensInfoHyperfocalDistance"/>.
            /// </summary>
            /// <param name="Min">Min AF distance</param>
            /// <param name="Max">Max AF distance</param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to set value due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully obtained result<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to set value due to an unknown error.<br/></returns>
            public MLResult SetControlAFDistanceRange(float Min, float Max)
                => MLResult.Create(InternalSetAFDistanceRange(Min, Max));

            /// <summary>
            /// Sets Lens focus distance
            /// </summary>
            /// <param name="OutData">API result</param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to set value due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully obtained result<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to set value due to an unknown error.<br/></returns>
            public MLResult SetLensFocusDistance(float Distance)
                => MLResult.Create(InternalSetLensFocusDistance(Distance));

            /// <summary>
            /// Gets AF mode
            /// </summary>
            /// <param name="Data">AF mode</param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to obtain result due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully obtained result<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to obtain result due to an unknown error.<br/></returns>
            public MLResult GetControlAFModeResultMetadata(out ControlAFMode Data)
                => MLResult.Create(InternalGetControlAFModeResultMetadata(out Data));

            /// <summary>
            /// Gets AF trigger
            /// </summary>
            /// <param name="OutData">AF trigger</param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to obtain result due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully obtained result<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to obtain result due to an unknown error.<br/></returns>
            public MLResult GetControlAFTriggerResultMetadata(out ControlAFTrigger Data)
                => MLResult.Create(InternalGetControlAFTriggerResultMetadata(out Data));

            /// <summary>
            /// Gets AF distance range. Both Min and Max are specified as focus distance in diopters.
            /// The min value specifies the lower bound for focus distance used by Autofocus algorithm and should be
            /// more than min focus distance which can be queried using <see cref="GetLensInfoMinimumFocusDistance"/>.
            /// The max value specifies the upper bound for focus distance used by Autofocus algorithm and should be
            /// less than hyperfocal distance which can be queried using api <see cref="GetLensInfoHyperfocalDistance"/>.
            /// </summary>
            /// <param name="Min">Min AF distance</param>
            /// <param name="Max">Max AF distance</param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to obtain result due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully obtained result<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to obtain result due to an unknown error.<br/></returns>
            public MLResult GetControlAFDistanceRangeResultMetadata(out float Min, out float Max)
                => MLResult.Create(InternalGetControlAFDistanceRangeResultMetadata(out Min, out Max));

            /// <summary>
            /// Gets AF state
            /// </summary>
            /// <param name="Data">AF state</param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to obtain result due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully obtained result<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to obtain result due to an unknown error.<br/></returns>
            public MLResult GetControlAFStateResultMetadata(out ControlAFState Data)
                => MLResult.Create(InternalGetControlAFStateResultMetadata(out Data));

            /// <summary>
            /// Gets AF scene change detection status
            /// </summary>
            /// <param name="OutData">AF scene change detected or not</param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to obtain result due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully obtained result<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to obtain result due to an unknown error.<br/></returns>
            public MLResult GetControlAFSceneChangeResultMetadata(out ControlAFSceneChange Data)
                => MLResult.Create(InternalGetControlAFSceneChangeResultMetadata(out Data));

            /// <summary>
            /// Gets Lens focus distance
            /// </summary>
            /// <param name="Data">Lens focus distance</param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to obtain result due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully obtained result<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to obtain result due to an unknown error.<br/></returns>
            public MLResult GetLensFocusDistanceResultMetadata(out float Data)
                => MLResult.Create(InternalGetLensFocusDistanceResultMetadata(out Data));

            /// <summary>
            /// Get Lens state
            /// </summary>
            /// <param name="Data">Lens state</param>
            /// <returns><see cref="MLResult.Code.InvalidParam"/>: Failed to obtain result due to an invalid input parameter.<br/>
            /// <see cref="MLResult.Code.Ok"/>: Successfully obtained result<br/>
            /// <see cref="MLResult.Code.UnspecifiedFailure"/>: Failed to obtain result due to an unknown error.<br/></returns>
            public MLResult GetLensStateResultMetadata(out LensState Data)
                => MLResult.Create(InternalGetLensStateResultMetadata(out Data));

            /// <summary>
            /// Gets AE regions.
            /// </summary>
            public MLResult GetControlAERegionsRequestMetadata(out int[][] OutData)
                => MLResult.Create(InternalGetControlAERegionsRequestMetadata(out OutData));

            /// <summary>
            /// Gets Available Modes.
            /// </summary>
            public MLResult GetControlAvailableModes(out ControlMode[] OutData)
                => MLResult.Create(InternalGetControlAvailableModes(out OutData));

            /// <summary>
            /// Sets color correction transform.
            /// </summary>
            public MLResult SetColorCorrectionTransform(Rational[][] Data)
                => MLResult.Create(InternalSetColorCorrectionTransform(Data));

            /// <summary>
            /// Gets AWB lock.
            /// </summary>
            public MLResult GetControlAWBLockAvailable(out ControlAWBLock OutData)
                => MLResult.Create(InternalGetControlAWBLockAvailable(out OutData));

            /// <summary>
            /// Gets sensor info active array sizes.
            /// </summary>
            public MLResult GetSensorInfoActiveArraySize(out int[] OutData)
                => MLResult.Create(InternalGetSensorInfoActiveArraySize(out OutData));

            /// <summary>
            /// Gets color correction mode.
            /// </summary>
            public MLResult GetColorCorrectionModeRequestMetadata(out ColorCorrectionMode OutData)
                => MLResult.Create(InternalGetColorCorrectionModeRequestMetadata(out OutData));

            /// <summary>
            /// Gets sensor orientation degree.
            /// </summary>
            public MLResult GetSensorOrientation(out int OutData)
                => MLResult.Create(InternalGetSensorOrientation(out OutData));

            /// <summary>
            /// Gets color correction gains.
            /// </summary>
            public MLResult GetColorCorrectionGainsRequestMetadata(out float[] OutData)
                => MLResult.Create(InternalGetColorCorrectionGainsRequestMetadata(out OutData));

            /// <summary>
            /// Gets color correction transform.
            /// </summary>
            public MLResult GetColorCorrectionTransformResultMetadata(out Rational[][] OutData)
                => MLResult.Create(InternalGetColorCorrectionTransformResultMetadata(out OutData));

            /// <summary>
            /// Gets AE antibanding mode.
            /// </summary>
            public MLResult GetControlAEAntibandingModeRequestMetadata(out ControlAEAntibandingMode OutData)
                => MLResult.Create(InternalGetControlAEAntibandingModeRequestMetadata(out OutData));

            /// <summary>
            /// Gets AE exposure compensation.
            /// </summary>
            public MLResult GetControlAEExposureCompensationRequestMetadata(out int OutData)
                => MLResult.Create(InternalGetControlAEExposureCompensationRequestMetadata(out OutData));

            /// <summary>
            /// Gets AE lock.
            /// </summary>
            public MLResult GetControlAELockRequestMetadata(out ControlAELock OutData)
                => MLResult.Create(InternalGetControlAELockRequestMetadata(out OutData));

            /// <summary>
            /// Gets AE mode.
            /// </summary>
            public MLResult GetControlAEModeRequestMetadata(out ControlAEMode OutData)
                => MLResult.Create(InternalGetControlAEModeRequestMetadata(out OutData));

            /// <summary>
            /// Gets AWB lock.
            /// </summary>
            public MLResult GetControlAWBLockRequestMetadata(out ControlAWBLock OutData)
                => MLResult.Create(InternalGetControlAWBLockRequestMetadata(out OutData));

            /// <summary>
            /// Gets AWB mode.
            /// </summary>
            public MLResult GetControlAWBModeRequestMetadata(out ControlAWBMode OutData)
                => MLResult.Create(InternalGetControlAWBModeRequestMetadata(out OutData));

            /// <summary>
            /// Gets mode.
            /// </summary>
            public MLResult GetControlModeRequestMetadata(out ControlMode OutData)
                => MLResult.Create(InternalGetControlModeRequestMetadata(out OutData));

            /// <summary>
            /// Gets Scene mode.
            /// </summary>
            public MLResult GetControlSceneModeRequestMetadata(out ControlSceneMode OutData)
                => MLResult.Create(InternalGetControlSceneModeRequestMetadata(out OutData));

            /// <summary>
            /// Gets sensor exposure time.
            /// </summary>
            public MLResult GetSensorExposureTimeRequestMetadata(out long OutData)
                => MLResult.Create(InternalGetSensorExposureTimeRequestMetadata(out OutData));

            /// <summary>
            /// Effect mode.
            /// </summary>
            public MLResult GetControlEffectModeRequestMetadata(out ControlEffectMode OutData)
                => MLResult.Create(InternalGetControlEffectModeRequestMetadata(out OutData));

            /// <summary>
            /// Gets AE lock.
            /// </summary>
            public MLResult GetControlAELockResultMetadata(out ControlAELock OutData)
                => MLResult.Create(InternalGetControlAELockResultMetadata(out OutData));

            /// <summary>
            /// Gets exposure time upper limit.
            /// </summary>
            public MLResult GetControlExposureUpperTimeLimitRequestMetadata(out long OutData)
                => MLResult.Create(InternalGetControlExposureUpperTimeLimitRequestMetadata(out OutData));

            /// <summary>
            /// Gets jpeg timestamp.
            /// </summary>
            public MLResult GetJpegGPSTimestampRequestMetadata(out long OutData)
                => MLResult.Create(InternalGetJpegGPSTimestampRequestMetadata(out OutData));

            /// <summary>
            /// Gets jpeg thumbnail size.
            /// </summary>
            public MLResult GetJpegThumbnailSizeRequestMetadata(out JpegThumbnailSize OutData)
                => MLResult.Create(InternalGetJpegThumbnailSizeRequestMetadata(out OutData));

            /// <summary>
            /// Gets jpeg quality.
            /// </summary>
            public MLResult GetJpegQualityRequestMetadata(out byte OutData)
                => MLResult.Create(InternalGetJpegQualityRequestMetadata(out OutData));

            /// <summary>
            /// Sets force apply mode.  Main camera and CV Camera share the same camera hardware resources.  When both the cameras are
            /// streaming, request metadata properties for both cameras are merged and then applied.  While merging, the metadata
            /// properties from Main Camera take precedence over CV camera metadata properties.  The force apply mode property can be used to
            /// override this.  If CV Camera metadata has force apply mode on, the CV Camera metadata properties take precedence over
            /// Main Camera metadata properties.
            /// </summary>
            public MLResult SetControlForceApplyMode(ControlForceApplyMode Data)
                => MLResult.Create(InternalSetControlForceApplyMode(Data));


            /// <summary>
            /// Gets force apply mode.  Main camera and CV Camera share the same camera hardware resources.  When both the cameras are
            /// streaming, request metadata properties for both cameras are merged and then applied.  While merging, the metadata
            /// properties from Main Camera take precedence over CV camera metadata properties.  The force apply mode property can be used to
            /// override this.  If CV Camera metadata has force apply mode on, the CV Camera metadata properties take precedence over
            /// Main Camera metadata properties.
            /// </summary>
            public MLResult GetControlForceApplyModeRequestMetadata(out ControlForceApplyMode OutData)
                => MLResult.Create(InternalGetControlForceApplyModeRequestMetadata(out OutData));

            /// <summary>
            /// Gets force apply mode.  Main camera and CV Camera share the same camera hardware resources.  When both the cameras are
            /// streaming, request metadata properties for both cameras are merged and then applied.  While merging, the metadata
            /// properties from Main Camera take precedence over CV camera metadata properties.  The force apply mode property can be used to
            /// override this.  If CV Camera metadata has force apply mode on, the CV Camera metadata properties take precedence over
            /// Main Camera metadata properties.
            /// </summary>
            public MLResult GetControlForceApplyModeResultMetadata(out ControlForceApplyMode OutData)
                => MLResult.Create(InternalGetControlForceApplyModeResultMetadata(out OutData));


            /// <summary>
            /// Sets color correction mode.
            /// </summary>
            public MLResult SetColorCorrectionMode(ColorCorrectionMode Data)
                => MLResult.Create(InternalSetColorCorrectionMode(Data));

            /// <summary>
            /// Gets color correction gains.
            /// </summary>
            public MLResult GetColorCorrectionGainsResultMetadata(out float[] OutData)
                => MLResult.Create(InternalGetColorCorrectionGainsResultMetadata(out OutData));

            /// <summary>
            /// Sets color correction gains.
            /// </summary>
            public MLResult SetColorCorrectionGains(float[] Data)
                => MLResult.Create(InternalSetColorCorrectionGains(Data));

            /// <summary>
            /// Sets AE antiband mode.
            /// </summary>
            public MLResult SetControlAEAntibandingMode(ControlAEAntibandingMode Data)
                => MLResult.Create(InternalSetControlAEAntibandingMode(Data));

            /// <summary>
            /// Sets AE exposure compensation.
            /// </summary>
            public MLResult SetControlAEExposureCompensation(int Data)
                => MLResult.Create(InternalSetControlAEExposureCompensation(Data));

            /// <summary>
            /// Sets AE mode.  ControlAEMode_Off is not supported if camera is configured for 15FPS
            /// </summary>
            public MLResult SetControlAEMode(ControlAEMode Data)
                => MLResult.Create(InternalSetControlAEMode(Data));

            /// <summary>
            /// set AE regions.
            /// </summary>
            public MLResult SetControlAERegions(int[][] Data)
                => MLResult.Create(InternalSetControlAERegions(Data));

            /// <summary>
            /// Sets AWB lock.
            /// </summary>
            public MLResult SetControlAWBLock(ControlAWBLock Data)
                => MLResult.Create(InternalSetControlAWBLock(Data));

            /// <summary>
            /// Sets AWB mode.
            /// </summary>
            public MLResult SetControlAWBMode(ControlAWBMode Data)
                => MLResult.Create(InternalSetControlAWBMode(Data));

            /// <summary>
            /// Sets mode.
            /// </summary>
            public MLResult SetControlMode(ControlMode Data)
                => MLResult.Create(InternalSetControlMode(Data));

            /// <summary>
            /// Sets Scene mode.
            /// </summary>
            public MLResult SetControlSceneMode(ControlSceneMode Data)
                => MLResult.Create(InternalSetControlSceneMode(Data));

            /// <summary>
            /// Gets AWB lock.
            /// </summary>
            public MLResult GetControlAWBLockResultMetadata(out ControlAWBLock OutData)
                => MLResult.Create(InternalGetControlAWBLockResultMetadata(out OutData));

            /// <summary>
            /// Sets sensor exposure time.
            /// </summary>
            public MLResult SetSensorExposureTime(long Data)
                => MLResult.Create(InternalSetSensorExposureTime(Data));

            /// <summary>
            /// Sets sensor sensitivity.
            /// </summary>
            public MLResult SetSensorSensitivity(int Data)
                => MLResult.Create(InternalSetSensorSensitivity(Data));

            /// <summary>
            /// Sets exposure time upper limit.
            /// </summary>
            public MLResult SetControlExposureUpperTimeLimit(long Data)
                => MLResult.Create(InternalSetControlExposureUpperTimeLimit(Data));

            /// <summary>
            /// Sets jpeg GPS coordinates.
            /// </summary>
            public MLResult SetJpegGPSCoordinates(double[] Data)
                => MLResult.Create(InternalSetJpegGPSCoordinates(Data));

            /// <summary>
            /// Sets jpeg timestamp.
            /// </summary>
            public MLResult SetJpegGPSTimestamp(long Data)
                => MLResult.Create(InternalSetJpegGPSTimestamp(Data));

            /// <summary>
            /// Sets jpeg thumbnail size.
            /// </summary>
            public MLResult SetJpegThumbnailSize(JpegThumbnailSize Data)
                => MLResult.Create(InternalSetJpegThumbnailSize(Data));

            /// <summary>
            /// Sets jpeg quality.
            /// </summary>
            public MLResult SetJpegQuality(byte Data)
                => MLResult.Create(InternalSetJpegQuality(Data));

            /// <summary>
            /// Gets color correction.
            /// </summary>
            public MLResult GetColorCorrectionModeResultMetadata(out ColorCorrectionMode OutData)
                => MLResult.Create(InternalGetColorCorrectionModeResultMetadata(out OutData));

            /// <summary>
            /// Gets color correction aberration.
            /// </summary>
            public MLResult GetColorCorrectionAberrationModeResultMetadata(
                out ColorCorrectionAberrationMode OutData)
                => MLResult.Create(InternalGetColorCorrectionAberrationModeResultMetadata(out OutData));

            /// <summary>
            /// Gets AE exposure compensation.
            /// </summary>
            public MLResult GetControlAEExposureCompensationResultMetadata(out int OutData)
                => MLResult.Create(InternalGetControlAEExposureCompensationResultMetadata(out OutData));

            /// <summary>
            /// Gets AE regions.
            /// </summary>
            public MLResult GetControlAERegionsResultMetadata(out int[][] OutData)
                => MLResult.Create(InternalGetControlAERegionsResultMetadata(out OutData));

            /// <summary>
            /// Gets AE target FPS range.
            /// </summary>
            public MLResult GetControlAETargetFPSRangeResultMetadata(out int[] OutData)
                => MLResult.Create(InternalGetControlAETargetFPSRangeResultMetadata(out OutData));

            /// <summary>
            /// Gets AE state.
            /// </summary>
            public MLResult GetControlAEStateResultMetadata(out ControlAEState OutData)
                => MLResult.Create(InternalGetControlAEStateResultMetadata(out OutData));

            /// <summary>
            /// Gets AWB state.
            /// </summary>
            public MLResult GetControlAWBStateResultMetadata(out ControlAWBState OutData)
                => MLResult.Create(InternalGetControlAWBStateResultMetadata(out OutData));

            /// <summary>
            /// Gets AWB mode.
            /// </summary>
            public MLResult GetControlAWBModeResultMetadata(out ControlAWBMode OutData)
                => MLResult.Create(InternalGetControlAWBModeResultMetadata(out OutData));

            /// <summary>
            /// Gets mode.
            /// </summary>
            public MLResult GetControlModeResultMetadata(out ControlMode OutData)
                => MLResult.Create(InternalGetControlModeResultMetadata(out OutData));

            /// <summary>
            /// Gets Scene mode.
            /// </summary>
            public MLResult GetControlSceneModeResultMetadata(out ControlSceneMode OutData)
                => MLResult.Create(InternalGetControlSceneModeResultMetadata(out OutData));

            /// <summary>
            /// Gets sensor exposure time.
            /// </summary>
            public MLResult GetSensorExposureTimeResultMetadata(out long OutData)
                => MLResult.Create(InternalGetSensorExposureTimeResultMetadata(out OutData));

            /// <summary>
            /// Gets sensor sensitivity.
            /// </summary>
            public MLResult GetSensorSensitivityResultMetadata(out int OutData)
                => MLResult.Create(InternalGetSensorSensitivityResultMetadata(out OutData));

            /// <summary>
            /// Gets frame captured timestamp.
            /// </summary>
            public MLResult GetSensorTimestampResultMetadata(out long OutData)
                => MLResult.Create(InternalGetSensorTimestampResultMetadata(out OutData));

            /// <summary>
            /// Gets sensor frame duration.
            /// </summary>
            public MLResult GetSensorFrameDurationResultMetadata(out long OutData)
                => MLResult.Create(InternalGetSensorFrameDurationResultMetadata(out OutData));

            /// <summary>
            /// Gets Effect mode.
            /// </summary>
            public MLResult GetControlEffectModeResultMetadata(out ControlEffectMode OutData)
                => MLResult.Create(InternalGetControlEffectModeResultMetadata(out OutData));

            /// <summary>
            /// Gets exposure time upper limit.
            /// </summary>
            public MLResult GetControlExposureUpperTimeLimitResultMetadata(out long OutData)
                => MLResult.Create(InternalGetControlExposureUpperTimeLimitResultMetadata(out OutData));

            /// <summary>
            /// Gets jpeg GPS coordinates.
            /// </summary>
            public MLResult GetJpegGPSCoordinatesResultMetadata(out double[] OutData)
                => MLResult.Create(InternalGetJpegGPSCoordinatesResultMetadata(out OutData));

            /// <summary>
            /// Gets jpeg timestamp.
            /// </summary>
            public MLResult GetJpegGPSTimestampResultMetadata(out long OutData)
                => MLResult.Create(InternalGetJpegGPSTimestampResultMetadata(out OutData));

            /// <summary>
            /// Gets jpeg thumbnail size.
            /// </summary>
            public MLResult GetJpegThumbnailSizeResultMetadata(out JpegThumbnailSize OutData)
                => MLResult.Create(InternalGetJpegThumbnailSizeResultMetadata(out OutData));

            /// <summary>
            /// Gets jpeg quality.
            /// </summary>
            public MLResult GetJpegQualityResultMetadata(out byte OutData)
                => MLResult.Create(InternalGetJpegQualityResultMetadata(out OutData));
        }
    }
}
