// Disabling deprecated warning for the internal project
#pragma warning disable 618

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public class MLCameraTests
    {
        private MLPermissions.Callbacks permissionCallbacks;
        private MLCamera captureCamera;
        private MLMediaRecorder mediaRecorder;
        private List<MLCamera.StreamCapability> streamCapabilities;
        private MLCamera.Metadata Metadata;
        private MLCamera.ConnectFlag ConnectFlag;
        private MLCameraBase.CaptureType CaptureType;
        private MLCameraBase.OutputFormat OutputFormat;
        private MLCameraBase.CaptureFrameRate FrameRate;

        private bool cameraDeviceAvailable;

        private bool IsCameraConnected => captureCamera != null && captureCamera.ConnectionEstablished;

        private GameObject rig;

        [OneTimeSetUp]
        public void AddRigToScene()
        {
            var prefab = Resources.Load("XR Rig") as GameObject;
            rig = UnityEngine.Object.Instantiate(prefab);
            Assert.IsNotNull(rig);
        }

        [OneTimeTearDown]
        public void RemoveRigFromScene()
        {
            if (rig != null)
            {
                UnityEngine.Object.Destroy(rig);
            }
        }

        [UnitySetUp]
        public IEnumerator MLCamera_EnableCamera()
        {
            yield return EnableMLCamera();
        }

        #region Test Parameters
        public enum VideoStabilization
        {
            Stabilization_Enabled,
            Stabilization_Disabled
        }

        private static readonly IEnumerable StabilizationToggle = new[]
        {
            VideoStabilization.Stabilization_Enabled,
            VideoStabilization.Stabilization_Disabled
        };

        private static readonly IEnumerable ConnectFlagsToTest = new[]
        {
            MLCameraBase.ConnectFlag.CamOnly,
            MLCameraBase.ConnectFlag.VirtualOnly,
            MLCameraBase.ConnectFlag.MR
        };

        private static IEnumerable MRQualitiesToTest = new[]
        {
            MLCameraBase.MRQuality._648x720,
            MLCameraBase.MRQuality._972x1080,
            MLCameraBase.MRQuality._1944x2160,
            MLCameraBase.MRQuality._960x720,
            MLCameraBase.MRQuality._1440x1080,
            MLCameraBase.MRQuality._2880x2160
        };

        private static IEnumerable FrameRatesToTest = new[]
        {
            //MLCameraBase.CaptureFrameRate.None,
            MLCameraBase.CaptureFrameRate._15FPS,
            MLCameraBase.CaptureFrameRate._30FPS,
            MLCameraBase.CaptureFrameRate._60FPS
        };

        private static IEnumerable OutputFormatsToTest = new[]
        {
            MLCameraBase.OutputFormat.YUV_420_888,
            MLCameraBase.OutputFormat.JPEG,
            MLCameraBase.OutputFormat.RGBA_8888
        };

        private static IEnumerable CaptureTypesToTest = new[]
        {
            MLCameraBase.CaptureType.Image,
            MLCameraBase.CaptureType.Video,
            MLCameraBase.CaptureType.Preview
        };
        #endregion

        [UnityTest]
        public IEnumerator MLCamera_ConnectCamera(
            [ValueSource(nameof(ConnectFlagsToTest))] MLCameraBase.ConnectFlag connectFlag,
            [ValueSource(nameof(MRQualitiesToTest))] MLCameraBase.MRQuality mrQuality,
            [ValueSource(nameof(FrameRatesToTest))] MLCameraBase.CaptureFrameRate frameRate,
            [ValueSource(nameof(StabilizationToggle))] VideoStabilization stabilization)
        {
            ConnectCamera(connectFlag, mrQuality, frameRate, (stabilization == VideoStabilization.Stabilization_Enabled));
            yield return new WaitForEndOfFrame();
            DisconnectCamera();
        }

        [UnityTest]
        public IEnumerator MLCamera_PrepareCaptureForCapturePreview(
            [ValueSource(nameof(FrameRatesToTest))] MLCameraBase.CaptureFrameRate frameRate,
            [ValueSource(nameof(ConnectFlagsToTest))] MLCameraBase.ConnectFlag connectFlag,
            [ValueSource(nameof(OutputFormatsToTest))] MLCameraBase.OutputFormat outputFormat)
        {
            FrameRate = frameRate;
            ConnectFlag = connectFlag;
            OutputFormat = outputFormat;
            PrepareCaptureForPreview();
            yield return new WaitForEndOfFrame();
        }

        [UnityTest]
        public IEnumerator MLCamera_PrepareCaptureForCaptureImage(
            [ValueSource(nameof(CaptureTypesToTest))] MLCameraBase.CaptureType captureType,
            [ValueSource(nameof(ConnectFlagsToTest))] MLCameraBase.ConnectFlag connectFlag,
            [ValueSource(nameof(OutputFormatsToTest))] MLCameraBase.OutputFormat outputFormat)
        {
            ConnectFlag = connectFlag;
            CaptureType = captureType;
            OutputFormat = outputFormat;
            PrepareCaptureForCaptureImage();
            yield return new WaitForEndOfFrame();
        }

        [UnityTest]
        public IEnumerator MLCamera_PrepareCaptureForStartRecording(
            [ValueSource(nameof(FrameRatesToTest))] MLCameraBase.CaptureFrameRate frameRate,
            [ValueSource(nameof(CaptureTypesToTest))] MLCameraBase.CaptureType captureType,
            [ValueSource(nameof(ConnectFlagsToTest))] MLCameraBase.ConnectFlag connectFlag,
            [ValueSource(nameof(OutputFormatsToTest))] MLCameraBase.OutputFormat outputFormat)
        {
            FrameRate = frameRate;
            ConnectFlag = connectFlag;
            CaptureType = captureType;
            OutputFormat = outputFormat;
            PrepareCaptureForStartRecording();
            yield return new WaitForSeconds(1);
            StopRecording();
            yield return new WaitForEndOfFrame();
        }

        private void ConnectCamera(MLCameraBase.ConnectFlag connectFlag, MLCameraBase.MRQuality mrQuality, MLCameraBase.CaptureFrameRate frameRate, bool enableVideoStabilization)
        {
            MLCameraBase.ConnectContext context = MLCameraBase.ConnectContext.Create();
            context.Flags = connectFlag;
            context.EnableVideoStabilization = enableVideoStabilization;

            if ((context.Flags != MLCameraBase.ConnectFlag.CamOnly && frameRate == MLCameraBase.CaptureFrameRate._15FPS) ||
                (frameRate == MLCameraBase.CaptureFrameRate._60FPS && (mrQuality.ToString().EndsWith("2160"))))
            {
                Assert.Ignore($"Unsupported camera configuration: {connectFlag} {mrQuality} {frameRate}");
            }

            if (context.Flags != MLCameraBase.ConnectFlag.CamOnly)
            {
                context.MixedRealityConnectInfo = MLCameraBase.MRConnectInfo.Create();
                context.MixedRealityConnectInfo.MRQuality = mrQuality;
                context.MixedRealityConnectInfo.FrameRate = frameRate;
            }

            captureCamera = MLCamera.CreateAndConnect(context);

            Assert.IsNotNull(captureCamera);
        }

        private void DisconnectCamera()
        {
            MLResult result;

            if (captureCamera == null || !IsCameraConnected)
            {
                // Note that some APIs like MLCameraInit() can be called before MLCameraConnect()
                // is called. This is to make sure all is cleaned up if CameraConnect is not called
                //result = MLCamera.Uninitialize();
                //Assert.IsTrue(result.IsOk);
                return;
            }

            streamCapabilities = null;

            result = captureCamera.Disconnect();
            Assert.IsTrue(result.IsOk, $"Couldn't disconnect camera: {result.Result}");
        }

        private IEnumerator EnableMLCamera()
        {
            const int MaxAttempts = 10;
            int attempts = 0;
            while (!cameraDeviceAvailable && attempts < MaxAttempts)
            {
                MLResult result =
                    MLCamera.GetDeviceAvailabilityStatus(MLCamera.Identifier.Main, out cameraDeviceAvailable);
                if (!(result.IsOk && cameraDeviceAvailable))
                {
                    // Wait until camera device is available
                    yield return new WaitForSeconds(1.0f);
                }
                attempts++;
            }

            Assert.IsTrue(attempts < MaxAttempts, "Too many unsuccessful attempts to init camera.");
            Assert.IsTrue(cameraDeviceAvailable, "failed to get main camera ready status");
        }

        private MLResult PrepareCaptureForPreview()
        {
            CaptureType = MLCameraBase.CaptureType.Preview;
            mediaRecorder = MLMediaRecorder.Create();

            if (CaptureType == MLCameraBase.CaptureType.Preview && OutputFormat == MLCameraBase.OutputFormat.YUV_420_888)
            {
                ConnectCamera(ConnectFlag, MLCameraBase.MRQuality._960x720, MLCameraBase.CaptureFrameRate._30FPS, false);

                Assert.IsTrue(GetImageStreamCapabilities(), "failed to get image stream capabilities");
            }
            else
            {
                Assert.Ignore($"Unsupported camera configuration: {CaptureType} {OutputFormat}");
            }

            MLCamera.CaptureConfig captureConfig = new MLCamera.CaptureConfig();
            captureConfig.CaptureFrameRate = FrameRate;
            captureConfig.StreamConfigs = new MLCamera.CaptureStreamConfig[1];
            captureConfig.StreamConfigs[0] = MLCamera.CaptureStreamConfig.Create(GetStreamCapability(), OutputFormat);
            captureConfig.StreamConfigs[0].Surface = mediaRecorder.InputSurface;

            if (captureCamera != null)
            {
                MLResult result = captureCamera.PrepareCapture(captureConfig, out Metadata);
                Assert.IsTrue(result.IsOk, $"PrepareCaptureForPreview PrepareCapture failed. Reason: {result}.");
                Assert.NotNull(Metadata);

                return result;
            }
            return MLResult.Create(MLResult.Code.Ok);
        }

        private MLResult PrepareCaptureForCaptureImage()
        {
            if (CaptureType == MLCameraBase.CaptureType.Image && OutputFormat != MLCameraBase.OutputFormat.RGBA_8888)
            {
                ConnectCamera(ConnectFlag, MLCameraBase.MRQuality._960x720, MLCameraBase.CaptureFrameRate._30FPS, false);

                Assert.IsTrue(GetImageStreamCapabilities(), "failed to get image stream capabilities");
            }
            else
            {
                Assert.Ignore($"Unsupported camera configuration: {CaptureType} {OutputFormat}");
            }
            MLCamera.CaptureConfig captureConfig = new MLCamera.CaptureConfig();

            captureConfig.CaptureFrameRate = MLCamera.CaptureFrameRate._30FPS;
            captureConfig.StreamConfigs = new MLCamera.CaptureStreamConfig[1];
            captureConfig.StreamConfigs[0] =
                MLCamera.CaptureStreamConfig.Create(GetStreamCapability(), OutputFormat);

            if (captureCamera != null)
            {
                MLResult result = captureCamera.PrepareCapture(captureConfig, out Metadata);

                if (!result.IsOk)
                    return result;

                result = captureCamera.PreCaptureAEAWB();
                Assert.IsTrue(result.IsOk, $"PrepareCaptureForCaptureImage PreCaptureAEAWB failed. Reason: {result}.");
                result = captureCamera.CaptureImage(1);
                Assert.IsTrue(result.IsOk, $"PrepareCaptureForCaptureImage CaptureImage failed. Reason: {result}.");

                DisconnectCamera();

                return result;
            }

            return MLResult.Create(MLResult.Code.Ok);
        }

        private MLResult PrepareCaptureForStartRecording()
        {
            if (CaptureType != MLCameraBase.CaptureType.Image && OutputFormat != MLCameraBase.OutputFormat.JPEG)
            {
                ConnectCamera(ConnectFlag, MLCameraBase.MRQuality._960x720, MLCameraBase.CaptureFrameRate._30FPS, false);

                Assert.IsTrue(GetImageStreamCapabilities(), "failed to get image stream capabilities");
            }
            else
            {
                Assert.Ignore($"Unsupported camera configuration: {CaptureType} {OutputFormat}");
            }

            if ((ConnectFlag != MLCameraBase.ConnectFlag.CamOnly && FrameRate == MLCameraBase.CaptureFrameRate._15FPS))
            {
                Assert.Ignore($"Unsupported camera configuration: {ConnectFlag} {FrameRate}");
            }

            MLMediaRecorder MediaRecorder = MLMediaRecorder.Create();
            Assert.NotNull(MediaRecorder, "Couldn't create media recorder.");

            MLCamera.CaptureConfig captureConfig = new MLCamera.CaptureConfig();
            captureConfig.CaptureFrameRate = FrameRate;
            captureConfig.StreamConfigs = new MLCamera.CaptureStreamConfig[1];
            captureConfig.StreamConfigs[0] = MLCamera.CaptureStreamConfig.Create(GetStreamCapability(), OutputFormat);
            captureConfig.StreamConfigs[0].Surface = MediaRecorder.InputSurface;

            if (captureCamera != null)
            {
                MLResult result = captureCamera.PrepareCapture(captureConfig, out Metadata);

                if (MLResult.DidNativeCallSucceed(result.Result, nameof(captureCamera.PrepareCapture)))
                {
                    captureCamera.PreCaptureAEAWB();

                    if (CaptureType == MLCamera.CaptureType.Video)
                    {
                        result = captureCamera.CaptureVideoStart();
                        Assert.IsTrue(result.IsOk, $"PrepareCaptureForStartRecording CaptureVideoStart failed. Reason: {result}.");
                    }

                    if (CaptureType == MLCamera.CaptureType.Preview)
                    {
                        result = captureCamera.CapturePreviewStart();
                        Assert.IsTrue(result.IsOk, $"PrepareCaptureForStartRecording CapturePreviewStart failed. Reason: {result}.");
                    }
                }

                return result;
            }
            return MLResult.Create(MLResult.Code.Ok);
        }

        private void StopRecording()
        {
            MLResult result;
            if (CaptureType == MLCamera.CaptureType.Video)
            {
                result = captureCamera.CaptureVideoStop();
                Assert.IsTrue(result.IsOk, $"PrepareCaptureForStartRecording CaptureVideoStop failed. Reason: {result}.");
            }

            if (CaptureType == MLCamera.CaptureType.Preview)
            {
                result = captureCamera.CapturePreviewStop();
                Assert.IsTrue(result.IsOk, $"PrepareCaptureForStartRecording CapturePreviewStop failed. Reason: {result}.");
            }
        }

        private bool GetImageStreamCapabilities()
        {
            var result =
                captureCamera.GetStreamCapabilities(out MLCamera.StreamCapabilitiesInfo[] streamCapabilitiesInfo);

            if (!result.IsOk)
            {
                Debug.Log("Could not get Stream capabilities Info.");
                return false;
            }

            streamCapabilities = new List<MLCamera.StreamCapability>();

            for (int i = 0; i < streamCapabilitiesInfo.Length; i++)
            {
                foreach (var streamCap in streamCapabilitiesInfo[i].StreamCapabilities)
                {
                    streamCapabilities.Add(streamCap);
                }
            }

            return streamCapabilities.Count > 0;
        }

        private MLCamera.StreamCapability GetStreamCapability()
        {
            if (ConnectFlag != MLCamera.ConnectFlag.CamOnly)
            {
                return streamCapabilities.FirstOrDefault(s => s.CaptureType == CaptureType);
            }

            foreach (var streamCapability in streamCapabilities.Where(s => s.CaptureType == CaptureType))
            {
                return streamCapability;
            }

            return streamCapabilities[0];
        }

        //private void SetUp()
        //{
        //    Assert.IsTrue(CheckCameraPermission());
        //}

        //private void TearDown()
        //{
        //    captureCamera = null;
        //}

        private bool CheckCameraPermission()
        {
            permissionCallbacks = new MLPermissions.Callbacks();
            MLPermissions.RequestPermission(MLPermission.Camera, permissionCallbacks);

            return MLPermissions.CheckPermission(MLPermission.Camera).IsOk;
        }
    }
}
