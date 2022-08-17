// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLLightingTrackingNativeBindings.cs" company="Magic Leap, Inc">
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

// Disable warnings about missing documentation for native interop.
#pragma warning disable 1591

namespace UnityEngine.XR.MagicLeap
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap;


    public sealed partial class MLLightingTracking
    {
        /// <summary>
        /// See ml_lighting_tracking.h for additional comments.
        /// </summary>
        private partial class NativeBindings : Native.MagicLeapNativeBindings
        {
            /// <summary>
            /// Create a Lighting Tracker.
            /// </summary>
            /// <param name="tracker">A reference which will contain the handle to the lighting tracker.</param>
            /// <returns>
            /// A pointer to an #MLHandle which will contain the handle to the lighting tracker.
            /// MLResult_Ok Lighting tracker has been created successfully.
            /// MLResult_UnspecifiedFailure The operation failed with an unspecified error.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLLightingTrackingCreate(ref ulong tracker);

            /// <summary>
            /// Destroy a Lighting Tracker.
            /// </summary>
            /// <param name="handle">A handle to a lighting Tracker created by MLLightingTrackingCreate().</param>
            /// <returns>
            /// MLResult_Ok The lighting tracker was successfully destroyed.
            /// MLResult_UnspecifiedFailure The operation failed with an unspecified error.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLLightingTrackingDestroy(ulong handle);

            /// <summary>
            /// Gets information about the color temperature state.
            /// </summary>
            /// <param name="handle">A handle to a Lighting Tracker created by MLLightingTrackingCreate().</param>
            /// <param name="outState">Information about the color temperature state.</param>
            /// <returns>
            /// <c>MLResult.Code.InvalidParam</c>The parameter out_state was not valid.
            /// MLResult_Ok Received color temperature state successfully.
            /// MLResult_UnspecifiedFailure The operation failed with an unspecified error.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLLightingTrackingGetColorTemperatureState(ulong handle, ref ColorTemperatureStateNative outState);

            /// <summary>
            /// Information about the color temperature state.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct ColorTemperatureStateNative
            {
                /// <summary>
                /// Color Temperature in Kelvin.
                /// </summary>
                public ushort ColorTemperature;

                /// <summary>
                ///  CIE <c>tristimulus</c> X value.
                /// </summary>
                public float XCIE;

                /// <summary>
                /// CIE <c>tristimulus</c> Y value.
                /// </summary>
                public float YCIE;

                /// <summary>
                /// CIE <c>tristimulus</c> Z value.
                /// </summary>
                public float ZCIE;

                /// <summary>
                /// Time when captured in nanoseconds since the Epoch.
                /// </summary>
                public long Timestamp;

                /// <summary>
                /// Create and return an initialized version of this struct.
                /// </summary>
                /// <returns>Initialized version of this struct.</returns>
                public static ColorTemperatureStateNative Create()
                {
                    return new ColorTemperatureStateNative
                    {
                        ColorTemperature = 0,
                        XCIE = 0.0f,
                        YCIE = 0.0f,
                        ZCIE = 0.0f,
                        Timestamp = 0
                    };
                }
            }
        }
    }
}

#endif
