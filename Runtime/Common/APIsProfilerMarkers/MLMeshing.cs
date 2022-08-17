// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLMeshing.cs" company="Magic Leap, Inc">
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
    /// <summary>
    /// Performance markers for MLMeshing.
    /// </summary>
    public sealed partial class MLMeshing
    {
        //Performance markers for MLMeshing Native Bindings calls

        private readonly static ProfilerMarker nativeMLMeshingCreateClientPerfMarker = new ProfilerMarker("MLMeshingNativeBindings.MLMeshingCreateClient");
        private readonly static ProfilerMarker nativeMLMeshingDestroyClientPerfMarker = new ProfilerMarker("MLMeshingNativeBindings.MLMeshingDestroyClient");
        private readonly static ProfilerMarker nativeMLMeshingInitSettingsPerfMarker = new ProfilerMarker("MLMeshingNativeBindings.MLMeshingInitSettings");
        private readonly static ProfilerMarker nativeMLMeshingUpdateSettingsPerfMarker = new ProfilerMarker("MLMeshingNativeBindings.MLMeshingUpdateSettings");
        private readonly static ProfilerMarker nativeMLMeshingRequestMeshInfoPerfMarker = new ProfilerMarker("MLMeshingNativeBindings.MLMeshingRequestMeshInfo");
        private readonly static ProfilerMarker nativeMLMeshingGetMeshInfoResultPerfMarker = new ProfilerMarker("MLMeshingNativeBindings.MLMeshingGetMeshInfoResult");
        private readonly static ProfilerMarker nativeMLMeshingRequestMeshPerfMarker = new ProfilerMarker("MLMeshingNativeBindings.MLMeshingRequestMesh");
        private readonly static ProfilerMarker nativeMLMeshingGetMeshResultPerfMarker = new ProfilerMarker("MLMeshingNativeBindings.MLMeshingGetMeshResult");
        private readonly static ProfilerMarker nativeMLMeshingFreeResourcePerfMarker = new ProfilerMarker("MLMeshingNativeBindings.MLMeshingFreeResource");

        //Performance markers for MLMeshing calls

        private readonly static ProfilerMarker requestMeshInfoPerfMarker = new ProfilerMarker("MLMeshing.RequestMeshInfo");
        private readonly static ProfilerMarker getMeshInfoResultsPerfMarker = new ProfilerMarker("MLMeshing.GetMeshInfoResults");
        private readonly static ProfilerMarker requestMeshPerfMarker = new ProfilerMarker("MLMeshing.RequestMesh");
        private readonly static ProfilerMarker updatePerfMarker = new ProfilerMarker("MLMeshing.Update");

    }
}
