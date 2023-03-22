using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLMediaFormatKey : NativeBindingsTests
    {
        [Test]
        public void NativeBindings_MLMediaFormatByteArray_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMediaFormatByteArraySize();
            var type = FindTypeByName("MLMediaFormatByteArray");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLMediaMuxerSampleData_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMediaMuxerSampleDataSize();
            var type = FindTypeByName("MLMediaMuxerSampleData");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLMediaRecorderOnInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMediaRecorderOnInfoSize();
            var type = FindTypeByName("MLMediaRecorderOnInfo");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLMediaRecorderOnError_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMediaRecorderOnErrorSize();
            var type = FindTypeByName("MLMediaRecorderOnError");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLMediaRecorderEventCallbacks_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMediaRecorderEventCallbacksSize();
            var type = FindTypeByName("MLMediaRecorderEventCallbacks");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLTTMLImage_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLTTMLImageSize();
            var type = FindTypeByName("MLTTMLImage");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLTTMLData_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLTTMLDataSize();
            var type = FindTypeByName("MLTTMLData");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
    }
}