using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLAudioOutput : NativeBindingsTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLAudioOutput);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLAudioGetOutputDevice_Exists()
        {
            AssertThatMethodExists("MLAudioGetOutputDevice");
        }

        [Test]
        public void NativeBinding_MLAudioGetMasterVolume_Exists()
        {
            AssertThatMethodExists("MLAudioGetMasterVolume");
        }

        [Test]
        public void NativeBinding_MLAudioSetMasterVolumeCallback_Exists()
        {
            AssertThatMethodExists("MLAudioSetMasterVolumeCallback");
        }

        [Test]
        public void NativeBinding_MLAudioGetResultString_Exists()
        {
            AssertThatMethodExists("MLAudioGetResultString");
        }

        [Test]
        public void NativeBinding_MLAudioSetSpatialSoundEnable_Exists()
        {
            AssertThatMethodExists("MLAudioSetSpatialSoundEnable");
        }

        [Test]
        public void NativeBinding_MLAudioGetSpatialSoundEnable_Exists()
        {
            AssertThatMethodExists("MLAudioGetSpatialSoundEnable");
        }

        [Test]
        public void NativeBinding_MLAudioSetSpatialSoundPosition_Exists()
        {
            AssertThatMethodExists("MLAudioSetSpatialSoundPosition");
        }

        [Test]
        public void NativeBinding_MLAudioGetSpatialSoundPosition_Exists()
        {
            AssertThatMethodExists("MLAudioGetSpatialSoundPosition");
        }

        [Test]
        public void NativeBinding_MLAudioSetMediaEventCallback_Exists()
        {
            AssertThatMethodExists("MLAudioSetMediaEventCallback");
        }

        [Test]
        public void NativeBinding_MLAudioSetSoundBypassesMasterVolume_Exists()
        {
            AssertThatMethodExists("MLAudioSetSoundBypassesMasterVolume");
        }

        [Test]
        public void NativeBinding_MLAudioGetSoundBypassesMasterVolume_Exists()
        {
            AssertThatMethodExists("MLAudioGetSoundBypassesMasterVolume");
        }
    }
}