using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public class MLTimeTests
    {
        MLResult result;
        MLTime time;

        [Test, Order(1)]
        public void MLTime_ConvertSystemTimeToMLTimeTest()
        {
            result = MLTime.ConvertSystemTimeToMLTime(0, out time);
            Assert.IsTrue(result.IsOk, string.Format("ConvertSystemTimeToMLTime failed: {0}", result.ToString()));
        }

        [Test, Order(2)]
        public void MLTime_ConvertMLTimeToSystemTimeTest()
        {
            result = MLTime.ConvertMLTimeToSystemTime(time, out long _);
            Assert.IsTrue(result.IsOk, string.Format("ConvertMLTimeToSystemTime failed: {0}", result.ToString()));
        }
    }
}
