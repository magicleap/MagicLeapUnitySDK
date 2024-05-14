// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%


using System.Diagnostics;

namespace MagicLeap.Android.NDK.Camera
{
    using System;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using UnityEngine.XR.MagicLeap.Unsafe;

    using static CameraNativeBindings;

    public unsafe struct ACaptureRequest : INullablePointer
    {
        public enum Template
        {
            /// <summary>
            /// Create a request suitable for a camera preview window. Specifically, this
            /// means that high frame rate is given priority over the highest-quality
            /// post-processing. These requests would normally be used with the
            /// {@link ACameraCaptureSession_setRepeatingRequest} method.
            /// This template is guaranteed to be supported on all camera devices.
            ///
            /// @see ACameraDevice_createCaptureRequest
            /// </summary>
            Preview = 1,

            /// <summary>
            /// Create a request suitable for still image capture. Specifically, this
            /// means prioritizing image quality over frame rate. These requests would
            /// commonly be used with the {@link ACameraCaptureSession_capture} method.
            /// This template is guaranteed to be supported on all camera devices.
            ///
            /// @see ACameraDevice_createCaptureRequest
            /// </summary>
            StillCapture = 2,

            /// <summary>
            /// Create a request suitable for video recording. Specifically, this means
            /// that a stable frame rate is used, and post-processing is set for
            /// recording quality. These requests would commonly be used with the
            /// {@link ACameraCaptureSession_setRepeatingRequest} method.
            /// This template is guaranteed to be supported on all camera devices.
            ///
            /// @see ACameraDevice_createCaptureRequest
            /// </summary>
            Record = 3,

            /// <summary>
            /// Create a request suitable for still image capture while recording
            /// video. Specifically, this means maximizing image quality without
            /// disrupting the ongoing recording. These requests would commonly be used
            /// with the {@link ACameraCaptureSession_capture} method while a request based on
            /// {@link TEMPLATE_RECORD} is is in use with {@link ACameraCaptureSession_setRepeatingRequest}.
            /// This template is guaranteed to be supported on all camera devices.
            ///
            /// @see ACameraDevice_createCaptureRequest
            /// </summary>
            VideoSnapshot = 4,

            /// <summary>
            /// Create a request suitable for zero shutter lag still capture. This means
            /// means maximizing image quality without compromising preview frame rate.
            /// AE/AWB/AF should be on auto mode.
            ///
            /// @see ACameraDevice_createCaptureRequest
            /// </summary>
            ZeroShutterLag = 5,

            /// <summary>
            /// A basic template for direct application control of capture
            /// parameters. All automatic control is disabled (auto-exposure, auto-white
            /// balance, auto-focus), and post-processing parameters are set to preview
            /// quality. The manual capture parameters (exposure, sensitivity, and so on)
            /// are set to reasonable defaults, but should be overriden by the
            /// application depending on the intended use case.
            /// This template is guaranteed to be supported on camera devices that support the
            /// {@link ACAMERA_REQUEST_AVAILABLE_CAPABILITIES_MANUAL_SENSOR} capability.
            ///
            /// @see ACameraDevice_createCaptureRequest
            /// </summary>
            Manual = 6,
        }

        private IntPtr value;

        public bool IsNull => value == IntPtr.Zero;

        internal IntPtr UserContext
        {
            get => ACaptureRequest_getUserContext(this, out var context) == CameraStatus.Ok ? context : IntPtr.Zero;
            set
            {
                if (ACaptureRequest_setUserContext(this, value) != CameraStatus.Ok)
                    throw new Exception("failed to set user context value");
            }
        }

        public void Dispose()
        {
            if (!IsNull)
                ACaptureRequest_free(this);

            value = IntPtr.Zero;
        }

        public bool TryAddOutputTarget(ACameraOutputTarget ptr)
        {
            this.CheckNullAndThrow();
            ptr.CheckNullAndThrow();
            var result = ACaptureRequest_addTarget(this, ptr);
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }

