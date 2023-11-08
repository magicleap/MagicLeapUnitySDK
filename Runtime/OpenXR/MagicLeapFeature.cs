// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
#if UNITY_OPENXR_1_9_0_OR_NEWER
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap;
using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    /// <summary>
    /// Enables the Magic Leap OpenXR Loader for Android, and modifies the AndroidManifest to be compatible with ML2.
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Support",
        Desc="Necessary to deploy a Magic Leap 2 compatible application.",
        Company = "Magic Leap",
        Version = "1.0.0",
        BuildTargetGroups = new []{ BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        FeatureId = featureId,
        OpenxrExtensionStrings = "XR_ML_compat XR_KHR_convert_timespec_time XR_EXT_view_configuration_depth_range"
    )]
#endif
    public partial class MagicLeapFeature : OpenXRFeature
    {
        private struct xrSessionCached
        {
            public int oldState;
            public int newState;
        }

        public enum FarClipMode : byte
        {
            /// <summary>
            /// Do not restrict the Camera's far clip plane distance.
            /// </summary>
            None,

            /// <summary>
            /// Restrict the Camera's far clip plane distance to no more than the maximum allowed by the device.
            /// </summary>
            Maximum,

            /// <summary>
            /// Restrict the Camera's far clip plane to no more than the distance recommended by Magic Leap.
            /// </summary>
            Recommended,
        }

        public enum NearClipMode : byte
        {
            /// <summary>
            /// Restrict the Camera's near clip plane to no less than the absolute minimum allowed (25cm).
            /// </summary>
            Minimum,

            /// <summary>
            /// Restrict the Camera's near clip plane to no less than the distance configured in the system's settings.
            /// </summary>
            Recommended,
#if DISABLE_MAGICLEAP_CLIP_ENFORCEMENT
            /// <summary>
            /// Unsupported
            /// </summary>
            None,
#endif
        }

        /// <summary>
        /// The feature id string. This is used to give the feature a well known id for reference.
        /// </summary>
        public const string featureId = "com.magicleap.openxr.feature.ml2";

        [SerializeField]
        private bool enableMLAudio = false;

        [SerializeField]
        [Tooltip("Determines if the far clipping plane should be clamped, and to what maximum value.")]
        private FarClipMode m_FarClipPolicy = FarClipMode.Recommended;

        [SerializeField]
        [Tooltip("Determines the minimum value the near clipping plane will be clamped to.")]
        private NearClipMode m_NearClipPolicy = NearClipMode.Recommended;

        public FarClipMode farClipPolicy => m_FarClipPolicy;
        public NearClipMode nearClipPolicy => m_NearClipPolicy;

        public float minNearZ => NativeBindings.MLOpenXRGetMinNearClippingPlane();
        public float recommendedNearZ => NativeBindings.MLOpenXRGetRecommendedNearClippingPlane();
        public float maxFarZ => NativeBindings.MLOpenXRGetMaxFarClippingPlane();
        public float recommendedFarZ => NativeBindings.MLOpenXRGetRecommendedFarClippingPlane();

        public bool IsMLAudioEnabled => enableMLAudio;

        private static List<XRMeshSubsystemDescriptor> s_MeshSubsysDesc = new List<XRMeshSubsystemDescriptor>();
        private static List<XRSessionSubsystemDescriptor> s_SessionSubsysDesc = new List<XRSessionSubsystemDescriptor>();

        public delegate void OnXRSessionStateChangeDelegate(int oldState, int newState);
        public static event OnXRSessionStateChangeDelegate OnXRSessionStateChange;

        private static List<xrSessionCached> xrSessionCacheList;

        protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
        {
            return NativeBindings.MLOpenXRInterceptFunctions(func);
        }

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            return NativeBindings.MLOpenXROnInstanceCreate(xrGetInstanceProcAddr, xrInstance);
        }

        protected override void OnInstanceDestroy(ulong xrInstance)
        {
            NativeBindings.MLOpenXROnInstanceDestroy(xrInstance);
        }

        protected override void OnAppSpaceChange(ulong xrSpace)
        {
            NativeBindings.MLOpenXROnAppSpaceChange(xrSpace);
        }

        protected override void OnSessionCreate(ulong xrSession)
        {
            NativeBindings.MLOpenXROnSessionCreate(xrSession);
        }

        protected override void OnSessionBegin(ulong xrSession)
        {
            base.OnSessionBegin(xrSession);

            Application.onBeforeRender += EnforceClippingPlanes;
        }

        protected override void OnSessionEnd(ulong xrSession)
        {
            base.OnSessionEnd(xrSession);

            Application.onBeforeRender -= EnforceClippingPlanes;
        }

        protected override void OnSessionDestroy(ulong xrSession)
        {
            NativeBindings.MLOpenXROnSessionDestroy(xrSession);
        }
        protected override void OnSessionStateChange(int oldState, int newState)
        {
            if (OnXRSessionStateChange != null)
            {
                OnXRSessionStateChange.Invoke(oldState, newState);
            }
            else
            {
                xrSessionCached newEvent;
                newEvent.oldState = oldState;
                newEvent.newState = newState;
                xrSessionCacheList.Add(newEvent);
            }

            NativeBindings.MLOpenXRUpdateDepthRangeValues();
        }

        protected override void OnSubsystemCreate()
        {
            xrSessionCacheList = new List<xrSessionCached>();

            base.OnSubsystemCreate();
            
            CreateSubsystem<XRMeshSubsystemDescriptor, XRMeshSubsystem>(s_MeshSubsysDesc, MagicLeapXrProvider.MeshingSubsystemId);
            CreateSubsystem<XRSessionSubsystemDescriptor, XRSessionSubsystem>(s_SessionSubsysDesc, MagicLeapXrProvider.SessionSubsystemId);
        }
        
        protected override void OnSubsystemDestroy()
        {
            base.OnSubsystemDestroy();
            DestroySubsystem<XRMeshSubsystem>();
            DestroySubsystem<XRSessionSubsystem>();
        }

        public void StartMeshSubsystem()
        {
            StartSubsystem<XRMeshSubsystem>();
        }

        public void StopMeshSubsystem()
        {
            StopSubsystem<XRMeshSubsystem>();
        }

        public static void GetCachedSessionEvents()
        {
            foreach (xrSessionCached cachedEvent in xrSessionCacheList)
            {
                OnXRSessionStateChange.Invoke(cachedEvent.oldState, cachedEvent.newState);
            }

            xrSessionCacheList.Clear();
        }

        private void EnforceClippingPlanes() => ApplyToCamera(Camera.main);

        public void ApplyFarClip(ref float zFar)
        {
            switch (m_FarClipPolicy)
            {
                case FarClipMode.Maximum:
                    zFar = Mathf.Min(zFar, Mathf.Min(zFar, maxFarZ));
                    break;
                case FarClipMode.Recommended:
                    zFar = Mathf.Min(zFar, recommendedFarZ);
                    break;
                case FarClipMode.None:
                default:
                    break;
            }
        }

        public void ApplyNearClip(ref float zNear)
        {
            switch (m_NearClipPolicy)
            {
                // Whatever is set in the system settings menu is our new minimum, even if it is
                // above the system recommendation
                case NearClipMode.Minimum:
                    zNear = Mathf.Max(zNear, minNearZ);
                    break;
                case NearClipMode.Recommended:
                    zNear = Mathf.Max(zNear, recommendedNearZ);
                    break;
#if DISABLE_MAGICLEAP_CLIP_ENFORCEMENT
                case NearClipMode.None:
                default:
                    break;
#endif
            }
        }

        public void ApplyToCamera(Camera camera, bool warnIfNearClipChanged = true)
        {
            if (!camera)
                return;

            var zFar = camera.farClipPlane;
            var zNear = camera.nearClipPlane;

            ApplyFarClip(ref zFar);
            ApplyNearClip(ref zNear);

            if (warnIfNearClipChanged && zNear > camera.nearClipPlane)
                Debug.LogWarning($"Main Camera's nearClipPlane value is less than the minimum value for this device. Increasing to {zNear}");

            camera.farClipPlane = zFar;
            camera.nearClipPlane = zNear;
        }
    }
}
#endif
