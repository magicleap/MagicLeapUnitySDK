// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Collections.Generic;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// Utility class to generate the hand mesh and trigger callbacks based on availability of hand mesh.
    /// </summary>
    [AddComponentMenu("XR/MagicLeap/MLHandMeshingBehavior")]
    public class MLHandMeshingBehavior : MonoBehaviour
    {
        [SerializeField, Tooltip("A Prefab with a Mesh Filter and Mesh Renderer")]
        private GameObject meshBlockPrefab = null;

        [SerializeField, Tooltip("Material applied on the mesh")]
        private Material meshMaterial = null;

        #pragma warning disable 414
        [SerializeField, Tooltip("Recalculate normals")]
        private bool recalculateNormals = false;

        private bool hasPendingRequest = false;
        #pragma warning restore 414

        private List<MeshFilter> meshFilters = new List<MeshFilter>();

#if UNITY_MAGICLEAP || UNITY_ANDROID
        // The parameters used for querying hand mesh
        protected MLHandMeshing.Request.Params handMeshingRequestParams = new MLHandMeshing.Request.Params();

        private MLHandMeshing.Request handMeshingRequest;

        private MLHandMeshing.Request HandMeshingRequest =>
            handMeshingRequest ??=
                MLHandMeshing.RequestHandMesh(handMeshingRequestParams);
#endif

        /// <summary>
        /// Setter for the Mesh Material.
        /// </summary>
        public Material MeshMaterial
        {
            set
            {
                if (value == null)
                {
                    Debug.LogWarning("Assigning a null Material. Is this intentional?");
                }
                meshMaterial = value;
            }
        }

        /// <summary>
        /// Starts MLHandMeshing, validates inspector variables and public properties, starts requesting for hand mesh data.
        /// </summary>
        private void Start()
        {
            if (meshBlockPrefab == null)
            {
                Debug.LogError("MLHandMeshingBehavior.meshBlockPrefab is not set, disabling script.");
                enabled = false;
                return;
            }

            if (meshMaterial == null)
            {
                Debug.LogError("MLHandMeshingBehavior.meshMaterial is not set, disabling script.");
                enabled = false;
                return;
            }

            hasPendingRequest = true;
        }

        /// <summary>
        /// Request hand mesh whenever possible
        /// </summary>
        private void Update()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            RequestHandMesh();
#endif
        }

        /// <summary>
        /// Resumes the requesting if needed.
        /// </summary>
        void OnEnable()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            // resume mesh requesting
            if (!hasPendingRequest && MLHandMeshing.IsStarted)
            {
                hasPendingRequest = true;
            }
#endif
        }

#if UNITY_MAGICLEAP || UNITY_ANDROID
        private void RequestHandMesh()
        {
            MLResult mlResult = HandMeshingRequest.TryGetResult(out MLHandMeshing.Request.Result result);
            if (mlResult.IsOk)
            {
                HandleOnReceiveHandMesh(result.Mesh);
                HandMeshingRequest.Start(handMeshingRequestParams);
            }
        }

        /// <summary>
        /// Handler when Mesh Request is complete.
        /// Builds the mesh if available. Invokes the callbacks.
        /// </summary>
        /// <param name="result">Status of the request.</param>
        /// <param name="meshData">Mesh Data, only valid when result is Ok.</param>
        private void HandleOnReceiveHandMesh(MLHandMeshing.Mesh meshData)
        {
            hasPendingRequest = false;

            int numMeshes = (meshData.MeshBlock == null) ? 0 : meshData.MeshBlock.Length;
            for (var i = 0; i < numMeshes; ++i)
            {
                MeshFilter meshFilter;
                if (meshFilters.Count < i + 1)
                {
                    GameObject go = Instantiate(meshBlockPrefab, transform, true);
                    meshFilter = go.GetComponent<MeshFilter>();
                    meshFilter.mesh = new Mesh();
                    meshFilters.Add(meshFilter);
                }
                else
                {
                    meshFilter = meshFilters[i];
                    meshFilter.gameObject.SetActive(true);
                    meshFilter.mesh.Clear();
                }
                MeshRenderer renderer = meshFilter.GetComponent<MeshRenderer>();
                renderer.material = meshMaterial;

                meshFilter.mesh ??= new Mesh();
                Mesh mesh = meshFilter.mesh;
                mesh.vertices = meshData.MeshBlock[i].Vertex;
                mesh.triangles = meshData.MeshBlock[i].Index;
                if (recalculateNormals)
                {
                    mesh.RecalculateNormals();
                }
                meshFilter.mesh = mesh;
            }

            for (var j = numMeshes; j < meshFilters.Count; ++j)
            {
                meshFilters[j].gameObject.SetActive(false);
            }

            if (enabled)
            {
                hasPendingRequest = true;
            }
        }
#endif
    }
}
