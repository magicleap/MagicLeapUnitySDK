using System.Reflection;
using NUnit.Framework;

namespace Tests.Runtime.NativeBindings
{
    public class MLMediaPlayerTrackNativeBindingsPlayModeTests : NativeBindingsPlayTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLMedia.Player.Track);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_FreeUnmanagedMemory_Exists()
        {
            AssertThatMethodExists("FreeUnmanagedMemory");
        }
    }
}