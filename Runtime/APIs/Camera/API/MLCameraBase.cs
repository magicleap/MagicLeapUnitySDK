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
    /// <summary>
    /// MLCamera class exposes static functions to query camera related
    /// functions. Most functions are currently a direct pass through functions to the
    /// native C-API functions and incur no overhead.
    /// </summary>
    public partial class MLCameraBase : MLAPIBase
    {
        #region properties

        /// <summary>
        /// Connection status of the camera.
        /// </summary>
        public bool ConnectionEstablished => cameraConnectionEstablished;

        /// <summary>
        /// A reference to Camera Preview texture. 
        /// </summary>
        public RenderTexture PreviewTexture => previewTexture;

        public float CurrentFPS => currentFPS;

        /// <summary>
        /// The connection context used to create this MLCamera object.
        /// </summary>
        public MLCamera.ConnectContext ConnectionContext => cameraConnectContext;

        #endregion

        public static MLCameraBase Create() => new MLCameraBase();

        public void Cleanup()
        {
            gcHandle.Free();
            InternalDisconnect();
            ClearPreviewTexture();
        }


        public void ClearPreviewTexture()
        {
            if (previewTexture != null)
            {
                GameObject.Destroy(previewTexture);
                previewTexture = null;
            }
        }


        /// <summary>
        /// Start preview provide raw frames through callback
        /// </summary>
        public MLResult CapturePreviewStart()
        {
            CreatePreviewTexture();
            MLResult.Code resultCode = MLCamera.NativeBindings.MLCameraCapturePreviewStart(Handle);
            isCapturingPreview = MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraCapturePreviewStart));
            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Stop preview.
        /// </summary>
        /// <returns></returns>
        public MLResult CapturePreviewStop()
        {
            MLResult.Code resultCode = MLCameraBase.NativeBindings.MLCameraCapturePreviewStop(Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraCapturePreviewStop));
            isCapturingPreview = false;
            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Check availablity of a given camera device.
        /// MLCamera instance should only be created & connected when the device is available.
        /// </summary>
        /// <param name="camId">Camera id to check availability for</param>
        /// <param name="deviceAvailable">True if device is available, false otherwise</param>
        /// <returns></returns>
        public static MLResult GetDeviceAvailabilityStatus(MLCamera.Identifier camId, out bool deviceAvailable)
        {
            if (!cameraInited)
            {
                InternalInitialize();
            }

            var resultCode = MLCamera.NativeBindings.MLCameraGetDeviceAvailabilityStatus(camId, out deviceAvailable);
            MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraGetDeviceAvailabilityStatus));
            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Start video capture. capture either encoded video or YUV/RGBA frames.
        /// YUV/RGBA frames are provides through callback. For encoded video capture
        /// valid MediaRecorder object should be configured in the PrepareCapture()
        /// call. The captured video YUV/RGBA frames will be returned via the
        /// OnRawVideoFrameAvailable and OnRawVideoFrameAvailable_NativeCallbackThread
        /// events.
        /// </summary>
        public MLResult CaptureVideoStart()
        {
            MLResult.Code resultCode = MLCamera.NativeBindings.MLCameraCaptureVideoStart(Handle);
            isCapturingVideo = MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraCaptureVideoStart));
            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Stop video capture.
        /// Should only be called at least 500ms after a CaptureVideoStart().
        /// </summary>
        public MLResult CaptureVideoStop()
        {
            MLResult.Code resultCode = MLCamera.NativeBindings.MLCameraCaptureVideoStop(Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraCaptureVideoStop));
            isCapturingVideo = false;
            return MLResult.Create(resultCode);
        }


        /// <summary>
        /// Get the <see cref="StreamCapabilitiesInfo">StreamCapabilitiesInfo</see> for the connected camera, which contains the 
        /// <see cref="StreamCapability">stream capabilities</see> for each supported capture stream.
        /// </summary>
        /// <remarks>
        /// One camera can support multiple capture streams and each stream has it's own set of
        /// <see cref="StreamCapability">StreamCapabilities</see> (capture types and resolutions).
        /// </remarks>
        /// <param name="streamCapabilities">Each item in the array refers to a possible capture stream.
        /// Every index within <see cref="StreamCapabilitiesInfo.StreamCapabilities">StreamCapabilitiesInfo.StreamCapabilities</see>
        /// represents a StreamCapability of that particular stream.</param>
        public MLResult GetStreamCapabilities(out MLCamera.StreamCapabilitiesInfo[] streamCapabilities) => InternalGetStreamCapabilities(out streamCapabilities);

        /// <summary>
        /// Disconnect the current camera.
        /// Should call MLCamera.Connect() again on this instance or create a new
        /// MLCamera object using MLCamera.CreateAndConnect() function.
        /// </summary>
        public MLResult Disconnect() => MLResult.Create(InternalDisconnect());

        public MLResult Connect(MLCamera.ConnectContext connectContext) => MLResult.Create(InternalConnect(connectContext));

        public static MLResult Uninitialize() => InternalUninitialize();

        /// <summary>
        /// Prepare for capture.
        /// </summary>
        /// <param name="captureConfig">Capture configuration.</param>
        /// <param name="cameraMetadata">Camera metadata object to configure the camera for captures.</param>
        public MLResult PrepareCapture(MLCamera.CaptureConfig captureConfig, out MLCamera.Metadata cameraMetadata) => MLResult.Create(InternalPrepareCapture(captureConfig, out cameraMetadata));

        /// <summary>
        /// Trigger AEAWB Convergence.
        /// This API triggers AEAWB sequence and returns when AEAWB convergence has been
        /// achieved. The users can either trigger and converge AEAWB metadata settings
        /// or use this API.
        /// </summary>
        public MLResult PreCaptureAEAWB()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLResult.Code resultCode = MLCamera.NativeBindings.MLCameraPreCaptureAEAWB(Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraPreCaptureAEAWB));
            return MLResult.Create(resultCode);
