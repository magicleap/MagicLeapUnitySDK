using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLVoice : NativeBindingsTests
    {
        [Test]
        public void NativeBindings_MLVoiceIntentEvent_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLVoiceIntentEventSize();
            var type = FindTypeByName("MLVoiceIntentEvent");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLVoiceIntentCallbacks_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLVoiceIntentCallbacksSize();
            var type = FindTypeByName("MLVoiceIntentCallbacks");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLVoiceIntentSettings_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLVoiceIntentSettingsSize();
            var type = FindTypeByName("MLVoiceIntentSettings");

            Assert.IsNull(type);
        }
    }
}
