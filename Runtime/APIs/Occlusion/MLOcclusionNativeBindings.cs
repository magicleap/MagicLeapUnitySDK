// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2023) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    public partial class MLOcclusion
    {
        /// <summary>
        /// See ml_occlusion.h for additional comments.
        /// </summary>
        private class NativeBindings : Native.MagicLeapNativeBindings
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct MLOcclusionSettings
            {
                /// <summary>
                /// Struct version.
                /// </summary>
                public uint Version;

                /// <summary>
                /// Minimum distance in meters to occlude objects.
                /// Possible values ranges from 0.3m up to 7.5m.
                /// </summary>
                public float MinDistance;

                /// <summary>
                /// Maximum distance in meters to occlude objects.
                //  Possible values ranges from 0.3m up to 7.5m.
                /// </summary>
                public float MaxDistance;
                public static MLOcclusionSettings Init(uint version = 1)
                {
                    return new MLOcclusionSettings
                    {
                        Version = version,
                        MaxDistance = 5f,
                        MinDistance = 0.3f,
                    };
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MLOcclusionMesh
            {
                /// <summary>
                /// Struct version.
                /// </summary>
                public uint Version;

                /// <summary>
                /// The timestamp when data was generated.
                /// </summary>
                public ulong Timestamp;

                /// <summary>
                /// The number of indices in index buffer.
                /// </summary>
                public uint IndexCount;

                /// <summary>
                /// The number of vertices in vertex buffer.
                /// </summary>
                public uint VertexCount;

                /// <summary>
                /// Pointer to the vertex buffer.
                /// All vertices are placed w.r.t.world origin.
                /// </summary>
                public IntPtr Vertex;

                /// <summary>
                /// Pointer to index buffer.
                /// In the index buffer each value is the index of a vertex in the vertex buffer.
                /// Three indices define one triangle.For example, the first triangle
                /// will have the vertices: vertex[index[0]], vertex[index[1]], vertex[index[2]].
                /// Index order is CW.
                /// </summary>
                public IntPtr Index;

                public static MLOcclusionMesh Init(uint version = 1)
                {
                    return new MLOcclusionMesh
                    {
                        Version = version,
                        Vertex = IntPtr.Zero,
                        Index = IntPtr.Zero,
                    };
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MLOcclusionMeshQuery
            {
                public uint Version;
                public static MLOcclusionMeshQuery Init(uint version = 1)
                {
                    return new MLOcclusionMeshQuery
                    {
                        Version = version,
                    };
                }
            }

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLOcclusionCreateClient(in MLOcclusionSettings settings, out ulong handle);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLOcclusionDestroyClient(ulong handle);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLOcclusionUpdateSettings(ulong handle, in MLOcclusionSettings settings);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLOcclusionGetLatestMesh(ulong handle, in MLOcclusionMeshQuery mesh_query, out MLOcclusionMesh out_mes_data);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLOcclusionReleaseMesh(ulong handle, out MLOcclusionMesh out_mes_data);
        }
    }
}