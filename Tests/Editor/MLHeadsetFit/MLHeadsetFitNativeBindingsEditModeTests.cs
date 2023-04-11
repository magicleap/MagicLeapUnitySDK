using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLHeadsetFit 
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLHeadsetFit);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLHeadsetFitDestroyClient_Exists()
        {
            AssertThatMethodExists("MLHeadsetFitDestroyClient");
        }

        [Test]
        public void NativeBinding_MLHeadsetFitCreateClient_Exists()
        {
            AssertThatMethodExists("MLHeadsetFitCreateClient");
        }

        [Test]
        public void NativeBinding_MLHeadsetFitGetState_Exists()
        {
            AssertThatMethodExists("MLHeadsetFitGetState");
        }
    }
}