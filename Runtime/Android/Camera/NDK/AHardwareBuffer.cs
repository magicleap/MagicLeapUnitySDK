// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%


using UnityEngine.Serialization;

namespace MagicLeap.Android
{
    using System;
    using System.Collections;
    using Unity.Collections;

    public struct ARect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public override string ToString()
            => $"ARect[left = {Left}, top = {Top}, right = {Right}, bottom = {Bottom}]";
    }

    public readonly struct ImageDimensions : IEnumerable
    {
        public readonly int Height;
        public readonly int Width;

        public int Size => Width * Height;

        public ImageDimensions(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public NativeArray<T> CreateNativeArray<T>(Allocator allocator,
            NativeArrayOptions options = NativeArrayOptions.ClearMemory) where T : unmanaged
            => new NativeArray<T>(Size, allocator, options);

        IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();

        public ImageCoordinates GetCoordinatesFromIndex(int index)
            => new ImageCoordinates(index % Width, index / Width, index);

        public Enumerator GetEnumerator() => new Enumerator(this);

        public struct Enumerator : IEnumerator
        {
            private int current;
            private int stride;
            private int size;
            private int x;
            private int y;

            internal Enumerator(in ImageDimensions imgDim)
            {
                current = -1;
                size = imgDim.Size;
                stride = imgDim.Width;
                x = -1;
                y = -1;
            }

            public void Dispose() {}

            object IEnumerator.Current => throw new NotImplementedException();
            public ImageCoordinates Current => new ImageCoordinates(x, y, current);

            public bool MoveNext()
            {
                if (current >= size)
                    return false;

                var cur = current++;
                x = cur % stride;
                y = cur / stride;

                return true;
            }

            public void Reset()
            {
                current = -1;
                x = -1;
                y = -1;
            }
        }
    }

    public readonly struct ImageCoordinates
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Index;

        public ImageCoordinates(int x, int y, int index)
        {
            this.X = x;
            this.Y = y;
            this.Index = index;
        }

        public void Deconstruct(out int outX, out int outY)
        {
            outX = X;
            outY = Y;
        }

        public void Deconstruct(out int outX, out int outY, out int outIndex)
        {
            outX = X;
            outY = Y;
            outIndex = Index;
        }
    }
}

namespace MagicLeap.Android.NDK.NativeWindow
{
    using System;
    using System.Diagnostics;
    using Unity.Collections.LowLevel.Unsafe;

    using Media;
    using static NativeWindowNativeBindings;

    public struct AHardwareBuffer : INullablePointer
    {
        public struct Description
        {
            public uint Width;
            public uint Height;
            public uint Layers;
            public Format Format;
            public Usage Usage;
            public uint Stride;
            private uint reserved0;
            private ulong reserved1;

            public bool HasMultiplePlanes
                => Format is Format.Y8Cb8Cr8_420 or Format.YCbCrP010;

        }

        public enum Format
        {
            /// <summary>
            /// Corresponding formats:
            ///   Vulkan: VK_FORMAT_R8G8B8A8_UNORM
            ///   OpenGL ES: GL_RGBA8
            /// </summary>
            R8G8B8A8UNorm           = 1,

            /// <summary>
            /// 32 bits per pixel, 8 bits per channel format where alpha values are
            /// ignored (always opaque).
            /// Corresponding formats:
            ///   Vulkan: VK_FORMAT_R8G8B8A8_UNORM
            ///   OpenGL ES: GL_RGB8
            /// </summary>
            R8G8B8X8UNorm           = 2,

            /// <summary>
            /// Corresponding formats:
            ///   Vulkan: VK_FORMAT_R8G8B8_UNORM
            ///   OpenGL ES: GL_RGB8
            /// </summary>
            R8G8B8UNorm             = 3,

            /// <summary>
            /// Corresponding formats:
            ///   Vulkan: VK_FORMAT_R5G6B5_UNORM_PACK16
            ///   OpenGL ES: GL_RGB565
            /// </summary>
            R5G6B5UNorm             = 4,

            /// <summary>
            /// Corresponding formats:
            ///   Vulkan: VK_FORMAT_R16G16B16A16_SFLOAT
            ///   OpenGL ES: GL_RGBA16F
            /// </summary>
            R16G16B16A16Float       = 0x16,

