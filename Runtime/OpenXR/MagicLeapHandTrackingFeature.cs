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
using System.Collections.Generic;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.Hands;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif
using NativeBindings = UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapFeature.NativeBindings;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    /// <summary>
    /// Necessary to deploy a Magic Leap 2 compatible application with Hand Tracking.
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 HandTracking Support",
        Desc = "Necessary to deploy a Magic Leap 2 compatible application with Hand Tracking.",
        Company = "Magic Leap",
        Version = "1.0.0",
        Priority = -1,
        BuildTargetGroups = new[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        FeatureId = featureId,
        OpenxrExtensionStrings = "XR_EXT_hand_tracking "
    )]
#endif
    public class MagicLeapHandTrackingFeature : MagicLeapOpenXRFeatureBase
    {
        /// <summary>
        /// The feature id string. This is used to give the feature a well known id for reference.
        /// </summary>
        public const string featureId = "com.magicleap.openxr.feature.ml2_handtracking";

        private static List<XRHandSubsystemDescriptor> s_HandSubsysDesc = new List<XRHandSubsystemDescriptor>();

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            if (!OpenXRRuntime.IsExtensionEnabled("XR_EXT_hand_tracking"))
            {
                Debug.LogWarning("XR_EXT_hand_tracking is not enabled, disabling MagicLeapHandTrackingFeature.");
                return false;
            }

            return base.OnInstanceCreate(xrInstance);
        }


        protected override void OnSessionCreate(ulong xrSession)
        {
            NativeBindings.MLOpenXRStartXRHandTracking();
        }

        protected override void OnSessionDestroy(ulong xrSession)
        {
            NativeBindings.MLOpenXRStopXRHandTracking();
        }

        protected override void OnSubsystemCreate()
        {

            base.OnSubsystemCreate();

            CreateSubsystem<XRHandSubsystemDescriptor, HandSubsystem>(s_HandSubsysDesc, MagicLeapXrProvider.HandSubsystemId);
        }

        protected override void OnSubsystemStart()
        {
            base.OnSubsystemStart();
            StartSubsystem<HandSubsystem>();
        }

        protected override void OnSubsystemStop()
        {
            base.OnSubsystemStop();

            StopSubsystem<HandSubsystem>();
        }

        protected override void OnSubsystemDestroy()
        {
            base.OnSubsystemDestroy();

            DestroySubsystem<HandSubsystem>();
        }
    }
}
#endif
