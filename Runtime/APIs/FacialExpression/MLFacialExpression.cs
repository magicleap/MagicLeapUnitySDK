// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

// Disabling deprecated warning for the internal project
#pragma warning disable 618

using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap.Native;
namespace UnityEngine.XR.MagicLeap
{
    public partial class MLFacialExpression : MLAutoAPISingleton<MLFacialExpression>
    {
        #region StructsAndEnums
        /// <summary>
        /// Available facial expressions.
        /// </summary>
        public enum EyeExpressionType
        {
            /// <summary>
            /// Blinking the left eye.
            /// </summary>
            BlinkLeft = 0,

            /// <summary>
            /// Blinking of the right eye.
            /// </summary>
            BlinkRight,

            /// <summary>
            /// Lower Lid upward movement of the left eye.
            /// </summary>
            LidTightenerLeft,

            /// <summary>
            /// Lower Lid upward movement of the right eye.
            /// </summary>
            LidTightenerRight,

            /// <summary>
            /// Upper lid upward movement of the left eye.
            /// </summary>
            EyeOpennessLeft,

            /// <summary>
            /// Upper lid upward movement of the right eye.
            /// </summary>
            EyeOpennessRight,

            /// <summary>
            /// Upward cheek movement, left.
            /// </summary>
            CheekRaiserLeft,

            /// <summary>
            /// Upward cheek movement, right.
            /// </summary>
            CheekRaiserRight,

            /// <summary>
            /// Downward brow movement, left.
            /// </summary>
            BrowLowererLeft,

            /// <summary>
            /// Downward brow movement, right.
            /// </summary>
            BrowLowererRight,

            /// <summary>
            /// Upward brow movement, left side.
            /// </summary>
            BrowRaiserLeft,

            /// <summary>
            /// Upward brow movement, right side.
            /// </summary>
            BrowRaiserRight
        }

        /// <summary>
        /// A structure containing settings for the facial expressions.
        /// </summary>
        public struct Settings
        {
            /// <summary>
            /// Enable MLFacialExpressionEyeData. If true, facial expressions will
            /// detect EyeData and the same can queried GetEyeData. This should be
            /// disabled when app does not need facial expression data
            /// </summary>
            public bool EnableEyeExpression;
        }

        /// <summary>
        /// A structure containing information about eye expressions.
        /// </summary>
        public struct EyeData
        {
            /// <summary>
            /// The MLTime timestamp when expression data was updated.
            /// </summary>
            public MLTime Timestamp;

            /// <summary>
            /// The values are between 0 and 1 and ordered such that the array should
            /// be indexed using EyeExpressionType.
            /// </summary>
            public float[] EyeExpressionWeights;
        }
        #endregion

        /// <summary>
        /// Start the API.
        /// </summary>
        protected override MLResult.Code StartAPI() => Instance.InternalMLFacialExpressionCreate();

        /// <summary>
        /// Stop the API.
        /// </summary>
        protected override MLResult.Code StopAPI() => Instance.InternalMLFacialExpressionStop();

        /// <summary>
        /// Updates Facial Expression system with new settings.
        /// </summary>
        /// <returns></returns>
        public static MLResult.Code UpdateSettings(in Settings settings) => Instance.InternalUpdateSettings(settings);

        /// <summary>
        /// Get the latest eye data from the Facial Expression system.
        /// </summary>
        public static MLResult.Code GetEyeData(out EyeData data) => Instance.InternalGetEyeData(out data);

        #region InternalMethods
        /// <summary>
        /// Creates Facial Expression system client.
        /// </summary>
        private MLResult.Code InternalMLFacialExpressionCreate()
        {
            if (!MLResult.DidNativeCallSucceed(MLPermissions.CheckPermission(MLPermission.FacialExpression).Result))
            {
                MLPluginLog.Error($"{nameof(MLFacialExpression)} requires missing permission {MLPermission.FacialExpression}");
                return MLResult.Code.PermissionDenied;
            }

            NativeBindings.MLFacialExpressionSettings settings = NativeBindings.MLFacialExpressionSettings.Init();
            MLResult.Code resultCode = NativeBindings.MLFacialExpressionCreateClient(ref settings, out Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLFacialExpressionCreateClient));

            return resultCode;
        }

        /// <summary>
        /// Destroy Facial Expression system client.
        /// </summary>
        private MLResult.Code InternalMLFacialExpressionStop()
        {
            var resultCode = NativeBindings.MLFacialExpressionDestroyClient(Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLFacialExpressionDestroyClient));
            return resultCode;
        }

        /// <summary>
        /// Updates API settings.
        /// </summary>
        private MLResult.Code InternalUpdateSettings(Settings settings)
        {
            NativeBindings.MLFacialExpressionSettings newSettings = NativeBindings.MLFacialExpressionSettings.Init();
            newSettings.EnableEyeExpression = settings.EnableEyeExpression;
            var resultCode = NativeBindings.MLFacialExpressionUpdateSettings(Handle, newSettings);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLFacialExpressionDestroyClient));

            return resultCode;
        }

        /// <summary>
        /// Grabs eye data.
        /// </summary>
        /// <returns></returns>
        private MLResult.Code InternalGetEyeData(out EyeData data)
        {

            data = new();
            NativeBindings.MLFacialExpressionEyeData outData = NativeBindings.MLFacialExpressionEyeData.Init();
            var resultCode = NativeBindings.MLFacialExpressionGetEyeData(Handle, out outData);
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLFacialExpressionGetEyeData)))
            {
                data.EyeExpressionWeights = outData.EyeExpressionWeights;
                data.Timestamp = outData.Timestamp;
            }

            return resultCode;
        }
        #endregion
    }
}
