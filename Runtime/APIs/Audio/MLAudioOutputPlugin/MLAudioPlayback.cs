using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLAudioPlayback : MLAutoAPISingleton<MLAudioPlayback>
    {
        // MLAudioOutput plugin converts the buffer of floats into a buffer of shorts
        private const uint OutputSampleSize = sizeof(short);
        private const uint BitsPerOutputSample = OutputSampleSize * 8;
        private const float MaxOutputPitch = 1.0f;

        private float[] outputDataBuffer;
        private uint outputBufferLength;
        private uint outputBufferSizeInBytes;
        private uint outputSampleRate;
        private uint outputChannels;
        private bool isStarted;

        /// <summary>
        /// Needs to be called from the main thread.
        /// TODO : remove requirement to call on main thread.
        /// </summary>
        public static void CreateAudioBuffer() => Instance.CreateAudioBufferInternal();

        public static void DestroyAudioBuffer() => Instance.DestroyAudioBufferInternal();

        /// <summary>
        /// Submit unity audio buffers to the playback plugin so that it gets
        /// submitted to MLAudio apis.
        /// </summary>
        /// <param name="data">Audio buffer</param>
        /// <param name="channels">Number of channels in the buffer</param>
        public static void SubmitBuffer(float[] data, int channels)
        {
            // SubmitBuffer() should not be the one to create the Instance because
            // the ctor and the initialization funcs it calls can only be called on the
            // main thread whereas SubmitBuffer() itself is called on the audio thread.
            // That's why we do not use the 'Instance' property here and only check
            // the state via the 'IsStarted' static property.
#if UNITY_EDITOR && UNITY_ANDROID
            if (IsStarted)
            {
                Instance.SubmitBufferInternal(data, channels);
            }
#endif
        }

#if UNITY_ANDROID
        // Don't call CreateAudioOutput() because the OnAudioFilterRead() func gets called very very early,
        // on editor startup, even before the user has had a chance to press the play button. And our ZI setup
        // happens when the play button is hit. So calling any MLAudioOutputPlugin funcs before that would result
        // in all ml_audio funcs returning MLResult_NotImplemented.
        protected override MLResult.Code StartAPI() => MLResult.Code.Ok;

        protected override MLResult.Code StopAPI()
        {
            if (isStarted)
            {
                return NativeBindings.DestroyAudioOutput() ? MLResult.Code.Ok : MLResult.Code.UnspecifiedFailure;
            }
            return MLResult.Code.Ok;
        }
#endif

        private void CreateAudioBufferInternal()
        {
            if (!isStarted)
            {
#if UNITY_EDITOR && UNITY_ANDROID
                this.outputSampleRate = (uint)AudioSettings.outputSampleRate;
                NativeBindings.CreateAudioOutput();
                this.isStarted = true;
#endif
            }
        }

        private void DestroyAudioBufferInternal()
        {
#if UNITY_EDITOR && UNITY_ANDROID
            NativeBindings.DestroyAudioOutput();
            isStarted = false;
            outputDataBuffer = null;
#endif
        }

        private void SubmitBufferInternal(float[] data, int channels)
        {
#if UNITY_EDITOR && UNITY_ANDROID
            if (!this.isStarted)
            {
                return;
            }

            // Prevent re-allocations in case this func gets called multiple
            // times before MLAudioPlayback.CreateAudioBuffer().
            if (this.outputDataBuffer == null)
            {
                this.outputBufferLength = (uint)data.Length;
                this.outputChannels = (uint)channels;
                this.outputDataBuffer = new float[this.outputBufferLength];
                this.outputBufferSizeInBytes = (this.outputBufferLength * OutputSampleSize);
                NativeBindings.CreateOutputBuffer(this.outputBufferSizeInBytes, this.outputChannels, this.outputSampleRate, BitsPerOutputSample, BitsPerOutputSample, MaxOutputPitch);
            }

            Array.Copy(data, this.outputDataBuffer, this.outputBufferLength);
            NativeBindings.OnUnityAudio(this.outputDataBuffer, this.outputBufferLength);

#if UNITY_EDITOR
            if (LuminXrProvider.IsZIRunning)
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
