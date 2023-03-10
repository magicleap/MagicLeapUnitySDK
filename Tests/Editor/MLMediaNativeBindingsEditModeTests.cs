using System.Reflection;
using NUnit.Framework;

namespace Tests.Editor
{
    public class MLMediaNativeBindingsEditModeTests : NativeBindingsTests
    {
        private MlSdkDllLoader lib;
    
        [OneTimeSetUp]
        public void Init()
        {
            lib = new MlSdkDllLoader();
            lib.Load("media_error.magicleap");
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            lib.Free();
        } 
    
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLMedia);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLMediaResultGetString_Exists()
        {
            AssertThatMethodExists("MLMediaResultGetString");
        }
    }
}