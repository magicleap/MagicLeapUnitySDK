// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using MagicLeap.OpenXR.LegacySupport;

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// MLCamera class exposes static functions to query camera related
    /// functions. Most functions are currently a direct pass through functions to the
    /// native C-API functions and incur no overhead.
    /// </summary>
    [RequireXRLoader]
    public sealed partial class MLCVCamera : MLAutoAPISingleton<MLCVCamera>
    {
        /// <summary>
        /// API status before application pause.
        /// </summary>
        private bool WasStarted = false;

        /// <summary>
        /// Get transform between world origin and the camera. This method relies on a camera timestamp
        /// that is normally acquired from the MLCameraResultExtras structure, therefore this method is
        /// best used within a capture callback to maintain as much accuracy as possible.
        /// Requires ComputerVision permission.
        /// </summary>tran
        /// <param name="vcamTimestamp">Time in nanoseconds to request the transform.</param>
        /// <param name="outTransform">Output transformation matrix on success.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if outTransform parameter was not valid (null).
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to obtain transform due to internal error.
        /// </returns>
        public static MLResult GetFramePose(MLTime vcamTimestamp, out Matrix4x4 outTransform)
        {
            MLResult result;

            getFramePosePerfMarker.Begin();
            result = Instance.InternalGetFramePose(NativeBindings.CameraID.ColorCamera, vcamTimestamp, out outTransform);
            getFramePosePerfMarker.End();

            return result;
        }

        protected override MLResult.Code StartAPI()
        {
            if (!MLDevice.IsReady())
            {
                MLPluginLog.WarningFormat("MLCamera API is attempting to start before the MagicLeap XR Loader has been initialiazed, this could cause issues with MLCVCamera features. If your application needs these features please wait to start API until Monobehavior.Start and if issue persists make sure ProjectSettings/XR/Initialize On Startup is enabled.");
            }

            nativeMLCVCameraTrackingCreatePerfMarker.Begin();
            MLResult.Code code = NativeBindings.MLCVCameraTrackingCreate(ref Handle);
            MLResult.DidNativeCallSucceed(code, nameof(NativeBindings.MLCVCameraTrackingCreate));
            nativeMLCVCameraTrackingCreatePerfMarker.End();

            return code;
        }

        protected override MLResult.Code StopAPI()
        {
            nativeMLCVCameraTrackingDestroyPerfMarker.Begin();
            MLResult.Code code = NativeBindings.MLCVCameraTrackingDestroy(Handle);
            nativeMLCVCameraTrackingDestroyPerfMarker.End();

            return code;
        }

        /// <summary>
        /// Get the frame pose.
        /// </summary>
        /// <param name="cameraId">The camera id.</param>
        /// <param name="vcamTimestamp">The timestamp of the frame pose.</param>
        /// <param name="outTransform">The transform of the frame pose.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        private MLResult InternalGetFramePose(NativeBindings.CameraID cameraId, MLTime vcamTimestamp, out Matrix4x4 outTransform)
        {
            if ((MLDevice.IsMagicLeapLoaderActive() && !MagicLeapXrProviderNativeBindings.IsHeadTrackingAvailable()) ||
                    (MLDevice.IsOpenXRLoaderActive() && !MLHeadTracking.IsAvailable()))
            {
                outTransform = default;
                return MLResult.Create(MLResult.Code.PoseNotFound, "HeadTracking is not available");
            }
            MagicLeapNativeBindings.MLTransform outInternalTransform = new MagicLeapNativeBindings.MLTransform();
            MLResult.Code resultCode = NativeBindings.MLCVCameraGetFramePose(Handle, GetHeadTrackerHandle(), cameraId, vcamTimestamp.Value, ref outInternalTransform);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLCVCameraGetFramePose));
            MLResult poseResult = MLResult.Create(resultCode);
            if (!poseResult.IsOk)
            {
                MLPluginLog.ErrorFormat("MLCamera.InternalGetFramePose failed to get camera frame pose. Reason: {0}", poseResult);
                outTransform = new Matrix4x4();
            }
            else
            {
                outTransform = MLConvert.ToUnity(outInternalTransform);
            }

            return poseResult;
        }

        private ulong GetHeadTrackerHandle()
        {
            if (MLDevice.IsMagicLeapLoaderActive())
                return MagicLeapXrProviderNativeBindings.GetHeadTrackerHandle();
            else
                return MLHeadTracking.Handle;
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
            if (IsStarted)
            {
                MLResult.Code result = StopAPI();
                if (result == MLResult.Code.Ok)
                {
                    WasStarted = true;
                }
            }
        }

        private void HandleApplicationUnpause()
        {
            if (WasStarted)
            {
                MLResult.Code result = StartAPI();
                if (result == MLResult.Code.Ok)
                {
                    WasStarted = false;
                }
            }
        }
    }
}
