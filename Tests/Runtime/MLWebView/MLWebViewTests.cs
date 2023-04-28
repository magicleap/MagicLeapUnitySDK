using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;
using WebView = UnityEngine.XR.MagicLeap.MLWebView;

namespace UnitySDKPlayTests
{
    public partial class MLWebViewTests
    {
        private const uint Width = 640;
        private const uint Height = 320;
        private const String HomeUrl = "https://www.magicleap.com/en-us/";
        private const String ExampleUrl = "https://www.example.com/";

        WebView webView = null;
        private bool callbackReceived = false;
        private bool loadEnded = false;
        private bool errorLoaded = false;

        // [Test]
        public void MLWebView_CheckPermission()
        {
            Assert.IsTrue(CheckWebViewPermissions());
        }

        [UnityTest]
        public IEnumerator MLWebView_Create()
        {
            yield return SetUp();
            Assert.NotNull(webView);
            yield return TearDown();
        }

        [UnityTest]
        public IEnumerator MLWebView_Destroy()
        {
            yield return SetUp();
            callbackReceived = false;
            MLResult result = webView.Destroy();
            yield return TearDownYieldCallback(result);
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_GoTo()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.GoTo(HomeUrl);
            yield return new WaitUntil(() => callbackReceived && (loadEnded || errorLoaded));
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_Reload()
        {
            yield return SetUp();
            ResetFlags();
            webView.GoTo(HomeUrl);
            yield return new WaitUntil(() => callbackReceived && (loadEnded || errorLoaded));
            ResetFlags();
            var result = webView.Reload();
            yield return new WaitUntil(() => callbackReceived && (loadEnded || errorLoaded));
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_GetURL()
        {
            yield return SetUp();
            ResetFlags();
            webView.GoTo(HomeUrl);
            yield return new WaitUntil(() => callbackReceived && (loadEnded || errorLoaded));
            ResetFlags();
            var result = webView.GetURL();
            yield return TearDown();
            Assert.That(result == HomeUrl, result);
        }

        // [UnityTest]
        public IEnumerator MLWebView_CanGoBackPositive()
        {
            yield return SetUp();
            ResetFlags();
            webView.GoTo(ExampleUrl);
            yield return new WaitUntil(() => callbackReceived && (loadEnded || errorLoaded));
            ResetFlags();
            webView.GoTo(HomeUrl);
            yield return new WaitUntil(() => callbackReceived && (loadEnded || errorLoaded));
            ResetFlags();
            var result = webView.CanGoBack();
            yield return TearDown();
            Assert.IsTrue(result);
        }

        // [UnityTest]
        public IEnumerator MLWebView_CanGoBackNegative()
        {
            yield return SetUp();
            var result = webView.CanGoBack();
            yield return TearDown();
            Assert.IsFalse(result);
        }

        // [UnityTest]
        public IEnumerator MLWebView_GoBackPositive()
        {
            yield return SetUp();
            ResetFlags();
            webView.GoTo(ExampleUrl);
            yield return new WaitUntil(() => callbackReceived && (loadEnded || errorLoaded));
            ResetFlags();
            webView.GoTo(HomeUrl);
            yield return new WaitUntil(() => callbackReceived && (loadEnded || errorLoaded));
            ResetFlags();
            var result = webView.GoBack();
            yield return new WaitUntil(() => callbackReceived && (loadEnded || errorLoaded));
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_GoBackNegative()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.GoBack();
            yield return new WaitUntil(() => (callbackReceived && (loadEnded || errorLoaded)) || !webView.CanGoBack());
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_CanGoForwardPositive()
        {
            yield return SetUp();
            ResetFlags();
            webView.GoTo(ExampleUrl);
            yield return new WaitUntil(() => callbackReceived && (loadEnded || errorLoaded));
            ResetFlags();
            webView.GoTo(HomeUrl);
            yield return new WaitUntil(() => callbackReceived && (loadEnded || errorLoaded));
            ResetFlags();
            webView.GoBack();
            yield return new WaitUntil(() => callbackReceived && (loadEnded || errorLoaded));
            ResetFlags();
            var result = webView.CanGoForward();
            yield return TearDown();
            Assert.IsTrue(result);
        }

        // [UnityTest]
        public IEnumerator MLWebView_CanGoForwardNegative()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.CanGoForward();
            yield return TearDown();
            Assert.IsFalse(result);
        }

        // [UnityTest]
        public IEnumerator MLWebView_GoForwardPositive()
        {
            yield return SetUp();
            ResetFlags();
            webView.GoTo(ExampleUrl);
            yield return new WaitUntil(() => callbackReceived && (loadEnded || errorLoaded));
            ResetFlags();
            webView.GoTo(HomeUrl);
            yield return new WaitUntil(() => callbackReceived && (loadEnded || errorLoaded));
            ResetFlags();
            webView.GoBack();
            yield return new WaitUntil(() => callbackReceived && (loadEnded || errorLoaded));
            ResetFlags();
            var result = webView.GoForward();
            yield return new WaitUntil(() => callbackReceived && (loadEnded || errorLoaded));
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_GoForwardNegative()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.GoForward();
            yield return new WaitUntil(() => (callbackReceived && (loadEnded || errorLoaded)) || !webView.CanGoForward());
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_GetZoomFactor()
        {
            yield return SetUp();
            ResetFlags();
            try
            {
                var result = webView.GetZoomFactor();
                Assert.IsNotNull(result);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            yield return TearDown();
        }

        // [UnityTest]
        public IEnumerator MLWebView_ClearCache()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.ClearCache();
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_RemoveAllCookies()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.RemoveAllCookies();
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_PauseDiscard()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.Pause(WebView.PauseType.Discard);
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_PauseMultiMedia()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.Pause(WebView.PauseType.MultiMedia);
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_PauseTimers()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.Pause(WebView.PauseType.Timers);
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_ResumeDiscard()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.Pause(WebView.PauseType.Discard);
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            result = webView.Resume();
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_ResumeMultiMedia()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.Pause(WebView.PauseType.MultiMedia);
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            result = webView.Resume();
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_ResumeTimers()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.Pause(WebView.PauseType.Timers);
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            result = webView.Resume();
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_ZoomIn()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.ZoomIn();
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_InjectMouseMove()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.InjectMouseMove(0u, 0u, WebView.EventFlags.None);
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_InjectMouseButtonDown_LeftMouseButton()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.InjectMouseButtonDown(0u, 0u, WebView.EventFlags.LeftMouseButton);
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_InjectMouseButtonUp_LeftMouseButton()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.InjectMouseButtonUp(0u, 0u, WebView.EventFlags.LeftMouseButton);
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_InjectMouseButtonDown_MiddleMouseButton()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.InjectMouseButtonDown(0u, 0u, WebView.EventFlags.MiddleMouseButton);
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_InjectMouseButtonUp_MiddleMouseButton()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.InjectMouseButtonUp(0u, 0u, WebView.EventFlags.MiddleMouseButton);
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_InjectMouseButtonDown_RightMouseButton()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.InjectMouseButtonDown(0u, 0u, WebView.EventFlags.RightMouseButton);
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_InjectMouseButtonUp_RightMouseButton()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.InjectMouseButtonUp(0u, 0u, WebView.EventFlags.RightMouseButton);
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_InjectChar_w()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.InjectChar('w');
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_InjectKeyDown_Back()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.InjectKeyDown(WebView.KeyCode.Back, (uint)WebView.EventFlags.None);
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_InjectKeyUp_Back()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.InjectKeyUp(WebView.KeyCode.Back, (uint)WebView.EventFlags.None);
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_GetScrollSize()
        {
            yield return SetUp();
            ResetFlags();
            try
            {
                var result = webView.GetScrollSize();
                Assert.IsNotNull(result);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            yield return TearDown();
        }

        // [UnityTest]
        public IEnumerator MLWebView_GetScrollOffset()
        {
            yield return SetUp();
            ResetFlags();
            try
            {
                var result = webView.GetScrollOffset();
                Assert.IsNotNull(result);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            yield return TearDown();
        }

        // [UnityTest]
        public IEnumerator MLWebView_ZoomOut()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.ZoomOut();
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_ResetZoomNoChange()
        {
            yield return SetUp();
            ResetFlags();
            var result = webView.ResetZoom();
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_ResetZoomIn()
        {
            yield return SetUp();
            ResetFlags();
            webView.ZoomIn();
            var result = webView.ResetZoom();
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        // [UnityTest]
        public IEnumerator MLWebView_ResetZoomOut()
        {
            yield return SetUp();
            ResetFlags();
            webView.ZoomOut();
            var result = webView.ResetZoom();
            yield return new WaitUntil(() => result.IsOk || callbackReceived);
            yield return TearDown();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        private IEnumerator SetUp()
        {
            CheckWebViewPermissions();
            webView = WebView.Create(Width, Height);
            RegisterCallbacks();
            yield return new WaitUntil(() => callbackReceived);
        }

        private IEnumerator TearDown()
        {
            ResetFlags();
            MLResult result = webView.Destroy();
            yield return TearDownYieldCallback(result);
        }

        private IEnumerator TearDownYieldCallback(MLResult result)
        {
            yield return new WaitUntil(() => callbackReceived || result.IsOk);
            UnregisterCallbacks();
            webView = null;
        }

        private void RegisterCallbacks()
        {
            webView.OnBeforeResourceLoaded += OnBeforeResourceLoaded;
            webView.OnCertificateErrorLoaded += OnCertificateErrorLoaded;
            webView.OnErrorLoaded += OnErrorLoaded;
            webView.OnKeyboardDismissed += OnKeyboardDismissed;
            webView.OnKeyboardShown += OnKeyboardShown;
            webView.OnLoadEnded += OnLoadEnded;
            webView.OnServiceConnected += OnServiceConnected;
            webView.OnServiceDisconnected += OnServiceDisconnected;
            webView.OnServiceFailed += OnServiceFailed;
            webView.OnWebViewDestroyed += OnWebViewDestroyed;
        }

        private void UnregisterCallbacks()
        {
            webView.OnBeforeResourceLoaded -= OnBeforeResourceLoaded;
            webView.OnCertificateErrorLoaded -= OnCertificateErrorLoaded;
            webView.OnErrorLoaded -= OnErrorLoaded;
            webView.OnKeyboardDismissed -= OnKeyboardDismissed;
            webView.OnKeyboardShown -= OnKeyboardShown;
            webView.OnLoadEnded -= OnLoadEnded;
            webView.OnServiceConnected -= OnServiceConnected;
            webView.OnServiceDisconnected -= OnServiceDisconnected;
            webView.OnServiceFailed -= OnServiceFailed;
            webView.OnWebViewDestroyed -= OnWebViewDestroyed;
        }

        private void ResetFlags()
        {
            callbackReceived = false;
            loadEnded = false;
            errorLoaded = false;
        }

        private void OnBeforeResourceLoaded(WebView webView, string resourceURL)
        {
            Debug.Log("OnBeforeResourceLoaded");
            callbackReceived = true;
        }

        void OnCertificateErrorLoaded(WebView webView, int errorCode, string url, string errorMessage, string details, bool certificateErrorIgnored)
        {
            Assert.Fail("OnCertificateErrorLoaded");
            callbackReceived = true;
        }

        void OnErrorLoaded(WebView webView, bool isMainFrame, int httpStatusCode, string errorStr, string failedUrl)
        {
            Assert.Fail($"OnErrorLoaded: {errorStr} for URL: {failedUrl}");
            callbackReceived = true;
            errorLoaded = true;
        }

        void OnKeyboardDismissed(WebView webView)
        {
            Debug.Log("OnKeyboardDismissed");
            callbackReceived = true;
        }

        void OnKeyboardShown(WebView webView, WebView.InputFieldData keyboardShowData)
        {
            Debug.Log("OnKeyboardShown");
            callbackReceived = true;
        }

        void OnLoadEnded(WebView webView, bool isMainFrame, int httpStatusCode)
        {
            Debug.Log("OnLoadEnded");
            callbackReceived = true;
            loadEnded = true;
        }

        void OnServiceConnected(WebView webView)
        {
            Debug.Log("OnServiceConnected");
            callbackReceived = true;
        }

        void OnServiceDisconnected(WebView webView)
        {
            Debug.Log("OnServiceDisconnected");
            callbackReceived = true;
        }

        void OnServiceFailed(WebView webView, MLResult result)
        {
            Assert.Fail("OnServiceFailed");
            callbackReceived = true;
        }

        void OnWebViewDestroyed(WebView webView)
        {
            Debug.Log("OnWebViewDestroyed");
            callbackReceived = true;
        }

        private static bool CheckWebViewPermissions()
        {
            MLPermissions.RequestPermission(MLPermission.WebView, new MLPermissions.Callbacks());

            return MLPermissions.CheckPermission(MLPermission.WebView).IsOk;
        }
    }
}
