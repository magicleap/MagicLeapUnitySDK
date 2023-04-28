using System.Reflection;
using NUnit.Framework;

namespace UnitySDKPlayTests
{
    public partial class YcbcrRenderer : NativeBindingsPlayTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.YcbcrRenderer);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLYcbcrRendererCreate_Exists()
        {
            AssertThatMethodExists("MLYcbcrRendererCreate");
        }

        [Test]
        public void NativeBinding_MLYcbcrRendererGetEventIdForPluginEvent_Exists()
        {
            AssertThatMethodExists("MLYcbcrRendererGetEventIdForPluginEvent");
        }

        [Test]
        public void NativeBinding_MLYcbcrRendererGetCallbackForPluginEvent_Exists()
        {
            AssertThatMethodExists("MLYcbcrRendererGetCallbackForPluginEvent");
        }
    }
}