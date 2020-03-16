// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLAppAnalyticsNativeBindings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if PLATFORM_LUMIN

using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Native;

// Disable warnings about missing documentation for native interop.
#pragma warning disable 1591

namespace UnityEngine.XR.MagicLeap.Internal
{
    /// <summary>
    /// See ml_app_analytics.h for additional comments
    /// </summary>
    public class MLAppAnalyticsNativeBindings : MagicLeapNativeBindings
    {
        private MLAppAnalyticsNativeBindings() {}

        public const string MLAppAnalyticsDll = "ml_app_analytics";

        [DllImport(MLAppAnalyticsDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern UnityEngine.XR.MagicLeap.MLResult.Code  MLAppAnalyticsUpload(string eventValue, string jsonValue);
    }
}

#endif
