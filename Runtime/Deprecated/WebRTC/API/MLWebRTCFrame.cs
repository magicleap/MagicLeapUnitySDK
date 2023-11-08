// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.MagicLeap
{
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
                /// Defines the supported output formats of the image planes.
                /// </summary>
                public enum OutputFormat
                {
                    /// <summary>
                    /// YUV_420_888 format.
                    /// </summary>
                    YUV_420_888,

                    /// <summary>
                    /// RGBA_8888 format.
                    /// </summary>
                    RGBA_8888,

                    /// <summary>
                    /// Native buffer format
                    /// </summary>
                    NativeBuffer
                }

                /// <summary>
                /// Defines amount of valid image planes to be found within the frame's image planes array output formats of the image planes.
                /// </summary>
                public static readonly Dictionary<OutputFormat, byte> NativeImagePlanesLength = new Dictionary<OutputFormat, byte>()
                {
                    { OutputFormat.YUV_420_888, 3 },
                    { OutputFormat.RGBA_8888, 1 },
                    { OutputFormat.NativeBuffer, 0 }
                };

                /// <summary>
                /// Gets the id of the frame.
                /// </summary>
                public ulong Id { get; private set; }

                /// <summary>
                /// Gets the timestamp of the frame in microseconds.
                /// </summary>
                public ulong TimeStampUs { get; private set; }

                /// <summary>
                /// Gets the format of the image planes in this frame.
                /// </summary>
                public MLWebRTC.VideoSink.Frame.OutputFormat Format { get; private set; }

                /// <summary>
                /// Gets the array of image planes contained in this frame.
                /// </summary>
                public MLWebRTC.VideoSink.Frame.PlaneInfo[] ImagePlanes { get; private set; }

                public NativeBufferInfo NativeFrame { get; private set; }

                /// <summary>
                /// Creates and returns an initialized version of this struct.
                /// </summary>
                /// <param name="id">Id of the frame.</param>
                /// <param name="timeStampUs">Timestamp of the frame in microseconds.</param>
                /// <param name="imagePlanes">Array of image planes this frame contains.</param>
                /// <param name="format">The output format of this frame.</param>
                /// <returns>An initialized version of this struct.</returns>
                public static Frame Create(ulong id, ulong timeStampUs, PlaneInfo[] imagePlanes, OutputFormat format)
                {
                    Frame frame = new Frame()
                    {
                        Id = id,
                        TimeStampUs = timeStampUs,
                        ImagePlanes = imagePlanes,
                        Format = format
                    };

                    return frame;
                }

                public static Frame Create(ulong id, ulong timeStampUs, NativeBufferInfo nativeBufferInfo)
                {
                    Frame frame = new Frame()
                    {
                        Id = id,
                        TimeStampUs = timeStampUs,
                        NativeFrame = nativeBufferInfo,
                        Format = OutputFormat.NativeBuffer
                    };

                    return frame;
                }

                internal static Frame Create(ulong id, NativeBindings.MLWebRTCFrame nativeFrame, PlaneInfo[] imagePlanes = null)
                {
                    Frame frame = new Frame()
                    {
                        Id = id,
                        TimeStampUs = nativeFrame.TimeStamp,
                        Format = nativeFrame.Format
                    };

                    if (frame.Format == OutputFormat.NativeBuffer)
                    {
                        var frameData = Marshal.PtrToStructure<NativeBindings.MLWebRTCNativeFrameInfo>(nativeFrame.FrameData);
                        frame.NativeFrame = NativeBufferInfo.Create(frameData);
                    }
                    else
                    {
                        var frameData = Marshal.PtrToStructure<NativeBindings.MLWebRTCFramePlanes>(nativeFrame.FrameData);
                        frame.ImagePlanes = (imagePlanes == null) ? new PlaneInfo[frameData.PlaneCount] : imagePlanes;
                        for (ushort i = 0; i < frameData.PlaneCount; ++i)
                        {
                            var src = frameData.ImagePlanes[i];
                            frame.ImagePlanes[i] = PlaneInfo.Create(src.Width, src.Height, src.Stride, src.BytesPerPixel, src.Size, src.ImageDataPtr);
                        }
                    }

                    return frame;
                }

                /// <summary>
                /// Override to display the contents of a frame as a string.
                /// </summary>
                /// <returns>A string representation of this struct.</returns>
                public override string ToString() => $"\nId: {this.Id}, \nTimeStamp (us): {this.TimeStampUs}, \nFormat: {this.Format}";
            }
        }
    }
}
