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
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Copy of android_LogPriority from android/log.h.
        /// </summary>
        public enum AndroidLogPriority : uint
        {
            Unkown = 0,
            Default,
            Verbose,
            Debug,
            Info,
            Warn,
            Error,
            Fatal,
            Silent,
        }

        /// <summary>
        /// Native bindings for the MLWebRTC class. 
        /// </summary>
        internal class NativeBindings : MagicLeapNativeBindings
        {
            /// <summary>
            /// Delegate describing the callback necessary to monitor when an debug message is received.
            /// </summary>
            /// <param name="logLevel">The log level of the debug utils.</param>
            /// <param name="message">The debug message.</param>
            /// <param name="context">Pointer to a context object.</param>
            public delegate void OnDebugMessageDelegate(AndroidLogPriority logLevel, [MarshalAs(UnmanagedType.LPStr)] string message, IntPtr context);

            /// <summary>
            /// Creates the MLWebRTC instance.
            /// </summary>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the instance was successfully created.
            /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
            /// </returns>
            [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebRTCInstanceCreate();

            /// <summary>
            /// Creates the MLWebRTC instance with more debug logs.
            /// </summary>
            /// <param name="debugUtils">The MLWebRTCDebugUtils object to initialize with.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the instance was successfully created.
            /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
            /// </returns>
            [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebRTCInstanceCreateWithDebugUtils(in MLWebRTCDebugUtils debugUtils);

            /// <summary>
            /// Destroys the MLWebRTC instance.
            /// </summary>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the instance was successfully destroyed.
            /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
            /// </returns>
            [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebRTCInstanceDestroy();

            /// <summary>
            /// Gets the string value of MLWebRTC specific result codes.
            /// </summary>
            /// <param name="result">The MLWebRTC specific result code.</param>
            /// <returns> 
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the instance was successfully destroyed.
            /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
            /// </returns>
            [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLWebRTCGetResultString(MLResult.Code result);

            /// <summary>
            /// Native callback that is invoked when a data channel closes.
            /// </summary>
            /// <param name="logLevel">The log level of the debug utils.</param>
            /// <param name="message">The debug message.</param>
            /// <param name="context">Pointer to a context object.</param>            
            [AOT.MonoPInvokeCallback(typeof(OnDebugMessageDelegate))]
            private static void OnDebugMessage(AndroidLogPriority logLevel, [MarshalAs(UnmanagedType.LPStr)] string message, IntPtr context)
            {
                Debug.LogError("MLWebRTC Debug: " + message);
            }

            /// <summary>
            /// The native representation of the MLWebRTC debug utilities object.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLWebRTCDebugUtils
            {
                /// <summary>
                /// Version of the struct.
                /// </summary>
                public uint Version;

                /// <summary>
                /// Pointer to a context object.
                /// </summary>
                public IntPtr Context;

                /// <summary>
                /// The log level to respect for debugging.
                /// </summary>
                public AndroidLogPriority LogLevel;

                /// <summary>
                /// The callback for when a debug message is received.
                /// </summary>
                public OnDebugMessageDelegate OnDebugMessage;

                /// <summary>
                /// Creates an initialized MLWebRTCDebugUtils object.
                /// </summary>
                /// <returns>An initialized MLWebRTCDebugUtils object.</returns>
                public static MLWebRTCDebugUtils Create()
                {
                    MLWebRTCDebugUtils debugUtils = new MLWebRTCDebugUtils
                    {
                        Version = 1,
                        LogLevel = AndroidLogPriority.Error,
                        OnDebugMessage = NativeBindings.OnDebugMessage
                    };

                    return debugUtils;
                }
            }
        }
    }
}
