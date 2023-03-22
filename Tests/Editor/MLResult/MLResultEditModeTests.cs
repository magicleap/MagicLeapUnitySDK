using NUnit.Framework;
using static UnityEngine.XR.MagicLeap.MLResult;

namespace UnitySDKEditorTests
{
    public class MLResult
    {
        /// <summary>
        /// Test to make sure "result.IsOk" works as expected.
        /// </summary>
        [Test]
        public void MLResult_with_Code_Ok_Returns_IsOk()
        {
            UnityEngine.XR.MagicLeap.MLResult result = Create(Code.Ok);

            Assert.IsTrue(result.IsOk);
        }

        /// <summary>
        /// Tests to make sure the MLResult.Create method behaves as expected.
        /// </summary>
        [Test]
        public void MLResult_Create_Returns_Result_with_Given_Code()
        {
            UnityEngine.XR.MagicLeap.MLResult result = Create(Code.Ok);

            Assert.AreEqual(result.Result, Code.Ok);
        }
    }
}