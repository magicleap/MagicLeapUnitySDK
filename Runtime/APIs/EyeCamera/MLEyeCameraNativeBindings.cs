// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2022-2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// MLEyeCamera class exposes static functions to query eye camera related
    /// functions. Most functions are currently a direct pass through functions to the
    /// native C-API functions and incur no overhead.
    /// </summary>
    public sealed partial class MLEyeCamera
    {
        /// <summary>
        /// See ml_eye_camera.h for additional comments.
        /// </summary>
        internal partial class NativeBindings : Native.MagicLeapNativeBindings
        {
            /// <summary>
            /// A structure to encapsulate the camera settings. 
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLEyeCameraSettings
            {
                /// <summary>
                /// Version of this structure. 
                /// </summary> 
                public readonly uint Version;

                /// <summary>
                /// Eye cameras that need to be enabled.
                /// See MLEyeCameraIdentifier for more details. 
                /// If you want to request frames from different eye camera then "OR" the cameras of interest to the app.
                /// </summary> 
                public readonly uint Cameras;

                internal MLEyeCameraSettings(uint version, MLEyeCameraIdentifier cameras)
                {
                    Version = version;
                    Cameras = (uint)cameras;
                }
            }

            /// <summary>
            /// A structure to encapsulate per plane info for each camera frame.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLEyeCameraFrameBuffer
            {
                /// <summary>
                /// Width of the output image in pixels.
                /// </summary> 
                public readonly uint Width;

                /// <summary>
                /// Height of the output image in pixels.
                /// </summary> 
                public readonly uint Height;

                /// <summary>
                /// Stride of the output image in bytes.
                /// </summary> 
                public readonly uint Stride;

                /// <summary>
                /// Number of bytes used to represent a pixel.
                /// </summary> 
                public readonly uint BytesPerPixel;

                /// <summary>
                /// Distance between 2 consecutive pixels in bytes. 
                /// </summary> 
                public readonly uint PixelStride;

                /// <summary>
                /// Number of bytes in the image output data. 
                /// </summary> 
                public readonly uint Size;

                /// <summary>
                /// Image data.
                /// </summary> 
                public readonly IntPtr Data;
            }

            /// <summary>
            /// A structure to encapsulate all the eye camera data. 
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLEyeCameraData
            {
                /// <summary>
                /// version contains the version number for this structure.
                /// </summary>
                public readonly uint Version;

                /// <summary>
                /// Number of camera frames.
                /// </summary>
                public readonly byte FrameCount;

                /// <summary>
                /// Camera frame data. The number of frames is specified by FrameCount.
                /// </summary>
                public readonly IntPtr Frames;

                internal MLEyeCameraData(uint version)
                {
                    Version = version;
                    FrameCount = 0;
                    Frames = IntPtr.Zero;
                }
            }

            /// <summary>
            /// A structure to encapsulate output data for each camera sensor.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLEyeCameraFrame
            {
                /// <summary>
                /// Camera Identifier specifies which camera is associated with this frame.
                /// </summary>
                public readonly MLEyeCameraIdentifier CameraID;

                /// <summary>
                /// A 64bit integer to index the frame number associated with this frame.
                /// </summary>
                public readonly long FrameNumber;

                /// <summary>
                /// Frame timestamp specifies the time at which the frame was captured.
                /// </summary>
                public readonly long TimeStamp;

                /// <summary>
                /// Frame buffer data.
                /// </summary>
                public readonly MLEyeCameraFrameBuffer FrameBuffer;
            }

            /// <summary>
            /// Brief connect to eye camera(s).
            /// </summary>
            /// <param name="setting">A pointer to MLEyeCameraSettings structure.</param>
            /// <param name="outHandle">A pointer to camera handle to be used in later APIs.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if connected to camera device(s) successfully.
            /// MLResult.Result will be <c>MLResult.Code.LicenseError</c> if necessary license is missing.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if one of the parameters is invalid.
            /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if the necessary permission is missing.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
            /// </returns>
            /// <remarks>apilevel 26. permissions android.permission.EYE_CAMERA (protection level: dangerous)</remarks>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLEyeCameraConnect(ref MLEyeCameraSettings setting, ref ulong outHandle);

            /// <summary>
            /// Update the eye camera settings.
            /// </summary>
            /// <param name="handle">Camera handle obtained from MLEyeCameraConnect.</param>
            /// <param name="settings">Pointer to MLEyeCameraSettings.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the settings updated successfully.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if one of the parameters is invalid.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed due to an internal error.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLEyeCameraUpdateSettings(ulong handle, ref MLEyeCameraSettings settings);


            /// <summary>
            /// Poll for Frames. Returns MLEyeCameraData with this latest data when available. 
            /// The memory is owned by the system. 
            /// Application should copy the data it needs to cache and release the memory by calling MLEyeCameraReleaseCameraData.
            /// This is a blocking call. API is not thread safe.
            /// If there are no new camera frames within the timeout_ms duration then the API will return MLResult_Timeout.
            /// </summary>
            /// <param name="handle">Camera handle obtained from MLEyeCameraConnect.</param>
            /// <param name="timeoutMS">Timeout in milliseconds.</param>
            /// <param name="outData">Eye camera data. Will be set to NULL if no valid data is available at this time.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if camera frames fetched successfully.
            /// MLResult.Result will be <c>MLResult.Code.Timeout</c> returned because no new frame available at this time.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if one of the parameters is invalid.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed due to an internal error.
            /// </returns>
            /// <remarks>apilevel 26. permissions None.</remarks>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLEyeCameraGetLatestCameraData(ulong handle, UInt64 timeoutMS, ref MLEyeCameraData outData);

            /// <summary>
            /// Releases specified #MLEyeCameraData object. 
            /// This function should be called exactly once for each call to #MLEyeCameraGetLatestCameraData. 
            /// MLEyeCameraData will be over-written when new data is available.
            /// </summary>
            /// <param name="handle">Camera handle obtained from MLEyeCameraConnect.</param>
            /// <param name="eyeCameraData">Pointer to a valid MLEyeCameraData object.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully released eye camera data.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if eyeCameraData parameter was not valid (NULL).
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed due to an internal error.
            /// </returns>
            /// <remarks>permissions None.</remarks>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLEyeCameraReleaseCameraData(ulong handle, ref MLEyeCameraData eyeCameraData);

            /// <summary>
            /// Disconnect from eye camera(s). This will disconnect from all the eye camera(s) currently connected.
            /// </summary>
            /// <param name="handle">Camera handle obtained from MLEyeCameraConnect.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if disconnected camera(s) successfully.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if invalid context.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to disconnect camera(s).
            /// </returns>
            /// <remarks>apilevel 26. permissions None.</remarks>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLEyeCameraDisconnect(ulong handle);
        }
    }
}

