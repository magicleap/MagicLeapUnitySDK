// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2022 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLDepthCamera
    {
        private class NativeBindings : MagicLeapNativeBindings
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct MLDepthCameraSettings
            {
                public uint Version;
                public uint Flags;
                public uint Mode;

                public static MLDepthCameraSettings Init()
                {
                    return new MLDepthCameraSettings()
                    {
                        Version = 1,
                        Flags = (uint)MLDepthCamera.CaptureFlags.DepthImage,
                        Mode = (uint)MLDepthCamera.Mode.LongRange
                    };
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MLDepthCameraIntrinsics
            {
                public const int MaxDistortionCoefficients = 5;
                
                public uint Width;
                
                public uint Height;

                public MLVec2f FocalLength;
                
                public MLVec2f PrincipalPoint;
                
                public float FoV;
                
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxDistortionCoefficients)]
                public double[] Distortion;

                public static MLDepthCameraIntrinsics Init(Intrinsics managed)
                {
                    var intrinsics = new MLDepthCameraIntrinsics
                    {
                        Width = managed.Width,
                        Height = managed.Height,
                        FocalLength = new MLVec2f() { X = managed.FocalLength.x, Y = managed.FocalLength.y },
                        PrincipalPoint = new MLVec2f() { X = managed.PrincipalPoint.x, Y = managed.PrincipalPoint.y },
                        FoV = managed.FoV,
                        Distortion = managed.GetDistortionList().ToArray()
                    };
                    return intrinsics;
                }

                public static Intrinsics ToManaged(MLDepthCameraIntrinsics nativeIntrinsics)
                {
                    return new Intrinsics()
                    {
                        Width = nativeIntrinsics.Width,
                        Height = nativeIntrinsics.Height,
                        FocalLength = MLConvert.ToUnity(nativeIntrinsics.FocalLength),
                        PrincipalPoint = MLConvert.ToUnity(nativeIntrinsics.PrincipalPoint),
                        FoV = nativeIntrinsics.FoV,
                        Distortion = new DistortionCoefficients(nativeIntrinsics.Distortion)
                    };
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MLDepthCameraFrameBuffer
            {
                public uint Width;
                public uint Height;
                public uint Stride;
                public uint BytesPerUnit;
                public uint Size;
                public IntPtr Data;

                public static MLDepthCameraFrameBuffer Init(FrameBuffer? managed)
                {
                    var result = new MLDepthCameraFrameBuffer();
                    if (managed != null)
                    {
                        result.Width = managed.Value.Width;
                        result.Height = managed.Value.Height;
                        result.Stride = managed.Value.Stride;
                        result.BytesPerUnit = managed.Value.BytesPerUnit;
                        result.Size = (managed.Value.Data != null) ? (uint)managed.Value.Data.Length : 0u;
                        if (managed.Value.Data != null)
                        {
                            Marshal.Copy(managed.Value.Data, 0, result.Data, managed.Value.Data.Length);
                        }
                        return result;
                    }
                    else
                    {
                        result.Data = IntPtr.Zero;
                        return result;
                    }
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MLDepthCameraData
            {
                public uint Version;
                
                public long FrameNumber;
                
                public long FrameTimestamp;
                
                public FrameType FrameType;

                public MLTransform CameraPose;

                public MLDepthCameraIntrinsics Intrinsics;
                
                public IntPtr DepthImageFrameBufferPtr;
                
                public IntPtr ConfidenceBufferFrameBufferPtr;
                
                public IntPtr DepthFlagsBufferFrameBufferPtr;
                
                public IntPtr AmbientRawDepthImageFrameBufferPtr;

                public static MLDepthCameraData Init()
                {
                    return new MLDepthCameraData()
                    {
                        Version = 1
                    };
                }

                public static MLDepthCameraData Init(Data managed)
                {
                    var data = Init();
                    data.FrameNumber = managed.FrameNumber;
                    data.FrameTimestamp = managed.FrameTimestamp;
                    data.FrameType = managed.FrameType;
                    data.CameraPose = new MLTransform()
                    {
                        Position = MLConvert.FromUnity(managed.Position),
                        Rotation = MLConvert.FromUnity(managed.Rotation)
                    };
                    data.Intrinsics = MLDepthCameraIntrinsics.Init(managed.Intrinsics);

                    var depthMapPlaneInfo = MLDepthCameraFrameBuffer.Init(managed.DepthImage);
                    Marshal.StructureToPtr(depthMapPlaneInfo, data.DepthImageFrameBufferPtr, true);

                    var confidenceMapPlaneInfo = MLDepthCameraFrameBuffer.Init(managed.ConfidenceBuffer);
                    Marshal.StructureToPtr(confidenceMapPlaneInfo, data.ConfidenceBufferFrameBufferPtr, true);

                    var depthFlagsPlaneInfo = MLDepthCameraFrameBuffer.Init(managed.DepthFlagsBuffer);
                    Marshal.StructureToPtr(depthFlagsPlaneInfo, data.DepthFlagsBufferFrameBufferPtr, true);

                    var aiFlagsPlaneInfo = MLDepthCameraFrameBuffer.Init(managed.AmbientRawDepthImage);
                    Marshal.StructureToPtr(aiFlagsPlaneInfo, data.AmbientRawDepthImageFrameBufferPtr, true);

                    return data;
                }
            }

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLDepthCameraConnect(in MLDepthCameraSettings settings, out ulong handle);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLDepthCameraUpdateSettings(ulong handle, in MLDepthCameraSettings settings);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLDepthCameraGetLatestDepthData(ulong handle, ulong timeoutMs, out IntPtr data);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLDepthCameraReleaseDepthData(ulong handle, IntPtr data);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLDepthCameraDisconnect(ulong handle);
        }
    }
}
