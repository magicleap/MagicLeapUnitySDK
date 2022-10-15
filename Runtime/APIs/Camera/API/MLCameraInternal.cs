// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

// ReSharper disable All

using UnityEngine.Android;

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
#if UNITY_MAGICLEAP || UNITY_ANDROID
    using UnityEngine.XR.MagicLeap.Native;
#endif

    /// <summary>
    /// MLCamera class exposes static functions to query camera related
    /// functions. Most functions are currently a direct pass through functions to the
    /// native C-API functions and incur no overhead.
    /// </summary>
    public sealed partial class MLCamera
    {
        #region properties

        private static bool cameraInited = false;

        /// <summary>
        /// A raw video queue lock object.
        /// </summary>
        private static object rawVideoFrameQueueLockObject = new object();

        /// <summary>
        /// Wait handle used to syncronize MLCamera.Stop() and any ongoing disconnect request.
        /// </summary>
        private AutoResetEvent cameraDisconnectWaitHandle = new AutoResetEvent(true);

        /// <summary>
        /// Preview Renderer;
        /// </summary>
        private MLCamera.Renderer previewRenderer;

        /// <summary>
        /// GC Handle.
        /// </summary>
        private GCHandle gcHandle;

        /// <summary>
        /// A reference to Camera Preview texture. 
        /// </summary>
        private RenderTexture previewTexture;

        /// <summary>
        /// Connection status of the camera.
        /// </summary>
        private bool cameraConnectionEstablished = false;

        /// <summary>
        /// Timestamp of previously captured frame. needed for FPS calculations.
        /// </summary>
        private long previousCaptureTimestamp;

        /// <summary>
        /// Calculated FPS based on recevied frames interval.
        /// </summary>
        private float currentFPS;

        /// <summary>
        /// Capture status of the camera.
        /// </summary>
        private bool isCapturingVideo = false;

        /// <summary>
        /// Gets a value indicating whether the camera preview is enabled and textures are being rendered.
        /// </summary>
        private bool isCapturingPreview = false;

        /// <summary>
        /// The preview texture for the camera captures.
        /// </summary>
        private Texture2D previewTexture2D = null;

        /// <summary>
        /// The resume connect state of the camera.
        /// </summary>
        private bool resumeConnect = false;

        /// <summary>
        /// The resume preview state of the camera.
        /// </summary>
        private bool resumePreviewCapture = false;

        /// <summary>
        /// The resume raw video capture state of the camera.
        /// </summary>
        private bool resumeVideoCapture = false;

        /// <summary>
        /// camera Connect Context
        /// </summary>
        private ConnectContext cameraConnectContext;

        /// <summary>
        /// Last used camera Capture Config
        /// </summary>
        private MLCamera.CaptureConfig cameraCaptureConfig;

        /// <summary>
        /// Used by the Async methods to ensure things don't happen concurrently when they shouldn't,
        /// since C-API is not threadsafe. 
        /// </summary>
        private static readonly object apiLock = new object();

        /// <summary>
        /// Gets the texture for the camera preview render.
        /// (null) when preview is not available yet.
        /// </summary>
        private Texture2D PreviewTexture2D
        {
            get => isCapturingPreview ? previewTexture2D : null;

            set => previewTexture2D = value;
        }

        private byte[][] byteArrays;

        #endregion

        private MLCamera()
        {
            gcHandle = GCHandle.Alloc(this, GCHandleType.Weak);
#if UNITY_MAGICLEAP || UNITY_ANDROID
            Handle = Native.MagicLeapNativeBindings.InvalidHandle;
            byteArrays = new byte[NativeBindings.MLCameraMaxImagePlanes][];
#endif
        }

        ~MLCamera()
        {
            gcHandle.Free();
            cameraDisconnectWaitHandle.WaitOne();
            Disconnect();
            InternalUninitialize();
            ClearPreviewTexture();
        }

        /// <summary>
        /// Create a preview texture.
        /// </summary>
        private void CreatePreviewTexture()
        {
            int width = cameraCaptureConfig.StreamConfigs[0].Width;
            int height = cameraCaptureConfig.StreamConfigs[0].Height;

            if (previewTexture != null && (previewTexture.width != width || previewTexture.height != height))
            {
                ClearPreviewTexture();
            }
            if (previewTexture == null)
            {
                previewTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            }

            // preview rendering not supported under Magic Leap App Simulator
#if !UNITY_EDITOR
            previewRenderer?.Cleanup();
            previewRenderer = new MLCamera.Renderer();
            previewRenderer.SetRenderBuffer(previewTexture);
#endif
        }

        private MLResult InternalCapturePreviewStart()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            lock (apiLock)
            {
                MLResult.Code resultCode = NativeBindings.MLCameraCapturePreviewStart(Handle);
                isCapturingPreview = MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraCapturePreviewStart));
                return MLResult.Create(resultCode);
            }
