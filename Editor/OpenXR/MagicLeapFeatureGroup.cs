// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if UNITY_OPENXR_1_7_0_OR_NEWER
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    [OpenXRFeatureSet(
        UiName = "Magic Leap",
        Description = "All Magic Leap OpenXR Features",
        FeatureSetId = "com.magicleap.openxr.featuregroup",
        SupportedBuildTargets = new [] { BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        FeatureIds = new [] {
            MagicLeapFeature.featureId ,
            MagicLeapHandTrackingFeature.featureId,
            MagicLeapRenderingExtensionsFeature.featureId,
            MagicLeapReferenceSpacesFeature.featureId,
            MagicLeapClippingPlaneEnforcementFeature.featureId,
            MagicLeapPlanesFeature.FeatureId,
        }
    )]
    public class MagicLeapFeatureGroup
    { }
}
#endif
