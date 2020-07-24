// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Developer Agreement, located
// here: https://auth.magicleap.com/terms/developer
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

#if PLATFORM_LUMIN
using UnityEngine.XR.MagicLeap.Native;
#endif

namespace MagicLeap.Core
{
    /// <summary>
    /// Core monobehavior for the MLMeshing API. Handles API initialization and shut down,
    /// as well as mesh querying and generation.
    /// </summary>
    [AddComponentMenu("XR/MagicLeap/MLMeshingBehavior")]
    public class MLMeshingBehavior : MonoBehaviour
    {
        /// <summary>
        /// The type of mesh that should be rendered.
        /// </summary>
        public enum MeshingType
        {
            Triangles,
            PointCloud
        }

        /// <summary>
        /// Delegate to handle when a mesh is created or updated.
        /// </summary>
        /// <param name="meshObject">The mesh.</param>
        public delegate void MeshNewOrUpdatedDelegate(MeshObject meshObject);

        #if PLATFORM_LUMIN
        /// <summary>
        /// Delegate to handle when a mesh is destroyed.
        /// </summary>
        /// <param name="meshID">The ID of the mesh destroyed.</param>
        public delegate void MeshRemovedDelegate(MagicLeapNativeBindings.MLCoordinateFrameUID meshID);
        #endif

        #pragma warning disable 0067
        /// <summary>
        /// Event for when a mesh block is added.
        /// </summary>
        public event MeshNewOrUpdatedDelegate OnMeshAdded;

        /// <summary>
        /// Event for when a mesh block is updated.
        /// </summary>
        public event MeshNewOrUpdatedDelegate OnMeshUpdated;
        #pragma warning restore 0067

        #if PLATFORM_LUMIN
        /// <summary>
        /// Event for when a mesh block is removed.
        /// </summary>
        public event MeshRemovedDelegate OnMeshRemoved;
        #endif

        /// <summary>
        /// The Transform that will serve as the parent of all mesh objects.
        /// </summary>
        public Transform MeshParent = null;

        /// <summary>
        /// Prefab to use for mesh instantiation.
        /// </summary>
        public GameObject MeshPrefab = null;

        /// <summary>
        /// Material to use for Triangle mesh type. Only required if MeshType is set to Triangles.
        /// </summary>
        public Material TriangleMeshMaterial = null;

        /// <summary>
        /// Material to use for PointCloud mesh type. Only required if MeshType is set to PointCloud.
        /// </summary>
        public Material PointCloudMeshMaterial = null;

        /// <summary>
        /// How often to check for updates, in seconds. More frequent updates will increase CPU usage.
        /// </summary>
        public float PollingRate = DefaultSettings.PollingRate; // Make sure at least 0

        /// <summary>
        /// Controls the number of mesh requests ongoing at a time. Larger numbers will lead to higher CPU usage.
        /// </summary>
        public uint MeshQueueSize = DefaultSettings.MeshQueueSize; // Make sure more than 1

        /// <summary>
        /// Controls the maximum number of blocks to query in a same mesh request. Larger numbers will lead to higher CPU usage.
        /// </summary>
        public uint MeshBatchSize = DefaultSettings.MeshBatchSize; // Make sure more than 1

        [SerializeField]
        private MLMeshing.LOD levelOfDetail = DefaultSettings.LevelOfDetail;

        /// <summary>
        /// Level of detail meshes will be requested and generated with.
        /// </summary>
        public MLMeshing.LOD LevelOfDetail
        {
            get
            {
                return levelOfDetail;
            }
            set
            {
                if (levelOfDetail != value)
                {
                    levelOfDetail = value;
                    CleanMeshes();
                }
            }
        }

        /// <summary>
        /// Used to determine if custom or default settings should be used.
        /// </summary>
        public bool UseDefaultSettings = true;

        [SerializeField]
        private MeshingType meshType = DefaultSettings.MeshType;

