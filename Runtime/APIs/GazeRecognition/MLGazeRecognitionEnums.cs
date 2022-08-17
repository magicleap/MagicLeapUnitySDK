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
    public partial class MLGazeRecognition
    {
        /// <summary>
        /// A set of mutually-exclusive behaviors that the Gaze Recognition system can report.
        /// </summary>
        public enum Behavior
        {
            /// <summary>
            /// Unknown.
            /// </summary>
            Unknown,

            /// <summary>
            ///  Both eyes closed.
            /// </summary>
            EyesClosed,

            /// <summary>
            ///  Blink detected.
            /// </summary>
            Blink,

            /// <summary>
            ///  User is fixating, eye position is stable.
            /// </summary>
            Fixation,

            /// <summary>
            ///  User is pursuing, eye velocity is low but nonzero.
            /// </summary>
            Pursuit,

            /// <summary>
            ///  User is making a saccade, eye velocity is high.
            /// </summary>
            Saccade
        };

        /// <summary>
        /// A set of possible error codes that the Gaze Recognition system can report.
        /// </summary>
        public enum Error
        {
            /// <summary>
            ///  No error, tracking is nominal.
            /// </summary>
            None,

            /// <summary>
            ///  Gaze Recognition system failed.
            /// </summary>
            Generic,
        };
    }
}
#endif
