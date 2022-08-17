// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCFrameNativeBindings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
#if UNITY_MAGICLEAP || UNITY_ANDROID
    using UnityEngine.XR.MagicLeap.Native;
#endif

    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Class that represents a source used by the MLWebRTC API.
        /// </summary>
        public partial class VideoSink
        {
            /// <summary>
            /// Struct representing a captured camera frame.
            /// </summary>
            public partial struct Frame
            {
                /// <summary>
                /// Native bindings for the MLWebRTC.Frame struct. 
                /// </summary>
                internal class NativeBindings
#if UNITY_MAGICLEAP || UNITY_ANDROID
                    : MagicLeapNativeBindings
#endif
                {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                    /// <summary>
                    /// Gets frame data.
                    /// </summary>
                    /// <param name="frameHandle">The handle to the frame to query.</param>
                    /// <param name="frame">Pointer to the frame data.</param>
                    /// <returns>
                    /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the dimensions were successfully obtained.
                    /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                    /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                    /// </returns>
                    [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                    public static extern MLResult.Code MLWebRTCFrameGetData(ulong frameHandle, ref MLWebRTCFrame frame);
#endif

                    /// <summary>
                    /// Buffer for native image plane arrays.
                    /// </summary>
                    private static readonly CircularBuffer<ImagePlaneInfoNative[]> nativeImagePlanesBuffer = CircularBuffer<ImagePlaneInfoNative[]>.Create(
                        new ImagePlaneInfoNative[PlaneInfo.MaxImagePlanes], 
                        new ImagePlaneInfoNative[PlaneInfo.MaxImagePlanes], 
                        new ImagePlaneInfoNative[PlaneInfo.MaxImagePlanes]);

                    /// <summary>
                    /// 4x4 matrix size for native frame transforms
                    /// </summary>
                    private const int TransformMatrixLength = 16;

                    /// <summary>
                    /// Representation of the native frame structure.
                    /// </summary>
                    [StructLayout(LayoutKind.Sequential)]
                    public struct MLWebRTCFrame
                    {
                        /// <summary>
                        /// Version of this structure.
                        /// </summary>
                        public uint Version;

                        /// <summary>
                        /// Frame data, can be CPU based image planes or
                        /// gpu based native surface. Check MLWebRTCFrame.Format
                        /// to confirm how this memory should be read.
                        /// </summary>
                        public FrameUnion FrameData;
                        
                        /// <summary>
                        /// Timestamp of the frame.
                        /// </summary>
                        public ulong TimeStamp;

                        /// <summary>
                        /// Output format that the image planes will be in.
                        /// </summary>
                        public OutputFormat Format;

                        public static MLWebRTCFrame Create(OutputFormat format)
                        {
                            MLWebRTCFrame frameNative = new MLWebRTCFrame();
                            frameNative.Version = 1;
                            frameNative.Format = format;
                            frameNative.FrameData.PlaneCount = NativeImagePlanesLength[format];
                            return frameNative;
                        }

                        /// <summary>
                        /// Creates and returns an initialized version of this struct from a MLWebRTC.VideoSink.Frame object.
                        /// </summary>
                        /// <param name="frame">The frame object to use for initializing.</param>
                        /// <returns>An initialized version of this struct.</returns>
                        public static MLWebRTCFrame Create(MLWebRTC.VideoSink.Frame frame)
                        {
                            MLWebRTCFrame frameNative = Create(frame.Format);
                            frameNative.TimeStamp = frame.TimeStampUs;
                            if (frame.Format == OutputFormat.NativeBuffer)
                            {
                                frameNative.FrameData.NativeFrameInfo.Width = frame.NativeFrame.Width;
                                frameNative.FrameData.NativeFrameInfo.Height = frame.NativeFrame.Height;
                                frameNative.FrameData.NativeFrameInfo.SurfaceHandle = frame.NativeFrame.SurfaceHandle;
                                frameNative.FrameData.NativeFrameInfo.NativeBufferHandle = frame.NativeFrame.NativeBufferHandle;
                                unsafe 
                                {
                                    for(int i = 0; i < TransformMatrixLength; i++)
                                    {
                                        frameNative.FrameData.NativeFrameInfo.Transform[i] = frame.NativeFrame.Transform[i];
                                    }
                                }
                            }
                            else
                            {
                                var planes = nativeImagePlanesBuffer.Get();
                                for (int i = 0; i < frameNative.FrameData.PlaneCount; ++i)
                                {
                                    frameNative.FrameData.SetPlaneAtIndex(i, new ImagePlaneInfoNative(frame.ImagePlanes[i]));
                                }
                            }

                            return frameNative;
                        }

                        public override string ToString()
                        {
                            var str = new System.Text.StringBuilder($"[MLWebRTCFrame: Version={Version}, TimeStamp={TimeStamp}, Format={Format}\n");
                            str.AppendLine($"\tFrameData:\n\t\t{FrameData}\n]");
                            return str.ToString();
                        }
                    }

                    [StructLayout(LayoutKind.Explicit)]
                    public struct FrameUnion
                    {
                        /// <summary>
                        /// Number of valid planes in the ImageFrame. 
                        /// 1 for RGB, 2 for YUV
                        /// </summary>
                        [FieldOffset(0)]
                        public byte PlaneCount;

                        /// <summary>
                        /// The RGB plane or Y plane in a YUV buffer
                        /// </summary>
                        [FieldOffset(8)]
                        public ImagePlaneInfoNative ImagePlanes_0;

                        /// <summary>
                        /// The U plane in a YUV buffer
                        /// </summary>
                        [FieldOffset(40)]
                        public ImagePlaneInfoNative ImagePlanes_1;

                        /// <summary>
                        /// The V plane in a YUV buffer
                        /// </summary>
                        [FieldOffset(72)]
                        public ImagePlaneInfoNative ImagePlanes_2;

                        /// <summary>
                        /// Structure representing a native surface HW buffer. 
                        /// </summary>
                        [FieldOffset(0)]
                        public MLWebRTCNativeFrameInfo NativeFrameInfo;

                        public ImagePlaneInfoNative GetPlaneAtIndex(int index)
                        {
                            return index switch
                            {
                                0 => ImagePlanes_0,
                                1 => ImagePlanes_1,
                                2 => ImagePlanes_2,
                                _ => new ImagePlaneInfoNative(),
                            };
                        }

                        public void SetPlaneAtIndex(int index, ImagePlaneInfoNative plane)
                        {
                            if (index == 0) ImagePlanes_0 = plane;
                            if (index == 1) ImagePlanes_1 = plane;
                            if (index == 2) ImagePlanes_2 = plane;
                        }

                        public ImagePlaneInfoNative[] GetPlanesArray() => new ImagePlaneInfoNative[] { ImagePlanes_0, ImagePlanes_1, ImagePlanes_2 };

                        public override string ToString()
                        {
                            var str = new System.Text.StringBuilder($"[FrameUnion: PlaneCount={PlaneCount}\n");
                            str.AppendLine($"\t\t{string.Join("\n\t\t", GetPlanesArray())}");
                            str.AppendLine($"\t\tNativeFrameInfo:{NativeFrameInfo}\n]");
                            return str.ToString();
                        }
                    }

                    [StructLayout(LayoutKind.Sequential, Pack = 4)]
                    public struct MLWebRTCNativeFrameInfo 
                    {
                        /// <summary>
                        /// Width of the native frame
                        /// </summary>
                        public uint Width;
                        
                        /// <summary>
                        /// Height of the native frame
                        /// </summary>
                        public uint Height;

                        /// <summary>
                        /// The 4x4 column-major tranformation matrix for the native frame
                        /// </summary>
                        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TransformMatrixLength)]
                        public unsafe fixed float Transform[TransformMatrixLength];

                        /// <summary>
                        /// Surface handle, from which native handle is acquired
                        /// Surface handle can be created using MLNativeSurfaceCreate
                        /// </summary>
                        public ulong SurfaceHandle;

                        /// <summary>
                        /// Native buffer handle can be acquired from MLNativeSurface using MLNativeSurfaceAcquireNextAvailableFrame.
                        /// Application should never release the native_buffer_handle once it is acquired.
                        /// If MLWebRTCFrame is created by application to send the frame through webrtc, webrtc will release
                        /// the native_buffer_handle using MLNativeSurfaceReleaseFrame after encoding.
                        /// Application should not release the native_buffer_handle send by webrtc for rendering whether it is local or remote sink.
                        /// </summary>
                        public ulong NativeBufferHandle;

                        public unsafe float[] GetTransformArray()
                        {
                            float[] buffer = new float[TransformMatrixLength];
                            fixed(float* ptr = Transform)
                            {
                                for(int i = 0; i < TransformMatrixLength; i++)
                                {
                                    buffer[i] = *(ptr + i);
                                }
                            }
                            return buffer;
                        }

                        public override string ToString()
                        {
                            return $"[MLWebRTCNativeFrameInfo: Width={Width}, Height={Height},\n\t\t\tTransform=({string.Join(',', GetTransformArray())}),\n\t\t\tSurfaceHandle={SurfaceHandle}, NativeBufferHandle={NativeBufferHandle}]";
                        }
                    }

                    /// <summary>
                    /// Representation of the native image plane structure.
                    /// </summary>
                    [StructLayout(LayoutKind.Sequential)]
                    public struct ImagePlaneInfoNative
                    {
                        /// <summary>
                        /// Width of the image plane.
                        /// </summary>
                        public uint Width;

                        /// <summary>
                        /// Height of the image plane.
                        /// </summary>
                        public uint Height;

                        /// <summary>
                        /// The stride of the image plane, representing how many bytes one row of the image plane contains.
                        /// </summary>
                        public uint Stride;

                        /// <summary>
                        /// The bytes per pixel of the image plane.
                        /// </summary>
                        public uint BytesPerPixel;

                        /// <summary>
                        /// Data of the image plane.
                        /// </summary>
                        public IntPtr ImageDataPtr;

                        /// <summary>
                        /// Size of the image plane.
                        /// </summary>
                        public uint Size;

                        /// <summary>
                        /// Sets data from an MLWebRTC.VideoSink.Frame.ImagePlane object.
                        /// </summary>
                        public ImagePlaneInfoNative (PlaneInfo planeInfo)
                        {
                            Width = planeInfo.Width;
                            Height = planeInfo.Height;
                            Stride = planeInfo.Stride;
                            BytesPerPixel = planeInfo.BytesPerPixel;
                            ImageDataPtr = planeInfo.DataPtr;
                            Size = planeInfo.Size;
                        }

                        public override string ToString()
                        {
                            return $"[Width={Width}, Height={Height}, Stride={Stride}, BytesPerPixel={BytesPerPixel}, DataPtr={ImageDataPtr}, Size={Size}]";
                        }
                    }
                }
            }
        }
    }
}
