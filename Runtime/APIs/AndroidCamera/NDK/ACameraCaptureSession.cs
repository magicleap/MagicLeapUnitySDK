// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace MagicLeap.Android.NDK.Camera
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    using static CameraNativeBindings;

    using StateCallback = System.Action<System.IntPtr, ACameraCaptureSession>;

    using CaptureStartedCallback = System.Action<System.IntPtr, ACameraCaptureSession, ACaptureRequest, long>;
    using CaptureResultCallback = System.Action<System.IntPtr, ACameraCaptureSession, ACaptureRequest, ACameraMetadata>;
    using CaptureFailedCallback = System.Action<System.IntPtr, ACameraCaptureSession, ACaptureRequest, ACameraCaptureFailure>;
    using CaptureSequenceEndedCallback = System.Action<System.IntPtr, ACameraCaptureSession, int, long>;
    using CaptureSequenceAbortedCallback = System.Action<System.IntPtr, ACameraCaptureSession, int>;
    using BufferLostCallback = System.Action<System.IntPtr, ACameraCaptureSession, ACaptureRequest, NativeWindow.ANativeWindow, long>;

    public struct ACameraCaptureSession : INullablePointer
    {
        public struct CaptureCallbacks
        {
            private IntPtr context;
            private IntPtr onCaptureStarted;
            private IntPtr onCaptureProgressed;
            private IntPtr onCaptureCompleted;
            private IntPtr onCaptureFailed;
            private IntPtr onCaptureSequenceCompleted;
            private IntPtr onCaptureSequenceAborted;
            private IntPtr onCaptureBufferLost;

            public static CaptureCallbacks Create(CaptureStartedCallback captureStarted = null,
                CaptureResultCallback captureProgressed = null, CaptureResultCallback captureCompleted = null,
                CaptureFailedCallback captureFailed = null, CaptureSequenceEndedCallback sequenceEnded = null,
                CaptureSequenceAbortedCallback sequenceAborted = null, BufferLostCallback bufferLost = null,
                IntPtr context = default)
            {
                if (captureStarted == null
                    && captureProgressed == null
                    && captureCompleted == null
                    && captureFailed == null
                    && sequenceEnded == null
                    && sequenceAborted == null
                    && bufferLost == null)
                    throw new InvalidOperationException("at least one callback must not be null");

                return new CaptureCallbacks
                {
                    context = context,
                    onCaptureStarted = captureStarted == null
                        ? IntPtr.Zero
                        : Marshal.GetFunctionPointerForDelegate(captureStarted),
                    onCaptureProgressed = captureProgressed == null
                        ? IntPtr.Zero
                        : Marshal.GetFunctionPointerForDelegate(captureProgressed),
                    onCaptureCompleted = captureCompleted == null
                        ? IntPtr.Zero
                        : Marshal.GetFunctionPointerForDelegate(captureCompleted),
                    onCaptureFailed = captureFailed == null
                        ? IntPtr.Zero
                        : Marshal.GetFunctionPointerForDelegate(captureFailed),
                    onCaptureSequenceCompleted = sequenceEnded == null
                        ? IntPtr.Zero
                        : Marshal.GetFunctionPointerForDelegate(sequenceEnded),
                    onCaptureSequenceAborted = sequenceAborted == null
                        ? IntPtr.Zero
                        : Marshal.GetFunctionPointerForDelegate(sequenceAborted),
                    onCaptureBufferLost = bufferLost == null
                        ? IntPtr.Zero
                        : Marshal.GetFunctionPointerForDelegate(bufferLost),
                };
            }
        }

        public struct StateCallbacks
        {
            private IntPtr context;
            private IntPtr onClosed;
            private IntPtr onReady;
            private IntPtr onActive;

            public static StateCallbacks Create(StateCallback active = null, StateCallback closed = null,
                StateCallback ready = null, IntPtr context = default)
            {
                if (active == null && closed == null && ready == null)
                    throw new InvalidOperationException("at least one callback must not be null");
                return new StateCallbacks
                {
                    context = context,
                    onActive = active == null
                        ? IntPtr.Zero
                        : Marshal.GetFunctionPointerForDelegate(active),
                    onClosed = closed == null
                        ? IntPtr.Zero
                        : Marshal.GetFunctionPointerForDelegate(closed),
                    onReady = ready == null
                        ? IntPtr.Zero
                        : Marshal.GetFunctionPointerForDelegate(ready),
                };
            }
        }

        private IntPtr value;

        public ACameraDevice Device
        {
            get
            {
                var result = ACameraCaptureSession_getDevice(this, out var devicePtr);
                result.CheckReturnValueAndThrow();
                return devicePtr;
            }
        }

        public bool IsNull => value == IntPtr.Zero;

        public void Dispose()
        {
            if (!IsNull)
                ACameraCaptureSession_close(this);

            value = IntPtr.Zero;
        }

        public bool TryCapture(NativeArray<ACaptureRequest> requests, out int sequenceId)
        {
            this.CheckNullAndThrow();
            CheckValidRequestArrayAndThrow(ref requests);
            unsafe
            {
                var result = ACameraCaptureSession_capture(this, null, requests.Length,
                    (ACaptureRequest*)requests.GetUnsafeReadOnlyPtr(), out sequenceId);
                result.CheckReturnValueAndThrow();
                return result == CameraStatus.Ok;
            }
        }

        public bool TryCaptureWithCallbacks(NativeArray<ACaptureRequest> requests, CaptureCallbacks callbacks, out int sequenceId)
        {
            this.CheckNullAndThrow();
            CheckValidRequestArrayAndThrow(ref requests);
            unsafe
            {
                var result = ACameraCaptureSession_capture(this, &callbacks, requests.Length,
                    (ACaptureRequest*)requests.GetUnsafeReadOnlyPtr(), out sequenceId);
                result.CheckReturnValueAndThrow();
                return result == CameraStatus.Ok;
            }
        }

        public bool TrySetRepeatingRequest(NativeArray<ACaptureRequest> requests, out int sequenceId)
        {
            this.CheckNullAndThrow();
            CheckValidRequestArrayAndThrow(ref requests);
            unsafe
            {
                var result = ACameraCaptureSession_setRepeatingRequest(this, null, requests.Length,
                    (ACaptureRequest*)requests.GetUnsafeReadOnlyPtr(), out sequenceId);
                result.CheckReturnValueAndThrow();
                return result == CameraStatus.Ok;
            }
        }

        public bool TrySetRepeatingRequestWithCallbacks(NativeArray<ACaptureRequest> requests, CaptureCallbacks callbacks, out int sequenceId)
        {
            this.CheckNullAndThrow();
            CheckValidRequestArrayAndThrow(ref requests);
            unsafe
            {
                var result = ACameraCaptureSession_setRepeatingRequest(this, &callbacks, requests.Length,
                    (ACaptureRequest*)requests.GetUnsafeReadOnlyPtr(), out sequenceId);
                result.CheckReturnValueAndThrow();
                return result == CameraStatus.Ok;
            }
        }

        public bool TryStopRepeating()
        {
            this.CheckNullAndThrow();
            var ret = ACameraCaptureSession_stopRepeating(this);
            ret.CheckReturnValueAndThrow();
            return ret == CameraStatus.Ok;
        }

        [Conditional("DEVELOPMENT_BUILD")]
        private static void CheckValidRequestArrayAndThrow(ref NativeArray<ACaptureRequest> requests)
        {
            if (!requests.IsCreated)
                throw new ArgumentNullException(nameof(requests));

            if (requests.Length == 0)
                throw new InvalidOperationException("request array is empty");

            for (var i = 0; i < requests.Length; ++i)
                if (requests[i].IsNull)
                    throw new NullReferenceException($"request at index '{i}' is null");

        }
    }
}
