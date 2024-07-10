// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MagicLeap.Android
{
    using AOT;
    using System;
    using System.Collections.Generic;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Jobs;
    using UnityEngine.XR.MagicLeap;
    using UnityEngine.XR.MagicLeap.Unsafe;

    using NDK.Camera;
    using NDK.Media;
    using MagicLeap.Android.NDK.Camera.Metadata;

    public sealed unsafe class AndroidCamera : IDisposable
    {
        struct AndroidCameraUpdate { }

        public enum CaptureFormat
        {
            [InspectorName("YUV")]
            YUV_420_888 = MediaFormat.Yuv_420_888,

            [InspectorName("JPEG")]
            JPEG = MediaFormat.Jpeg
        }

        public enum CaptureFrameRate
        {
            _15 = 15,
            _30 = 30,
            _60 = 60
        }

        private static List<AndroidCamera> camerasToUpdate = null;

        private static void UpdateCameras()
        {
            foreach (var camera in camerasToUpdate)
                camera.ReadAndProcessFrame();
        }

        internal static IDisposable RegisterCameraForUpdate(AndroidCamera camera)
            => AndroidCameraPlayerLoopUtility.LazyRegisterPlayerLoopUpdateInternal(ref camerasToUpdate, camera, typeof(AndroidCameraUpdate),
                UpdateCameras, typeof(Initialization.XREarlyUpdate));

        private struct CameraPerformanceTracker
        {
            // This is basically just a simple ringbuffer.
            // We use a power of two as the buffer size,
            // since that allows us to use (buffer_size - 1)
            // as a bitmask for the index (lastSample).
            // This also works if/when lastSample
            // overflows and wraps, as we mask out
            // the sign bit with SAMPLE_MASK, so
            // we can guarantee that (<index> & SAMPLE_MASK)
            // will never be negative, regardless of the
            // actual value of <index>.
            const int NUM_SAMPLES = 64;
            const int SAMPLE_MASK = NUM_SAMPLES - 1;
            private fixed long timestampSamples[NUM_SAMPLES];
            private int lastSample;
            private int locked;

            public bool HasSufficientSamples
                => lastSample > SAMPLE_MASK;

            public static CameraPerformanceTracker Create()
            {
                var tracker = new CameraPerformanceTracker();
                tracker.lastSample = -1;
                tracker.locked = 0;

                return tracker;
            }

            public void AddSample(long timestamp)
            {
                Lock();
                AddSampleInternal(timestamp);
                Unlock();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void AddSampleInternal(long timestamp)
                => timestampSamples[++lastSample & SAMPLE_MASK] = timestamp;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void Lock()
            {
                int backoff = 1;
                // (for reference below)
                // locked == 0 means "unlocked"
                // locked == 1 means "locked"
                //
                // if 'locked' is currently 0, we want to set it to 1 to indicate we're locking
                // the data. if it's not 0, it means some other thread has already locked the data
                // and we need to wait.
                // .CompareExchange() allows us to combine the read, compare, and (assuming the
                // compare returns true) the write as a single operation from the perspective of all
                // other threads on the system.
                // So here, we're testing 'locked' to see if it's currently 0, and IFF that comparison
                // succeeds, set it to 1.
                // Additionally, .CompareExchange() returns the current value of 'locked', so
                // by checking it in a while() loop (like we do below), we can block *this thread*
                // from moving forward until the *other thread* has completed it work, and set
                // 'locked' back to 0.
                while (Interlocked.CompareExchange(ref locked, 1, 0) == 1)
                {
                    // There shouldn't be significant contention here, as there are only two
                    // threads updating the array, but the exponential backoff is here as a
                    // safety valve to prevent either thread from hammering the cpu with
                    // atomic operations. We also want to minimize the risk of either thread
                    // getting put to sleep (ie, forced to yield its timeslice and/or worse,
                    // getting kicked out of the kernel's runqueue), so .SpinWait() here feels
                    // like an acceptable tradeoff between latency and overall efficiency.
                    Thread.SpinWait(backoff);
                    backoff <<= 1;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void Unlock()
                => locked = 0; // loads and stores of pointer-width or less are atomic in .net by default.

            public long ReadSample(int age)
            {
                Lock();
                var result = ReadSampleInternal(lastSample, age);
                Unlock();
                return result;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal long ReadSampleInternal(int start, int age)
                => timestampSamples[(start - age) & SAMPLE_MASK];

            public int ReadSamples(NativeArray<long> array)
            {
                if (!array.IsCreated)
                    throw new ArgumentNullException(nameof(array));

                if (array.Length > NUM_SAMPLES)
                    throw new Exception($"size of array exceeds maximum number of samples");

                if (lastSample < 0)
                    return 0;

                Lock();
                var start = lastSample;
                int i;
                for (i = 0; i < array.Length; ++i)
                {
                    var sample = ReadSampleInternal(start, i);
                    if (sample == 0)
                        break;
                    array[i] = sample;
                }

                Unlock();

                return i;
            }
        }

        private struct UnsafeCameraState
        {
            public ACameraDevice cameraDevice;
            public ACameraCaptureSession cameraCaptureSession;
            public ACameraOutputTarget cameraOutputTarget;
            public ACaptureSessionOutput captureSessionOutput;
            public ACaptureSessionOutputContainer captureSessionOutputContainer;
            public AImageReader imageReader;
            public NativeRingBuffer ringBuffer;
            public NativeRingBuffer ringBufferForTimestamps;
            public CameraPerformanceTracker tracker;
            public IntPtr activeRequestObject;

            public void Dispose()
            {
                if (ringBuffer.IsCreated)
                    ringBuffer.Dispose();
                if (ringBufferForTimestamps.IsCreated)
                    ringBufferForTimestamps.Dispose();
                cameraDevice.Dispose();
                imageReader.Dispose();
            }
        }

        private UnsafeCameraState* cameraState;
        private ACameraCaptureSession.CaptureCallbacks captureCallbacks;
        private IDisposable updateSubscription;

        public MediaFormat Format => cameraState->imageReader.Format;

        public int ImageHeight => cameraState->imageReader.Height;

        public int ImageWidth => cameraState->imageReader.Width;

        internal ACameraDevice NativeCamera => cameraState->cameraDevice;

        internal AndroidCamera(ACameraDevice cameraDevice, AImageReader imageReader)
        {
            cameraState = UnsafeUtilityEx.CallocTracked(Allocator.Persistent,
                new UnsafeCameraState
                {
                    cameraDevice = cameraDevice,
                    imageReader = imageReader,
                });

            var listener = AImageReader.ImageListener.Create(OnImageAvailable, new IntPtr(cameraState));
            if (!cameraState->imageReader.TrySetImageListener(listener))
                throw new Exception("failed to install listener callback on image reader");

            if (!ACameraOutputTarget.TryCreate(imageReader.Window, out cameraState->cameraOutputTarget))
                throw new Exception("failed to create camera output target");

            if (!ACaptureSessionOutputContainer.TryCreate(out cameraState->captureSessionOutputContainer))
                throw new Exception("failed to create capture session output container");

            if (!ACaptureSessionOutput.TryCreate(imageReader.Window, out cameraState->captureSessionOutput))
                throw new Exception("failed to create capture session output");

            if (!cameraState->captureSessionOutputContainer.TryAddOutput(cameraState->captureSessionOutput))
                throw new Exception("failed to add capture session output to container");

            var stateCallbacks =
                ACameraCaptureSession.StateCallbacks.Create(OnSessionActive, OnSessionClosed, OnSessionReady);

            if (!cameraState->cameraDevice.TryCreateCaptureSessionWithCallbacks(
                    cameraState->captureSessionOutputContainer, stateCallbacks,
                    out cameraState->cameraCaptureSession))
                throw new Exception("failed to create camera capture session");

            cameraState->ringBuffer = new NativeRingBuffer(64, Allocator.Persistent);
            cameraState->ringBufferForTimestamps = new NativeRingBuffer(64, Allocator.Persistent);

            captureCallbacks = ACameraCaptureSession.CaptureCallbacks.Create(OnCaptureStarted, captureCompleted: OnCaptureCompleted, context: new IntPtr(cameraState));

            updateSubscription = RegisterCameraForUpdate(this);
        }

        ~AndroidCamera() => Dispose(false);

        public void Dispose() => Dispose(true);

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (cameraState == null)
                    throw new ObjectDisposedException("object is already disposed");
            }
            else if (cameraState != null)
                Debug.LogWarning("Please call .Dispose() to properly release camera resources");

            updateSubscription?.Dispose();
            cameraState->Dispose();
            UnsafeUtility.FreeTracked(cameraState, Allocator.Persistent);
            cameraState = null;
        }

        [MonoPInvokeCallback(typeof(Action<IntPtr, ACameraCaptureSession, ACaptureRequest, ACameraMetadata>))]
        private static void OnCaptureCompleted(IntPtr context, ACameraCaptureSession session,
            ACaptureRequest request, ACameraMetadata metadata)
        {
            var camera = (UnsafeCameraState*)context;
            if (camera == null)
                return;

            camera->activeRequestObject = IntPtr.Zero;
        }

        [MonoPInvokeCallback(typeof(Action<IntPtr, ACameraCaptureSession, ACaptureRequest, long>))]
        private static void OnCaptureStarted(IntPtr context, ACameraCaptureSession session,
            ACaptureRequest request, long timestampInNS)
        {
            var camera = (UnsafeCameraState*)context;
            if (camera == null)
                return;

            var reqObj = request.UserContext;
            if (reqObj == IntPtr.Zero)
                return;

            using (var writer = camera->ringBufferForTimestamps.AsNonblockingWriter())
                if (writer.TryWrite(timestampInNS))
                    writer.FinishWrite();

            camera->activeRequestObject = reqObj;
        }

        [MonoPInvokeCallback(typeof(Action<IntPtr, AImageReader>))]
        private static void OnImageAvailable(IntPtr context, AImageReader reader)
        {
            var camera = (UnsafeCameraState*)context;
            if (camera == null)
                return;

            if (!reader.TryAcquireNextImage(out var img))
                return;

            var reqObj = camera->activeRequestObject;
            if (reqObj == IntPtr.Zero)
            {
                img.Dispose();
                return;
            }

            NativeImage nci;
            // the native camera image holds a reference to the underlying hw buffer,
            // so we can safely delete/release the AImage object immediately back
            // to the image reader.
            using (img)
            {
                var unsafeImage = AHardwareBufferImageBackend.Create(img, Allocator.Persistent);
                nci = new NativeImage(unsafeImage, camera->activeRequestObject);
            }
            // var unsafeImage = AImageBackend.Create(img, Allocator.TempJob);
            // nci = new NativeImage(unsafeImage, camera->activeRequestObject);

            var writer = camera->ringBuffer.AsNonblockingWriter();

            if (writer.TryWrite(nci))
                writer.FinishWrite();
            else
                nci.Dispose();

            writer.Dispose();
        }

        public CaptureRequest CreateCaptureRequest(MagicLeapCameras.VideoCaptureMode captureMode, CaptureRequest.OnFrameAvailable onFrameAvailable,
            RequestTemplate template = RequestTemplate.Preview, IntPtr context = default)
        {
            if (cameraState == null)
                throw new NullReferenceException();

            if (!cameraState->cameraDevice.TryCreateCaptureRequest(template.ToNDKTemplate(), out var requestPtr))
                throw new Exception("failed to create capture request native handle");

            if (!requestPtr.TryAddOutputTarget(cameraState->cameraOutputTarget))
                throw new Exception("failed to add output target to request");

            return new CaptureRequest(this, requestPtr, onFrameAvailable, context);
        }

        public int GetTimestamps(NativeArray<long> timestampArray)
            => cameraState->tracker.HasSufficientSamples
                ? cameraState->tracker.ReadSamples(timestampArray)
                : 0;

        public bool TryStopRepeatingRequest()
            => cameraState->cameraCaptureSession.TryStopRepeating();

        public bool TrySubmitCaptureRequest(CaptureRequest request, bool repeating = false)
        {
            if (request == null || request.NativeRequest.IsNull)
                throw new ArgumentNullException(nameof(request));

            var array = new NativeArray<ACaptureRequest>(1, Allocator.Temp);
            array[0] = request.NativeRequest;

            var success = repeating
                ? cameraState->cameraCaptureSession.TrySetRepeatingRequestWithCallbacks(array, captureCallbacks,
                    out _)
                : cameraState->cameraCaptureSession.TryCaptureWithCallbacks(array, captureCallbacks, out _);

            array.Dispose();

            return success;
        }

        public bool TrySubmitCaptureRequests(CaptureRequest[] requests, bool repeating = false)
        {
            if (requests == null)
                throw new ArgumentNullException(nameof(requests));

            if (requests.Length == 0)
                throw new ArgumentException("requests array must contain at least 1 item");

            var array = ExtractRequestPtrs(requests);

            var success = repeating
                ? cameraState->cameraCaptureSession.TrySetRepeatingRequestWithCallbacks(array, captureCallbacks,
                    out _)
                : cameraState->cameraCaptureSession.TryCaptureWithCallbacks(array, captureCallbacks, out _);

            array.Dispose();

            return success;
        }

        private static NativeArray<ACaptureRequest> ExtractRequestPtrs(CaptureRequest[] reqs)
        {
            if (reqs == null || reqs.Length == 0)
                return default;

            var array = new NativeArray<ACaptureRequest>(reqs.Length, Allocator.Temp);
            for (var i = 0; i < reqs.Length; ++i)
                array[i] = reqs[i].NativeRequest;

            return array;
        }

        private void HandleImage(NativeImage image, bool isLatest)
        {
            if (!image.IsCreated)
                return;

            var shouldProcess = isLatest;
            if (shouldProcess)
            {
                if (CaptureRequest.TryGetFromIntPtr(image.RequestHandle, out var request))
                    image.Dispose(request.ProcessNewFrame(image));
                else
                    Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "HandleImage failed: was supposed to process image, but couldn't extract capture request");
                return;
            }

            image.Dispose();
        }

        private void ReadAndProcessFrame()
        {
            using (var reader = cameraState->ringBufferForTimestamps.AsNonblockingReader())
            {
                bool didRead = false;
                while (reader.TryRead(out long timestamp))
                {
                    cameraState->tracker.AddSample(timestamp);

                    didRead = true;
                }
                if (didRead)
                    reader.FinishRead();
            }

            NativeImage nci = default;
            using (var reader = cameraState->ringBuffer.AsNonblockingReader())
            {
                if (reader.TryRead(out nci))
                    reader.FinishRead();
            }

            HandleImage(nci, true);
        }

        [MonoPInvokeCallback(typeof(Action<IntPtr, ACameraCaptureSession>))]
        static void OnSessionActive(IntPtr context, ACameraCaptureSession session)
        {
        }

        [MonoPInvokeCallback(typeof(Action<IntPtr, ACameraCaptureSession>))]
        static void OnSessionClosed(IntPtr context, ACameraCaptureSession session)
        {
        }

        [MonoPInvokeCallback(typeof(Action<IntPtr, ACameraCaptureSession>))]
        static void OnSessionReady(IntPtr context, ACameraCaptureSession session)
        {
        }

    }

}
