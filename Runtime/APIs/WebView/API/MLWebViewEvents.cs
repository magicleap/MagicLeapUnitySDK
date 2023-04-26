// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;

    /// <summary>
    /// MLWebView class exposes static functions that allows an application to instantiate a hardware
    /// accelerated WebView and interact with it(via "mouse" and "keyboard" events).
    /// </summary>
    public partial class MLWebView
    {
        /// <summary>
        /// The delegate for the before reasource loaded event.
        /// </summary>
        /// <param name="webView">The MLWebView associated with this callback.</param>
        /// <param name="resourceURL">The url of the resource about to be loaded.</param>
        public delegate void OnBeforeResourceLoadDelegate(MLWebView webView, string resourceURL);

        /// <summary>
        /// The delegate for the reasource loaded event.
        /// </summary>
        /// <param name="webView">The MLWebView associated with this callback.</param>
        /// <param name="isMainFrame">Whether this event was for the main frame.</param>
        /// <param name="httpStatusCode">The standard http status code.</param>
        public delegate void OnLoadEndDelegate(MLWebView webView, bool isMainFrame, int httpStatusCode);

        /// <summary>
        /// The delegate for the reasource load error event.
        /// </summary>
        /// <param name="webView">The MLWebView associated with this callback.</param>
        /// <param name="isMainFrame">True if this event was for the main frame.</param>
        /// <param name="httpStatusCode">Http status code for the URL load failure.</param>
        /// <param name="errorStr">The stringified version of the error code.</param>
        /// <param name="failedUrl">The url that caused the load error.</param>
        public delegate void OnLoadErrorDelegate(MLWebView webView, bool isMainFrame, int httpStatusCode, string errorStr, string failedUrl);

        /// <summary>
        /// The delegate for the certificate error event.
        /// </summary>
        /// <param name="webView">The MLWebView associated with this callback.</param>
        /// <param name="errorCode">Error code for ssl error.</param>
        /// <param name="url">The url associated to the certificate error.</param>
        /// <param name="errorMessage">Certificate error short description.</param>
        /// <param name="details">Certificate error details.</param>
        public delegate void OnCertificateErrorDelegate(MLWebView webView, int errorCode, string url, string errorMessage, string details, bool certificateErrorIgnored);

        /// <summary>
        /// The delegate for the keyboard show event.
        /// </summary>
        /// <param name="webView">The MLWebView associated with this callback.</param>
        /// <param name="keyboardShowData">Data about the clicked input field.</param>
        public delegate void OnShowKeyboardDelegate(MLWebView webView, InputFieldData keyboardShowData);

        /// <summary>
        /// The delegate for the keyboard dismiss event.
        /// </summary>
        /// <param name="webView">The MLWebView associated with this callback.</param>
        public delegate void OnKeyboardDismissDelegate(MLWebView webView);

        /// <summary>
        /// The delegate for the webview destroy event.
        /// </summary>
        /// <param name="webView">The MLWebView associated with this callback.</param>
        public delegate void OnDestroyDelegate(MLWebView webView);

        /// <summary>
        /// The delegate for the webview service connected event.
        /// </summary>
        /// <param name="webView">The MLWebView associated with this callback.</param>
        public delegate void OnServiceConnectedDelegate(MLWebView webView);

        /// <summary>
        /// The delegate for the webview service disconnected event.
        /// </summary>
        /// <param name="webView">The MLWebView associated with this callback.</param>
        public delegate void OnServiceDisconnectedDelegate(MLWebView webView);

        /// <summary>
        /// The delegate for the webview service failed event.
        /// </summary>
        /// <param name="webView">The MLWebView associated with this callback.</param>
        /// <param name="result">The MLResult code associated with the failure.</param>
        public delegate void OnServiceFailedDelegate(MLWebView webView, MLResult result);

        /// <summary>
        /// The delegate for the webview before popup event.
        /// </summary>
        /// <param name="webView">The MLWebView associated with this callback.</param>
        /// <param name="url">The URL for the popup to load.</param>
        public delegate void OnBeforePopupDelegate(MLWebView webView, string url, bool popupAccepted);

        /// <summary>
        /// The delegate for the webview popup opened event.
        /// </summary>
        /// <param name="webView">The MLWebView associated with this callback.</param>
        /// <param name="popupID">The ID of the popup.</param>
        /// <param name="url">The URL associated with the popup.</param>
        public delegate void OnPopupOpenedDelegate(MLWebView webView, ulong popupID, string url);

        /// <summary>
        /// The delegate fpr the webview popup closed event.
        /// </summary>
        /// <param name="webView">The MLWebView associated with this callback.</param>
        /// <param name="handle">The webview handle of the popup being closed.</param>
        public delegate void OnPopupClosedDelegate(MLWebView webView, ulong handle);

        /// <summary>
        /// Event raised just before resources are loaded.
        /// </summary>
        public event OnBeforeResourceLoadDelegate OnBeforeResourceLoaded = delegate { };

        /// <summary>
        /// Event raised when resources finish loading.
        /// </summary>
        public event OnLoadEndDelegate OnLoadEnded = delegate { };

        /// <summary>
        /// Event raised when there was an error during loading.
        /// </summary>
        public event OnLoadErrorDelegate OnErrorLoaded = delegate { };

        /// <summary>
        /// Event raised when there was an error during certification loading.
        /// </summary>
        public event OnCertificateErrorDelegate OnCertificateErrorLoaded = delegate { };

        /// <summary>
        /// Event raised if user clicked inputfield and keyboard should be displayed.
        /// </summary>
        public event OnShowKeyboardDelegate OnKeyboardShown = delegate { };

        /// <summary>
        /// Event rasied if Keyboard should be dismissed.
        /// </summary>
        public event OnKeyboardDismissDelegate OnKeyboardDismissed = delegate { };

        /// <summary>
        /// Event raised when WebView is destroyed.
        /// </summary>
        public event OnDestroyDelegate OnWebViewDestroyed = delegate { };

        /// <summary>
        /// Event raised when WebView service is connected.
        /// </summary>
        public event OnServiceConnectedDelegate OnServiceConnected = delegate { };

        /// <summary>
        /// Event raised when WebView service is disconnected.
        /// </summary>
        public event OnServiceDisconnectedDelegate OnServiceDisconnected = delegate { };

        /// <summary>
        /// Event raised when WebView service fails to connect.
        /// </summary>
        public event OnServiceFailedDelegate OnServiceFailed = delegate { };

        /// <summary>
        /// Event raised when WebView checks if a URL is OK to load in a popup.
        /// </summary>
        public event OnBeforePopupDelegate OnBeforePopup = delegate { };

        /// <summary>
        /// Event raised when WebView opened a popup.
        /// </summary>
        public event OnPopupOpenedDelegate OnPopupOpened = delegate { };

        /// <summary>
        /// Event raised when Webview is closing a popup.
        /// </summary>
        public event OnPopupClosedDelegate OnPopupClosed = delegate { };
    }
}

