// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2023) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine.XR.MagicLeap.Native;
namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    public partial class MLFacialExpression
    {
        /// <summary>
        /// See ml_facial_expression.h for additional comments.
        /// </summary>
        private class NativeBindings : Native.MagicLeapNativeBindings
        {
            /// <summary>
            /// Number of eye expression types.
            /// </summary>
            const int MLFacialExpressionEyeExpressionTypeCount = 12;

            /// <summary>
            /// A structure containing settings for the facial expressions.
            /// This structure must be initialized by calling #MLFacialExpressionSettingsInit before use.
            /// (And calling ZeroStruct on our side.)
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLFacialExpressionSettings
            {
                /// <summary>
                /// Version of this settings
                /// </summary>
                public uint Version;

                /// <summary>
                /// Enable MLFacialExpressionEyeData.
                /// If true, facial expressions will detect #MLFacialExpressionEyeData and the same can queried using
                /// MLFacialExpressionGetEyeData. This should be disabled when app does not need facial expression data.
                /// </summary>
                public bool EnableEyeExpression;

                public static MLFacialExpressionSettings Init(uint version = 1)
                {
                    return new MLFacialExpressionSettings
                    {
                        Version = version,
                        EnableEyeExpression = true
                    };
                }
            }

            /// <summary>
            /// A structure containing information about eye expressions.
            /// This structure must be initialized by calling MLFacialExpressionEyeDataInit before use.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLFacialExpressionEyeData
            {
                /// <summary>
                /// Version of the structure.
                /// </summary>
                public uint Version;

                /// <summary>
                /// The MLTime timestamp when expression data was updated.
                /// </summary>
                public long Timestamp;

                /// <summary>
                /// An array of floats of size eye_expression_count. The values are between 0
                /// and 1 and ordered such that the array can be indexed using
                /// MLFacialExpressionEyeExpressionType.
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = MLFacialExpressionEyeExpressionTypeCount)]
                public float[] EyeExpressionWeights;

                public static MLFacialExpressionEyeData Init(uint version = 1)
                {
                    return new MLFacialExpressionEyeData
                    {
                        Version = version,
                    };
                }
            }

            /// <summary>
            /// Creates a Facial Expression Client handle. Although multiple client handles
            /// can be created they all represent the same facial expressions backend system.
            /// </summary>
            /// <para>API Level 29</para>
            /// permissions com.magicleap.permission.FACIAL_EXPRESSION (protection level: dangerous)
            /// <param name="settings">
            /// Settings that configures the facial expressions system.
            /// </param>
            /// <param name="handle">
            /// The handle to be created.
            /// </param>
            /// <returns>
            /// MLResult.Code.InvalidParam: One or more input parameters are not valid.<br/>
            /// MLResult.Code.Ok: Facial expressions client was successfully created.<br/>
            /// MLResult.Code.PerceptionSystemNotStarted: Perception System has not been started.<br/>
            /// MLResult.Code.PermissionDenied: Necessary permission is missing. <br/>
            /// MLResult.Code.UnspecifiedFailure: Operation failed for unknown reason.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLFacialExpressionCreateClient(ref MLFacialExpressionSettings settings, out ulong handle);

            /// <summary>
            /// Update the Facial Expression system with new settings
            /// </summary>
            /// <para>API Level 29</para>
            /// permissions None
            /// <param name="handle">
            /// Handle Facial expressions client handle created by MLFacialExpressionCreateClient.
            /// </param>
            /// <param name="settings">
            /// Settings New Facial Expression settings..
            /// </param>
            /// <returns>
            /// MLResult.Code.InvalidParam: One or more input parameters are not valid.<br/>
            /// MLResult.Code.Ok: Facial expression settings was updated successfully.<br/>
            /// MLResult.Code.PerceptionSystemNotStarted: Perception System has not been started.<br/>
            /// MLResult.Code.UnspecifiedFailure: Operation failed for unknown reason. <br/>
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLFacialExpressionUpdateSettings(ulong handle, in MLFacialExpressionSettings settings);

            /// <summary>
            /// Get facial expressions data.
            /// handle Facial expression client handle created by MLFacialExpressionCreateClient.
            /// <para>API Level 29</para>
            /// permissions None
            /// </summary>
            /// <param name="handle">
            /// Facial expressions client handle created by MLFacialExpressionCreateClient.
            /// </param>
            /// <param name="out_eye_data">
            /// Eye expressions data.
            /// </param>
            /// <returns>
            /// MLResult.Code.InvalidParam: One or more input parameters are not valid.<br/>
            /// MLResult.Code.Ok: Facial expressions data was retrieved successfully.<br/>
            /// MLResult.Code.PerceptionSystemNotStarted: Perception System has not been started.<br/>
            /// MLResult.Code.UnspecifiedFailure: Operation failed for unknown reason. <br/>
            /// MLResult.Code.HeadsetFitIssue: Operation failed because unable to detect the eyes, check MLHeadsetFitStatus.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLFacialExpressionGetEyeData(ulong handle, out MLFacialExpressionEyeData eye_data);

            /// <summary>
            /// Destroy client handle and free client resources.
            /// <para>API Level 29</para>
            /// permissions None
            /// </summary>
            /// <param name="handle">
            /// Facial expression client handle created by MLFacialExpressionCreateClient.
            /// </param>
            /// <returns>
            /// MLResult.Code.InvalidParam: One or more input parameters are not valid.<br/>
            /// MLResult.Code.Ok: Client handle was successfully destroyed.<br/>
            /// MLResult.Code.PerceptionSystemNotStarted: Perception System has not been started.<br/>
            /// MLResult.Code.UnspecifiedFailure: Operation failed for unknown reason.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLFacialExpressionDestroyClient(ulong handle);
        }
    }
}