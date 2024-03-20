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
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport;

namespace UnityEditor.XR.OpenXR.Features.MagicLeapSupport
{
    [OpenXRFeatureSet(
        UiName = "Magic Leap",
        Description = "Features supported by the Magic Leap 2 platform.",
        FeatureSetId = "com.magicleap.openxr.featuregroup",
        SupportedBuildTargets = new [] { BuildTargetGroup.Android },
        FeatureIds = new[] {
            MagicLeapFeature.FeatureId,
            MagicLeapControllerProfile.FeatureId,
            MagicLeapRenderingExtensionsFeature.FeatureId,
            MagicLeapReferenceSpacesFeature.FeatureId,
            MagicLeapPlanesFeature.FeatureId,
            MagicLeapUserCalibrationFeature.FeatureId,
            MagicLeapSystemNotificationsFeature.FeatureId,
            MagicLeapMarkerUnderstandingFeature.FeatureId,
            MagicLeapLocalizationMapFeature.FeatureId,
            MagicLeapSpatialAnchorsFeature.FeatureId,
            MagicLeapSpatialAnchorsStorageFeature.FeatureId,
            MagicLeapFacialExpressionFeature.FeatureId,
            MagicLeapMeshingFeature.FeatureId
        },
        DefaultFeatureIds = new[] {
            MagicLeapFeature.FeatureId,
            MagicLeapControllerProfile.FeatureId,
            MagicLeapRenderingExtensionsFeature.FeatureId,
            MagicLeapReferenceSpacesFeature.FeatureId,
            MagicLeapPlanesFeature.FeatureId,
            MagicLeapUserCalibrationFeature.FeatureId,
            MagicLeapSystemNotificationsFeature.FeatureId,
            MagicLeapMarkerUnderstandingFeature.FeatureId,
            MagicLeapLocalizationMapFeature.FeatureId,
            MagicLeapSpatialAnchorsFeature.FeatureId,
            MagicLeapSpatialAnchorsStorageFeature.FeatureId,
            MagicLeapFacialExpressionFeature.FeatureId,
            MagicLeapMeshingFeature.FeatureId
        }
    )]
    public class MagicLeapFeatureGroup { }

#if UNITY_EDITOR_WIN
    [OpenXRFeatureSet(
        UiName = "Magic Leap AppSim",
        Description = "All features supported by the Magic Leap 2 platform.",
        FeatureSetId = "com.magicleap.openxr.featuregroup.appsim",
        SupportedBuildTargets = new[] { BuildTargetGroup.Standalone },
        FeatureIds = new[] {
            MagicLeapFeature.FeatureId,
            MagicLeapControllerProfile.FeatureId,
            MagicLeapRenderingExtensionsFeature.FeatureId,
            MagicLeapReferenceSpacesFeature.FeatureId,
            MagicLeapPlanesFeature.FeatureId,
            MagicLeapUserCalibrationFeature.FeatureId,
            MagicLeapSystemNotificationsFeature.FeatureId,
            MagicLeapMarkerUnderstandingFeature.FeatureId,
            MagicLeapLocalizationMapFeature.FeatureId,
            MagicLeapSpatialAnchorsFeature.FeatureId,
            MagicLeapSpatialAnchorsStorageFeature.FeatureId,
            MagicLeapFacialExpressionFeature.FeatureId,
            MagicLeapMeshingFeature.FeatureId
        },
        DefaultFeatureIds = new[] {
            MagicLeapFeature.FeatureId,
            MagicLeapControllerProfile.FeatureId,
            MagicLeapPlanesFeature.FeatureId,
            MagicLeapMarkerUnderstandingFeature.FeatureId,
            MagicLeapSpatialAnchorsFeature.FeatureId,
            MagicLeapMeshingFeature.FeatureId
        }
    )]
#endif
    public class MagicLeapFeatureAppSimGroup { }
}
#endif
