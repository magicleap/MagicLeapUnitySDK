// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

// Disabling deprecated warning for the internal project
#pragma warning disable 618

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// API for MLWebView that allows an application to instantiate a hardware
    /// accelerated WebView and interact with it(via "mouse" and "keyboard" events).
    /// </summary>
    public partial class MLWebView
    {

        /// <summary>
        /// GC Handle.
        /// </summary>
        private GCHandle gcHandle;

        private MLWebView()
        {
            gcHandle = GCHandle.Alloc(this, GCHandleType.Weak);
            Handle = Native.MagicLeapNativeBindings.InvalidHandle;
        }

        ~MLWebView()
        {
            gcHandle.Free();
        }

        /// <summary>
        /// Create a MLWebView. 
        /// The MLWebView will be ready to use once this function returns with MLResult_OK.
        /// </summary>
        /// <param name="width">Width of the WebView in pixels.</param>
        /// <param name="height">Height of the WebView in pixels.</param>
        /// <param name="Handle">handle to the WebView tab created.</param>
        /// <returns>MLResult.Code.Ok if the MLWebView is ready for use.</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if Unable to create the MLWebView.</returns>
        /// <returns>MLResult.Code.InvalidParam if the parameter was null pointer.</returns>
        /// <returns>MLResult.Code.PermissionDenied its missing the permission(s).</returns>
        private MLResult.Code CreateInternal(uint width, uint height, bool isPopup, ulong popupID)
        {
            Handle = MagicLeapNativeBindings.InvalidHandle;
            var resultCode = MLPermissions.CheckPermission(MLPermission.WebView).Result;
            if (!MLResult.DidNativeCallSucceed(resultCode, nameof(CreateInternal)))
            {
                MLPluginLog.Error($"{nameof(MLWebView)} requires missing permission {MLPermission.Internet}");
                return MLResult.Code.PermissionDenied;
            }

            var settings = NativeBindings.Settings.Create(gcHandle, width, height, isPopup, popupID);

            MLResult.Code result = NativeBindings.MLWebViewCreate(out Handle, ref settings);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewCreate));
            return result;
        }

        /// <summary>
        /// Destroy a MLWebView. The MLWebView will be terminated by this function call and the handle shall no longer be valid.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to destroy.</param>
        /// <returns>MLResult.Code.Ok if was destroyed successfully.</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if an error occurred destroying the MLWebView.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        private MLResult.Code DestroyInternal()
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return MLResult.Code.InvalidParam;
            }

            MLResult.Code result = NativeBindings.MLWebViewDestroy(Handle);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewDestroy));
            Handle = MagicLeapNativeBindings.InvalidHandle;
            return result;
        }

        /// <summary>
        /// Go to a URL with the specified MLWebView.  Note that success with this call only indicates that a load will be
        /// attempted.  Caller can be notified about issues loading the URL via the event handler on_load_error.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to issue the URL request to.</param>
        /// <param name="url">URL that will be loaded.</param>
        /// <returns>MLResult.Code.Ok if WebView is attempting to load the specified URL.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        private MLResult.Code GoToInternal(string url)
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return MLResult.Code.InvalidParam;
            }
            var result = NativeBindings.MLWebViewGoTo(Handle, url);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewGoTo));
            return result;
        }

        /// <summary>
        /// Trigger a "Back" action in the MLWebView.  Query #MLWebViewCanGoBack before calling this method.  If there is no valid
        /// page to go back to, this method will be no-op.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to issue the go back request to.</param>
        /// <returns>MLResult.Code.Ok if WebView Back action was initiated or cannot go back any further.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        private MLResult.Code GoBackInternal()
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return MLResult.Code.InvalidParam;
            }
            var result = NativeBindings.MLWebViewGoBack(Handle);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewGoBack));
            return result;
        }

        /// <summary>
        /// Trigger a "Forward" action in the MLWebView.  Query MLWebViewCanGoForward before calling this method.  If there is no
        /// valid page to go forward to, this method will be no-op.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to issue the go forward request to.</param>
        /// <returns>MLResult.Code.Ok if WebView Forward action was initiated or cannot go forward any further.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        private MLResult.Code GoForwardInternal()
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return MLResult.Code.InvalidParam;
            }
            var result = NativeBindings.MLWebViewGoForward(Handle);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewGoForward));
            return result;
        }

        /// <summary>
        /// Trigger a "Reload" action in the MLWebView.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to reload.</param>
        /// <returns>MLResult.Code.Ok if WebView Reload action was initiated.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        private MLResult.Code ReloadInternal()
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return MLResult.Code.InvalidParam;
            }
            var result = NativeBindings.MLWebViewReload(Handle);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewReload));
            return result;
        }

        /// <summary>
        /// Get the current URL.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to get the current URL address of.</param>
        /// <returns>Current URL.</returns>
        private string GetURLInternal()
        {
            string url = string.Empty;

            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return url;
            }

            MLResult.Code result = NativeBindings.MLWebViewGetUrl(Handle, out uint length, IntPtr.Zero);

            if (MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewGetUrl)))
            {
                IntPtr stringPtr = Marshal.AllocHGlobal((int)length);

                try
                {
                    result = NativeBindings.MLWebViewGetUrl(Handle, out length, stringPtr);
                    MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewGetUrl));
                    url = Marshal.PtrToStringAnsi(stringPtr);
                }
                finally
                {
                    Marshal.FreeHGlobal(stringPtr);
                }
            }

            return url;
        }

        /// <summary>
        /// Checks if the "Back" action is currently valid.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to check go back status of.</param>
        /// <returns>True if can use the "Back" action.</returns>
        private bool CanGoBackInternal()
        {
            bool canGoBack = false;
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return canGoBack;
            }

            MLResult.Code result = NativeBindings.MLWebViewCanGoBack(Handle, out canGoBack);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewCanGoBack), IllegalStatePredicate);
            return canGoBack;
        }

        /// <summary>
        /// Checks if the "Forward" action is currently valid.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to check go forward status of.</param>
        /// <returns>True if can use the "Forward" action.</returns>
        private bool CanGoForwardInternal()
        {
            bool canGoForward = false;

            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return canGoForward;
            }

            MLResult.Code result = NativeBindings.MLWebViewCanGoForward(Handle, out canGoForward);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewCanGoForward), IllegalStatePredicate);
            return canGoForward;
        }

        /// <summary>
        /// Illegal state just means that webview is paused, use this to not log an error for appropriate functions.
        /// </summary>
        private bool IllegalStatePredicate(MLResult.Code code) => code == MLResult.Code.Ok || code == MLResult.Code.IllegalState;

        /// <summary>
        /// Moves the WebView mouse.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to issue mouse move to.</param>
        /// <param name="xPosition">Horizontal position of the cursor.</param>
        /// <param name="yPosition">Vertical position of the cursor. </param>
        /// <param name="modifiers">Should be one or combination of EventFlags.</param>
        /// <returns>MLResult.Code.Ok if internal mouse was moved.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        private MLResult.Code InjectMouseMoveInternal(uint xPosition, uint yPosition, EventFlags modifiers)
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return MLResult.Code.InvalidParam;
            }

            NativeBindings.CursorState cursorState = NativeBindings.CursorState.Create(xPosition, yPosition, modifiers);
            var result = NativeBindings.MLWebViewInjectMouseMove(Handle, ref cursorState);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewInjectMouseMove));
            return result;
        }

        /// <summary>
        /// Sends a mouse button down/pressed event on a specific location on screen.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to issue mouse down to.</param>
        /// <param name="xPosition">Horizontal position of the cursor.</param>
        /// <param name="yPosition">Vertical position of the cursor.</param>
        /// <param name="modifiers">Should be one or combination of EventFlags.</param>
        /// <returns>MLResult.Code.Ok if successful.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        private MLResult.Code InjectMouseButtonDownInternal(uint xPosition, uint yPosition, EventFlags modifiers)
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return MLResult.Code.InvalidParam;
            }

            NativeBindings.CursorState cursorState = NativeBindings.CursorState.Create(xPosition, yPosition, modifiers);
            var result = NativeBindings.MLWebViewInjectMouseButtonDown(Handle, ref cursorState);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewInjectMouseButtonDown));
            return result;
        }

        /// <summary>
        /// Sends a mouse button up/released event on a specific location on screen.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to issue mouse up to.</param>
        /// <param name="xPosition">Horizontal position of the cursor.</param>
        /// <param name="yPosition">Vertical position of the cursor.</param>
        /// <param name="modifiers">Should be one or combination of EventFlags.</param>
        /// <returns>MLResult.Code.Ok if successful.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        private MLResult.Code InjectMouseButtonUpInternal(uint xPosition, uint yPosition, EventFlags modifiers)
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return MLResult.Code.InvalidParam;
            }

            NativeBindings.CursorState cursorState = NativeBindings.CursorState.Create(xPosition, yPosition, modifiers);
            var result = NativeBindings.MLWebViewInjectMouseButtonUp(Handle, ref cursorState);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewInjectMouseButtonUp));
            return result;
        }

        /// <summary>
        /// Sends a printable char keyboard event to MLWebView.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to issue char utf code to.</param>
        /// <param name="charUtf32">printable char utf code</param>
        /// <returns>MLResult.Code.Ok if key event was injected.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        private MLResult.Code InjectCharInternal(char charUtf32)
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return MLResult.Code.InvalidParam;
            }

            var result = NativeBindings.MLWebViewInjectChar(Handle, Convert.ToUInt32(charUtf32));
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewInjectChar));
            return result;
        }

        /// <summary>
        /// Sends a key down/pressed event to MLWebView.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to issue key down to.</param>
        /// <param name="keyCode">MLWebView.KeyCode.</param>
        /// <param name="modifierMask">Should be one or combination of MLWebView.EventFlags.</param>
        /// <returns>MLResult.Code.Ok if key event was injected.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        private MLResult.Code InjectKeyDownInternal(KeyCode keyCode, uint modifierMask)
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return MLResult.Code.InvalidParam;
            }

            var result = NativeBindings.MLWebViewInjectKeyDown(Handle, keyCode, modifierMask);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewInjectKeyDown));
            return result;
        }

        /// <summary>
        /// Sends a key up/release event to MLWebView.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to issue key up to.</param>
        /// <param name="keyCode">MLWebView.KeyCode.</param>
        /// <param name="modifierMask">Should be one or combination of MLWebView.EventFlags.</param>
        /// <returns>MLResult.Code.Ok if key event was injected.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        private MLResult.Code InjectKeyUpInternal(KeyCode keyCode, uint modifierMask)
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return MLResult.Code.InvalidParam;
            }

            var result = NativeBindings.MLWebViewInjectKeyUp(Handle, keyCode, modifierMask);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewInjectKeyUp));
            return result;
        }

        /// <summary>
        /// Triggers a mouse "Scroll" event.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to issue scroll to.</param>
        /// <param name="xPixels">The number of pixels to scroll on the x axis.</param>
        /// <param name="yPixels">The number of pixels to scroll on the y axis.</param>
        /// <returns>MLResult.Code.Ok if MLWebView was scrolled.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        private MLResult.Code ScrollByInternal(uint xPixels, uint yPixels)
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return MLResult.Code.InvalidParam;
            }

            var result = NativeBindings.MLWebViewScrollBy(Handle, xPixels, yPixels);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewScrollBy));
            return result;
        }

        /// <summary>
        /// Get the entire scrollable size of the webview. 
        /// This should be typically called afer HandleOnLoadEnd to determine the scollable size
        /// of the main frame of the loaded page.Some pages might dynamically resize and this should be called
        /// before each frame draw to correctly determine the scrollable size of the webview.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to request scroll size from.</param>
        /// <returns>Vector2Int representing the entire width and height of the webview, in pixels.</returns>
        /// TODO: To be removed due to deprecation.
        private Vector2Int GetScrollSizeInternal()
        {
            int width = 0;
            int height = 0;
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return new Vector2Int(width, height);
            }

            MLResult.Code result = NativeBindings.MLWebViewGetScrollSize(Handle, out width, out height);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewGetScrollSize));
            return new Vector2Int(width, height);
        }

        /// <summary>
        /// Get the scroll offset of the webview.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab request scroll offset from.</param>
        /// <returns>Vector2Int representing the horizontal and vertical offset of the webview, in pixels.</returns>
        /// TODO: To be removed due to deprecation.
        private Vector2Int GetScrollOffsetInternal()
        {
            int x = 0;
            int y = 0;
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return new Vector2Int(x, y);
            }

            MLResult.Code result = NativeBindings.MLWebViewGetScrollOffset(Handle, out x, out y);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewGetScrollOffset));
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// Reset zoom level to 1.0.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to reset zoom of.</param>
        /// <returns>MLResult.Code.Ok if MLWebView zoom was reset.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if it failed to reset zoom due to an internal error.</returns>
        private MLResult.Code ResetZoomInternal()
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return MLResult.Code.InvalidParam;
            }

            var result = NativeBindings.MLWebViewResetZoom(Handle);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewResetZoom));
            return result;
        }

        /// <summary>
        /// Zoom in one level.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to issue zoom in to.</param>
        /// <returns>MLResult.Code.Ok if MLWebView zoomed in.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        /// <returns>MLResult.Code.WebViewResultZoomLimitReached if cannot zoom in any further.</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if it failed to reset zoom due to an internal error.</returns>
        private MLResult.Code ZoomInInternal()
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return MLResult.Code.InvalidParam;
            }

            var result = NativeBindings.MLWebViewZoomIn(Handle);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewZoomIn));
            return result;
        }

        /// <summary>
        /// Zoom out one level.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to issue zoom out to.</param>
        /// <returns>MLResult.Code.Ok if MLWebView zoomed out.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        /// <returns>MLResult.Code.WebViewResultZoomLimitReached if cannot zoom out any further.</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if it failed to reset zoom due to an internal error.</returns>
        private MLResult.Code ZoomOutInternal()
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return MLResult.Code.InvalidParam;
            }

            var result = NativeBindings.MLWebViewZoomOut(Handle);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewZoomOut));
            return result;
        }

        /// <summary>
        /// Get the current zoom factor.  The default zoom factor is 1.0.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to request zoom factor from.</param>
        /// <returns>Current numeric value for zoom factor.</returns>
        private double GetZoomFactorInternal()
        {
            double zoomFactor = 1;
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return zoomFactor;
            }

            MLResult.Code result = NativeBindings.MLWebViewGetZoomFactor(Handle, out zoomFactor);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewGetZoomFactor), IllegalStatePredicate);
            return zoomFactor;
        }

        /// <summary>
        /// Remove all webview cookies.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to clear all cookies of.</param>
        /// <returns>MLResult.Code.Ok if all cookies removed successfully.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if removing all cookies failed due to an internal error.</returns>
        private MLResult.Code RemoveAllCookiesInternal()
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return MLResult.Code.InvalidParam;
            }

            var result = NativeBindings.MLWebViewRemoveAllCookies(Handle);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewRemoveAllCookies));
            return result;
        }

        /// <summary>
        /// Clear the webview cache.
        /// </summary>
        /// <param name="Handle">handle to the WebView tab to clear cache of.</param>
        /// <returns>MLResult.Code.Ok if cache cleared successfully</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if clearing cache failed due to an internal error.</returns>
        private MLResult.Code ClearCacheInternal()
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
            {
                MLPluginLog.Error("invalid WebView handle");
                return MLResult.Code.InvalidParam;
            }

            var result = NativeBindings.MLWebViewClearCache(Handle);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewClearCache));
            return result;
        }


        /// <summary>
        /// Pause the webview. Call MLWebViewResume to resume.
        /// This method provides a multiple pause types to the webview.
        /// </summary>
        /// <param name="pauseType">The type of pause to be used.</param>
        /// <returns>MLResult.Code.Ok if paused successfully.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle or PauseType value.</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if failed due to an internal error.</returns>
        /// <returns>MLResult.Code.Pending if the MLWebView handle is not ready to use. See an asynchronous mode of MLWebViewCreate.</returns>
        private MLResult.Code PauseInternal(PauseType pauseType)
        {
            MLResult.Code result = NativeBindings.MLWebViewPause(Handle, pauseType);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewPause));
            return result;
        }

        /// <summary>
        /// Resumes a webview after a previous call of the MLWebViewPause.
        /// Resume webview to the normal operation for all webview pause types.
        /// </summary>
        /// <returns>MLResult.Code.Ok if resumed successfully.</returns>
        /// <returns>MLResult.Code.IllegalState if WebView was paused. See MLWebViewPause.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if failed due to an internal error.</returns>
        /// <returns>MLResult.Code.Pending if the MLWebView handle is not ready to use. See an asynchronous mode of MLWebViewCreate.</returns>
        private MLResult.Code ResumeInternal()
        {
            MLResult.Code result = NativeBindings.MLWebViewResume(Handle);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewResume));
            return result;
        }
    }
}
