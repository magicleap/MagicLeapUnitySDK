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
