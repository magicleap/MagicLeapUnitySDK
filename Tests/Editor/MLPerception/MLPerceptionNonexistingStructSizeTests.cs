using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLPerception : NativeBindingsTests
    {
        [Test]
        public void NativeBindings_MLPerceptionSettings_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLPerceptionSettingsSize();
            var type = FindTypeByName("MLPerceptionSettings");

            Assert.IsNull(type);
        }
    }
}