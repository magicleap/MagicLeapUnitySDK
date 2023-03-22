using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLHeadsetFit : NativeBindingsTests
    {
        [Test]
        public void NativeBindings_MLHeadsetFitState_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLHeadsetFitStateSize();
            var type = FindTypeByName("MLHeadsetFitState");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
    }
}