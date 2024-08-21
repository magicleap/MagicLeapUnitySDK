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
using MagicLeap.OpenXR.NativeDelegates;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.NativeTypes;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace MagicLeap.OpenXR.Features.UserCalibration
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 User Calibration",
        Desc = "Necessary to deploy a Magic Leap 2 compatible application with User calibration events.",
        Company = "Magic Leap",
        Version = "1.0.0",
        Priority = -1,
        BuildTargetGroups = new[] { BuildTargetGroup.Android },
        FeatureId = FeatureId,
        OpenxrExtensionStrings = "XR_ML_user_calibration"
    )]
#endif
    public class MagicLeapUserCalibrationFeature : MagicLeapOpenXRFeatureWithInterception<MagicLeapUserCalibrationFeature>
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_usercalibration";
        
        private UserCalibrationNativeFunctions nativeFunctions;

        private XrEventDataHeadsetFitChanged headsetFit;
        private XrEventDataEyeCalibrationChanged eyeCalibration;

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

#pragma warning disable 618
        public struct HeadsetFitData
        {
            [Obsolete("HeadsetFitData.status will be deprecated. Use Status instead")]
            public HeadsetFitStatus status;
            [Obsolete("HeadsetFitData.time will be deprecated. Use Time instead")]
            public ulong time;

            public HeadsetFitStatus Status
            {
                get => status;
                set => status = value;
            }

            public long Time
            {
                get => (long)time;
                set => time = (ulong)value;
            }
        };

        public struct EyeCalibrationData
        {
            [Obsolete("EyeCalibrationData.status will be deprecated. Use Status instead")]
            public EyeCalibrationStatus status;

            public EyeCalibrationStatus Status
            {
                get => status;
                set => status = value;
            }
        }
#pragma warning restore 618

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            if (!OpenXRRuntime.IsExtensionEnabled("XR_ML_user_calibration"))
            {
                Debug.LogWarning("XR_ML_user_calibration is not enabled, disabling MagicLeapUserCalibrationFeature.");
                return false;
            }

            var instanceCreateResult = base.OnInstanceCreate(xrInstance);
            if (!instanceCreateResult)
            {
                return false;
            }

            nativeFunctions = CreateNativeFunctions<UserCalibrationNativeFunctions>();
            return true;
        }

        protected override void MarkFunctionsToIntercept()
        {
            InterceptPollEvent = true;
        }

        internal override XrResult OnPollEvent(ulong instance, IntPtr eventBuffer, XrPollEvent origPollEvent)
        {
            var result = base.OnPollEvent(instance, eventBuffer, origPollEvent);
            if (result != XrResult.Success)
            {
                return result;
            }
            unsafe
            {
                var eventBufferPtr = (XrEventDataBuffer*)eventBuffer;
                switch ((uint)eventBufferPtr->Type)
                {
                    case (uint)XrUserCalibrationStructTypes.EventDataHeadsetFitChanged:
                        {
                            headsetFit = *(XrEventDataHeadsetFitChanged*)eventBuffer;
                            break;
                        }

                    case (uint)XrUserCalibrationStructTypes.EventDataEyeCalibrationChanged:
                        {
                            eyeCalibration = *(XrEventDataEyeCalibrationChanged*)eventBuffer;
                            break;
                        }
                }
            }

            return result;
        }

        public bool EnableUserCalibrationEvents(bool enable)
        {
            unsafe
            {
                var calibrationEventInfo = new XrUserCalibrationEnableEventsInfo
                {
                    Type = XrUserCalibrationStructTypes.UserCalibrationEnableEventsInfo,
                    Enabled = enable ? 1U : 0,
                };
                var xrResult = nativeFunctions.XrEnableUserCalibrationEvents(AppInstance, in calibrationEventInfo);
                return Utils.DidXrCallSucceed(xrResult, nameof(nativeFunctions.XrEnableUserCalibrationEvents));
            }
        }

        public bool GetLastHeadsetFit(out HeadsetFitData data)
        {
            data = default;
            if (headsetFit.Type != XrUserCalibrationStructTypes.EventDataHeadsetFitChanged)
            {
                return false;
            }

            data.Status = (HeadsetFitStatus)headsetFit.Status;
            data.Time = headsetFit.Time;
            return true;
        }

        public bool GetLastEyeCalibration(out EyeCalibrationData data)
        {
            data = default;
            if (eyeCalibration.Type != XrUserCalibrationStructTypes.EventDataEyeCalibrationChanged)
            {
                return false;
            }

            data.Status = (EyeCalibrationStatus)eyeCalibration.Status;
            return true;
        }

    }
}
