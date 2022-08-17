// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLHeadsetFitNativeBindings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
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
