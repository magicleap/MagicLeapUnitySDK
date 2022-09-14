// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using Unity.Profiling;

namespace UnityEngine.XR.MagicLeap
{
    public sealed partial class MLCVCamera
    {
        //Profiler markers for MLCVCamera Native Bindings calls

        private readonly static ProfilerMarker nativeMLCVCameraTrackingCreatePerfMarker = new ProfilerMarker("MLCVCameraNativeBindings.MLCVCameraTrackingCreate");
        private readonly static ProfilerMarker nativeMLCVCameraTrackingDestroyPerfMarker = new ProfilerMarker("MLCVCameraNativeBindings.MLCVCameraTrackingDestroy");

        //Profiler markers for MLCVCamera calls

        private readonly static ProfilerMarker getFramePosePerfMarker = new ProfilerMarker("MLCVCameraNativeBindings.GetFramePose");

    }

}
