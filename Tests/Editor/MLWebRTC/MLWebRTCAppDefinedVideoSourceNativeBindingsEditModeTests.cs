using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLWebRTC 
    {
        public partial class AppDefinedVideoSource : NativeBindingsTests
        {
            [SetUp]
            public void SetupNativeBindings()
            {
                var apiType = typeof(UnityEngine.XR.MagicLeap.MLWebRTC.AppDefinedVideoSource);
                nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceCreateAppDefinedVideoSource_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceCreateAppDefinedVideoSource");
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceAppDefinedVideoSourcePushFrame_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceAppDefinedVideoSourcePushFrame");
            }
        }
    }

   
}