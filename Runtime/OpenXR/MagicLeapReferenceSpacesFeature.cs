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
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace MagicLeap.OpenXR.Features
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Reference Spaces",
        Desc="Support for additional XR reference spaces supported by Magic Leap 2",
        Company = "Magic Leap",
        Version = "1.0.0",
        BuildTargetGroups = new []{ UnityEditor.BuildTargetGroup.Android, UnityEditor.BuildTargetGroup.Standalone },
        FeatureId = FeatureId,
        OpenxrExtensionStrings = "XR_MSFT_unbounded_reference_space " +
                                 "XR_EXT_local_floor "
    )]
#endif
    public class MagicLeapReferenceSpacesFeature : MagicLeapOpenXRFeatureBase
    {
        public const string FeatureId = "com.magicleap.openxr.feature.reference_spaces";
    }
}
