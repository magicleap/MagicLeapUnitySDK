namespace MagicLeap.Android
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    using UnityEngine.XR.MagicLeap.Unsafe;
    using NDK.Media;
    using NDK.NativeWindow;


    internal unsafe struct UnsafeImage : IDisposable
    {
        public struct VTable
        {
            [NativeDisableUnsafePtrRestriction]
            public void* data;
            [NativeDisableUnsafePtrRestriction]
            public delegate*<void*, void> destroy;
            [NativeDisableUnsafePtrRestriction]
            public delegate*<void*, ARect> getCropRect;
            [NativeDisableUnsafePtrRestriction]
            public delegate*<void*, MediaFormat> getFormat;
            [NativeDisableUnsafePtrRestriction]
            public delegate*<void*, int> getHeight;
            [NativeDisableUnsafePtrRestriction]
            public delegate*<void*, int> getNumPlanes;
            [NativeDisableUnsafePtrRestriction]
            public delegate*<void*, long> getTimestamp;
            [NativeDisableUnsafePtrRestriction]
            public delegate*<void*, int> getWidth;
            [NativeDisableUnsafePtrRestriction]
            public delegate*<void*, int, UnsafePlane*, bool> tryGetPlane;
            [NativeDisableUnsafePtrRestriction]
            public delegate*<void*, bool> tryLockData;
            [NativeDisableUnsafePtrRestriction]
            public delegate*<void*, bool> tryUnlockData;
            [NativeDisableUnsafePtrRestriction] 
            public delegate*<void*, out AHardwareBuffer, bool> tryGetHardwareBuffer;
        }

        private VTable* vTable;
        private Allocator allocator;

        internal Allocator Allocator => allocator;

        public ARect CropRect
        {
            get
            {
                CheckVTableAndThrow();
                return vTable->getCropRect(vTable->data);
            }
        }

        public MediaFormat Format
        {
            get
            {
                CheckVTableAndThrow();
                return vTable->getFormat(vTable->data);
            }
        }

        public int Height
        {
            get
            {
                CheckVTableAndThrow();
                return vTable->getHeight(vTable->data);
            }
        }

        public bool IsCreated => vTable != null;

        public bool IsMultiPlanar => NumberOfPlanes >= 1;

        public int NumberOfPlanes
        {
            get
            {
                CheckVTableAndThrow();
                return vTable->getNumPlanes(vTable->data);
            }
        }

        public long Timestamp
        {
            get
            {
                CheckVTableAndThrow();
                return vTable->getTimestamp(vTable->data);
            }
        }

        public int Width
        {
            get
            {
                CheckVTableAndThrow();
                return vTable->getWidth(vTable->data);
            }
        }

        public UnsafeImage(VTable vtable, Allocator allocator)
        {
            this.allocator = allocator;
            vTable = UnsafeUtilityEx.CallocTracked(this.allocator, vtable);
        }

        public void Dispose()
        {
            CheckVTableAndThrow();

            vTable->destroy(vTable->data);
            UnsafeUtility.FreeTracked(vTable, allocator);
            vTable = null;
            allocator = Allocator.Invalid;
        }

        public bool TryGetHardwareBuffer(out AHardwareBuffer hardwareBuffer)
        {
            CheckVTableAndThrow();
            if (vTable->tryGetHardwareBuffer != null)
            {
                return vTable->tryGetHardwareBuffer(vTable->data, out hardwareBuffer);
            }
            hardwareBuffer = default;
            return false;
        }
        

        public bool TryGetPlane(int planeIdx, out UnsafePlane outPlane)
        {
            CheckVTableAndThrow();
            CheckPlaneIndexAndThrow(planeIdx);

            UnsafePlane p = default;
            var success = vTable->tryGetPlane(vTable->data, planeIdx, &p);
            outPlane = success ? p : default;
            return success;
        }

        public bool TryLockData()
        {
            CheckVTableAndThrow();

            if (vTable->tryLockData == null)
                return true;

            return vTable->tryLockData(vTable->data);
        }

        public bool TryUnlockData()
        {
            CheckVTableAndThrow();

            if (vTable->tryUnlockData == null)
                return true;

            return vTable->tryUnlockData(vTable->data);
        }

        [Conditional("DEVELOPMENT_BUILD")]
        private void CheckPlaneIndexAndThrow(int planeIdx)
        {
            int planeCount = NumberOfPlanes;
            if (planeIdx < 0 || planeIdx >= planeCount)
                throw new IndexOutOfRangeException(
                    $"plane index must be between 0 and {planeCount - 1}, inclusive");
        }

        [Conditional("DEVELOPMENT_BUILD")]
        private void CheckVTableAndThrow()
        {
            if (vTable == null)
                throw new NullReferenceException();
        }
    }

    internal readonly struct DataLayout
    {
        public readonly int BytesPerPixel;
        public readonly int Height;
        public readonly int PixelStride;
        public readonly int RowStride;
        public readonly int Width;

        public int DataLength
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => UnityEngine.Mathf.Max(RowStride, Width * BytesPerPixel) * Height;
        }

        public int PixelCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Width * Height;
        }

        public DataLayout(int width, int height, int pixelStride, int rowStride, int bytesPerPixel)
        {
            BytesPerPixel = bytesPerPixel;
            Height = height;
            PixelStride = pixelStride;
            RowStride = rowStride;
            Width = width;
        }

        public DataLayout(AHardwareBuffer.Description description)
        {
            BytesPerPixel = description.Format.BytesPerPixel();
            Height = (int)description.Height;
            PixelStride = description.Format.BytesPerPixel();
            RowStride = (int)description.Stride;
            Width = (int)description.Width;
        }

        public string Debug()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("DataLayout:");
            sb.AppendLine($"\tWidth = {Width}, Height = {Height}");
            sb.AppendLine($"\tPixelStride = {PixelStride}. RowStride = {RowStride}");
            sb.AppendLine($"\tBytesPerPixel = {BytesPerPixel}");
            return sb.ToString();
        }
    }

    internal unsafe struct UnsafePlane
    {
            [NativeDisableUnsafePtrRestriction]
            private byte* data;
            private DataLayout layout;

            public byte* Data => data;

            public int DataLength => layout.DataLength;

            public bool IsValid => data != null;

            public int PixelStride
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => layout.PixelStride;
            }

            public int RowStride
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => layout.RowStride;
            }

            internal UnsafePlane(byte* data, DataLayout layout)
            {
                this.data = data;
                this.layout = layout;
            }

            internal UnsafePlane(AHardwareBuffer.Plane plane, AHardwareBuffer.Description desc)
            {
                data = plane.Data;
                layout = new DataLayout((int)desc.Width, (int)desc.Height, (int)plane.PixelStride, (int)plane.RowStride,
                    desc.Format.BytesPerPixel());
            }

            public NativeArray<byte> CopyToNativeArray(Allocator allocator)
            {
                var array = new NativeArray<byte>(layout.DataLength, allocator, NativeArrayOptions.ClearMemory);
                var ptr = array.GetUnsafePtr();
                UnsafeUtility.MemCpy(ptr, data, layout.DataLength);
                return array;
            }

            internal string DebugRepr(string name = null)
            {
                name = string.IsNullOrEmpty(name) ? nameof(UnsafeImage) : name;
                return $"{name}[ _Data = {UnsafeUtilityEx.FormatAddress(data)}, _Layout = {layout.Debug()} ]";
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T GetDataAtOffset<T>(int offset) where T : unmanaged
            {
                CheckOffsetBoundsAndThrow<T>(offset);
                UnsafeUtilityEx.CheckTypeSizeAndThrow<T>(layout.BytesPerPixel);
                return GetDataAtOffsetUnchecked<T>(offset);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T GetDataAtOffsetUnchecked<T>(int offset) where T : unmanaged
                => UnsafeUtility.ReadArrayElement<T>(data, offset);

            [Conditional("DEVELOPMENT_BUILD")]
            private void CheckOffsetBoundsAndThrow<T>(int offset) where T : unmanaged
            {
                var length = layout.DataLength;
                var sz = offset + sizeof(T);

                if (sz >= length)
                    throw new IndexOutOfRangeException(
                        $"Trying to access data at offset 0x{offset:X} with size 0x{sizeof(T):X}, but length is only 0x{length:X}");
            }
    }
}
