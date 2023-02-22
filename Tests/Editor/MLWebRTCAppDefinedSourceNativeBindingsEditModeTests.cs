using System.Reflection;
using NUnit.Framework;

namespace Tests.Editor
{
    public class MLWebRTCAppDefinedSourceNativeBindingsEditModeTests : NativeBindingsTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLWebRTC.AppDefinedSource);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }
    }
}