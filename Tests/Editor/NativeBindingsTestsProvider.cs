using System.Runtime.InteropServices;

namespace UnitySDKEditorTests
{
    public class NativeBindingsTestsProvider
    {
        // Use SDK loader lib for features that will work in ML App Sim
        public const string MLSdkTestsProviderDll = "ml_sdk_tests_provider";

#if UNITY_EDITOR

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLVec3fSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLVersionSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLAudioMultibandLevelSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLAudioDispersionPropertiesSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLAudioReverbPropertiesSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLAudioSpatialSoundDistancePropertiesSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLAudioSpatialSoundRadiationPropertiesSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLAudioRaycastSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLAudioObstructionSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLAudioRaycastRequestSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLAudioRaycastResultSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLAudioBufferFormatSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLAudioBufferSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLAudioSpectralAnalysisSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCameraMetadataRationalSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCameraMRConnectInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCameraConnectContextSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCameraCaptureStreamCapsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCameraCaptureStreamConfigSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCameraCaptureConfigSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCameraPlaneInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCameraOutputSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCameraIntrinsicCalibrationParametersSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCameraResultExtrasSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCameraDeviceAvailabilityInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCameraDeviceAvailabilityStatusCallbacksSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCameraDeviceStatusCallbacksSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCameraCaptureCallbacksSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLControllerConfigurationSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLControllerStateExSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLControllerSystemStateExSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCoordinateFrameUIDSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLDepthCameraSettingsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLDepthCameraIntrinsicsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLDepthCameraFrameBufferSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLDepthCameraDataSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLEyeCalibrationStateSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLEyeTrackingStaticDataSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLEyeTrackingStateExSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLGazeRecognitionStaticDataSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLGazeRecognitionStateSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLGestureClassificationStaticDataSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLGestureClassificationPostureFingerDataSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLGestureClassificationFingerStateSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLGestureClassificationStateSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLGestureClassificationDataSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLGraphicsOptionsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLGraphicsFrameParamsExSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLGraphicsClipExtentsInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLGraphicsClipExtentsParamsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLGraphicsClipExtentsInfoArrayExSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLGraphicsRenderTargetSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLGraphicsRenderBufferInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLGraphicsRenderTargetsInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLGraphicsVirtualCameraInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLGraphicsFrameInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLGraphicsClientPerformanceInfoSize();
    
        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLHandTrackingCFUIDsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLHandTrackingStaticDataSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLHandTrackingHandStateSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLHandTrackingDataSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLHeadsetFitStateSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLHeadTrackingStaticDataSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLHeadTrackingStateSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLInputControllerTouchpadGestureSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLInputControllerCallbacksExSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLInputControllerStateSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLInputControllerStateExSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLInputConnectedDevicesListSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLInputPreDefinedPatternSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLInputBuzzCommandSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLInputCustomHapticsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLInputCustomHapticsInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMarkerTrackerDecodedTypedDataSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMarkerTrackerDecodedArucoDataSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMarkerTrackerDecodedBinaryDataSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMarkerTrackerDecodedDataSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMarkerTrackerResultSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMarkerTrackerResultArraySize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMarkerTrackerCustomProfileSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMarkerTrackerSettingsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCea608CaptionStyleColorSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCea608CaptionPACSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCea608CaptionLineExSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCea608CaptionSegmentExSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCea708CaptionColorSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCea708CaptionPenAttrSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCea708CaptionPenColorSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCea708CaptionPenLocationSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCea708CaptionWindowAttrSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCea708CaptionWindowSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLCea708CaptionEventSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaCodecBufferInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaCodecInputBufferInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaCodecOutputBufferInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaCodecOutputFormatInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaCodecErrorInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaCodecCallbacksExSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaCodecFrameRenderedInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaCodecOutputFrameListenerSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaCodecCryptoPatternSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaCodecCryptoInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaCodecListQueryResultsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaCodecListProfileLevelSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaDRMByteArraySize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaDRMByteArrayListSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaDRMKeyValueSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaDRMKeyValueArraySize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaDRMKeyRequestInputParamSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaDRMRequestMessageSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaDRMCryptoInputParamSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaDRMHMACInputParamSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaDRMRSAInputParamSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaDRMEventInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaDRMExpirationUpdateInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaDRMKeyStatusSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaDRMKeyStatusInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaDRMEventCallbacksSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaExtractorDRMSchemeInitDataSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaFormatByteArraySize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaMuxerSampleDataSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaPlayerTrackDRMInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaPlayerSubtitleDataSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaPlayerBufferingSettingsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaPlayerOnBufferingUpdateInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaPlayerOnCompletionInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaPlayerOnErrorInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaPlayerOnInfoInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaPlayerOnPreparedInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaPlayerOnSeekCompleteInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaPlayerOnVideoSizeChangedInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaPlayerOnTrackDRMInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaPlayerOnResetCompleteInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaPlayerOnFramePackingInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaPlayerEventCallbacksExSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaPlayerTrackInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaPlayerTrackDRMSessionInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaPlayerMetricsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaRecorderOnInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaRecorderOnErrorSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMediaRecorderEventCallbacksSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLTTMLImageSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLTTMLDataSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLPSSHEntrySize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLWebVTTDataSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMeshingSettingsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMeshingExtentsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMeshingBlockInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMeshingMeshInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMeshingBlockRequestSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMeshingMeshRequestSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMeshingBlockMeshSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMeshingMeshSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLNativeSurfaceFrameAvailableInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLNativeSurfaceConfigSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLNativeSurfaceOnFrameAvailableCallbackSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLPerceptionSettingsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLPlaneSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLPolygonSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLPlaneBoundarySize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLPlaneBoundariesSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLPlaneBoundariesListSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLPlanesQuerySize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLTransformDerivativesSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLSpatialAnchorLocalizationInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLSpatialAnchorSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLSpatialAnchorCreateInfoSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLSpatialAnchorQueryFilterSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLVec2fSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLXYZfSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLQuaternionfSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLTransformSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLMat4fSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLRectfSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLRectiSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLColor32Size();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLColor4fSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLUUIDSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLVoiceIntentEventSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLVoiceIntentCallbacksSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLVoiceIntentSettingsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLWebViewCursorStateSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLWebViewEventCallbacksSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLWebViewSettingsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLWebViewProcessSettingsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLWorldCameraSettingsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLWorldCameraIntrinsicsSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLWorldCameraFrameBufferSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLWorldCameraFrameSize();

        [DllImport(MLSdkTestsProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong GetMLWorldCameraDataSize();
    
#endif
    }
}