// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;

    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Class that represents a video sink used by the MLWebRTC API.
        /// Video sinks are fed data by media sources and produces frames to render.
        /// </summary>
        public partial class VideoSink
        {
            /// <summary>
            /// Struct representing a captured camera frame.
            /// </summary>
            public partial struct Frame
            {
                /// <summary>
                /// Struct representing an image plane that comes from some captured camera frame.
                /// </summary>
                public struct PlaneInfo
                {
                    /// <summary>
                    /// Max amount of image planes that can be in a single frame.
                    /// </summary>
                    public const int MaxImagePlanes = 3;

                    /// <summary>
                    /// Gets the width of the image plane.
                    /// </summary>
                    public uint Width { get; private set; }

                    /// <summary>
                    /// Gets the height of the image plane.
                    /// </summary>
                    public uint Height { get; private set; }

                    /// <summary>
                    /// Gets the stride of the image plane, representing how many bytes one row of the image plane contains.
                    /// </summary>
                    public uint Stride { get; private set; }

                    /// <summary>
                    /// Gets the bytes per pixel of the image plane.
                    /// </summary>
                    public uint BytesPerPixel { get; private set; }

                    /// <summary>
                    /// Gets the size of the image plane, representing how many bytes in total the entire image plane contains.
                    /// </summary>
                    public uint Size { get; private set; }

                    /// <summary>
                    /// Gets the pointer to the unmanaged memory where the actual image data is found.
                    /// </summary>
                    public IntPtr DataPtr { get; private set; }

                    /// <summary>
                    /// Creates and returns an initialized version of this struct.
                    /// </summary>
                    /// <param name="width">Width of the image plane.</param>
                    /// <param name="height">Height of the image plane.</param>
                    /// <param name="stride">Stride of the image plane.</param>
                    /// <param name="bytesPerPixel">Bytes per pixel of the image plane.</param>
                    /// <param name="size">Size of the image plane.</param>
                    /// <param name="dataPtr">Pointer to the image data for the image plane.</param>
                    /// <returns>An initialized version of this struct.</returns>
                    public static PlaneInfo Create(uint width, uint height, uint stride, uint bytesPerPixel, uint size, IntPtr dataPtr)
                    {
                        PlaneInfo imagePlane = new PlaneInfo()
                        {
                            Width = width,
                            Height = height,
                            Stride = stride,
                            BytesPerPixel = bytesPerPixel,
                            Size = size,
                            DataPtr = dataPtr
                        };

                        return imagePlane;
                    }

                    /// <summary>
                    /// Override to display the contents of a image plane as a string.
                    /// </summary>
                    /// <returns>A string representation of this struct.</returns>
                    public override string ToString() => $"\nWidth: {this.Width} pixels, \nHeight: {this.Height} pixels, \nBytes Per Pixel:{this.BytesPerPixel} bytes, \nStride: {this.Stride} bytes per row, \nSize: {this.Size} total bytes";
                }

                public struct NativeBufferInfo
                {
                    public uint Width { get; set; }

                    public uint Height { get; set; }

                    public float[] Transform { get; set; }

                    public ulong SurfaceHandle { get; set; }

                    public ulong NativeBufferHandle { get; set; }

                    internal static NativeBufferInfo Create(NativeBindings.MLWebRTCNativeFrameInfo nativeFrameInfo)
                    {
                        NativeBufferInfo nativeBufferInfo = new NativeBufferInfo()
                        {
                            Width = nativeFrameInfo.Width,
                            Height = nativeFrameInfo.Height,
                            Transform = nativeFrameInfo.Transform,
                            SurfaceHandle = nativeFrameInfo.SurfaceHandle,
                            NativeBufferHandle = nativeFrameInfo.NativeBufferHandle
                        };

                        return nativeBufferInfo;
                    }

                    public override string ToString() => $"\nWidth: {this.Width} pixels, \nHeight: {this.Height} pixels, \nSurfaceHandle:{this.SurfaceHandle}, \nNativeBufferHandle: {this.NativeBufferHandle}";
                }
            }
        }
    }
}
