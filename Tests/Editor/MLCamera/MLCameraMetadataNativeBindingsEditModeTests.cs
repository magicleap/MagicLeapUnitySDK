// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
#pragma warning disable CS0618 // Type or member is obsolete
    public partial class MLCamera
    {
        public class Metadata : NativeBindingsTests
        {
            private MlSdkDllLoader lib;

            [OneTimeSetUp]
            public void Init()
            {
                lib = new MlSdkDllLoader();
                lib.Load("camera_metadata.magicleap");
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                lib.Free();
            }

            [SetUp]
            public void SetupNativeBindings()
            {
                var apiType = typeof(UnityEngine.XR.MagicLeap.MLCamera.Metadata);
                nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetSensorSensitivityRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetSensorSensitivityRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetControlAELock_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetControlAELock");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAEMaxRegions_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAEMaxRegions");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAEModeResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAEModeResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAEAntibandingModeResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAEAntibandingModeResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetControlForceApplyMode_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetControlForceApplyMode");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAECompensationStep_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAECompensationStep");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetColorCorrectionTransformRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetColorCorrectionTransformRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetSensorInfoSensitivityRange_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetSensorInfoSensitivityRange");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetControlEffectMode_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetControlEffectMode");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetColorCorrectionAberrationMode_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetColorCorrectionAberrationMode");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetColorCorrectionAvailableAberrationModes_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetColorCorrectionAvailableAberrationModes");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetColorCorrectionAberrationModeRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetColorCorrectionAberrationModeRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAELockAvailable_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAELockAvailable");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetJpegGPSCoordinatesRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetJpegGPSCoordinatesRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAvailableEffectModes_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAvailableEffectModes");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAvailableSceneModes_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAvailableSceneModes");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAEAvailableModes_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAEAvailableModes");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAECompensationRange_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAECompensationRange");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAWBAvailableModes_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAWBAvailableModes");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAFAvailableModes_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAFAvailableModes");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetLensInfoHyperfocalDistance_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetLensInfoHyperfocalDistance");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetLensInfoMinimumFocusDistance_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetLensInfoMinimumFocusDistance");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetLensInfoMinimumFocusDistanceIncrement_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetLensInfoMinimumFocusDistanceIncrement");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAFModeRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAFModeRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAFTriggerRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAFTriggerRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetAFDistanceRangeRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetAFDistanceRangeRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetLensFocusDistanceRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetLensFocusDistanceRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetControlAFMode_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetControlAFMode");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetControlAFTrigger_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetControlAFTrigger");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetAFDistanceRange_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetAFDistanceRange");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetLensFocusDistance_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetLensFocusDistance");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAFModeResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAFModeResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAFTriggerResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAFTriggerResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetAFDistanceRangeResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetAFDistanceRangeResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAFStateResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAFStateResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAFSceneChangeResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAFSceneChangeResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetLensFocusDistanceResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetLensFocusDistanceResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetLensStateResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetLensStateResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAERegionsRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAERegionsRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAvailableModes_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAvailableModes");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetColorCorrectionTransform_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetColorCorrectionTransform");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAWBLockAvailable_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAWBLockAvailable");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetSensorInfoActiveArraySize_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetSensorInfoActiveArraySize");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetColorCorrectionModeRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetColorCorrectionModeRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetSensorOrientation_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetSensorOrientation");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetColorCorrectionGainsRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetColorCorrectionGainsRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetColorCorrectionTransformResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetColorCorrectionTransformResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAEAntibandingModeRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAEAntibandingModeRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAEExposureCompensationRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAEExposureCompensationRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAELockRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAELockRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAEModeRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAEModeRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAWBLockRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAWBLockRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAWBModeRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAWBModeRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlModeRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlModeRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlSceneModeRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlSceneModeRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetSensorExposureTimeRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetSensorExposureTimeRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlEffectModeRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlEffectModeRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAELockResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAELockResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlExposureUpperTimeLimitRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlExposureUpperTimeLimitRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetJpegGPSTimestampRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetJpegGPSTimestampRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetJpegThumbnailSizeRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetJpegThumbnailSizeRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlForceApplyModeRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlForceApplyModeRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetJpegQualityRequestMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetJpegQualityRequestMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetColorCorrectionMode_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetColorCorrectionMode");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetColorCorrectionGainsResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetColorCorrectionGainsResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetColorCorrectionGains_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetColorCorrectionGains");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetControlAEAntibandingMode_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetControlAEAntibandingMode");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetControlAEExposureCompensation_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetControlAEExposureCompensation");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetControlAEMode_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetControlAEMode");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetControlAERegions_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetControlAERegions");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetControlAWBLock_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetControlAWBLock");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetControlAWBMode_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetControlAWBMode");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetControlMode_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetControlMode");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetControlSceneMode_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetControlSceneMode");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAWBLockResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAWBLockResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetSensorExposureTime_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetSensorExposureTime");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetSensorSensitivity_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetSensorSensitivity");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetControlExposureUpperTimeLimit_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetControlExposureUpperTimeLimit");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetJpegGPSCoordinates_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetJpegGPSCoordinates");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetJpegGPSTimestamp_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetJpegGPSTimestamp");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetJpegThumbnailSize_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetJpegThumbnailSize");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataSetJpegQuality_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataSetJpegQuality");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetColorCorrectionModeResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetColorCorrectionModeResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetColorCorrectionAberrationModeResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetColorCorrectionAberrationModeResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAEExposureCompensationResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAEExposureCompensationResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAERegionsResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAERegionsResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAETargetFPSRangeResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAETargetFPSRangeResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAEStateResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAEStateResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAWBStateResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAWBStateResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlAWBModeResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlAWBModeResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlModeResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlModeResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlSceneModeResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlSceneModeResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetSensorExposureTimeResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetSensorExposureTimeResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetSensorSensitivityResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetSensorSensitivityResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetSensorTimestampResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetSensorTimestampResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetSensorFrameDurationResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetSensorFrameDurationResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlEffectModeResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlEffectModeResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlExposureUpperTimeLimitResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlExposureUpperTimeLimitResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetJpegGPSCoordinatesResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetJpegGPSCoordinatesResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetJpegGPSTimestampResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetJpegGPSTimestampResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetJpegThumbnailSizeResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetJpegThumbnailSizeResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetControlForceApplyModeResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetControlForceApplyModeResultMetadata");
            }

            [Test]
            public void NativeBinding_MLCameraMetadataGetJpegQualityResultMetadata_Exists()
            {
                AssertThatMethodExists("MLCameraMetadataGetJpegQualityResultMetadata");
            }
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
