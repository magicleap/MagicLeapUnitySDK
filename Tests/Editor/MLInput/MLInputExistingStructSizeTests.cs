using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLInput : NativeBindingsTests
    {
        [Test]
        public void NativeBindings_MLInputControllerCallbacksEx_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLInputControllerCallbacksExSize();
            var type = FindTypeByName("MLInputControllerCallbacksEx");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
        
        [Test]
        public void NativeBindings_MLInputControllerStateEx_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLInputControllerStateExSize();
            var type = FindTypeByName("MLInputControllerStateEx");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLInputConnectedDevicesList_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLInputConnectedDevicesListSize();
            var type = FindTypeByName("MLInputConnectedDevicesList");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLInputPreDefinedPattern_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLInputPreDefinedPatternSize();
            var type = FindTypeByName("MLInputPreDefinedPattern");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLInputBuzzCommand_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLInputBuzzCommandSize();
            var type = FindTypeByName("MLInputBuzzCommand");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLInputCustomHaptics_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLInputCustomHapticsSize();
            var type = FindTypeByName("MLInputCustomHaptics");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLInputCustomHapticsInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLInputCustomHapticsInfoSize();
            var type = FindTypeByName("MLInputCustomHapticsInfo");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
    }
}