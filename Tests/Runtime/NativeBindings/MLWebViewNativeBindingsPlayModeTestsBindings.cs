using NUnit.Framework;
using System.Reflection;

namespace UnitySDKPlayTests
{
    public partial class MLWebView : NativeBindingsPlayTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLWebView);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_GetJavaVM_Exists()
        {
            AssertThatMethodExists("GetJavaVM");
        }

        [Test]
        public void NativeBinding_GetAppContext_Exists()
        {
            AssertThatMethodExists("GetAppContext");
        }
    }
}