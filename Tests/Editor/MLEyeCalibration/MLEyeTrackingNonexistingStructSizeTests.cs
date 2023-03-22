using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLEyeCalibration
    {
        [Test]
        public void NativeBindings_MLEyeTrackingStaticData_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLEyeTrackingStaticDataSize();
            var type = FindTypeByName("MLEyeTrackingStaticData");

            Assert.IsNull(type);
        }
    }
}