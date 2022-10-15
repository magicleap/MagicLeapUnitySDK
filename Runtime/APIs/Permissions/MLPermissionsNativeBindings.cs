// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if UNITY_MAGICLEAP || UNITY_ANDROID && UNITY_EDITOR

namespace UnityEngine.XR.MagicLeap
{
    using System.Runtime.InteropServices;

    public sealed partial class MLPermissions
    {
        /// <summary>
        /// Permissions NativeBindings are only used when running in the Editor with Magic Leap App Simulator
        /// </summary>
        private class NativeBindings : Native.MagicLeapNativeBindings
        {
            /// <summary>
            /// Prevents a default instance of the <see cref="NativeBindings"/> class from being created.
            /// </summary>
            private NativeBindings() { }

            /// <summary>
            /// Start the ZI permissions-checking subsystem. This function should be called
            /// before any permissions-checking functions are called while running in the Editor.
            /// </summary>
            /// <returns>
            /// <c>MLResult.Code.Ok</c> if the permission system startup succeeded.<br/><br/>
            /// <c>MLResult.Code.UnspecifiedFailure</c> if the permission system failed to startup.
            /// </returns>
            [DllImport(MLZIPermissionsDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLZIPermissionsStart();

            /// <summary>
            /// Stop the ZI permissions system. Call to clean up the ZI permission system at application termination.
            /// </summary>
            /// <returns>
            /// <c>MLResult.Code.OK</c> if the stop was successful.<br/><br/>
            /// <c>MLResult.Code.UnspecifiedFailure</c> if the permissions system failed to stop.
            /// </returns>
            [DllImport(MLZIPermissionsDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLZIPermissionsStop();

            /// <summary>
            /// Queries whther or not the application has been granted the given permission.<br/><br/>
            /// see https://developer.android.com/training/permissions/requesting
            /// </summary>
            /// <param name="permissionName">The name of the permission to query.</param>
            /// <returns>
            /// <c>MLResult.Code.InvalidParam</c> The permissionName parameter is not valid (or null)<br/><br/>
            /// <c>MLResult.Code.Ok</c> The permission is granted.<br/><br/>
            /// <c>MLResult.Code.PermissionDenied</c> The permission is denied.<br/><br/>
            /// <c>MLResult.Code.Pending</c> The permission request has not been resolved.<br/><br/>
            /// <c>MLResult.Code.UnspecifiedFailure</c> There was an unknown error.<br/><br/>
            /// </returns>
            [DllImport(MLZIPermissionsDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLZIPermissionsIsGranted(string permissionName);

            /// <summary>
            /// Request the given permission asynchronously (non-blocking).<br/>
            /// Client code can poll the permission state to see if the permission was granted, denied or still pending.<br/><br/>
            /// see https://developer.android.com/training/permissions/requesting <br/>
            /// </summary>
            /// <param name="permissionName">The name of the permission to request.</param>
            /// <returns>
            /// <c>MLResult.Code.InvalidParam</c> The permission_name parameter is not valid (or null).<br/><br/>
            /// <c>MLResult.Code.Ok</c> The query was successful.<br/><br/>
            /// <c>MLResult.Code.UnspecifiedFailure</c> There was an unknown error.<br/><br/>
            /// </returns>
            [DllImport(MLZIPermissionsDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLZIPermissionsRequest(string permissionName);
        }
    }
}

#endif
