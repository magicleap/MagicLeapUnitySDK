#if UNITY_OPENXR_1_9_0_OR_NEWER

using System;
using System.Collections.Generic;
using OpenXR.PointCloud;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    /// <summary>
    ///     Enables the Magic Leap OpenXR Loader for Android, and modifies the AndroidManifest to be compatible with ML2.
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Meshing Subsystem", 
        Desc = "Necessary to deploy a Magic Leap 2 compatible application with world meshing", 
        Company = "Magic Leap", 
        Version = "1.0.0", 
        Priority = -2, 
        FeatureId = FeatureId, 
        BuildTargetGroups = new[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone }, 
        OpenxrExtensionStrings = MeshingExtensionName
    )]
#endif
    public partial class MagicLeapMeshingFeature : MagicLeapOpenXRFeatureBase
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_mesh_detection";
        private const string MeshingExtensionName = "XR_ML_world_mesh_detection XR_EXT_future";

        private MeshingMode currentMode = MeshingMode.Triangles;
        private Vector3 meshBoundsOrigin;
        private Vector3 meshBoundsScale;
        private Quaternion meshBoundsRotation;
        
        /// <summary>
        /// The origin of the meshing volume bounds
        /// </summary>
        public Vector3 MeshBoundsOrigin
        {
            get => meshBoundsOrigin;
            set
            {
                meshBoundsOrigin = value;
                MagicLeapXrMeshingNativeBindings.MLOpenXRSetMeshBoundsOrigin(value);
            }
        }

        /// <summary>
        /// The scale of the meshing bounds volume
        /// </summary>
        public Vector3 MeshBoundsScale
        {
            get => meshBoundsScale;
            set
            {
                meshBoundsScale = value;
                MagicLeapXrMeshingNativeBindings.MLOpenXRSetMeshBoundsScale(value);
            }
        }
        
        /// <summary>
        /// The rotation of the meshing bounds volume
        /// </summary>
        public Quaternion MeshBoundsRotation
        {
            get => meshBoundsRotation;
            set
            {
                meshBoundsRotation = value;
                MagicLeapXrMeshingNativeBindings.MLOpenXRSetMeshBoundsOrientation(value);
            }
        }
        
        /// <summary>
        /// The render mode of the generated mesh.
        /// </summary>
        public MeshingMode MeshRenderMode
        {
            get => currentMode;
            set => SetRenderMode(value);
        }
        
        /// <summary>
        /// The density of the meshes generated
        /// </summary>
        public float MeshDensity
        {
            set
            {
                MagicLeapXrMeshingNativeBindings.MLOpenXRSetMeshDensity(value);
            }
        }

        protected override string GetFeatureId()
        {
            return FeatureId;
        }

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            var exts = MeshingExtensionName.Split(' ');
            foreach (var ext in exts)
            {
                if (!OpenXRRuntime.IsExtensionEnabled(ext))
                {
                    Debug.LogError($"{ext} is not enabled. Disabling {nameof(MagicLeapMeshingFeature)}");
                    return false;
                }
            }

            return base.OnInstanceCreate(xrInstance);
        }

        protected override void OnSubsystemCreate()
        {
            base.OnSubsystemCreate();
            CreateSubsystem<XRMeshSubsystemDescriptor, XRMeshSubsystem>(new List<XRMeshSubsystemDescriptor>(), MagicLeapXrProvider.MeshingSubsystemId);
            CreateSubsystem<XRPointCloudSubsystemDescriptor, XRPointCloudSubsystem>(new List<XRPointCloudSubsystemDescriptor>(), MagicLeapXrProvider.PointCloudSubsystemId);
        }

        protected override void OnSubsystemStart()
        {
            base.OnSubsystemStart();
            StartSubsystem<XRMeshSubsystem>();
        }

        protected override void OnSubsystemStop()
        {
            base.OnSubsystemStop();
            StopSubsystem<XRMeshSubsystem>();
            StopSubsystem<XRPointCloudSubsystem>();
        }

        protected override void OnSubsystemDestroy()
        {
            base.OnSubsystemDestroy();
            DestroySubsystem<XRMeshSubsystem>();
            DestroySubsystem<XRPointCloudSubsystem>();
        }

        internal void StartSubsystemForMLMeshing()
        {
            StartSubsystem<XRMeshSubsystem>();
        }

        internal void StopSubsystemForMLMeshing()
        {
            StopSubsystem<XRMeshSubsystem>();
        }
        
        /// <summary>
        /// Update the query settings for the mesh generation
        /// </summary>
        /// <param name="settings"></param>
        public void UpdateMeshQuerySettings(in MeshingQuerySettings settings)
        {
            MagicLeapXrMeshingNativeBindings.MLOpenXRMeshingUpdateSettings(in settings);
        }

        private void SetRenderMode(MeshingMode mode)
        {
            if (mode == currentMode)
            {
                return;
            }
            if (mode == MeshingMode.PointCloud && currentMode == MeshingMode.Triangles)
            {
                StopSubsystem<XRMeshSubsystem>();
                StartSubsystem<XRPointCloudSubsystem>();
            }

            if (mode == MeshingMode.Triangles && currentMode == MeshingMode.PointCloud)
            {
                StopSubsystem<XRPointCloudSubsystem>();
                StartSubsystem<XRMeshSubsystem>();
            }
            currentMode = mode;
            MagicLeapXrMeshingNativeBindings.MLOpenXRSetMeshRenderMode(currentMode);
        }

        public void SetMeshQueryBounds(Vector3 position, Vector3 scale, Quaternion rotation)
        {
            meshBoundsOrigin = position;
            meshBoundsScale = scale;
            meshBoundsRotation = rotation;
            MagicLeapXrMeshingNativeBindings.MLOpenXRSetMeshQueryBounds(in position,in rotation,in scale);
        }

        public void GetMeshIds(out TrackableId[] trackableIds)
        {
            trackableIds = Array.Empty<TrackableId>();
            unsafe
            {
                var buffer = (TrackableId*)IntPtr.Zero;
                MagicLeapXrMeshingNativeBindings.MLOpenXRAcquireMeshIds(ref buffer, out var trackableCount);
                if (trackableCount == 0)
                {
                    return;
                }

                trackableIds = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<TrackableId>(buffer, trackableCount, Allocator.None).ToArray();
            }
        }

        public bool GetMeshData(in TrackableId meshId, out Vector3[] positions, out Vector3[] normals, out float[] confidence)
        {
            positions = Array.Empty<Vector3>();
            normals = Array.Empty<Vector3>();
            confidence = Array.Empty<float>();
            unsafe
            {
                var positionsBuffer = (Vector3*)IntPtr.Zero;
                var normalBuffer = (Vector3*)IntPtr.Zero;
                var confidenceBuffer = (float*)IntPtr.Zero;

                var result = MagicLeapXrMeshingNativeBindings.MLOpenXRAcquireMeshData(in meshId, ref positionsBuffer, out var positionCount, ref normalBuffer, out var normalCount, ref confidenceBuffer, out var confidenceCount);
                if (!result)
                {
                    return false;
                }

                positions = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3>(positionsBuffer, positionCount, Allocator.None).ToArray();
                normals = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3>(normalBuffer, normalCount, Allocator.None).ToArray();
                confidence = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(confidenceBuffer, confidenceCount, Allocator.None).ToArray();
                return true;
            }
        }
        
        public void InvalidateMeshes()
        {
            MagicLeapXrMeshingNativeBindings.MLOpenXRInvalidateMeshes();
        }
    }
}
#endif
