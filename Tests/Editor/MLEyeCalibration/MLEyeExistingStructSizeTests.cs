using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLEyeCalibration
    {
        [Test]
        public void NativeBindings_MLEyeCalibrationState_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLEyeCalibrationStateSize();
            var type = FindTypeByName("MLEyeCalibrationState");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
        
        [Test]
        public void NativeBindings_MLEyeTrackingStateEx_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLEyeTrackingStateExSize();
            var type = FindTypeByName("MLEyeTrackingStateEx");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
    }
}