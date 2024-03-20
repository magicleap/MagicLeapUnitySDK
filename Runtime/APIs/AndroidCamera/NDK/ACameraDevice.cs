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

    using static CameraNativeBindings;

    using ErrorStateCallback = System.Action<System.IntPtr, ACameraDevice, ACameraDevice.ErrorCode>;
    using StateCallback = System.Action<System.IntPtr, ACameraDevice>;

    public struct ACameraDevice : INullablePointer
    {
        public enum ErrorCode
        {
            /// <summary>
            /// The camera device is in use already.
            /// </summary>
            CameraInUse = 1,

            /// <summary>
            /// The system-wide limit for number of open cameras or camera resources has
            /// been reached, and more camera devices cannot be opened until previous
            /// instances are closed.
            /// </summary>
            MaxCamerasInUse = 2,

            /// <summary>
            /// The camera is disabled due to a device policy, and cannot be opened.
            /// </summary>
            CameraDisabled = 3,

            /// <summary>
            /// The camera device has encountered a fatal error.
            /// <p>The camera device needs to be re-opened to be used again.</p>
            /// </summary>
            CameraDeviceFatalError = 4,

            /// <summary>
            /// The camera service has encountered a fatal error.
            /// <p>The Android device may need to be shut down and restarted to restore
            /// camera function, or there may be a persistent hardware problem.
            /// An attempt at recovery may be possible by closing the
            /// CameraDevice and the CameraManager, and trying to acquire all resources
            /// again from scratch.</p>
            /// </summary>
            CameraServiceFatalError = 5
        }

        public struct StateCallbacks
        {
            private IntPtr context;
            private IntPtr onDisconnected;
            private IntPtr onError;

            public static StateCallbacks Create(StateCallback onDisconnected = null, ErrorStateCallback onError = null,
                IntPtr context = default)
            {
                return new StateCallbacks
                {
                    context = context,
                    onDisconnected = onDisconnected != null
                        ? Marshal.GetFunctionPointerForDelegate(onDisconnected)
                        : IntPtr.Zero,
                    onError = onError != null
                        ? Marshal.GetFunctionPointerForDelegate(onDisconnected)
                        : IntPtr.Zero,
                };
            }
        }

        private IntPtr value;

        /// <summary>
        /// Note: Be careful when repeatedly accessing this property, because it allocates a new C# string each time
        /// and can generate additional GC pressure.
        /// </summary>
        public string Id
        {
            get
            {
                unsafe
                {
                    return Marshal.PtrToStringAuto(new IntPtr(ACameraDevice_getId(this)));
                }
            }
        }

        public bool IsNull => value == IntPtr.Zero;

        public void Dispose()
        {
            if (!IsNull)
                ACameraDevice_close(this);

            value = IntPtr.Zero;
        }

        public bool TryCreateCaptureRequest(ACaptureRequest.Template template, out ACaptureRequest request)
        {
            this.CheckNullAndThrow();
            var result = ACameraDevice_createCaptureRequest(this, template, out request);
            result.CheckReturnValueAndThrow();
            return result  == CameraStatus.Ok;
        }

        public bool TryCreateCaptureSession(ACaptureSessionOutputContainer container,
            out ACameraCaptureSession session)
        {
            this.CheckNullAndThrow();
            container.CheckNullAndThrow();
            unsafe
            {
                var result = ACameraDevice_createCaptureSession(this, container, null, out session);
                result.CheckReturnValueAndThrow();
                return result == CameraStatus.Ok;
            }
        }

        public bool TryCreateCaptureSessionWithCallbacks(ACaptureSessionOutputContainer container,
            ACameraCaptureSession.StateCallbacks callbacks, out ACameraCaptureSession session)
        {
            this.CheckNullAndThrow();
            container.CheckNullAndThrow();
            unsafe
            {
                var result = ACameraDevice_createCaptureSession(this, container, &callbacks, out session);
                result.CheckReturnValueAndThrow();
                return result == CameraStatus.Ok;
            }
        }
    }


}
