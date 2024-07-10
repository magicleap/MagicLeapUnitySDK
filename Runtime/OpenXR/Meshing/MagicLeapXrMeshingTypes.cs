
using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.ARSubsystems;

namespace MagicLeap.OpenXR.Features.Meshing
{
    internal unsafe struct XrMeshId
    {
        internal fixed ulong Id[2];
        
        internal static XrMeshId CreateFrom<T>(T id)  where T: unmanaged
        {
            var copy = id;
            return *(XrMeshId*)&copy;
        }
        
        
        internal T Convert<T>() where T : unmanaged
        {
            var copy = this;
            return *(T*)(&copy);
        }
    }
    
    public enum MeshingMode : byte
    {
        Triangles,
        PointCloud
    }

    internal unsafe struct FrameMeshInfo
    {
        internal TrackableId* AddedIds;
        internal uint AddedCount;
        internal TrackableId* RemovedIds;
        internal uint RemovedCount;
        internal TrackableId* UpdatedIds;
        internal uint UpdatedCount;
    }

    /// <summary>
    ///     The flags to represent the generated mesh's properties
    /// </summary>
    [Flags]
    public enum MeshDetectorFlags : byte
    {
        /// <summary>
        ///     Whether to compute the normals of the mesh
        /// </summary>
        ComputeNormals = 1 << 1,

        /// <summary>
        ///     Whether to compute the confidence data for the mesh
        /// </summary>
        ComputeConfidence = 1 << 2,

        /// <summary>
        ///     Whether to planarize the generated mesh
        /// </summary>
        Planarize = 1 << 3,

        /// <summary>
        ///     When enabled, the mesh skirt (overlapping area between two mesh blocks) will be removed. This field is only valid
        ///     when the mesh is not a point cloud.
        /// </summary>
        MeshSkirt = 1 << 4
    }

    /// <summary>
    ///     The mesh generation settings
    /// </summary>
    [Serializable]
    public struct MeshingQuerySettings
    {
        public float fillHoleLength;
        public float appliedDisconnectedComponentArea;
        public MeshDetectorFlags meshDetectorFlags;

        /// <summary>
        ///     Whether to use the ion allocator on the device to store the mesh data.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool useIonAllocator;

        public static MeshingQuerySettings DefaultSettings()
        {
            return new MeshingQuerySettings
            {
                fillHoleLength = 0.25f,
                appliedDisconnectedComponentArea = 0.25f,
                meshDetectorFlags = MeshDetectorFlags.Planarize | MeshDetectorFlags.ComputeNormals,
                useIonAllocator = false,
            };
        }
    }
}

