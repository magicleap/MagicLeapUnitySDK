﻿using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace MagicLeap.OpenXR.Features.PixelSensors
{
    /// <summary>
    /// Represents a plane from a sensor frame
    /// </summary>
    public struct PixelSensorPlane
    {
        internal XrPixelSensorPlane NativePlane;
        public uint Width => NativePlane.Width;
        public uint Height => NativePlane.Height;
        public uint Stride => NativePlane.Stride;
        public uint BytesPerPixel => NativePlane.BytesPerPixel;
        public uint PixelStride => NativePlane.PixelStride;
        public NativeArray<byte> ByteData;
    }

    /// <summary>
    /// Represents the frame data of a sensor
    /// </summary>
    public struct PixelSensorFrame
    {
        /// <summary>
        /// Whether the frame is valid or not.
        /// </summary>
        public bool IsValid { get; private set; }

        private XrPixelSensorFrame internalFrame;

        /// <summary>
        /// The type of the frame
        /// </summary>
        public PixelSensorFrameType FrameType => internalFrame.FrameType;

        public PixelSensorPlane[] Planes;
        
        /// <summary>
        /// The capture time of this frame
        /// </summary>
        public long CaptureTime { get; private set; }

        private struct FlipTextureJob : IJob
        {
            [ReadOnly]
            public NativeArray<byte> InputArray;

            [ReadOnly]
            public uint Height;

            [ReadOnly]
            public uint RowStride;

            [WriteOnly]
            public NativeArray<byte> OutputArray;

            public void Execute()
            {
                var startingRow = (int)((Height - 1) * RowStride);
                var targetIndex = 0;
                while (startingRow >= 0)
                {
                    for (var i = 0; i < RowStride; i++)
                    {
                        OutputArray[targetIndex] = InputArray[startingRow + i];
                        targetIndex++;
                    }

                    startingRow -= (int)RowStride;
                }
            }
        }

        internal unsafe void CreateFromNativeFrame(XrPixelSensorFrame* frame, Allocator allocator, bool shouldFlipTexture, long captureTime)
        {
            IsValid = true;
            internalFrame = *frame;
            Planes = new PixelSensorPlane[frame->PlaneCount];
            CaptureTime = captureTime;
            for (var i = 0; i < frame->PlaneCount; i++)
            {
                ref var inputPlane = ref frame->Planes[i];
                ref var outputPlane = ref Planes[i];
                outputPlane.NativePlane = inputPlane;
                outputPlane.ByteData = new NativeArray<byte>((int)inputPlane.BufferSize, allocator);
                var byteArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(inputPlane.Buffer, (int)inputPlane.BufferSize, Allocator.None);
                if (!shouldFlipTexture || FrameType == PixelSensorFrameType.Jpeg)
                {
                    outputPlane.ByteData.CopyFrom(byteArray);
                    continue;
                }

                new FlipTextureJob
                {
                    InputArray = byteArray,
                    Height = inputPlane.Height,
                    RowStride = inputPlane.Stride,
                    OutputArray = outputPlane.ByteData
                }.Schedule().Complete();
            }
        }
    }
}
