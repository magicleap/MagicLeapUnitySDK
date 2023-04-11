using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLNativeSurface 
    {
        [Test]
        public void NativeBindings_MLNativeSurfaceFrameAvailableInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLNativeSurfaceFrameAvailableInfoSize();
            var type = FindTypeByName("MLNativeSurfaceFrameAvailableInfo");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLNativeSurfaceConfig_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLNativeSurfaceConfigSize();
            var type = FindTypeByName("MLNativeSurfaceConfig");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLNativeSurfaceOnFrameAvailableCallback_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLNativeSurfaceOnFrameAvailableCallbackSize();
            var type = FindTypeByName("MLNativeSurfaceOnFrameAvailableCallback");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
    }
}