#else
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif
        }

        private void ClearPreviewTexture()
        {
            if (previewTexture != null)
            {
                GameObject.Destroy(previewTexture);
                previewTexture = null;
            }
        }

        /// <summary>
        /// Releases allocated memory for video capture circular buffers.
        /// </summary>
        private void ReleaseCaptureBuffers()
        {
            for (int i = 0; i < byteArrays.Length; ++i)
            {
                byteArrays[i] = null;
            }
        }

        /// <summary>
        /// Issues an event when a render event has occurred.
        /// </summary>
        private void GLPluginEvent()
        {
            // preview rendering not supported under Magic Leap App Simulator
#if !UNITY_EDITOR
            previewRenderer?.Render();
#endif
        }

        protected async override void OnApplicationPause(bool pauseStatus)
        {
            applicationPausePerfMarker.Begin();
            MLResult.Code result = pauseStatus ? await InternalPause() : await InternalResume();
            applicationPausePerfMarker.End();
            if (result != MLResult.Code.Ok)
            {
                Debug.LogErrorFormat("MLCamera.ApplicationPause failed to {0} the camera. Reason: {1}",
                    pauseStatus ? "pause" : "resume", result);
            }
        }

        /// <summary>
        /// Pause the camera capture.
        /// </summary>
        /// <param name="flagsOnly">Set to true if you only need to store the current state of camera captures from Unity's perspective without actually invoking platform functions. False by default.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        private async Task<MLResult.Code> InternalPause(bool flagsOnly = false)
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLResult.Code result = MLResult.Code.Ok;

            if (cameraConnectionEstablished)
            {
                resumeConnect = true;
                resumePreviewCapture = isCapturingPreview;
                resumeVideoCapture = isCapturingVideo;

                result = await Task.Run(() => InternalDisconnect(flagsOnly));
                if (!MLResult.IsOK(result))
                {
                    return result;
                }
            }

            return result;
#else
            return MLResult.Code.NotImplemented;
#endif
        }

        /// <summary>
        /// Resume the camera capture.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.MediaGenericUnexpectedNull</c> if failed to connect to camera device due to null pointer.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if a parameter is invalid (null).
        /// MLResult.Result will be <c>MLResult.Code.Timeout</c> if failed to connect to camera device due to timeout.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if failed to allocate memory.
        /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
        /// </returns>
        private async Task<MLResult.Code> InternalResume()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLResult.Code resultCode = MLResult.Code.Ok;

            if (resumeConnect)
            {
                if(InternalCheckCameraPermission() != MLResult.Code.Ok)
                {
                    return MLResult.Code.PermissionDenied;
                }

                resultCode = InternalConnect(cameraConnectContext);

                resumeConnect = false;
                if (!MLResult.IsOK(resultCode))
                {
                    MLPluginLog.ErrorFormat("MLCamera.Resume failed to connect camera. Reason: {0}", resultCode);
                    return resultCode;
                }

                if (resumePreviewCapture)
                {
                    resultCode = InternalPrepareCapture(cameraCaptureConfig, out MLCamera.Metadata _);

                    if (!MLResult.IsOK(resultCode))
                    {
                        MLPluginLog.ErrorFormat("MLCamera.Resume failed to prepare capture. Reason: {0}", resultCode);
                        return resultCode;
                    }

                    await PreCaptureAEAWBAsync();

                    resultCode = (await CapturePreviewStartAsync()).Result;

                    resumePreviewCapture = false;
                    if (!MLResult.IsOK(MLResult.Code.Ok))
                    {
                        MLPluginLog.ErrorFormat("MLCamera.Resume failed to start camera preview. Reason: {0}", resultCode);
                        return resultCode;
                    }
                }
                else if (resumeVideoCapture)
                {
                    PrepareCapture(cameraCaptureConfig, out Metadata _);

                    if (!MLResult.IsOK(resultCode))
                    {
                        MLPluginLog.ErrorFormat("MLCamera.Resume failed to prepare capture. Reason: {0}", resultCode);
                        return resultCode;
                    }

                    await PreCaptureAEAWBAsync();
                    resultCode = (await CaptureVideoStartAsync()).Result;

                    resumeVideoCapture = false;
                    if (!MLResult.IsOK(MLResult.Code.Ok))
                    {
                        MLPluginLog.ErrorFormat("MLCamera.Resume failed to start camera preview. Reason: {0}", resultCode);
                        return resultCode;
                    }
                }
            }

            return resultCode;
#else
            return MLResult.Code.NotImplemented;
