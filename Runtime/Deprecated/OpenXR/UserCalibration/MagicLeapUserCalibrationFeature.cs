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
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapOpenXRNativeDelegates;
using UnityEngine.XR.OpenXR.NativeTypes;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    using MagicLeapUserCalibrationNativeTypes;
    using MagicLeapOpenXRFeatureNativeTypes;
    
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.UserCalibration")]
    public class MagicLeapUserCalibrationFeature : MagicLeapOpenXRFeatureWithInterception<MagicLeapUserCalibrationFeature>
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_usercalibration";
        
        private MagicLeapUserCalibrationNativeFunctions nativeFunctions;

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

            nativeFunctions = CreateNativeFunctions<MagicLeapUserCalibrationNativeFunctions>();
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
                var eventBufferPtr = (XrEventBuffer*)eventBuffer;
                switch (eventBufferPtr->Type)
                {
                    case (ulong)XrUserCalibrationStructTypes.EventDataHeadsetFitChanged:
                    {
                        headsetFit = *(XrEventDataHeadsetFitChanged*)eventBuffer;
                        break;
                    }

                    case (ulong)XrUserCalibrationStructTypes.EventDataEyeCalibrationChanged:
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
