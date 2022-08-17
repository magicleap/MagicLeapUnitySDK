// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLMediaNativeBindings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if UNITY_MAGICLEAP || UNITY_ANDROID

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// MLMedia APIs.
    /// </summary>
    public sealed partial class MLMedia
    {
        /// <summary>
        /// General media native bindings.
        /// </summary>
        internal partial class NativeBindings : Native.MagicLeapNativeBindings
        {
            /// <summary>
            /// Gets the result string for a MLResult.Code.
            /// </summary>
            /// <param name="resultCode">The MLResult.Code to be requested.</param>
            /// <returns>The result string.</returns>
            public static string GetResultString(MLResult.Code resultCode)
            {
                IntPtr ptr = NativeBindings.MLMediaResultGetString(resultCode);
                return Marshal.PtrToStringAnsi(ptr);
            }

            /// <summary>
            /// Gets a readable version of the result code as an ASCII string.
            /// </summary>
            /// <param name="result">The MLResult.Code that should be converted.</param>
            /// <returns>ASCII string containing a readable version of the result code.</returns>
            [DllImport(MLMediaErrorDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaResultGetString(MLResult.Code result);
        }
    }
}

#endif