#endif
        }

        ///DONE
        /// <summary>
        /// Disconnect and clear the capture callbacks for the camera.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if completed successfully.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if a required permission is missing.
        /// </returns>
        private MLResult.Code DisconnectNative()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            var resultCode = NativeBindings.MLCameraDisconnect(Handle);

#if !UNITY_EDITOR
            // preview rendering not supported under Magic Leap App Simulator
            MLThreadDispatch.ScheduleMain(() => previewRenderer?.Cleanup());
#endif
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraDisconnect));

            return resultCode;
#else
            return MLResult.Code.NotImplemented;
#endif
        }

        /// <summary>
        /// Establish a connection to the camera.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if connected to camera device successfully.
        /// MLResult.Result will be <c>MLResult.Code.MediaGenericUnexpectedNull</c> if failed to connect to camera device due to null pointer.
        /// MLResult.Result will be <c>MLResult.Code.Timeout</c> if failed  connecting the camera due to timeout.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if failed to allocate memory.
        /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if a required permission is missing.
        /// </returns>
        private static MLResult InternalInitialize()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLResult.Code resultCode = MLResult.Code.Ok;
            lock (apiLock)
            {
                if (!cameraInited)
                {
                    NativeBindings.MLCameraDeviceAvailabilityStatusCallbacks callbacks =
                        NativeBindings.MLCameraDeviceAvailabilityStatusCallbacks.Create();
                    resultCode = NativeBindings.MLCameraInit(ref callbacks, IntPtr.Zero);
                    cameraInited = MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraInit));
                }
            }

            return MLResult.Create(resultCode);
#else
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif
        }

        private static MLResult InternalUninitialize()
        {
            if (!cameraInited)  // already uninited
                return MLResult.Create(MLResult.Code.Ok);

            cameraInited = false;
#if UNITY_MAGICLEAP || UNITY_ANDROID
            NativeBindings.MLCameraDeviceAvailabilityStatusCallbacks callbacks =
                NativeBindings.MLCameraDeviceAvailabilityStatusCallbacks.CreateUninitialized();
            var resultCode = NativeBindings.MLCameraDeInit();
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraDeInit));
            return MLResult.Create(resultCode);
#else
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif
        }

        private static MLResult.Code InternalCheckCameraPermission()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            var check = MLPermissions.CheckPermission(MLPermission.Camera);
            if (!MLResult.DidNativeCallSucceed(check.Result, nameof(MLPermissions.CheckPermission)))
            {
                return MLResult.Code.PermissionDenied;
            }
#endif
            return MLResult.Code.Ok;
        }

        private MLResult.Code InternalConnect(ConnectContext cameraConnectContext)
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            this.cameraConnectContext = cameraConnectContext;

            NativeBindings.MLCameraConnectContext context = NativeBindings.MLCameraConnectContext.Create(cameraConnectContext);

            MLResult.Code resultCode = MLResult.Code.Ok;

            lock (apiLock)
            {
                if (!cameraConnectionEstablished)
                {
                    resultCode = NativeBindings.MLCameraConnect(ref context, out Handle);

                    if (!MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraConnect)))
                    {
                        Debug.LogErrorFormat("MLCamera.Connect failed connecting to the camera. Reason: {0}", resultCode);
                        connectPerfMarker.End();
                        return resultCode;
                    }

                    NativeBindings.MLCameraCaptureCallbacks captureCallbacks = NativeBindings.MLCameraCaptureCallbacks.Create();

                    if (!MLResult.DidNativeCallSucceed(NativeBindings.MLCameraSetCaptureCallbacks(Handle, ref captureCallbacks, GCHandle.ToIntPtr(gcHandle)), nameof(NativeBindings.MLCameraSetCaptureCallbacks)))
                    {
                        MLPluginLog.ErrorFormat("MLCamera.CaptureCallbacks failed seting the capture callbacks. Reason: {0}", resultCode);
                        connectPerfMarker.End();
                        return resultCode;
                    }

                    NativeBindings.MLCameraDeviceStatusCallbacks deviceCallbacks = NativeBindings.MLCameraDeviceStatusCallbacks.Create();

                    if (!MLResult.DidNativeCallSucceed(NativeBindings.MLCameraSetDeviceStatusCallbacks(Handle, ref deviceCallbacks, GCHandle.ToIntPtr(gcHandle)), nameof(NativeBindings.MLCameraSetDeviceStatusCallbacks)))
                    {
                        MLPluginLog.ErrorFormat("MLCamera.DeviceStatusCallbacks failed setting the device callbacks. Reason: {0}", resultCode);
                        connectPerfMarker.End();
                        return resultCode;
                    }

                    cameraConnectionEstablished = MLResult.IsOK(resultCode);
                }
            }
            if (!MLResult.IsOK(resultCode))
            {
                MLPluginLog.ErrorFormat("MLCamera.InternalConnect failed to populate characteristics for MLCamera. Reason: {0}", resultCode);
                connectPerfMarker.End();
                return resultCode;
            }

            connectPerfMarker.End();
            return resultCode;
