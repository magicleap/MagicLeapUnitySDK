// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLMeshing.cs" company="Magic Leap, Inc">
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
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Unity.Collections;

    #if PLATFORM_LUMIN
    using UnityEngine.XR.MagicLeap.Native;
    #endif

    /// <summary>
    /// MLMeshing class is the entry point for the Meshing API.
    /// </summary>
    public sealed partial class MLMeshing : MLAPISingleton<MLMeshing>
    {
        #if PLATFORM_LUMIN
        /// <summary>
        /// Delegate to handle mesh info request results
        /// </summary>
        /// <param name="meshInfo">Mesh information for requested query.</param>
        public delegate void MeshQueryResultsDelegate(ulong requestHandle, MeshRequestResult mesh);

        /// <summary>
        /// Stores the meshing client handle.
        /// </summary>
        private ulong meshingClientHandle = MagicLeapNativeBindings.InvalidHandle;

        /// <summary>
        /// Contains the current meshing client settings.
        /// </summary>
        private MeshSettings meshSettings;

        /// <summary>
        /// Stores the ongoing mesh info requests.
        /// </summary>
        private List<ulong> meshInfoRequests = new List<ulong>();

        /// <summary>
        /// Stores the finished mesh info requests result handles.
        /// </summary>
        private List<ulong> meshInfoRequestResultHandles = new List<ulong>();

        /// <summary>
        /// Stores the succesfully finished mesh info requests results.
        /// </summary>
        private Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, Tuple<BlockInfo, ulong>> meshInfoRequestsResults = new Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, Tuple<BlockInfo, ulong>>();

        /// <summary>
        /// Stores the ongoing mesh blocks requests.
        /// </summary>
        private Dictionary<ulong, MeshQueryResultsDelegate> meshRequests = new Dictionary<ulong, MeshQueryResultsDelegate>();

        /// <summary>
        /// Stores the succesfully finished mesh blocks requests results.
        /// </summary>
        private Dictionary<ulong, MeshRequestResult> meshRequestsResults = new Dictionary<ulong, MeshRequestResult>();
        #endif

        /// <summary>
        /// Request flags for the meshing system.
        /// </summary>
        [Flags]
        public enum SettingsFlags
        {
            /// <summary>
            /// If set, will return a point cloud instead of a triangle mesh.
            /// </summary>
            PointCloud = 1 << 0,

            /// <summary>
            /// If set, the system will compute the normals for the triangle vertices.
            /// </summary>
            ComputeNormals = 1 << 1,

            /// <summary>
            /// If set, the system will compute the confidence values.
            /// </summary>
            ComputeConfidence = 1 << 2,

            /// <summary>
            /// If set, the system will planarize the returned mesh (planar regions will be smoothed out).
            /// </summary>
            Planarize = 1 << 3,

            /// <summary>
            /// If set, the mesh skirt (overlapping area between two mesh blocks) will be removed.
            /// </summary>
            RemoveMeshSkirt = 1 << 4,

            /// <summary>
            /// If set, winding order of indices will be be changed from counter clockwise to clockwise.
            /// </summary>
            IndexOrderCW = 1 << 5,
        }

        /// <summary>
        /// Level of detail of the block mesh.
        /// </summary>
        public enum LOD
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
            Maximum,
        }

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
        /// Result of a mesh request.
        /// </summary>
        public enum MeshResult
        {
            /// <summary>
            /// Mesh request has succeeded.
            /// </summary>
            Success,

            /// <summary>
            /// Mesh request has failed.
            /// </summary>
            Failed,

            /// <summary>
            /// Mesh request is pending.
            /// </summary>
            Pending,

            /// <summary>
            /// There are partial updates on the mesh request.
            /// </summary>
            PartialUpdate,
        }

        #if PLATFORM_LUMIN
        /// <summary>
        /// Maximum time (in microseconds) to allow requests results.
        /// </summary>
        public static ulong MaximumRequestTime = 1000;

        /// <summary>
        /// Gets or sets the current meshing client settings.
        /// </summary>
        public static MeshSettings CurrentMeshSettings
        {
            get
            {
                if (!IsValidInstance())
                {
                    MLPluginLog.ErrorFormat("MLMeshing.CurrentMeshSettings.get failed. Reason: No instance for MLMeshing.");
                    return new MeshSettings();
                }

                if (!NativeBindings.MLHandleIsValid(_instance.meshingClientHandle))
                {
                    MLPluginLog.ErrorFormat("MLMeshing.CurrentMeshSettings.get failed. Reason: Meshing client handle is invalid");
                    return new MeshSettings();
                }

                return _instance.meshSettings;
            }
            set
            {
                if (!IsValidInstance())
                {
                    MLPluginLog.ErrorFormat("MLMeshing.CurrentMeshSettings.set failed. Reason: No instance for MLMeshing.");
                    return;
                }

                if (!NativeBindings.MLHandleIsValid(_instance.meshingClientHandle))
                {
                    MLPluginLog.ErrorFormat("MLMeshing.CurrentMeshSettings.set failed. Reason: Meshing client handle is invalid");
                    return;
                }

                MLResult.Code resultCode = NativeBindings.MLMeshingUpdateSettings(_instance.meshingClientHandle, in value);
                if (resultCode != MLResult.Code.Ok)
                {
                    MLPluginLog.ErrorFormat("MLMeshing.CurrentMeshSettings.set failed to update the current settings. Reason: {}", MLResult.CodeToString(resultCode));
                    return;
                }

                _instance.meshSettings = value;
            }
        }

        /// <summary>
        /// Starts the Meshing API.
        /// </summary>
        /// <param name="settings">The settings to start the meshing client with.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed in.
        /// MLResult.Result will be<c> MLResult.Code.PrivilegeDenied</c> if there was a lack of privileges.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        public static MLResult Start(MeshSettings? settings = null)
        {
            if (settings == null)
            {
                settings = MeshSettings.Create();
            }

            CreateInstance(settings.Value);

            return MLMeshing.BaseStart(true);
        }

        /// <summary>
        /// Generates a new Mesh Info request.
        /// </summary>
        /// <param name="extents">Mesh extents to use for request.</param>
        /// <param name="callBack">Callback to trigger when request finishes.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed in.
        /// </returns>
        public static MLResult RequestMeshInfo(in MeshExtents extents)
        {
            if (!IsValidInstance())
            {
                MLPluginLog.Error("MLMeshing.RequestMeshInfo failed. Reason: No instance for MLMeshing.");
                return MLResult.Create(MLResult.Code.UnspecifiedFailure);
            }

            if (!MagicLeapNativeBindings.MLHandleIsValid(_instance.meshingClientHandle))
            {
                MLPluginLog.Error("MLMeshing.RequestMeshInfo failed. Reason: Meshing client handle is invalid.");
                return MLResult.Create(MLResult.Code.UnspecifiedFailure);
            }

            NativeBindings.MeshExtentsNative nativeExtents = new NativeBindings.MeshExtentsNative
            {
                Data = extents
            };

            MLResult.Code resultCode = NativeBindings.MLMeshingRequestMeshInfo(_instance.meshingClientHandle, in nativeExtents, out ulong requestHandle);
            if(resultCode != MLResult.Code.Ok)
            {
                MLPluginLog.ErrorFormat("MLMeshing.RequestMeshInfo failed to generate a new request. Reason: {0}", MLResult.CodeToString(resultCode));
                return MLResult.Create(resultCode);
            }

            _instance.meshInfoRequests.Add(requestHandle);

            return MLResult.Create(MLResult.Code.Ok);
        }

        /// <summary>
        /// Populates input dictionary with latest mesh info request results and erases local dictionary.
        /// </summary>
        /// <param name="results">Results dictionary to populate.</param>
        public static void GetMeshInfoResults(out Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, Tuple<BlockInfo, ulong>> results)
        {
            if (!IsValidInstance())
            {
                MLPluginLog.Error("MLMeshing.GetMeshInfoResults failed. Reason: No instance for MLMeshing.");
                results = new Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, Tuple<BlockInfo, ulong>>();
                return;
            }

            if (!MagicLeapNativeBindings.MLHandleIsValid(_instance.meshingClientHandle))
            {
                MLPluginLog.Error("MLMeshing.GetMeshInfoResults failed. Reason: Meshing client handle is invalid.");
                results = new Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, Tuple<BlockInfo, ulong>>();
                return;
            }

            results = new Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, Tuple<BlockInfo, ulong>>(_instance.meshInfoRequestsResults);
            _instance.meshInfoRequestsResults.Clear();
        }

        /// <summary>
        /// Generates a new Mesh request.
        /// </summary>
        /// <param name="meshRequests">Mesh blocks to request.</param>
        /// <param name="callBack">Callback to trigger when request finishes.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed in.
        /// </returns>
        public static MLResult RequestMesh(in List<MeshRequest> meshRequests, out ulong requestHandle, MeshQueryResultsDelegate callBack)
        {
            if (!IsValidInstance())
            {
                MLPluginLog.Error("MLMeshing.RequestMesh failed. Reason: No instance for MLMeshing.");
                requestHandle = MagicLeapNativeBindings.InvalidHandle;
                return MLResult.Create(MLResult.Code.UnspecifiedFailure);
            }

            if (!MagicLeapNativeBindings.MLHandleIsValid(_instance.meshingClientHandle))
            {
                MLPluginLog.Error("MLMeshing.RequestMesh failed. Reason: Meshing client handle is invalid.");
                requestHandle = MagicLeapNativeBindings.InvalidHandle;
                return MLResult.Create(MLResult.Code.UnspecifiedFailure);
            }

            NativeBindings.MeshRequestNative request = NativeBindings.MeshRequestNative.Create(in meshRequests);

            MLResult.Code resultCode = NativeBindings.MLMeshingRequestMesh(_instance.meshingClientHandle, in request, out requestHandle);
            if(resultCode != MLResult.Code.Ok)
            {
                MLPluginLog.ErrorFormat("MLMeshing.RequestMesh failed to generate a new request. Reason: {0}", MLResult.CodeToString(resultCode));
                return MLResult.Create(resultCode);
            }

            _instance.meshRequests.Add(requestHandle, callBack);

            return MLResult.Create(MLResult.Code.Ok);
        }

        /// <summary>
        /// Create the meshing client.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed in.
        /// MLResult.Result will be<c> MLResult.Code.PrivilegeDenied</c> if there was a lack of privileges.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        protected override MLResult StartAPI()
        {
            MLResult.Code resultCode = NativeBindings.MLMeshingCreateClient(out meshingClientHandle, in meshSettings);

            if (resultCode != MLResult.Code.Ok)
            {
                MLPluginLog.ErrorFormat("MLMeshing.StartAPI failed to create meshing client. Reason: {0}", MLResult.CodeToString(resultCode));
                return MLResult.Create(resultCode);
            }

            if (!MagicLeapNativeBindings.MLHandleIsValid(meshingClientHandle))
            {
                MLPluginLog.Error("MLMeshing.StartAPI failed. Reason: Invalid handle returned when initializing an instance of MLMeshing.");
                return MLResult.Create(MLResult.Code.UnspecifiedFailure);
            }

            return MLResult.Create(MLResult.Code.Ok);
        }

        /// <summary>
        /// Destroys the meshing client.
        /// </summary>
        /// <param name="isSafeToAccessManagedObjects">Boolean that tells if it is safe to clear managed memory</param>
        protected override void CleanupAPI(bool isSafeToAccessManagedObjects)
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(meshingClientHandle))
            {
                return;
            }

            MLResult.Code resultCode = NativeBindings.MLMeshingDestroyClient(in meshingClientHandle);
            if (resultCode != MLResult.Code.Ok)
            {
                MLPluginLog.ErrorFormat("MLMeshing.CleanupAPI failed to destroy the meshing client. Reason: {0}", MLResult.CodeToString(resultCode));
                return;
            }
        }

        /// <summary>
        /// Handles processing of both mesh info and mesh block querying.
        /// </summary>
        protected override void Update()
        {
            MLResult.Code resultCode;

            // Process Mesh Info requests.
            if (meshInfoRequests.Count > 0)
            {
                foreach (ulong requestHandle in meshInfoRequests)
                {
                    resultCode = NativeBindings.MLMeshingGetMeshInfoResult(meshingClientHandle, requestHandle, out NativeBindings.MeshInfoNative info);
                    if (resultCode != MLResult.Code.Pending)
                    {
                        if (resultCode != MLResult.Code.Ok)
                        {
                            meshInfoRequestResultHandles.Add(requestHandle);
                            MLPluginLog.ErrorFormat("MLMeshing.Update failed to get mesh info results. Reason: {0}", MLResult.CodeToString(resultCode));
                            continue;
                        }

                        foreach(BlockInfo blockInfo in info.Data)
                        {
                            if((!meshInfoRequestsResults.ContainsKey(blockInfo.ID) ||
                                (meshInfoRequestsResults.ContainsKey(blockInfo.ID) && meshInfoRequestsResults[blockInfo.ID].Item1.Timestamp < blockInfo.Timestamp)))
                            {
                                if(!meshInfoRequestsResults.ContainsKey(blockInfo.ID))
                                {
                                    meshInfoRequestsResults.Add(blockInfo.ID, new Tuple<BlockInfo, ulong>(blockInfo, info.Timestamp));
                                }
                                else
                                {
                                    meshInfoRequestsResults[blockInfo.ID] = new Tuple<BlockInfo, ulong>(blockInfo, info.Timestamp);
                                }
                            }
                        }

                        meshInfoRequestResultHandles.Add(requestHandle);

                        resultCode = NativeBindings.MLMeshingFreeResource(meshingClientHandle, in requestHandle);
                        if (resultCode != MLResult.Code.Ok)
                        {
                            MLPluginLog.ErrorFormat("MLMeshing.Update failed to release mesh info results resource. Reason: {0}", MLResult.CodeToString(resultCode));
                            continue;
                        }
                    }
                }

                for (int i = 0; i < meshInfoRequestResultHandles.Count; ++i)
                {
                    meshInfoRequests.Remove(meshInfoRequestResultHandles[i]);
                }

                meshInfoRequestResultHandles.Clear();
            }

            // Process Mesh requests
            if (meshRequests.Count > 0)
            {
                foreach (KeyValuePair<ulong, MeshQueryResultsDelegate> pair in meshRequests)
                {
                    resultCode = NativeBindings.MLMeshingGetMeshResult(meshingClientHandle, pair.Key, out NativeBindings.MeshRequestResultNative meshResults);
                    if (resultCode != MLResult.Code.Pending)
                    {
                        ulong requestHandle = pair.Key;

                        meshRequestsResults.Add(requestHandle, meshResults.Data);

                        if (resultCode != MLResult.Code.Ok)
                        {
                            MLPluginLog.ErrorFormat("MLMeshing.Update failed to get mesh results. Reason: {0}", MLResult.CodeToString(resultCode));
                        }
                        else
                        {
                            resultCode = NativeBindings.MLMeshingFreeResource(meshingClientHandle, in requestHandle);
                            if (resultCode != MLResult.Code.Ok)
                            {
                                MLPluginLog.ErrorFormat("MLMeshing.Update failed to release mesh results resource. Reason: {0}", MLResult.CodeToString(resultCode));
                                continue;
                            }
                        }
                    }
                }

                foreach (KeyValuePair<ulong, MeshRequestResult> pair in meshRequestsResults)
                {
                    meshRequests[pair.Key].Invoke(pair.Key, pair.Value);
                    meshRequests.Remove(pair.Key);
                }

                meshRequestsResults.Clear();
            }
        }

        /// <summary>
        /// Constructor. Initializes mesh settings.
        /// </summary>
        /// <param name="settings">New settings.</param>
        private MLMeshing(MeshSettings settings)
        {
            this.meshSettings = settings;
        }

        /// <summary>
        /// Creates an instance of the Meshing API
        /// </summary>
        /// <param name="settings">The settings to start the meshing client with.</param>
        private static void CreateInstance(MeshSettings settings)
        {
            if (!MLMeshing.IsValidInstance())
            {
                MLMeshing._instance = new MLMeshing(settings);
            }
        }

        /// <summary>
        /// Mesh Settings for the underlying system.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MeshSettings
        {
            /// <summary>
            /// Request flags that are a combination of MLMeshingFlags.
            /// </summary>
            public SettingsFlags Flags;

            /// <summary>
            /// Perimeter (in meters) of holes you wish to have filled.
            /// </summary>
            public float FillHoleLength;

            /// <summary>
            /// Any component that is disconnected from the main mesh and
            /// which has an area(in meters^2) less than this size will be removed.
            /// </summary>
            public float DisconnectedComponentArea;

            /// <summary>
            /// Creates a new mesh settings instance with the system level settings values.
            /// </summary>
            /// <returns>The new mesh settings instance</returns>
            public static MeshSettings Create()
            {
                MLResult.Code resultCode = NativeBindings.MLMeshingInitSettings(out MeshSettings settings);

                if(resultCode != MLResult.Code.Ok)
                {
                    MLPluginLog.ErrorFormat("MLMeshing.MLSettings.Create failed to initialize new settings. Reason: {0}", MLResult.CodeToString(resultCode));
                    return new MeshSettings();
                }

                settings.Flags |= MLMeshing.SettingsFlags.IndexOrderCW;
                return settings;
            }
        }

        /// <summary>
        /// Axis aligned bounding box for querying updated mesh info.
        /// </summary>
        public struct MeshExtents
        {
            /// <summary>
            /// The center of the bounding box.
            /// </summary>
            public Vector3 Center;

            /// <summary>
            /// The rotation of the bounding box.
            /// </summary>
            public Quaternion Rotation;

            /// <summary>
            /// The size of the bounding box. When a mesh request is complete
            /// all mesh blocks that intersect with the extents provided here
            /// will be returned in MLMeshingMeshInfo.
            /// </summary>
            public Vector3 Extents;
        }

        /// <summary>
        /// Representation of a mesh block.
        /// </summary>
        public struct BlockInfo : IComparable<BlockInfo>
        {
            /// <summary>
            /// The coordinate frame UID to represent the block.
            /// </summary>
            public MagicLeapNativeBindings.MLCoordinateFrameUID ID;

            /// <summary>
            /// The extents of the bounding box.
            /// </summary>
            public MeshExtents Extents;

            /// <summary>
            /// The timestamp (in microseconds) when block was updated.
            /// </summary>
            public ulong Timestamp;

            /// <summary>
            /// The state of the Mesh Block.
            /// </summary>
            public MeshState State;

            /// <summary>
            /// Used for storing into Priority Queue.
            /// </summary>
            /// <param name="other">Other BlockInfo to compare against.</param>
            /// <returns>Result of comparison.</returns>
            public int CompareTo(BlockInfo other)
            {
                return this.Timestamp.CompareTo(other.Timestamp);
            }
        }

        /// <summary>
        /// Request structure to get the actual mesh for a block.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MeshRequest
        {
            /// <summary>
            /// The UID to represent the block.
            /// </summary>
            public MagicLeapNativeBindings.MLCoordinateFrameUID ID;

            /// <summary>
            /// The LOD level to request.
            /// </summary>
            public LOD Level;
        }

        /// <summary>
        /// Result of a Mesh request. See MLMeshingGetMeshResult().
        /// </summary>
        public struct MeshRequestResult
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
            /// The mesh data.
            /// </summary>
            public List<BlockMesh> Meshes;
        }

        /// <summary>
        /// Final structure for a block mesh.
        /// </summary>
        public struct BlockMesh
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
            /// Unity Mesh component storing the mesh data
            /// </summary>
            public Mesh MeshData;

            /// <summary>
            /// Confidence buffer.
            /// </summary>
            public NativeArray<float> Confidences;
        }
        #endif
    }
}
