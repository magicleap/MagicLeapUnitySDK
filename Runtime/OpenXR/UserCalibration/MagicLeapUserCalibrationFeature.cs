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
using System.Collections.Generic;
using UnityEngine.XR.MagicLeap;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif
using NativeBindings = UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapFeature.NativeBindings;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    #if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 User Calibration Support",
        Desc = "Necessary to deploy a Magic Leap 2 compatible application with User calibration events.",
        Company = "Magic Leap",
        Version = "1.0.0",
        Priority = -1,
        BuildTargetGroups = new[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        FeatureId = featureId,
        OpenxrExtensionStrings = "XR_ML_user_calibration"
    )]
    #endif
    public partial class MagicLeapUserCalibrationFeature : MagicLeapOpenXRFeatureBase
    {
        public const string featureId = "com.magicleap.openxr.feature.ml2_usercalibration";

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            if (!OpenXRRuntime.IsExtensionEnabled("XR_ML_user_calibration"))
            {
                Debug.LogWarning("XR_ML_user_calibration is not enabled, disabling MagicLeapUserCalibrationFeature.");
                return false;
            }

            return base.OnInstanceCreate(xrInstance);
        }

        public enum HeadsetFitStatus
        {
            Unknown = 0,
            NotWorn,
            GoodFit,
            BadFit,
        }

        public enum EyeCalibrationStatus
        {
            Unknown = 0,
            None,
            Coarse,
            Fine,
        }

        public struct HeadsetFitData
        {
            public HeadsetFitStatus status;

            public long time;
        };

        public struct EyeCalibrationData
        {
            public EyeCalibrationStatus status;
        }

        private HeadsetFitData storedHeadsetFit;
        private EyeCalibrationData storedEyeCalibration;

        public bool EnableUserCalibrationEvents(bool enable)
        {
            return NativeBindings.MLOpenXREnableUserCalibrationEvents(enable);
        }

        public bool GetLastHeadsetFit(out HeadsetFitData data)
        {
            HeadsetFitStatus headsetFitStatus = HeadsetFitStatus.Unknown;
            long headsetTime = 0;
            bool result = NativeBindings.MLOpenXRGetHeadsetFitData(ref headsetFitStatus, ref headsetTime);
            if (result)
            {
                storedHeadsetFit.status = headsetFitStatus;
                storedHeadsetFit.time = headsetTime;
            }
            data = storedHeadsetFit;
            return result;
        }

        public bool GetLastEyeCalibration(out EyeCalibrationData data)
        {
            EyeCalibrationStatus eyeCalibrationStatus = EyeCalibrationStatus.Unknown;
            bool result = NativeBindings.MLOpenXRGetEyeCalibrationData(ref eyeCalibrationStatus);
            if (result)
                storedEyeCalibration.status = eyeCalibrationStatus;
            
            data = storedEyeCalibration;
            return result;
        }

    }
}
#endif
