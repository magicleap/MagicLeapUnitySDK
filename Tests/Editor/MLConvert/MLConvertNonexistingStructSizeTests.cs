using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLConvert
    {
        [Test]
        public void NativeBindings_MLTransformDerivatives_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLTransformDerivativesSize();
            var type = FindTypeByName("MLTransformDerivatives");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLXYZf_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLXYZfSize();
            var type = FindTypeByName("MLXYZf");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLColor32_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLColor32Size();
            var type = FindTypeByName("MLColor32");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLColor4f_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLColor4fSize();
            var type = FindTypeByName("MLColor4f");

            Assert.IsNull(type);
        }
        
        [Test]
        public void NativeBindings_MLVersion_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLVersionSize();
            var type = FindTypeByName("MLVersion");

            Assert.IsNull(type);
        }
    }
}