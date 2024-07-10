// Disabling deprecated warning for the internal project
#pragma warning disable 618

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public partial class MLHandTrackingTests
    {
        private InputDevice leftHandDevice;
        private InputDevice rightHandDevice;

        //[Test]
        public void HandTracking_CheckPermission()
        {
            Assert.IsTrue(CheckPermission());
        }

        [SetUp]
        public void HandTracking_StartTracking()
        {
            // CheckPermission();
            try
            {
                InputSubsystem.Extensions.MLHandTracking.StartTracking();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TearDown]
        public void HandTracking_StopTracking()
        {
            try
            {
                InputSubsystem.Extensions.MLHandTracking.StopTracking();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void HandTracking_SetPreRenderHandUpdate()
        {
            //  CheckPermission();
            try
            {
                //InputSubsystem.Extensions.MLHandTracking.StartTracking();
                InputSubsystem.Extensions.MLHandTracking.SetPreRenderHandUpdate(true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void HandTracking_FindMagicLeapDevice()
        {
            //  CheckPermission();
            try
            {
                // InputSubsystem.Extensions.MLHandTracking.StartTracking();
                InputSubsystem.Extensions.MLHandTracking.SetPreRenderHandUpdate(true);
                if (!leftHandDevice.isValid || !rightHandDevice.isValid)
                {
                    leftHandDevice = InputSubsystem.Utils.FindMagicLeapDevice(InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.Left);
                    rightHandDevice = InputSubsystem.Utils.FindMagicLeapDevice(InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.Right);
                    return;
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void HandTracking_TryGetFeatureValue()
        {
            //  CheckPermission();
            try
            {
                // InputSubsystem.Extensions.MLHandTracking.StartTracking();
                InputSubsystem.Extensions.MLHandTracking.SetPreRenderHandUpdate(true);
                if (!leftHandDevice.isValid || !rightHandDevice.isValid)
                {
                    leftHandDevice = InputSubsystem.Utils.FindMagicLeapDevice(InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.Left);
                    rightHandDevice = InputSubsystem.Utils.FindMagicLeapDevice(InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.Right);
                    return;
                }
                leftHandDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.Hand.Confidence, out float leftConfidence);
                leftHandDevice.TryGetFeatureValue(CommonUsages.isTracked, out bool leftIsTracked);

                rightHandDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.Hand.Confidence, out float rightConfidence);
                rightHandDevice.TryGetFeatureValue(CommonUsages.isTracked, out bool rightIsTracked);
                Assert.Pass();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        private bool CheckPermission()
        {
            return MLPermissions.CheckPermission(MLPermission.HandTracking).IsOk;
        }
    }
}
