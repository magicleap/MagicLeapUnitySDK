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
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Jobs;
    using UnityEngine.XR.MagicLeap.Unsafe;
    using NDK.Media;
    using NDK.NativeWindow;

    [NativeContainer]
    [NativeContainerIsReadOnly]
    public struct NativeImage : IDisposable
    {
        private struct DisposeData
        {
            public UnsafeImage image;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            public AtomicSafetyHandle m_Safety;
#endif
        }

        private struct DisposeJob : IJob
        {
            public DisposeData Data;

            public void Execute()
            {
                if (Data.image.IsCreated)
                    Data.image.Dispose();
            }
        }

        private UnsafeImage image;
        private IntPtr requestHandle;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private AtomicSafetyHandle m_Safety;

        internal static readonly int s_staticSafetyId = AtomicSafetyHandle.NewStaticSafetyId<NativeImage>();
#endif

        public ImageDimensions Dimensions => new ImageDimensions(Width, Height);

        public bool IsCreated => image.IsCreated;

        public bool IsMultiplanar => image.IsMultiPlanar;

        public int Height
        {
            get
            {
                CheckedCreatedAndThrow();
                return image.Height;
            }
        }

        public int NumberOfPlanes
        {
            get
            {
                CheckedCreatedAndThrow();
                return image.NumberOfPlanes;
            }
        }


        internal IntPtr RequestHandle
        {
            get
            {
                CheckedCreatedAndThrow();
                return requestHandle;
            }
        }

        public long Timestamp
        {
            get
            {
                CheckedCreatedAndThrow();
                return image.Timestamp;
            }
        }

        public int Width
        {
            get
            {
                CheckedCreatedAndThrow();
                return image.Width;
            }
        }

        internal NativeImage(UnsafeImage image, IntPtr requestHandle)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_Safety = AtomicSafetyHandle.Create();
            AtomicSafetyHandle.SetAllowReadOrWriteAccess(m_Safety, false);
            AtomicSafetyHandle.SetAllowSecondaryVersionWriting(m_Safety, false);

            AtomicSafetyHandle.SetStaticSafetyId(ref m_Safety, s_staticSafetyId);
#endif
            this.image = image;
            this.requestHandle = requestHandle;
        }

        public bool TryGetHardwareBuffer(out AHardwareBuffer hardwareBuffer)
        {
            hardwareBuffer = default;
            return image.IsCreated && image.TryGetHardwareBuffer(out hardwareBuffer);
        }

        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckDeallocateAndThrow(m_Safety);
            AtomicSafetyHandle.Release(m_Safety);
#endif
            if (!image.IsCreated)
                return;

            image.Dispose();
            requestHandle = IntPtr.Zero;
        }

        public JobHandle Dispose(JobHandle depends)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckDeallocateAndThrow(m_Safety);
#endif
            if (!image.IsCreated)
                return default;

            var handle = new DisposeJob
            {
                Data = new DisposeData
                {
                    image = image,
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    m_Safety = m_Safety,
#endif
                },
            }.Schedule(depends);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.Release(m_Safety);
