// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLCamera.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// APIs for accessing Camera Device and to do Camera Capture.
    /// </summary>
    public sealed partial class MLCamera : MLAPIBase
    {
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
        public ConnectContext ConnectionContext => cameraConnectContext;

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
            MLCamera camera = new MLCamera();
            return camera.InternalConnect(connectContext) == MLResult.Code.Ok ? camera : null;
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
        public MLResult GetStreamCapabilities(out StreamCapabilitiesInfo[] streamCapabilities)
        {
            return InternalGetStreamCapabilities(out streamCapabilities);
        }

        /// <summary>
        /// Disconnect the current camera.
        /// Should call MLCamera.Connect() again on this instance or create a new
        /// MLCamera object using MLCamera.CreateAndConnect() function.
        /// </summary>
        public MLResult Disconnect()
        {
            return MLResult.Create(InternalDisconnect());
        }

        public static MLResult Uninitialize()
        {
            return InternalUninitialize();
        }

        /// <summary>
        /// Prepare for capture.
        /// </summary>
        /// <param name="captureConfig">Capture configuration.</param>
        /// <param name="cameraMetadata">Camera metadata object to configure the camera for captures.</param>
        public MLResult PrepareCapture(CaptureConfig captureConfig, out Metadata cameraMetadata)
        {
            return MLResult.Create(InternalPrepareCapture(captureConfig, out cameraMetadata));
        }

        /// <summary>
        /// Trigger AEAWB Convergence.
        /// This API triggers AEAWB sequence and returns when AEAWB convergence has been
        /// achieved. The users can either trigger and converge AEAWB metadata settings
        /// or use this API.
        /// </summary>
        public MLResult PreCaptureAEAWB()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLResult.Code resultCode = NativeBindings.MLCameraPreCaptureAEAWB(Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraPreCaptureAEAWB));
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
            MLResult.Code resultCode = NativeBindings.MLCameraUpdateCaptureSettings(Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraUpdateCaptureSettings));
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
        public MLResult CaptureImage(uint numImages)
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLResult.Code resultCode = NativeBindings.MLCameraCaptureImage(Handle, numImages);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraCaptureImage));
            return MLResult.Create(resultCode);
#else
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif
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
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLResult.Code resultCode = NativeBindings.MLCameraCaptureVideoStart(Handle);
            isCapturingVideo = MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraCaptureVideoStart));
            return MLResult.Create(resultCode);
#else
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif
        }

        /// <summary>
        /// Stop video capture.
        /// Should only be called at least 500ms after a CaptureVideoStart().
        /// </summary>
        public MLResult CaptureVideoStop()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLResult.Code resultCode = NativeBindings.MLCameraCaptureVideoStop(Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraCaptureVideoStop));
            isCapturingVideo = false;
            return MLResult.Create(resultCode);
#else
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif
        }

        /// <summary>
        /// Start preview provide raw frames through callback
        /// </summary>
        public MLResult CapturePreviewStart()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            CreatePreviewTexture();
            MLResult.Code resultCode = NativeBindings.MLCameraCapturePreviewStart(Handle);
            isCapturingPreview = MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraCapturePreviewStart));
            return MLResult.Create(resultCode);
#else
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif
        }

        /// <summary>
        /// Stop preview.
        /// </summary>
        /// <returns></returns>
        public MLResult CapturePreviewStop()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLResult.Code resultCode = NativeBindings.MLCameraCapturePreviewStop(Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraCapturePreviewStop));
            isCapturingPreview = false;
            return MLResult.Create(resultCode);
#else
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif
        }

        public MLResult GetDeviceStatus(out DeviceStatusFlag status)
        {
            status = DeviceStatusFlag.Error;
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLResult.Code resultCode = NativeBindings.MLCameraGetDeviceStatus(Handle, out uint statusFlags);
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraGetDeviceStatus)))
            {
                status = (DeviceStatusFlag)statusFlags;
            }
            return MLResult.Create(resultCode);
#else
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif
        }

        public MLResult GetErrorCode(out ErrorType error)
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLResult.Code resultCode = NativeBindings.MLCameraGetErrorCode(Handle, out error);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraGetErrorCode));
            return MLResult.Create(resultCode);
#else
            error = ErrorType.None;
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif
        }

        /// <summary>
        /// Provides an MLCamera.Metadata object to configure the camera settings.
        /// </summary>
        public MLResult GetCameraCharacteristics(out Metadata cameraMetadata)
        {
            cameraMetadata = null;
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLResult.Code resultCode = NativeBindings.MLCameraGetCameraCharacteristics(Handle, out ulong metadataHandle);
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCameraGetErrorCode)))
            {
                cameraMetadata = new Metadata(metadataHandle);
            }
            return MLResult.Create(resultCode);
#else
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif
        }

        /// <summary>
        /// Check availablity of a given camera device.
        /// MLCamera instance should only be created & connected when the device is available.
        /// </summary>
        /// <param name="camId">Camera id to check availability for</param>
        /// <param name="deviceAvailable">True if device is available, false otherwise</param>
        /// <returns></returns>
        public static MLResult GetDeviceAvailabilityStatus(Identifier camId, out bool deviceAvailable)
        {
            if (!cameraInited)
            {
                InternalInitialize();
            }

#if UNITY_MAGICLEAP || UNITY_ANDROID
            return MLResult.Create(NativeBindings.MLCameraGetDeviceAvailabilityStatus(camId, out deviceAvailable));
#else
            deviceAvailable = false;
            return MLResult.Create(MLResult.Code.NotImplemented);
#endif
        }
    }
}
