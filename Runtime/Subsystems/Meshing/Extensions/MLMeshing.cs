// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.ARSubsystems;
    using UnityEngine.XR.MagicLeap.Native;

    public partial class MeshingSubsystem
    {
        public static partial class Extensions
        {
            public static partial class MLMeshing
            {

                /// <summary>
                /// State of a block mesh.
                /// </summary>
                public enum MeshState
                {
                    /// <summary>
                    /// Mesh has been created. 
                    /// </summary>
                    New,

                    /// <summary>
                    /// Mesh has been updated.
                    /// </summary>
                    Updated,

                    /// <summary>
                    /// Mesh has been deleted.
                    /// </summary>
                    Deleted,

                    /// <summary>
                    /// Mesh is unchanged.
                    /// </summary>
                    Unchanged,
                }

                /// <summary>
                /// Level of detail of the block mesh.
                /// </summary>
                public enum LevelOfDetail
                {
                    /// <summary>
                    /// Minimum Level of Detail (LOD) for the mesh.
                    /// </summary>
                    Minimum,

                    /// <summary>
                    /// Medium Level of Detail (LOD) for the mesh.
                    /// </summary>
                    Medium,

                    /// <summary>
                    /// Maximum Level of Detail (LOD) for the mesh.
                    /// </summary>
                    Maximum
                }
                
                /// <summary>
                /// Step the Level of detail to Minimum, Medium and Maximum
                /// </summary>
                /// <param name="density">float input value</param>
                /// <returns></returns>
                public static LevelOfDetail DensityToLevelOfDetail( float density )
                {
                    if (density < 0.33f)
                        return LevelOfDetail.Minimum;
                    else if (density < 0.66f)
                        return LevelOfDetail.Medium;
                    else
                        return LevelOfDetail.Maximum;
                }
                
                /// <summary>
                /// Convert a LevelOfDetail to a float, ranged between 0 and 1
                /// </summary>
                /// <param name="lod">Level of detail</param>
                /// <returns>Float value between 0 and 1</returns>
                public static float LevelOfDetailToDensity( LevelOfDetail lod )
                {
                    if (lod == LevelOfDetail.Minimum)
                        return 0.0f;
                    else if (lod == LevelOfDetail.Medium)
                        return 0.5f;
                    else
                        return 1.0f;
                }

                public delegate MeshingSubsystem.Extensions.MLMeshing.MeshBlockRequest[] OnMeshBlockRequests(MeshingSubsystem.Extensions.MLMeshing.MeshBlockInfo[] blockInfos);

                /// <summary>
                /// Representation of a mesh block.
                /// </summary>
                public readonly struct MeshBlockInfo
                {
                    internal MeshBlockInfo(MLMeshing.NativeBindings.MLMeshingBlockInfo nativeBlockInfo)
                    {
                        id = nativeBlockInfo.cfuid.ToString();
                        pose = new Pose(MLConvert.ToUnity(nativeBlockInfo.extents.center), MLConvert.ToUnity(nativeBlockInfo.extents.rotation));
                        extents = MLConvert.ToUnity(nativeBlockInfo.extents.extents);
                        MLTime.ConvertSystemTimeToMLTime(nativeBlockInfo.timestamp, out timestamp);
                        state = nativeBlockInfo.state;
                    }

                    /// <summary>
                    /// The coordinate frame UID to represent the block.
                    /// </summary>
                    public readonly string id;

                    /// <summary>
                    /// The pose of the mesh block.
                    /// </summary>
                    public readonly Pose pose;

                    /// <summary>
                    /// The extents of the bounding box.
                    /// </summary>
                    public readonly Vector3 extents;

                    /// <summary>
                    /// The timestamp when block was updated.
                    /// </summary>
                    public readonly MLTime timestamp;

                    /// <summary>
                    /// The state of the mesh block.
                    /// </summary>
                    public readonly MeshState state;
                }

                public readonly struct MeshBlockRequest
                {
                    internal MeshBlockRequest(MLMeshing.NativeBindings.MLMeshingBlockRequest nativeBlockRequest)
                    {
                        this.id = nativeBlockRequest.cfuid.ToString();
                        this.levelOfDetail = nativeBlockRequest.levelOfDetail;
                    }

                    public MeshBlockRequest(string id, LevelOfDetail levelOfDetail)
                    {
                        this.id = id;
                        this.levelOfDetail = levelOfDetail;
                    }

                    /// <summary>
                    /// The id to represent the block to be requested.
                    /// </summary>
                    public readonly string id;

                    /// <summary>
                    /// The LOD level to request.
                    /// </summary>
                    public readonly LevelOfDetail levelOfDetail;
                }


                internal static class NativeBindings
                {

                    /// <summary>
                    /// Axis aligned bounding box for querying updated mesh info.
                    /// </summary>
                    [StructLayout(LayoutKind.Sequential)]
                    public struct MLMeshingExtents
                    {
                        /// <summary>
                        /// The center of the bounding box.
                        /// </summary>
                        public MagicLeapNativeBindings.MLVec3f center;

                        /// <summary>
                        /// The rotation of the bounding box. 
                        /// </summary>
                        public MagicLeapNativeBindings.MLQuaternionf rotation;

                        /// <summary>
                        /// The size of the bounding box. When a mesh request is complete all mesh blocks within 10 meters 
                        /// of the device that intersect with the extents provided here will be returned in MLMeshingMeshInfo.
                        /// </summary>
                        public MagicLeapNativeBindings.MLVec3f extents;
                    }

                    /// <summary>
                    /// Representation of a mesh block.
                    /// </summary>
                    [StructLayout(LayoutKind.Sequential)]
                    public struct MLMeshingBlockInfo
                    {
                        /// <summary>
                        /// The coordinate frame UID to represent the block.
                        /// </summary>
                        public MagicLeapNativeBindings.MLCoordinateFrameUID cfuid;

                        /// <summary>
                        /// The extents of the bounding box.
                        /// </summary>
                        public MLMeshingExtents extents;

                        /// <summary>
                        /// The timestamp when block was updated.
                        /// </summary>
                        public long timestamp;

                        /// <summary>
                        /// The state of the Mesh Block.
                        /// </summary>
                        public MeshState state;
                    }

                    /// <summary>
                    /// Response structure for the mesh block info.
                    /// </summary>
                    [StructLayout(LayoutKind.Sequential)]
                    public struct MLMeshingMeshInfo
                    {
                        /// <summary>
                        /// The response timestamp to a earlier request.
                        /// </summary>
                        public long timestamp;

                        /// <summary>
                        /// The number of responses in data pointer.
                        /// </summary>
                        public uint dataCount;

                        /// <summary>
                        /// The meshinfo returned by the system.
                        /// </summary>
                        public IntPtr data;
                    }

                    [StructLayout(LayoutKind.Sequential)]
                    public readonly struct MLMeshingBlockRequest
                    {
                        internal MLMeshingBlockRequest(MLMeshing.MeshBlockRequest blockRequest)
                        {
                            this.cfuid = MagicLeapNativeBindings.MLCoordinateFrameUID.EmptyFrame;
                            this.cfuid.FromString(blockRequest.id);
                            this.levelOfDetail = blockRequest.levelOfDetail;
                        }

                        internal MLMeshingBlockRequest(MagicLeapNativeBindings.MLCoordinateFrameUID cfuid, MLMeshing.LevelOfDetail levelOfDetail)
                        {
                            this.cfuid = cfuid;
                            this.levelOfDetail = levelOfDetail;
                        }

                        /// <summary>
                        /// The UID to represent the block.
                        /// </summary>
                        public readonly MagicLeapNativeBindings.MLCoordinateFrameUID cfuid;

                        /// <summary>
                        /// The LOD level to request.
                        /// </summary>
                        public readonly MLMeshing.LevelOfDetail levelOfDetail;
                    }

                    [StructLayout(LayoutKind.Sequential)]
                    public struct MLMeshingMeshRequest
                    {
                        /// <summary>
                        /// Type selector for the structure.
                        /// </summary>
                        public int requestCount;

                        /// <summary>
                        /// Binary data size.
                        /// </summary>
                        public IntPtr blockRequests;
                    }
                }

                public static partial class Config
                {
                    public static void SetCustomMeshBlockRequests(OnMeshBlockRequests onBlockRequests)
                    {
                        customBlockRequests = onBlockRequests;

                        if (customBlockRequests != null)
                        {
                            MagicLeapXrProviderNativeBindings.MeshingSetMeshRequestCallback(OnMeshRequest);
                            MagicLeapXrProviderNativeBindings.MeshingSetFreeBlockRequestPointerCallback(OnFreeMeshRequestPointer);
                        }
                        else
                        {
                            MagicLeapXrProviderNativeBindings.MeshingSetMeshRequestCallback(null);
                            MagicLeapXrProviderNativeBindings.MeshingSetFreeBlockRequestPointerCallback(null);
                        }
                    }

                    public static IntPtr AcquireConfidence(MeshId meshId, out int count) => NativeBindings.MeshingAcquireConfidence(meshId, out count);
                    public static void ReleaseConfidence(MeshId meshId) => NativeBindings.MeshingReleaseConfidence(meshId);
                    public static void SetBounds(Transform transform, Vector3 extents) => SetBounds(transform.localPosition, transform.localRotation, extents);
                    public static void SetBounds(Vector3 position, Quaternion rotation, Vector3 extents) => NativeBindings.MeshingSetBounds(position, rotation, extents);

                    public static int batchSize
                    {
                        set { NativeBindings.MeshingSetBatchSize(value); }
                    }

                    public static float density
                    {
                        set { NativeBindings.MeshingSetDensity(value); }
                    }

                    public static Settings meshingSettings
                    {
                        set
                        {
                            SubsystemFeatures.SetCurrentFeatureEnabled(Feature.Meshing | Feature.PointCloud, false);
                            if (value.flags.HasFlag(Flags.PointCloud))
                            {
                                SubsystemFeatures.SetCurrentFeatureEnabled(Feature.PointCloud, true);
                            }
                            else
                            {
                                SubsystemFeatures.SetCurrentFeatureEnabled(Feature.Meshing, true);
                            }
                            NativeBindings.MeshingUpdateSettings(ref value);
                        }
                    }

                    [Flags]
                    public enum Flags
                    {
                        None = 0,
                        PointCloud = 1 << 0,
                        ComputeNormals = 1 << 1,
                        ComputeConfidence = 1 << 2,
                        Planarize = 1 << 3,
                        RemoveMeshSkirt = 1 << 4,
                        IndexOrderCW = 1 << 5
                    }

                    private static IntPtr arrayPtr;
                    private static OnMeshBlockRequests customBlockRequests;

                    [AOT.MonoPInvokeCallback(typeof(MagicLeapXrProviderNativeBindings.CreateBlockRequestsDelegate))]
                    private static void OnMeshRequest(ref MeshingSubsystem.Extensions.MLMeshing.NativeBindings.MLMeshingMeshInfo meshInfo, ref MeshingSubsystem.Extensions.MLMeshing.NativeBindings.MLMeshingMeshRequest meshRequest)
                    {
                        if (meshInfo.data == IntPtr.Zero)
                            return;

                        var blockInfos = new MeshingSubsystem.Extensions.MLMeshing.MeshBlockInfo[meshInfo.dataCount];

                        IntPtr walkPtr = meshInfo.data;
                        for (int i = 0; i < meshInfo.dataCount; ++i)
                        {
                            var nativeBlockInfo = Marshal.PtrToStructure<MeshingSubsystem.Extensions.MLMeshing.NativeBindings.MLMeshingBlockInfo>(walkPtr);
                            blockInfos[i] = new MeshingSubsystem.Extensions.MLMeshing.MeshBlockInfo(nativeBlockInfo);
                            walkPtr = new IntPtr(walkPtr.ToInt64() + Marshal.SizeOf<MeshingSubsystem.Extensions.MLMeshing.NativeBindings.MLMeshingBlockInfo>());
                        }

                        var blockRequests = customBlockRequests(blockInfos);
                        var nativeBlockRequests = new MLMeshing.NativeBindings.MLMeshingBlockRequest[blockRequests.Length];
                        for (int i = 0; i < nativeBlockRequests.Length; ++i)
                        {
                            nativeBlockRequests[i] = new MLMeshing.NativeBindings.MLMeshingBlockRequest(blockRequests[i]);
                        }

                        meshRequest.requestCount = nativeBlockRequests.Length;
                        meshRequest.blockRequests = MarshalArrayToPtr(nativeBlockRequests);
                    }

                    [AOT.MonoPInvokeCallback(typeof(MagicLeapXrProviderNativeBindings.CallFreeBlockRequestPointerDelegate))]
                    private static void OnFreeMeshRequestPointer()
                    {
                        Marshal.FreeHGlobal(arrayPtr);
                    }

                    private static IntPtr MarshalArrayToPtr<T>(T[] array)
                    {
                        arrayPtr = Marshal.AllocHGlobal(Marshal.SizeOf<T>() * array.Length);

                        IntPtr walkPtr = arrayPtr;

                        foreach (var element in array)
                        {
                            Marshal.StructureToPtr(element, walkPtr, true);
                            walkPtr = new IntPtr(walkPtr.ToInt64()) + Marshal.SizeOf<T>();
                        }

                        return arrayPtr;
                    }

                    [StructLayout(LayoutKind.Sequential)]
                    public struct Settings
                    {
                        public Flags flags;
                        public float fillHoleLength;
                        public float disconnectedComponentArea;
                    }



                }

            }
        }
    }
}
