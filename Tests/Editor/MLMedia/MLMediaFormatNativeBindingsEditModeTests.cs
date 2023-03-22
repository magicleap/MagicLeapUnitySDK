using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLMediaFormat : NativeBindingsTests
    {
        private MlSdkDllLoader lib;
    
        [OneTimeSetUp]
        public void Init()
        {
            lib = new MlSdkDllLoader();
            lib.Load("media_format.magicleap");
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            lib.Free();
        } 
    
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLMediaFormat);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLMediaFormatCreateVideo_Exists()
        {
            AssertThatMethodExists("MLMediaFormatCreateVideo");
        }

        [Test]
        public void NativeBinding_MLMediaFormatCreateAudio_Exists()
        {
            AssertThatMethodExists("MLMediaFormatCreateAudio");
        }

        [Test]
        public void NativeBinding_MLMediaFormatCreateSubtitle_Exists()
        {
            AssertThatMethodExists("MLMediaFormatCreateSubtitle");
        }

        [Test]
        public void NativeBinding_MLMediaFormatCreate_Exists()
        {
            AssertThatMethodExists("MLMediaFormatCreate");
        }

        [Test]
        public void NativeBinding_MLMediaFormatCreateCopy_Exists()
        {
            AssertThatMethodExists("MLMediaFormatCreateCopy");
        }

        [Test]
        public void NativeBinding_MLMediaFormatDestroy_Exists()
        {
            AssertThatMethodExists("MLMediaFormatDestroy");
        }

        [Test]
        public void NativeBinding_MLMediaFormatObjectToString_Exists()
        {
            AssertThatMethodExists("MLMediaFormatObjectToString");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKeyValueInt32_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKeyValueInt32");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKeyValueInt64_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKeyValueInt64");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKeyValueFloat_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKeyValueFloat");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKeySize_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKeySize");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKeyString_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKeyString");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKeyByteBuffer_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKeyByteBuffer");
        }

        [Test]
        public void NativeBinding_MLMediaFormatKeyByteBufferRelease_Exists()
        {
            AssertThatMethodExists("MLMediaFormatKeyByteBufferRelease");
        }

        [Test]
        public void NativeBinding_MLMediaFormatSetKeyInt32_Exists()
        {
            AssertThatMethodExists("MLMediaFormatSetKeyInt32");
        }

        [Test]
        public void NativeBinding_MLMediaFormatSetKeyInt64_Exists()
        {
            AssertThatMethodExists("MLMediaFormatSetKeyInt64");
        }

        [Test]
        public void NativeBinding_MLMediaFormatSetKeyFloat_Exists()
        {
            AssertThatMethodExists("MLMediaFormatSetKeyFloat");
        }

        [Test]
        public void NativeBinding_MLMediaFormatSetKeySize_Exists()
        {
            AssertThatMethodExists("MLMediaFormatSetKeySize");
        }

        [Test]
        public void NativeBinding_MLMediaFormatSetKeyString_Exists()
        {
            AssertThatMethodExists("MLMediaFormatSetKeyString");
        }

        [Test]
        public void NativeBinding_MLMediaFormatSetKeyByteBuffer_Exists()
        {
            AssertThatMethodExists("MLMediaFormatSetKeyByteBuffer");
        }
    }
}