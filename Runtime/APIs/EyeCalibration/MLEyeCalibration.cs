// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2022 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// MLEyeCalibration Summary placeholder.
    /// </summary>
    public partial class MLEyeCalibration : MLAutoAPISingleton<MLEyeCalibration>
    {
        /// <summary>
        /// A set of possible eye calibration codes that the eye calibration system can report.
        /// </summary>
        public enum Status
        {
            /// <summary>
            ///  No valid calibration was found for eyes.
            /// </summary>
            None,

            /// <summary>
            ///  Calibration is of lower accuracy.
            /// </summary>
            Coarse,

            /// <summary>
            ///  Calibration is of higher accuracy.
            /// </summary>
            Fine,
        };

        /// <summary>
        /// A struct representing the eye calibration state.
        /// </summary>
        public readonly struct State
        {
            /// <summary>
            /// MLEyeCalibrationStatus enum defined above.
            /// </summary>
            public readonly Status EyeCalibration;

            /// <summary>
            /// Epoch timestamp of last calibration in microseconds.
            /// </summary>
            public readonly long Timestamp;

            internal State(Status status, long timestamp)
            {
                EyeCalibration = status;
                Timestamp = timestamp;
            }
        }

        #region MLAutoAPISingleton
        /// <summary>
        /// Start the API.
        /// </summary>
        protected override MLResult.Code StartAPI() => Instance.InternalMLEyeCalibrationCreateClient();

        /// <summary>
        /// Stop the API.
        /// </summary>
        protected override MLResult.Code StopAPI() => Instance.InternalMLEyeCalibrationDestroyClient();

        /// <summary>
        /// Handle Unity application pause and resume
        /// </summary>
        /// <param name="pauseStatus">If the application is now paused or not</param>
        protected override void OnApplicationPause(bool pauseStatus)
        {
            base.OnApplicationPause(pauseStatus);
            if (pauseStatus)
            {
                Instance.InternalMLEyeCalibrationDestroyClient();
            }
            else
            {
                Instance.InternalMLEyeCalibrationCreateClient();
            }
        }
        #endregion

        /// <summary>
        /// Gets information about the user's current eye calibration.
        /// </summary>
        public static MLResult GetState(out State state) => MLResult.Create(Instance.InternalMLEyeCalibrationGetState(out state));

        /// <summary>
        /// Creates an eye calibration client.
        /// </summary>
        private MLResult.Code InternalMLEyeCalibrationCreateClient()
        {
            var resultCode = NativeBindings.MLEyeCalibrationCreateClient(out Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLEyeCalibrationCreateClient));
            return resultCode;
        }

        /// <summary>
        /// Destroys an eye calibration client.
        /// </summary>
        private MLResult.Code InternalMLEyeCalibrationDestroyClient()
        {
            var resultCode = NativeBindings.MLEyeCalibrationDestroyClient(Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLEyeCalibrationDestroyClient));
            return resultCode;
        }

        /// <summary>
        /// Gets information about the user's current eye calibration.
        /// </summary>
        private MLResult.Code InternalMLEyeCalibrationGetState(out State state)
        {
            var nativeState = NativeBindings.MLEyeCalibrationState.Create();
            var resultCode = NativeBindings.MLEyeCalibrationGetState(Handle, out nativeState);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLEyeCalibrationGetState));
            state = new State(nativeState.EyeCalibration, nativeState.EpochTimestampUs);
            return resultCode;
        }
    }
}
