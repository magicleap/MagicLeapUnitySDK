using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLDepthCamera
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLDepthCamera);        
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLDepthCameraConnect_Exists()
        {
            AssertThatMethodExists("MLDepthCameraConnect");   
        }

        [Test]
        public void NativeBinding_MLDepthCameraUpdateSettings_Exists()
        {
            AssertThatMethodExists("MLDepthCameraUpdateSettings");
        }

        [Test]
        public void NativeBinding_MLDepthCameraGetLatestDepthData_Exists()
        {
            AssertThatMethodExists("MLDepthCameraGetLatestDepthData");
        }

        [Test]
        public void NativeBinding_MLDepthCameraReleaseDepthData_Exists()
        {
            AssertThatMethodExists("MLDepthCameraReleaseDepthData");
        }

        [Test]
        public void NativeBinding_MLDepthCameraDisconnect_Exists()
        {
            AssertThatMethodExists("MLDepthCameraDisconnect");
        }
    }
}