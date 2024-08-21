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
using MagicLeap.OpenXR.SystemInfo;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.NativeTypes;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace MagicLeap.OpenXR.Features.SystemNotifications
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
    public class MagicLeapSystemNotificationsFeature : MagicLeapOpenXRFeatureBase
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_systemnotification";
        private const string ExtensionName = "XR_ML_system_notifications";

        private SystemNotificationNativeFunctions systemNotificationNativeFunctions;
        private SystemInfoNativeFunctions systemInfoNativeFunctions;
        private XrSystemId systemId;

        protected override bool UsesExperimentalExtensions => true;

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            if (OpenXRRuntime.IsExtensionEnabled(ExtensionName))
            {
                var instanceResult = base.OnInstanceCreate(xrInstance);
                if (instanceResult)
                {
                    systemNotificationNativeFunctions = CreateNativeFunctions<SystemNotificationNativeFunctions>();
                    systemInfoNativeFunctions = CreateNativeFunctions<SystemInfoNativeFunctions>();
                    return true;
                }
            }
            Debug.LogError($"{ExtensionName} is not enabled. Disabling {nameof(MagicLeapSystemNotificationsFeature)}");
            return false;
        }
        
        public XrResult SuppressSystemNotifications(bool suppressNotifications)
        {
            unsafe
            {
                if (!IsSystemNotificationSupported())
                {
                    Debug.LogError("Cannot suppress notifications. Either SKU is not medical or System Notification feature not enabled.");
                    return XrResult.RuntimeFailure;
                }

                var enableInfo = new XrSystemNotificationSetInfo
                {
                    Type = XrSystemNotificationStructTypes.XrTypeSystemNotificationsSetInfo,
                    Next = default,
                    SuppressNotifications = suppressNotifications ? 1 : 0U
                };
            
                var xrResult = systemNotificationNativeFunctions.XrSetSystemNotifications(AppInstance, in enableInfo);
                Utils.DidXrCallSucceed(xrResult, nameof(systemNotificationNativeFunctions.XrSetSystemNotifications));
                return xrResult;
            }
        }

        private XrResult GetSystemNotificationProperties(out ulong flags)
        {
            flags = 0;
            unsafe
            {
                if (systemId == default)
                {
                    var systemResult = systemInfoNativeFunctions.GetSystemId(out systemId);
                    if (!Utils.DidXrCallSucceed(systemResult, nameof(systemInfoNativeFunctions.XrGetSystem)))
                    {
                        return systemResult;
                    }
                }
                var systemProperties = new XrSystemProperties
                {
                    Type = XrSystemInfoTypes.XrTypeSystemProperties,
                };

                var systemNotificationProperties = new XrSystemNotificationProperties
                {
                    Type = XrSystemNotificationStructTypes.XrTypeSystemNotificationsProperties,
                };
                systemProperties.Next = new IntPtr(&systemNotificationProperties);
                var xrResult = systemInfoNativeFunctions.XrGetSystemProperties(AppInstance, systemId, out systemProperties);
                if(!Utils.DidXrCallSucceed(xrResult, nameof(systemInfoNativeFunctions.XrGetSystemProperties)))
                {
                    return xrResult;
                }
                flags = (ulong)systemNotificationProperties.SupportedFeatures;
                return xrResult;
            }
        }

        private bool IsSystemNotificationSupported()
        {
            var result = GetSystemNotificationProperties(out var flags);
            if (!Utils.DidXrCallSucceed(result, nameof(GetSystemNotificationProperties)))
            {
                return false;
            }
            return (flags & (ulong)XrSystemNotificationsCapabilityFlags.XrSystemNotificationCapabilitySuppression) > 0;
        }
    }
}
