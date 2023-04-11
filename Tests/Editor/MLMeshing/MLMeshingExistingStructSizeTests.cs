using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLMeshing
    {
        [Test]
        public void NativeBindings_MLMeshingSettings_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMeshingSettingsSize();
            var type = FindTypeByName("MLMeshingSettings");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLMeshingExtents_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMeshingExtentsSize();
            var type = FindTypeByName("MLMeshingExtents");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLMeshingBlockInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMeshingBlockInfoSize();
            var type = FindTypeByName("MLMeshingBlockInfo");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLMeshingMeshInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMeshingMeshInfoSize();
            var type = FindTypeByName("MLMeshingMeshInfo");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLMeshingBlockRequest_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMeshingBlockRequestSize();
            var type = FindTypeByName("MLMeshingBlockRequest");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLMeshingMeshRequest_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMeshingMeshRequestSize();
            var type = FindTypeByName("MLMeshingMeshRequest");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
    }
}