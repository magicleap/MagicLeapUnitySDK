// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLImageTracker.cs" company="Magic Leap, Inc">
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
    public partial class MLImageTracker
    {
        //Profiler markers for MLImageTracker Native Bindings calls

        private readonly static ProfilerMarker nativeMLImageTrackerInitSettingsPerfMarker = new ProfilerMarker("MLImageTrackerNativeBindings.MLImageTrackerInitSettings");
        private readonly static ProfilerMarker nativeMLImageTrackerCreatePerfMarker = new ProfilerMarker("MLImageTrackerNativeBindings.MLImageTrackerCreate");
        private readonly static ProfilerMarker nativeMLImageTrackerUpdateSettingsPerfMarker = new ProfilerMarker("MLImageTrackerNativeBindings.MLImageTrackerUpdateSettings");
        private readonly static ProfilerMarker nativeMLImageTrackerDestroyPerfMarker = new ProfilerMarker("MLImageTrackerNativeBindings.MLImageTrackerDestroy");
        private readonly static ProfilerMarker nativeMLImageTrackerAddTargetFromArrayPerfMarker = new ProfilerMarker("MLImageTrackerNativeBindings.MLImageTrackerAddTargetFromArray");
        private readonly static ProfilerMarker nativeMLImageTrackerRemoveTargetPerfMarker = new ProfilerMarker("MLImageTrackerNativeBindings.MLImageTrackerRemoveTarget");
        private readonly static ProfilerMarker nativeMLImageTrackerUpdateTargetSettingsPerfMarker = new ProfilerMarker("MLImageTrackerNativeBindings.MLImageTrackerUpdateTargetSettings");
        private readonly static ProfilerMarker nativeMLImageTrackerGetTargetStaticDataPerfMarker = new ProfilerMarker("MLImageTrackerNativeBindings.MLImageTrackerGetTargetStaticData");
        private readonly static ProfilerMarker nativeMLImageTrackerGetTargetResultPerfMarker = new ProfilerMarker("MLImageTrackerNativeBindings.MLImageTrackerGetTargetResult");

        //Profiler markers for MLImageTracker calls
        private readonly static ProfilerMarker getCurrentTrackerSettingsPerfMarker = new ProfilerMarker("MLImageTracker.GetCurrentTrackerSettings");
        private readonly static ProfilerMarker updateTrackerSettingsPerfMarker = new ProfilerMarker("MLImageTracker.UpdateTrackerSettings");
        private readonly static ProfilerMarker disablePerfMarker = new ProfilerMarker("MLImageTracker.Disable");
        private readonly static ProfilerMarker addTargetPerfMarker = new ProfilerMarker("MLImageTracker.AddTarget");
        private readonly static ProfilerMarker removeTargetPerfMarker = new ProfilerMarker("MLImageTracker.RemoveTarget");
        private readonly static ProfilerMarker startAPIPerfMarker = new ProfilerMarker("MLImageTracker.StartAPI");
        private readonly static ProfilerMarker stopAPIPerfMarker = new ProfilerMarker("MLImageTracker.StopAPI");
        private readonly static ProfilerMarker updatePerfMarker = new ProfilerMarker("MLImageTracker.Update");

        //Profiler markers for MLImageTrackerTarget calls

        private readonly static ProfilerMarker disposePerfMarker = new ProfilerMarker("MLImageTrackerTarget.Dispose");
        private readonly static ProfilerMarker updateTrackingDataPerfMarker = new ProfilerMarker("MLImageTrackerTarget.UpdateTrackingData");
        private readonly static ProfilerMarker setTargetLongerDimensionPerfMarker = new ProfilerMarker("MLImageTrackerTarget.SetTargetLongerDimension");

    }
}
