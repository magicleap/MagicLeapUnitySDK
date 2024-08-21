// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%


namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Native;

    /// <summary>
    /// Provides a multithreadig implementation of the camera api.
    /// Handles pause behavior automatically.
    /// </summary>
    public sealed partial class MLCamera : MLCameraBase
    {
        /// <summary>
        /// Event that gets fired when the camera is paused
        /// </summary>
        public event Action OnCameraPaused;
        
        /// <summary>
        /// Event that gets fired when the camera is resumed after a pause
        /// </summary>
        public event Action OnCameraResumed;
        
        /// <summary>
        /// Whether the camera is currently paused
        /// </summary>
        public bool IsPaused { get; private set; }
        
        private static int instanceCounter = 0;

        private MLCamera() : base() => Interlocked.Increment(ref instanceCounter);

        ~MLCamera()
        {
            Cleanup();
            cameraDisconnectWaitHandle.WaitOne();
            if (Interlocked.Decrement(ref instanceCounter) == 0)
                InternalUninitialize();
        }

        new public static MLCamera Create() => new MLCamera();

        /// <summary>
        /// Wait handle used to syncronize MLCamera.Stop() and any ongoing disconnect request.
        /// </summary>
        private AutoResetEvent cameraDisconnectWaitHandle = new AutoResetEvent(true);

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

        /// <summary>
        /// Creates an MLCamera instance and connects the camera device. Will return null if
        /// the <see cref="ConnectContext">ConnectContext</see> is incorrectly configured.
        /// For example, if the context's <see cref="ConnectContext.Flags">Flags</see> are set to
        /// <see cref="ConnectFlag">MLCamera.ConnectFlag.VirtualOnly</see> when targeting the 
        /// <see cref="Identifier.CV">CV camera</see>.
        /// </summary>
        /// <param name="connectContext">Connection settings</param>
        /// <returns>MLCamera instance if connection was successful, null otherwise</returns>
        public static MLCamera CreateAndConnect(ConnectContext connectContext)
        {
            var camera = MLCamera.Create();
            return camera.InternalConnect(connectContext) == MLResult.Code.Ok ? camera : null;
        }


        /// <summary>
        /// Asynchronously creates an MLCamera instance and connects the camera device. Will return null if
        /// the <see cref="ConnectContext">ConnectContext</see> is incorrectly configured.
        /// For example, if the context's <see cref="ConnectContext.Flags">Flags</see> are set to
        /// <see cref="ConnectFlag">MLCamera.ConnectFlag.VirtualOnly</see> when targeting the 
        /// <see cref="Identifier.CV">CV camera</see>.
        /// </summary>
        /// <param name="connectContext">Connection settings</param>
        /// <returns>MLCamera instance if connection was successful, null otherwise</returns>
        public static Task<MLCamera> CreateAndConnectAsync(ConnectContext connectContext)
        {
            if (InternalCheckCameraPermission() == MLResult.Code.Ok)
            {
                var camera = new MLCamera()
                {
#if !UNITY_EDITOR   // preview rendering not supported under Magic Leap App Simulator
                    previewRenderer = new Renderer()
#endif
                };
                return Task.Run(() =>
                {
                    var resultCode = camera.InternalConnect(connectContext);
                    return resultCode == MLResult.Code.Ok ? camera : null;
                });
            }
            else
            {
                return new Task<MLCamera>(() => { return null; });
            }
        }

        /// <summary>
        /// Asynchronously isconnect the camera.
        /// Should call MLCamera.Connect() again on this instance or create a new
        /// MLCamera object using MLCamera.CreateAndConnect() function.
        /// </summary>
        public Task<MLResult> DisconnectAsync()
        {
            return Task.Run(() =>
            {
                var resultCode = InternalDisconnect(isAsync: true);
                return MLResult.Create(resultCode);
            });
        }

        /// <summary>
        /// Trigger asynchronous AEAWB Convergence.
        /// This API triggers AEAWB sequence and completes when AEAWB convergence has been
        /// achieved. The users can either trigger and converge AEAWB metadata settings
        /// or use this API.
        /// </summary>
        public Task<MLResult> PreCaptureAEAWBAsync()
        {
            return Task.Run(() =>
            {
                lock (apiLock)
                {
                    MLResult.Code resultCode = NativeBindings.MLCameraPreCaptureAEAWB(Handle);
                    MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraPreCaptureAEAWB));
                    return MLResult.Create(resultCode);
                }
            });
        }

        /// <summary>
        /// Asynchronously start video capture of either encoded video or YUV/RGBA frames.
        /// YUV/RGBA frames are provides through callback. For encoded video capture
        /// valid MediaRecorder object should be configured in the <see cref="PrepareCapture(CaptureConfig, out Metadata)"/>
        /// call. The captured video YUV/RGBA frames will be returned via the
        /// OnRawVideoFrameAvailable and OnRawVideoFrameAvailable_NativeCallbackThread
        /// events.
        /// </summary>
        public Task<MLResult> CaptureVideoStartAsync()
        {
            return Task.Run(() =>
            {
                MLResult.Code resultCode = MLResult.Code.Ok;
                lock (apiLock)
                {
                    if (!isCapturingVideo)
                    {
                        resultCode = NativeBindings.MLCameraCaptureVideoStart(Handle);
                        isCapturingVideo = MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraCaptureVideoStart));
                    }
                }
                return MLResult.Create(resultCode);
            });
        }

        /// <summary>
        /// Asynchronously stop video capture.
        /// Should only be called at least 500ms after <see cref="CaptureVideoStart"/> or <see cref="CaptureVideoStartAsync"/>.
        /// </summary>
        public Task<MLResult> CaptureVideoStopAsync()
        {
            return Task.Run(() =>
            {
                MLResult.Code resultCode = MLResult.Code.Ok;
                lock (apiLock)
                {
                    if (isCapturingVideo)
                    {
                        resultCode = NativeBindings.MLCameraCaptureVideoStop(Handle);
                        MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraCaptureVideoStop));
                        isCapturingVideo = false;
                    }
                }
                return MLResult.Create(resultCode);
            });
        }

        /// <summary>
        /// Asynchronously start preview capture
        /// The captured preview YUV frames will be returned to the application via <see cref="OnPreviewCaptureCompleted"/>.
        /// Use <see cref="CapturePreviewStop"/> or <see cref="CapturePreviewStopAsync"/> to stop the capture.
        /// </summary>
        public Task<MLResult> CapturePreviewStartAsync()
        {
            // this has to be called on main thread
            MLThreadDispatch.ScheduleMain(CreatePreviewTexture);

            return Task.Run(() =>
            {
                MLResult.Code resultCode = MLResult.Code.Ok;
                lock (apiLock)
                {
                    if (!isCapturingPreview)
                    {
                        resultCode = NativeBindings.MLCameraCapturePreviewStart(Handle);
                        isCapturingPreview = MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraCapturePreviewStart));
                    }
                }
                return MLResult.Create(resultCode);
            });
        }

        /// <summary>
        /// Asynchronously stop preview capture.
        /// </summary>
        public Task<MLResult> CapturePreviewStopAsync()
        {
            return Task.Run(() =>
            {
                MLResult.Code resultCode = MLResult.Code.Ok;
                lock (apiLock)
                {
                    if (isCapturingPreview)
                    {
                        resultCode = NativeBindings.MLCameraCapturePreviewStop(Handle);
                        MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraCapturePreviewStop));
                        isCapturingPreview = false;
                    }

                    MLThreadDispatch.ScheduleMain(ClearPreviewTexture);
                }
                return MLResult.Create(resultCode);
            });
        }

        /// <summary>
        /// Asynchronously capture still image.
        /// Result will be availble via the MLCamera.OnRawImageAvailable event.
        /// </summary>
        /// <param name="numImages">Number of images to capture. Valid range is 1-10</param>
        public Task<MLResult> CaptureImageAsync(uint numImages = 1)
        {
            return Task.Run(() =>
            {
                MLResult.Code resultCode = MLResult.Code.Ok;
                lock (apiLock)
                {
                    resultCode = NativeBindings.MLCameraCaptureImage(Handle, numImages);
                    MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraCaptureImage));
                }
                return MLResult.Create(resultCode);
            });
        }


        protected override void OnApplicationPause(bool pauseStatus)
        {
            applicationPausePerfMarker.Begin();
            MLResult.Code result = MLResult.Code.Ok;
            if (pauseStatus)
            {
                result = Pause();
            }
            else
            {
                Resume();
            }
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
        private MLResult.Code Pause(bool flagsOnly = false)
        {
            OnCameraPaused?.Invoke();
            IsPaused = true;
            MLResult.Code result = MLResult.Code.Ok;

            if (cameraConnectionEstablished)
            {
                resumeConnect = true;
                resumePreviewCapture = isCapturingPreview;
                resumeVideoCapture = isCapturingVideo;

                result = InternalDisconnect(flagsOnly);
                if (!MLResult.IsOK(result))
                {
                    return result;
                }
            }

            return result;
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
        private MLResult.Code Resume()
        {
            MLResult.Code resultCode = MLResult.Code.Ok;

            if (resumeConnect)
            {
                if (InternalCheckCameraPermission() != MLResult.Code.Ok)
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

                    PreCaptureAEAWB();

                    resultCode = CapturePreviewStart().Result;

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

                    PreCaptureAEAWB();
                    resultCode = CaptureVideoStart().Result;

                    resumeVideoCapture = false;
                    if (!MLResult.IsOK(MLResult.Code.Ok))
                    {
                        MLPluginLog.ErrorFormat("MLCamera.Resume failed to start camera preview. Reason: {0}", resultCode);
                        return resultCode;
                    }
                }
            }

            IsPaused = false;
            OnCameraResumed?.Invoke();
            return resultCode;
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
    }
}
