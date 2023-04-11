using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public class MLGestureClassification : NativeBindingsTests
    {
        [Test]
        public void NativeBindings_MLGestureClassificationStaticData_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLGestureClassificationStaticDataSize();
            var type = FindTypeByName("MLGestureClassificationStaticData");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLGestureClassificationPostureFingerData_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLGestureClassificationPostureFingerDataSize();
            var type = FindTypeByName("MLGestureClassificationPostureFingerData");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLGestureClassificationFingerState_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLGestureClassificationFingerStateSize();
            var type = FindTypeByName("MLGestureClassificationFingerState");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLGestureClassificationState_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLGestureClassificationStateSize();
            var type = FindTypeByName("MLGestureClassificationState");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLGestureClassificationData_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLGestureClassificationDataSize();
            var type = FindTypeByName("MLGestureClassificationData");

            Assert.IsNull(type);
        }
    }
}