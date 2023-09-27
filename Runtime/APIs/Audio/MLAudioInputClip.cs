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
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Manages Audio.
    /// </summary>
    public partial class MLAudioInput
    {
        /// <summary>
        /// The type of mic input to capture.
        /// </summary>
        public enum MicCaptureType
        {
            /// <summary>
            ///	User voice, suitable for communications.
            ///	Mono, 16 bit integer, 16k sampling rate.
            /// </summary>
            VoiceComm,

            /// <summary>
            /// High fidelity user voice, suitable for recording.
            /// Mono, 16 bit integer, 48k sampling rate.
            /// </summary>
            VoiceCapture,

            /// <summary>
            ///	Mic mix representing the user's surroundings.
            ///	Stereo, 16 bit integer, 48k sampling rate.
            /// </summary>
            WorldCapture = 4
        }

        /// <summary>
        /// Gets the sample rate preferences of the device based on the capture type.
        /// </summary>
        public static int GetSampleRate(MicCaptureType captureType)
        {
            int sampleRate = 0;

            switch (captureType)
            {
                case MicCaptureType.VoiceComm:
                    sampleRate = 16000;
                    break;

                case MicCaptureType.VoiceCapture:
                case MicCaptureType.WorldCapture:
                    sampleRate = 48000;
                    break;
            }

            return sampleRate;

        }

        /// <summary>
        /// Gets the channels of the capture type.
        /// </summary>
        public static int GetChannels(MicCaptureType captureType)
        {
            int channels = 0;

            switch (captureType)
            {
                case MicCaptureType.VoiceComm:
                case MicCaptureType.VoiceCapture:
                    channels = 1;
                    break;

                case MicCaptureType.WorldCapture:
                    channels = 2;
                    break;
            }

            return channels;
        }

        /// <summary>
        /// Streams audio input data into an AudioClip.
        /// </summary>
        public abstract class Clip : IDisposable
        {

            private readonly GCHandle gcHandle;
            private readonly uint numSamples;

            private ulong captureHandle = Native.MagicLeapNativeBindings.InvalidHandle;

            public Clip(MicCaptureType captureType, uint samplesLengthInSeconds, uint channels)
            {
                gcHandle = GCHandle.Alloc(this, GCHandleType.Weak);

                // get the best buffer format to use
                MLResult.Code resultCode = NativeBindings.MLAudioGetBufferedInputDefaults(channels, (uint)GetSampleRate(captureType), out MLAudioOutput.NativeBindings.MLAudioBufferFormat bufferFormat, out uint recommendedSizeInBytes, out uint minimumSizeInBytes);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLAudioGetBufferedInputDefaults));

                // create the audio input stream 
                resultCode = NativeBindings.MLAudioCreateInputFromMicCapture(captureType, in bufferFormat, recommendedSizeInBytes, HandleMLAudioBufferCallback, GCHandle.ToIntPtr(gcHandle), out captureHandle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLAudioCreateInputFromMicCapture));

                // start the stream
                resultCode = NativeBindings.MLAudioStartInput(captureHandle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLAudioStartInput));
            }

            ~Clip() => Dispose();

            /// <summary>
            /// Stops streaming data.
            /// </summary>
            public void Dispose()
            {
                if (!Native.MagicLeapNativeBindings.MLHandleIsValid(captureHandle))
                    return;

                gcHandle.Free();

                var resultCode = NativeBindings.MLAudioStopInput(captureHandle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLAudioStopInput));

                captureHandle = Native.MagicLeapNativeBindings.InvalidHandle;
            }

            /// <summary>
            /// Converts MLAudioInput's buffer into float samples that get enqueued into the circular buffer to be pushed to the audio clip later.
            /// </summary>
            private void CopyMLAudioInputBuffer()
            {
                var resultCode = NativeBindings.MLAudioGetInputBuffer(captureHandle, out MLAudioOutput.NativeBindings.MLAudioBuffer buffer);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLAudioGetInputBuffer));

                float[] samples;
                // copy ml audio input buffer
                unsafe
                {
                    // cast the buffer of bytes into a buffer of shorts
                    short* shortSamples = (short*)buffer.Ptr;
                    uint shortSamplesSize = buffer.Size / sizeof(short);

                    // init float array if needed
                    samples = new float[shortSamplesSize];

                    // convert the short samples into normalized float samples
                    for (int i = 0; i < shortSamplesSize; i++)
                        samples[i] = (float)shortSamples[i] / short.MaxValue;
                }

                OnReceiveSamples(samples);

                resultCode = NativeBindings.MLAudioReleaseInputBuffer(captureHandle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLAudioReleaseInputBuffer));
            }

            /// <summary>
            /// Native callback for when an input buffer becomes available.
            /// Converts the input buffer's data and copies it into the AudioClip's data.
            /// </summary>
            [AOT.MonoPInvokeCallback(typeof(NativeBindings.MLAudioOnBufferDelegate))]
            private static void HandleMLAudioBufferCallback(ulong handle, IntPtr context)
            {
                var mlAudioInputClip = GCHandle.FromIntPtr(context).Target as Clip;

                if (mlAudioInputClip == null)
                    return;

                mlAudioInputClip.CopyMLAudioInputBuffer();
            }

            /// <summary>
            /// Gets called from a thread when new samples are recorded.
            /// </summary>
            protected abstract void OnReceiveSamples(float[] samples);
        }
    }
}
