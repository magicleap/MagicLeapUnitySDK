// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLPermissions.cs" company="Magic Leap, Inc">
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
    public sealed partial class MLPermissions
    {
        //Profiler markers for MLPermissions Native Bindings calls

        private readonly static ProfilerMarker nativeMLPermissionsCheckPermissionPerfMarker = new ProfilerMarker("MLPermissionsNativeBindings.MLPermissionsCheckPermission");
        private readonly static ProfilerMarker nativeMLPermissionsRequestPermissionPerfMarker = new ProfilerMarker("MLPermissionsNativeBindings.MLPermissionsRequestPermission");
        private readonly static ProfilerMarker nativeMLPermissionsRequestPermissionAsyncPerfMarker = new ProfilerMarker("MLPermissionsNativeBindings.MLPermissionsRequestPermissionAsync");
        private readonly static ProfilerMarker nativeMLPermissionsRequestPermissionTryGetPerfMarker = new ProfilerMarker("MLPermissionsNativeBindings.MLPermissionsRequestPermissionTryGet");
        private readonly static ProfilerMarker nativeMLPermissionsGetResultStringPerfMarker = new ProfilerMarker("MLPermissionsNativeBindings.MLPermissionsGetResultString");

        //Profiler markers for MLPermissions calls
        private readonly static ProfilerMarker checkPermissionPerfMarker = new ProfilerMarker("MLPermissions.CheckPermission");
    }

}
