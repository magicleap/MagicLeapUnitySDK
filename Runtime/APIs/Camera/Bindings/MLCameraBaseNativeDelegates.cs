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
        internal partial class NativeBindings
        {
            public delegate void DeviceAvailabilityStatusDelegate(ref MLCameraDeviceAvailabilityInfo info);

            /// <summary>
            /// A generic delegate for camera events.
            /// </summary>
            /// <param name="data">Custom data returned when the callback is triggered, user metadata.</param>
            public delegate void OnDataCallback(IntPtr data);

            /// <summary>
            /// A delegate for camera error events.
            /// </summary>
            /// <param name="error">The type of error that was reported.</param>
            /// <param name="data">Custom data returned when the callback is triggered, user metadata.</param>
            public delegate void OnErrorDataCallback(MLCamera.ErrorType error, IntPtr data);

            /// <summary>
            /// A delegate for image buffer events.
            /// </summary>
            /// <param name="output">The camera output type.</param>
            /// <param name="data">Custom data returned when the callback is triggered, user metadata.</param>
            public delegate void OnOutputRefDataCallback(ref MLCameraOutput output, IntPtr data);

            /// <summary>
            /// A delegate for camera preview events.
            /// </summary>
            /// <param name="metadataHandle">A handle to the metadata.</param>
            /// <param name="data">Custom data returned when the callback is triggered, user metadata.</param>
            public delegate void OnHandleDataCallback(ulong metadataHandle, IntPtr data);

            /// <summary>
            /// A delegate for camera capture events.
            /// </summary>
            /// <param name="extra">A structure containing extra result information.</param>
            /// <param name="data">Custom data returned when the callback is triggered, user metadata.</param>
            public delegate void OnResultExtrasRefDataCallback(ref MLCameraResultExtras extra, IntPtr data);

            /// <summary>
            /// A delegate for camera capture events with additional information.
            /// </summary>
            /// <param name="metadataHandle">A handle to the metadata.</param>
            /// <param name="extra">A structure containing extra result information.</param>
            /// <param name="data">Custom data returned when the callback is triggered, user metadata.</param>
            public delegate void OnHandleAndResultExtrasRefDataCallback(MLCamera.Metadata metadataHandle,
                ref MLCameraResultExtras extra, IntPtr data);

            public delegate void OnCaptureFailedDelegate(ref MLCameraResultExtras extra, IntPtr data);

            public delegate void OnCaptureAbortedDelegate(IntPtr data);

            public delegate void OnPreviewBufferAvailableDelegate(ulong bufferHandle, ulong metadataHandle,
                ref MLCameraResultExtras extra, IntPtr data);

            public delegate void OnDeviceStreamingDelegate(IntPtr data);

            public delegate void OnDeviceIdleDelegate(IntPtr data);

            public delegate void OnImageBufferAvailableDelegate(ref MLCameraOutput output, ulong metadataHandle,
                ref MLCameraResultExtras extra, IntPtr data);

            public delegate void OnVideoBufferAvailableDelegate(ref MLCameraOutput output, ulong metadataHandle,
                ref MLCameraResultExtras extra, IntPtr data);

            public delegate void OnDeviceErrorDelegate(MLCamera.ErrorType error, IntPtr data);

            public delegate void OnDeviceDisconnectedDelegate(MLCamera.DisconnectReason reason, IntPtr data);

            public delegate void OnCaptureCompletedDelegate(ulong metadataHandle, ref MLCameraResultExtras extra, IntPtr data);
        }
    }
}
