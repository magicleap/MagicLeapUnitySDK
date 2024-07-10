// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

// Disabling deprecated warning for the internal project
#pragma warning disable 618

using System;
using System.IO;
using System.Runtime.InteropServices;
using static UnityEngine.XR.MagicLeap.Native.MagicLeapNativeBindings;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLOcclusion : MLAutoAPISingleton<MLOcclusion>
    {

        NativeBindings.MLOcclusionMesh outMesh = NativeBindings.MLOcclusionMesh.Init();
        NativeBindings.MLOcclusionMeshQuery meshQuery = NativeBindings.MLOcclusionMeshQuery.Init();

        public struct Settings
        {
            /// <summary>
            /// Minimum distance in meters to occlude objects.
            /// Possible values ranges from 0.3m up to 7.5m.
            /// </summary>
            public float MinDistance;

            /// <summary>
            /// Maximum distance in meters to occlude objects.
            /// Possible values ranges from 0.3m up to 7.5m.
            /// </summary>
            public float MaxDistance;
        }

        public struct OcclusionMesh
        {
            /// <summary>
            /// The timestamp when data was generated.
            /// </summary>
            public MLTime Timestamp;

            /// <summary>
            /// The number of indices in index buffer.
            /// </summary>
            public uint IndexCount;

            /// <summary>
            /// The number of vertices in vertex buffer.
            /// </summary>
            public uint VertexCount;

            /// <summary>
            ///// Pointer to the vertex buffer.
            ///// All vertices are placed w.r.t.world origin.
            ///// </summary>
            public Vector3[] Vertex;

            ///// <summary>
            ///// Pointer to index buffer.
            ///// In the index buffer each value is the index of a vertex in the vertex buffer.
            ///// Three indices define one triangle. For example, the first triangle will have
            ///// the vertices: vertex[index[0]], vertex[index[1]], vertex[index[2]].
            ///// Index order is CW.
            ///// </summary>
            public int[] Index;
        }

        protected override MLResult.Code StartAPI() => Instance.InternalMLOcclusionStart();

        protected override MLResult.Code StopAPI() => Instance.InternalMLOcclusionStop();

        public static MLResult.Code UpdateSettings(in Settings settings) => Instance.InternalMLOcclusionUpdateSettings(in settings);

        public static MLResult.Code GetLatestMesh(out OcclusionMesh mesh) => Instance.InternalGetLatestMesh(out mesh);

        MLResult.Code InternalMLOcclusionStart()
        {
            if (!MLResult.DidNativeCallSucceed(MLPermissions.CheckPermission(MLPermission.SpatialMapping).Result))
            {
                MLPluginLog.Error($"{nameof(MLOcclusion)} requires missing permission {MLPermission.SpatialMapping}");
                return MLResult.Code.PermissionDenied;
            }

            NativeBindings.MLOcclusionSettings settings = NativeBindings.MLOcclusionSettings.Init();
            MLResult.Code result = NativeBindings.MLOcclusionCreateClient(settings, out Handle);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLOcclusionCreateClient));
            return result;
        }

        MLResult.Code InternalMLOcclusionStop()
        {
            MLResult.Code result = NativeBindings.MLOcclusionDestroyClient(Handle);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLOcclusionDestroyClient));
            return result;
        }

        MLResult.Code InternalMLOcclusionUpdateSettings(in Settings settings)
        {
            NativeBindings.MLOcclusionSettings internalSettings = NativeBindings.MLOcclusionSettings.Init();
            internalSettings.MaxDistance = settings.MaxDistance;
            internalSettings.MinDistance = settings.MinDistance;
            MLResult.Code result = NativeBindings.MLOcclusionUpdateSettings(Handle, internalSettings);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLOcclusionUpdateSettings));
            return result;
        }

        MLResult.Code InternalGetLatestMesh(out OcclusionMesh mesh)
        {
            mesh = new();
            MLResult.Code result = NativeBindings.MLOcclusionGetLatestMesh(Handle, in meshQuery, out outMesh);

            if (MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLOcclusionGetLatestMesh)))
            {

                MLTime.ConvertSystemTimeToMLTime((long)outMesh.Timestamp, out mesh.Timestamp);
                mesh.IndexCount = outMesh.IndexCount;
                mesh.VertexCount = outMesh.VertexCount;

                MarshalIndexArray(outMesh.Index, (int)outMesh.IndexCount, out mesh.Index);
                ReverseIndices(mesh.Index);

                MarshalVectorArray(outMesh.Vertex, (int)outMesh.VertexCount, out mesh.Vertex);

                result = NativeBindings.MLOcclusionReleaseMesh(Handle, out outMesh);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLOcclusionReleaseMesh));
            }

            return result;
        }

        /// <summary>
        /// Special vector marshalling used in place of array marshal for performance and to apply 
        /// extra z-negation operation (inverted rendering otherwise, works alongside ReverseIndices)
        /// </summary>
        /// <param name="indicesPtr"> Pointer to index buffer </param>
        /// <param name="count"> number of elements in index buffer</param>
        /// <param name="index"> location for marshalled index array</param>
        void MarshalVectorArray(IntPtr verticesPtr, int count, out Vector3[] vertex)
        {
            vertex = new Vector3[count];
            int totalSize = Marshal.SizeOf<MLVec3f>() * count;
            byte[] tempBuffer = new byte[totalSize];
            Marshal.Copy(verticesPtr, tempBuffer, 0, totalSize);

            using (MemoryStream memoryStream = new MemoryStream(tempBuffer))
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
                float x, y, z;
                for (int i = 0; i < count; ++i)
                {
                    x = reader.ReadSingle();
                    y = reader.ReadSingle();
                    z = reader.ReadSingle();
                    vertex[i] = new Vector3(x, y, -z);
                }
            }
        }
        /// <summary>
        /// Special index marshalling used in place of array marshal for performance
        /// </summary>
        /// <param name="indicesPtr"> Pointer to index buffer </param>
        /// <param name="count"> number of elements in index buffer</param>
        /// <param name="index"> location for marshalled index array</param>
        void MarshalIndexArray(IntPtr indicesPtr, int count, out int[] index)
        {
            index = new int[count];
            int tSize = Marshal.SizeOf<int>();

            byte[] tempBuffer = new byte[tSize * count];
            Marshal.Copy(indicesPtr, tempBuffer, 0, tSize * count);
            Buffer.BlockCopy(tempBuffer, 0, index, 0, tSize * count);
        }

        /// <summary>
        /// Reverses the indices passed from `MLOcclusionGetLatestMesh` from CW to CCW.
        /// It does so by swaping the outer indices (index[0] & index[2]) around.
        /// </summary>
        /// <param name="idx">Indices to reverse.</param>
        void ReverseIndices(int[] idx)
        {
            for (int i = 0; i < idx.Length; i += 3)
            {
                int temp = idx[i];
                idx[i] = idx[i + 2];
                idx[i + 2] = temp;
            }
        }
    }
}