#endif
            image = default;

            return handle;
        }

        private struct UnlockBufferJob : IJob
        {
            public UnsafeImage Source;

            public void Execute()
            {
                Source.TryUnlockData();
            }
        }

        public bool TryGetData(out NativePlane outData, out ImageDimensions outDim)
        {
            CheckedCreatedAndThrow();
            outData = default;
            outDim = new ImageDimensions(Width, Height);

            if (!TryLockDataInternal())
                return false;

            if (!TryGetDataInternal(out outData, out outDim))
                return false;

            if (!TryUnlockDataInternal())
                return false;

            return true;
        }

        internal bool TryGetDataInternal(out NativePlane outData, out ImageDimensions outDim)
        {
            outDim = new ImageDimensions(Width, Height);
            outData = default;

            if (!image.TryGetPlane(0, out var plane))
            {
                UnityEngine.Debug.LogError("Failed to acquire data");
                return false;
            }

            outData = new NativePlane(plane);

            return true;
        }

        public bool TryGetYUVData(out NativeYUVPlanes outYuvPlanes)
        {
            CheckedCreatedAndThrow();
            outYuvPlanes = default;
            if (!TryLockDataInternal())
                return false;

            if (!TryGetYUVDataUnchecked(out outYuvPlanes))
                return false;

            if (!TryUnlockDataInternal())
            {
                // TODO :: log the failure, but should the whole call fail, too?
                return false;
            }

            return true;
        }

        internal bool TryGetYUVDataUnchecked(out NativeYUVPlanes outYuvPlanes)
        {
            outYuvPlanes = default;

            NativePlane y = default, u = default, v = default;
            for (var i = 0; i < image.NumberOfPlanes; ++i)
            {
                if (!image.TryGetPlane(i, out var plane))
                    return false;
                switch (i)
                {
                    case 0:
                        y = new NativePlane(plane);
                        break;
                    case 1:
                        u = new NativePlane(plane);
                        break;
                    case 2:
                        v = new NativePlane(plane);
                        break;
                }
            }

            outYuvPlanes = new NativeYUVPlanes(Dimensions, NumberOfPlanes, y, u, v);

            return true;
        }

        internal bool TryLockDataInternal() => image.TryLockData();

        internal bool TryUnlockDataInternal() => image.TryUnlockData();

        internal JobHandle TryUnlockDataAsyncInternal(JobHandle dependency)
            => new UnlockBufferJob { Source = image }.Schedule(dependency);

        [Conditional("DEVELOPMENT_BUILD")]
        internal void CheckedCreatedAndThrow()
        {
            if (!IsCreated)
                throw new NullReferenceException();
        }
    }

    public static class NativeImageSourceUnsafeUtility
    {
        public static bool TryGetYUVDataAssumeLocked(this NativeImage self, out NativeYUVPlanes outYuvPlanes)
        {
            self.CheckedCreatedAndThrow();
            return self.TryGetYUVDataUnchecked(out outYuvPlanes);
        }

        public static bool TryLockData(this NativeImage self)
        {
            self.CheckedCreatedAndThrow();
            return self.TryLockDataInternal();
        }

        public static bool TryUnlockData(this NativeImage self)
        {
            self.CheckedCreatedAndThrow();
            return self.TryUnlockDataInternal();
        }

        public static JobHandle TryUnlockDataAsync(this NativeImage self, JobHandle dependency)
        {
            self.CheckedCreatedAndThrow();
            return self.TryUnlockDataAsyncInternal(dependency);
        }
    }

    [NativeContainer]
    [NativeContainerIsReadOnly]
    public struct NativePlane
    {
        private UnsafePlane _Plane;

        public bool IsValid => _Plane.IsValid;

        internal UnsafePlane UnsafePlane => _Plane;

        public int PixelStride
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _Plane.PixelStride;
        }

        public int RowStride
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _Plane.RowStride;
        }

        internal NativePlane(UnsafePlane plane)
        {
            _Plane = plane;
        }

        public NativeArray<byte> CopyToNativeArray(Allocator allocator)
            => _Plane.CopyToNativeArray(allocator);

        internal string DebugRepr(string name = null)
        {
            name = string.IsNullOrEmpty(name) ? nameof(NativePlane) : name;
            return _Plane.DebugRepr(name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetDataAtOffset<T>(int offset) where T : unmanaged
            => _Plane.GetDataAtOffset<T>(offset);
    }


    public readonly struct NativeYUVPlanes
    {
        public readonly NativePlane YPlane;
        public readonly NativePlane UPlane;
        public readonly NativePlane VPlane;

        public readonly ImageDimensions Dimensions;
        public readonly int PlaneCount;

        internal NativeYUVPlanes(ImageDimensions dim, int planeCount, NativePlane y, NativePlane u, NativePlane v)
        {
            Dimensions = dim;
            PlaneCount = planeCount;
            YPlane = y;
            UPlane = u;
            VPlane = v;
        }
    }

    internal static unsafe class AHardwareBufferImageBackend
    {
        private struct Backend
        {
            public AHardwareBuffer buffer;
            public ARect cropRect;
            public AHardwareBuffer.Description desc;
            public AHardwareBuffer.Planes planes;
            public void* data;
            public long timestamp;
            public int numPlanes;
            public byte isLocked;
        }

        public static UnsafeImage Create(AImage image, Allocator allocator)
        {
            var backend = UnsafeUtilityEx.CallocTracked(allocator, new Backend
            {
                buffer = image.HardwareBuffer,
                cropRect = image.CropRect,
                desc = image.HardwareBuffer.description,
                timestamp = image.TimestampInNanoseconds,
                numPlanes = image.NumberOfPlanes,
                isLocked = 0,
            });

            UnityEngine.Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, $"using AHardwareBuffer at {backend->buffer}");

            backend->buffer.Acquire();

            var vtable = new UnsafeImage.VTable
            {
                data = backend,
                destroy = &Destroy,
                getCropRect = &GetCropRect,
                getFormat = &GetFormat,
                getHeight = &GetHeight,
                getNumPlanes = &GetNumPlanes,
                getTimestamp = &GetTimestamp,
                getWidth = &GetWidth,
                tryGetPlane = &TryGetPlane,
                tryLockData = &TryLockData,
                tryUnlockData = &TryUnlockData,
                tryGetHardwareBuffer = &TryGetHardwareBuffer
            };

            return new UnsafeImage(vtable, allocator);
        }

        private static void Destroy(void* obj) => CheckPointerAndThrow(obj)->buffer.ReleaseUnchecked();

        private static ARect GetCropRect(void* obj) => CheckPointerAndThrow(obj)->cropRect;

        private static MediaFormat GetFormat(void* obj) => CheckPointerAndThrow(obj)->desc.Format.ToMediaFormat();

        private static int GetHeight(void* obj) => (int)CheckPointerAndThrow(obj)->desc.Height;

        private static int GetNumPlanes(void* obj) => CheckPointerAndThrow(obj)->numPlanes;

        private static long GetTimestamp(void* obj) => CheckPointerAndThrow(obj)->timestamp;

        private static int GetWidth(void* obj) => (int)CheckPointerAndThrow(obj)->desc.Width;

        private static bool TryGetHardwareBuffer(void* data, out AHardwareBuffer hardwareBuffer)
        {
            hardwareBuffer = CheckPointerAndThrow(data)->buffer;
            return true;
        }

        private static bool TryGetPlane(void* obj, int planeIdx, UnsafePlane* outPlane)
        {
            if (outPlane == null)
                return false;

            var backend = CheckPointerAndThrow(obj);
            bool didLock = false;
            if (backend->isLocked == 0)
            {
                if (!TryLockData(obj))
                    return false;
                didLock = true;
            }

            if (backend->desc.HasMultiplePlanes)
            {
                var plane = backend->planes.PlaneFor(planeIdx);
                *outPlane = new UnsafePlane(plane, backend->desc);
            }
            else
            {
                *outPlane = new UnsafePlane((byte*)backend->data, new DataLayout(backend->desc));
            }

            if (didLock)
            {
                if (!TryUnlockData(obj))
                    return false;
            }

            return true;
        }

        private static bool TryLockData(void* obj)
        {
            var backend = CheckPointerAndThrow(obj);
            var success = (backend->desc.HasMultiplePlanes)
                ? backend->buffer.TryLockPlaneData(AHardwareBuffer.Usage.CpuReadRarely, out backend->planes)
                : backend->buffer.TryLockData(AHardwareBuffer.Usage.CpuReadRarely, out backend->data);
            if (success)
                backend->isLocked = 1;
            return success;
        }

        private static bool TryUnlockData(void* obj)
        {
            var backend = CheckPointerAndThrow(obj);
            var success = backend->buffer.TryUnlockData();
            backend->isLocked = 0;
            return success;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Backend* CheckPointerAndThrow(void* obj)
        {
#if DEVELOPMENT_BUILD || ENABLE_UNITY_COLLECTIONS_CHECKS
            if (obj == null)
                throw new NullReferenceException();
#endif
            return UnsafeUtilityEx.PunTypeUnchecked<Backend>(obj);
        }
    }

    internal static unsafe class AImageBackend
    {
        private struct Backend
        {
            public AImage image;
        }

        public static UnsafeImage Create(AImage image, Allocator allocator)
        {
            var backend = UnsafeUtilityEx.CallocTracked(allocator, new Backend
            {
                image = image,
            });

            var vtable = new UnsafeImage.VTable
            {
                data = backend,
                destroy = &Destroy,
                getCropRect = &GetCropRect,
                getFormat = &GetFormat,
                getHeight = &GetHeight,
                getNumPlanes = &GetNumPlanes,
                getTimestamp = &GetTimestamp,
                getWidth = &GetWidth,
                tryGetPlane = &TryGetPlane,
                tryLockData = null,
                tryUnlockData = null,
                tryGetHardwareBuffer = &TryGetHardwareBuffer
            };
            
            return new UnsafeImage(vtable, allocator);
        }

        private static void Destroy(void* obj) => CheckPointerAndThrow(obj)->image.Dispose();

        private static ARect GetCropRect(void* obj)
            => CheckPointerAndThrow(obj)->image.CropRect;

        private static MediaFormat GetFormat(void* obj) => CheckPointerAndThrow(obj)->image.Format;

        private static int GetHeight(void* obj) => CheckPointerAndThrow(obj)->image.Height;

        private static int GetNumPlanes(void* obj) => CheckPointerAndThrow(obj)->image.NumberOfPlanes;

        private static long GetTimestamp(void* obj) => CheckPointerAndThrow(obj)->image.TimestampInNanoseconds;

        private static int GetWidth(void* obj) => (int)CheckPointerAndThrow(obj)->image.Width;

        private static bool TryGetPlane(void* obj, int planeIdx, UnsafePlane* outPlane)
        {
            if (outPlane == null)
                return false;

            var image = CheckPointerAndThrow(obj)->image;

            var success = image.TryGetPlaneData(planeIdx, out var dataLength, out var pixelStride, out var rowStride, out var buffer);
            if (!success)
                return false;

            var layout = new DataLayout(image.Width, image.Height, pixelStride, rowStride,
                image.Format.BytesPerPixel());
            *outPlane = new UnsafePlane(buffer, layout);
            return true;
        }

        private static bool TryGetHardwareBuffer(void* data, out AHardwareBuffer hardwareBuffer)
        {
            hardwareBuffer = CheckPointerAndThrow(data)->image.HardwareBuffer;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Backend* CheckPointerAndThrow(void* obj)
        {
#if DEVELOPMENT_BUILD || ENABLE_UNITY_COLLECTIONS_CHECKS
            if (obj == null)
                throw new NullReferenceException();
#endif
            return UnsafeUtilityEx.PunTypeUnchecked<Backend>(obj);
        }
    }
}