            /// <summary>
            /// Corresponding formats:
            ///   Vulkan: VK_FORMAT_A2B10G10R10_UNORM_PACK32
            ///   OpenGL ES: GL_RGB10_A2
            /// </summary>
            R10G10B10A2UNorm        = 0x2b,

            /// <summary>
            /// Opaque binary blob format.
            /// Must have height 1 and one layer, with width equal to the buffer
            /// size in bytes. Corresponds to Vulkan buffers and OpenGL buffer
            /// objects. Can be bound to the latter using GL_EXT_external_buffer.
            /// </summary>
            Blob                     = 0x21,

            /// <summary>
            /// Corresponding formats:
            ///   Vulkan: VK_FORMAT_D16_UNORM
            ///   OpenGL ES: GL_DEPTH_COMPONENT16
            /// </summary>
            D16UNorm                = 0x30,

            /// <summary>
            /// Corresponding formats:
            ///   Vulkan: VK_FORMAT_X8_D24_UNORM_PACK32
            ///   OpenGL ES: GL_DEPTH_COMPONENT24
            /// </summary>
            D24UNorm                = 0x31,

            /// <summary>
            /// Corresponding formats:
            ///   Vulkan: VK_FORMAT_D24_UNORM_S8_UINT
            ///   OpenGL ES: GL_DEPTH24_STENCIL8
            /// </summary>
            D24UNormS8UInt        = 0x32,

            /// <summary>
            /// Corresponding formats:
            ///   Vulkan: VK_FORMAT_D32_SFLOAT
            ///   OpenGL ES: GL_DEPTH_COMPONENT32F
            /// </summary>
            D32Float                = 0x33,

            /// <summary>
            /// Corresponding formats:
            ///   Vulkan: VK_FORMAT_D32_SFLOAT_S8_UINT
            ///   OpenGL ES: GL_DEPTH32F_STENCIL8
            /// </summary>
            D32FloatS8UInt        = 0x34,

            /// <summary>
            /// Corresponding formats:
            ///   Vulkan: VK_FORMAT_S8_UINT
            ///   OpenGL ES: GL_STENCIL_INDEX8
            /// </summary>
            S8UInt                  = 0x35,

            /// <summary>
            /// YUV 420 888 format.
            /// Must have an even width and height. Can be accessed in OpenGL
            /// shaders through an external sampler. Does not support mip-maps
            /// cube-maps or multi-layered textures.
            /// </summary>
            Y8Cb8Cr8_420             = 0x23,

            /// <summary>
            /// YUV P010 format.
            /// Must have an even width and height. Can be accessed in OpenGL
            /// shaders through an external sampler. Does not support mip-maps
            /// cube-maps or multi-layered textures.
            /// </summary>
            YCbCrP010               = 0x36,

            /// <summary>
            /// Corresponding formats:
            ///   Vulkan: VK_FORMAT_R8_UNORM
            ///   OpenGL ES: GR_GL_R8
            /// </summary>
            R8UNorm                 = 0x38,
        }

        public unsafe struct Plane
        {
            [NativeDisableUnsafePtrRestriction]
            public byte* Data;
            public uint PixelStride;
            public uint RowStride;

            public bool IsValid => Data != null;
        }

        public unsafe struct Planes
        {
            private const uint kSizeOfPlaneInBytes = 16;
            private const uint kMaxPlanes = 4;

            private int planeCount;
            private int padding0;
            private fixed byte planesRaw[(int)(kSizeOfPlaneInBytes * kMaxPlanes)];

            public int PlaneCount => planeCount;


            public Plane this[int index]
            {
                get
                {
                    CheckPlaneIndexAndThrow(index);
                    fixed (byte* start = planesRaw)
                    {
                        return UnsafeUtility.ReadArrayElement<Plane>(start, index);
                    }
                }
                set
                {
                    CheckPlaneIndexAndThrow(index);
                    fixed (byte* start = planesRaw)
                    {
                        UnsafeUtility.WriteArrayElement(start, index, value);
                    }
                }
            }
            
