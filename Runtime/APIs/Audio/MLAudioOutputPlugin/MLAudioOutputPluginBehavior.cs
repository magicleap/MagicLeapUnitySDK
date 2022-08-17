// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLAudioOutputPluginBehavior.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2021 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    
    [RequireComponent(typeof(AudioListener))]
    [DisallowMultipleComponent]
    public class MLAudioOutputPluginBehavior : MonoBehaviour
    {
        private const string MSAPluginId = "MSA";

        void Start()
        {
#if UNITY_EDITOR && UNITY_ANDROID
            string spatializerPluginName = AudioSettings.GetSpatializerPluginName();
            if (spatializerPluginName.ToLower().Contains(MSAPluginId.ToLower()))
            {
                Destroy(this);
                return;
            }

            MLAudioPlayback.CreateAudioBuffer();
#else
            Destroy(this);
#endif
        }

        // OnAudioFilterRead runs outside of the main thread and before Start().
        void OnAudioFilterRead(float[] data, int channels)
        {
#if UNITY_EDITOR && UNITY_ANDROID
            MLAudioPlayback.SubmitBuffer(data, channels);
#endif
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR && UNITY_ANDROID
            MLAudioPlayback.DestroyAudioBuffer();
#endif
        }
    }
}
