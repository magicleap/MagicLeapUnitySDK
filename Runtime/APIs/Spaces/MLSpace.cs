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
        /// </summary>
        /// permissions com.magicleap.permission.SPACE_MANAGER (protection level: normal)
        protected override MLResult.Code StartAPI() => Instance.InternalMLSpacesStart();

        /// <summary>
        /// Stop the API.
        /// </summary>
        protected override MLResult.Code StopAPI() => Instance.InternalMLSpacesStop();

        /// <summary>
        /// Export an on device Magic Leap Space.
        /// </summary>
        public static MLResult.Code ExportSpace(in SpaceInfo info, out SpaceData data) => Instance.InternalExportSpace(in info, out data);

        /// <summary>
        /// Import a Magic Leap Space.
        /// </summary>
        public static MLResult.Code ImportSpace(in SpaceData data, out SpaceInfo id) => Instance.InternalImportSpace(in data, out id);

        /// <summary>
        /// Get the list of available spaces.
        /// The list of spaces returned will depend on the current device mapping mode.
        /// Only the Spaces associated with the current mapping mode will be returned by
        /// this call.Device mapping mode can be changed via the system application(s).
        /// </summary>
        public static MLResult.Code GetSpaceList(out Space[] spaceList) => Instance.InternalGetSpaceList(out spaceList);

        /// <summary>
        /// Requests the localization info.
        /// </summary>
        public static MLResult.Code RequestLocalization(ref SpaceInfo info) => Instance.InternalRequestLocalization(ref info);

        /// <summary>
        /// Requests the localization info.
        /// </summary>
        public static MLResult.Code GetLocalizationResult(out LocalizationResult result) => Instance.InternalGetLocalizationResult(out result);

        public delegate void MLSpaceDelegate(ref SpaceLocalizationResult result, IntPtr data);
        public delegate void OnLocalizationDelegate(LocalizationResult result);

        /// <summary>
        /// Event callback fired when a space event occurs.
        /// </summary>
        public static event OnLocalizationDelegate OnLocalizationEvent
        {
            add
            {
                OnLocalizationChanged += value;
            }
            remove
            {
                OnLocalizationChanged -= value;
            }
        }
    }
}

