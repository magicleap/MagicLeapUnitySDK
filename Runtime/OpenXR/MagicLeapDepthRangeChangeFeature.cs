// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
using UnityEditor;
#endif

namespace MagicLeap.OpenXR.Features
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Depth Range Change",
        Desc = "Enable changing depth range values at runtime.",
        Company = "Magic Leap",
        Version = "1.0.0",
        BuildTargetGroups = new[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        FeatureId = FeatureId,
        OpenxrExtensionStrings = "XR_ML_view_configuration_depth_range_change"
    )]
#endif
    public class MagicLeapDepthRangeChangeFeature : MagicLeapOpenXRFeatureBase
    {
        public const string FeatureId = "com.magicleap.openxr.feature.depth_range_change";

        protected override bool UsesExperimentalExtensions => true;
    }
}
