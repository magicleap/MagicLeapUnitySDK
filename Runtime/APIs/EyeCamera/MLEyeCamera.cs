// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2022-2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.MagicLeap
{
    public sealed partial class MLEyeCamera : MLAutoAPISingleton<MLEyeCamera>
    {
        /// <summary>
        /// The maximum number of eye camera frames.
        /// </summary>
        public const int MaxFrameCount = 4;

        /// <summary>
        /// The enumerated value representing which eye cameras are currently active.
        /// </summary>
        /// <returns></returns>
        public static MLEyeCameraIdentifier ActiveCameras => (MLEyeCameraIdentifier)Instance.settings.Cameras;

        /// <summary>
        /// The number of active eye cameras.
        /// </summary>
        /// <value></value>
        public static int ActiveCamerasCount
        {
            get
            {
                int count = 0;

                if (ActiveCameras.HasFlag(MLEyeCameraIdentifier.LeftTemple))
                    count++;
                if (ActiveCameras.HasFlag(MLEyeCameraIdentifier.LeftNasal))
                    count++;
                if (ActiveCameras.HasFlag(MLEyeCameraIdentifier.RightNasal))
                    count++;
                if (ActiveCameras.HasFlag(MLEyeCameraIdentifier.RightTemple))
                    count++;

                return count;
            }
        }

        /// <summary>
        /// Converts an image frame data pointer into a byte array.
        /// </summary>
        /// <param name="imageFrame">The associated eye camera frame buffer for an eye camera.</param>
        /// <param name="byteArray">The image data from the native eye camera data as a byte array.</param>
        public static void CopyImageFrameDataToByteArray(EyeCameraFrameBuffer imageFrame, ref byte[] byteArray) => Marshal.Copy(imageFrame.Data, byteArray, 0, byteArray.Length);

        /// <summary>
        /// Polls for Frames. Returns EyeCameraData with this latest data when available.  
        /// If there are no new camera frames within the timeout_ms duration then the API will return MLResult_Timeout.
        /// </summary>
        /// <param name="outData">Eye camera data to be output.</param>
        /// <param name="timeoutMS">Timeout in milliseconds.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if succesfully fetched the camera frames and released the native data.
        /// MLResult.Result will be <c>MLResult.Code.Timeout</c> returned because no new frame available at this time.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if one of the parameters is invalid.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed due to an internal error.
        /// </returns>
        public static MLResult GetLatestCameraData(out EyeCameraData outData, UInt64 timeoutMS) => Instance.InternalMLEyeCameraGetLatestCameraData(out outData, timeoutMS);

        /// <summary>
        /// Update the eye camera settings.
        /// </summary>
        /// <param name="cameras">The specific eye cameras that will be active with the updated settings.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the settings updated successfully.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if one of the parameters is invalid.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed due to an internal error.
        /// </returns>
        public static MLResult UpdateSettings(MLEyeCameraIdentifier cameras) => Instance.InternalMLEyeCameraUpdateSettings(cameras);
    }
}