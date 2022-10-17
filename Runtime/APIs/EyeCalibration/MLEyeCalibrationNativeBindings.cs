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
            public static extern MLResult.Code MLEyeCalibrationGetState(ulong Handle, out MLEyeCalibrationState OutState);
        }
    }
}
