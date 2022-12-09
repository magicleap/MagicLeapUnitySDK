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
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Class that represents a source used by the MLWebRTC API.
        /// </summary>
        public partial class AppDefinedVideoSource
        {
            /// <summary>
            /// Native bindings for the MLWebRTC.AppDefinedVideoSource class. 
            /// </summary>
            internal new class NativeBindings : MagicLeapNativeBindings
            {
                /// <summary>
                /// Initialized a given appDefinedVideoSource object and sets it's callbacks.
                /// </summary>
                /// <param name="appDefinedVideoSource">The AppDefinedVideoSource object to initialize.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the connection was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                public static MLResult.Code InitializeAppDefinedVideoSource(MLWebRTC.AppDefinedVideoSource appDefinedVideoSource)
                {
                    appDefinedVideoSource.TrackType = Type.Video;
                    appDefinedVideoSource.IsLocal = true;
                    appDefinedVideoSource.gcHandle = GCHandle.Alloc(appDefinedVideoSource);
                    IntPtr gcHandlePtr = GCHandle.ToIntPtr(appDefinedVideoSource.gcHandle);

                    ulong appDefinedVideoSourceHandle = MagicLeapNativeBindings.InvalidHandle;

                    AppDefinedSource.NativeBindings.MLWebRTCAppDefinedSourceEventCallbacks callbacks = AppDefinedSource.NativeBindings.MLWebRTCAppDefinedSourceEventCallbacks.Create(gcHandlePtr, OnSetEnabled, OnDestroyed);
                    MLResult.Code resultCode = NativeBindings.MLWebRTCSourceCreateAppDefinedVideoSource(in callbacks, out appDefinedVideoSourceHandle);

                    appDefinedVideoSource.Handle = appDefinedVideoSourceHandle;

                    if (!MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCSourceCreateAppDefinedVideoSource)))
                    {
                        appDefinedVideoSource.gcHandle.Free();
                    }

                    return resultCode;
                }

                /// <summary>
                /// Callback that is invoked when the source has been enabled or disabled. This callback will be called on the main thread.
                /// </summary>
                /// <param name="enabled">True if the source was enabled.</param>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(AppDefinedSource.NativeBindings.OnSetEnabledDelegate))]
                private static void OnSetEnabled(bool enabled, IntPtr context)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(context);
                    AppDefinedVideoSource videoSource = gcHandle.Target as AppDefinedVideoSource;
                    videoSource?.OnSourceSetEnabled(enabled);
                }

                /// <summary>
                /// Callback that is invoked when the source has been destroyed. This callback will be called on the main thread.
                /// </summary>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(AppDefinedSource.NativeBindings.OnDestroyedDelegate))]
                private static void OnDestroyed(IntPtr context)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(context);
                    AppDefinedVideoSource videoSource = gcHandle.Target as AppDefinedVideoSource;
                    videoSource?.OnSourceDestroy();
                }

                /// <summary>
                /// Creates the local source that links to the user's camera and mic.
                /// </summary>
                /// <param name="sourceHandle">The handle to the local source to return to the caller.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the local source was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceCreateAppDefinedVideoSource(in AppDefinedSource.NativeBindings.MLWebRTCAppDefinedSourceEventCallbacks callbacks, out ulong sourceHandle);

                /// <summary>
                /// Creates the local source that links to the user's camera and mic.
                /// </summary>
                /// <param name="sourceHandle">The handle to the local source to return to the caller.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the local source was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceAppDefinedVideoSourcePushFrame(ulong sourceHandle, in MLWebRTC.VideoSink.Frame.NativeBindings.MLWebRTCFrame frameNative);
            }
        }
    }
}
