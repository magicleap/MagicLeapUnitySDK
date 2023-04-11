using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLAudioInput : NativeBindingsTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLAudioInput);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLAudioSetMicMute_Exists()
        {
            AssertThatMethodExists("MLAudioSetMicMute");
        }

        [Test]
        public void NativeBinding_MLAudioGetMicMute_Exists()
        {
            AssertThatMethodExists("MLAudioGetMicMute");
        }

        [Test]
        public void NativeBinding_MLAudioSetMicMuteCallback_Exists()
        {
            AssertThatMethodExists("MLAudioSetMicMuteCallback");
        }

        [Test]
        public void NativeBinding_MLAudioGetResultString_Exists()
        {
            AssertThatMethodExists("MLAudioGetResultString");
        }

        [Test]
        public void NativeBinding_MLAudioGetBufferedInputDefaults_Exists()
        {
            AssertThatMethodExists("MLAudioGetBufferedInputDefaults");
        }

        [Test]
        public void NativeBinding_MLAudioCreateInputFromMicCapture_Exists()
        {
            AssertThatMethodExists("MLAudioCreateInputFromMicCapture");
        }

        [Test]
        public void NativeBinding_MLAudioGetInputBuffer_Exists()
        {
            AssertThatMethodExists("MLAudioGetInputBuffer");
        }

        [Test]
        public void NativeBinding_MLAudioReleaseInputBuffer_Exists()
        {
            AssertThatMethodExists("MLAudioReleaseInputBuffer");
        }

        [Test]
        public void NativeBinding_MLAudioStartInput_Exists()
        {
            AssertThatMethodExists("MLAudioStartInput");
        }

        [Test]
        public void NativeBinding_MLAudioStopInput_Exists()
        {
            AssertThatMethodExists("MLAudioStopInput");
        }
    }
}