            public Plane PlaneFor(int planeIdx)
            {
                CheckPlaneIndexAndThrow(planeIdx);
                fixed (byte* @base = planesRaw)
                {
                    return UnsafeUtility.ReadArrayElement<Plane>(@base, planeIdx);
                }
            }

            [Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            private void CheckPlaneIndexAndThrow(int planeIdx)
            {
                if (planeIdx < 0 || planeIdx >= PlaneCount)
                    throw new IndexOutOfRangeException(
                        $"plane index must be between 0 and {PlaneCount - 1}, inclusive");
            }
        }

        [Flags]
        public enum Usage : ulong
        {
            /// <summary>
            /// The buffer will never be locked for direct CPU reads using the
            /// AHardwareBuffer_lock() function. Note that reading the buffer
            /// using OpenGL or Vulkan functions or memory mappings is still
            /// allowed.
            /// </summary>
            CpuReadNever        = 0UL,

            /// <summary>
            /// The buffer will sometimes be locked for direct CPU reads using
            /// the AHardwareBuffer_lock() function. Note that reading the
            /// buffer using OpenGL or Vulkan functions or memory mappings
            /// does not require the presence of this flag.
            /// </summary>
            CpuReadRarely       = 2UL,

            /// <summary>
            /// The buffer will often be locked for direct CPU reads using
            /// the AHardwareBuffer_lock() function. Note that reading the
            /// buffer using OpenGL or Vulkan functions or memory mappings
            /// does not require the presence of this flag.
            /// </summary>
            CpuReadOften        = 3UL,

            /// <summary>
            /// CPU read value mask.
            /// </summary>
            CpuReadMask         = 0xFUL,

            /// <summary>
            /// The buffer will never be locked for direct CPU writes using the
            /// AHardwareBuffer_lock() function. Note that writing the buffer
            /// using OpenGL or Vulkan functions or memory mappings is still
            /// allowed.
            /// </summary>
            CpuWriteNever       = 0UL << 4,

            /// <summary>
            /// The buffer will sometimes be locked for direct CPU writes using
            /// the AHardwareBuffer_lock() function. Note that writing the
            /// buffer using OpenGL or Vulkan functions or memory mappings
            /// does not require the presence of this flag.
            /// </summary>
            CpuWriteRarely      = 2UL << 4,

            /// <summary>
            /// The buffer will often be locked for direct CPU writes using
            /// the AHardwareBuffer_lock() function. Note that writing the
            /// buffer using OpenGL or Vulkan functions or memory mappings
            /// does not require the presence of this flag.
            /// </summary>
            CpuWriteOften       = 3UL << 4,

            /// <summary>
            /// CPU write value mask.
            /// </summary>
            CpuWriteMask        = 0xFUL << 4,

            /// <summary>
            /// The buffer will be read from by the GPU as a texture.
            /// </summary>
            GpuSampledImage     = 1UL << 8,

            /// <summary>
            /// The buffer will be written to by the GPU as a framebuffer attachment.
            /// </summary>
            GpuFramebuffer       = 1UL << 9,

            /// <summary>
            /// The buffer will be written to by the GPU as a framebuffer
            /// attachment.
            ///
            /// Note that the name of this flag is somewhat misleading: it does
            /// not imply that the buffer contains a color format. A buffer with
            /// depth or stencil format that will be used as a framebuffer
            /// attachment should also have this flag. Use the equivalent flag
            /// AHARDWAREBUFFER_USAGE_GPU_FRAMEBUFFER to avoid this confusion.
            /// </summary>
            GpuColorOutput      = GpuFramebuffer,

            /// <summary>
            /// The buffer will be used as a composer HAL overlay layer.
            ///
            /// This flag is currently only needed when using ASurfaceTransaction_setBuffer
            /// to set a buffer. In all other cases, the framework adds this flag
            /// internally to buffers that could be presented in a composer overlay.
            /// ASurfaceTransaction_setBuffer is special because it uses buffers allocated
            /// directly through AHardwareBuffer_allocate instead of buffers allocated
            /// by the framework.
            /// </summary>
            ComposerOverlay      = 1UL << 11,

