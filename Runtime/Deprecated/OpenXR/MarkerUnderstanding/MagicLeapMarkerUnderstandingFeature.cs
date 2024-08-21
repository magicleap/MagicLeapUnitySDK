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

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MarkerUnderstanding")]
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

            nativeFunctions = CreateNativeFunctions<MagicLeapMarkerUnderstandingNativeFunctions>();
            return true;
        }

        protected override void OnSessionEnd(ulong xrSession)
        {
            base.OnSessionEnd(xrSession);
            DestroyAllMarkerDetectors();
        }
    }
}
