// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%


using UnityEngine;
using UnityEngine.XR.OpenXR;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace MagicLeap.OpenXR.Features.MarkerUnderstanding
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Marker Understanding",
        Desc = "Necessary to deploy a Magic Leap 2 compatible application with Marker Understanding events.",
        Company = "Magic Leap",
        Version = "1.0.0",
        Priority = -1,
        BuildTargetGroups = new[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        FeatureId = FeatureId,
        OpenxrExtensionStrings = "XR_ML_marker_understanding"
    )]
#endif
    public partial class MagicLeapMarkerUnderstandingFeature : MagicLeapOpenXRFeatureBase
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_markerunderstanding";

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            if (!OpenXRRuntime.IsExtensionEnabled("XR_ML_marker_understanding"))
            {
                Debug.LogWarning($"XR_ML_marker_understanding is not enabled, disabling {nameof(MagicLeapMarkerUnderstandingFeature)}.");
                return false;
            }

            var instanceCreateResult = base.OnInstanceCreate(xrInstance);
            if (!instanceCreateResult)
            {
                return false;
            }

            nativeFunctions = CreateNativeFunctions<MarkerUnderstandingNativeFunctions>();
            return true;
        }

        protected override void OnSessionEnd(ulong xrSession)
        {
            base.OnSessionEnd(xrSession);
            DestroyAllMarkerDetectors();
        }
    }
}
