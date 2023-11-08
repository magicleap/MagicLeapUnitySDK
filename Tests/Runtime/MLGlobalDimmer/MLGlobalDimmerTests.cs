using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public class MLGlobalDimmerTests
    {
        MLResult result;

        [Test]
        public void MLGlobalDimmer_SetValue()
        {
#if UNITY_OPENXR_1_9_0_OR_NEWER
            result = MLGlobalDimmer.SetValue(1);
            Assert.IsTrue(result.IsOk, string.Format("SetValue failed: {0}", result.ToString()));
#endif
        }
    }
}

