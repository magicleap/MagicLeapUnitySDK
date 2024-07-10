using System;
using UnityEngine;

namespace MagicLeap.OpenXR.Features.Planes
{
    internal enum XrPlaneStructTypes : ulong
    {
        PlaneDetectorCreateInfo = 1000429001,
        PlaneDetectorBeginInfo = 1000429002,
        PlaneDetectorGetInfo = 1000429003,
        PlaneDetectorLocations = 1000429004,
        PlaneDetectorLocation = 1000429005,
        PlaneDetectorPolygonBuffer = 1000429006,
    }
    
    internal struct XrPlaneDetector
    {
        private readonly ulong handle;

        internal XrPlaneDetector(ulong xrHandle = default)
        {
            handle = xrHandle;
        }
        public static implicit operator XrPlaneDetector(ulong handle) => new(handle);
        public static implicit operator ulong(XrPlaneDetector planeDetector) => planeDetector.handle;
    }


    internal enum XrPlaneDetectionState
    {
        None,
        Pending,
        Done,
        Error,
        Fatal
    }

    [Flags]
    internal enum XrPlaneDetectionCapabilityFlags : ulong
    {
        PlaneDetection = 0x00000001,
        PlaneHoles = 0x00000002,
        SemanticCeiling = 0x00000004,
        SemanticFloor = 0x00000008,
        SemanticWall = 0x00000010,
        SemanticPlatform = 0x00000020,
        Orientation = 0x00000040
    }

    [Flags]
    internal enum XrPlaneDetectorFlags : ulong
    {
        XrPlaneDetectorEnableContourBit = 0x00000001
    }

    internal struct XrSystemPlaneDetectionProperties
    {
        internal XrPlaneStructTypes Type;
        internal IntPtr Next;
        internal XrPlaneDetectionCapabilityFlags SupportedFeatures;
    }

    internal struct XrPlaneDetectorCreateInfo
    {
        internal XrPlaneStructTypes Type;
        internal IntPtr Next;
        internal XrPlaneDetectorFlags Flags;
    }

    internal unsafe struct XrPlaneDetectorBeginInfo
    {
        internal XrPlaneStructTypes Type;
        internal IntPtr Next;
        internal ulong Space;
        internal long Time;
        internal uint OrientationCount;
        internal XrPlaneDetectorOrientation* Orientations;
        internal uint SemanticTypesCount;
        internal XrPlaneDetectorSemanticTypes* SemanticTypes;
        internal uint MaxPlanes;
        internal float MinArea;
        internal XrPose BoundingBoxPose;
        internal Vector3 BoundingBoxExtents;
    }

    internal struct XrPlaneDetectorGetInfo
    {
        internal XrPlaneStructTypes Type;
        internal IntPtr Next;
        internal ulong Space;
        internal long Time;
    }

    internal struct XrPlaneDetectorLocation
    {
        internal XrPlaneStructTypes Type;
        internal IntPtr Next;
        internal ulong PlaneId;
        internal Spaces.XrSpaceLocationFlagsML LocationFlags;
        internal XrPose Pose;
        internal Vector2 Extents;
        internal XrPlaneDetectorOrientation Orientation;
        internal XrPlaneDetectorSemanticTypes SemanticType;
        internal uint PolygonBufferCount;
    }

    internal unsafe struct XrPlaneDetectorLocations
    {
        internal XrPlaneStructTypes Type;
        internal IntPtr Next;
        internal uint PlaneLocationCapacityInput;
        internal uint PlaneLocationCountOutput;
        internal XrPlaneDetectorLocation* PlaneLocations;
    }

    internal unsafe struct XrPlaneDetectorPolygonBuffer
    {
        internal XrPlaneStructTypes Type;
        internal IntPtr Next;
        internal uint VertexCapacityInput;
        internal uint VertexCountOutput;
        internal Vector2* Vertices;
    }
}
