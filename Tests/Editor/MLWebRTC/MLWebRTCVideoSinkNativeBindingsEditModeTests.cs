using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLWebRTC
    {
        public partial class VideoSink : NativeBindingsTests
        {
            [SetUp]
            public void SetupNativeBindings()
            {
                var apiType = typeof(UnityEngine.XR.MagicLeap.MLWebRTC.VideoSink);
                nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
            }

            [Test]
            public void NativeBinding_MLWebRTCVideoSinkCreate_Exists()
            {
                AssertThatMethodExists("MLWebRTCVideoSinkCreate");
            }

            [Test]
            public void NativeBinding_MLWebRTCVideoSinkSetSource_Exists()
            {
                AssertThatMethodExists("MLWebRTCVideoSinkSetSource");
            }

            [Test]
            public void NativeBinding_MLWebRTCVideoSinkIsNewFrameAvailable_Exists()
            {
                AssertThatMethodExists("MLWebRTCVideoSinkIsNewFrameAvailable");
            }

            [Test]
            public void NativeBinding_MLWebRTCVideoSinkAcquireNextAvailableFrame_Exists()
            {
                AssertThatMethodExists("MLWebRTCVideoSinkAcquireNextAvailableFrame");
            }

            [Test]
            public void NativeBinding_MLWebRTCVideoSinkReleaseFrame_Exists()
            {
                AssertThatMethodExists("MLWebRTCVideoSinkReleaseFrame");
            }

            [Test]
            public void NativeBinding_MLWebRTCVideoSinkDestroy_Exists()
            {
                AssertThatMethodExists("MLWebRTCVideoSinkDestroy");
            }
        }
    }
}