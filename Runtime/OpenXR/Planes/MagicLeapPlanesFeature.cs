#if UNITY_OPENXR_1_7_0_OR_NEWER

using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap;

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
    [OpenXRFeature(UiName = "Magic Leap 2 Plane Detection Support",
        Desc="Necessary to deploy a Magic Leap 2 compatible application with Planes detection",
        Company = "Magic Leap",
        Version = "1.0.0",
        Priority = -2,
        FeatureId = FeatureId,
        BuildTargetGroups = new[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        OpenxrExtensionStrings = PlaneExtensionName
    )]
#endif
    public class MagicLeapPlanesFeature : MagicLeapOpenXRFeatureBase
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_planes";
        private const string PlaneExtensionName = "XR_EXT_plane_detection";
        
        private readonly List<XRPlaneSubsystemDescriptor> _planeSubsystemDescriptors = new();

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            if (OpenXRRuntime.IsExtensionEnabled(PlaneExtensionName))
            {
                return base.OnInstanceCreate(xrInstance);
            }
            Debug.LogError($"{PlaneExtensionName} is not enabled. Disabling {nameof(MagicLeapPlanesFeature)}");
            return false;
        }

        protected override void OnSubsystemCreate()
        {
            base.OnSubsystemCreate();
            CreateSubsystem<XRPlaneSubsystemDescriptor, XRPlaneSubsystem>(_planeSubsystemDescriptors, MagicLeapXrProvider.PlanesSubsystemId);
        }

        protected override void OnSubsystemStart()
        {
            base.OnSubsystemStart();
            StartSubsystem<XRPlaneSubsystem>();
        }

        protected override void OnSubsystemStop()
        {
            base.OnSubsystemStop();
            StopSubsystem<XRPlaneSubsystem>();
        }

        protected override void OnSubsystemDestroy()
        {
            base.OnSubsystemDestroy();
            DestroySubsystem<XRPlaneSubsystem>();
        }
    }
}
#endif
