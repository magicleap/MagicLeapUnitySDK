// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2022 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLDepthCamera
    {
        /// <summary>
        /// Output data for each depth camera sensor.
        /// </summary>
        public sealed class Data
        {
            /// <summary>
            /// A 64bit integer to index the frame number associated with this frame. 
            /// </summary>
            public long FrameNumber { get; internal set; }

            /// <summary>
            /// Frame timestamp specifies the time at which the frame was captured. 
            /// </summary>
            public MLTime FrameTimestamp { get; internal set; }

            /// <summary>
            /// Depth camera mode used for capturing this frame.
            /// </summary>
            public FrameType FrameType { get; internal set; }

            /// <summary>
            /// World position of the depth camera pose
            /// </summary>
            public Vector3 Position { get; internal set; }

            /// <summary>
            /// World rotation of the depth camera pose
            /// </summary>
            public Quaternion Rotation { get; internal set; }

            /// <summary>
            /// Camera intrinsic parameters
            /// </summary>
            public Intrinsics Intrinsics { get; internal set; }

            /// <summary>
            /// <para>Depth map stores the depth data from the depth camera. Depth is represented in
            /// meters and gives the radial distance of the real world location from the
            /// depth camera coordinate frame. See <see cref="Intrinsics"/>, <see cref="Position"/>, and
            /// <see cref="Rotation"/> for more details.</para>
            /// Cast the <see cref="FrameBuffer.Data"/> to float data type.
            /// </summary>
            public FrameBuffer? DepthImage { get; internal set; }

            /// <summary>
            /// <para>Confidence map stores the confidence values for each pixel in the <see cref="DepthImage"/> buffer. The confidence score is derived from the sensor
            /// noise and it is not normalized. The higher the value the higher the confidence. Applications can determine what confidence threshold to use
            /// based on their use case.</para>
            /// Cast the <see cref="FrameBuffer.Data"/> to float data type.
            /// </summary>
            public FrameBuffer? ConfidenceBuffer { get; internal set; }

            /// <summary>
            /// Each pixel in the <see cref="DepthImage"/> frame buffer will have an associated set of flags.
            /// </summary>
            public FrameBuffer? DepthFlagsBuffer { get; internal set; }

            /// <summary>
            /// This is the raw depth camera sensor data captured without the depth camera
            /// illumination and corresponds to the amount of ambient light incident on the sensor.
            /// </summary>
            public FrameBuffer? AmbientRawDepthImage { get; internal set; }

            /// <summary>
            /// This is the raw depth camera sensor data captured with the depth camera
            /// illumination and corresponds to the amount of total light incident on the
            /// sensor.
            /// </summary>
            public FrameBuffer? RawDepthImage { get; internal set; }
        }
    }
}
