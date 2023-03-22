using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLMediaRecorder : NativeBindingsTests
    {
        private MlSdkDllLoader lib;
    
        [OneTimeSetUp]
        public void Init()
        {
            lib = new MlSdkDllLoader();
            lib.Load("media_recorder.magicleap");
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            lib.Free();
        } 
    
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLMediaRecorder);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLMediaRecorderSetOutputFileForPath_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderSetOutputFileForPath");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderPrepare_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderPrepare");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderStop_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderStop");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderCreate_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderCreate");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderDestroy_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderDestroy");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderSetOutputFileForFD_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderSetOutputFileForFD");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderSetEventCallbacks_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderSetEventCallbacks");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderSetVideoSource_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderSetVideoSource");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderSetAudioSource_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderSetAudioSource");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderSetOutputFormat_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderSetOutputFormat");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderSetVideoEncoder_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderSetVideoEncoder");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderSetAudioEncoder_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderSetAudioEncoder");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderSetMaxDuration_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderSetMaxDuration");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderSetMaxFileSize_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderSetMaxFileSize");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderSetGeoLocation_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderSetGeoLocation");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderStart_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderStart");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderGetInputSurface_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderGetInputSurface");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderGetMaxAmplitude_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderGetMaxAmplitude");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderReleaseInputSurface_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderReleaseInputSurface");
        }

        [Test]
        public void NativeBinding_MLMediaRecorderReset_Exists()
        {
            AssertThatMethodExists("MLMediaRecorderReset");
        }
    }
}