#else
            return MLResult.Code.NotImplemented;
#endif
        }

        /// <summary>
        /// Disconnect the camera.
        /// </summary>
        /// <param name="flagsOnly">Set to true if you only need to disconect the camera and not stop any ongoing captuers. Useful when disconnecting due to internal errors. False by default.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if completed successfully.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if a required permission is missing.
        /// </returns>
        private MLResult.Code InternalDisconnect(bool flagsOnly = false)
        {
            disconnectPerfMarker.Begin();

            if (this.cameraConnectionEstablished)
            {
                if (this.isCapturingVideo && !flagsOnly)
                {
                    var stopResult = CaptureVideoStop();
                    if (stopResult.IsOk)
                    {
                        this.isCapturingVideo = false;
                    }
                }

                if (this.isCapturingPreview && !flagsOnly)
                {
                    var stopResult = CapturePreviewStop();
                    if (stopResult.IsOk)
                    {
                        isCapturingPreview = false;
                    }
                }

                MLResult.Code result = DisconnectNative();

                // Assume that connection is no longer established
                // even if there is some failure disconnecting.
                this.cameraConnectionEstablished = false;
            }

            ReleaseCaptureBuffers();
            disconnectPerfMarker.Begin();

            return MLResult.Code.Ok;
        }

        private MLResult InternalGetStreamCapabilities(out StreamCapabilitiesInfo[] streamCapabilities)
        {
            streamCapabilities = null;

#if UNITY_MAGICLEAP || UNITY_ANDROID
            var resultCode = NativeBindings.MLCameraGetNumSupportedStreams(Handle, out uint supportedStreams);
            if (!MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraGetNumSupportedStreams)))
            {
                return MLResult.Create(resultCode);
            }

            streamCapabilities = new StreamCapabilitiesInfo[supportedStreams];
            int sizeOfStruct = Marshal.SizeOf(typeof(NativeBindings.MLCameraCaptureStreamCaps));

            for (uint i = 0; i < supportedStreams; i++)
            {
                streamCapabilities[i] = new StreamCapabilitiesInfo();
                uint streamCapsMax = 0;

                resultCode = NativeBindings.MLCameraGetStreamCaps(Handle, i, ref streamCapsMax, IntPtr.Zero);
                if (!MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraGetStreamCaps)))
                {
                    return MLResult.Create(resultCode);
                }

                streamCapabilities[i].StreamCapabilities = new StreamCapability[streamCapsMax];
                IntPtr pointer = Marshal.AllocHGlobal((int)streamCapsMax *
                    Marshal.SizeOf(typeof(NativeBindings.MLCameraCaptureStreamCaps)));
                resultCode = NativeBindings.MLCameraGetStreamCaps(Handle, i, ref streamCapsMax, pointer);
                if (!MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraGetStreamCaps)))
                {
                    Marshal.FreeHGlobal(pointer);
                    return MLResult.Create(resultCode);
                }

                for (int j = 0; j < streamCapsMax; j++)
                {
                    NativeBindings.MLCameraCaptureStreamCaps nativeStreamCap =
                        Marshal.PtrToStructure<NativeBindings.MLCameraCaptureStreamCaps>(pointer);

                    streamCapabilities[i].StreamCapabilities[j] = new StreamCapability()
                    {
                        Width = nativeStreamCap.Width,
                        Height = nativeStreamCap.Height,
                        CaptureType = nativeStreamCap.CaptureType
                    };

                    pointer += sizeOfStruct;
                }
            }

            return MLResult.Create(resultCode);
#else
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif
        }

        private MLResult.Code InternalPrepareCapture(CaptureConfig captureConfig, out Metadata cameraMetadata)
        {
            cameraCaptureConfig = captureConfig;
            cameraMetadata = null;

#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLResult.Code resultCode = MLResult.Code.Ok;
            NativeBindings.MLCameraCaptureConfig nativeCaptureConfig = NativeBindings.MLCameraCaptureConfig.Create(captureConfig);
            resultCode = NativeBindings.MLCameraPrepareCapture(Handle, ref nativeCaptureConfig, out ulong metadataHandle);
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraPrepareCapture)))
            {
                cameraMetadata = new Metadata(metadataHandle);
            }
            return resultCode;
#else
            cameraMetadata = null;
            return MLResult.Code.NotImplemented;
#endif
        }
    }
}
