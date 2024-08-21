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
    /// <summary>
    /// APIs for accessing Camera Device and to do Camera Capture.
    /// </summary>
    public partial class MLCameraBase
    {
        #region Events
        /// <summary>
        /// Camera status callback, device available.
        /// </summary>
        public static event MLCamera.OnDeviceAvailabilityStatusDelegate OnDeviceAvailable
        {
            add
            {
                internalOnDeviceAvailable += value;
                if (GetDeviceAvailabilitySubscriberCount() > 0)
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
        public static event MLCamera.OnDeviceAvailabilityStatusDelegate OnDeviceUnavailable
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
        public event MLCamera.OnDeviceStatusDelegate OnDeviceIdle;

        /// <summary>
        /// Callback is invoked when the camera is streaming.
        /// </summary>
        public event MLCamera.OnDeviceStatusDelegate OnDeviceStreaming;

        /// <summary>
        /// Callback is invoked when the camera is disconnected.
        /// </summary>
        public event MLCamera.OnDeviceDisconnectedDelegate OnDeviceDisconnected;

        /// <summary>
        /// Camera status callback, device error.
        /// </summary>
        public event MLCamera.OnDeviceErrorDelegate OnDeviceError;

        /// <summary>
        /// Callback is invoked when a capture has failed when the camera device
        /// failed to produce a capture result for the request.
        /// </summary>
        public event MLCamera.OnCaptureFailedDelegate OnCaptureFailed;

        /// <summary>
        /// Callback is invoked when an ongoing video or preview capture or both
        /// are aborteddue to an error.
        /// </summary>
        public event MLCamera.OnCaptureAbortedDelegate OnCaptureAborted;

        /// <summary>
        /// Callback is invoked when capturing single frame is completed and result is available.
        /// </summary>
        public event MLCamera.OnCaptureCompletedDelegate OnCaptureCompleted;

        /// <summary>
        /// Callback is invoked when a preview video frame buffer is available with MLCamera.CaptureType.Preview.
        /// Not valid for MR/VR Capture since it does not have preview support.
        /// </summary>
#pragma warning disable CS0067
        public event MLCamera.OnPreviewCaptureCompletedDelegate OnPreviewCaptureCompleted;
#pragma warning restore CS0067

        /// <summary>
        /// Callback is invoked when a captured image buffer is available.
        /// </summary>
        public event MLCamera.OnCapturedFrameAvailableDelegate OnRawImageAvailable;

        /// <summary>
        /// Callback is invoked when a captured raw/compressed video frame buffer is available, invoked on the main thread.
        /// </summary>
        public event MLCamera.OnCapturedFrameAvailableDelegate OnRawVideoFrameAvailable;

        /// <summary>
        /// Camera capture callback, capture raw video frame, invoked on the same thread as the native callback,
        /// allowing the use of the unmanaged native pointer to the frame data memory.
        /// </summary>
        public event MLCamera.OnCapturedFrameAvailableDelegate OnRawVideoFrameAvailable_NativeCallbackThread;

        /// <summary>
        /// Callback is invoked when a captured preview frame buffer is available, invoked on the main thread.
        /// </summary>
        public event MLCamera.OnPreviewBufferAvailableDelegate OnPreviewBufferAvailable;


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
        /// <param name="extra">Carries capture result information of current captured frame.</param>
        public delegate void OnCaptureFailedDelegate(ResultExtras extra);

        /// <summary>
        /// Delegate to notify the app when a capture request is aborted.
        /// </summary>
        public delegate void OnCaptureAbortedDelegate();

        /// <summary>
        /// Delegate to notify the app when a capture request is completed.
        /// </summary>
        /// <param name="metadataHandle">Handle to metadata of captured frame.</param>
        /// <param name="extra">Carries capture result information of current captured frame.</param>
        public delegate void OnCaptureCompletedDelegate(Metadata metadataHandle, ResultExtras extra);

        /// <summary>
        /// Delegate to notify the app when the result of a preview capture is available.
        /// </summary>
        /// <param name="result">Result reporting whether the preview capture completed or not.</param>
        public delegate void OnPreviewCaptureCompletedDelegate(MLResult result);


        /// <summary>
        /// Delegate to notify the app when the frame data of a capture is available.
        /// </summary>
        /// <param name="frameInfo">Frame data</param>
        /// <param name="extra">Carries capture result information of current captured frame.</param>
        public delegate void OnCapturedFrameAvailableDelegate(CameraOutput frameInfo, ResultExtras resultExtras, Metadata metadataHandle);

        /// <summary>
        /// Delegate to notify the app when the frame data of a preview is available.
        /// </summary>
        /// <param name="metadataHandle">Handle to metadata of captured frame.</param>
        /// <param name="extra">Carries capture result information of current captured frame.</param>
        public delegate void OnPreviewBufferAvailableDelegate(Metadata metadataHandle, ResultExtras extra);

        protected static MLCamera.OnDeviceAvailabilityStatusDelegate internalOnDeviceAvailable;
        protected static MLCamera.OnDeviceAvailabilityStatusDelegate internalOnDeviceUnavailable;

        private static int GetDeviceAvailabilitySubscriberCount()
        {
            int deviceAvailableListeners = (internalOnDeviceAvailable != null) ? internalOnDeviceAvailable.GetInvocationList().Length : 0;
            int deviceUnavailableListeners = (internalOnDeviceUnavailable != null) ? internalOnDeviceUnavailable.GetInvocationList().Length : 0;
            return deviceAvailableListeners + deviceUnavailableListeners;
        }

        #endregion
    }
}
