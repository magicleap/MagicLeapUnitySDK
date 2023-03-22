using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLInput 
    {
        [Test]
        public void NativeBindings_MLInputControllerTouchpadGesture_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLInputControllerTouchpadGestureSize();
            var type = FindTypeByName("MLInputControllerTouchpadGesture");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLInputControllerState_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLInputControllerStateSize();
            var type = FindTypeByName("MLInputControllerState");

            Assert.IsNull(type);
        }
    }
}