// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using MagicLeap.OpenXR.Features.EyeTracker;
using MagicLeap.OpenXR.Features.FacialExpressions;
using MagicLeap.OpenXR.Features.LightEstimation;
using MagicLeap.OpenXR.Features.LocalizationMaps;
using MagicLeap.OpenXR.Features.MarkerUnderstanding;
using MagicLeap.OpenXR.Features.Meshing;
using MagicLeap.OpenXR.Features.PhysicalOcclusion;
using MagicLeap.OpenXR.Features.PixelSensors;
using MagicLeap.OpenXR.Features.Planes;
using MagicLeap.OpenXR.Features.Reprojection;
using MagicLeap.OpenXR.Features.SpatialAnchors;
using MagicLeap.OpenXR.Features.SystemNotifications;
using MagicLeap.OpenXR.Features.UserCalibration;
using MagicLeap.OpenXR.InteractionProfiles;
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;

namespace MagicLeap.OpenXR.Features
{
    [OpenXRFeatureSet(
        UiName = "Magic Leap 2",
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
            MagicLeapMarkerUnderstandingFeature.FeatureId,
            MagicLeapLocalizationMapFeature.FeatureId,
            MagicLeapSecondaryViewSupportFeature.FeatureId
        },
        DefaultFeatureIds = new[] {
            MagicLeapFeature.FeatureId,
            MagicLeapControllerProfile.FeatureId,
            MagicLeapRenderingExtensionsFeature.FeatureId,
            MagicLeapReferenceSpacesFeature.FeatureId,
            MagicLeapPlanesFeature.FeatureId,
            MagicLeapUserCalibrationFeature.FeatureId,
            MagicLeapMarkerUnderstandingFeature.FeatureId,
            MagicLeapLocalizationMapFeature.FeatureId,
            MagicLeapSecondaryViewSupportFeature.FeatureId
        }
    )]
    public class MagicLeapFeatureGroup { }

    [OpenXRFeatureSet(
        UiName = "Magic Leap 2 Experimental",
        Description = "Features supported by the Magic Leap 2 platform, which use still experimental OpenXR extensions.",
        FeatureSetId = "com.magicleap.openxr.featuregroup-exp",
        SupportedBuildTargets = new[] { BuildTargetGroup.Android },
        FeatureIds = new[] {
            MagicLeapSpatialAnchorsFeature.FeatureId,
            MagicLeapSpatialAnchorsStorageFeature.FeatureId,
            MagicLeapFacialExpressionFeature.FeatureId,
            MagicLeapMeshingFeature.FeatureId,
            MagicLeapLightEstimationFeature.FeatureId,
            MagicLeapPixelSensorFeature.FeatureId,
            MagicLeapDepthRangeChangeFeature.FeatureId,
            MagicLeapSystemNotificationsFeature.FeatureId,
            MagicLeapPhysicalOcclusionFeature.FeatureId,
            MagicLeapReprojectionFeature.FeatureId,
            MagicLeapEyeTrackerFeature.FeatureId,
        },
        DefaultFeatureIds = new[] {
            MagicLeapSpatialAnchorsFeature.FeatureId,
            MagicLeapSpatialAnchorsStorageFeature.FeatureId,
            MagicLeapFacialExpressionFeature.FeatureId,
            MagicLeapMeshingFeature.FeatureId,
            MagicLeapLightEstimationFeature.FeatureId,
            MagicLeapPixelSensorFeature.FeatureId,
            MagicLeapDepthRangeChangeFeature.FeatureId,
            MagicLeapSystemNotificationsFeature.FeatureId,
            MagicLeapPhysicalOcclusionFeature.FeatureId,
            MagicLeapReprojectionFeature.FeatureId,
            MagicLeapEyeTrackerFeature.FeatureId,
        }
    )]
    public class MagicLeapExperimentalFeatureGroup { }

#if UNITY_EDITOR_WIN
    [OpenXRFeatureSet(
        UiName = "Magic Leap 2",
        Description = "Features supported by Magic Leap 2 Remote Rendering.",
        FeatureSetId = "com.magicleap.openxr.featuregroup.appsim",
        SupportedBuildTargets = new[] { BuildTargetGroup.Standalone },
        FeatureIds = new[] {
            MagicLeapFeature.FeatureId,
            MagicLeapControllerProfile.FeatureId,
            MagicLeapRenderingExtensionsFeature.FeatureId,
            MagicLeapReferenceSpacesFeature.FeatureId,
            MagicLeapPlanesFeature.FeatureId,
            MagicLeapUserCalibrationFeature.FeatureId,
            MagicLeapMarkerUnderstandingFeature.FeatureId,
            MagicLeapLocalizationMapFeature.FeatureId,
            MagicLeapSecondaryViewSupportFeature.FeatureId
        },
        DefaultFeatureIds = new[] {
            MagicLeapFeature.FeatureId,
            MagicLeapControllerProfile.FeatureId,
            MagicLeapRenderingExtensionsFeature.FeatureId,
            MagicLeapReferenceSpacesFeature.FeatureId,
            MagicLeapPlanesFeature.FeatureId,
            MagicLeapUserCalibrationFeature.FeatureId,
            MagicLeapMarkerUnderstandingFeature.FeatureId,
            MagicLeapLocalizationMapFeature.FeatureId,
            MagicLeapSecondaryViewSupportFeature.FeatureId
        }
    )]
#endif
    public class MagicLeapFeatureAppSimGroup { }

#if UNITY_EDITOR_WIN
    [OpenXRFeatureSet(
        UiName = "Magic Leap 2 Experimental",
        Description = "Features supported by Magic Leap 2 Remote Rendering, which use still experimental OpenXR extensions.",
        FeatureSetId = "com.magicleap.openxr.featuregroup.appsim-exp",
        SupportedBuildTargets = new[] { BuildTargetGroup.Standalone },
        FeatureIds = new[] {
            MagicLeapSpatialAnchorsFeature.FeatureId,
            MagicLeapSpatialAnchorsStorageFeature.FeatureId,
            MagicLeapFacialExpressionFeature.FeatureId,
            MagicLeapMeshingFeature.FeatureId,
            MagicLeapLightEstimationFeature.FeatureId,
            MagicLeapPixelSensorFeature.FeatureId,
            MagicLeapDepthRangeChangeFeature.FeatureId,
            MagicLeapSystemNotificationsFeature.FeatureId
        },
        DefaultFeatureIds = new[] {
            MagicLeapSpatialAnchorsFeature.FeatureId,
            MagicLeapSpatialAnchorsStorageFeature.FeatureId,
            MagicLeapFacialExpressionFeature.FeatureId,
            MagicLeapMeshingFeature.FeatureId,
            MagicLeapLightEstimationFeature.FeatureId,
            MagicLeapPixelSensorFeature.FeatureId,
            MagicLeapDepthRangeChangeFeature.FeatureId,
            MagicLeapSystemNotificationsFeature.FeatureId
        }
    )]
#endif
    public class MagicLeapExperimentalFeatureAppSimGroup { }

}
