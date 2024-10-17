using System.Collections.Concurrent;
using System.Collections.Generic;
using MagicLeap.Android;
using MagicLeap.Android.NDK.NativeWindow;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.XR.MagicLeap
{
    public class YCbCrHardwareBufferRenderer : YCbCrRenderer, YCbCrRenderer.IHardwareBufferProvider, YCbCrRenderer.IFrameTransformMatrixProvider
    {
        private record Plane
        {
            public uint PixelStride;
            public uint RowStride;
            public NativeArray<byte> Data;
        }
        
        private AHardwareBuffer hardwareBuffer;
        private readonly ConcurrentQueue<List<Plane>> queuedFrames = new();
        private readonly ConcurrentQueue<AHardwareBuffer> queuedHardwareBuffers = new();
        private bool HasBufferBeenCreated => !hardwareBuffer.IsNull;
        private readonly IHardwareBufferProvider hardwareBufferProviderOverride;
        private bool ShouldFlipFrame { get; }
        
        private unsafe struct UVCopyJob : IJob
        {
            [ReadOnly] public NativeArray<byte> UPlaneSource;
            [NativeDisableUnsafePtrRestriction] public byte* UPlane;
            public uint InputRowStride;
            public uint InputPixelStride;

            public uint OutputURowStride;
            public uint OutputUPixelStride;


            [ReadOnly] public NativeArray<byte> VPlaneSource;
            [NativeDisableUnsafePtrRestriction] public byte* VPlane;
            public uint OutputVRowStride;
            public uint OutputVPixelStride;


            public void Execute()
            {
                var currentUDest = UPlane;
                var currentVDest = VPlane;

                var uSource = (byte*)UPlaneSource.GetUnsafePtr();
                var vSource = (byte*)VPlaneSource.GetUnsafePtr();
                //Copy over each plane
                for (var i = 0; i < UPlaneSource.Length / InputRowStride; i++)
                {
                    UnsafeUtility.MemCpyStride(currentUDest, (int)OutputUPixelStride, uSource, (int)InputPixelStride, 1, (int)InputRowStride);
                    UnsafeUtility.MemCpyStride(currentVDest, (int)OutputVPixelStride, vSource, (int)InputPixelStride, 1, (int)InputRowStride);

                    currentUDest += OutputURowStride;
                    currentVDest += OutputVRowStride;

                    uSource += InputRowStride;
                    vSource += InputRowStride;
                }
            }
        }

        /// <summary>
        ///     Create a YCbCrHardwareBufferRenderer object (NOTE: Cleanup has to be done manually by calling Dispose)
        /// </summary>
        /// <param name="shouldFlipFrame">Whether to flip the YUV data for rendering</param>
        public YCbCrHardwareBufferRenderer(bool shouldFlipFrame = true)
        {
            ShouldFlipFrame = shouldFlipFrame;
            Initialize();
        }

        /// <summary>
        ///     Create a YCbCrHardwareBufferRenderer object. (NOTE: Cleanup has to be done manually by calling Dispose)
        /// </summary>
        /// <param name="hardwareBufferProvider">
        ///     The IHardwareBufferProvider override that the renderer will use instead of waiting
        ///     for the data to be set manually
        /// </param>
        /// <param name="shouldFlipFrame">Whether to flip the YUV data for rendering</param>
        public YCbCrHardwareBufferRenderer(IHardwareBufferProvider hardwareBufferProvider, bool shouldFlipFrame = true) : this(shouldFlipFrame)
        {
            hardwareBufferProviderOverride = hardwareBufferProvider;
        }

        /// <summary>
        ///     Queue up a plane data to the next rendered frame. Planes should be submitted in the order of Y,U,V
        ///     NOTE: Plane data should be of the format NV12 (after subsampling), so it should be a full Y plane followed by an
        ///     interleaved U and V half-plane
        ///     The data will only be copied over the next render call, so it is the caller's responsibility to make sure the data
        ///     persists till then
        /// </summary>
        /// <param name="rowStride">The row stride of the plane</param>
        /// <param name="pixelStride">The pixel stride of the plane</param>
        /// <param name="planeData">The raw byte data of the plane</param>
        public void SetPlaneData(uint rowStride, uint pixelStride, NativeArray<byte> planeData)
        {
            if (!HasBufferBeenCreated)
            {
                CreateHardwareBuffer((uint)RenderTarget.width, (uint)RenderTarget.height);
            }

            List<Plane> planeList;
            if (queuedFrames.Count == 0)
            {
                planeList = new List<Plane>();
                queuedFrames.Enqueue(planeList);
            }
            else
            {
                if (!queuedFrames.TryPeek(out planeList))
                {
                    return;
                }
            }

            var plane = new Plane
            {
                PixelStride = pixelStride,
                RowStride = rowStride,
                Data = planeData
            };

            planeList.Add(plane);
        }

        /// <summary>
        ///     Queue an AHardwareBuffer directly for rendering. The buffer is directly passed to the native YCbCr renderer and is
        ///     acquired only the next draw call
        /// </summary>
        /// <param name="hwBuffer">The </param>
        public void QueueHardwareBuffer(AHardwareBuffer hwBuffer)
        {
            queuedHardwareBuffers.Enqueue(hwBuffer);
        }

        /// <summary>
        ///     Queue up a NativeImage for rendering
        /// </summary>
        /// <param name="nativeImage">The native image to render next</param>
        public void QueueNativeImage(NativeImage nativeImage)
        {
            if (nativeImage.TryGetHardwareBuffer(out var hwBuffer))
            {
                QueueHardwareBuffer(hwBuffer);
            }
        }

        private void DestroyHardwareBuffer()
        {
            if (!hardwareBuffer.IsNull)
            {
                hardwareBuffer.Dispose();
            }
        }

        private void CreateHardwareBuffer(uint width, uint height)
        {
            if (hardwareBufferProviderOverride != null)
            {
                hardwareBufferProviderOverride.AcquireNextAvailableHwBuffer(out _);
                return;
            }

            DestroyHardwareBuffer();
            var description = new AHardwareBuffer.Description
            {
                Width = width,
                Height = height,
                Layers = 1,
                Format = AHardwareBuffer.Format.Y8Cb8Cr8_420,
                Usage = AHardwareBuffer.Usage.CpuWriteOften | AHardwareBuffer.Usage.GpuSampledImage
            };

            if (!AHardwareBuffer.TryAllocate(description, out hardwareBuffer))
            {
                Debug.LogError("Unable to allocate hardware buffer");
                return;
            }

            //Acquire to keep the buffer around
            hardwareBuffer.Acquire();
        }


        bool IHardwareBufferProvider.AcquireNextAvailableHwBuffer(out AHardwareBuffer hwBuffer)
        {
            unsafe
            {
                //if a hardwarebuffer provider override was given, use that for buffer acquisition
                if (hardwareBufferProviderOverride != null)
                {
                    return hardwareBufferProviderOverride.AcquireNextAvailableHwBuffer(out hwBuffer);
                }

                // If hardware buffers were queued, use that
                if (queuedHardwareBuffers.Count > 0)
                {
                    if (queuedHardwareBuffers.TryDequeue(out var queuedBuffer))
                    {
                        queuedBuffer.Acquire();
                        hwBuffer = queuedBuffer;
                        return true;
                    }

                    hwBuffer = default;
                    return false;
                }

                hwBuffer = default;
                if (!HasBufferBeenCreated || queuedFrames.Count == 0)
                {
                    return false;
                }

                hardwareBuffer.Acquire();
                if (!hardwareBuffer.TryLockPlaneData(AHardwareBuffer.Usage.CpuWriteOften, out var planes))
                {
                    return false;
                }

                if (!queuedFrames.TryDequeue(out var currentFrame))
                {
                    return false;
                }

                if (currentFrame.Count != 3)
                {
                    return false;
                }

                var width = (int)currentFrame[0].RowStride;
                //Y plane is direct memcpy with stride
                var yPlane = planes[0];
                var uPlane = planes[1];
                var vPlane = planes[2];

                var sourceYPlane = currentFrame[0];
                var sourceUPlane = currentFrame[1];
                var sourceVPlane = currentFrame[2];

                if (width != 0)
                {
                    UnsafeUtility.MemCpyStride(yPlane.Data, (int)yPlane.RowStride, sourceYPlane.Data.GetUnsafePtr(), width, width, sourceYPlane.Data.Length / width);
                }

                if (sourceUPlane.Data.Length > 0 && sourceVPlane.Data.Length > 0)
                {
                    new UVCopyJob
                    {
                        UPlaneSource = sourceUPlane.Data,
                        UPlane = uPlane.Data,
                        InputRowStride = sourceUPlane.RowStride,
                        OutputURowStride = uPlane.RowStride,
                        VPlaneSource = sourceVPlane.Data,
                        VPlane = vPlane.Data,
                        OutputVRowStride = vPlane.RowStride,
                        OutputUPixelStride = uPlane.PixelStride,
                        OutputVPixelStride = vPlane.PixelStride,
                        InputPixelStride = sourceVPlane.PixelStride
                    }.Schedule().Complete();
                }

                hardwareBuffer.TryUnlockData();
                hwBuffer = hardwareBuffer;
                return true;
            }
        }

        void IHardwareBufferProvider.ReleaseHwBuffer(AHardwareBuffer hwBuffer)
        {
            if (hardwareBufferProviderOverride != null)
            {
                hardwareBufferProviderOverride.ReleaseHwBuffer(hwBuffer);
                return;
            }

            if (hwBuffer.IsNull)
            {
                return;
            }

            hwBuffer.Release();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            DestroyHardwareBuffer();
        }

        bool IFrameTransformMatrixProvider.GetFrameTransformMatrix(ref Matrix4x4 frameTransformMatColMajor)
        {
            if (!ShouldFlipFrame)
            {
                return true;
            }

            frameTransformMatColMajor = Matrix4x4.identity;
            frameTransformMatColMajor.m11 = -1;
            frameTransformMatColMajor.m13 = 1;
            return true;
        }
    }
}
