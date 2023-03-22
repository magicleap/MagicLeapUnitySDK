using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLHandTracking : NativeBindingsTests
    {
        [Test]
        public void NativeBindings_MLHandTrackingStaticData_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLHandTrackingStaticDataSize();
            var type = FindTypeByName("MLHandTrackingStaticData");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLHandTrackingHandState_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLHandTrackingHandStateSize();
            var type = FindTypeByName("MLHandTrackingHandState");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLHandTrackingData_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLHandTrackingDataSize();
            var type = FindTypeByName("MLHandTrackingData");

            Assert.IsNull(type);
        }
        
        [Test]
        public void NativeBindings_MLHandTrackingCFUIDs_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLHandTrackingCFUIDsSize();
            var type = FindTypeByName("MLHandTrackingCFUIDs");

            Assert.IsNull(type);
        }
    }
}