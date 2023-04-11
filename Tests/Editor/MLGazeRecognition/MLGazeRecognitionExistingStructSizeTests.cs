using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLGazeRecognition : NativeBindingsTests
    {
        [Test]
        public void NativeBindings_MLGazeRecognitionStaticData_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLGazeRecognitionStaticDataSize();
            var type = FindTypeByName("MLGazeRecognitionStaticData");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLGazeRecognitionState_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLGazeRecognitionStateSize();
            var type = FindTypeByName("MLGazeRecognitionState");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
    }
}