// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using Unity.Profiling;

namespace UnityEngine.XR.MagicLeap
{
    public sealed partial class MLCamera
    {
        //Profiler markers for MLCamera Native Bindings calls

        private readonly static ProfilerMarker nativeMLCameraConnectPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraConnect");
        private readonly static ProfilerMarker nativeMLCameraDisconnectPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraDisconnect");
        private readonly static ProfilerMarker nativeMLCameraPrepareCapturePerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraPrepareCapture");
        private readonly static ProfilerMarker nativeMLCameraSetDeviceStatusCallbacksPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraSetDeviceStatusCallbacks");
        private readonly static ProfilerMarker nativeMLCameraCaptureImagePerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraCaptureImage");
        private readonly static ProfilerMarker nativeMLCameraCaptureImageRawPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraCaptureImageRaw");
        private readonly static ProfilerMarker nativeMLCameraCaptureVideoStartPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraCaptureVideoStart");
        private readonly static ProfilerMarker nativeMLCameraCaptureRawVideoStartPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraCaptureRawVideoStart");
        private readonly static ProfilerMarker nativeMLCameraCaptureVideoStopPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraCaptureVideoStop");
        private readonly static ProfilerMarker nativeMLCameraGetErrorCodePerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraGetErrorCode");
        private readonly static ProfilerMarker nativeMLCameraGetCameraCharacteristicsPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraGetCameraCharacteristics");
        private readonly static ProfilerMarker nativeMLCameraSetOutputFormatPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraSetOutputFormat");
        private readonly static ProfilerMarker nativeGetPluginRenderCallbackPerfMarker = new ProfilerMarker("MLCameraNativeBindings.GetPluginRenderCallback");
        private readonly static ProfilerMarker nativeGetPluginRenderCleanupCallbackPerfMarker = new ProfilerMarker("MLCameraNativeBindings.GetPluginRenderCleanupCallback");
        private readonly static ProfilerMarker nativeSetTextureFromUnityPerfMarker = new ProfilerMarker("MLCameraNativeBindings.SetTextureFromUnity");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetColorCorrectionAvailableAberrationModesPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetColorCorrectionAvailableAberrationModes");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAEAvailableModesPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAEAvailableModes");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAECompensationRangePerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAECompensationRange");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAECompensationStepPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAECompensationStep");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAELockAvailablePerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAELockAvailable");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAWBAvailableModesPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAWBAvailableModes");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAWBLockAvailablePerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAWBLockAvailable");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetScalerProcessedSizesPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetScalerProcessedSizes");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetScalerAvailableMaxDigitalZoomPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetScalerAvailableMaxDigitalZoom");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetScalerAvailableStreamConfigurationsPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetScalerAvailableStreamConfigurations");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetSensorInfoActiveArraySizePerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetSensorInfoActiveArraySize");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetSensorInfoSensitivityRangePerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetSensorInfoSensitivityRange");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetSensorInfoExposureTimeRangePerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetSensorInfoExposureTimeRange");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetSensorOrientationPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetSensorOrientation");
        private readonly static ProfilerMarker nativeMLCameraMetadataSetSensorInfoExposureTimeRangePerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataSetSensorInfoExposureTimeRange");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetColorCorrectionModeRequestMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetColorCorrectionModeRequestMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetColorCorrectionTransformRequestMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetColorCorrectionTransformRequestMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetColorCorrectionGainsRequestMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetColorCorrectionGainsRequestMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetColorCorrectionAberrationModeRequestMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetColorCorrectionAberrationModeRequestMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAEAntibandingModeRequestMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAEAntibandingModeRequestMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAEExposureCompensationRequestMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAEExposureCompensationRequestMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAELockRequestMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAELockRequestMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAEModeRequestMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAEModeRequestMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAETargetFPSRangeRequestMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAETargetFPSRangeRequestMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAWBLockRequestMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAWBLockRequestMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAWBModeRequestMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAWBModeRequestMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetSensorExposureTimeRequestMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetSensorExposureTimeRequestMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetSensorSensitivityRequestMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetSensorSensitivityRequestMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetScalerCropRegionRequestMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetScalerCropRegionRequestMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataSetColorCorrectionModePerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataSetColorCorrectionMode");
        private readonly static ProfilerMarker nativeMLCameraMetadataSetColorCorrectionTransformPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataSetColorCorrectionTransform");
        private readonly static ProfilerMarker nativeMLCameraMetadataSetColorCorrectionGainsPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataSetColorCorrectionGains");
        private readonly static ProfilerMarker nativeMLCameraMetadataSetColorCorrectionAberrationModePerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataSetColorCorrectionAberrationMode");
        private readonly static ProfilerMarker nativeMLCameraMetadataSetControlAEAntibandingModePerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataSetControlAEAntibandingMode");
        private readonly static ProfilerMarker nativeMLCameraMetadataSetControlAEExposureCompensationPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataSetControlAEExposureCompensation");
        private readonly static ProfilerMarker nativeMLCameraMetadataSetControlAELockPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataSetControlAELock");
        private readonly static ProfilerMarker nativeMLCameraMetadataSetControlAEModePerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataSetControlAEMode");
        private readonly static ProfilerMarker nativeMLCameraMetadataSetControlAETargetFPSRangePerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataSetControlAETargetFPSRange");
        private readonly static ProfilerMarker nativeMLCameraMetadataSetControlAWBLockPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataSetControlAWBLock");
        private readonly static ProfilerMarker nativeMLCameraMetadataSetControlAWBModePerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataSetControlAWBMode");
        private readonly static ProfilerMarker nativeMLCameraMetadataSetSensorExposureTimePerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataSetSensorExposureTime");
        private readonly static ProfilerMarker nativeMLCameraMetadataSetSensorSensitivityPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataSetSensorSensitivity");
        private readonly static ProfilerMarker nativeMLCameraMetadataSetScalerCropRegionPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataSetScalerCropRegion");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetColorCorrectionModeResultMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetColorCorrectionModeResultMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetColorCorrectionTransformResultMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetColorCorrectionTransformResultMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetColorCorrectionAberrationModeResultMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetColorCorrectionAberrationModeResultMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetColorCorrectionGainsResultMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetColorCorrectionGainsResultMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAEAntibandingModeResultMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAEAntibandingModeResultMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAEExposureCompensationResultMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAEExposureCompensationResultMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAELockResultMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAELockResultMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAEModeResultMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAEModeResultMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAETargetFPSRangeResultMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAETargetFPSRangeResultMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAEStateResultMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAEStateResultMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAWBLockResultMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAWBLockResultMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetControlAWBStateResultMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetControlAWBStateResultMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetSensorExposureTimeResultMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetSensorExposureTimeResultMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetSensorSensitivityResultMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetSensorSensitivityResultMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetSensorTimestampResultMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetSensorTimestampResultMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetScalerCropRegionResultMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetScalerCropRegionResultMetadata");
        private readonly static ProfilerMarker nativeMLCameraMetadataGetSensorFrameDurationResultMetadataPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraMetadataGetSensorFrameDurationResultMetadata");
        private readonly static ProfilerMarker nativeMLCameraSetCaptureCallbacksExPerfMarker = new ProfilerMarker("MLCameraNativeBindings.MLCameraSetCaptureCallbacksEx");

        //Profiler markers for MLCamera calls.

        internal readonly static ProfilerMarker connectPerfMarker = new ProfilerMarker("MLCamera.Connect");
        internal readonly static ProfilerMarker disconnectPerfMarker = new ProfilerMarker("MLCamera.Disconnect");
        private readonly static ProfilerMarker captureImagePerfMarker = new ProfilerMarker("MLCamera.CaptureImage");
        private readonly static ProfilerMarker captureRawImageAsyncPerfMarker = new ProfilerMarker("MLCamera.CaptureRawImageAsync");
        private readonly static ProfilerMarker startVideoCapturecPerfMarker = new ProfilerMarker("MLCamera.StartVideoCapture");
        private readonly static ProfilerMarker startVideoCaptureAsyncPerfMarker = new ProfilerMarker("MLCamera.StartVideoCaptureAsync");
        private readonly static ProfilerMarker startRawVideoCapturePerfMarker = new ProfilerMarker("MLCamera.StartRawVideoCapture");
        private readonly static ProfilerMarker startRawVideoCaptureAsyncPerfMarker = new ProfilerMarker("MLCamera.StartRawVideoCaptureAsync");
        private readonly static ProfilerMarker stopVideoCapturePerfMarker = new ProfilerMarker("MLCamera.StopVideoCapture");
        private readonly static ProfilerMarker stopVideoCaptureAsyncPerfMarker = new ProfilerMarker("MLCamera.StopVideoCaptureAsync");
        private readonly static ProfilerMarker prepareCapturePerfMarker = new ProfilerMarker("MLCamera.PrepareCapture");
        private readonly static ProfilerMarker submitCapturePerfMarker = new ProfilerMarker("MLCamera.SubmitCapture");
        private readonly static ProfilerMarker startPreviewPerfMarker = new ProfilerMarker("MLCamera.StartPreview");
        private readonly static ProfilerMarker applicationPausePerfMarker = new ProfilerMarker("MLCamera.ApplicationPause");
        private readonly static ProfilerMarker stopPreviewPerfMarker = new ProfilerMarker("MLCamera.StopPreview");
        private readonly static ProfilerMarker startAPIPerfMarker = new ProfilerMarker("MLCamera.StartAPI");
        private readonly static ProfilerMarker stopAPIPerfMarker = new ProfilerMarker("MLCamera.StopAPI");
        private readonly static ProfilerMarker updatePerfMarker = new ProfilerMarker("MLCamera.Update");
        private readonly static ProfilerMarker queueRawVideoFrameCallbackPerfMarker = new ProfilerMarker("MLCamera.QueueRawVideoFrameCallback");
        private readonly static ProfilerMarker createPreviewTexturePerfMarker = new ProfilerMarker("MLCamera.CreatePreviewTexture");
        private readonly static ProfilerMarker onCaptureCompletedCallbackPerfMarker = new ProfilerMarker("MLCamera.OnCaptureCompletedCallback");
        private readonly static ProfilerMarker onImageBufferAvailableCallbackPerfMarker = new ProfilerMarker("MLCamera.OnImageBufferAvailableCallback");
        private readonly static ProfilerMarker onVideoBufferAvailableCallbackPerfMarker = new ProfilerMarker("MLCamera.OnVideoBufferAvailableCallback");
        private readonly static ProfilerMarker glPluginEventPerfMarker = new ProfilerMarker("MLCamera.GLPluginEvent");

        //Profiler markers for MLCameraSettings calls.

        private readonly static ProfilerMarker applyPerfMarker = new ProfilerMarker("MLCameraSettings.Apply");


    }
}
