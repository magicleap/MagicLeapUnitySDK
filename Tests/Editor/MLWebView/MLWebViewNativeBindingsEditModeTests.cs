using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLWebView : NativeBindingsTests
    {
        private MlSdkDllLoader lib;
    
        [OneTimeSetUp]
        public void Init()
        {
            lib = new MlSdkDllLoader();
            lib.Load("webview.magicleap");
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            lib.Free();
        } 
    
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLWebView);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLWebViewCreate_Exists()
        {
            AssertThatMethodExists("MLWebViewCreate");
        }

        [Test]
        public void NativeBinding_MLWebViewDestroy_Exists()
        {
            AssertThatMethodExists("MLWebViewDestroy");
        }

        [Test]
        public void NativeBinding_MLWebViewSetEventCallbacks_Exists()
        {
            AssertThatMethodExists("MLWebViewSetEventCallbacks");
        }

        [Test]
        public void NativeBinding_MLWebViewAcquireNextAvailableFrame_Exists()
        {
            AssertThatMethodExists("MLWebViewAcquireNextAvailableFrame");
        }

        [Test]
        public void NativeBinding_MLWebViewReleaseFrame_Exists()
        {
            AssertThatMethodExists("MLWebViewReleaseFrame");
        }

        [Test]
        public void NativeBinding_MLWebViewGoTo_Exists()
        {
            AssertThatMethodExists("MLWebViewGoTo");
        }

        [Test]
        public void NativeBinding_MLWebViewGoBack_Exists()
        {
            AssertThatMethodExists("MLWebViewGoBack");
        }

        [Test]
        public void NativeBinding_MLWebViewGoForward_Exists()
        {
            AssertThatMethodExists("MLWebViewGoForward");
        }

        [Test]
        public void NativeBinding_MLWebViewReload_Exists()
        {
            AssertThatMethodExists("MLWebViewReload");
        }

        [Test]
        public void NativeBinding_MLWebViewGetUrl_Exists()
        {
            AssertThatMethodExists("MLWebViewGetUrl");
        }

        [Test]
        public void NativeBinding_MLWebViewCanGoBack_Exists()
        {
            AssertThatMethodExists("MLWebViewCanGoBack");
        }

        [Test]
        public void NativeBinding_MLWebViewCanGoForward_Exists()
        {
            AssertThatMethodExists("MLWebViewCanGoForward");
        }

        [Test]
        public void NativeBinding_MLWebViewInjectMouseMove_Exists()
        {
            AssertThatMethodExists("MLWebViewInjectMouseMove");
        }

        [Test]
        public void NativeBinding_MLWebViewInjectMouseButtonDown_Exists()
        {
            AssertThatMethodExists("MLWebViewInjectMouseButtonDown");
        }

        [Test]
        public void NativeBinding_MLWebViewInjectMouseButtonUp_Exists()
        {
            AssertThatMethodExists("MLWebViewInjectMouseButtonUp");
        }

        [Test]
        public void NativeBinding_MLWebViewInjectChar_Exists()
        {
            AssertThatMethodExists("MLWebViewInjectChar");
        }

        [Test]
        public void NativeBinding_MLWebViewInjectKeyDown_Exists()
        {
            AssertThatMethodExists("MLWebViewInjectKeyDown");
        }

        [Test]
        public void NativeBinding_MLWebViewInjectKeyUp_Exists()
        {
            AssertThatMethodExists("MLWebViewInjectKeyUp");
        }

        [Test]
        public void NativeBinding_MLWebViewResetZoom_Exists()
        {
            AssertThatMethodExists("MLWebViewResetZoom");
        }

        [Test]
        public void NativeBinding_MLWebViewZoomIn_Exists()
        {
            AssertThatMethodExists("MLWebViewZoomIn");
        }

        [Test]
        public void NativeBinding_MLWebViewZoomOut_Exists()
        {
            AssertThatMethodExists("MLWebViewZoomOut");
        }

        [Test]
        public void NativeBinding_MLWebViewGetZoomFactor_Exists()
        {
            AssertThatMethodExists("MLWebViewGetZoomFactor");
        }

        [Test]
        public void NativeBinding_MLWebViewScrollBy_Exists()
        {
            AssertThatMethodExists("MLWebViewScrollBy");
        }

        [Test]
        public void NativeBinding_MLWebViewGetScrollSize_Exists()
        {
            AssertThatMethodExists("MLWebViewGetScrollSize");
        }

        [Test]
        public void NativeBinding_MLWebViewGetScrollOffset_Exists()
        {
            AssertThatMethodExists("MLWebViewGetScrollOffset");
        }

        [Test]
        public void NativeBinding_MLWebViewGetFrameTransformMatrix_Exists()
        {
            AssertThatMethodExists("MLWebViewGetFrameTransformMatrix");
        }

        [Test]
        public void NativeBinding_MLWebViewRemoveAllCookies_Exists()
        {
            AssertThatMethodExists("MLWebViewRemoveAllCookies");
        }

        [Test]
        public void NativeBinding_MLWebViewPause_Exists()
        {
            AssertThatMethodExists("MLWebViewPause");
        }

        [Test]
        public void NativeBinding_MLWebViewResume_Exists()
        {
            AssertThatMethodExists("MLWebViewResume");
        }

        [Test]
        public void NativeBinding_MLWebViewClearCache_Exists()
        {
            AssertThatMethodExists("MLWebViewClearCache");
        }
    }
}