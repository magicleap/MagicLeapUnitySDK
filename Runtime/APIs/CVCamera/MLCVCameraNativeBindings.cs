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
    using System.Runtime.InteropServices;

    /// <summary>
    /// MLCamera class exposes static functions to query camera related
    /// functions. Most functions are currently a direct pass through functions to the
    /// native C-API functions and incur no overhead.
    /// </summary>
    public sealed partial class MLCVCamera
    {
        /// <summary>
        /// See ml_cv_camera.h for additional comments.
        /// </summary>
        private partial class NativeBindings : Native.MagicLeapNativeBindings
        {
            /// <summary>
            /// <c>MLCVCameraIntrinsics_MaxDistortionCoefficients</c> from ml_cv_camera.h
            /// </summary>
            public const uint MaxDistortionCoefficients = 5;

            /// <summary>
            /// MLCVCameraID enum from ml_cv_camera.h
            /// </summary>
            public enum CameraID : uint
            {
                /// <summary>
                /// RGB Camera.
                /// </summary>
                ColorCamera = 0,
            }

            /// <summary>
            /// Create Camera Tracker.
            /// </summary>
            /// <param name="cvCameraHandle">tracker Handle.</param>
            /// <returns>MLResult_Ok On success.
            /// MLResult_PermissionDenied Necessary permission is missing.
            /// MLResult_UnspecifiedFailure Unable to create tracker.</returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLCVCameraTrackingCreate(ref ulong cvCameraHandle);

            /// <summary>
            /// Destroy Tracker after usage.
            /// </summary>
            /// <param name="cvCameraHandle">MLHandle previously created with MLCVCameraTrackingCreate.</param>
            /// <returns>
            /// MLResult_Ok On success.
            /// MLResult_PermissionDenied Necessary permission is missing.
            /// MLResult_UnspecifiedFailure Unable to create tracker.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLCVCameraTrackingDestroy(ulong cvCameraHandle);

            /// <summary>
            /// Get the camera pose in the world coordinate system.
            /// </summary>
            /// <param name="cvCameraHandle">MLHandle previously created with MLCVCameraTrackingCreate.</param>
            /// <param name="headHandle">MLHandle previously created with MLHeadCameraCreate.</param>
            /// <param name="id">The camera id.</param>
            /// <param name="vcamTimestamp">The timestamp of the frame pose.</param>
            /// <param name="outTransform">The transform of the frame pose.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// </returns>
            /// <returns></returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLCVCameraGetFramePose(ulong cvCameraHandle, ulong headHandle, CameraID id, long vcamTimestamp, ref MLTransform outTransform);
        }
    }
}
