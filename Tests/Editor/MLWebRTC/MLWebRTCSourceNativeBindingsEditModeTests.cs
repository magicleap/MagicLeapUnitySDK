using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLWebRTC
    {
        public class Source : NativeBindingsTests
        {
            [SetUp]
            public void SetupNativeBindings()
            {
                var apiType = typeof(UnityEngine.XR.MagicLeap.MLWebRTC.Source);
                nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceCreateLocalSourceForCamera_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceCreateLocalSourceForCamera");
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceCreateLocalSourceForMicrophoneEx_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceCreateLocalSourceForMicrophoneEx");
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceCreateAppDefinedAudioSourceEx_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceCreateAppDefinedAudioSourceEx");
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceIsEnabled_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceIsEnabled");
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceSetEnabled_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceSetEnabled");
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceGetType_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceGetType");
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceGetTrackId_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceGetTrackId");
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceReleaseTrackId_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceReleaseTrackId");
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceDestroy_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceDestroy");
            }
        }
    }
}