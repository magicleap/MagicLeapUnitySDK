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
    /// API for MLWebView that allows an application to instantiate a hardware
    /// accelerated WebView and interact with it(via "mouse" and "keyboard" events).
    /// </summary>
    public partial class MLWebView : MLAPIBase
    {
        /// <summary>
        /// The type of the webview pause.
        /// </summary>
        public enum PauseType
        {
            /// <summary>
            /// Pause all multimedia activities of the webview.
            /// </summary>
            MultiMedia,

            /// <summary>
            /// Pause javascript timers of the webview.
            /// </summary>
            Timers,

            /// <summary>
            /// Pause and discard the webview rendering process. But keep alive the MLWebView handle.
            /// </summary>
            Discard
        }

        /// <summary>
        /// Create a MLWebView. 
        /// The MLWebView will be ready to use once this function returns with MLResult_OK.
        /// </summary>
        /// <param name="width">Width of the WebView in pixels.</param>
        /// <param name="height">Height of the WebView in pixels.</param>
        /// <returns>MLWebView instance if creation was successful, null otherwise.</returns>
        public static MLWebView Create(uint width, uint height, bool isPopup = false, ulong popupId = 0)
        {
            MLWebView webView = new MLWebView();
            return webView.CreateInternal(width, height, isPopup, popupId) == MLResult.Code.Ok ? webView : null;

        }

        /// <summary>
        /// Destroy a MLWebView. The MLWebView will be terminated by this function call and the  shall no longer be valid.
        /// </summary>
        /// <returns>MLResult.Code.Ok if was destroyed successfully.</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if an error occurred destroying the MLWebView.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult Destroy() => MLResult.Create(DestroyInternal());

        /// <summary>
        /// Retrieves the assigned web view handle.
        /// </summary>
        /// <returns>The assigned web view handle, MagicLeapNativeBindings.InvalidHandle if it has not been created.</returns>
        public ulong WebViewHandle
        {
            get => Handle;
        }

        /// <summary>
        /// Flag to indicate if urls issuing certificate errors should be loaded or not
        /// </summary>
        public bool IgnoreCertificateError
        {
            get; set;
        } = false;

        /// <summary>
        /// Flag to indicate if the application accepts the popup
        /// </summary>
        /// <value></value>
        public bool AcceptPopup
        {
            get; set;
        } = true;

        /// <summary>
        /// Go to a URL with the specified MLWebView.  Note that success with this call only indicates that a load will be
        /// attempted.  Caller can be notified about issues loading the URL via the event r on_load_error.
        /// </summary>
        /// <param name="url">URL that will be loaded.</param>
        /// <returns>MLResult.Code.Ok if WebView is attempting to load the specified URL.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult GoTo(string url) => MLResult.Create(GoToInternal(url));

        /// <summary>
        /// Trigger a "Reload" action in the MLWebView.
        /// </summary>
        /// <returns>MLResult.Code.Ok if WebView Reload action was initiated.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult Reload() => MLResult.Create(ReloadInternal());

        /// <summary>
        /// Trigger a "Back" action in the MLWebView.  Query #MLWebViewCanGoBack before calling this method.  If there is no valid
        /// page to go back to, this method will be no-op.
        /// </summary>
        /// <returns>MLResult.Code.Ok if WebView Back action was initiated or cannot go back any further.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult GoBack() => MLResult.Create(GoBackInternal());

        /// <summary>
        /// Trigger a "Forward" action in the MLWebView.  Query MLWebViewCanGoForward before calling this method.  If there is no
        /// valid page to go forward to, this method will be no-op.
        /// </summary>
        /// <returns>MLResult.Code.Ok if WebView Forward action was initiated or cannot go forward any further.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult GoForward() => MLResult.Create(GoForwardInternal());

        /// <summary>
        /// Checks if the "Back" action is currently valid.
        /// </summary>
        /// <returns>True if can use the "Back" action.</returns>
        public bool CanGoBack() => CanGoBackInternal();

        /// <summary>
        /// Checks if the "Forward" action is currently valid.
        /// </summary>
        /// <returns>True if can use the "Forward" action.</returns>
        public bool CanGoForward() => CanGoForwardInternal();

        /// <summary>
        /// Get the current URL.
        /// </summary>
        /// <returns>Current URL.</returns>
        public string GetURL() => GetURLInternal();

        /// <summary>
        /// Moves the WebView mouse.
        /// </summary>
        /// <param name="xPosition">Horizontal position of the cursor.</param>
        /// <param name="yPosition">Vertical position of the cursor. </param>
        /// <param name="modifiers">Should be one or combination of EventFlags.</param>
        /// <returns>MLResult.Code.Ok if internal mouse was moved.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult InjectMouseMove(uint xPosition, uint yPosition, EventFlags modifiers) => MLResult.Create(InjectMouseMoveInternal(xPosition, yPosition, modifiers));

        /// <summary>
        /// Sends a mouse button down/pressed event on a specific location on screen. 
        /// </summary>
        /// <param name="xPosition">Horizontal position of the cursor.</param>
        /// <param name="yPosition">Vertical position of the cursor.</param>
        /// <param name="modifiers">Should be one or combination of EventFlags.</param>
        /// <returns>MLResult.Code.Ok if successful.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult InjectMouseButtonDown(uint xPosition, uint yPosition, EventFlags modifiers) => MLResult.Create(InjectMouseButtonDownInternal(xPosition, yPosition, modifiers));

        /// <summary>
        /// Sends a mouse button up/released event on a specific location on screen.
        /// </summary>
        /// <param name="xPosition">Horizontal position of the cursor.</param>
        /// <param name="yPosition">Vertical position of the cursor.</param>
        /// <param name="modifiers">Should be one or combination of EventFlags.</param>
        /// <returns>MLResult.Code.Ok if successful.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult InjectMouseButtonUp(uint xPosition, uint yPosition, EventFlags modifiers) => MLResult.Create(InjectMouseButtonUpInternal(xPosition, yPosition, modifiers));

        /// <summary>
        /// Sends a printable char keyboard event to MLWebView.
        /// </summary>
        /// <param name="charUtf32">printable char utf code</param>
        /// <returns>MLResult.Code.Ok if key event was injected.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult InjectChar(char charUtf) => MLResult.Create(InjectCharInternal(charUtf));

        /// <summary>
        /// Sends a key down/pressed event to MLWebView.
        /// </summary>
        /// <param name="keyCode">MLWebView.KeyCode.</param>
        /// <param name="modifierMask">Should be one or combination of MLWebView.EventFlags.</param>
        /// <returns>MLResult.Code.Ok if key event was injected.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult InjectKeyDown(MLWebView.KeyCode keyCode, uint modifierMask) => MLResult.Create(InjectKeyDownInternal(keyCode, modifierMask));

        /// <summary>
        /// Sends a key up/release event to MLWebView.
        /// </summary>
        /// <param name="keyCode">MLWebView.KeyCode.</param>
        /// <param name="modifierMask">Should be one or combination of MLWebView.EventFlags.</param>
        /// <returns>MLResult.Code.Ok if key event was injected.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult InjectKeyUp(MLWebView.KeyCode keyCode, uint modifierMask) => MLResult.Create(InjectKeyUpInternal(keyCode, modifierMask));

        /// <summary>
        /// Triggers a mouse "Scroll" event.
        /// </summary>
        /// <param name="xPixels">The number of pixels to scroll on the x axis.</param>
        /// <param name="yPixels">The number of pixels to scroll on the y axis.</param>
        /// <returns>MLResult.Code.Ok if MLWebView was scrolled.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult ScrollBy(uint xPixels, uint yPixels) => MLResult.Create(ScrollByInternal(xPixels, yPixels));

        /// <summary>
        /// Get the entire scrollable size of the webview. 
        /// This should be typically called afer OnLoadEnd to determine the scollable size
        /// of the main frame of the loaded page.Some pages might dynamically resize and this should be called
        /// before each frame draw to correctly determine the scrollable size of the webview.
        /// </summary>
        /// <returns>Vector2Int representing the entire width and height of the webview, in pixels.</returns>
        [Obsolete("GetScrollSize has been deprecated and will be removed in a future release.")]
        public Vector2Int GetScrollSize() => GetScrollSizeInternal();

        /// <summary>
        /// Get the scroll offset of the webview.
        /// </summary>
        /// <returns>Vector2Int representing the horizontal and vertical offset of the webview, in pixels.</returns>
        [Obsolete("GetScrollOffset has been deprecated and will be removed in a future release.")]
        public Vector2Int GetScrollOffset() => GetScrollOffsetInternal();

        /// <summary>
        /// Reset zoom level to 1.0.
        /// </summary>
        /// <returns>MLResult.Code.Ok if MLWebView zoom was reset.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if it failed to reset zoom due to an internal error.</returns>
        public MLResult ResetZoom() => MLResult.Create(ResetZoomInternal());

        /// <summary>
        /// Zoom in one level.
        /// </summary>
        /// <returns>MLResult.Code.Ok if MLWebView zoomed in.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        /// <returns>MLResult.Code.WebViewResultZoomLimitReached if cannot zoom in any further.</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if it failed to reset zoom due to an internal error.</returns>
        public MLResult ZoomIn() => MLResult.Create(ZoomInInternal());

        /// <summary>
        /// Zoom out one level.
        /// </summary>
        /// <returns>MLResult.Code.Ok if MLWebView zoomed out.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        /// <returns>MLResult.Code.WebViewResultZoomLimitReached if cannot zoom out any further.</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if it failed to reset zoom due to an internal error.</returns>
        public MLResult ZoomOut() => MLResult.Create(ZoomOutInternal());

        /// <summary>
        /// Get the current zoom factor.  The default zoom factor is 1.0.
        /// </summary>
        /// <returns>Current numeric value for zoom factor.</returns>
        public double GetZoomFactor() => GetZoomFactorInternal();

        /// <summary>
        /// Clear the webview cache.
        /// </summary>
        /// <returns>MLResult.Code.Ok if cache cleared successfully</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if clearing cache failed due to an internal error.</returns>
        public MLResult ClearCache() => MLResult.Create(ClearCacheInternal());

        /// <summary>
        /// Remove all webview cookies.
        /// </summary>
        /// <returns>MLResult.Code.Ok if all cookies removed successfully.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if removing all cookies failed due to an internal error.</returns>
        public MLResult RemoveAllCookies() => MLResult.Create(RemoveAllCookiesInternal());

        /// <summary>
        /// Pause the webview. Call MLWebViewResume to resume.
        /// This method provides a multiple pause types to the webview.
        /// </summary>
        /// <param name="pauseType">The type of pause to be used.</param>
        /// <returns>MLResult.Code.Ok if paused successfully.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle or PauseType value.</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if failed due to an internal error.</returns>
        /// <returns>MLResult.Code.Pending if the MLWebView handle is not ready to use. See an asynchronous mode of MLWebViewCreate.</returns>
        public MLResult Pause(PauseType pauseType) => MLResult.Create(PauseInternal(pauseType));

        /// <summary>
        /// Resumes a webview after a previous call of Pause.
        /// Resume webview to the normal operation for all webview pause types.
        /// </summary>
        /// <param name="handle">The webview being accessed.</param>
        /// <returns>MLResult.Code.Ok if resumed successfully.</returns>
        /// <returns>MLResult.Code.IllegalState if WebView was paused. See MLWebViewPause.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if failed due to an internal error.</returns>
        /// <returns>MLResult.Code.Pending if the MLWebView handle is not ready to use. See an asynchronous mode of MLWebViewCreate.</returns>
        public MLResult Resume() => MLResult.Create(ResumeInternal());

        /// <summary>
        /// Struct containing data about clicked input field in WebView.
        /// </summary>
        public struct InputFieldData
        {
            /// <summary>
            /// Horizontal position of the input field.
            /// </summary>
            public int X;

            /// <summary>
            /// Vertical position of the input field.
            /// </summary>
            public int Y;

            /// <summary>
            /// Width of the input field.
            /// </summary>
            public int Width;

            /// <summary>
            /// Height of the input field.
            /// </summary>
            public int Height;

            /// <summary>
            /// One or combination of TextInputFlags.
            /// </summary>
            public TextInputFlags TextInputFlags;

            /// <summary>
            /// One of TextInputType.
            /// </summary>
            public TextInputType TextInputType;
        }
    }
}
