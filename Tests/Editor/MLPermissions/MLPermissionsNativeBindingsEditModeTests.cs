using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLPermissions : NativeBindingsTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLPermissions);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLZIPermissionsStart_Exists()
        {
            AssertThatMethodExists("MLZIPermissionsStart");
        }

        [Test]
        public void NativeBinding_MLZIPermissionsStop_Exists()
        {
            AssertThatMethodExists("MLZIPermissionsStop");
        }

        [Test]
        public void NativeBinding_MLZIPermissionsIsGranted_Exists()
        {
            AssertThatMethodExists("MLZIPermissionsIsGranted");
        }

        [Test]
        public void NativeBinding_MLZIPermissionsRequest_Exists()
        {
            AssertThatMethodExists("MLZIPermissionsRequest");
        }
    }
}