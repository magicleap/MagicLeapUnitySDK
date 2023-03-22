using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public class XRStructSizeTests : MagicLeapNativeBindingsStructSizeTests
    {
        [Test]
        public void NativeBindings_MLCoordinateFrameUID_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCoordinateFrameUIDSize();
            var type = FindTypeByName("MLCoordinateFrameUID");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
        
        [Test]
        public void NativeBindings_MLPolygon_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLPolygonSize();
            var type = FindTypeByName("MLPolygon");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
    }
}