using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public class MLController : NativeBindingsTests
    {
        [Test]
        public void NativeBindings_MLControllerConfiguration_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLControllerConfigurationSize();
            var type = FindTypeByName("MLControllerConfiguration");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLControllerStateEx_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLControllerStateExSize();
            var type = FindTypeByName("MLControllerStateEx");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLControllerSystemStateEx_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLControllerSystemStateExSize();
            var type = FindTypeByName("MLControllerSystemStateEx");

            Assert.IsNull(type);
        }

    }
}