        /// <summary>
        /// Gets or sets the current meshing type.
        /// </summary>
        public MeshingType MeshType
        {
            get
            {
                return meshType;
            }
            set
            {
                if (meshType != value)
                {
                    if ((value == MeshingType.PointCloud && PointCloudMeshMaterial == null) || (value == MeshingType.Triangles && TriangleMeshMaterial == null))
                    {
                        Debug.LogErrorFormat("Error: MLMeshingBehavior failed to set MeshType, disabling script. Reason: Material member required for mesh type {0} is null.", value.ToString());
                        enabled = false;
                        return;
                    }

                    meshType = value;
                    settingsDirty = true;
                }
            }
        }

        [SerializeField]
        private bool computeNormals = DefaultSettings.ComputeNormals;

        /// <summary>
        /// Gets or sets if normals should be computed.
        /// </summary>
        public bool ComputeNormals
        {
            get
            {
                return computeNormals;
            }
            set
            {
                if (computeNormals != value)
                {
                    computeNormals = value;
                    settingsDirty = true;
                }
            }
        }

        [SerializeField]
        private bool requestVertexConfidence = DefaultSettings.RequestVertexConfidence;

        /// <summary>
        /// Gets or sets if vertex confidences values should be computed.
        /// </summary>
        public bool RequestVertexConfidence
        {
            get
            {
                return requestVertexConfidence;
            }
            set
            {
                if (requestVertexConfidence != value)
                {
                    requestVertexConfidence = value;
                    settingsDirty = true;
                }
            }
        }

        [SerializeField]
        private bool planarize = DefaultSettings.Planarize;

        /// <summary>
        /// Gets or sets if mesh should be planarized.
        /// </summary>
        public bool Planarize
        {
            get
            {
                return planarize;
            }
            set
            {
                if (planarize != value)
                {
                    planarize = value;
                    settingsDirty = true;
                }
            }
        }

        [SerializeField]
        private bool removeMeshSkirt = DefaultSettings.RemoveMeshSkirt;

        /// <summary>
        /// Gets or sets if the mesh skirt (overlapping area between two mesh blocks) will be removed.
        /// </summary>
        public bool RemoveMeshSkirt
        {
            get
            {
                return removeMeshSkirt;
            }
            set
            {
                if (removeMeshSkirt != value)
                {
                    removeMeshSkirt = value;
                    settingsDirty = true;
                }
            }
        }

        [SerializeField]
        private float fillHoleLength = DefaultSettings.FillHoleLength;

        /// <summary>
        /// Gets or sets the boundary distance (in meters) of holes you wish to have filled.
        /// </summary>
        public float FillHoleLength
        {
            get
            {
                return fillHoleLength;
            }
            set
            {
                if (fillHoleLength != value)
                {
                    fillHoleLength = value;
                    settingsDirty = true;
                }
            }
        }

        [SerializeField]
        private float disconnectedComponentArea = DefaultSettings.DisconnectedComponentArea;

        /// <summary>
        /// Gets or sets the minimum value that a mesh must have in order to not be removed.
        /// </summary>
        public float DisconnectedComponentArea
        {
            get
            {
                return disconnectedComponentArea;
            }
            set
            {
                if (disconnectedComponentArea != value)
                {
                    disconnectedComponentArea = value;
                    settingsDirty = true;
                }
            }
        }

        /// <summary>
        /// Indicates if settings have changed.
        /// </summary>
        private bool settingsDirty = false;

        /// <summary>
        /// Time from last update. Used to determine if update should occur again.
        /// </summary>
        private DateTime lastUpdateTime = DateTime.MinValue;

        #if PLATFORM_LUMIN
        /// <summary>
        /// Current mesh extents.
        /// </summary>
        private MLMeshing.MeshExtents meshExtents = new MLMeshing.MeshExtents();

