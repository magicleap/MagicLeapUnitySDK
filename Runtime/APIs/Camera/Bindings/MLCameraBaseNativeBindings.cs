// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

// Disable warnings about missing documentation for native interop.
#pragma warning disable 1591

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// MLCamera class exposes static functions to query camera related
    /// functions. Most functions are currently a direct pass through functions to the
    /// native C-API functions and incur no overhead.
    /// </summary>
    public partial class MLCameraBase
    {
        /// <summary>
        /// This class defines the C# interface to the C functions/structures in "ml_camera.h".
        /// </summary>

        internal partial class NativeBindings : Native.MagicLeapNativeBindings
        {
            /// <summary>
            /// Number of MLCamera image planes.
            /// </summary>
            public const int MLCameraMaxImagePlanes = 3;

            /// <summary>
            /// Default distortion vector size.
            /// </summary>
            private const int MLCameraMaxDistortionCoefficients = 5;

            /// <summary>
            /// Max No of streams supported by logical camera 
            /// </summary>
            private const int MLCameraMaxStreams = 2;

            #region V2

            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLCameraInit(ref MLCameraDeviceAvailabilityStatusCallbacks deviceAvailabilityStatusCallback, IntPtr userData);

            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLCameraDeInit();

            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLCameraConnect(ref MLCameraConnectContext inputContext, out ulong handle);

            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLCameraGetNumSupportedStreams(ulong handle, out uint numSupportedStreams);

            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl)]
            internal static extern MLResult.Code MLCameraGetStreamCaps(ulong contextHandle, uint streamIndex, ref uint numStreamCaps, IntPtr streamCaps);

            /// <summary>
            /// Disconnect from camera device.
            /// </summary>
            /// <param name="contextHandle">Capture operation type.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if disconnected from camera device successfully.
            /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if a required permission is missing.
            /// </returns>
            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLCameraDisconnect(ulong contextHandle);

            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl)]
            internal static extern MLResult.Code MLCameraPreCaptureAEAWB(ulong contextHandle);

            /// <summary>
            /// Prepare for capture.
            /// This API prepares capture per specified MLCamera.CaptureType by creating
            /// a capture request, and a handle to which is returned to the user, who can choose
            /// to manipulate the request data(metadata) via APIs defined in ml_camera_metadata.h
            /// before performing the capture.
            /// Shall be called after MLCameraConnect().
            /// </summary>
            /// <param name="contextHandle">Context obtained from MLCameraConnect.</param>
            /// <param name="config">Capture configuration.</param>
            /// <param name="metadataHandle">Handle to the capture metadata. Only valid if result is MLResult_Ok.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if prepared for capture successfully.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to prepare for capture due to an invalid parameter.
            /// MLResult.Result will be <c>MLResult.Code.MediaGenericUnexpectedNull</c> if failed to prepare for capture due to null pointer.
            /// MLResult.Result will be <c>MLResult.Code.BadType</c> if failed to prepare for capture due to null pointer.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if failed to allocate memory.
            /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if a required permission is missing.
            /// </returns>
            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLCameraPrepareCapture(ulong contextHandle, ref MLCameraCaptureConfig config, out ulong metadataHandle);

            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl)]
            internal static extern MLResult.Code MLCameraUpdateCaptureSettings(ulong contextHandle);

            /// <summary>
            /// Set the client-implemented callbacks to convey camera device status.
            /// Client needs to implement the callbacks defined by MLCameraDeviceStatusCallbacks.
            /// The library passes the camera device status to the client via those callbacks.
            /// Shall be called before MLCameraConnect().
            /// </summary>
            /// <param name="deviceStatusCallbacks">Capture status callbacks.</param>
            /// <param name="data">User metadata.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if callbacks were set successfully.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// </returns>
            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLCameraSetDeviceStatusCallbacks(ulong contextHandle, ref MLCameraDeviceStatusCallbacks deviceStatusCallbacks, IntPtr data);

            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl)]
            internal static extern MLResult.Code MLCameraSetCaptureCallbacks(ulong handle, ref MLCameraCaptureCallbacks captureCallbacks, IntPtr data);

            /// <summary>
            /// The output image will be returned in capture callback on_image_buffer_available
            /// </summary>
            /// <param name="contextHandle">Context obtained from MLCameraConnect.</param>
            /// <param name="numImages">no of images to capture valid range is 1-10.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if image was captured successfully.
            /// MLResult.Result will be <c>MLResult.Code.MediaGenericInvalidOperation</c> if failed to capture image due to on-going video recording.
            /// MLResult.Result will be <c>MLResult.Code.MediaGenericUnexpectedNull</c> if failed to capture image due to null pointer.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to capture image due to an invalid parameter.
            /// MLResult.Result will be <c>MLResult.Code.Timeout</c> if failed to capture image due to timeout.
            /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if a required permission is missing.
            /// </returns>
            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern MLResult.Code MLCameraCaptureImage(ulong contextHandle, uint numImages);

            /// <summary>
            /// Start video capture and save output to a file.
            /// <c>The captured video and audio streams will be encoded with AVC and AAC codecs
            /// and packed in mp4 container format and stored into the file specified by the
            /// file path.The library is responsible for opening and closing the file.The
            /// current supported video resolution is 1080p.
            /// If this function is invoked before the camera sensor has locked AE and AWB,
            /// it will be blocked till AE, AWB is locked and then starts to capture.
            /// MLCameraCaptureVideoStop() needs to be called to stop the capture.</c>
            /// </summary>
            /// <param name="path">File path to store the output video.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if started video recording successfully.
            /// MLResult.Result will be <c>MLResult.Code.MediaGenericUnexpectedNull</c> if failed to start video recording due to null pointer.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to start video recording due to invalid input parameter.
            /// MLResult.Result will be <c>MLResult.Code.Timeout</c> if failed to start video recording image due to timeout.
            /// MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if failed to allocate memory.
            /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if a required permission is missing.
            /// </returns>
            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern MLResult.Code MLCameraCaptureVideoStart(ulong contextHandle);

            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl)]
            internal static extern MLResult.Code MLCameraCapturePreviewStart(ulong contextHandle);

            /// <summary>
            /// Stop video capture.
            /// <c>User should allow some time, i.e., >500ms, after MLCameraCaptureVideoStart and before
            /// calling this API, as captured frames are being encoded.Otherwise, MLResult_UnspecifiedFailure
            /// will be returned.</c>
            /// </summary>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if stopped video recording successfully.
            /// MLResult.Result will be <c>MLResult.Code.MediaGenericUnexpectedNull</c> if failed to stop video recording due to null pointer.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if a required permission is missing.
            /// </returns>
            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLCameraCaptureVideoStop(ulong contextHandle);

            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl)]
            internal static extern MLResult.Code MLCameraCapturePreviewStop(ulong contextHandle);

            /// <summary>
            /// Poll camera device status.
            /// Use #MLCamera.DeviceStatusFlag to view specific status bit.
            /// Call MLCameraGetErrorCode() to obtain the error code if
            /// MLCamera.DeviceStatusFlag.Error bit is set.
            /// Note: This API can still be used even if MLCameraSetDeviceStatusCallbacks() has been called.
            /// </summary>
            /// <param name="contextHandle">Context obtained from MLCameraConnect.</param>
            /// <param name="outDeviceStatus">Device status.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if obtained device status successfully.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to obtain device status due to invalid input parameter.
            /// </returns>
            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLCameraGetDeviceStatus(ulong contextHandle, out uint outDeviceStatus);

            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl)]
            internal static extern MLResult.Code MLCameraGetDeviceAvailabilityStatus(MLCamera.Identifier camId, [MarshalAs(UnmanagedType.I1)] out bool deviceAvailabilityStatus);

            /// <summary>
            /// Obtain device error code.
            /// </summary>
            /// <param name="outErrorCode">Device error code.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if obtained device error code successfully.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to obtain device error code due to invalid input parameter.
            /// </returns>
            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLCameraGetErrorCode(ulong contextHandle, out MLCamera.ErrorType outErrorCode);


            /// <summary>
            /// Obtain handle for retrieving camera characteristics.
            /// This API provides the handle for retrieving camera characteristics via APIs
            /// defined in ml_camera_metadata.h.
            /// </summary>
            /// <param name="outHandle">Handle to access camera characteristic metadata.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if obtained camera characteristic handle successfully.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to obtain camera characteristic handle due to invalid input parameter.
            /// MLResult.Result will be <c>MLResult.Code.MediaGenericUnexpectedNull</c> if failed to capture raw image due to null pointer.
            /// MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if failed to allocate memory.
            /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if a required permission is missing.
            /// </returns>
            [DllImport(MLCameraDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLCameraGetCameraCharacteristics(ulong contextHandle, out ulong outMetadataHandle);

            #endregion
        }
    }
}
