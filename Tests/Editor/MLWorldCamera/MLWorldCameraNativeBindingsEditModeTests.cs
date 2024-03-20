using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLWorldCamera 
    {
        private MlSdkDllLoader lib;
    
        [OneTimeSetUp]
        public void Init()
        {
            lib = new MlSdkDllLoader();
            lib.Load("perception.magicleap");
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            lib.Free();
        } 
    
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLWorldCamera);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLWorldCameraConnect_Exists()
        {
            AssertThatMethodExists("MLWorldCameraConnect");
        }

        [Test]
        public void NativeBinding_MLWorldCameraUpdateSettings_Exists()
        {
            AssertThatMethodExists("MLWorldCameraUpdateSettings");
        }

        [Test]
        public void NativeBinding_MLWorldCameraGetLatestWorldCameraData_Exists()
        {
            AssertThatMethodExists("MLWorldCameraGetLatestWorldCameraData");
        }

        [Test]
        public void NativeBinding_MLWorldCameraReleaseCameraData_Exists()
        {
            AssertThatMethodExists("MLWorldCameraReleaseCameraData");
        }

        [Test]
        public void NativeBinding_MLWorldCameraDisconnect_Exists()
        {
            AssertThatMethodExists("MLWorldCameraDisconnect");
        }
    }
}
