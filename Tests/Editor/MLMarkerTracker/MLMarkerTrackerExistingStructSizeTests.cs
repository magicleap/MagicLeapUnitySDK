using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLMarkerTracker : NativeBindingsTests
    {
        [Test]
        public void NativeBindings_MLMarkerTrackerDecodedTypedData_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMarkerTrackerDecodedTypedDataSize();
            var type = FindTypeByName("MLMarkerTrackerDecodedTypedData");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLMarkerTrackerDecodedArucoData_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMarkerTrackerDecodedArucoDataSize();
            var type = FindTypeByName("MLMarkerTrackerDecodedArucoData");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLMarkerTrackerDecodedBinaryData_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMarkerTrackerDecodedBinaryDataSize();
            var type = FindTypeByName("MLMarkerTrackerDecodedBinaryData");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLMarkerTrackerDecodedData_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMarkerTrackerDecodedDataSize();
            var type = FindTypeByName("MLMarkerTrackerDecodedData");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLMarkerTrackerResult_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMarkerTrackerResultSize();
            var type = FindTypeByName("MLMarkerTrackerResult");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLMarkerTrackerResultArray_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMarkerTrackerResultArraySize();
            var type = FindTypeByName("MLMarkerTrackerResultArray");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLMarkerTrackerCustomProfile_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMarkerTrackerCustomProfileSize();
            var type = FindTypeByName("MLMarkerTrackerCustomProfile");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLMarkerTrackerSettings_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMarkerTrackerSettingsSize();
            var type = FindTypeByName("MLMarkerTrackerSettings");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
    }
}