using System.Reflection;
using NUnit.Framework;

namespace Tests.Editor
{
    public class MLWebRTCFrameNativeBindingsEditModeTests : NativeBindingsTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLWebRTC.VideoSink.Frame);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLWebRTCFrameGetData_Exists()
        {
            AssertThatMethodExists("MLWebRTCFrameGetData");
        }
    }
}