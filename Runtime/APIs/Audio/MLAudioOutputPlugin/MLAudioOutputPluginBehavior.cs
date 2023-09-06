// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    [RequireComponent(typeof(AudioListener))]
    [DisallowMultipleComponent]
    public class MLAudioOutputPluginBehavior : MonoBehaviour
    {
        private const string MSAPluginId = "MSA";

        void Start()
        {
#if UNITY_EDITOR
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
#if UNITY_EDITOR
            MLAudioPlayback.SubmitBuffer(data, channels);
#endif
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            MLAudioPlayback.DestroyAudioBuffer();
#endif
        }
    }
}
