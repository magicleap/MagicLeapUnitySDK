// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLLightingTracking.cs" company="Magic Leap, Inc">
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
    public sealed partial class MLLightingTracking
    {
        //Profiler markers for MLLightingTracking Native Bindings calls

        private readonly static ProfilerMarker nativeMLLightingTrackingCreatePerfMarker = new ProfilerMarker("MLLightingTrackingNativeBindings.MLLightingTrackingCreate");
        private readonly static ProfilerMarker nativeMLLightingTrackingDestroyPerfMarker = new ProfilerMarker("MLLightingTrackingNativeBindings.MLLightingTrackingDestroy");
        private readonly static ProfilerMarker nativeMLLightingTrackingGetColorTemperatureStatePerfMarker = new ProfilerMarker("MLLightingTrackingNativeBindings.MLLightingTrackingGetColorTemperatureState");

        //Profiler markers for MLLightingTracking calls
        private readonly static ProfilerMarker startAPIPerfMarker = new ProfilerMarker("MLLightingTracking.StartAPI");
        private readonly static ProfilerMarker cleanupAPIPerfMarker = new ProfilerMarker("MLLightingTracking.CleanupAPI");
        private readonly static ProfilerMarker updatePerfMarker = new ProfilerMarker("MLLightingTracking.Update");

    }
}
