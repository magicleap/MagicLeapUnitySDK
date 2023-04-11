using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public class MLCameraBase : NativeBindingsTests
    {
        private MlSdkDllLoader lib;
    
        [OneTimeSetUp]
        public void Init()
        {
            lib = new MlSdkDllLoader();
            lib.Load("camera.magicleap");
        }
    
        [OneTimeTearDown]
        public void Cleanup()
        {
            lib.Free();
        } 
    
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLCameraBase);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic );
        }

        [Test]
        public void NativeBinding_MLCameraInit_Exists()
        {
            AssertThatMethodExists("MLCameraInit");
        }

        [Test]
        public void NativeBinding_MLCameraDeInit_Exists()
        {
            AssertThatMethodExists("MLCameraDeInit");
        }

        [Test]
        public void NativeBinding_MLCameraConnect_Exists()
        {
            AssertThatMethodExists("MLCameraConnect");
        }

        [Test]
        public void NativeBinding_MLCameraGetNumSupportedStreams_Exists()
        {
            AssertThatMethodExists("MLCameraGetNumSupportedStreams");
        }

        [Test]
        public void NativeBinding_MLCameraDisconnect_Exists()
        {
            AssertThatMethodExists("MLCameraDisconnect");
        }

        [Test]
        public void NativeBinding_MLCameraPrepareCapture_Exists()
        {
            AssertThatMethodExists("MLCameraPrepareCapture");
        }

        [Test]
        public void NativeBinding_MLCameraSetDeviceStatusCallbacks_Exists()
        {
            AssertThatMethodExists("MLCameraSetDeviceStatusCallbacks");
        }

        [Test]
        public void NativeBinding_MLCameraCaptureImage_Exists()
        {
            AssertThatMethodExists("MLCameraCaptureImage");
        }

        [Test]
        public void NativeBinding_MLCameraCaptureVideoStart_Exists()
        {
            AssertThatMethodExists("MLCameraCaptureVideoStart");
        }

        [Test]
        public void NativeBinding_MLCameraCaptureVideoStop_Exists()
        {
            AssertThatMethodExists("MLCameraCaptureVideoStop");
        }

        [Test]
        public void NativeBinding_MLCameraGetDeviceStatus_Exists()
        {
            AssertThatMethodExists("MLCameraGetDeviceStatus");
        }

        [Test]
        public void NativeBinding_MLCameraGetErrorCode_Exists()
        {
            AssertThatMethodExists("MLCameraGetErrorCode");
        }

        [Test]
        public void NativeBinding_MLCameraGetCameraCharacteristics_Exists()
        {
            AssertThatMethodExists("MLCameraGetCameraCharacteristics");
        }
    }
}