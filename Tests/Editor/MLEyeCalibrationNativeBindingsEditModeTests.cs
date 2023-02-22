using System.Reflection;
using NUnit.Framework;

namespace Tests.Editor
{
    public class MLEyeCalibrationNativeBindingsEditModeTests : NativeBindingsTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLEyeCalibration);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLEyeCalibrationCreateClient_Exists()
        {
            AssertThatMethodExists("MLEyeCalibrationCreateClient");
        }

        [Test]
        public void NativeBinding_MLEyeCalibrationDestroyClient_Exists()
        {
            AssertThatMethodExists("MLEyeCalibrationDestroyClient");
        }

        [Test]
        public void NativeBinding_MLEyeCalibrationGetState_Exists()
        {
            AssertThatMethodExists("MLEyeCalibrationGetState");
        }
    }
}