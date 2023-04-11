using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLNativeSurface : NativeBindingsTests
    {
        private MlSdkDllLoader lib;
    
        [OneTimeSetUp]
        public void Init()
        {
            lib = new MlSdkDllLoader();
            lib.Load("native_surface.magicleap");
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            lib.Free();
        } 
    
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLNativeSurface);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLNativeSurfaceCreate_Exists()
        {
            AssertThatMethodExists("MLNativeSurfaceCreate");
        }

        [Test]
        public void NativeBinding_MLNativeSurfaceRelease_Exists()
        {
            AssertThatMethodExists("MLNativeSurfaceRelease");
        }

        [Test]
        public void NativeBinding_MLNativeSurfaceAcquireNextAvailableFrame_Exists()
        {
            AssertThatMethodExists("MLNativeSurfaceAcquireNextAvailableFrame");
        }

        [Test]
        public void NativeBinding_MLNativeSurfaceAcquireHardwareBufferFromNativeBuffer_Exists()
        {
            AssertThatMethodExists("MLNativeSurfaceAcquireHardwareBufferFromNativeBuffer");
        }

        [Test]
        public void NativeBinding_MLNativeSurfaceReleaseFrame_Exists()
        {
            AssertThatMethodExists("MLNativeSurfaceReleaseFrame");
        }

        [Test]
        public void NativeBinding_MLNativeSurfaceGetFrameTransformationMatrix_Exists()
        {
            AssertThatMethodExists("MLNativeSurfaceGetFrameTransformationMatrix");
        }

        [Test]
        public void NativeBinding_MLNativeSurfaceGetFrameTimestamp_Exists()
        {
            AssertThatMethodExists("MLNativeSurfaceGetFrameTimestamp");
        }

        [Test]
        public void NativeBinding_MLNativeSurfaceGetFrameQueueBufferTimestamp_Exists()
        {
            AssertThatMethodExists("MLNativeSurfaceGetFrameQueueBufferTimestamp");
        }

        [Test]
        public void NativeBinding_MLNativeSurfaceGetFrameNumber_Exists()
        {
            AssertThatMethodExists("MLNativeSurfaceGetFrameNumber");
        }

        [Test]
        public void NativeBinding_MLNativeSurfaceGetFrameCropRect_Exists()
        {
            AssertThatMethodExists("MLNativeSurfaceGetFrameCropRect");
        }

        [Test]
        public void NativeBinding_MLNativeSurfaceSetOnFrameAvailableCallback_Exists()
        {
            AssertThatMethodExists("MLNativeSurfaceSetOnFrameAvailableCallback");
        }
    }
}