using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLWebView
    {
        [Test]
        public void NativeBindings_MLWebViewCursorState_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLWebViewCursorStateSize();
            var type = FindTypeByName("MLWebViewCursorState");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLWebViewEventCallbacks_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLWebViewEventCallbacksSize();
            var type = FindTypeByName("MLWebViewEventCallbacks");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLWebViewSettings_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLWebViewSettingsSize();
            var type = FindTypeByName("MLWebViewSettings");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLWebViewProcessSettings_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLWebViewProcessSettingsSize();
            var type = FindTypeByName("MLWebViewProcessSettings");

            Assert.IsNull(type);
        }
        
        [Test]
        public void NativeBindings_MLWebVTTData_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLWebVTTDataSize();
            var type = FindTypeByName("MLWebVTTData");

            Assert.IsNull(type);
        }
    }
}