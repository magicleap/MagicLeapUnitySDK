// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLCamera.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
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
