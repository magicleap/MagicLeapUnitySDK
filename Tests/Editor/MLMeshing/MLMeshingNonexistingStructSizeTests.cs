using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLMeshing : NativeBindingsTests
    {
        [Test]
        public void NativeBindings_MLMeshingBlockMesh_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMeshingBlockMeshSize();
            var type = FindTypeByName("MLMeshingBlockMesh");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLMeshingMesh_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMeshingMeshSize();
            var type = FindTypeByName("MLMeshingMesh");

            Assert.IsNull(type);
        }
    }
}