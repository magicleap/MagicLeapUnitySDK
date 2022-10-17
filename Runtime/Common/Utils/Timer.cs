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
    public class Timer
    {
        public bool LimitPassed
        {
            get
            {
                return Elapsed() > _timeLimit;
            }
        }

        // All time properties calculated in seconds.
        private float _startTime = 0f;
        private float _timeSinceStart = 0f;
        private float _timeLimit = 0f;


        public Timer(float _timeLimitInSeconds)
        {
            Initialize(_timeLimitInSeconds);
        }

        public void Initialize(float _timeLimitInSeconds)
        {
            _timeLimit = _timeLimitInSeconds;
            Reset();
        }

        public void Reset()
        {
            _startTime = Time.realtimeSinceStartup;
        }

        public float Elapsed()
        {
            _timeSinceStart = Time.realtimeSinceStartup - _startTime;
            return _timeSinceStart;
        }
    }
}
