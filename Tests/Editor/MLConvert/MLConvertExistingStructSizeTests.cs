using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLConvert : NativeBindingsTests
    {
        [Test]
        public void NativeBindings_Vec3f_Struct_Size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLVec3fSize();
            var type = FindTypeByName("MLVec3f");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
        
        [Test]
        public void NativeBindings_MLVec2f_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLVec2fSize();
            var type = FindTypeByName("MLVec2f");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
        
        [Test]
        public void NativeBindings_MLQuaternionf_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLQuaternionfSize();
            var type = FindTypeByName("MLQuaternionf");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLTransform_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLTransformSize();
            var type = FindTypeByName("MLTransform");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLMat4f_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMat4fSize();
            var type = FindTypeByName("MLMat4f");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLRectf_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLRectfSize();
            var type = FindTypeByName("MLRectf");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLRecti_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLRectiSize();
            var type = FindTypeByName("MLRecti");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLUUID_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLUUIDSize();
            var type = FindTypeByName("MLUUID");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
    }
}