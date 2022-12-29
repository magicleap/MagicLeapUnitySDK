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
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap.Native;

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
                internal class NativeBindings : MagicLeapNativeBindings
                {
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
                        /// Frame data to be interpreted as a <see cref="MLWebRTCNativeFrameInfo"/> if Format is <see cref="OutputFormat.NativeBuffer"/>,
                        /// otherwise interpreted as a <see cref="MLWebRTCFramePlanes"/>.
                        /// </summary>
                        public IntPtr FrameData;

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
                            frameNative.Version = 2;
                            frameNative.Format = format;
                            return frameNative;
                        }

                        /// <summary>
                        /// Caller MUST call <c>FreeUnmanagedMemory</c> when finished with the returned <c>MLWebRTCFrame</c>! Unmanaged memory 
                        /// will be allocated for the <c>FrameData</c> pointer.<br/><br/>
                        /// Creates and returns an initialized version of this struct from a <see cref="MLWebRTC.VideoSink.Frame"/> object.
                        /// </summary>
                        /// <param name="frame">The frame object to use for initializing.</param>
                        /// <returns>An initialized version of this struct.</returns>
                        public static MLWebRTCFrame Create(MLWebRTC.VideoSink.Frame frame)
                        {
                            MLWebRTCFrame frameNative = Create(frame.Format);
                            frameNative.TimeStamp = frame.TimeStampUs;
                            if (frame.Format == OutputFormat.NativeBuffer)
                            {
                                MLWebRTCNativeFrameInfo frameData = new MLWebRTCNativeFrameInfo()
                                {
                                    Width = frame.NativeFrame.Width,
                                    Height = frame.NativeFrame.Height,
                                    SurfaceHandle = frame.NativeFrame.SurfaceHandle,
                                    NativeBufferHandle = frame.NativeFrame.NativeBufferHandle,
                                    Transform = new float[frame.NativeFrame.Transform.Length]
                                };
                                Array.Copy(frame.NativeFrame.Transform, frameData.Transform, frame.NativeFrame.Transform.Length);

                                frameNative.FrameData = Marshal.AllocHGlobal(Marshal.SizeOf(frameData));
                                Marshal.StructureToPtr(frameData, frameNative.FrameData, false);
                            }
                            else
                            {
                                MLWebRTCFramePlanes frameData = new MLWebRTCFramePlanes()
                                {
                                    PlaneCount = (byte)((frame.Format == OutputFormat.YUV_420_888) ? 3 : 1),
                                    ImagePlanes = nativeImagePlanesBuffer.Get()
                                };
                                for (int i = 0; i < frame.ImagePlanes.Length; i++)
                                {
                                    frameData.ImagePlanes[i] = new ImagePlaneInfoNative(frame.ImagePlanes[i]);
                                }

                                frameNative.FrameData = Marshal.AllocHGlobal(Marshal.SizeOf(frameData));
                                Marshal.StructureToPtr(frameData, frameNative.FrameData, false);
                            }

                            return frameNative;
                        }

                        public void FreeUnmanagedMemory()
                        {
                            if (FrameData != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(FrameData);
                                FrameData = IntPtr.Zero;
                            }
                        }

                        public override string ToString()
                        {
                            var str = new System.Text.StringBuilder($"[MLWebRTCFrame: Version={Version}, TimeStamp={TimeStamp}, Format={Format}\n");
                            str.AppendLine($"\tFrameData:\n\t\t{FrameData}\n]");
                            return str.ToString();
                        }
                    }

                    [StructLayout(LayoutKind.Sequential)]
                    public struct MLWebRTCFramePlanes
                    {
                        /// <summary>
                        /// Number of output image planes. 3 for <see cref="OutputFormat.YUV_420_888"/>, 1 for <see cref="OutputFormat.RGBA_8888"/>
                        /// </summary>
                        public byte PlaneCount;

                        /// <summary>
                        /// The image planes making up the output image. Array length is constant, actual number of planes is specified by PlaneCount.
                        /// </summary>
                        [MarshalAs(UnmanagedType.ByValArray, SizeConst = PlaneInfo.MaxImagePlanes)]
                        public ImagePlaneInfoNative[] ImagePlanes;
                    }

                    [StructLayout(LayoutKind.Sequential)]
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
                        public float[] Transform;

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

                        public override string ToString()
                        {
                            return $"[MLWebRTCNativeFrameInfo: Width={Width}, Height={Height},\n\t\t\tTransform=({string.Join(',', Transform)}),\n\t\t\tSurfaceHandle={SurfaceHandle}, NativeBufferHandle={NativeBufferHandle}]";
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
                        public ImagePlaneInfoNative(PlaneInfo planeInfo)
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
