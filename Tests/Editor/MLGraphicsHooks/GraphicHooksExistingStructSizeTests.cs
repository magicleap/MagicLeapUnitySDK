using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLGraphicsHooks
    {
        [Test]
        public void NativeBindings_MLGraphicsFrameParamsEx_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLGraphicsFrameParamsExSize();
            var type = FindTypeByName("MLGraphicsFrameParamsEx");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
    }
}