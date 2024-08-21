// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using System;
using System.Collections.Generic;
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
    using NativeInterop;

    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.Meshing")]

    public partial class MagicLeapMeshingFeature : MagicLeapOpenXRFeatureBase
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_mesh_detection";
        private const string MeshingExtensionName = "XR_ML_world_mesh_detection XR_EXT_future";

        private MeshingMode currentMode = MeshingMode.Triangles;
        private Vector3 meshBoundsOrigin;
        private Vector3 meshBoundsScale;
        private Quaternion meshBoundsRotation;
        private FeatureLifecycleNativeListener meshingLifecycleListener;
        
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
            set => MagicLeapXrMeshingNativeBindings.MLOpenXRSetMeshDensity(value);
        }

        protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
        {
            MagicLeapXrMeshingNativeBindings.MLOpenXRGetMeshingFeatureListener(ref meshingLifecycleListener);
            return base.HookGetInstanceProcAddr(func);
        }

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            var extensions = MeshingExtensionName.Split(' ');
            foreach (var extension in extensions)
            {
                if (OpenXRRuntime.IsExtensionEnabled(extension))
                {
                    continue;
                }
                    
                Debug.LogError($"{extension} is not enabled. Disabling {nameof(MagicLeapMeshingFeature)}");
                return false;
            }

            var instanceCreationResult = base.OnInstanceCreate(xrInstance);
            if (!instanceCreationResult)
            {
                return false;
            }
                
            meshingLifecycleListener.InstanceCreated(xrInstance, xrGetInstanceProcAddr);
            return true;
        }

        protected override void OnSessionCreate(ulong xrSession)
        {
            base.OnSessionCreate(xrSession);
            meshingLifecycleListener.SessionCreated(xrSession);
        }

        protected override void OnAppSpaceChange(ulong xrSpace)
        {
            base.OnAppSpaceChange(xrSpace);
            meshingLifecycleListener.AppSpaceChanged(xrSpace);
        }

        protected override void OnInstanceDestroy(ulong xrInstance)
        {
            base.OnInstanceDestroy(xrInstance);
            meshingLifecycleListener.InstanceDestroyed(xrInstance);
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
            currentMode = mode;
            MagicLeapXrMeshingNativeBindings.MLOpenXRSetMeshRenderMode(currentMode);
            StopSubsystem<XRMeshSubsystem>();
            StopSubsystem<XRPointCloudSubsystem>();
            if (currentMode == MeshingMode.Triangles)
            {
                StartSubsystem<XRMeshSubsystem>();
            }

            if (currentMode == MeshingMode.PointCloud)
            {
                StartSubsystem<XRPointCloudSubsystem>();
            }
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
