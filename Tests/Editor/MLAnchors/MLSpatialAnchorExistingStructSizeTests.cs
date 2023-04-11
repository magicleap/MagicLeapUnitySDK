using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLAnchors
    {
        [Test]
        public void NativeBindings_MLSpatialAnchorLocalizationInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLSpatialAnchorLocalizationInfoSize();
            var type = FindTypeByName("MLSpatialAnchorLocalizationInfo");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLSpatialAnchor_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLSpatialAnchorSize();
            var type = FindTypeByName("MLSpatialAnchor");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLSpatialAnchorCreateInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLSpatialAnchorCreateInfoSize();
            var type = FindTypeByName("MLSpatialAnchorCreateInfo");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLSpatialAnchorQueryFilter_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLSpatialAnchorQueryFilterSize();
            var type = FindTypeByName("MLSpatialAnchorQueryFilter");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
    }
}