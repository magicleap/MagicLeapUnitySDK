// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;

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

    public sealed unsafe class AndroidCamera : IDisposable
    {
        struct AndroidCameraUpdate {}

        private static List<AndroidCamera> camerasToUpdate = null;

        private static void UpdateCameras()
        {
            foreach (var camera in camerasToUpdate)
                camera.ReadAndProcessFrame();
        }

        internal static IDisposable RegisterCameraForUpdate(AndroidCamera camera)
            => AndroidCameraPlayerLoopUtility.LazyRegisterPlayerLoopUpdateInternal(ref camerasToUpdate, camera, typeof(AndroidCameraUpdate),
                UpdateCameras);

        private struct UnsafeCameraState
        {
            public ACameraDevice cameraDevice;
            public ACameraCaptureSession cameraCaptureSession;
            public ACameraOutputTarget cameraOutputTarget;
            public ACaptureSessionOutput captureSessionOutput;
            public ACaptureSessionOutputContainer captureSessionOutputContainer;
            public AImageReader imageReader;
            public NativeRingBuffer ringBuffer;
            public IntPtr activeRequestObject;

            public void Dispose()
            {
                if (ringBuffer.IsCreated)
                    ringBuffer.Dispose();
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
                var unsafeImage = AHardwareBufferImageBackend.Create(img, Allocator.TempJob);
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

        public CaptureRequest CreateCaptureRequest(RequestTemplate template,
            CaptureRequest.OnFrameAvailable onFrameAvailable, IntPtr context = default)
        {
            if (cameraState == null)
                throw new NullReferenceException();

            if (!cameraState->cameraDevice.TryCreateCaptureRequest(template.ToNDKTemplate(), out var requestPtr))
                throw new Exception("failed to create capture request native handle");

            if (!requestPtr.TryAddOutputTarget(cameraState->cameraOutputTarget))
                throw new Exception("failed to add output target to request");

            return new CaptureRequest(this, requestPtr, onFrameAvailable, context);
        }

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

        private void ReadAndProcessFrame()
        {
            var reader = cameraState->ringBuffer.AsNonblockingReader();

            var didRead = reader.TryRead(out NativeImage nci);
            if (didRead)
                reader.FinishRead();
            reader.Dispose();

            if (!didRead)
                return;

            var jobHandle = CaptureRequest.TryGetFromIntPtr(nci.RequestHandle, out var request)
                ? request.ProcessNewFrame(nci)
                : new JobHandle();

            nci.Dispose(jobHandle);
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
