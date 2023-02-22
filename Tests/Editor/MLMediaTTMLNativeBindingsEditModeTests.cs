using System.Reflection;
using NUnit.Framework;

namespace Tests.Editor
{
    public class MLMediaTTMLNativeBindingsEditModeTests : NativeBindingsTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLMediaTTML);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }
    }
}