// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLRaycast.cs" company="Magic Leap, Inc">
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
    public partial class MLRaycast
    {
        //Profiler markers for MLRaycast Native Bindings calls

        private readonly static ProfilerMarker nativeMLRaycastCreatePerfMarker = new ProfilerMarker("MLRaycastNativeBindings.MLRaycastCreate");
        private readonly static ProfilerMarker nativeMLRaycastRequestPerfMarker = new ProfilerMarker("MLRaycastNativeBindings.MLRaycastRequest");
        private readonly static ProfilerMarker nativeMLRaycastGetResultPerfMarker = new ProfilerMarker("MLRaycastNativeBindings.MLRaycastGetResult");
        private readonly static ProfilerMarker nativeMLRaycastDestroyPerfMarker = new ProfilerMarker("MLRaycastNativeBindings.MLRaycastDestroy");

        //Profiler markers for MLRaycast calls

        private readonly static ProfilerMarker raycastPerfMarker = new ProfilerMarker("MLRaycast.Raycast");
        private readonly static ProfilerMarker startAPIPerfMarker = new ProfilerMarker("MLRaycast.StartAPI");
        private readonly static ProfilerMarker updatePerfMarker = new ProfilerMarker("MLRaycast.Update");
        private readonly static ProfilerMarker stopAPIPerfMarker = new ProfilerMarker("MLRaycast.StopAPI");


    }
}
