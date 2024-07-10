// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using System.Collections.Generic;
using MagicLeap.OpenXR.Spaces;
using MagicLeap.OpenXR.Subsystems;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.OpenXR;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace MagicLeap.OpenXR.Features.SpatialAnchors
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Spatial Anchor Subsystem",
        Desc = "Create spatial anchors at target locations using ARAnchorSubsystem.",
        Company = "Magic Leap",
        Version = "1.0.0",
        BuildTargetGroups = new[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        FeatureId = FeatureId,
        OpenxrExtensionStrings = ExtensionName
    )]
#endif // UNITY_EDITOR
    public class MagicLeapSpatialAnchorsFeature : MagicLeapOpenXRFeatureBase
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_spatialanchor";
        private const string ExtensionName = "XR_ML_spatial_anchors XR_EXT_future";

        private readonly List<XRAnchorSubsystemDescriptor> anchorSubsystemDescriptors = new();

        internal MagicLeapSpatialAnchorsNativeFunctions SpatialAnchorsNativeFunctions;
        internal SpacesNativeFunctions SpaceInfoNativeFunctions;

        protected override bool UsesExperimentalExtensions => true;

        private struct AnchorsUpdateType
        { }

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            var extensions = ExtensionName.Split(' ');
            foreach(var extension in extensions)
            {
                if (OpenXRRuntime.IsExtensionEnabled(extension))
                {
                    continue;
                }

                Debug.LogError($"{extension} is not enabled. Disabling {nameof(MagicLeapSpatialAnchorsFeature)}");
                return false;
            }
            var instanceCreateResult = base.OnInstanceCreate(xrInstance);

            if (instanceCreateResult)
            {
                MLXrAnchorSubsystem.RegisterDescriptor();
            }

            SpatialAnchorsNativeFunctions = CreateNativeFunctions<MagicLeapSpatialAnchorsNativeFunctions>();
            SpaceInfoNativeFunctions = CreateNativeFunctions<SpacesNativeFunctions>();

            return instanceCreateResult;
        }

        protected override void OnSubsystemCreate()
        {
            base.OnSubsystemCreate();
            CreateSubsystem<XRAnchorSubsystemDescriptor, XRAnchorSubsystem>(anchorSubsystemDescriptors, MagicLeapXrProvider.AnchorSubsystemId);
        }

        protected override void OnSubsystemStart()
        {
            base.OnSubsystemStart();
            StartSubsystem<XRAnchorSubsystem>();
        }

        protected override void OnSubsystemStop()
        {
            base.OnSubsystemStop();
            StopSubsystem<XRAnchorSubsystem>();
        }

        protected override void OnSubsystemDestroy()
        {
            base.OnSubsystemDestroy();
            DestroySubsystem<XRAnchorSubsystem>();
        }
    }
}
