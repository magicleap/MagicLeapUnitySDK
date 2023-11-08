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
