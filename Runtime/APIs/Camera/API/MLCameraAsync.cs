// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2022 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Threading.Tasks;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLCamera
    {
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
                var resultCode = InternalDisconnect();
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
#if UNITY_MAGICLEAP || UNITY_ANDROID
                lock (apiLock)
                {
                    MLResult.Code resultCode = NativeBindings.MLCameraPreCaptureAEAWB(Handle);
                    MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraPreCaptureAEAWB));
                    return MLResult.Create(resultCode);
                }
#else
                return MLResult.Create(MLResult.Code.NotImplemented);
#endif
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
#if UNITY_MAGICLEAP || UNITY_ANDROID
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
#else
                return MLResult.Create(MLResult.Code.NotImplemented);
#endif
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
#if UNITY_MAGICLEAP || UNITY_ANDROID
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
#else
                return MLResult.Create(MLResult.Code.NotImplemented);
#endif
            });
        }

        /// <summary>
        /// Asynchronously start preview capture
        /// The captured preview YUV frames will be returned to the application via <see cref="OnPreviewCaptureCompleted"/>.
        /// Use <see cref="CapturePreviewStop"/> or <see cref="CapturePreviewStopAsync"/> to stop the capture.
        /// </summary>
        public Task<MLResult> CapturePreviewStartAsync()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            // this has to be called on main thread
            CreatePreviewTexture();
#endif
            return Task.Run(() =>
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
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
#else
                return MLResult.Create(MLResult.Code.NotImplemented);
#endif
            });
        }

        /// <summary>
        /// Asynchronously stop preview capture.
        /// </summary>
        public Task<MLResult> CapturePreviewStopAsync()
        {
            return Task.Run(() =>
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
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
#else
                return MLResult.Create(MLResult.Code.NotImplemented);
#endif
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
#if UNITY_MAGICLEAP || UNITY_ANDROID
                MLResult.Code resultCode = MLResult.Code.Ok;
                lock (apiLock)
                {
                    resultCode = NativeBindings.MLCameraCaptureImage(Handle, numImages);
                    MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraCaptureImage));
                }
                return MLResult.Create(resultCode);
#else
                return MLResult.Create(MLResult.Code.NotImplemented);
#endif
            });
        }
    }
}
