// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLEyeCalibrationNativeBindings.cs" company="Magic Leap, Inc">
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
    /// MLEyeCalibration description goes here.
    /// </summary>
    public partial class MLEyeCalibration
    {
        /// <summary>
        /// See ml_eye_calibration.h for additional comments.
        /// </summary>
        private class NativeBindings : Native.MagicLeapNativeBindings
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct MLEyeCalibrationState
            {
                public uint Version;

                public Status EyeCalibration;

                public long EpochTimestampUs;

                public static MLEyeCalibrationState Create() => new() { Version = 1 };
            };

            /// <summary>
            /// Creates an eye calibration client.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLEyeCalibrationCreateClient(out ulong OutHandle);

            /// <summary>
            /// Destroys an eye calibration client.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLEyeCalibrationDestroyClient(ulong Handle);

            /// <summary>
            /// Gets information about the user's current eye calibration.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLEyeCalibrationGetState(ulong EyeCalibrationTracker, out MLEyeCalibrationState OutState);
        }
    }
}
