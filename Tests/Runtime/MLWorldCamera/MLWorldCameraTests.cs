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

        private static IEnumerable SettingsModes = new[]
        {
            MLWorldCamera.Mode.LowExposure,
            MLWorldCamera.Mode.NormalExposure,
            MLWorldCamera.Mode.Unknown,
            MLWorldCamera.Mode.LowExposure | MLWorldCamera.Mode.NormalExposure
        };

        private static IEnumerable SettingsIDs = new[]
        {
            MLWorldCamera.CameraId.Center,
            MLWorldCamera.CameraId.Left,
            MLWorldCamera.CameraId.Right,
            MLWorldCamera.CameraId.All,
            MLWorldCamera.CameraId.Center | MLWorldCamera.CameraId.Left,
            MLWorldCamera.CameraId.Center | MLWorldCamera.CameraId.Right,
            MLWorldCamera.CameraId.Right | MLWorldCamera.CameraId.Left
        };
        
        [SetUp]
        public void Connect()
        {
            worldCamera = new MLWorldCamera();
            var settings = new MLWorldCamera.Settings();
            var result = worldCamera.Connect(settings);
            result.AssertOk();
        }
        
        [Test]
        public void MLWorldCamera_Connect()
        {
            Assert.That(worldCamera.IsConnected);
        }
        
        [Test]
        public void MLWorldCamera_UpdateSettings(
            [ValueSource(nameof(SettingsModes))] MLWorldCamera.Mode mode,
            [ValueSource(nameof(SettingsIDs))] MLWorldCamera.CameraId cameraId)
        {
            var settings = new MLWorldCamera.Settings(mode, cameraId);
            var result = worldCamera.UpdateSettings(settings);
            result.AssertOk();
        }

        [Test]
        public void MLWorldCamera_GetLatestWorldCameraData()
        {
            MLResult result = worldCamera.GetLatestWorldCameraData(out var frames);
            result.AssertOk();
        }

        [Test]
        public void MLWorldCamera_Disconnect()
        {
            var result = worldCamera.Disconnect();
            result.AssertOk();
        }
    }
}
