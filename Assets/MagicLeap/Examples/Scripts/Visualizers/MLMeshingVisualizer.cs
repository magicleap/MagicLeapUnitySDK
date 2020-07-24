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

//EXPERIMENTAL

#if UNITY_EDITOR || PLATFORM_LUMIN
using UnityEngine;
using MagicLeap.Core;

namespace MagicLeap
{
    /// <summary>
    /// This class allows you to change meshing properties at runtime, including the rendering mode.
    /// Manages the MLSpatialMapper behaviour and tracks the meshes.
    /// </summary>
    public class MLMeshingVisualizer : MonoBehaviour
    {
        public enum MeshRenderMode
        {
            None,
            Wireframe,
            PointCloud,
            Occlusion
        }

        [SerializeField, Tooltip("The MLMeshingBehavior from which to get update on mesh types.")]
        private MLMeshingBehavior _meshingBehavior = null;

        [SerializeField, Tooltip("The material to apply for occlusion.")]
        private Material _occlusionMaterial = null;

        public MeshRenderMode RenderMode { get; private set; } = MeshRenderMode.Wireframe;
        private int _renderModeCount;

        /// <summary>
        /// Start listening for MLSpatialMapper events.
        /// </summary>
        void Awake()
        {
            // Validate all required game objects.
            if (_meshingBehavior == null)
            {
                Debug.LogError("Error: MeshingVisualizer._meshingBehavior is not set, disabling script!");
                enabled = false;
                return;
            }
            if(_occlusionMaterial == null)
            {
                Debug.LogError("Error: MeshingVisualizer._occlusionMaterial is not set, disabling script!");
                enabled = false;
                return;
            }

            _renderModeCount = System.Enum.GetNames(typeof(MeshRenderMode)).Length;
        }

        /// <summary>
        /// Register for new and updated freagments.
        /// </summary>
        void Start()
        {
            _meshingBehavior.OnMeshAdded += HandleOnMeshReady;
            _meshingBehavior.OnMeshUpdated += HandleOnMeshReady;
        }

        /// <summary>
        /// Unregister callbacks.
        /// </summary>
        void OnDestroy()
        {
            _meshingBehavior.OnMeshAdded -= HandleOnMeshReady;
            _meshingBehavior.OnMeshUpdated -= HandleOnMeshReady;
        }

        /// <summary>
        /// Set the render mode to the next in enum;
        /// </summary>
        public void SetNextRendereMode()
        {
            RenderMode = (MeshRenderMode)((int)(RenderMode + 1) % _renderModeCount);

            // Clear existing meshes to process the new mesh type.
            switch (RenderMode)
            {
                case MeshRenderMode.Wireframe:
                case MeshRenderMode.Occlusion:
                case MeshRenderMode.None:
                {
                    _meshingBehavior.MeshType = MLMeshingBehavior.MeshingType.Triangles;

                    break;
                }
                case MeshRenderMode.PointCloud:
                {
                    _meshingBehavior.MeshType = MLMeshingBehavior.MeshingType.PointCloud;

                    break;
                }
                default:
                    break;
            }

            foreach(MeshRenderer renderer in _meshingBehavior.MeshParent.GetComponentsInChildren<MeshRenderer>())
            {
                if (renderer.gameObject.name.StartsWith("Mesh"))
                {
                    UpdateRenderer(renderer);
                }
            }
        }

        /// <summary>
        /// Updates the currently selected render material on the MeshRenderer.
        /// </summary>
        /// <param name="meshRenderer">The MeshRenderer that should be updated.</param>
        private void UpdateRenderer(MeshRenderer meshRenderer)
        {
            if (meshRenderer != null)
            {
                switch(RenderMode)
                {
                    case MeshRenderMode.Occlusion:
                        meshRenderer.enabled = true;
                        meshRenderer.material = _occlusionMaterial;
                        break;
                    case MeshRenderMode.None:
                        meshRenderer.enabled = false;
                        break;
                    case MeshRenderMode.PointCloud:
                    case MeshRenderMode.Wireframe:
                        meshRenderer.enabled = true;
                        break;
                    default:
                        break;

                }
            }
        }

        /// <summary>
        /// Handles the MeshReady event, which tracks and assigns the correct mesh renderer materials.
        /// </summary>
        /// <param name="meshId">Id of the mesh that got added / upated.</param>
        private void HandleOnMeshReady(MLMeshingBehavior.MeshObject meshObject)
        {
            UpdateRenderer(meshObject.Obj.GetComponent<MeshRenderer>());
        }
    }
}

#endif
