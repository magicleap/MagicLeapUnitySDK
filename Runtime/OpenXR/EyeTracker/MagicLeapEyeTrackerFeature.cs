// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace MagicLeap.OpenXR.Features.EyeTracker
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Eye Tracker",
        Desc = "Necessary to deploy a Magic Leap 2 compatible application with Eye Tracker events.",
        Company = "Magic Leap",
        Version = "1.0.0",
        Priority = -1,
        BuildTargetGroups = new[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        FeatureId = FeatureId,
        OpenxrExtensionStrings = "XR_ML_eye_tracker"
    )]
#endif
    public partial class MagicLeapEyeTrackerFeature : MagicLeapOpenXRFeatureBase
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_eyetracker";
        public const string ExtensionName = "XR_ML_eye_tracker";

        [Obsolete("DeviceLocalizedName is no longer applicable since MagicLeapEyeTrackerFeature is no longer an interaction profile. Therefore it is no longer an InputDevice and Eye Tracking Data must be received through MagicLeapEyeTrackerFeature.GetEyeTrackerData().")]
        public const string DeviceLocalizedName = "Magic Leap Eye Tracker OpenXR";

        private EyeTrackerNativeFunctions nativeFunctions;

        protected override bool UsesExperimentalExtensions => true;

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            if (OpenXRRuntime.IsExtensionEnabled(ExtensionName))
            {
                var result = base.OnInstanceCreate(xrInstance);
                if (result)
                {
                    nativeFunctions = CreateNativeFunctions<EyeTrackerNativeFunctions>();
                }

                return result;
            }

            Debug.LogError($"{ExtensionName} is not enabled. Disabling {nameof(MagicLeapEyeTrackerFeature)}");
            return false;
        }
    }
    
    [Obsolete("EyeTrackerUsages is no longer applicable since MagicLeapEyeTrackerFeature is no longer an interaction profile. Therefore it is no longer an InputDevice and Eye Tracking Data must be received through MagicLeapEyeTrackerFeature.GetEyeTrackerData().")]
    public static class EyeTrackerUsages
    {
        public static InputFeatureUsage<Vector3> gazePosition = new InputFeatureUsage<Vector3>("gazePosition");
        public static InputFeatureUsage<Quaternion> gazeRotation = new InputFeatureUsage<Quaternion>("gazeRotation");
        public static InputFeatureUsage<Vector3> leftPosition = new InputFeatureUsage<Vector3>("leftPosition");
        public static InputFeatureUsage<Quaternion> leftRotation = new InputFeatureUsage<Quaternion>("leftRotation");
        public static InputFeatureUsage<Vector3> rightPosition = new InputFeatureUsage<Vector3>("rightPosition");
        public static InputFeatureUsage<Quaternion> rightRotation = new InputFeatureUsage<Quaternion>("rightRotation");
        public static InputFeatureUsage<Vector3> vergencePosition = new InputFeatureUsage<Vector3>("vergencePosition");
        public static InputFeatureUsage<Quaternion> vergenceRotation = new InputFeatureUsage<Quaternion>("vergenceRotation");
    }
}