        public bool TryGetAllTags(Allocator allocator, out NativeArray<uint> tags)
        {
            this.CheckNullAndThrow();
            if (allocator <= Allocator.None)
                throw new ArgumentException($"{nameof(allocator)} is not a valid allocator");

            tags = default;

            int numTags = 0;

            var result = ACaptureRequest_getAllTags(this, ref numTags, null);
            result.CheckReturnValueAndThrow();
            if (result != CameraStatus.Ok)
                return false;

            if (numTags == 0)
                return false;

            uint* tagBuffer =
                (uint*)UnsafeUtility.MallocTracked(sizeof(uint) * numTags, UnsafeUtility.AlignOf<uint>(), allocator, 0);

            result = ACaptureRequest_getAllTags(this, ref numTags, &tagBuffer);
            result.CheckReturnValueAndThrow();
            if (result != CameraStatus.Ok)
            {
                UnsafeUtility.FreeTracked(tagBuffer, allocator);
                return false;
            }

            tags = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<uint>(tagBuffer, numTags, allocator);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref tags, UnsafeUtilityEx.CreateAtomicSafetyHandleForAllocator(allocator));
#endif
            return true;
        }

        public bool TryGetMetadata(uint tag, out ACameraMetadata.Entry.ReadOnly outEntry)
        {
            this.CheckNullAndThrow();
            var result = ACaptureRequest_getConstEntry(this, tag, out outEntry);
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }

        public bool TrySetMetadataEntry(uint tag, NativeArray<byte> data)
        {
            this.CheckNullAndThrow();
            CheckValidArrayAndThrow(data);

            var result = ACaptureRequest_setEntry_u8(this, tag, (uint)data.Length,
                (byte*)data.GetUnsafeReadOnlyPtr());
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }

        public bool TrySetMetadataEntry(uint tag, NativeArray<int> data)
        {
            this.CheckNullAndThrow();
            CheckValidArrayAndThrow(data);

            var result = ACaptureRequest_setEntry_i32(this, tag, (uint)data.Length,
                (int*)data.GetUnsafeReadOnlyPtr());
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }

        public bool TrySetMetadataEntry(uint tag, NativeArray<long> data)
        {
            this.CheckNullAndThrow();
            CheckValidArrayAndThrow(data);

            var result = ACaptureRequest_setEntry_i64(this, tag, (uint)data.Length,
                (long*)data.GetUnsafeReadOnlyPtr());
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }

        public bool TrySetMetadataEntry(uint tag, NativeArray<float> data)
        {
            this.CheckNullAndThrow();
            CheckValidArrayAndThrow(data);

            var result = ACaptureRequest_setEntry_float(this, tag, (uint)data.Length,
                (float*)data.GetUnsafeReadOnlyPtr());
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }

        public bool TrySetMetadataEntry(uint tag, NativeArray<double> data)
        {
            this.CheckNullAndThrow();
            CheckValidArrayAndThrow(data);

            var result = ACaptureRequest_setEntry_double(this, tag, (uint)data.Length,
                (double*)data.GetUnsafeReadOnlyPtr());
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }

        public bool TrySetMetadataEntry(uint tag, NativeArray<ACameraMetadata.Rational> data)
        {
            this.CheckNullAndThrow();
            CheckValidArrayAndThrow(data);

            var result = ACaptureRequest_setEntry_rational(this, tag, (uint)data.Length,
                (ACameraMetadata.Rational*)data.GetUnsafeReadOnlyPtr());
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }

        [Conditional("DEVELOPMENT_BUILD")]
        private static void CheckValidArrayAndThrow<T>(NativeArray<T> array) where T : unmanaged
        {
            if (!array.IsCreated)
                throw new ArgumentNullException(nameof(array));

            if (array.Length == 0)
                throw new ArgumentException("Length of native array must be greater than 0");
        }
    }
}
