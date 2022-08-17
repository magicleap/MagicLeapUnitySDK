// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLHandMeshing.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if UNITY_MAGICLEAP || UNITY_ANDROID

using Unity.Profiling;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLHandMeshing
    {
        //Profiler markers for MLHandMeshing Native Bindings calls

        private readonly static ProfilerMarker nativeMLHandMeshingCreateClientPerfMarker = new ProfilerMarker("MLHandMeshingNativeBindings.MLHandMeshingCreateClient");
        private readonly static ProfilerMarker nativeMLHandMeshingDestroyClientPerfMarker = new ProfilerMarker("MLHandMeshingNativeBindings.MLHandMeshingDestroyClient");
        private readonly static ProfilerMarker nativeMLHandMeshingRequestMeshPerfMarker = new ProfilerMarker("MLHandMeshingNativeBindings.MLHandMeshingRequestMesh");
        private readonly static ProfilerMarker nativeMLHandMeshingGetResultPerfMarker = new ProfilerMarker("MLHandMeshingNativeBindings.MLHandMeshingGetResult");
        private readonly static ProfilerMarker nativeMLHandMeshingFreeResourcePerfMarker = new ProfilerMarker("MLHandMeshingNativeBindings.MLHandMeshingFreeResource");

        //Profiler markers for MLHandMeshing calls

        private readonly static ProfilerMarker requestHandMeshPerfMarker = new ProfilerMarker("MLHandMeshing.RequestHandMesh");
        private readonly static ProfilerMarker startAPIPerfMarker = new ProfilerMarker("MLHandMeshing.StartAPI");
        private readonly static ProfilerMarker cleanupAPIPerfMarker = new ProfilerMarker("MLHandMeshing.CleanupAPI");
        private readonly static ProfilerMarker updatePerfMarker = new ProfilerMarker("MLHandMeshing.Update");

    }
}

#endif
