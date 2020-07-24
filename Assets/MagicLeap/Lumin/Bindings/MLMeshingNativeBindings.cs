// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLMeshingNativeBindings.cs" company="Magic Leap, Inc">
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
    using Unity.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap.Native;
    using UnityEngine.Rendering;
    using Unity.Collections.LowLevel.Unsafe;

    #if PLATFORM_LUMIN

    /// <summary>
    /// Handles the meshing API.
    /// </summary>
    public sealed partial class MLMeshing : MLAPISingleton<MLMeshing>
    {
        /// <summary>
        /// See ml_meshing2.h for additional comments.
        /// </summary>
        private partial class NativeBindings : Native.MagicLeapNativeBindings
        {
            /// <summary>
            /// Prevents a default instance of the <see cref="NativeBindings"/> class from being created.
            /// </summary>
            private NativeBindings()
            {
            }

            /// <summary>
            /// Create the meshing client.
            /// </summary>
            /// <param name="clientHandle">The handle to the created client.</param>
            /// <param name="settings">The initial settings to be used for meshing.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed in.
            /// MLResult.Result will be<c> MLResult.Code.PrivilegeDenied</c> if there was a lack of privileges.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMeshingCreateClient(out ulong clientHandle, in MLMeshing.MeshSettings settings);

            /// <summary>
            /// Free the client resources.
            /// </summary>
            /// <param name="clientHandle">The client to destroy.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed in.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMeshingDestroyClient(in ulong clientHandle);

            /// <summary>
            /// Initialize the meshing settings with system defaults.
            /// </summary>
            /// <param name="settings">The initial settings to be used for meshing.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed in.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMeshingInitSettings(out MLMeshing.MeshSettings settings);

            /// <summary>
            /// Update the meshing settings at runtime.
            /// </summary>
            /// <param name="clientHandle">The handle to the created client.</param>
            /// <param name="settings">The updated settings to be used for meshing.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed in.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMeshingUpdateSettings(ulong clientHandle, in MLMeshing.MeshSettings settings);

            /// <summary>
            /// Request the Mesh Info which includes CFUIDs and bounding extents of the blocks.
            /// </summary>
            /// <param name="clientHandle">The handle to the created client.</param>
            /// <param name="extents">The region of interest for meshing.</param>
            /// <param name="requestHandle">The handle for the current request. Needs to be passed to query the result of the request.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed in.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMeshingRequestMeshInfo(ulong clientHandle, in NativeBindings.MeshExtentsNative extents, out ulong requestHandle);

            /// <summary>
            /// Get the Result of a previous MeshInfo request.
            /// </summary>
            /// <param name="clientHandle">The handle to the created client.</param>
            /// <param name="requestHandle">The handle populated in a prev MLMeshingGetMeshInfo.</param>
            /// <param name="info">The final result which will be populated only if the result is successful.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed in.
            /// MLResult.Result will be <c>MLResult.Code.Pending</c> if pending update.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMeshingGetMeshInfoResult(ulong clientHandle, ulong requestHandle, out NativeBindings.MeshInfoNative info);

            /// <summary>
            /// Request the Mesh for all CFUIDs populated in request.
            /// </summary>
            /// <param name="clientHandle">The handle to the created client.</param>
            /// <param name="request">The request for meshes of interest.</param>
            /// <param name="requestHandle">The handle for the current request. Needs to be passed to query the result of the request.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed in.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMeshingRequestMesh(ulong clientHandle, in NativeBindings.MeshRequestNative request, out ulong requestHandle);

            /// <summary>
            /// Get the Result of a previous Mesh request.
            /// </summary>
            /// <param name="clientHandle">The handle to the created client.</param>
            /// <param name="requestHandle">The handle populated in a prev MLMeshingGetMesh.</param>
            /// <param name="meshResults">The final result which will be populated only if the result is successful.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed in.
            /// MLResult.Result will be <c>MLResult.Code.Pending</c> if pending update.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMeshingGetMeshResult(ulong clientHandle, ulong requestHandle, out NativeBindings.MeshRequestResultNative meshResults);

            /// <summary>
            /// Free resources created by the meshing APIS. Needs to be called whenever MLMeshingGetMeshInfoResult,
            /// MLMeshingGetMeshResult return a success.
            /// </summary>
            /// <param name="clientHandle">The handle to the created client.</param>
            /// <param name="requestHandle">The handle populated in a prev request.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed in.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMeshingFreeResource(ulong clientHandle, in ulong requestHandle);

            /// <summary>
            /// Axis aligned bounding box for querying updated mesh info.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MeshExtentsNative
            {
                /// <summary>
                /// The center of the bounding box.
                /// </summary>
                public MLVec3f Center;

                /// <summary>
                /// The rotation of the bounding box.
                /// </summary>
                public MLQuaternionf Rotation;

                /// <summary>
                /// The size of the bounding box. When a mesh request is complete
                /// all mesh blocks that intersect with the extents provided here
                /// will be returned in MLMeshingMeshInfo.
                /// </summary>
                public MLVec3f Extents;

                /// <summary>
                /// Gets or sets the managed structure from the unmanaged one.
                /// </summary>
                public MLMeshing.MeshExtents Data
                {
                    get
                    {
                        return new MLMeshing.MeshExtents
                        {
                            Center = MLConvert.ToUnity(this.Center),
                            Rotation = MLConvert.ToUnity(this.Rotation),
                            Extents = MLConvert.ToUnity(this.Extents, false)
                        };
                    }
                    set
                    {
                        this.Center = MLConvert.FromUnity(value.Center);
                        this.Rotation = MLConvert.FromUnity(value.Rotation);
                        this.Extents = MLConvert.FromUnity(value.Extents, false);
                    }
                }
            }

            /// <summary>
            /// Response structure for the mesh block info.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MeshInfoNative
            {
                /// <summary>
                /// The response timestamp (in microseconds) to a earlier request.
                /// </summary>
                public ulong Timestamp;

                /// <summary>
                /// The number of responses in data pointer.
                /// </summary>
                public uint DataCount;

                /// <summary>
                /// The meshinfo returned by the system.<c>MLMeshing.BlockInfoNative</c> values.
                /// </summary>
                public IntPtr BlockInfoData;

                /// <summary>
                /// Gets the managed structure from the unmanaged one.
                /// </summary>
                public List<BlockInfo> Data
                {
                    get
                    {
                        List<BlockInfo> blocks = new List<BlockInfo>();

                        IntPtr walker = BlockInfoData;
                        int blockSize = Marshal.SizeOf(typeof(BlockInfo));

                        for (int i = 0; i < DataCount; ++i)
                        {
                            unsafe
                            {
                                BlockInfoNative* newBlock = (BlockInfoNative*)walker;
                                blocks.Add(newBlock->Data);
                            }

                            walker = new IntPtr(walker.ToInt64() + blockSize);
                        }

                        return blocks;
                    }
                }
            }

            /// <summary>
            /// Representation of a mesh block.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct BlockInfoNative
            {
                /// <summary>
                /// The coordinate frame UID to represent the block.
                /// </summary>
                public MagicLeapNativeBindings.MLCoordinateFrameUID ID;

                /// <summary>
                /// The extents of the bounding box.
                /// </summary>
                public MeshExtentsNative Extents;

                /// <summary>
                /// The timestamp (in microseconds) when block was updated.
                /// </summary>
                public ulong Timestamp;

                /// <summary>
                /// The state of the Mesh Block.
                /// </summary>
                public MeshState State;

                /// <summary>
                /// Gets the managed structure from the unmanaged one.
                /// </summary>
                public BlockInfo Data
                {
                    get
                    {
                        return new BlockInfo
                        {
                            ID = this.ID,
                            Extents = this.Extents.Data,
                            Timestamp = this.Timestamp,
                            State = this.State
                        };
                    }
                }
            }

            /// <summary>
            /// Request structure to get the actual mesh for a set of blocks.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MeshRequestNative
            {
                /// <summary>
                /// The number of blocks requested.
                /// </summary>
                public int RequestCount;

                /// <summary>
                /// Per block request. <c>MLMeshing.MeshRequest</c> values.
                /// </summary>
                public IntPtr Data;

                /// <summary>
                /// Create new unmanaged block request buffer from managed List.
                /// </summary>
                /// <param name="requests">The list of block request.</param>
                /// <returns>A new allocated MeshRequestNative struct.</returns>
                public unsafe static MeshRequestNative Create(in List<MeshRequest> requests)
                {
                    MeshRequestNative result = new MeshRequestNative()
                    {
                        RequestCount = requests.Count,
                    };

                    fixed (MeshRequest* pMeshRequests = requests.ToArray())
                    {
                        result.Data = (IntPtr)pMeshRequests;
                    }

                    return result;
                }
            }

            /// <summary>
            /// Result of a Mesh request. See MLMeshingGetMeshResult().
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MeshRequestResultNative
            {
                /// <summary>
                /// The result of the meshing. Can have partial updates.
                /// </summary>
                public MeshResult Result;

                /// <summary>
                /// The timestamp (in microseconds) when data was generated.
                /// </summary>
                public ulong Timestamp;

                /// <summary>
                /// Number of meshes available in data.
                /// </summary>
                public uint DataCount;

                /// <summary>
                /// The mesh data. <c>NativeBindings.BlockMeshNative</c> values.
                /// </summary>
                public IntPtr BlockMeshData;

                /// <summary>
                /// Gets the managed structure from the unmanaged one.
                /// </summary>
                public MeshRequestResult Data
                {
                    get
                    {
                        List<BlockMesh> meshes = new List<BlockMesh>();

                        if(this.DataCount > 0 && this.BlockMeshData != IntPtr.Zero)
                        {
                            IntPtr walker = BlockMeshData;
                            int meshSize = Marshal.SizeOf(typeof(BlockMeshNative));

                            for (int i = 0; i < DataCount; ++i)
                            {
                                unsafe
                                {
                                    BlockMeshNative* newMesh = (BlockMeshNative*)walker;
                                    meshes.Add(newMesh->Data);
                                }

                                walker = new IntPtr(walker.ToInt64() + meshSize);
                            }
                        }

                        return new MeshRequestResult()
                        {
                            Result = this.Result,
                            Timestamp = this.Timestamp,
                            Meshes = meshes
                        };

                    }
                }
            }

            /// <summary>
            /// Final structure for a block mesh.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct BlockMeshNative
            {
                /// <summary>
                /// The result of the meshing.
                /// </summary>
                public MeshResult Result;

                /// <summary>
                /// The coordinate FrameID of the block.
                /// </summary>
                public MagicLeapNativeBindings.MLCoordinateFrameUID ID;

                /// <summary>
                /// The LOD level of the meshing request.
                /// </summary>
                public LOD Level;

                /// <summary>
                /// The settings with which meshing took place.
                /// </summary>
                public SettingsFlags Flags;

                /// <summary>
                /// The number of indices in index buffer.
                /// </summary>
                public ushort IndexCount;

                /// <summary>
                /// The number of vertices in vertex/normal/confidence buffer.
                /// </summary>
                public uint VertexCount;

                /// <summary>
                /// Pointer to the vertex buffer. <c>Vector3</c> values.
                /// </summary>
                public IntPtr Vertex;

                /// <summary>
                /// Pointer to index buffer. <c>Short</c> values.
                /// In the index buffer each value is the index of a vertex in the vertex
                /// buffer.Three indices define one triangle.For example, the first triangle
                /// will have the vertices: vertex[index[0]], vertex[index[1]], vertex[index[2]].
                /// Index order is defined by the MLMeshingFlags_IndexOrderCW flag.
                /// </summary>
                public IntPtr Index;

                /// <summary>
                /// Pointer to normals buffer. <c>Vector3</c> values.
                /// </summary>
                public IntPtr Normal;

                /// <summary>
                /// Pointer to confidence buffer. <c>Float</c> values.
                /// </summary>
                public IntPtr Confidence;

                /// <summary>
                /// Gets the managed structure from the unmanaged one.
                /// </summary>
                public BlockMesh Data
                {
                    get
                    {
                        unsafe
                        {
                            Mesh mesh = new Mesh();

                            mesh.name = string.Format("Mesh {0}", this.ID);

                            if (this.VertexCount == 0 || this.Vertex == IntPtr.Zero || this.IndexCount == 0 || this.Index == IntPtr.Zero)
                            {
                                return new BlockMesh()
                                {
                                    Result = this.Result,
                                    ID = this.ID,
                                    Level = this.Level,
                                    Flags = this.Flags,
                                    MeshData = mesh,
                                    Confidences = default
                                };
                            }

                            NativeArray<Vector3> vertices = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3>(this.Vertex.ToPointer(), (int)this.VertexCount, Allocator.None);

                            #if ENABLE_UNITY_COLLECTIONS_CHECKS
                            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref vertices, AtomicSafetyHandle.Create());
                            #endif

                            if (this.Flags.HasFlag(SettingsFlags.ComputeNormals) && this.Normal != IntPtr.Zero)
                            {
                                NativeArray<Vector3> normals = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3>(this.Normal.ToPointer(), (int)this.VertexCount, Allocator.None);

                                #if ENABLE_UNITY_COLLECTIONS_CHECKS
                                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref normals, AtomicSafetyHandle.Create());
                                #endif

                                NativeArray<VertexData> buffer = new NativeArray<VertexData>((int)this.VertexCount, Allocator.None);
                                for (int i = 0; i < this.VertexCount; i++)
                                {
                                    buffer[i] = new VertexData { Position = MLConvert.ToUnity(vertices[i]), Normal = MLConvert.ToUnity(normals[i]) };
                                }

                                var layout = new[]
                                {
                                    new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                                    new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
                                };

                                mesh.SetVertexBufferParams((int)this.VertexCount, layout);
                                mesh.SetVertexBufferData(buffer, 0, 0, (int)this.VertexCount);
                            }

                            else
                            {
                                for (int i = 0; i < vertices.Length; ++i)
                                {
                                    vertices[i] = MLConvert.ToUnity(vertices[i]);
                                }

                                var layout = new[]
                                {
                                    new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3)
                                };

                                mesh.SetVertexBufferParams((int)this.VertexCount, layout);
                                mesh.SetVertexBufferData(vertices, 0, 0, (int)this.VertexCount);
                            }

                            NativeArray<short> indices = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>(Index.ToPointer(), IndexCount, Allocator.None);

                            #if ENABLE_UNITY_COLLECTIONS_CHECKS
                            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref indices, AtomicSafetyHandle.Create());
                            #endif

                            mesh.SetIndexBufferParams(this.IndexCount, IndexFormat.UInt16);
                            mesh.SetIndexBufferData(indices, 0, 0, this.IndexCount);

                            mesh.SetSubMesh(0, new SubMeshDescriptor(0, this.IndexCount, this.Flags.HasFlag(SettingsFlags.PointCloud) ? MeshTopology.Points : MeshTopology.Triangles));

                            mesh.Optimize();
                            mesh.RecalculateBounds();

                            NativeArray<float> confidences;

                            if (this.Flags.HasFlag(SettingsFlags.ComputeConfidence) && this.Confidence != IntPtr.Zero)
                            {
                                confidences = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(Confidence.ToPointer(), (int)VertexCount, Allocator.Persistent);

                                #if ENABLE_UNITY_COLLECTIONS_CHECKS
                                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref confidences, AtomicSafetyHandle.Create());
                                #endif
                            }
                            else
                            {
                                confidences = new NativeArray<float>();
                            }

                            return new BlockMesh()
                            {
                                Result = this.Result,
                                ID = this.ID,
                                Level = this.Level,
                                Flags = this.Flags,
                                MeshData = mesh,
                                Confidences = confidences
                            };
                        }
                    }
                }
            }

            /// <summary>
            /// Helper struct used as layout when normals are requested.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct VertexData
            {
                /// <summary>
                /// Position data of vertex.
                /// </summary>
                public Vector3 Position;

                /// <summary>
                /// Normal data of vertex.
                /// </summary>
                public Vector3 Normal;

            }
        }
    }
    #endif
}