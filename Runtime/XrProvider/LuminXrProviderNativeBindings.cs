// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="LuminXrProviderNativeBindings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2021 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;

namespace UnityEngine.XR.MagicLeap
{
    using System.Runtime.InteropServices;
#if UNITY_MAGICLEAP || UNITY_ANDROID
    using Native;
    using System;
    using System.IO;
    using System.Collections.Generic;
    using UnityEngine.XR.InteractionSubsystems;
    using UnityEngine.XR.ARSubsystems;
#endif

    public sealed class LuminXrProviderNativeBindings : Native.MagicLeapNativeBindings
    {
        internal const string LuminXrProviderDll = "LuminXrProvider";

        [DllImport(LuminXrProviderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void LuminXrProviderAddLibraryPath([MarshalAs(UnmanagedType.LPStr)] string path);

#if UNITY_EDITOR
        [DllImport(UnityMagicLeapDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void UnityMagicLeap_SetLibraryPath([MarshalAs(UnmanagedType.LPStr)] string path);

        [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void MLSdkLoaderResetLibraryPaths();

        [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void MLSdkLoaderUnloadAllLibraries();

        [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern MLResult.Code MLZIIsServerConfigured([MarshalAs(UnmanagedType.I1)] out bool isConfigured);
#endif

#if UNITY_MAGICLEAP || UNITY_ANDROID
        public enum LogLevel : uint
        {
            Fatal,
            Error,
            Warning,
            Info,
            Debug,
            Verbose,
        }

        private static ulong DebugHandle;
        private static ulong loaderDebugHandle;
        public const string SettingsKey = "com.magicleap.unitysdk.settings";

        LuminXrProviderSettings GetSettings()
        {
            LuminXrProviderSettings settings = null;
            // When running in the Unity Editor, we have to load user's customization of configuration data directly from
            // EditorBuildSettings. At runtime, we need to grab it from the static instance field instead.
#if UNITY_EDITOR
            UnityEditor.EditorBuildSettings.TryGetConfigObject(SettingsKey, out settings);
#else
            //settings = SampleSettings.s_RuntimeInstance;
#endif
            return settings;
        }

        /// <summary>
        /// Delegate describing the callback necessary to monitor when an debug message is received.
        /// </summary>
        /// <param name="logLevel">The log level of the debug utils.</param>
        /// <param name="message">The debug message.</param>
        /// <param name="context">Pointer to a context object.</param>
        public delegate void OnDebugMessageDelegate(LogLevel logLevel, [MarshalAs(UnmanagedType.LPStr)] string message, IntPtr context);

        [DllImport(LuminXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool StartEyeTracking();

        [DllImport(LuminXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StartHandTracking();

        [DllImport(LuminXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StartGestureTracking();

        [DllImport(LuminXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StopGestureTracking();

        [DllImport(LuminXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong GetHandTrackerHandle();

        [DllImport(LuminXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong GetControllerTrackerHandle();
        
        [DllImport(LuminXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong GetHeadTrackerHandle();

        [DllImport(LuminXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong GetInputHandle();

        [DllImport(LuminXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code GetUnityPose(MLCoordinateFrameUID cfuid, out Pose pose);

        /// <summary>
        /// Native callback that is invoked when a data channel closes.
        /// </summary>
        /// <param name="logLevel">The log level of the debug utils.</param>
        /// <param name="message">The debug message.</param>
        /// <param name="context">Pointer to a context object.</param>            
        [AOT.MonoPInvokeCallback(typeof(OnDebugMessageDelegate))]
        private static void OnDebugMessage(LogLevel logLevel, [MarshalAs(UnmanagedType.LPStr)] string message, IntPtr context)
        {
            if (logLevel == LogLevel.Fatal)
            {
                throw new Exception(message);
            }
            else if (logLevel == LogLevel.Error)
            {
                Debug.LogError("[LuminXr] " + message);
            }
            else if (logLevel == LogLevel.Warning)
            {
                Debug.LogWarning("[LuminXr] " + message);
            }
            else
            {
                Debug.Log("[LuminXr] " + message);
            }
        }

        /// <summary>
        /// The native representation of the LuminXrProvider debug utilities object.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct LuminXrProviderDebugUtils
        {
            /// <summary>
            /// Pointer to a context object.
            /// </summary>
            public IntPtr Context;

            /// <summary>
            /// The log level to respect for debugging.
            /// </summary>
            public LogLevel LogLevel;

            /// <summary>
            /// The callback for when a debug message is received.
            /// </summary>
            public OnDebugMessageDelegate OnDebugMessage;

            /// <summary>
            /// Creates an initialized MLWebRTCDebugUtils object.
            /// </summary>
            /// <returns>An initialized MLWebRTCDebugUtils object.</returns>
            public static LuminXrProviderDebugUtils Create()
            {
                LuminXrProviderDebugUtils debugUtils = new LuminXrProviderDebugUtils
                {
                    Context = IntPtr.Zero,
                    LogLevel = LogLevel.Error,
                    OnDebugMessage = LuminXrProviderNativeBindings.OnDebugMessage
                };

                return debugUtils;
            }
        }
#endif
    }
}
