// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2023) Magic Leap, Inc. All Rights Reserved.
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
    public partial class MagicLeapFacialExpressionFeature : MagicLeapOpenXRFeatureBase
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_facialexpression";
        
        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            if (!OpenXRRuntime.IsExtensionEnabled("XR_ML_facial_expression"))
            {
                Debug.LogWarning($"XR_ML_facial_expression is not enabled, disabling {nameof(MagicLeapFacialExpressionFeature)}.");
                return false;
            }

            var instanceCreateResult = base.OnInstanceCreate(xrInstance);
            if (!instanceCreateResult)
            {
                return false;
            }
            nativeFunctions = CreateNativeFunctions<MagicLeapFacialExpressionNativeFunctions>();
            return true;
        }

        protected override void OnSessionDestroy(ulong xrSession)
        {
            DestroyClient();
            base.OnSessionDestroy(xrSession);
        }
    }
}
