// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

// ReSharper disable All

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap.Native;


    /// <summary>
    /// MLCamera class exposes static functions to query camera related
    /// functions. Most functions are currently a direct pass through functions to the
    /// native C-API functions and incur no overhead.
    /// </summary>
    public partial class MLCameraBase
    {
        #region properties

        protected static bool cameraInited = false;

        /// <summary>
        /// A raw video queue lock object.
        /// </summary>
        protected static object rawVideoFrameQueueLockObject = new object();

        /// <summary>
        /// Preview Renderer;
        /// </summary>
        protected MLCamera.Renderer previewRenderer;

        /// <summary>
        /// GC Handle.
        /// </summary>
        protected GCHandle gcHandle;

        /// <summary>
        /// A reference to Camera Preview texture. 
        /// </summary>
        protected RenderTexture previewTexture;

        /// <summary>
        /// Connection status of the camera.
        /// </summary>
        protected bool cameraConnectionEstablished = false;

        /// <summary>
        /// Timestamp of previously captured frame. needed for FPS calculations.
        /// </summary>
        protected long previousCaptureTimestamp;

        /// <summary>
        /// Calculated FPS based on recevied frames interval.
        /// </summary>
        protected float currentFPS;

        /// <summary>
        /// Capture status of the camera.
        /// </summary>
        protected bool isCapturingVideo = false;

        /// <summary>
        /// Gets a value indicating whether the camera preview is enabled and textures are being rendered.
        /// </summary>
        protected bool isCapturingPreview = false;

        /// <summary>
        /// The preview texture for the camera captures.
        /// </summary>
        protected Texture2D previewTexture2D = null;

        /// <summary>
        /// The resume connect state of the camera.
        /// </summary>
        protected bool resumeConnect = false;

        /// <summary>
        /// The resume preview state of the camera.
        /// </summary>
        protected bool resumePreviewCapture = false;

        /// <summary>
        /// The resume raw video capture state of the camera.
        /// </summary>
        protected bool resumeVideoCapture = false;

        /// <summary>
        /// camera Connect Context
        /// </summary>
        protected MLCamera.ConnectContext cameraConnectContext;

        /// <summary>
        /// Last used camera Capture Config
        /// </summary>
        protected MLCamera.CaptureConfig cameraCaptureConfig;

        /// <summary>
        /// Callback is invoked when a captured raw/compressed video frame buffer is available, invoked on the main thread.
        /// </summary>
        protected event MLCamera.OnCapturedFrameAvailableDelegate OnRawVideoFrameAvailableInternal;

        /// <summary>
        /// Capture status of the camera before application pause.
        /// </summary>
        private bool wasCapturingVideo = false;

        /// <summary>
        /// Gets the texture for the camera preview render.
        /// (null) when preview is not available yet.
        /// </summary>
        private Texture2D PreviewTexture2D
        {
            get => isCapturingPreview ? previewTexture2D : null;

            set => previewTexture2D = value;
        }

        protected byte[][] byteArrays;

        #endregion

        protected MLCameraBase()
        {
            gcHandle = GCHandle.Alloc(this, GCHandleType.Weak);
            Handle = Native.MagicLeapNativeBindings.InvalidHandle;
            byteArrays = new byte[MLCamera.NativeBindings.MLCameraMaxImagePlanes][];
            OnRawVideoFrameAvailableInternal += HandleOnRawVideoFrameAvailableInternal;
        }

        /// <summary>
        /// Create a preview texture.
        /// </summary>
        protected void CreatePreviewTexture()
        {
            int width = cameraCaptureConfig.StreamConfigs[0].Width;
            int height = cameraCaptureConfig.StreamConfigs[0].Height;

            ClearPreviewTexture();
            previewTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);

            // preview rendering not supported under Magic Leap App Simulator
#if !UNITY_EDITOR
            previewRenderer?.Cleanup();
            previewRenderer = new MLCamera.Renderer();
            previewRenderer.SetRenderBuffer(previewTexture);
#endif
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
        protected void GLPluginEvent()
        {
            // preview rendering not supported under Magic Leap App Simulator
#if !UNITY_EDITOR
            previewRenderer?.Render();
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
        private MLResult.Code DisconnectNative(bool isAsync = false)
        {
            var resultCode = MLCamera.NativeBindings.MLCameraDisconnect(Handle);

            // preview rendering not supported under Magic Leap App Simulator
            if (isAsync)
            {
                MLThreadDispatch.ScheduleMain(() => previewRenderer?.Cleanup());
            }
            else
            {
                previewRenderer?.Cleanup();
            }
            MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraDisconnect));

            return resultCode;
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
        protected static MLResult InternalInitialize()
        {
            MLResult.Code resultCode = MLResult.Code.Ok;

            if (!cameraInited)
            {
                MLCamera.NativeBindings.MLCameraDeviceAvailabilityStatusCallbacks callbacks =
                    MLCamera.NativeBindings.MLCameraDeviceAvailabilityStatusCallbacks.Create();
                resultCode = MLCamera.NativeBindings.MLCameraInit(ref callbacks, IntPtr.Zero);
                cameraInited = MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraInit));
            }

            return MLResult.Create(resultCode);
        }
        protected static MLResult InternalUninitialize()
        {
            if (!cameraInited)  // already uninited
                return MLResult.Create(MLResult.Code.Ok);

            cameraInited = false;
            MLCamera.NativeBindings.MLCameraDeviceAvailabilityStatusCallbacks callbacks =
                MLCamera.NativeBindings.MLCameraDeviceAvailabilityStatusCallbacks.CreateUninitialized();
            var resultCode = MLCamera.NativeBindings.MLCameraDeInit();
            MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraDeInit));
            return MLResult.Create(resultCode);
        }

        protected static MLResult.Code InternalCheckCameraPermission()
        {
            var check = MLPermissions.CheckPermission(MLPermission.Camera);
            if (!MLResult.DidNativeCallSucceed(check.Result, nameof(MLPermissions.CheckPermission)))
            {
                return MLResult.Code.PermissionDenied;
            }
            return MLResult.Code.Ok;
        }

        protected MLResult.Code InternalConnect(MLCamera.ConnectContext cameraConnectContext)
        {
            this.cameraConnectContext = cameraConnectContext;

            MLCamera.NativeBindings.MLCameraConnectContext context = MLCamera.NativeBindings.MLCameraConnectContext.Create(cameraConnectContext);

            MLResult.Code resultCode = MLResult.Code.Ok;


            if (!cameraConnectionEstablished)
            {
                resultCode = MLCamera.NativeBindings.MLCameraConnect(ref context, out Handle);

                if (!MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraConnect)))
                {
                    Debug.LogErrorFormat("MLCamera.Connect failed connecting to the camera. Reason: {0}", resultCode);
                    MLCamera.connectPerfMarker.End();
                    return resultCode;
                }

                MLCamera.NativeBindings.MLCameraCaptureCallbacks captureCallbacks = MLCamera.NativeBindings.MLCameraCaptureCallbacks.Create();

                if (!MLResult.DidNativeCallSucceed(MLCamera.NativeBindings.MLCameraSetCaptureCallbacks(Handle, ref captureCallbacks, GCHandle.ToIntPtr(gcHandle)), nameof(MLCamera.NativeBindings.MLCameraSetCaptureCallbacks)))
                {
                    MLPluginLog.ErrorFormat("MLCamera.CaptureCallbacks failed seting the capture callbacks. Reason: {0}", resultCode);
                    MLCamera.connectPerfMarker.End();
                    return resultCode;
                }

                MLCamera.NativeBindings.MLCameraDeviceStatusCallbacks deviceCallbacks = MLCamera.NativeBindings.MLCameraDeviceStatusCallbacks.Create();

                if (!MLResult.DidNativeCallSucceed(MLCamera.NativeBindings.MLCameraSetDeviceStatusCallbacks(Handle, ref deviceCallbacks, GCHandle.ToIntPtr(gcHandle)), nameof(MLCamera.NativeBindings.MLCameraSetDeviceStatusCallbacks)))
                {
                    MLPluginLog.ErrorFormat("MLCamera.DeviceStatusCallbacks failed setting the device callbacks. Reason: {0}", resultCode);
                    MLCamera.connectPerfMarker.End();
                    return resultCode;
                }

                cameraConnectionEstablished = MLResult.IsOK(resultCode);
            }

            if (!MLResult.IsOK(resultCode))
            {
                MLPluginLog.ErrorFormat("MLCamera.InternalConnect failed to populate characteristics for MLCamera. Reason: {0}", resultCode);
                MLCamera.connectPerfMarker.End();
                return resultCode;
            }

            MLCamera.connectPerfMarker.End();
            return resultCode;
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
        protected MLResult.Code InternalDisconnect(bool flagsOnly = false, bool isAsync = false)
        {
            MLCamera.disconnectPerfMarker.Begin();

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

                MLResult.Code result = DisconnectNative(isAsync);

                // Assume that connection is no longer established
                // even if there is some failure disconnecting.
                this.cameraConnectionEstablished = false;
            }

            ReleaseCaptureBuffers();
            MLCamera.disconnectPerfMarker.Begin();

            return MLResult.Code.Ok;
        }

        protected MLResult InternalGetStreamCapabilities(out MLCamera.StreamCapabilitiesInfo[] streamCapabilities)
        {
            streamCapabilities = null;

            var resultCode = MLCamera.NativeBindings.MLCameraGetNumSupportedStreams(Handle, out uint supportedStreams);
            if (!MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraGetNumSupportedStreams)))
            {
                return MLResult.Create(resultCode);
            }

            streamCapabilities = new MLCamera.StreamCapabilitiesInfo[supportedStreams];
            int sizeOfStruct = Marshal.SizeOf(typeof(MLCamera.NativeBindings.MLCameraCaptureStreamCaps));

            for (uint i = 0; i < supportedStreams; i++)
            {
                streamCapabilities[i] = new MLCamera.StreamCapabilitiesInfo();
                uint streamCapsMax = 0;

                resultCode = MLCamera.NativeBindings.MLCameraGetStreamCaps(Handle, i, ref streamCapsMax, IntPtr.Zero);
                if (!MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraGetStreamCaps)))
                {
                    return MLResult.Create(resultCode);
                }

                streamCapabilities[i].StreamCapabilities = new MLCamera.StreamCapability[streamCapsMax];
                IntPtr pointer = Marshal.AllocHGlobal((int)streamCapsMax *
                    Marshal.SizeOf(typeof(MLCamera.NativeBindings.MLCameraCaptureStreamCaps)));
                resultCode = MLCamera.NativeBindings.MLCameraGetStreamCaps(Handle, i, ref streamCapsMax, pointer);
                if (!MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraGetStreamCaps)))
                {
                    Marshal.FreeHGlobal(pointer);
                    return MLResult.Create(resultCode);
                }

                for (int j = 0; j < streamCapsMax; j++)
                {
                    MLCamera.NativeBindings.MLCameraCaptureStreamCaps nativeStreamCap =
                        Marshal.PtrToStructure<MLCamera.NativeBindings.MLCameraCaptureStreamCaps>(pointer);

                    streamCapabilities[i].StreamCapabilities[j] = new MLCamera.StreamCapability()
                    {
                        Width = nativeStreamCap.Width,
                        Height = nativeStreamCap.Height,
                        CaptureType = nativeStreamCap.CaptureType
                    };

                    pointer += sizeOfStruct;
                }
            }

            return MLResult.Create(resultCode);
        }

        protected MLResult.Code InternalPrepareCapture(MLCamera.CaptureConfig captureConfig, out MLCamera.Metadata cameraMetadata)
        {
            cameraCaptureConfig = captureConfig;
            cameraMetadata = null;

            MLResult.Code resultCode = MLResult.Code.Ok;
            MLCamera.NativeBindings.MLCameraCaptureConfig nativeCaptureConfig = MLCamera.NativeBindings.MLCameraCaptureConfig.Create(captureConfig);
            resultCode = MLCamera.NativeBindings.MLCameraPrepareCapture(Handle, ref nativeCaptureConfig, out ulong metadataHandle);
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraPrepareCapture)))
            {
                cameraMetadata = new MLCamera.Metadata(metadataHandle);
            }
            return resultCode;
        }

        /// <summary>
        /// Handles the event of a new image getting captured.
        /// </summary>
        /// <param name="imageData">The raw data of the image.</param>
        protected void HandleOnRawVideoFrameAvailableInternal(MLCamera.CameraOutput capturedFrame, MLCamera.ResultExtras resultExtras, MLCamera.Metadata metadataHandle)
        {
            if ((isCapturingVideo/* || isCapturingPreview*/) && (!wasCapturingVideo/* && !wasCapturingPreview*/))
            {
                OnRawVideoFrameAvailable?.Invoke(capturedFrame, resultExtras, metadataHandle);
            }
        }
    }
}
