using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public class MLHeadTracking : MagicLeapNativeBindingsStructSizeTests
    {
        [Test]
        public void NativeBindings_MLHeadTrackingStaticData_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLHeadTrackingStaticDataSize();
            var type = FindTypeByName("MLHeadTrackingStaticData");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLHeadTrackingState_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLHeadTrackingStateSize();
            var type = FindTypeByName("MLHeadTrackingState");

            Assert.IsNull(type);
        }
    }
}