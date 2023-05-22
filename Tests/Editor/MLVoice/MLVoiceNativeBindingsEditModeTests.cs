using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLVoice
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLVoice);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.Public);
        }

        [Test]
        public void NativeBinding_MLVoiceIntentCreate_Exists()
        {
            AssertThatMethodExists("MLVoiceIntentCreate");
        }

        [Test]
        public void NativeBinding_MLVoiceIntentDestroy_Exists()
        {
            AssertThatMethodExists("MLVoiceIntentDestroy");
        }

        [Test]
        public void NativeBinding_MLVoiceIntentConfigureSettings_Exists()
        {
            AssertThatMethodExists("MLVoiceIntentConfigureSettings");
        }

        [Test]
        public void NativeBinding_MLVoiceIntentSetCallbacks_Exists()
        {
            AssertThatMethodExists("MLVoiceIntentSetCallbacks");
        }

        [Test]
        public void NativeBinding_MLVoiceIntentStartProcessing_Exists()
        {
            AssertThatMethodExists("MLVoiceIntentStartProcessing");
        }

        [Test]
        public void NativeBinding_MLVoiceIntentStopProcessing_Exists()
        {
            AssertThatMethodExists("MLVoiceIntentStopProcessing");
        }

        [Test]
        public void NativeBinding_MLVoiceIntentIsEnabled_Exists()
        {
            AssertThatMethodExists("MLVoiceIntentIsEnabled");
        }
    }
}
