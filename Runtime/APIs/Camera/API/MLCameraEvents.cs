// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLCameraEvents.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
#if UNITY_MAGICLEAP || UNITY_ANDROID
    using UnityEngine.XR.MagicLeap.Native;

#endif

    /// <summary>
    /// APIs for accessing Camera Device and to do Camera Capture.
    /// </summary>
    public sealed partial class MLCamera
    {
        #region Events
        /// <summary>
        /// Camera status callback, device available.
        /// </summary>
        public static event OnDeviceAvailabilityStatusDelegate OnDeviceAvailable
        {
            add
            {
                internalOnDeviceAvailable += value;
                if (GetDeviceAvailabilitySubscriberCount () > 0)
                {
                    InternalInitialize();
                }
            }

            remove
            {
                internalOnDeviceAvailable -= value;
                if (GetDeviceAvailabilitySubscriberCount() == 0)
                {
                    InternalUninitialize();
                }
            }
        }

        /// <summary>
        /// Camera status callback, device unavailable.
        /// </summary>
        public static event OnDeviceAvailabilityStatusDelegate OnDeviceUnavailable
        {
            add
            {
                internalOnDeviceUnavailable += value;
                if (GetDeviceAvailabilitySubscriberCount() > 0)
                {
                    InternalInitialize();
                }
            }

            remove
            {
                internalOnDeviceUnavailable -= value;
                if (GetDeviceAvailabilitySubscriberCount() == 0)
                {
                    InternalUninitialize();
                }
            }
        }

        /// <summary>
        /// Callback is invoked when the camera stops streaming.
        /// </summary>
        public event OnDeviceStatusDelegate OnDeviceIdle = delegate { };

        /// <summary>
        /// Callback is invoked when the camera is streaming.
        /// </summary>
        public event OnDeviceStatusDelegate OnDeviceStreaming = delegate { };

        /// <summary>
        /// Callback is invoked when the camera is disconnected.
        /// </summary>
        public event OnDeviceDisconnectedDelegate OnDeviceDisconnected = delegate { };

        /// <summary>
        /// Camera status callback, device error.
        /// </summary>
        public event OnDeviceErrorDelegate OnDeviceError = delegate { };

        /// <summary>
        /// Callback is invoked when a capture has failed when the camera device
        /// failed to produce a capture result for the request.
        /// </summary>
        public event OnCaptureFailedDelegate OnCaptureFailed = delegate { };

        /// <summary>
        /// Callback is invoked when an ongoing video or preview capture or both
        /// are aborteddue to an error.
        /// </summary>
        public event OnCaptureAbortedDelegate OnCaptureAborted = delegate { };

        /// <summary>
        /// Callback is invoked when capturing single frame is completed and result is available.
        /// </summary>
        public event OnCaptureCompletedDelegate OnCaptureCompleted = delegate { };

        /// <summary>
        /// Callback is invoked when a preview video frame buffer is available with MLCamera.CaptureType.Preview.
        /// Not valid for MR/VR Capture since it does not have preview support.
        /// </summary>
        public event OnPreviewCaptureCompletedDelegate OnPreviewCaptureCompleted = delegate { };

        /// <summary>
        /// Callback is invoked when a captured image buffer is available.
        /// </summary>
        public event OnCapturedFrameAvailableDelegate OnRawImageAvailable = delegate { };

        /// <summary>
        /// Callback is invoked when a captured raw/compressed video frame buffer is available, invoked on the main thread.
        /// </summary>
        public event OnCapturedFrameAvailableDelegate OnRawVideoFrameAvailable = delegate { };

        /// <summary>
        /// Camera capture callback, capture raw video frame, invoked on the same thread as the native callback,
        /// allowing the use of the unmanaged native pointer to the frame data memory.
        /// </summary>
        public event OnCapturedFrameAvailableDelegate OnRawVideoFrameAvailable_NativeCallbackThread =
            delegate { };

        private static OnDeviceAvailabilityStatusDelegate internalOnDeviceAvailable = delegate { };
        private static OnDeviceAvailabilityStatusDelegate internalOnDeviceUnavailable = delegate { };

        private static int GetDeviceAvailabilitySubscriberCount()
        {
            return internalOnDeviceAvailable.GetInvocationList().Length + internalOnDeviceUnavailable.GetInvocationList().Length;
        }

        #endregion

        #region Delegates

        /// <summary>
        /// Delegate to notify the app about camera device availability.
        /// </summary>
        /// <param name="camId">Identifier for the camera device which is available.</param>
        public delegate void OnDeviceAvailabilityStatusDelegate(Identifier camId);

        /// <summary>
        /// Delegate to notify the app about camera device status (idle or streaming)
        /// </summary>
        public delegate void OnDeviceStatusDelegate();

        /// <summary>
        /// Delegate to notify the app about camera device error
        /// </summary>
        /// <param name="error">Error code</param>
        public delegate void OnDeviceErrorDelegate(ErrorType error);

        /// <summary>
        /// Delegate to notify the app when the camera device disconnects.
        /// </summary>
        /// <param name="reason">Disconnect reason</param>
        public delegate void OnDeviceDisconnectedDelegate(DisconnectReason reason);

        /// <summary>
        /// Delegate to notify the app when a capture request fails.
        /// </summary>
        public delegate void OnCaptureFailedDelegate(ResultExtras extra);

        /// <summary>
        /// Delegate to notify the app when a capture request is aborted.
        /// </summary>
        public delegate void OnCaptureAbortedDelegate();

        /// <summary>
        /// Delegate to notify the app when a capture request is completed.
        /// </summary>
        public delegate void OnCaptureCompletedDelegate(ulong metadataHandle, ResultExtras extra);
        
        /// <summary>
        /// Delegate to notify the app when the result of a preview capture is available.
        /// </summary>
        /// <param name="result">Result reporting whether the preview capture completed or not.</param>
        public delegate void OnPreviewCaptureCompletedDelegate(MLResult result);

        /// <summary>
        /// Delegate to notify the app when the frame data of a capture is available.
        /// </summary>
        /// <param name="frameInfo">Frame data</param>
        public delegate void OnCapturedFrameAvailableDelegate(CameraOutput frameInfo, ResultExtras resultExtras);

        #endregion
    }
}
