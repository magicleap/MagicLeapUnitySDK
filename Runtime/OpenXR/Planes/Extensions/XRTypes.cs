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
            internal struct MLXrPose
            {
                internal Vector3 position;
                internal Quaternion rotation;
            }
            

            [StructLayout(LayoutKind.Sequential)]
            internal unsafe struct MLXrPlaneDetectorBeginInfo
            {
                internal uint orientationCount;
                internal MLXrPlaneDetectorOrientation* orientations;
                internal uint semanticTypeCount;
                internal MLXrPlaneDetectorSemanticType* semanticTypes;
                internal uint maxPlanes;
                internal float minArea;
                internal MLXrPose boundingBoxPose;
                internal Vector3 boundingBoxExtents;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal readonly struct MLXrPlaneDetectorLocation
            {
                internal readonly ulong planeId;
                internal readonly MLXrPose pose;
                internal readonly Vector2 extents;
                internal readonly MLXrPlaneDetectorOrientation orientation;
                internal readonly MLXrPlaneDetectorSemanticType semanticType;
                internal readonly uint polygonBufferCount;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal unsafe struct MLXrPlaneDetectorLocations
            {
                internal uint planeLocationCapacityInput;
                internal readonly uint planeLocationCountOutput;
                internal MLXrPlaneDetectorLocation* planeLocations;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal unsafe struct MLXrPlaneDetectorPolygonBuffer
            {
                internal uint vertexCapacityInput;
                internal readonly uint vertexCountOutput;
                internal  Vector2* vertices;
            }
        }
    }
}
