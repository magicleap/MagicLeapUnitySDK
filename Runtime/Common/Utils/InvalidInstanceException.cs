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
    using System;

    /// <summary>
    /// Simple custom exception for instance error checking.
    /// </summary>
    public class InvalidInstanceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidInstanceException"/> class.
        /// </summary>
        /// <param name="message">Message to provide in the exception</param>
        public InvalidInstanceException(string message)
            : base(message)
        {
        }
    }
}
