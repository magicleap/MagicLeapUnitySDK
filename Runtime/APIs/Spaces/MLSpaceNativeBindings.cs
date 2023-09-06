// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2023) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;

    public partial class MLSpace
    {
        /// <summary>
        /// See ml_space.h for additional comments.
        /// </summary>
        public class NativeBindings : Native.MagicLeapNativeBindings
        {
            /// <summary>
            /// Creates a Magic Leap Space manager handle.
            /// </summary>
            [DllImport(MLSpaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpaceManagerCreate(in Settings settings, out ulong handle);

            /// <summary>
            /// Set the callbacks for events related to the Magic Leap Space.
            /// </summary>
            [DllImport(MLSpaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpaceSetCallbacks(ulong handle, ref SpaceCallbacks callbacks, IntPtr userData );

            /// <summary>
            /// The list memory is owned by the library, call #MLSpaceReleaseSpaceList to
            /// release the memory. Each get #MLSpaceGetSpaceList should have a corresponding
            /// #MLSpaceReleaseSpaceList.
            /// </summary>
            [DllImport(MLSpaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpaceGetSpaceList(ulong hanlde, out SpaceFilter queryFilter, out SpaceList spaceList);

            /// <summary>
            /// Release the list of available spaces.
            /// </summary>
            [DllImport(MLSpaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpaceReleaseSpaceList(ulong handle, out SpaceList list);
            
            /// <summary>
            /// Send a request to localize to a given Magic Leap Space.
            /// </summary>
            [DllImport(MLSpaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpaceRequestLocalization(ulong handle, ref SpaceLocalizationInfo spaceList);

            /// <summary>
            /// Get the localization results.
            /// </summary>
            [DllImport(MLSpaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpaceGetLocalizationResult(ulong handle, out SpaceLocalizationResult result);
            
            /// <summary>
            /// Destroys a Space manager handle.
            /// </summary>
            [DllImport(MLSpaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpaceManagerDestroy(ulong handle);

            /// <summary>
            /// The #MLSpaceImportInfo memory is owned by the app and the app should make sure
            /// to release the memory once the API call has returned
            /// </summary>
            [DllImport(MLSpaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpaceImportSpace(in SpaceImportInfo importInfo, ref SpaceImportOutData data);

            /// <summary>
            /// Export an on device Magic Leap Space.
            /// </summary>
            [DllImport(MLSpaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpaceExportSpace(in SpaceExportInfo exportInfo, ref SpaceExportOutData data);

            /// <summary>
            /// Release resources acquired in #MLSpaceExportSpace.
            /// </summary>
            [DllImport(MLSpaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpaceReleaseExportData(ref SpaceExportOutData data);
            
        }
    }
}

