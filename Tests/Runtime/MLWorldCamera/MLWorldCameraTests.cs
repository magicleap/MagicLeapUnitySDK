using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    [TestFixture]
    public partial class MLWorldCameraTests
    {
        private MLWorldCamera worldCamera;
        private MLWorldCamera.Settings settings = new MLWorldCamera.Settings(MLWorldCamera.Mode.NormalExposure | MLWorldCamera.Mode.LowExposure, MLWorldCamera.CameraId.All);

        [Test]
        public void MLWorldCamera_CheckPermission()
        {
            Assert.IsTrue(CheckWorldCameraPermissions());
        }

        [UnityTest]
        public IEnumerator MLWorldCamera_Connect()
        {
            MLResult result = SetUpConnect();
            Assert.That(result.IsOk, result.ToString());
            yield return TearDown();
        }

        [UnityTest]
        public IEnumerator MLWorldCamera_IsStarted()
        {
            yield return SetUp();
            Assert.IsTrue(worldCamera.IsConnected);
            yield return TearDown();
        }

        [UnityTest]
        public IEnumerator MLWorldCamera_GetLatestWorldCameraData()
        {
            yield return SetUp();
            MLResult result = worldCamera.GetLatestWorldCameraData(out var frames);
            yield return new WaitUntil(() => result.IsOk || worldCamera.IsConnected);
            Assert.That(result.IsOk, $"{result.ToString()} {frames}");
            yield return TearDown();
        }

        [UnityTest]
        public IEnumerator MLWorldCamera_MarshallDataFromFrames()
        {
            yield return SetUp();
            MLResult result = worldCamera.GetLatestWorldCameraData(out var frames);
            yield return new WaitUntil(() => result.IsOk || worldCamera.IsConnected);
            if (result.IsOk)
            {
                try
                {
                    byte[] imageData = new byte[frames[0].FrameBuffer.DataSize];

                    var cameraAndFrameType = new ValueTuple<MLWorldCamera.CameraId, MLWorldCamera.Frame.Type>(frames[0].CameraId, frames[0].FrameType);
                    System.Runtime.InteropServices.Marshal.Copy(frames[0].FrameBuffer.Data, imageData, 0, frames[0].FrameBuffer.DataSize);

                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
            else
            {
                Assert.Fail($"MLWorldCamera__GetLatestWorldCameraData result: {result}");
            }
            yield return TearDown();
        }

        private IEnumerator SetUp()
        {
            var result = SetUpConnect();
            yield return new WaitUntil(() => result.IsOk || worldCamera.IsConnected);
        }

        private MLResult SetUpConnect()
        {
            CheckWorldCameraPermissions();
            worldCamera = new();
            return worldCamera.Connect(settings);
        }

        private IEnumerator TearDown()
        {
            worldCamera.Disconnect();
            yield return new WaitUntil(() => !worldCamera.IsConnected);
        }

        private static bool CheckWorldCameraPermissions()
        {
            MLPermissions.RequestPermission(MLPermission.Camera, new MLPermissions.Callbacks());

            return MLPermissions.CheckPermission(MLPermission.Camera).IsOk;
        }

        //TODO: Create test for worldCamVisualizer.RenderFrame(frame);
    }
}