            /// <summary>
            /// The buffer is protected from direct CPU access or being read by
            /// non-secure hardware, such as video encoders.
            ///
            /// This flag is incompatible with CPU read and write flags. It is
            /// mainly used when handling DRM video. Refer to the EGL extension
            /// EGL_EXT_protected_content and GL extension
            /// GL_EXT_protected_textures for more information on how these
            /// buffers are expected to behave.
            /// </summary>
            ProtectedContent     = 1UL << 14,

            /// <summary>
            /// The buffer will be read by a hardware video encoder.
            /// </summary>
            VideoEncode          = 1UL << 16,

            /// <summary>
            /// The buffer will be used for direct writes from sensors.
            /// When this flag is present, the format must be AHARDWAREBUFFER_FORMAT_BLOB.
            /// </summary>
            SensorDirectData    = 1UL << 23,

            /// <summary>
            /// The buffer will be used as a shader storage or uniform buffer object.
            /// When this flag is present, the format must be AHARDWAREBUFFER_FORMAT_BLOB.
            /// </summary>
            GpuDataBuffer       = 1UL << 24,

            /// <summary>
            /// The buffer will be used as a cube map texture.
            /// When this flag is present, the buffer must have a layer count
            /// that is a multiple of 6. Note that buffers with this flag must be
            /// bound to OpenGL textures using the extension
            /// GL_EXT_EGL_image_storage instead of GL_KHR_EGL_image.
            /// </summary>
            GpuCubeMap          = 1UL << 25,

            /// <summary>
            /// The buffer contains a complete mipmap hierarchy.
            /// Note that buffers with this flag must be bound to OpenGL textures using
            /// the extension GL_EXT_EGL_image_storage instead of GL_KHR_EGL_image.
            /// </summary>
            GpuMipmapComplete   = 1UL << 26,

            Vendor0  = 1UL << 28,
            Vendor1  = 1UL << 29,
            Vendor2  = 1UL << 30,
            Vendor3  = 1UL << 31,
            Vendor4  = 1UL << 48,
            Vendor5  = 1UL << 49,
            Vendor6  = 1UL << 50,
            Vendor7  = 1UL << 51,
            Vendor8  = 1UL << 52,
            Vendor9  = 1UL << 53,
            Vendor10 = 1UL << 54,
            Vendor11 = 1UL << 55,
            Vendor12 = 1UL << 56,
            Vendor13 = 1UL << 57,
            Vendor14 = 1UL << 58,
            Vendor15 = 1UL << 59,
            Vendor16 = 1UL << 60,
            Vendor17 = 1UL << 61,
            Vendor18 = 1UL << 62,
            Vendor19 = 1UL << 63,
        }

        private IntPtr value;

        public Description description
        {
            get
            {
                AHardwareBuffer_describe(this, out var desc);
                return desc;
            }
        }

        public bool IsNull => value == IntPtr.Zero;

        public void Acquire()
        {
            this.CheckNullAndThrow();
            AHardwareBuffer_acquire(this);
        }

        public void Dispose()
        {
            if (!IsNull)
                ReleaseUnchecked();

            value = IntPtr.Zero;
        }

        public static bool IsSupported(Description description)
            => AHardwareBuffer_isSupported(ref description) == 1;

        public override string ToString()
            => $"0x{value.ToInt64():X}";

        public static bool TryAllocate(Description description, out AHardwareBuffer buffer)
            => AHardwareBuffer_allocate(ref description, out buffer) == 0;

        public unsafe bool TryLockData(Usage usageFlags, out void* outAddress)
        {
            this.CheckNullAndThrow();
            CheckMultiPlanarAndThrow(false);

            var ret = AHardwareBuffer_lock(this, usageFlags, -1, null, out outAddress);

            return CheckResultAndThrow(ret);
        }

        public bool TryLockPlaneData(Usage usageFlags, out Planes planes)
        {
            this.CheckNullAndThrow();
            CheckMultiPlanarAndThrow();

            int ret;

            unsafe
            {
                ret = AHardwareBuffer_lockPlanes(this, usageFlags, -1, null, out planes);
            }

            return CheckResultAndThrow(ret);
        }

        public bool TryUnlockData()
        {
            this.CheckNullAndThrow();

            int ret;

            unsafe
            {
                ret = AHardwareBuffer_unlock(this, null);
            }

            return CheckResultAndThrow(ret);
        }

