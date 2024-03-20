using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLWorldCamera : NativeBindingsTests
    {
        [Test]
        public void NativeBindings_MLWorldCameraSettings_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLWorldCameraSettingsSize();
            var type = FindTypeByName("MLWorldCameraSettings");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLWorldCameraIntrinsics_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLWorldCameraIntrinsicsSize();
            var type = FindTypeByName("MLWorldCameraIntrinsics");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLWorldCameraFrameBuffer_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLWorldCameraFrameBufferSize();
            var type = FindTypeByName("MLWorldCameraFrameBuffer");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLWorldCameraFrame_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLWorldCameraFrameSize();
            var type = FindTypeByName("MLWorldCameraFrame");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLWorldCameraData_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLWorldCameraDataSize();
            var type = FindTypeByName("MLWorldCameraData");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
    }
}
