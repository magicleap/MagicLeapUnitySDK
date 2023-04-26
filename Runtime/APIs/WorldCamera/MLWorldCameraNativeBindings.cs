// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWorldCameraNativeBindings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%


namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;

    public partial class MLWorldCamera
    {
        /// <summary>
        /// See ml_world_camera.h for additional comments.
        /// </summary>
        internal class NativeBindings : Native.MagicLeapNativeBindings
        {
            private const int MaxRadialDistortionCoefficients = 4;
            private const int MaxTangentialDistortionCoefficients = 2;

            /// <summary>
            /// World camera intrinsic parameters.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLWorldCameraIntrinsics
            {
                /// <summary>
                /// Camera width.
                /// </summary>
                public readonly uint Width;

                /// <summary>
                /// Camera height.
                /// </summary>
                public readonly uint Height;

                /// <summary>
                /// Camera focal length.
                /// </summary>
                public readonly Vector2 FocalLength;

                /// <summary>
                /// Camera principal point.
                /// </summary>
                public readonly Vector2 PrincipalPoint;

                /// <summary>
                /// Field of view in degrees.
                /// </summary>
                public readonly float Fov;

                /// <summary>
                /// Radial distortion vector.
                /// The radial distortion co-efficients are in the following order: [k1, k2, k3, k4].
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxRadialDistortionCoefficients)]
                public readonly double[] RadialDistortion;

                /// <summary>
                /// Tangential distortion vector.
                /// The tangential distortion co-efficients are in the following order: [p1, p2].
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxTangentialDistortionCoefficients)]
                public readonly double[] TangentialDistortion;

                public override string ToString() => $"Width:{Width}, Height:{Height}, FOV: {Fov}";
            }


            /// <summary>
            /// A structure to encapsulate the camera settings.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLWorldCameraSettings
            {
                public MLWorldCameraSettings(in Settings settings)
                {
                    Version = 1;
                    Mode = (uint)settings.Mode;
                    Cameras = (uint)settings.Cameras;
                }

                /// <summary>
                /// Version of this structure.
                /// </summary>
                public readonly uint Version;

                /// <summary>
                /// World camera mode. If you want to request frames from different camera modes
                /// then "OR" the modes of interest to the app.
                /// The system may not be able to service all the requested camera modes.
                /// This parameter is treated as a hint and data will be provided for
                ///	the requested camera modes when available.
                /// </summary>
                public readonly uint Mode;

                /// <summary>
                /// World cameras that need to be enabled.If you want to request frames from different world camera
                /// then "OR" the modes of interest to the app.
                /// The system may not be able to service all the requested cameras.
                ///	This parameter is treated as a hint and data will be provided from
                ///	the requested world cameras when available.
                /// </summary>
                public readonly uint Cameras;

            };

            /// <summary>
            /// A structure to encapsulate per plane info for each camera frame.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLWorldCameraFrameBuffer
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
                /// Number of bytes in the frame.
                /// </summary>
                public readonly uint Size;

                /// <summary>
                /// Buffer data.
                /// </summary>
                public readonly IntPtr Data;

                public override string ToString() => $"Width: {Width}, Height: {Height}, Stride: {Stride}, BytesPerPixel: {BytesPerPixel}, Size: {Size}";
            }

            /// <summary>
            /// A structure to encapsulate output data for each camera sensor.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLWorldCameraFrame
            {
                /// <summary>
                /// Camera Identifier specifies which camera is associated with this frame.
                /// </summary>
                public readonly MLWorldCamera.CameraId Id;

                /// <summary>
                /// A 64bit integer to index the frame number associated with this frame.
                /// </summary>
                public readonly long FrameNumber;

                /// <summary>
                /// Frame timestamp specifies the time at which the frame was captured.
                /// </summary>
                public readonly long TimeStamp;

                /// <summary>
                /// Camera intrinsic parameters.
                /// </summary>
                public readonly NativeBindings.MLWorldCameraIntrinsics Intrinsics;

                /// <summary>
                /// World camera pose in the world co-ordinate system.
                /// </summary>
                public readonly MLTransform CameraPose;

                /// <summary>
                /// Frame buffer data.
                /// </summary>
                public readonly MLWorldCameraFrameBuffer FrameBuffer;

                /// <summary>
                /// Maps to the world camera mode used for capturing the camera frames.
                /// </summary>
                public readonly Frame.Type FrameType;

                public override string ToString() => $"Id: {Id}, FrameNumber:{FrameNumber}, Intrinsics: {Intrinsics}, FrameBuffer: {FrameBuffer}, FrameType: {FrameType}";
            }


            /// <summary>
            /// A structure to encapsulate output data for each camera sensor.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLWorldCameraData
            {
                /// <summary>
                /// Version of this structure.
                /// </summary>
                public readonly uint Version;

                /// <summary>
                /// Number of camera frames populated.
                /// </summary>
                public readonly byte FrameCount;

                /// <summary>
                /// Camera frame data.
                /// </summary>
                public readonly IntPtr Frames;

                public MLWorldCameraData(uint version)
                {
                    Version = version;
                    FrameCount = 0;
                    Frames = IntPtr.Zero;
                }
            };

            /// <summary>
            /// Connect to world cameras.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWorldCameraConnect(in MLWorldCameraSettings settings, out ulong handle);

            /// <summary>
            /// Update the world camera settings.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWorldCameraUpdateSettings(ulong handle, in MLWorldCameraSettings settings);

            /// <summary>
            /// Poll for Frames.  Returns #MLWorldCameraData with this latest data when available.  The memory is owned by the system.
            /// Application should copy the data it needs to cache it and then release the memory by calling
            /// #MLWorldCameraReleaseCameraData.  This is a blocking call.  API is not thread safe.  If there are no new world camera data frames for a given
            /// duration (duration determined by the system) then the API will return MLResult_Timeout.
            /// </summary>

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWorldCameraGetLatestWorldCameraData(ulong handle, uint timeOutMs, ref IntPtr cameraData);

            /// <summary>
            /// Releases specified #MLWorldCameraData object.  This function should be called exactly once for each call to
            /// #MLWorldCameraGetLatestCameraData.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWorldCameraReleaseCameraData(ulong handle, IntPtr cameraData);

            /// <summary>
            /// Disconnect from world camera. This will disconnect from all the world camera currently connected.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWorldCameraDisconnect(ulong handle);

        }
    }
}

