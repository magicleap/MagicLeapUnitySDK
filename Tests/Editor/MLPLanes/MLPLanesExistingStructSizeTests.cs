using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLPLanes : NativeBindingsTests
    {
        [Test]
        public void NativeBindings_MLPlane_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLPlaneSize();
            var type = FindTypeByName("MLPlane");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
        
        [Test]
        public void NativeBindings_MLPlaneBoundary_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLPlaneBoundarySize();
            var type = FindTypeByName("MLPlaneBoundary");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLPlaneBoundaries_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLPlaneBoundariesSize();
            var type = FindTypeByName("MLPlaneBoundaries");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLPlaneBoundariesList_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLPlaneBoundariesListSize();
            var type = FindTypeByName("MLPlaneBoundariesList");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLPlanesQuery_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLPlanesQuerySize();
            var type = FindTypeByName("MLPlanesQuery");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
    }
}