        /// <summary>
        /// Priority queue containing the meshes that still need to be generated.
        /// </summary>
        private PriorityQueue<MLMeshing.BlockInfo> meshesToGenerate = new PriorityQueue<MLMeshing.BlockInfo>();

        /// <summary>
        /// IDs of the meshes that still need generation.
        /// </summary>
        private HashSet<MagicLeapNativeBindings.MLCoordinateFrameUID> meshesToGenerateIDs = new HashSet<MagicLeapNativeBindings.MLCoordinateFrameUID>();

        /// <summary>
        /// Meshes currently being generated.
        /// </summary>
        private Dictionary<ulong, Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, MLMeshing.MeshState>> meshesBeingGenerated = new Dictionary<ulong, Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, MLMeshing.MeshState>>();

        /// <summary>
        /// Contains all generated active meshes.
        /// </summary>
        private Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, MeshObject> meshObjMap = new Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, MeshObject>();
        #endif

        /// <summary>
        /// Verify required members and start the MLMeshing API.
        /// </summary>
        void Start()
        {
            if (MeshParent == null)
            {
                Debug.LogError("Error: MLMeshingBehavior.MeshParent is not set, disabling script.");
                enabled = false;
                return;
            }

            if (MeshPrefab == null)
            {
                Debug.LogError("Error: MLMeshingBehavior.MeshPrefab is not set, disabling script.");
                enabled = false;
                return;
            }

            if ((MeshType == MeshingType.PointCloud && PointCloudMeshMaterial == null) || (MeshType == MeshingType.Triangles && TriangleMeshMaterial == null))
            {
                Debug.LogErrorFormat("Error: MLMeshingBehavior failed because material member required for mesh type {0} is null, disbling script.", MeshType.ToString());
                enabled = false;
                return;
            }

            #if PLATFORM_LUMIN
            MLMeshing.MeshSettings initSettings;

            if (UseDefaultSettings)
            {
                initSettings = MLMeshing.MeshSettings.Create();
                SetSettings(ref initSettings);
            }
            else
            {
                initSettings = GetSettings();
            }

            MLResult result = MLMeshing.Start(initSettings);
            if(!result.IsOk)
            {
                Debug.LogErrorFormat("Error: MLMeshingBehavior failed starting MLMeshing, disabling script. Reason: {0}", result);
                enabled = false;
                return;
            }

            #if UNITY_EDITOR
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += StopAPIBeforeAssemblyReload;
            #endif

            #endif
        }

        /// <summary>
        /// Handle settings and bounds changes and process queries.
        /// </summary>
        void Update()
        {
            if(MLMeshing.IsStarted)
            {
                if (settingsDirty)
                {
                    UpdateSettings();
                }

                if (transform.hasChanged)
                {
                    UpdateMeshExtents();
                    transform.hasChanged = false;
                }

                if (((float)(DateTime.Now - lastUpdateTime).TotalSeconds > PollingRate))
                {
                    ProcessMeshInfoRequests();
                    lastUpdateTime = DateTime.Now;
                }

                ProcessMeshRequests();
            }
        }