        internal void ReleaseUnchecked()
            => AHardwareBuffer_release(this);
        
        [Conditional("DEVELOPMENT_BUILD")]
        internal void CheckMultiPlanarAndThrow(bool expectedMultiPlanar = true)
        {
            var valid = expectedMultiPlanar == description.HasMultiplePlanes;

            if (valid)
                return;

            if (expectedMultiPlanar)
                throw new InvalidOperationException(
                    $"Cannot perform a multi-planar operation on a buffer with a single plane");
            else
                throw new InvalidOperationException(
                    $"Cannot perform a single planar operation on a buffer with multiple planes");

        }

        private bool CheckResultAndThrow(int actual, int expected = 0)
        {
            bool success = expected == actual;
#if DEVELOPMENT_BUILD
            if (!success)
                throw new Exception($"expected result: {expected}, actual result was: {actual}");
#endif
            return success;
        }

    }

    public static class AHardwareBufferExtensions
    {
        public static void Release(ref this AHardwareBuffer buffer)
        {
            buffer.CheckNullAndThrow();
            buffer.ReleaseUnchecked();
        }

        public static int BytesPerPixel(this AHardwareBuffer.Format format)
        {
            switch (format)
            {
                case AHardwareBuffer.Format.R8G8B8A8UNorm:
                    return 4;
                case AHardwareBuffer.Format.R8G8B8X8UNorm:
                    return 4;
                case AHardwareBuffer.Format.R8G8B8UNorm:
                    return 3;
                case AHardwareBuffer.Format.R5G6B5UNorm:
                    return 2;
                case AHardwareBuffer.Format.R16G16B16A16Float:
                    return 8;
                case AHardwareBuffer.Format.R10G10B10A2UNorm:
                    return 4;
                case AHardwareBuffer.Format.Blob:
                    return 1;
                case AHardwareBuffer.Format.D16UNorm:
                    return 2;
                case AHardwareBuffer.Format.D24UNorm:
                    return 3;
                case AHardwareBuffer.Format.D24UNormS8UInt:
                    return 4;
                case AHardwareBuffer.Format.D32Float:
                    return 4;
                case AHardwareBuffer.Format.D32FloatS8UInt:
                    return 5;
                case AHardwareBuffer.Format.S8UInt:
                    return 1;
                case AHardwareBuffer.Format.Y8Cb8Cr8_420:
                    return 1;
                case AHardwareBuffer.Format.YCbCrP010:
                    return 2;
                case AHardwareBuffer.Format.R8UNorm:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        internal static MediaFormat ToMediaFormat(this AHardwareBuffer.Format format)
        {
            switch (format)
            {
                case AHardwareBuffer.Format.R8G8B8A8UNorm:
                    return MediaFormat.Rgba8888;
                case AHardwareBuffer.Format.R8G8B8X8UNorm:
                    return MediaFormat.Rgbx8888;
                case AHardwareBuffer.Format.R8G8B8UNorm:
                    return MediaFormat.Rgb888;
                case AHardwareBuffer.Format.R5G6B5UNorm:
                    return MediaFormat.Rgb565;
                case AHardwareBuffer.Format.R16G16B16A16Float:
                    return MediaFormat.Rgba_Fp16;
                case AHardwareBuffer.Format.D16UNorm:
                    return MediaFormat.Depth16;
                case AHardwareBuffer.Format.Y8Cb8Cr8_420:
                    return MediaFormat.Yuv_420_888;
                // AHardwareBuffer formats with no MediaFormat equivalent.
                case AHardwareBuffer.Format.YCbCrP010:
                case AHardwareBuffer.Format.R8UNorm:
                case AHardwareBuffer.Format.S8UInt:
                case AHardwareBuffer.Format.D32FloatS8UInt:
                case AHardwareBuffer.Format.D32Float:
                case AHardwareBuffer.Format.D24UNormS8UInt:
                case AHardwareBuffer.Format.D24UNorm:
                case AHardwareBuffer.Format.Blob:
                case AHardwareBuffer.Format.R10G10B10A2UNorm:
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }
    }
}
