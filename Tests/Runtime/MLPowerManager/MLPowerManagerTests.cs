using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public class MLPowerManagerTests
    {
        MLResult result;

        #region Test Parameters
        // Some states can't be tested; None will be returned for 
        // invalid/no power, DisabledWhileCharging is returned when the 
        // controller is charging, and Sleep is not supported
        private static readonly IEnumerable PowerStatesToTest = new[]
        {
            MLPowerManager.PowerState.Normal,
            MLPowerManager.PowerState.Standby,
        };
        #endregion

        // Ensure that controller is connected to avoid PowerManagerNotConnect error
        [Test]
        public void MLPowerManager_SetPowerState(
            [ValueSource(nameof(PowerStatesToTest))] MLPowerManager.PowerState state)
        {
            result = MLResult.Create(MLPowerManager.SetPowerState(new MLPowerManager.Settings { State = state }));
            Assert.IsTrue(result.IsOk, string.Format("SetPowerState failed: {0} on state {1}", result.ToString(), state.ToString()));
        }

        [Test]
        public void MLPowerManager_GetComponentProperties()
        {
            result = MLResult.Create(MLPowerManager.GetComponentProperties(out _));
            Assert.IsTrue(result.IsOk, string.Format("GetComponentProperties failed: {0}", result.ToString()));
        }

        [Test]
        public void MLPowerManager_GetAvailblePowerStates()
        {
            result = MLResult.Create(MLPowerManager.GetAvailablePowerStates(out _));
            Assert.IsTrue(result.IsOk, string.Format("GetAvailablePowerStates failed: {0}", result.ToString()));
        }

        [Test]
        public void MLPowerManager_GetPowerState()
        {
            result = MLResult.Create(MLPowerManager.GetPowerState(out _));
            Assert.IsTrue(result.IsOk, string.Format("GetPowerState failed: {0}", result.ToString()));
        }

        [Test]
        public void MLPowerManager_GetAvailableProperties()
        {
            result = MLResult.Create(MLPowerManager.GetAvailableProperties(out _));
            Assert.IsTrue(result.IsOk, string.Format("GetAvailableProperties failed: {0}", result.ToString()));
        }
    }
}
