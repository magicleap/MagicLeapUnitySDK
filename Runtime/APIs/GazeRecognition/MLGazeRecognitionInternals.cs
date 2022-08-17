// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLGazeRecognition.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if UNITY_MAGICLEAP || UNITY_ANDROID

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
            return NativeBindings.MLGazeRecognitionCreate(out Handle);
        }

        /// <summary>
        /// Stop the API.
        /// </summary>
        protected override MLResult.Code StopAPI()
        {
            return NativeBindings.MLGazeRecognitionDestroy(Handle);
        }
        
        /// <summary>
        /// Get information about the user's gaze.
        /// </summary>
        private MLResult.Code InternalMLGazeRecognitionGetState(out State state)
        {
            MLResult.Code result = NativeBindings.MLGazeRecognitionGetState(Handle, out NativeBindings.MLGazeRecognitionState internalState);
            MLResult.DidNativeCallSucceed(result, "MLGazeRecognitionGetState");
            state = new State(internalState);
            return result;
        }

        /// <summary>
        /// Get static information about Gaze Recognition.
        /// </summary>
        private MLResult.Code InternalMLGazeRecognitionGetStaticData(out StaticData data)
        {
            MLResult.Code result = NativeBindings.MLGazeRecognitionGetStaticData(Handle, out NativeBindings.MLGazeRecognitionStaticData internalData);
            result = LuminXrProviderNativeBindings.GetUnityPose(internalData.Vergence, out Pose pose);
            data = new StaticData(pose);
            return result;
        }
    }
}
#endif
