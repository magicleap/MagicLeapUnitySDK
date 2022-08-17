// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLHandMeshing.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if UNITY_MAGICLEAP || UNITY_ANDROID

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Threading.Tasks;
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// The MLHandMeshing API is used to request for the mesh information of the hands.
    /// </summary>
    public sealed partial class MLHandMeshing : MLAutoAPISingleton<MLHandMeshing>
    {
        public static Request RequestHandMesh(Request.Params requestParams) => new Request(requestParams);

        /// <summary>
        /// Structure to represent a hand mesh.
        /// </summary>
        public struct Mesh
        {
            /// <summary>
            /// Gets the array of mesh blocks.
            /// </summary>
            public Block[] MeshBlock { get; internal set; }

            /// <summary>
            /// Structure to represent a mesh block
            /// </summary>
            public struct Block
            {
                /// <summary>
                /// Gets the array of vertex positions relative to the origin.
                /// This can be directly plugged into Mesh.vertices.
                /// </summary>
                public Vector3[] Vertex { get; internal set; }

                /// <summary>
                /// Gets the array of Indices. Guaranteed to be a multiple of 3. Every 3 indices creates a triangles.
                /// This can be directly plugged into Mesh.triangles.
                /// </summary>
                public int[] Index { get; internal set; }
            }
        }

        public partial class Request : MLRequest<Request.Params, Request.Result>
        {
            public Request(Params parameters)
            {
                this.parameters.tracker = Instance.nativeTracker;
                Start(parameters);
            }

            ~Request() => Dispose(true);

            public override MLResult Start(Params parameters)
            {
                Dispose(true);
                this.parameters = parameters;
                return MLResult.Create(MLHandMeshing.Instance.RequestMesh(this.parameters.tracker, ref this.handle));
            }

            public override MLResult TryGetResult(out Result result)
            {
                if (!MagicLeapNativeBindings.MLHandleIsValid(this.handle))
                {
                    MLResult startResult = this.Start(this.parameters);
                    if (!startResult.IsOk)
                    {
                        result = default;
                        return startResult;
                    }
                }

                NativeBindings.MeshNative meshNative = new NativeBindings.MeshNative();
                MLResult.Code resultCode = MLHandMeshing.Instance.GetResult(this.parameters.tracker, this.handle, ref meshNative);
                if (resultCode != MLResult.Code.Pending)
                {
                    MLResult.DidNativeCallSucceed(resultCode, "HandMeshingRequest.TryGetResult(out Result result)");
                }

                MLResult mlResult = MLResult.Create(resultCode);

                if (mlResult.IsOk)
                {
                    result = new Result(meshNative);
                    this.Dispose(true);
                }
                else
                {
                    result = default;
                }

                return mlResult;
            }

            protected override void Dispose(bool disposing)
            {
                if (MagicLeapNativeBindings.MLHandleIsValid(this.handle))
                {
                    MLResult.DidNativeCallSucceed(MLHandMeshing.Instance.FreeResource(this.parameters.tracker, ref this.handle), "HandMeshingRequest.Dispose(bool disposing)");
                    this.handle = MagicLeapNativeBindings.InvalidHandle;
                }
            }
        }

        public partial class Request
        {
            public struct Params
            {
                internal ulong tracker;
            }
        }

        public partial class Request
        {
            public struct Result
            {
                internal Result(NativeBindings.MeshNative meshNative)
                {
                    Mesh = meshNative.Data;
                }

                public Mesh Mesh;
            }
        }
    }
}

#endif
