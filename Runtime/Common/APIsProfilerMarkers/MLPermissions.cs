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
