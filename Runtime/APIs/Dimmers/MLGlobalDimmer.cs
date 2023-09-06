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
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport;
#endif

namespace UnityEngine.XR.MagicLeap
{
    public static partial class MLGlobalDimmer
    {
        /// <summary>
        /// Set the manual value for the global dimmer Updates are reflected in
        /// the next client submitted frame. Global dimmer update through this
        /// func is applicable only if auto dimmer is disabled. Any smooth ramping
        /// from auto dimmer to application final dimmer should be handled by the
        /// application itself. Since the dimmer value set through this func is
        /// reflected only in the next client submitted frame, the smooth ramp
        /// stepping interval is equal to current application frame rate
        /// </summary>
        /// <param name="dimmerValue">dimmer value in valid range [0.0 to 1.0].
        /// 0.0 corresponds no global dimming while 1.0 corresponds to max global
        /// dimming.</param>
        /// <param name="enabled">Enable or disable the global dimmer.</param>
#if UNITY_OPENXR_1_7_0_OR_NEWER
        public static MLResult.Code SetValue(float dimmerValue, bool enabled)
        {
            float clampedValue = Mathf.Clamp(dimmerValue, 0.0f, 1.0f);
            var dimmerInfoStruct = new NativeBindings.XrGlobalDimmerFrameEndInfoML(clampedValue, enabled);
            
            var resultCode = NativeBindings.MLOpenXRSetGlobalDimmerFrameEndInfoParams(dimmerInfoStruct);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLOpenXRSetGlobalDimmerFrameEndInfoParams));
            
            return resultCode;
        }
#else
        public static MLResult SetValue(float dimmerValue, bool enabled = true)
        {
            float clampedValue = Mathf.Clamp(dimmerValue, 0.0f, 1.0f);
            NativeBindings.UnityMagicLeap_RenderingSetGlobalDimmerValue(clampedValue);
            return MLResult.Create(MLResult.Code.Ok);
        }
#endif
    }
}
