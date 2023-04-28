using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public class MLMarkerTrackerTests
    {
        [Test]
        public async void MarkerTracker_StartScanningAsync()
        {
            await MLMarkerTracker.StartScanningAsync(null);
        }

        [Test]
        public async void MarkerTracker_StopScanningAsync()
        {
            await MLMarkerTracker.StartScanningAsync(null);
            await MLMarkerTracker.StopScanningAsync();
        }
    }
}
