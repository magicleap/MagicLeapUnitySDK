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
            Saccade,
            
            /// <summary>
            /// User is winking with the left eye
            /// </summary>
            WinkLeft,
            
            /// <summary>
            /// User is winking with the right eye
            /// </summary>
            WinkRight
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
