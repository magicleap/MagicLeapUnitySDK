// Disabling deprecated warning for the internal project
#pragma warning disable 618

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public class MLPermissionsTests
    {
        [Test]
        public void Camera_Permission_Request_Test()
        {
            Assert.IsTrue(RequestPermission(MLPermission.Camera));
        }

        [Test]
        public void RecordAudio_Permission_Request_Test()
        {
            Assert.IsTrue(RequestPermission(MLPermission.RecordAudio));
        }

        [Test]
        public void ReadExternalStorage_Permission_Request_Test()
        {
            Assert.IsTrue(RequestPermission(MLPermission.ReadExternalStorage));
        }

        [Test]
        public void WriteExternalStorage_Permission_Request_Test()
        {
            Assert.IsTrue(RequestPermission(MLPermission.WriteExternalStorage));
        }

        [Test]
        public void Internet_Permission_Request_Test()
        {
            Assert.IsTrue(RequestPermission(MLPermission.Internet));
        }

        [Test]
        public void EyeTracking_Permission_Request_Test()
        {
            Assert.IsTrue(RequestPermission(MLPermission.EyeTracking));
        }

        [Test]
        public void PupilSize_Permission_Request_Test()
        {
            Assert.IsTrue(RequestPermission(MLPermission.PupilSize));
        }

        [Test]
        public void SpatialAnchors_Permission_Request_Test()
        {
            Assert.IsTrue(RequestPermission(MLPermission.SpatialAnchors));
        }

        [Test]
        public void HandTracking_Permission_Request_Test()
        {
            Assert.IsTrue(RequestPermission(MLPermission.HandTracking));
        }

        [Test]
        public void WebView_Permission_Request_Test()
        {
            Assert.IsTrue(RequestPermission(MLPermission.WebView));
        }

        [Test]
        public void MarkerTracking_Permission_Request_Test()
        {
            Assert.IsTrue(RequestPermission(MLPermission.MarkerTracking));
        }

        [Test]
        public void VoiceInput_Permission_Request_Test()
        {
            Assert.IsTrue(RequestPermission(MLPermission.VoiceInput));
        }

        [Test]
        public void SpatialMapping_Permission_Request_Test()
        {
            Assert.IsTrue(RequestPermission(MLPermission.SpatialMapping));
        }

        [Test]
        public void DepthCamera_Permission_Request_Test()
        {
            Assert.IsTrue(RequestPermission(MLPermission.DepthCamera));
        }

        [Test]
        [TestCase("123123123")]
        public void Invalid_Permission_Request_Test(string permission)
        {
            Assert.IsFalse(RequestPermission(permission));
        }

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Empty_Permission_Request_Test(string permission)
        {
            LogAssert.Expect(LogType.Error, "MLPermissions: requested permission name is blank");
            RequestPermission(permission);
        }

        private bool RequestPermission(string permission)
        {
            MLPermissions.Callbacks permissionCallbacks = new MLPermissions.Callbacks();
            MLPermissions.RequestPermission(permission, permissionCallbacks);

            return MLPermissions.CheckPermission(permission).IsOk;
        }
    }
}
