// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLSpace : MLAutoAPISingleton<MLSpace>
    {
        /// <summary>
        /// Start the API. 
        /// Permissions: com.magicleap.permission.SPACE_MANAGER (protection level: normal)
        /// </summary>
        protected override MLResult.Code StartAPI() => Instance.InternalMLSpacesStart();

        /// <summary>
        /// Stop the API.
        /// </summary>
        protected override MLResult.Code StopAPI() => Instance.InternalMLSpacesStop();

        /// <summary>
        /// Export an on device Magic Leap Space.
        /// Permissions: com.magicleap.permission.SPACE_IMPORT_EXPORT (protection level:dangerous).
        /// </summary>
        /// <param name="info">Information needed to export the space</param>
        /// <param name="data">Exported space data</param>
        /// <returns>
        /// MLResult.Code will be <c>MLResult.Code.Ok</c> if space was exported successfully.
        /// MLResult.Code will be <c>MLResult.Code.InvalidParam</c> if 1 or more input parameters are not valid.
        /// MLResult.Code will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
        /// MLResult.Code will be <c>MLResult.UnspecifiedFailure</c> if failed for unknown reason.
        /// MLResult.Code will be <c>MLResult.Code.UnavailableSpace</c> if failed due to unavailable space.
        /// </returns>
        public static MLResult.Code ExportSpace(in SpaceInfo info, out SpaceData data) => Instance.InternalExportSpace(in info, out data);

        /// <summary>
        /// Import a Magic Leap Space.
        /// Permissions: com.magicleap.permission.SPACE_IMPORT_EXPORT (protection level:dangerous).
        /// </summary>
        /// <param name="data">Information needed to import the space</param>
        /// <param name="id">Space data returned on successful import</param>
        /// <returns>
        /// MLResult.Code will be <c>MLResult.Code.Ok</c> if space was imported successfully.
        /// MLResult.Code will be <c>MLResult.Code.InvalidParam</c> if 1 or more input parameters are not valid.
        /// MLResult.Code will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
        /// MLResult.Code will be <c>MLResult.UnspecifiedFailure</c> if failed for unknown reason.
        /// MLResult.Code will be <c>MLResult.Code.IncompatibleSpace</c> if operation failed due to incompatible space.
        /// MLResult.Code will be <c>MLResult.Code.SpaceAlreadyExists</c> if space being imported already exists.
        /// </returns>
        public static MLResult.Code ImportSpace(in SpaceData data, out SpaceInfo id) => Instance.InternalImportSpace(in data, out id);

        /// <summary>
        /// Get the list of available spaces.
        /// The list of spaces returned will depend on the current device mapping mode.
        /// Only the Spaces associated with the current mapping mode will be returned by
        /// this call.Device mapping mode can be changed via the system application(s).
        /// Permissions: None.
        /// </summary>
        /// <param name="spaceList">List of spaces currently available to the device</param>
        /// <returns>
        /// MLResult.Code will be <c>MLResult.Code.Ok</c> if the list of spaces was retrieved successfully.
        /// MLResult.Code will be <c>MLResult.Code.InvalidParam</c> if input parameter is not valid.
        /// MLResult.Code will be <c>MLResult.Code.Unauthenticated</c> if authentication credentials invalid for this operation.
        /// MLResult.Code will be <c>MLResult.UnspecifiedFailure</c> if failed for unknown reason.
        /// MLResult.Code will be <c>MLResult.Code.SpacesServerError</c> if operation failed due to server-side error.
        /// MLResult.Code will be <c>MLResult.Code.SpacesServiceUnavailable</c> if operation failed due to service not being ready.
        /// </returns>
        public static MLResult.Code GetSpaceList(out Space[] spaceList) => Instance.InternalGetSpaceList(out spaceList);

        /// <summary>
        /// Send a request to localize to a given Magic Leap Space.
        /// This is an asynchronous request. Use <c>GetLocalizationResult</c> to get the results of 
        /// the localization, or add a listener to <c>OnLocalizationEvent</c>.
        /// Permissions: None.
        /// </summary>
        /// <param name="info">Information about Space to localize into</param>
        /// <returns>
        /// MLResult.Code will be <c>MLResult.Code.Ok</c> if the request was submitted successfully.
        /// MLResult.Code will be <c>MLResult.Code.InvalidParam</c> if input parameter is not valid.
        /// MLResult.Code will be <c>MLResult.Code.Unauthenticated</c> if authentication credentials invalid for this operation.
        /// MLResult.Code will be <c>MLResult.UnspecifiedFailure</c> if failed for unknown reason.
        /// MLResult.Code will be <c>MLResult.Code.SpacesServerError</c> if operation failed due to server-side error.
        /// MLResult.Code will be <c>MLResult.Code.SpacesServiceUnavailable</c> if operation failed due to service not being ready.
        /// </returns>
        public static MLResult.Code RequestLocalization(ref SpaceInfo info) => Instance.InternalRequestLocalization(ref info);

        /// <summary>
        /// Get the localization results. Returns the results of the most recent localization
        /// request.
        /// Permissions: None.
        /// </summary>
        /// <param name="result">Contains the result of the localization request</param>
        /// <returns>
        /// MLResult.Code will be <c>MLResult.Code.Ok</c> if the result was retrieved successfully.
        /// MLResult.Code will be <c>MLResult.Code.InvalidParam</c> if input parameter is not valid.
        /// MLResult.Code will be <c>MLResult.Code.Unauthenticated</c> if authentication credentials invalid for this operation.
        /// MLResult.Code will be <c>MLResult.UnspecifiedFailure</c> if failed for unknown reason.
        /// </returns>
        public static MLResult.Code GetLocalizationResult(out LocalizationResult result) => Instance.InternalGetLocalizationResult(out result);

        public delegate void MLSpaceDelegate(ref SpaceLocalizationResult result, IntPtr data);
        public delegate void OnLocalizationDelegate(LocalizationResult result);

        /// <summary>
        /// This callback will be invoked whenever there is an update to the localization status.
        /// Localization events can occur when the application requests for localization via 
        /// <see cref="RequestLocalization(ref SpaceInfo)"/> or due to other events outside the 
        /// control of the app such as head tracking failure, localization loss.
        /// </summary>
        public static event OnLocalizationDelegate OnLocalizationEvent
        {
            add => OnLocalizationChanged += value;
            remove => OnLocalizationChanged -= value;
        }
    }
}

