using System.Reflection;
using NUnit.Framework;

namespace Tests.Editor
{
    public class MLCVCameraNativeBindingsEditModeTests : NativeBindingsTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLCVCamera);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLCVCameraTrackingCreate_Exists()
        {
            AssertThatMethodExists("MLCVCameraTrackingCreate");
        }

        [Test]
        public void NativeBinding_MLCVCameraTrackingDestroy_Exists()
        {
            AssertThatMethodExists("MLCVCameraTrackingDestroy");
        }

        [Test]
        public void NativeBinding_MLCVCameraGetFramePose_Exists()
        {
            AssertThatMethodExists("MLCVCameraGetFramePose");
        }
    }
}