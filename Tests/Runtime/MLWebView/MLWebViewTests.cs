// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
// Disabling deprecated warning for the internal project
#pragma warning disable 618

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
        private const string HomeUrl = "https://www.magicleap.com/en-us/";
        private const string ExampleUrl = "https://www.example.com/";

        WebView webView = null;
        private bool callbackReceived = false;

        // [Test]
        public void MLWebView_CheckPermission()
        {
            Assert.IsTrue(CheckWebViewPermissions());
        }

        [Test]
        public void MLWebView_Create()
        {
            Assert.NotNull(webView);
        }

        [Test]
        public void MLWebView_Destroy()
        {
            callbackReceived = false;
            MLResult result = webView.Destroy();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_GoTo()
        {
            ResetFlags();
            var result = webView.GoTo(HomeUrl);
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_Reload()
        {
            ResetFlags();
            webView.GoTo(HomeUrl);
            ResetFlags();
            var result = webView.Reload();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_GetURL()
        {
            ResetFlags();
            webView.GoTo(HomeUrl);
            ResetFlags();
            var result = webView.GetURL();
            Assert.That(result == HomeUrl, result);
        }

        [Test]
        public void MLWebView_CanGoBackPositive()
        {
            ResetFlags();
            webView.GoTo(ExampleUrl);
            ResetFlags();
            webView.GoTo(HomeUrl);
            ResetFlags();
            var result = webView.CanGoBack();
            Assert.IsTrue(result);
        }

        [Test]
        public void MLWebView_CanGoBackNegative()
        {
            var result = webView.CanGoBack();
            Assert.IsFalse(result);
        }

        [Test]
        public void MLWebView_GoBackPositive()
        {
            ResetFlags();
            webView.GoTo(ExampleUrl);
            ResetFlags();
            webView.GoTo(HomeUrl);
            ResetFlags();
            var result = webView.GoBack();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_GoBackNegative()
        {
            ResetFlags();
            var result = webView.GoBack();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_CanGoForwardPositive()
        {
            ResetFlags();
            webView.GoTo(ExampleUrl);
            ResetFlags();
            webView.GoTo(HomeUrl);
            ResetFlags();
            webView.GoBack();
            ResetFlags();
            var result = webView.CanGoForward();
            Assert.IsTrue(result);
        }

        [Test]
        public void MLWebView_CanGoForwardNegative()
        {
            ResetFlags();
            var result = webView.CanGoForward();
            Assert.IsFalse(result);
        }

        [Test]
        public void MLWebView_GoForwardPositive()
        {
            ResetFlags();
            webView.GoTo(ExampleUrl);
            ResetFlags();
            webView.GoTo(HomeUrl);
            ResetFlags();
            webView.GoBack();
            ResetFlags();
            var result = webView.GoForward();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_GoForwardNegative()
        {
            ResetFlags();
            var result = webView.GoForward();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_GetZoomFactor()
        {
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
        }

        [Test]
        public void MLWebView_ClearCache()
        {
            ResetFlags();
            var result = webView.ClearCache();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_RemoveAllCookies()
        {
            ResetFlags();
            var result = webView.RemoveAllCookies();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_PauseDiscard()
        {
            ResetFlags();
            var result = webView.Pause(WebView.PauseType.Discard);
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_PauseMultiMedia()
        {
            ResetFlags();
            var result = webView.Pause(WebView.PauseType.MultiMedia);
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_PauseTimers()
        {
            ResetFlags();
            var result = webView.Pause(WebView.PauseType.Timers);
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_ResumeDiscard()
        {
            ResetFlags();
            var result = webView.Pause(WebView.PauseType.Discard);
            result = webView.Resume();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_ResumeMultiMedia()
        {
            ResetFlags();
            var result = webView.Pause(WebView.PauseType.MultiMedia);
            result = webView.Resume();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_ResumeTimers()
        {
            ResetFlags();
            var result = webView.Pause(WebView.PauseType.Timers);
            result = webView.Resume();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_ZoomIn()
        {
            ResetFlags();
            var result = webView.ZoomIn();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_InjectMouseMove()
        {
            ResetFlags();
            var result = webView.InjectMouseMove(0u, 0u, WebView.EventFlags.None);
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_InjectMouseButtonDown_LeftMouseButton()
        {
            ResetFlags();
            var result = webView.InjectMouseButtonDown(0u, 0u, WebView.EventFlags.LeftMouseButton);
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_InjectMouseButtonUp_LeftMouseButton()
        {
            ResetFlags();
            var result = webView.InjectMouseButtonUp(0u, 0u, WebView.EventFlags.LeftMouseButton);
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_InjectMouseButtonDown_MiddleMouseButton()
        {
            ResetFlags();
            var result = webView.InjectMouseButtonDown(0u, 0u, WebView.EventFlags.MiddleMouseButton);
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_InjectMouseButtonUp_MiddleMouseButton()
        {
            ResetFlags();
            var result = webView.InjectMouseButtonUp(0u, 0u, WebView.EventFlags.MiddleMouseButton);
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_InjectMouseButtonDown_RightMouseButton()
        {
            ResetFlags();
            var result = webView.InjectMouseButtonDown(0u, 0u, WebView.EventFlags.RightMouseButton);
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_InjectMouseButtonUp_RightMouseButton()
        {
            ResetFlags();
            var result = webView.InjectMouseButtonUp(0u, 0u, WebView.EventFlags.RightMouseButton);
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_InjectChar_w()
        {
            ResetFlags();
            var result = webView.InjectChar('w');
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_InjectKeyDown_Back()
        {
            ResetFlags();
            var result = webView.InjectKeyDown(WebView.KeyCode.Back, (uint)WebView.EventFlags.None);
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_InjectKeyUp_Back()
        {
            ResetFlags();
            var result = webView.InjectKeyUp(WebView.KeyCode.Back, (uint)WebView.EventFlags.None);
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_ZoomOut()
        {
            ResetFlags();
            var result = webView.ZoomOut();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_ResetZoomNoChange()
        {
            ResetFlags();
            var result = webView.ResetZoom();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_ResetZoomIn()
        {
            ResetFlags();
            webView.ZoomIn();
            var result = webView.ResetZoom();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLWebView_ResetZoomOut()
        {
            ResetFlags();
            webView.ZoomOut();
            var result = webView.ResetZoom();
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [UnitySetUp]
        private void SetUp()
        {
            //CheckWebViewPermissions();
            webView = WebView.Create(Width, Height);
            RegisterCallbacks();
            Assert.IsNotNull(webView);
            // yield return new WaitUntil(() => callbackReceived);
        }

        [UnityTearDown]
        private void TearDown()
        {
            ResetFlags();
            MLResult result = webView.Destroy();
            Assert.That(result.IsOk, result.Result.ToString());
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
