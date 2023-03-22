using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLGraphicsHooks 
    {
        [Test]
        public void NativeBindings_MLGraphicsOptions_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLGraphicsOptionsSize();
            var type = FindTypeByName("MLGraphicsOptions");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLGraphicsClipExtentsInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLGraphicsClipExtentsInfoSize();
            var type = FindTypeByName("MLGraphicsClipExtentsInfo");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLGraphicsClipExtentsParams_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLGraphicsClipExtentsParamsSize();
            var type = FindTypeByName("MLGraphicsClipExtentsParams");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLGraphicsClipExtentsInfoArrayEx_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLGraphicsClipExtentsInfoArrayExSize();
            var type = FindTypeByName("MLGraphicsClipExtentsInfoArrayEx");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLGraphicsRenderTarget_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLGraphicsRenderTargetSize();
            var type = FindTypeByName("MLGraphicsRenderTarget");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLGraphicsRenderBufferInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLGraphicsRenderBufferInfoSize();
            var type = FindTypeByName("MLGraphicsRenderBufferInfo");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLGraphicsRenderTargetsInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLGraphicsRenderTargetsInfoSize();
            var type = FindTypeByName("MLGraphicsRenderTargetsInfo");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLGraphicsVirtualCameraInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLGraphicsVirtualCameraInfoSize();
            var type = FindTypeByName("MLGraphicsVirtualCameraInfo");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLGraphicsFrameInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLGraphicsFrameInfoSize();
            var type = FindTypeByName("MLGraphicsFrameInfo");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLGraphicsClientPerformanceInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLGraphicsClientPerformanceInfoSize();
            var type = FindTypeByName("MLGraphicsClientPerformanceInfo");

            Assert.IsNull(type);
        }
    }
}