#else
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif
        }

        /// <summary>
        /// Update capture setting
        /// This API updates the capture settings for an active preview/video stream.
        /// The API can be used to update capture settings like exposure compensation
        /// manual exposure settings like exposure time, iso setting. The capture settings
        /// can be changed in MLCamera.Metadata object returned by MLCamera.PrepareCapture()
        /// function and then this API can be called to apply the capture settings.
        /// </summary>
        public MLResult UpdateCaptureSettings()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLResult.Code resultCode = MLCamera.NativeBindings.MLCameraUpdateCaptureSettings(Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraUpdateCaptureSettings));
            return MLResult.Create(resultCode);
#else
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif        
        }

        /// <summary>
        /// Capture still image.
        /// Result will be availble via the MLCamera.OnRawImageAvailable event.
        /// </summary>
        /// <param name="numImages">no of images to capture valid range is 1-10</param>
        public MLResult CaptureImage(uint numImages = 1)
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLResult.Code resultCode = MLCamera.NativeBindings.MLCameraCaptureImage(Handle, numImages);
            MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraCaptureImage));
            return MLResult.Create(resultCode);

#else
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif
        }

        /// <summary>
        /// Poll camera device status.
        /// Use <see cref="DeviceStatusFlag"/> to view specific status bit.
        /// Call <see cref="GetErrorCode(out ErrorType)"/> to obtain the error code if
        /// <see cref="DeviceStatusFlag.Error"/> bit is set.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public MLResult GetDeviceStatus(out MLCamera.DeviceStatusFlag status)
        {
            status = MLCamera.DeviceStatusFlag.Error;
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLResult.Code resultCode = MLCamera.NativeBindings.MLCameraGetDeviceStatus(Handle, out uint statusFlags);
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraGetDeviceStatus)))
            {
                status = (MLCamera.DeviceStatusFlag)statusFlags;
            }
            return MLResult.Create(resultCode);
#else
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif
        }

        /// <summary>
        /// Obtain device error code.
        /// </summary>
        /// <param name="error">The <see cref="ErrorType"/> which triggered <see cref="DeviceStatusFlag.Error"/></param>
        /// <returns></returns>
        public MLResult GetErrorCode(out MLCamera.ErrorType error)
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLResult.Code resultCode = MLCamera.NativeBindings.MLCameraGetErrorCode(Handle, out error);
            MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraGetErrorCode));
            return MLResult.Create(resultCode);
#else
            error = ErrorType.None;
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif
        }

        /// <summary>
        /// Provides an MLCamera.Metadata object to configure the camera settings.
        /// </summary>
        public MLResult GetCameraCharacteristics(out MLCamera.Metadata cameraMetadata)
        {
            cameraMetadata = null;
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLResult.Code resultCode = MLCamera.NativeBindings.MLCameraGetCameraCharacteristics(Handle, out ulong metadataHandle);
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(MLCamera.NativeBindings.MLCameraGetErrorCode)))
            {
                cameraMetadata = new MLCamera.Metadata(metadataHandle);
            }
            return MLResult.Create(resultCode);
#else
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif
        }

        protected override void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                HandleApplicationPause();
            }
            else
            {
                HandleApplicationUnpause();
            }
        }

        private void HandleApplicationPause()
        {
            if (isCapturingVideo)
            {
                MLResult result = CaptureVideoStop();
                if (result.IsOk)
                {
                    wasCapturingVideo = true;
                }
            }
        }

        private void HandleApplicationUnpause()
        {
            if (wasCapturingVideo)
            {
                MLResult result = CaptureVideoStart();
                if (result.IsOk)
                {
                    wasCapturingVideo = false;
                }
            }
        }
    }
}
