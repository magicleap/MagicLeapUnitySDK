using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLDepthCamera : NativeBindingsTests
    {
        [Test]
        public void NativeBindings_MLDepthCameraSettings_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLDepthCameraSettingsSize();
            var type = FindTypeByName("MLDepthCameraSettings");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLDepthCameraIntrinsics_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLDepthCameraIntrinsicsSize();
            var type = FindTypeByName("MLDepthCameraIntrinsics");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLDepthCameraFrameBuffer_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLDepthCameraFrameBufferSize();
            var type = FindTypeByName("MLDepthCameraFrameBuffer");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLDepthCameraData_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLDepthCameraDataSize();
            var type = FindTypeByName("MLDepthCameraData");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
    }
}