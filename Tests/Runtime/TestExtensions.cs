using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace UnityEngine.XR.MagicLeap
{
    public static class TestExtensions
    {
        public static void AssertOk(this MLResult mlResult, string methodName = "The API", params MLResult.Code[] validResultCodes)
        {
#if UNITY_EDITOR
            if (mlResult.Result == MLResult.Code.NotImplemented)
            {
                Assert.Ignore($"{methodName} is not implemented on host platform.");
            }
#endif
            if (validResultCodes.Length == 0)
            {
                Assert.That(mlResult.IsOk, $"{methodName} returned {mlResult.Result.ToString()}");
            }
            else
            {
                if (validResultCodes.All(code => mlResult.Result != code))
                {
                    Assert.Fail($"{methodName} returned {mlResult.Result.ToString()}");
                }
            }
        }
    }
}
