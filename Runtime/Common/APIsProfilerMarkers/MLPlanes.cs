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
    public partial class MLPlanes
    {
        //Profiler markers for MLPlanes Native Bindings calls
        private readonly static ProfilerMarker nativeCreatePerfMarker = new ProfilerMarker("MLPlanesNativeBindings.MLPlanesCreate");
        private readonly static ProfilerMarker nativeQueryBeginPerfMarker = new ProfilerMarker("MLPlanesNativeBindings.MLPlanesQueryBegin");
        private readonly static ProfilerMarker nativeDestroyPerfMarker = new ProfilerMarker("MLPlanesNativeBindings.MLPlanesDestroy");
        private readonly static ProfilerMarker nativeQueryGetResultsPerfMarker = new ProfilerMarker("MLPlanesNativeBindings.MLPlanesQueryGetResultsWithBoundaries");
        private readonly static ProfilerMarker nativeReleaseBoundariesPerfMarker = new ProfilerMarker("MLPlanesNativeBindings.MLPlanesReleaseBoundariesList");

        //Profiler markers for MLPlanes calls.
        private readonly static ProfilerMarker getPlanesPerfMarker = new ProfilerMarker("MLPlanes.GetPlanes");
        private readonly static ProfilerMarker startAPIPerfMarker = new ProfilerMarker("MLPlanes.StartAPI");
        private readonly static ProfilerMarker stopAPIPerfMarker = new ProfilerMarker("MLPlanes.StopAPI");
        private readonly static ProfilerMarker updatePerfMarker = new ProfilerMarker("MLPlanes.Update");
    }
}
