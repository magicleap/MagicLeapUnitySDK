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
    using System.Runtime.InteropServices;
    using NativeWindow;

    using static MediaNativeBindings;

    public struct AImageReader : INullablePointer
    {
        public struct ImageListener
        {
            private IntPtr context;
            private IntPtr onImageAvailable;

            public bool IsCreated => onImageAvailable != IntPtr.Zero;

            public static ImageListener Create(Action<IntPtr, AImageReader> onImageAvailable, IntPtr context = default)
            {
                if (onImageAvailable == null)
                    throw new ArgumentNullException(nameof(onImageAvailable));

                return new ImageListener
                {
                    context = context,
                    onImageAvailable = Marshal.GetFunctionPointerForDelegate(onImageAvailable),
                };
            }
        }

        private IntPtr value;

        public bool IsNull => value == IntPtr.Zero;

        public MediaFormat Format
        {
            get
            {
                var result = AImageReader_getFormat(this, out var format);
                result.CheckReturnValueAndThrow();
                return format;
            }
        }

        public int Height
        {
            get
            {
                var result = AImageReader_getHeight(this, out var height);
                result.CheckReturnValueAndThrow();
                return height;
            }
        }

        public int MaxImages
        {
            get
            {
                var result = AImageReader_getMaxImages(this, out var maxImages);
                result.CheckReturnValueAndThrow();
                return maxImages;
            }
        }

        public int Width
        {
            get
            {
                var result = AImageReader_getWidth(this, out var width);
                result.CheckReturnValueAndThrow();
                return width;
            }
        }

        public ANativeWindow Window
        {
            get
            {
                var result = AImageReader_getWindow(this, out var window);
                result.CheckReturnValueAndThrow();
                return window;
            }
        }

        public void Dispose()
        {
            if (!IsNull)
                AImageReader_delete(this);

            value = IntPtr.Zero;
        }

        public bool TryAcquireLatestImage(out AImage outImage)
        {
            this.CheckNullAndThrow();
            return AImageReader_acquireLatestImage(this, out outImage) == MediaStatus.Ok;
        }

        public bool TryAcquireNextImage(out AImage outImage)
        {
            this.CheckNullAndThrow();
            return AImageReader_acquireNextImage(this, out outImage) == MediaStatus.Ok;
        }

        public static bool TryCreate(int width, int height, MediaFormat format, int maxImages,
            out AImageReader reader)
        {
            var result = AImageReader_new(width, height, format, maxImages, out reader);
            return result == MediaStatus.Ok;
        }

        public static bool TryCreate(int width, int height, MediaFormat format, AHardwareBuffer.Usage usage, int maxImages, out AImageReader reader)
            => AImageReader_newWithUsage(width, height, format, usage, maxImages, out reader) == MediaStatus.Ok;

        public bool TrySetImageListener(ImageListener listener)
        {
            this.CheckNullAndThrow();
            if (!listener.IsCreated)
                return false;

            return AImageReader_setImageListener(this, ref listener) == MediaStatus.Ok;
        }
    }
}
