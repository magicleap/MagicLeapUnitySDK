using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLAudioPlayback
    {
        protected static MLAudioPlayback Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MLAudioPlayback();
                    MLThreadDispatch.ScheduleMain(() => instance.outputSampleRate = (uint)AudioSettings.outputSampleRate);
                }

                return instance;
            }
        }

        private static MLAudioPlayback instance;

        // MLAudioOutput plugin converts the buffer of floats into a buffer of shorts
        private const uint OutputSampleSize = sizeof(short);
        private const uint BitsPerOutputSample = OutputSampleSize * 8;
        private const float MaxOutputPitch = 1.0f;
        private const uint DefaultCacheDurationInSeconds = 4;

        private uint cacheDurationInSeconds;
        private uint outputBufferSizeInBytes;
        private uint outputSampleRate;
        private uint outputChannels;
        private bool isStarted;
        private bool bufferCreated;

        /// <summary>
        /// Creates the audio buffer with the default cache size.
        /// </summary>
        public static void CreateAudioBuffer() => Instance.CreateAudioBufferInternal(DefaultCacheDurationInSeconds);

        /// <summary>
        /// Creates the audio buffer with the specified cache size.
        /// </summary>
        public static void CreateAudioBuffer(uint cacheDurationInSeconds) => Instance.CreateAudioBufferInternal(cacheDurationInSeconds);

        public static void DestroyAudioBuffer() => Instance.DestroyAudioBufferInternal();

        /// <summary>
        /// Submit unity audio buffers to the playback plugin so that it gets
        /// submitted to MLAudio apis.
        /// </summary>
        /// <param name="data">Audio buffer</param>
        /// <param name="channels">Number of channels in the buffer</param>
        public static void SubmitBuffer(float[] data, int channels) => Instance.SubmitBufferInternal(data, channels);

        private void CreateAudioBufferInternal(uint cacheDurationInSeconds)
        {
            if (!isStarted)
            {
#if UNITY_EDITOR
                this.cacheDurationInSeconds = cacheDurationInSeconds;
                NativeBindings.CreateAudioOutput();
                this.bufferCreated = false;
                this.isStarted = true;
#endif
            }
        }

        private void DestroyAudioBufferInternal()
        {
#if UNITY_EDITOR
            NativeBindings.DestroyAudioOutput();
            isStarted = false;
#endif
        }

        private void SubmitBufferInternal(float[] data, int channels)
        {
#if UNITY_EDITOR
            if (!this.isStarted)
            {
                return;
            }

            // Prevent re-allocations in case this func gets called multiple
            // times before MLAudioPlayback.CreateAudioBuffer().
            if (!bufferCreated)
            {
                this.outputChannels = (uint)channels;
                this.outputBufferSizeInBytes = ((uint)data.Length * OutputSampleSize);
                NativeBindings.CreateOutputBuffer(this.outputBufferSizeInBytes, this.outputChannels, this.outputSampleRate, BitsPerOutputSample, BitsPerOutputSample, MaxOutputPitch, this.cacheDurationInSeconds);
                bufferCreated = true;
            }

            NativeBindings.OnUnityAudio(data, (uint)data.Length);

#if UNITY_EDITOR
            if (MagicLeapXrProvider.IsZIRunning)
            {
#endif
                // Mutes the audio that will be submitted to the engine for playback via FMOD.
                for (int i = 0; i < data.Length; ++i)
                    data[i] = 0;
#if UNITY_EDITOR
            }
#endif
#endif
        }

    }
}
