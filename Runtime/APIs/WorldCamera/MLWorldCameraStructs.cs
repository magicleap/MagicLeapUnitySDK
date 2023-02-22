// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWorldCameraStructs.cs" company="Magic Leap, Inc">
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
    using System.Text;
    using Native;
    public partial class MLWorldCamera
    {
        /// <summary>
        /// A structure to encapsulate the camera settings.
        /// </summary>
        [Serializable]
        public struct Settings
        {
            public Settings(Mode mode, CameraId cameras)
            {
                this.mode = mode;
                this.cameras = cameras;
            }

            public Mode Mode => mode;

            public CameraId Cameras => cameras;

            [SerializeField]
            private Mode mode;

            [SerializeField]
            private CameraId cameras;

            public override string ToString() => $"Mode: {mode}, Cameras: {cameras}";
        }

        /// <summary>
        /// A structure to encapsulate per plane info for each camera frame.
        /// </summary>
        public readonly struct Frame
        {
            /// <summary>
            /// Enumeration of camera mode used when capturing a frame.
            /// </summary>
            public enum Type
            {
                /// <summary>
                ///  None.
                /// </summary>
                Unkown,

                /// <summary>
                /// Frame captured using #MLWorldCameraMode_LowExposure mode.
                /// </summary>
                LowExposure,

                /// <summary>
                /// Frame captured using #MLWorldCameraMode_NormalEposure mode.
                /// </summary>
                NormalExposure,
            };

            internal Frame(NativeBindings.MLWorldCameraFrame nativeFrame)
            {
                CameraId = nativeFrame.Id;
                CameraPose = new Pose(MLConvert.ToUnity(nativeFrame.CameraPose.Position), MLConvert.ToUnity(nativeFrame.CameraPose.Rotation));
                CameraIntrinsics = new CameraIntrinsics(nativeFrame.Intrinsics);
                FrameNumber = nativeFrame.FrameNumber;
                FrameType = nativeFrame.FrameType;
                FrameBuffer = new Buffer(nativeFrame.FrameBuffer);
                MLTime.ConvertSystemTimeToMLTime(nativeFrame.TimeStamp, out TimeStamp);
            }

            /// <summary>
            /// Camera Identifier specifies which camera is associated with this frame.
            /// </summary>
            public readonly CameraId CameraId;

            /// <summary>
            /// World camera pose in the world co-ordinate system.
            /// </summary>
            public readonly Pose CameraPose;

            /// <summary>
            /// Camera intrinsics.
            /// </summary>
            public readonly CameraIntrinsics CameraIntrinsics;

            /// <summary>
            /// A 64bit integer to index the frame number associated with this frame.
            /// </summary>
            public readonly long FrameNumber;

            /// <summary>
            /// Maps to the world camera mode used for capturing the camera frames.
            /// </summary>
            public readonly Type FrameType;

            /// <summary>
            /// Frame buffer data.
            /// </summary>
            public readonly Buffer FrameBuffer;

            /// <summary>
            /// Frame timestamp specifies the time at which the frame was captured.
            /// </summary>
            public readonly MLTime TimeStamp;

            public override string ToString() => $"Id: {CameraId}, Pose: {CameraPose}, Intrinsics: {CameraIntrinsics}, FrameBufferInfo: {FrameBuffer}, TimeStamp: {TimeStamp}";

            /// <summary>
            /// A structure to encapsulate per plane info for each camera frame.
            /// </summary>
            public readonly struct Buffer
            {
                internal Buffer(NativeBindings.MLWorldCameraFrameBuffer nativeFrameBuffer)
                {
                    Width = nativeFrameBuffer.Width;
                    Height = nativeFrameBuffer.Height;
                    Stride = nativeFrameBuffer.Stride;
                    BytesPerPixel = nativeFrameBuffer.BytesPerPixel;
                    Data = nativeFrameBuffer.Data;
                    DataSize = (int)nativeFrameBuffer.Size;
                }

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
                public readonly int DataSize;

                /// <summary>
                /// Image data.
                /// </summary>
                public readonly IntPtr Data;

                public override string ToString() => $"Width: {Width}, Height: {Height}, Stride: {Stride}, BytesPerPixel: {BytesPerPixel}, Size: {DataSize}";
            };
        }

        /// <summary>
        /// World camera intrinsic parameters.
        /// </summary>
        public readonly struct CameraIntrinsics
        {
            internal CameraIntrinsics(in NativeBindings.MLWorldCameraIntrinsics nativeIntrinsics)
            {
                Width = nativeIntrinsics.Width;
                Height = nativeIntrinsics.Height;
                FocalLength = nativeIntrinsics.FocalLength;
                PrincipalPoint = nativeIntrinsics.PrincipalPoint;
                Fov = nativeIntrinsics.Fov;
                RadialDistortion = nativeIntrinsics.RadialDistortion;
                TangentialDistortion = nativeIntrinsics.TangentialDistortion;
            }

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
            public readonly double[] RadialDistortion;

            /// <summary>
            /// Tangential distortion vector.
            /// The tangential distortion co-efficients are in the following order: [p1, p2].
            /// </summary>
            public readonly double[] TangentialDistortion;

            public override string ToString() => $"Width:{Width}, Height:{Height}, FocalLength: {FocalLength}, PrincipalPoint: {PrincipalPoint} , FOV: {Fov}, RadialDistortion: {GetArrayAsString(RadialDistortion)}, TangentialDistortion: {GetArrayAsString(TangentialDistortion)}";

            private string GetArrayAsString(double[] distortions)
            {
                StringBuilder sb = new StringBuilder("[");
                for (int i = 0; i < distortions.Length; ++i)
                {
                    var value = distortions[i];
                    sb.Append($"{value}");
                    if (i < distortions.Length - 1)
                        sb.Append(", ");
                }

                sb.Append("]");

                return sb.ToString();
            }
        }
    }
}
