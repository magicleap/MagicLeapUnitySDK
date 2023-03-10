using NUnit.Framework;
using UnityEngine.XR.MagicLeap;

namespace Tests.Editor
{
    public class MLResultEditModeTests
    {
        /// <summary>
        /// Test to make sure "result.IsOk" works as expected.
        /// </summary>
        [Test]
        public void MLResult_with_Code_Ok_Returns_IsOk()
        {
            MLResult result = MLResult.Create(MLResult.Code.Ok);

            Assert.IsTrue(result.IsOk);
        }

        /// <summary>
        /// Tests to make sure the MLResult.Create method behaves as expected.
        /// </summary>
        [Test]
        public void MLResult_Create_Returns_Result_with_Given_Code()
        {
            MLResult result = MLResult.Create(MLResult.Code.Ok);

            Assert.AreEqual(result.Result, MLResult.Code.Ok);
        }
    }
}