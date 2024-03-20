// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
#if UNITY_OPENXR_1_9_0_OR_NEWER
using UnityEngine.XR.OpenXR.NativeTypes;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif // UNITY_EDITOR

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 System Notification Control",
        Desc = "Enable/Disable the suppression of system notifications.",
        Company = "Magic Leap",
        Version = "1.0.0",
        BuildTargetGroups = new[] { BuildTargetGroup.Android },
        FeatureId = FeatureId,
        OpenxrExtensionStrings = ExtensionName
    )]
#endif // UNITY_EDITOR
    public partial class MagicLeapSystemNotificationsFeature : MagicLeapOpenXRFeatureBase
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_systemnotification";
        public const string ExtensionName = "XR_ML_system_notifications";

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            if (OpenXRRuntime.IsExtensionEnabled(ExtensionName))
            {
                return base.OnInstanceCreate(xrInstance);
            }
            Debug.LogError($"{ExtensionName} is not enabled. Disabling {nameof(MagicLeapSystemNotificationsFeature)}");
            return false;
        }

        protected override string GetFeatureId() => FeatureId;

        public XrResult SuppressSystemNotifications(bool suppressNotifications)
        {
            if (!IsSystemoNotificationSpported())
            {
                Debug.LogError("Cannot suppress notifications. Either SKU is not medical or System Notification feature not enabled.");
                return XrResult.RuntimeFailure;
            }
            return NativeBindings.MLOpenXRSuppressSystemNotifications(suppressNotifications);
        }

        private XrResult GetSystemNotificationProperties(out ulong flags)
        {
            return NativeBindings.MLOpenXRGetSystemNotificationsProperties(out flags);
        }

        private bool IsSystemoNotificationSpported()
        {
            return NativeBindings.MLOpenXRIsSystemoNotificationSpported();
        }
    }
}
#endif // UNITY_OPENXR_1_9_0_OR_NEWER
