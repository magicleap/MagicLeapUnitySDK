// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// MLHeadsetFit description goes here.
    /// </summary>
    public partial class MLHeadsetFit
    {
        /// <summary>
        /// See ml_headset_fit.h for additional comments.
        /// </summary>
        private class NativeBindings : Native.MagicLeapNativeBindings
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct MLHeadsetFitState
            {
                public uint Version;

                public Status FitStatus;

                public long EpochTimestampUs;

                public static MLHeadsetFitState Create() => new() { Version = 1 };
            };

            /// <summary>
            /// Destroys headset fit client.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLHeadsetFitDestroyClient(ulong handle);

            /// <summary>
            /// Creates a headset fit client.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLHeadsetFitCreateClient(out ulong handle);

            /// <summary>
            /// Gets information about the user's current headset fit.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLHeadsetFitGetState(ulong handle, out MLHeadsetFitState state);
        }
    }
}
