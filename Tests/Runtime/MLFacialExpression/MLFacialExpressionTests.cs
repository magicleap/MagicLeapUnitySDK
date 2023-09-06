using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public class MLFacialExpressionTests
    {
        MLResult result;

        [Test]
        public void MLFacialExpression_UpdateSettingsEnable()
        {
            result = MLResult.Create(MLFacialExpression.UpdateSettings(new MLFacialExpression.Settings { EnableEyeExpression = true}));
            Assert.IsTrue(result.IsOk, string.Format("UpdateSettingsEnable failed: {0}", result.ToString()));
        }

        [Test]
        public void MLFacialExpression_UpdateSettingsDisable()
        {
            result = MLResult.Create(MLFacialExpression.UpdateSettings(new MLFacialExpression.Settings { EnableEyeExpression = true }));
            Assert.IsTrue(result.IsOk, string.Format("UpdateSettingsEnable failed: {0}", result.ToString()));
        }

        [Test]
        public void MLFacialExpression_GetEyeData()
        {
            Assert.Pass("Must be tested with manual scene at this time");
        }
    }
}
