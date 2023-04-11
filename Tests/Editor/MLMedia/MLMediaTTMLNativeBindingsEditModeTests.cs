using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLMediaTTML : NativeBindingsTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLMediaTTML);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }
    }
}