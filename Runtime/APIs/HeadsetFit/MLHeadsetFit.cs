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
    /// MLHeadsetFit Summary placeholder.
    /// </summary>
    public partial class MLHeadsetFit : MLAutoAPISingleton<MLHeadsetFit>
    {
        /// <summary>
        /// Represents the different fit status that the system can report.
        /// If status is neither NotWorn nor Unknown, then headset is being worn.
        /// </summary>
        public enum Status
        {
            /// <summary>
            ///  Headset fit status not available for unknown reason.
            /// </summary>
            Unknown = 0,

            /// <summary>
            ///  Headset not worn.
            /// </summary>
            NotWorn,

            /// <summary>
            ///  Good fit.
            /// </summary>
            GoodFit,

            /// <summary>
            ///  Bad fit.
            /// </summary>
            BadFit,
        };

        /// <summary>
        /// Headset fit state
        /// </summary>
        public readonly struct State
        {
            /// <summary>
            /// Headset fit status
            /// </summary>
            public readonly Status FitStatus;

            /// <summary>
            /// The epoch time of the last update in microseconds.
            /// </summary>
            public readonly long Timestamp;

            internal State(Status status, long timestamp)
            {
                FitStatus = status;
                Timestamp = timestamp;
            }
        }

        #region MLAutoAPISingleton
        /// <summary>
        /// Start the API.
        /// </summary>
        protected override MLResult.Code StartAPI() => Instance.InternalMLHeadsetFitCreateClient();

        /// <summary>
        /// Stop the API.
        /// </summary>
        protected override MLResult.Code StopAPI() => Instance.InternalMLHeadsetFitDestroyClient();

        /// <summary>
        /// Handle Unity application pause and resume
        /// </summary>
        /// <param name="pauseStatus">If the application is now paused or not</param>
        protected override void OnApplicationPause(bool pauseStatus)
        {
            base.OnApplicationPause(pauseStatus);
            if (pauseStatus)
            {
                Instance.InternalMLHeadsetFitDestroyClient();
            }
            else
            {
                Instance.InternalMLHeadsetFitCreateClient();
            }
        }
        #endregion

        /// <summary>
        /// Gets information about the user's current headset fit.
        /// </summary>
        public static MLResult GetState(out State state) => MLResult.Create(Instance.InternalMLHeadsetFitGetState(out state));

        /// <summary>
        /// Creates a headset fit client.
        /// </summary>
        private MLResult.Code InternalMLHeadsetFitCreateClient()
        {
            var resultCode = NativeBindings.MLHeadsetFitCreateClient(out Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLHeadsetFitCreateClient));
            return resultCode;
        }

        /// <summary>
        /// Destroys headset fit client.
        /// </summary>
        private MLResult.Code InternalMLHeadsetFitDestroyClient()
        {
            var resultCode = NativeBindings.MLHeadsetFitDestroyClient(Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLHeadsetFitDestroyClient));
            return resultCode;
        }

        /// <summary>
        /// Gets information about the user's current headset fit.
        /// </summary>
        private MLResult.Code InternalMLHeadsetFitGetState(out State state)
        {
            var nativeState = NativeBindings.MLHeadsetFitState.Create();
            var resultCode = NativeBindings.MLHeadsetFitGetState(Handle, out nativeState);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLHeadsetFitGetState));
            state = new State(nativeState.FitStatus, nativeState.EpochTimestampUs);
            return resultCode;
        }
    }
}
