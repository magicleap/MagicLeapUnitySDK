// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace MagicLeap.Android.NDK.Media
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using NativeWindow;

    using static MediaNativeBindings;

    public struct AImage : INullablePointer
    {

        private IntPtr value;

        public ARect CropRect
        {
            get
            {
                var result = AImage_getCropRect(this, out var cropRect);
                result.CheckReturnValueAndThrow();
                return cropRect;
            }
        }

        public MediaFormat Format
        {
            get
            {
                var result = AImage_getFormat(this, out var format);
                result.CheckReturnValueAndThrow();
                return format;
            }
        }

        public AHardwareBuffer HardwareBuffer
        {
            get
            {
                var result = AImage_getHardwareBuffer(this, out var buffer);
                result.CheckReturnValueAndThrow();
                return buffer;
            }
        }

        public int Height
        {
            get
            {
                var result = AImage_getHeight(this, out var height);
                result.CheckReturnValueAndThrow();
                return height;
            }
        }

        public bool IsNull => value == IntPtr.Zero;

        public int NumberOfPlanes
        {
            get
            {
                var result = AImage_getNumberOfPlanes(this, out var numPlanes);
                result.CheckReturnValueAndThrow();
                return numPlanes;
            }
        }

        public long TimestampInNanoseconds
        {
            get
            {
                var result = AImage_getTimestamp(this, out var timestampNs);
                result.CheckReturnValueAndThrow();
                return timestampNs;
            }
        }

        public int Width
        {
            get
            {
                var result = AImage_getWidth(this, out var width);
                result.CheckReturnValueAndThrow();
                return width;
            }
        }

        public void Dispose()
        {
            if (!IsNull)
                AImage_delete(this);

            value = IntPtr.Zero;
        }

        public unsafe bool TryGetPlaneData(int planeIdx, out int dataLength, out int pixelStride, out int rowStride, out byte* data)
        {
            this.CheckNullAndThrow();
            CheckPlaneIndexAndThrow(planeIdx);

            dataLength = 0;
            pixelStride = 0;
            rowStride = 0;
            data = null;

            var result = AImage_getPlanePixelStride(this, planeIdx, out pixelStride);
            result.CheckReturnValueAndThrow();
            if (result != MediaStatus.Ok)
                return false;

            result = AImage_getPlaneRowStride(this, planeIdx, out rowStride);
            result.CheckReturnValueAndThrow();
            if (result != MediaStatus.Ok)
                return false;

            result = AImage_getPlaneData(this, planeIdx, out data, out dataLength);
            result.CheckReturnValueAndThrow();
            return result == MediaStatus.Ok;
        }

        [Conditional("DEVELOPMENT_BUILD")]
        private void CheckPlaneIndexAndThrow(int planeIdx)
        {
            var numPlanes = NumberOfPlanes;
            if (planeIdx < 0 || planeIdx >= numPlanes)
                throw new IndexOutOfRangeException($"plane index must be between 0 and {numPlanes - 1}, inclusive");
        }
    }
}
