using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLMarkerTracker : NativeBindingsTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLMarkerTracker);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLMarkerTrackerCreate_Exists()
        {
            AssertThatMethodExists("MLMarkerTrackerCreate");
        }

        [Test]
        public void NativeBinding_MLMarkerTrackerDestroy_Exists()
        {
            AssertThatMethodExists("MLMarkerTrackerDestroy");
        }

        [Test]
        public void NativeBinding_MLMarkerTrackerGetResult_Exists()
        {
            AssertThatMethodExists("MLMarkerTrackerGetResult");
        }

        [Test]
        public void NativeBinding_MLMarkerTrackerReleaseResult_Exists()
        {
            AssertThatMethodExists("MLMarkerTrackerReleaseResult");
        }

        [Test]
        public void NativeBinding_MLMarkerTrackerUpdateSettings_Exists()
        {
            AssertThatMethodExists("MLMarkerTrackerUpdateSettings");
        }
    }
}