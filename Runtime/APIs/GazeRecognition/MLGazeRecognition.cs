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
    ///	This API can be used to get info about gaze.
    /// </summary>
    public partial class MLGazeRecognition : MLAutoAPISingleton<MLGazeRecognition>
    {
        /// <summary>
        /// Get information about the user's gaze.
        /// </summary>
        /// <param name="state">Information about the gaze.</param>
        /// <returns>
        ///	<c>MLResult_InvalidParam</c> The state parameter was not valid (null).
        /// <c>MLResult_Ok</c> gaze Recognition static was successfully received.
        /// <c>MLResult_UnspecifiedFailure</c> Failed to receive gaze Recognition state data.
        /// </returns>
        public static MLResult GetState(out State state) => MLResult.Create(Instance.InternalMLGazeRecognitionGetState(out state));

        /// <summary>
        /// Get static information about Gaze Recognition.
        /// </summary>
        /// <param name="data">Target to populate the data about Gaze Recognition..</param>
        /// <returns>
        /// <c>MLResult_InvalidParam</c> The data parameter was not valid (null).
        /// <c>MLResult_Ok gaze Recognition</c> data was successfully received.
        /// <c>MLResult_UnspecifiedFailure</c> Failed to receive gaze Recognition static data.
        /// </returns>
        public static MLResult GetStaticData(out StaticData data) => MLResult.Create(Instance.InternalMLGazeRecognitionGetStaticData(out data));
    }
}
#endif
