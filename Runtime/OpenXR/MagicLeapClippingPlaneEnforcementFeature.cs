#if UNITY_OPENXR_1_7_0_OR_NEWER
using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

using MLOpenXRNative = UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapFeature.NativeBindings;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Clipping Plane Enforcement Support",
        Desc="Support for controlling the min/max depth range on Magic Leap 2",
        Company = "Magic Leap",
        Version = "1.0.0",
        BuildTargetGroups = new []{ BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        FeatureId = featureId,
        OpenxrExtensionStrings = "XR_EXT_view_configuration_depth_range"
    )]
#endif
    public class MagicLeapClippingPlaneEnforcementFeature : MagicLeapOpenXRFeatureBase
    {
        public enum FarClipMode : byte
        {
            None,
            Recommended,
        }

        public enum NearClipMode : byte
        {
            Recommended,
#if DISABLE_MAGICLEAP_CLIP_ENFORCEMENT
            // None is unsupported for near clipping as Legal requires us to enforce the near clip plane.
            None,
#endif
        }
        
        public const string featureId = "com.magicleap.openxr.feature.clipping_plane_enforcement";

        [SerializeField]
        [Tooltip("Determines if the far clipping plane should be clamped, and to what maximum value.")]
        private FarClipMode m_FarClipPolicy;

        [SerializeField]
        [Tooltip("Determines the minimum value the near clipping plane will be clamped to.")]
        private NearClipMode m_NearClipPolicy;

        public FarClipMode farClipPolicy => m_FarClipPolicy;
        public NearClipMode nearClipPolicy => m_NearClipPolicy;

        public float recommendedNearZ => MLOpenXRGetRecommendedNearClippingPlane();
        public float recommendedFarZ => MLOpenXRGetRecommendedFarClippingPlane();

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

        private void EnforceClippingPlanes() => ApplyToCamera(Camera.main);

        public void ApplyFarClip(ref float zFar)
        {
            switch (m_FarClipPolicy)
            {
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

        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern float MLOpenXRGetRecommendedNearClippingPlane();

        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern float MLOpenXRGetRecommendedFarClippingPlane();
    }
}
#endif
