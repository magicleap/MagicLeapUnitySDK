using System;
using System.Collections.Generic;
using MagicLeap.Android.NDK.NativeWindow;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.XR.MagicLeap
{
    public class YCbCrHardwareBufferRenderer : YCbCrRenderer, YCbCrRenderer.IHardwareBufferProvider
    {
        private record Plane
        {
            public uint PixelStride;
            public uint RowStride;
            public NativeArray<byte> Data;
        }
        
        private AHardwareBuffer hardwareBuffer;
        
        private readonly Queue<List<Plane>> queuedFrames = new();

        private bool HasBufferBeenCreated => !hardwareBuffer.IsNull;

        private readonly IHardwareBufferProvider hardwareBufferProviderOverride;

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
        
        public YCbCrHardwareBufferRenderer()
        {
            Initialize();
        }

        public YCbCrHardwareBufferRenderer(IHardwareBufferProvider hardwareBufferProvider)
        : this()
        {
            hardwareBufferProviderOverride = hardwareBufferProvider;
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
                Usage = AHardwareBuffer.Usage.CpuWriteOften | AHardwareBuffer.Usage.GpuSampledImage,
            };
            
            if (!AHardwareBuffer.TryAllocate(description, out hardwareBuffer))
            {
                Debug.LogError("Unable to allocate hardware buffer");
                return;
            }
            //Acquire to keep the buffer around
            hardwareBuffer.Acquire();
        }


        bool IHardwareBufferProvider.AcquireNextAvailableHwBuffer(out IntPtr hwBuffer)
        {
            unsafe
            {
                if (hardwareBufferProviderOverride != null)
                {
                    return hardwareBufferProviderOverride.AcquireNextAvailableHwBuffer(out hwBuffer);
                }
            
                hwBuffer = IntPtr.Zero;
                if (!HasBufferBeenCreated || queuedFrames.Count == 0)
                {
                    return false;
                }

                hardwareBuffer.Acquire();
                if (!hardwareBuffer.TryLockPlaneData(AHardwareBuffer.Usage.CpuWriteOften, out var planes))
                {
                    return false;
                }

                var currentFrame = queuedFrames.Dequeue();
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
                hwBuffer = hardwareBuffer.GetNativeHandle();
                return true;
            }
        }

        public void SetPlaneData(int planeIndex, uint rowStride, uint pixelStride, NativeArray<byte> planeData)
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
                planeList = queuedFrames.Peek();
            }

            var plane = new Plane
            {
                PixelStride = pixelStride,
                RowStride = rowStride,
                Data = planeData
            };
            
            planeList.Add(plane);
        }

        void IHardwareBufferProvider.ReleaseHwBuffer(IntPtr hwBuffer)
        {
            if (hardwareBufferProviderOverride != null)
            {
                hardwareBufferProviderOverride.ReleaseHwBuffer(hwBuffer);
                return;
            }
            
            if (!HasBufferBeenCreated)
            {
                return;
            }
            hardwareBuffer.Release();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            DestroyHardwareBuffer();
        }
    }
}
