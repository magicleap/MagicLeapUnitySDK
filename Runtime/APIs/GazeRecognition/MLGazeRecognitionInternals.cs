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
    /// <summary>
    /// Runs once per Unity Update loop.
    /// </summary>
    public partial class MLGazeRecognition
    {
        /// <summary>
        /// Start the API.
        /// </summary>
        protected override MLResult.Code StartAPI()
        {
            var resultCode = NativeBindings.MLGazeRecognitionCreate(out Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLGazeRecognitionCreate));
            return resultCode;
        }

        /// <summary>
        /// Stop the API.
        /// </summary>
        protected override MLResult.Code StopAPI()
        {
            var resultCode = NativeBindings.MLGazeRecognitionDestroy(Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLGazeRecognitionDestroy));
            return resultCode;
        }

        /// <summary>
        /// Get information about the user's gaze.
        /// </summary>
        private MLResult.Code InternalMLGazeRecognitionGetState(out State state)
        {
            NativeBindings.MLGazeRecognitionState internalState = new NativeBindings.MLGazeRecognitionState(1);
            MLResult.Code result = NativeBindings.MLGazeRecognitionGetState(Handle, ref internalState);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLGazeRecognitionGetState));
            state = new State(internalState);
            return result;
        }

        /// <summary>
        /// Get static information about Gaze Recognition.
        /// </summary>
        private MLResult.Code InternalMLGazeRecognitionGetStaticData(out StaticData data)
        {
            NativeBindings.MLGazeRecognitionStaticData internalData = new NativeBindings.MLGazeRecognitionStaticData(2);
            MLResult.Code result = NativeBindings.MLGazeRecognitionGetStaticData(Handle, ref internalData);
            result = MagicLeapXrProviderNativeBindings.GetUnityPose(internalData.Vergence, out Pose pose);
            data = new StaticData(pose, internalData.EyeHeightMax, internalData.EyeWidthMax);
            return result;
        }
    }
}
