// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="LuminXrProviderSettings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2021 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.MagicLeap
{
    [System.Serializable]
    public class LuminXrProviderSettings : ScriptableObject
    {
#if UNITY_MAGICLEAP || UNITY_ANDROID
#if !UNITY_EDITOR
        /// <summary>Static instance that will hold the runtime asset instance we created in our build process.</summary>
        public static LuminXrProviderSettings s_RuntimeInstance = null;
#endif

        
        [SerializeField]
        private LuminXrProviderNativeBindings.LogLevel m_logLevel = LuminXrProviderNativeBindings.LogLevel.Error;

        public LuminXrProviderNativeBindings.LogLevel LogLevel
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
#endif
    }
}
