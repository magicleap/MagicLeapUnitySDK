using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLAnchors : NativeBindingsTests
    {
        private MlSdkDllLoader lib;
    
        [OneTimeSetUp]
        public void Init()
        {
            lib = new MlSdkDllLoader();
            lib.Load("perception.magicleap");
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            lib.Free();
        } 
    
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLAnchors);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.Public);
        }

        [Test]
        public void NativeBinding_MLSpatialAnchorCreate_Exists()
        {
            AssertThatMethodExists("MLSpatialAnchorCreate");
        }

        [Test]
        public void NativeBinding_MLSpatialAnchorTrackerCreate_Exists()
        {
            AssertThatMethodExists("MLSpatialAnchorTrackerCreate");
        }

        [Test]
        public void NativeBinding_MLSpatialAnchorTrackerDestroy_Exists()
        {
            AssertThatMethodExists("MLSpatialAnchorTrackerDestroy");
        }

        [Test]
        public void NativeBinding_MLSpatialAnchorPublish_Exists()
        {
            AssertThatMethodExists("MLSpatialAnchorPublish");
        }

        [Test]
        public void NativeBinding_MLSpatialAnchorDelete_Exists()
        {
            AssertThatMethodExists("MLSpatialAnchorDelete");
        }

        [Test]
        public void NativeBinding_MLSpatialAnchorUpdate_Exists()
        {
            AssertThatMethodExists("MLSpatialAnchorUpdate");
        }

        [Test]
        public void NativeBinding_MLSpatialAnchorQueryCreate_Exists()
        {
            AssertThatMethodExists("MLSpatialAnchorQueryCreate");
        }

        [Test]
        public void NativeBinding_MLSpatialAnchorQueryDestroy_Exists()
        {
            AssertThatMethodExists("MLSpatialAnchorQueryDestroy");
        }

        [Test]
        public void NativeBinding_MLSpatialAnchorQueryGetResult_Exists()
        {
            AssertThatMethodExists("MLSpatialAnchorQueryGetResult");
        }

        [Test]
        public void NativeBinding_MLSpatialAnchorGetLocalizationInfo_Exists()
        {
            AssertThatMethodExists("MLSpatialAnchorGetLocalizationInfo");
        }
    }
}