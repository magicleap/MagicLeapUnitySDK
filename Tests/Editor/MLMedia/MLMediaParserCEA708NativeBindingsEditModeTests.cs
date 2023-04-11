using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLMedia
    {
        public class ParserCEA708 : NativeBindingsTests
        {
            private MlSdkDllLoader lib;

            [OneTimeSetUp]
            public void Init()
            {
                lib = new MlSdkDllLoader();
                lib.Load("media_cea708parser.magicleap");
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                lib.Free();
            }

            [SetUp]
            public void SetupNativeBindings()
            {
                var apiType = typeof(UnityEngine.XR.MagicLeap.MLMedia.ParserCEA708);
                nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
            }

            [Test]
            public void NativeBinding_MLMediaCea708ParserCreate_Exists()
            {
                AssertThatMethodExists("MLMediaCea708ParserCreate");
            }

            [Test]
            public void NativeBinding_MLMediaCea708ParserDestroy_Exists()
            {
                AssertThatMethodExists("MLMediaCea708ParserDestroy");
            }

            [Test]
            public void NativeBinding_MLMediaCea708ParserSetEmitEventCallback_Exists()
            {
                AssertThatMethodExists("MLMediaCea708ParserSetEmitEventCallback");
            }

            [Test]
            public void NativeBinding_MLMediaCea708ParserParse_Exists()
            {
                AssertThatMethodExists("MLMediaCea708ParserParse");
            }
        }
    }
}