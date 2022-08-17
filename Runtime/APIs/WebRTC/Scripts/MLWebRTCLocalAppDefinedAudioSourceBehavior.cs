// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace MagicLeap.Core
{
    using UnityEngine;
    using UnityEngine.XR.MagicLeap;
    public class MLWebRTCLocalAppDefinedAudioSourceBehavior : MonoBehaviour
    {
        private const int BitsPerSample = sizeof(short) * 8;
        // TODO: see KARROT-359
        private int channels = 1;//2;
        private int sampleRate = 0;
        private MLAudioOutput.SampleFormatType type = MLAudioOutput.SampleFormatType.Int;
        private MLWebRTC.AppDefinedAudioSource definedAudioSource;
        private MLAudioOutput.BufferFormat audioBufferFormat;
        private bool isBufferCreated;
        private short[] audioBuffer = new short[0];

        public void Init(MLWebRTC.AppDefinedAudioSource definedSource)
        {
            this.definedAudioSource = definedSource;
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            // Mute Unity audio and grab audio settings when waiting for a defined audio source. 
            if (this.definedAudioSource == null)
            {
                for (int i = 0; i < data.Length; ++i)
                    data[i] = 0;

                return;
            }

            if (!isBufferCreated)
            {
                this.sampleRate = AudioSettings.outputSampleRate;
                this.audioBufferFormat = new MLAudioOutput.BufferFormat((uint)this.channels, (uint)this.sampleRate, (uint)BitsPerSample, this.type, MLAudioOutput.ChannelFormatType.Default);
                Debug.Log(audioBufferFormat);
                this.isBufferCreated = true;
            }


            if (this.audioBuffer.Length != data.Length)
                this.audioBuffer = new short[data.Length];

            // Convert audio data to 16bit ints and then mute original audio.
            for (int i = 0; i < data.Length; ++i)
            {
                this.audioBuffer[i] = (data[i] >= 0.0f) ? (short)(data[i] * short.MaxValue) : (short)(data[i] * -short.MinValue);
                data[i] = 0;
            }

#if UNITY_MAGICLEAP || UNITY_ANDROID
            var r = this.definedAudioSource.PushData(this.audioBuffer, in this.audioBufferFormat);
#endif
        }
    }
}
