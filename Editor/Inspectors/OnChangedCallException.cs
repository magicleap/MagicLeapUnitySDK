// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2022 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;

#if UNITY_EDITOR
namespace UnityEngine.XR.MagicLeap
{
    public class OnChangedCallException : Exception
    {
        public OnChangedCallException(string message) : base(message)
        {
            Debug.LogError(message);
        }
    }
}
#endif
