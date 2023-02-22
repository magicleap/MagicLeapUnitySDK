using System.Reflection;
using NUnit.Framework;

namespace Tests.Runtime.NativeBindings
{
    public class MLMediaPlayerNativeBindingsPlayModeTests : NativeBindingsPlayTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLMedia.Player);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLUnityQueueMediaPlayerResetAndDestroy_Exists()
        {
            AssertThatMethodExists("MLUnityQueueMediaPlayerResetAndDestroy");
        }
    }
}