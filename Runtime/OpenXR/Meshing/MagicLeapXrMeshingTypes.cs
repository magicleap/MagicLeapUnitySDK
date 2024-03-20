#if UNITY_OPENXR_1_9_0_OR_NEWER

using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapMeshingFeature
    {
        public enum MeshingMode : byte
        {
            Triangles,
            PointCloud
        }

        internal unsafe struct FrameMeshInfo
        {
            internal TrackableId* addedIds;
            internal uint addedCount;
            internal TrackableId* removedIds;
            internal uint removedCount;
            internal TrackableId* updatedIds;
            internal uint updatedCount;
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
            [MarshalAs(UnmanagedType.I1)] public bool useIonAllocator;

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
}

#endif