        /// <summary>
        /// Stop the MLMeshing API.
        /// </summary>
        void OnDestroy()
        {
            #if PLATFORM_LUMIN
            if (MLMeshing.IsStarted)
            {
                MLMeshing.Stop();
            }
            #if UNITY_EDITOR
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload -= StopAPIBeforeAssemblyReload;
            #endif

            #endif
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Stop the MLMeshinddg API before live code changes are compiled.
        /// </summary>
        void StopAPIBeforeAssemblyReload()
        {
            if (MLMeshing.IsStarted)
            {
                MLMeshing.Stop();
            }
        }
        #endif

        #if PLATFORM_LUMIN
        /// <summary>
        /// Get the current MeshSettings.
        /// </summary>
        /// <returns>The current MeshSettings.</returns>
        private MLMeshing.MeshSettings GetSettings()
        {
            MLMeshing.SettingsFlags flags = MLMeshing.SettingsFlags.IndexOrderCW;

            if (meshType == MeshingType.PointCloud)
                flags |= MLMeshing.SettingsFlags.PointCloud;
            if (computeNormals)
                flags |= MLMeshing.SettingsFlags.ComputeNormals;
            if (requestVertexConfidence)
                flags |= MLMeshing.SettingsFlags.ComputeConfidence;
            if (planarize)
                flags |= MLMeshing.SettingsFlags.Planarize;
            if (removeMeshSkirt)
                flags |= MLMeshing.SettingsFlags.RemoveMeshSkirt;

            return new MLMeshing.MeshSettings()
            {
                Flags = flags,
                FillHoleLength = fillHoleLength,
                DisconnectedComponentArea = disconnectedComponentArea
            };
        }

        /// <summary>
        /// Sets the settings to the given value.
        /// </summary>
        /// <param name="settings">The settings to set values to.</param>
        private void SetSettings(ref MLMeshing.MeshSettings settings)
        {
            meshType = ((settings.Flags & MLMeshing.SettingsFlags.PointCloud) != 0) ? MeshingType.PointCloud : MeshingType.Triangles;
            computeNormals = ((settings.Flags & MLMeshing.SettingsFlags.ComputeNormals) != 0);
            requestVertexConfidence = ((settings.Flags & MLMeshing.SettingsFlags.ComputeConfidence) != 0);
            planarize = ((settings.Flags & MLMeshing.SettingsFlags.Planarize) != 0);
            removeMeshSkirt = ((settings.Flags & MLMeshing.SettingsFlags.RemoveMeshSkirt) != 0);
            fillHoleLength = settings.FillHoleLength;
            disconnectedComponentArea = settings.DisconnectedComponentArea;
        }
        #endif

        /// <summary>
        /// Updates the current settings based on the class variables.
        /// </summary>
        private void UpdateSettings()
        {
            CleanMeshes();

            #if PLATFORM_LUMIN
            MLMeshing.SettingsFlags flags = MLMeshing.SettingsFlags.IndexOrderCW;

            if (meshType == MeshingType.PointCloud)
                flags |= MLMeshing.SettingsFlags.PointCloud;
            if (computeNormals)
                flags |= MLMeshing.SettingsFlags.ComputeNormals;
            if (requestVertexConfidence)
                flags |= MLMeshing.SettingsFlags.ComputeConfidence;
            if (planarize)
                flags |= MLMeshing.SettingsFlags.Planarize;
            if (removeMeshSkirt)
                flags |= MLMeshing.SettingsFlags.RemoveMeshSkirt;

            MLMeshing.CurrentMeshSettings = new MLMeshing.MeshSettings()
            {
                Flags = flags,
                FillHoleLength = fillHoleLength,
                DisconnectedComponentArea = disconnectedComponentArea
            };



            settingsDirty = false;
            #endif
        }

        /// <summary>
        /// Cleans all the cached meshes. Used when settings change at runtime in order to facilitate mesh reconstruction.
        /// </summary>
        private void CleanMeshes()
        {
            #if PLATFORM_LUMIN
            foreach(MeshObject meshObject in meshObjMap.Values)
            {
                Destroy(meshObject.Obj);
            }

            meshObjMap.Clear();
            #endif
        }

        /// <summary>
        /// Updates the mesh extents based on the current transform values.
        /// </summary>
        private void UpdateMeshExtents()
        {
            #if PLATFORM_LUMIN
            meshExtents.Center = transform.localPosition;
            meshExtents.Rotation = transform.localRotation;
            meshExtents.Extents = transform.localScale;
            #endif
        }

        /// <summary>
        /// Processes latest Mesh Info request results.
        /// </summary>
        private void ProcessMeshInfoRequests()
        {
            #if PLATFORM_LUMIN
            MLResult result = MLMeshing.RequestMeshInfo(in meshExtents);
            if (!result.IsOk)
            {
                Debug.LogErrorFormat("Error: MLMeshingBehavior failed to create a new mesh info request, disabling script. Reason: {0}", result);
                enabled = false;
                return;
            }

            MLMeshing.GetMeshInfoResults(out Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, Tuple<MLMeshing.BlockInfo, ulong>> results);
            if (results.Count > 0)
            {
                foreach (Tuple<MLMeshing.BlockInfo, ulong> pair in results.Values)
                {
                    switch (pair.Item1.State)
                    {
                        case MLMeshing.MeshState.New:
                        case MLMeshing.MeshState.Updated:
                        case MLMeshing.MeshState.Unchanged:
                            AddToQueueIfNecessary(pair.Item1);
                            break;

                        case MLMeshing.MeshState.Deleted:
                            OnMeshRemoved?.Invoke(pair.Item1.ID);

                            meshesToGenerateIDs.Remove(pair.Item1.ID);

                            if (meshObjMap.TryGetValue(pair.Item1.ID, out MeshObject meshObj))
                            {
                                Destroy(meshObj.Obj);
                                meshObjMap.Remove(pair.Item1.ID);
                            }

                            break;
                        default:
                            break;
                    }
                }
            }
            #endif
        }

        #if PLATFORM_LUMIN
        /// <summary>
        /// Adds mesh info to the map of meshes to generate.
        /// </summary>
        /// <param name="info">The mesh info to add.</param>
        void AddToQueueIfNecessary(MLMeshing.BlockInfo info)
        {
            if (meshesToGenerateIDs.Contains(info.ID) || (info.State == MLMeshing.MeshState.Unchanged && meshObjMap.ContainsKey(info.ID)))
            {
                return;
            }

            meshesToGenerateIDs.Add(info.ID);
            meshesToGenerate.Enqueue(info);
        }
        #endif

        /// <summary>
        /// Requests mesh for mesh infos waiting to be generated.
        /// </summary>
        private void ProcessMeshRequests()
        {
            #if PLATFORM_LUMIN
            List<MLMeshing.MeshRequest> meshRequests = new List<MLMeshing.MeshRequest>();
            Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, MLMeshing.MeshState> blocksPerRequest = new Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, MLMeshing.MeshState>();
            int currentBatchCount;

            while (meshesBeingGenerated.Count < MeshQueueSize && meshesToGenerate.Count > 0)
            {
                currentBatchCount = 0;

                while(currentBatchCount < MeshBatchSize && meshesToGenerate.Count > 0)
                {
                    MLMeshing.BlockInfo info = meshesToGenerate.Dequeue();
                    if (meshesToGenerateIDs.Contains(info.ID))
                    {
                        meshRequests.Add(new MLMeshing.MeshRequest() { ID = info.ID, Level = LevelOfDetail });
                        blocksPerRequest.Add(info.ID, info.State);
                        meshesToGenerateIDs.Remove(info.ID);
                    }

                    ++currentBatchCount;
                }

                if(blocksPerRequest.Count == 0)
                {
                    Debug.LogErrorFormat("Error: MLMeshingBehavior failed to create a new mesh request, disabling script. Reason: Couldn't fill out mesh batch.");
                    enabled = false;
                    return;
                }

                MLResult result = MLMeshing.RequestMesh(in meshRequests, out ulong requestHandle, OnMeshRequestResult);
                if (!result.IsOk)
                {
                    Debug.LogErrorFormat("Error: MLMeshingBehavior failed to create a new mesh request, disabling script. Reason: {0}", result);
                    enabled = false;
                    return;
                }

                meshesBeingGenerated.Add(requestHandle, blocksPerRequest);

                blocksPerRequest.Clear();
                meshRequests.Clear();
            }
            #endif
        }

        #if PLATFORM_LUMIN
        /// <summary>
        /// Handle the result of a mesh request.
        /// </summary>
        /// <param name="requestHandle">Handle of the request.</param>
        /// <param name="meshResult">The mesh results.</param>
        private void OnMeshRequestResult(ulong requestHandle, MLMeshing.MeshRequestResult meshResult)
        {
            if(!meshesBeingGenerated.ContainsKey(requestHandle))
            {
                Debug.LogError("Error: MLMeshingBehavior failed to get a new mesh request result, disabling script. Reason: Request handle not in generation list.");
                enabled = false;
                return;
            }

            if (meshResult.Result != MLMeshing.MeshResult.Success)
            {
                Debug.LogErrorFormat("Error: MLMeshingBehavior failed to get a new mesh request result, disabling script. Reason: {0}", meshResult.Result);
                enabled = false;
                return;
            }

            foreach(MLMeshing.BlockMesh blockMesh in meshResult.Meshes)
            {
                MeshObject meshObj = GetOrCreateGameObject(blockMesh.ID, out bool newMesh);

                MeshRenderer meshRenderer = meshObj.Obj.GetComponent<MeshRenderer>();
                if (!meshRenderer)
                {
                    Debug.LogError("Error: MLMeshingBehavior failed to get a new mesh request result, disabling script. Reason: MLMeshingBehavior.MeshPrefab missing MeshRenderer component");
                    enabled = false;
                    return;
                }
                else
                {
                    meshRenderer.material = blockMesh.Flags.HasFlag(MLMeshing.SettingsFlags.PointCloud) ? PointCloudMeshMaterial : TriangleMeshMaterial;
                }

                MeshCollider collider = meshObj.Obj.GetComponent<MeshCollider>();
                if (collider)
                {
                    if (blockMesh.Flags.HasFlag(MLMeshing.SettingsFlags.PointCloud))
                    {
                        collider.enabled = false;
                    }
                    else
                    {
                        collider.sharedMesh = blockMesh.MeshData;
                    }
                }

                MeshFilter meshFilter = meshObj.Obj.GetComponent<MeshFilter>();
                if(!meshFilter)
                {
                    Debug.LogError("Error: MLMeshingBehavior failed to get a new mesh request result, disabling script. Reason: MLMeshingBehavior.MeshPrefab missing MeshFilter component");
                    enabled = false;
                    return;
                }
                else
                {
                    if (meshFilter.sharedMesh != null)
                    {
                        meshFilter.sharedMesh = blockMesh.MeshData;
                    }
                    else
                    {
                        meshFilter.mesh = blockMesh.MeshData;
                    }
                }

                meshObj.Result = blockMesh.Result;
                meshObj.ID = blockMesh.ID;
                meshObj.LevelOfDetail = blockMesh.Level;
                meshObj.Flags = blockMesh.Flags;
                meshObj.Confidence = (blockMesh.Confidences.Length == 0) ? default : new List<float>(blockMesh.Confidences.ToArray());

                if (newMesh == true)
                {
                    OnMeshAdded?.Invoke(meshObj);
                }
                else
                {
                    OnMeshUpdated?.Invoke(meshObj);
                }
            }

            meshesBeingGenerated.Remove(requestHandle);
        }

        /// <summary>
        /// Gets the object corresponding to the given ID or creates a new one.
        /// </summary>
        /// <param name="meshID">The ID of the mesh in question.</param>
        /// <returns>The newly generated mesh object.</returns>
        private MeshObject GetOrCreateGameObject(MagicLeapNativeBindings.MLCoordinateFrameUID meshID, out bool newMesh)
        {
            newMesh = !meshObjMap.TryGetValue(meshID, out MeshObject meshObj);
            if (newMesh)
            {
                meshObj = new MeshObject();
                meshObj.Obj = Instantiate(MeshPrefab, MeshParent);
                meshObj.Obj.name = string.Format("Mesh {0}", meshID);
                meshObj.Obj.SetActive(true);
                meshObjMap.Add(meshID, meshObj);
            }

            return meshObj;
        }
        #endif

        /// <summary>
        /// Contains the default values for all the available settings in the class.
        /// </summary>
        private static class DefaultSettings
        {
            public static Vector3 BoundsExtents = Vector3.one * 10f;
            public static float FillHoleLength = 1f;
            public static bool ComputeNormals = true;
            public static MeshingType MeshType = MeshingType.Triangles;
            public static bool Planarize = false;
            public static float DisconnectedComponentArea = .25f;
            public static float PollingRate = 0.25f;
            public static uint MeshQueueSize = 4;
            public static uint MeshBatchSize = 16;
            public static bool RequestVertexConfidence = false;
            public static bool RemoveMeshSkirt = false;
            public static MLMeshing.LOD LevelOfDetail = MLMeshing.LOD.Medium;
        }

        /// <summary>
        /// Stores all the data for the object generated from a mesh result.
        /// </summary>
        public struct MeshObject
        {
            /// <summary>
            /// The result of the meshing.
            /// </summary>
            public MLMeshing.MeshResult Result;

            #if PLATFORM_LUMIN
            /// <summary>
            /// The coordinate FrameID of the block.
            /// </summary>
            public MagicLeapNativeBindings.MLCoordinateFrameUID ID;
            #endif

            /// <summary>
            /// The LOD level of the meshing request.
            /// </summary>
            public MLMeshing.LOD LevelOfDetail;

            /// <summary>
            /// Stores the settings this mesh was generated with.
            /// </summary>
            public MLMeshing.SettingsFlags Flags;

            /// <summary>
            /// Game object storing generated from prefab and mesh data.
            /// </summary>
            public GameObject Obj;

            /// <summary>
            /// List of confidence value. Only filled out if Flags.ComputeConfidence is set.
            /// </summary>
            public List<float> Confidence;
        }


        /// <summary>
        /// Helper priority queue container.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        private class PriorityQueue<T> where T : IComparable<T>
        {
            private List<T> data;

            public int Count
            {
                get
                {
                    return data.Count;
                }
            }

            public PriorityQueue()
            {
                this.data = new List<T>();
            }

            public void Enqueue(T item)
            {
                data.Add(item);
                int ci = data.Count - 1; // child index; start at end
                while (ci > 0)
                {
                    int pi = (ci - 1) / 2; // parent index
                    if (data[ci].CompareTo(data[pi]) >= 0) break; // child item is larger than (or equal) parent so we're done
                    T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
                    ci = pi;
                }
            }

            public T Dequeue()
            {
                // assumes pq is not empty; up to calling code
                int li = data.Count - 1; // last index (before removal)
                T frontItem = data[0];   // fetch the front
                data[0] = data[li];
                data.RemoveAt(li);

                --li; // last index (after removal)
                int pi = 0; // parent index. start at front of pq
                while (true)
                {
                    int ci = pi * 2 + 1; // left child index of parent
                    if (ci > li) break;  // no children so done
                    int rc = ci + 1;     // right child
                    if (rc <= li && data[rc].CompareTo(data[ci]) < 0) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
                        ci = rc;
                    if (data[pi].CompareTo(data[ci]) <= 0) break; // parent is smaller than (or equal to) smallest child so done
                    T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp; // swap parent and child
                    pi = ci;
                }
                return frontItem;
            }

            public T Peek()
            {
                T frontItem = data[0];
                return frontItem;
            }

            public bool IsConsistent()
            {
                // is the heap property true for all data?
                if (data.Count == 0) return true;
                int li = data.Count - 1; // last index
                for (int pi = 0; pi < data.Count; ++pi) // each parent index
                {
                    int lci = 2 * pi + 1; // left child index
                    int rci = 2 * pi + 2; // right child index

                    if (lci <= li && data[pi].CompareTo(data[lci]) > 0) return false; // if lc exists and it's greater than parent then bad.
                    if (rci <= li && data[pi].CompareTo(data[rci]) > 0) return false; // check the right child too.
                }
                return true; // passed all checks
            }
        }
    }
}
