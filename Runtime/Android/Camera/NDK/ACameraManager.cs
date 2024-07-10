// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using AOT;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap.Android.NDK.Camera
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    using static CameraNativeBindings;


    using AvailabilityCallback = System.Action<System.IntPtr, string>;

    public struct ACameraManager : INullablePointer
    {
        public struct AvailabilityCallbacks
        {
            private IntPtr context;
            private IntPtr onCameraAvailable;
            private IntPtr onCameraUnavailable;

            public static AvailabilityCallbacks Create(AvailabilityCallback onCameraAvailable = null, AvailabilityCallback onCameraUnavailable = null,
                IntPtr context = default)
            {
                IntPtr oca = onCameraAvailable != null
                    ? Marshal.GetFunctionPointerForDelegate(onCameraAvailable)
                    : IntPtr.Zero;
                IntPtr ocu = onCameraUnavailable != null
                    ? Marshal.GetFunctionPointerForDelegate(onCameraUnavailable)
                    : IntPtr.Zero;
                return new AvailabilityCallbacks
                {
                    context = context,
                    onCameraAvailable = oca,
                    onCameraUnavailable = ocu,
                };
            }
        }

        public struct AvailabilityCallbackSubscription : IDisposable
        {
            private AvailabilityCallbacks callbacks;
            private ACameraManager cameraManager;

            internal static AvailabilityCallbackSubscription Create(ACameraManager manager, AvailabilityCallbacks callbacks)
                => new AvailabilityCallbackSubscription()
                {
                    callbacks = callbacks,
                    cameraManager = manager
                };

            public void Dispose()
            {
                if (!cameraManager.IsNull)
                    cameraManager.TryUnregisterAvailabilityCallbacks(ref callbacks);
            }
        }

        private IntPtr value;

        public bool IsNull => value == IntPtr.Zero;

        public static ACameraManager Create()
            => ACameraManager_create();

        public void Dispose()
        {
            if (!IsNull)
                ACameraManager_delete(this);
            value = IntPtr.Zero;
        }

        public bool TryGetCameraIds(out ACameraIdList list)
        {
            this.CheckNullAndThrow();
            var result = ACameraManager_getCameraIdList(this, out list);
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }

        public bool TryGetCameraMetadata(string cameraId, out ACameraMetadata metadata)
        {
            this.CheckNullAndThrow();
            var result = ACameraManager_getCameraCharacteristics(this, cameraId, out metadata);
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }

        public unsafe bool TryGetCameraMetadataNonAlloc(byte* cameraId, out ACameraMetadata metadata)
        {
            this.CheckNullAndThrow();
            var result = ACameraManager_getCameraCharacteristicsNonAlloc(this, cameraId, out metadata);
            return result == CameraStatus.Ok;
        }

        public bool TryOpenCamera(string cameraId, out ACameraDevice camera)
        {
            this.CheckNullAndThrow();
            unsafe
            {
                ACameraDevice.StateCallbacks callbacks = ACameraDevice.StateCallbacks.Create(OnCameraDisconnected, OnCameraError);
                var result = ACameraManager_openCamera(this, cameraId, &callbacks, out camera);
                result.CheckReturnValueAndThrow();
                return result == CameraStatus.Ok;
            }
        }

        public bool TryOpenCameraWithCallbacks(string cameraId, ACameraDevice.StateCallbacks callbacks, out ACameraDevice camera)
        {
            this.CheckNullAndThrow();
            unsafe
            {
                var result = ACameraManager_openCamera(this, cameraId, &callbacks, out camera);
                result.CheckReturnValueAndThrow();
                return result == CameraStatus.Ok;
            }
        }

        public unsafe bool TryOpenCameraNonAlloc(byte* cameraId, out ACameraDevice camera)
        {
            this.CheckNullAndThrow();
            var result = ACameraManager_openCameraNonAlloc(this, cameraId, null, out camera);
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }

        public unsafe bool TryOpenCameraWithCallbacksNonAlloc(byte* cameraId, ACameraDevice.StateCallbacks callbacks, out ACameraDevice camera)
        {
            this.CheckNullAndThrow();
            var result = ACameraManager_openCameraNonAlloc(this, cameraId, &callbacks, out camera);
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }

        public bool TryRegisterAvailabilityCallbacks(ref AvailabilityCallbacks callbacks)
        {
            this.CheckNullAndThrow();
            var result = ACameraManager_registerAvailabilityCallback(this, ref callbacks);
            return result == CameraStatus.Ok;
        }

        public bool TrySubscribeToAvailabilityCallbacks(AvailabilityCallbacks callbacks,
            out AvailabilityCallbackSubscription sub)
        {
            sub = default;
            if (!TryRegisterAvailabilityCallbacks(ref callbacks))
                return false;

            sub = AvailabilityCallbackSubscription.Create(this, callbacks);
            return true;
        }

        public bool TryUnregisterAvailabilityCallbacks(ref AvailabilityCallbacks callbacks)
        {
            this.CheckNullAndThrow();
            var result = ACameraManager_unregisterAvailabilityCallback(this, ref callbacks);
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }

        [MonoPInvokeCallback(typeof(Action<IntPtr, ACameraDevice>))]
        static void OnCameraDisconnected(IntPtr context, ACameraDevice camera)
        {
            UnityEngine.Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, $"Warning: Camera '{camera.Id}' has been disconnected");
        }

        [MonoPInvokeCallback(typeof(Action<IntPtr, ACameraDevice, ACameraDevice.ErrorCode>))]
        static void OnCameraError(IntPtr context, ACameraDevice camera, ACameraDevice.ErrorCode errorCode)
        {
            UnityEngine.Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, $"Error: Camera '{camera.Id} reported error: {errorCode}");
        }
    }
}
