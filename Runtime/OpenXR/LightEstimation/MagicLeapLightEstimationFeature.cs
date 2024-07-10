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

namespace MagicLeap.OpenXR.Features.LightEstimation
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Light Estimation",
        Desc = "Necessary to deploy a Magic Leap 2 compatible application with Light Estimation events.",
        Company = "Magic Leap",
        Version = "1.0.0",
        Priority = -1,
        BuildTargetGroups = new[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        FeatureId = FeatureId,
        OpenxrExtensionStrings = "XR_ML_light_estimation"
    )]
#endif
    public partial class MagicLeapLightEstimationFeature : MagicLeapOpenXRFeatureBase
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_lightestimation";
        public const string ExtensionName = "XR_ML_light_estimation";

        private LightEstimationNativeFunctions nativeFunctions;

        protected override bool UsesExperimentalExtensions => true;

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            if (OpenXRRuntime.IsExtensionEnabled(ExtensionName))
            {
                var result = base.OnInstanceCreate(xrInstance);
                if (result)
                {
                    nativeFunctions = NativeFunctionsBase.Create<LightEstimationNativeFunctions>(InstanceProcAddr, AppInstance);
                }
                return result;
            }
            Debug.LogError($"{ExtensionName} is not enabled. Disabling {nameof(MagicLeapLightEstimationFeature)}");
            return false;

        }
    }
}
