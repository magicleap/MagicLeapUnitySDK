// Disabling deprecated warning for the internal project
#pragma warning disable 618

using NUnit.Framework;
using System;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public partial class MLEyesTests
    {
        private MagicLeapInputs mlInputs;
        private readonly MLPermissions.Callbacks permissionCallbacks = new MLPermissions.Callbacks();

        [Test]
        public void EyeTracking_CheckPermission()
        {
            Assert.IsTrue(CheckPermission());
        }

        [Test]
        public void EyeTracking_StartTracking()
        {
            SetUp();
            try
            {
                InputSubsystem.Extensions.MLEyes.StartTracking();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            TearDown();
        }

        [Test]
        public void EyeTracking_StopTracking()
        {
            SetUp();
            InputSubsystem.Extensions.MLEyes.StartTracking();
            try
            {
                TearDown();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        private void SetUp()
        {
            mlInputs = new MagicLeapInputs();
            mlInputs.Enable();

            CheckPermission();
        }

        private void TearDown()
        {
            mlInputs.Disable();
            mlInputs.Dispose();

            InputSubsystem.Extensions.MLEyes.StopTracking();
        }

        private bool CheckPermission()
        {
            MLPermissions.RequestPermission(MLPermission.EyeTracking, permissionCallbacks);

            return MLPermissions.CheckPermission(MLPermission.EyeTracking).IsOk;
        }
    }
}
