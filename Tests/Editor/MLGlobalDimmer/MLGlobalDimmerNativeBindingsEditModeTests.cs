using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLGlobalDimmer : NativeBindingsTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLGlobalDimmer);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_UnityMagicLeap_RenderingSetGlobalDimmerValue_Exists()
        {
            AssertThatMethodExists("UnityMagicLeap_RenderingSetGlobalDimmerValue");
        }
    }
}