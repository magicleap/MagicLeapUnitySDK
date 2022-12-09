// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.MagicLeap
{
    [System.Serializable]
    public class MagicLeapXrProviderSettings : ScriptableObject
    {
#if !UNITY_EDITOR
        /// <summary>Static instance that will hold the runtime asset instance we created in our build process.</summary>
        public static MagicLeapXrProviderSettings s_RuntimeInstance = null;
#endif


        [SerializeField]
        private MagicLeapXrProviderNativeBindings.LogLevel m_logLevel = MagicLeapXrProviderNativeBindings.LogLevel.Error;

        public MagicLeapXrProviderNativeBindings.LogLevel LogLevel
        {
            get => m_logLevel;
            private set { m_logLevel = value; }
        }

        void Awake()
        {
#if !UNITY_EDITOR
            s_RuntimeInstance = this;
#endif
        }
    }
}
