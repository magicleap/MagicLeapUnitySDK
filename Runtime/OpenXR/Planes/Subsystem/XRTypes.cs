// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Runtime.InteropServices;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MLXrPlaneSubsystem
    {
        public static class XrTypes
        {
            public enum MLXrPlaneDetectorOrientation : uint
            {
                HorizontalUpward,
                HorizontalDownward,
                Vertical,
                Arbitrary,
            }

            public enum MLXrPlaneDetectorSemanticType : uint
            {
                Ceiling = 1U,
                Floor,
                Wall,
                Platform,
            }

            public enum MLXrPlaneDetectionState : uint
            {
                None = 0,
                Pending,
                Done,
                Error,
                Fatal
            }
            
            [StructLayout(LayoutKind.Sequential)]
            internal unsafe struct MLXrPlaneDetectorBeginInfo
            {
                internal uint OrientationCount;
                internal MLXrPlaneDetectorOrientation* Orientations;
                internal uint SemanticTypeCount;
                internal MLXrPlaneDetectorSemanticType* SemanticTypes;
                internal uint MaxPlanes;
                internal float MinArea;
                internal Pose BoundingBoxPose;
                internal Vector3 BoundingBoxExtents;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal readonly struct MLXrPlaneDetectorLocation
            {
                internal readonly ulong PlaneId;
                internal readonly Pose Pose;
                internal readonly Vector2 Extents;
                internal readonly MLXrPlaneDetectorOrientation Orientation;
                internal readonly MLXrPlaneDetectorSemanticType SemanticType;
                internal readonly uint PolygonBufferCount;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal unsafe struct MLXrPlaneDetectorLocations
            {
                internal uint PlaneLocationCapacityInput;
                internal readonly uint PlaneLocationCountOutput;
                internal MLXrPlaneDetectorLocation* PlaneLocations;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal unsafe struct MLXrPlaneDetectorPolygonBuffer
            {
                internal uint VertexCapacityInput;
                internal readonly uint VertexCountOutput;
                internal  Vector2* Vertices;
            }
        }
    }
}
