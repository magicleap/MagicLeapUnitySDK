using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLWebRTC
    {
        public class AudioSink : NativeBindingsTests
        {
            [SetUp]
            public void SetupNativeBindings()
            {
                var apiType = typeof(UnityEngine.XR.MagicLeap.MLWebRTC.AudioSink);
                nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
            }

            [Test]
            public void NativeBinding_MLWebRTCAudioSinkCreate_Exists()
            {
                AssertThatMethodExists("MLWebRTCAudioSinkCreate");
            }

            [Test]
            public void NativeBinding_MLWebRTCAudioSinkCreateEx_Exists()
            {
                AssertThatMethodExists("MLWebRTCAudioSinkCreateEx");
            }

            [Test]
            public void NativeBinding_MLWebRTCAudioSinkSetSource_Exists()
            {
                AssertThatMethodExists("MLWebRTCAudioSinkSetSource");
            }

            [Test]
            public void NativeBinding_MLWebRTCAudioSinkSetPosition_Exists()
            {
                AssertThatMethodExists("MLWebRTCAudioSinkSetPosition");
            }

            [Test]
            public void NativeBinding_MLWebRTCAudioSinkSetPositionEx_Exists()
            {
                AssertThatMethodExists("MLWebRTCAudioSinkSetPositionEx");
            }

            [Test]
            public void NativeBinding_MLWebRTCAudioSinkSetSoundVolume_Exists()
            {
                AssertThatMethodExists("MLWebRTCAudioSinkSetSoundVolume");
            }

            [Test]
            public void NativeBinding_MLWebRTCAudioSinkSetSoundOrientation_Exists()
            {
                AssertThatMethodExists("MLWebRTCAudioSinkSetSoundOrientation");
            }

            [Test]
            public void NativeBinding_MLWebRTCAudioSinkSetDirectSoundLevels_Exists()
            {
                AssertThatMethodExists("MLWebRTCAudioSinkSetDirectSoundLevels");
            }

            [Test]
            public void NativeBinding_MLWebRTCAudioSinkSetRoomSoundLevels_Exists()
            {
                AssertThatMethodExists("MLWebRTCAudioSinkSetRoomSoundLevels");
            }

            [Test]
            public void NativeBinding_MLWebRTCAudioSinkSetSoundDistanceProperties_Exists()
            {
                AssertThatMethodExists("MLWebRTCAudioSinkSetSoundDistanceProperties");
            }

            [Test]
            public void NativeBinding_MLWebRTCAudioSinkSetSoundRadiationProperties_Exists()
            {
                AssertThatMethodExists("MLWebRTCAudioSinkSetSoundRadiationProperties");
            }

            [Test]
            public void NativeBinding_MLWebRTCAudioSinkResetPosition_Exists()
            {
                AssertThatMethodExists("MLWebRTCAudioSinkResetPosition");
            }

            [Test]
            public void NativeBinding_MLWebRTCAudioSinkSetCacheSize_Exists()
            {
                AssertThatMethodExists("MLWebRTCAudioSinkSetCacheSize");
            }

            [Test]
            public void NativeBinding_MLWebRTCAudioSinkDestroy_Exists()
            {
                AssertThatMethodExists("MLWebRTCAudioSinkDestroy");
            }
        }
    }
}