using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLTime : NativeBindingsTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLTime);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLTimeConvertSystemTimeToMLTime_Exists()
        {
            AssertThatMethodExists("MLTimeConvertSystemTimeToMLTime");
        }

        [Test]
        public void NativeBinding_MLTimeConvertMLTimeToSystemTime_Exists()
        {
            AssertThatMethodExists("MLTimeConvertMLTimeToSystemTime");
        }
    }
}