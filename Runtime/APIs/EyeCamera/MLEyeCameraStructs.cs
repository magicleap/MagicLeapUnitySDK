// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2022-2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.MagicLeap
{
    public sealed partial class MLEyeCamera
    {
        public readonly struct EyeCameraData
        {
            /// <summary>
            /// Number of camera frames.
            /// </summary>
            public readonly byte FrameCount;

            /// <summary>
            /// Camera frame data. The number of frames is specified by FrameCount.
            /// </summary>
            public readonly EyeCameraFrame[] Frames;

            internal EyeCameraData(NativeBindings.MLEyeCameraData eyeCameraData)
            {
                FrameCount = eyeCameraData.FrameCount;

                // marshal unmanaged array to struct array
                var size = Marshal.SizeOf(typeof(EyeCameraFrame));
                Frames = new EyeCameraFrame[FrameCount];

                for (int i = 0; i < FrameCount; i++)
                {
                    IntPtr ins = new IntPtr(eyeCameraData.Frames.ToInt64() + i * size);
                    Frames[i] = Marshal.PtrToStructure<EyeCameraFrame>(ins);
                }
            }
        }

        public readonly struct EyeCameraFrameBuffer
        {
            /// <summary>
            /// Width of the output image in pixels.
            /// </summary> 
            public readonly uint Width;

            /// <summary>
            /// Height of the output image in pixels.
            /// </summary> 
            public readonly uint Height;

            /// <summary>
            /// Stride of the output image in bytes.
            /// </summary> 
            public readonly uint Stride;

            /// <summary>
            /// Number of bytes used to represent a pixel.
            /// </summary> 
            public readonly uint BytesPerPixel;

            /// <summary>
            /// Distance between 2 consecutive pixels in bytes. 
            /// </summary> 
            public readonly uint PixelStride;

            /// <summary>
            /// Number of bytes in the image output data. 
            /// </summary> 
            public readonly uint Size;

            /// <summary>
            /// Image data.
            /// </summary> 
            public readonly IntPtr Data;

            internal EyeCameraFrameBuffer(NativeBindings.MLEyeCameraFrameBuffer eyeCameraFrameBuffer)
            {
                Width = eyeCameraFrameBuffer.Width;
                Height = eyeCameraFrameBuffer.Height;
                Stride = eyeCameraFrameBuffer.Stride;
                BytesPerPixel = eyeCameraFrameBuffer.BytesPerPixel;
                PixelStride = eyeCameraFrameBuffer.PixelStride;
                Size = eyeCameraFrameBuffer.Size;
                Data = eyeCameraFrameBuffer.Data;
            }
        }

        public readonly struct EyeCameraFrame
        {
            /// <summary>
            /// Camera Identifier specifies which camera is associated with this frame.
            /// </summary>
            public readonly MLEyeCameraIdentifier CameraID;

            /// <summary>
            /// A 64bit integer to index the frame number associated with this frame.
            /// </summary>
            public readonly long FrameNumber;

            /// <summary>
            /// Frame timestamp specifies the time at which the frame was captured.
            /// </summary>
            public readonly long TimeStamp;

            /// <summary>
            /// Frame buffer data.
            /// </summary>
            public readonly EyeCameraFrameBuffer FrameBuffer;

            internal EyeCameraFrame(NativeBindings.MLEyeCameraFrame eyeCameraFrame)
            {
                CameraID = eyeCameraFrame.CameraID;
                FrameNumber = eyeCameraFrame.FrameNumber;
                TimeStamp = eyeCameraFrame.TimeStamp;
                FrameBuffer = new EyeCameraFrameBuffer(eyeCameraFrame.FrameBuffer);
            }
        }
    }
}
