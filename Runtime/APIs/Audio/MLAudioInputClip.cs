// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLAudioInputClip.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
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
            ///	 Raw stream from front two mics.
            ///	 Stereo, 16 bit integer, 48k sampling rate.
            /// </summary>
            FrontMics,

            /// <summary>
            ///	Raw stream from side two mics.
            ///	Stereo, 16 bit integer, 48k sampling rate.
            /// </summary>
            SideMics,

            /// <summary>
            ///	Mic mix representing the user's surroundings.
            ///	Stereo, 16 bit integer, 48k sampling rate.
            /// </summary>
            WorldCapture
        }

        /// <summary>
        /// Stores list of audio clips to prevent premature garbage collection.
        /// </summary>
        private List<Clip> mlAudioClips = new List<Clip>();

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
                case MicCaptureType.FrontMics:
                case MicCaptureType.SideMics:
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

                case MicCaptureType.FrontMics:
                case MicCaptureType.SideMics:
                case MicCaptureType.WorldCapture:
                    channels = 2;
                    break;
            }

            return channels;
        }

        /// <summary>
        /// Returns an object that streams audio into it's AudioClip member.
        /// TODO: Create a non-streamed audio clip for when loop is set to false and use audioClip.SetData() instead.
        /// </summary>
        public static Clip CreateClip(MicCaptureType captureType, bool loop, int lengthSec, int frequency) => Instance.CreateClipInternal(captureType, loop, lengthSec, frequency);

        private Clip CreateClipInternal(MicCaptureType captureType, bool loop, int lengthSec, int frequency)
        {
            var streamedClip = new Clip(captureType, loop, (uint)lengthSec, (uint)frequency, (uint)MLAudioInput.GetChannels(captureType));
            mlAudioClips.Add(streamedClip);
            return streamedClip;
        }


        /// <summary>
        /// Streams audio input data into an AudioClip.
        /// </summary>
        public class Clip
        {
            private const uint CIRCULAR_BUFFER_SIZE_MULTIPLIER = 12;

            public AudioClip UnityAudioClip => this.audioClip;

            // This is only tracked for a streamed audio clip (when "loop" is set to true in CreateClip)
            public int SamplesPosition
            {
                get;
                private set;
            }

            public int ReadPosition
            {
                get;
                private set;
            }

            private readonly AudioClip audioClip;
            private readonly GCHandle gcHandle;
            private readonly bool isStreamed;
            private readonly object lockObject;
            private readonly CircularBuffer<float> circularBuffer;
            private readonly uint numSamples;

            private ulong captureHandle = Native.MagicLeapNativeBindings.InvalidHandle;
            private float[] mlAudioSamplesFloats = new float[0];

            public Clip(MicCaptureType captureType, bool stream, uint samplesLengthInSeconds, uint sampleRate, uint channels)
            {
                isStreamed = stream;
                lockObject = new object();
                gcHandle = GCHandle.Alloc(this, GCHandleType.Weak);

                // calculate appropriate circular buffer size
                numSamples = samplesLengthInSeconds * sampleRate;
                uint circularBufferSize = numSamples * CIRCULAR_BUFFER_SIZE_MULTIPLIER;
                circularBuffer = new CircularBuffer<float>(circularBufferSize);

                // create an audio clip to stream to with the registered callbacks or not based on if it should loop it's audio data
                audioClip = AudioClip.Create(string.Empty, (int)numSamples, (int)channels, (int)sampleRate, isStreamed, isStreamed ? OnAudioDataRead : null, OnAudioDataPosition);

#if UNITY_ANDROID
                // get the best buffer format to use
                MLResult.Code resultCode = NativeBindings.MLAudioGetBufferedInputDefaults(channels, (uint)MLAudioInput.GetSampleRate(captureType), out MLAudioOutput.NativeBindings.MLAudioBufferFormat bufferFormat, out uint recommendedSizeInBytes, out uint minimumSizeInBytes);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLAudioGetBufferedInputDefaults));

                // create the audio input stream 
                resultCode = NativeBindings.MLAudioCreateInputFromMicCapture(captureType, in bufferFormat, recommendedSizeInBytes, HandleMLAudioBufferCallback, GCHandle.ToIntPtr(gcHandle), out captureHandle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLAudioCreateInputFromMicCapture));

                // start the stream
                resultCode = NativeBindings.MLAudioStartInput(captureHandle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLAudioStartInput));
#endif
            }

            ~Clip() => Stop();

            /// <summary>
            /// Gets the current data of the audio clip.
            /// </summary>
            public void GetData(float[] audioData, int offsetSamples)
            {
                if (!isStreamed)
                {
                    audioClip.GetData(audioData, offsetSamples);
                }
                else
                {
                    var lengthToCopy = Math.Min(mlAudioSamplesFloats.Length, audioData.Length);
                    offsetSamples %= lengthToCopy;
                    Array.Copy(mlAudioSamplesFloats, offsetSamples, audioData, 0, lengthToCopy);
                }
            }

            /// <summary>
            /// Stops streaming data.
            /// </summary>
            public void Stop()
            {
                if (!Native.MagicLeapNativeBindings.MLHandleIsValid(captureHandle))
                    return;

                gcHandle.Free();
#if UNITY_ANDROID
                var resultCode = NativeBindings.MLAudioStopInput(captureHandle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLAudioStopInput));
#endif
                captureHandle = Native.MagicLeapNativeBindings.InvalidHandle;
            }

            /// <summary>
            /// Callback for when an audio clip's read position is updated.
            /// </summary>
            private void OnAudioDataPosition(int newPosition)
            {
                ReadPosition = newPosition;
            }

            /// <summary>
            /// Callback for when an audio clip's data is available to alter.
            /// Only called when an audio clip is streaming.
            /// </summary>
            private void OnAudioDataRead(float[] audioClipData)
            {
                if (!Native.MagicLeapNativeBindings.MLHandleIsValid(captureHandle))
                    return;

                int numDequeued = 0;

                // dequeue buffered audio data into the audio clip's data
                lock (lockObject)
                    numDequeued = circularBuffer.Dequeue(audioClipData);

                SamplesPosition = (SamplesPosition + numDequeued) % (int)numSamples;

                // mute the rest of the audio clip's data that wasn't copied over
                if (numDequeued < audioClipData.Length)
                {
                    for (int i = numDequeued; i < audioClipData.Length; ++i)
                        audioClipData[i] = 0;
                }
            }


            /// <summary>
            /// Converts MLAudioInput's buffer into float samples that get enqueued into the circular buffer to be pushed to the audio clip later.
            /// </summary>
            private void CopyMLAudioInputBuffer()
            {
#if UNITY_ANDROID
                var resultCode = NativeBindings.MLAudioGetInputBuffer(captureHandle, out MLAudioOutput.NativeBindings.MLAudioBuffer buffer);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLAudioGetInputBuffer));

                // copy ml audio input buffer
                unsafe
                {
                    // cast the buffer of bytes into a buffer of shorts
                    short* shortSamples = (short*)buffer.Ptr;
                    uint shortSamplesSize = buffer.Size / sizeof(short);

                    // init float array if needed
                    if (mlAudioSamplesFloats.Length != shortSamplesSize)
                        mlAudioSamplesFloats = new float[shortSamplesSize];

                    // convert the short samples into normalized float samples
                    for (int i = 0; i < shortSamplesSize; i++)
                        mlAudioSamplesFloats[i] = (float)shortSamples[i] / short.MaxValue;
                }
#endif

                if (isStreamed)
                {
                    // enqueue into circular buffer with lock because OnAudioDataRead runs on another thread
                    lock (lockObject)
                        circularBuffer.Enqueue(mlAudioSamplesFloats);
                }
                else
                {
                    // enqueue audio samples until buffer has had numSamples amount of samples pushed into it
                    for (int i = 0; i < mlAudioSamplesFloats.Length; i++)
                    {
                        circularBuffer.Enqueue(mlAudioSamplesFloats[i]);

                        if (circularBuffer.Count >= numSamples)
                        {
                            var recordedSamples = new float[numSamples];
                            circularBuffer.Dequeue(recordedSamples);
#if UNITY_ANDROID
                            Native.MLThreadDispatch.ScheduleMain(() => audioClip.SetData(recordedSamples, 0));
                            circularBuffer.Clear();
                            resultCode = NativeBindings.MLAudioReleaseInputBuffer(captureHandle);
                            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLAudioReleaseInputBuffer));
#endif
                            Stop();
                            return;
                        }
                    }
                }

#if UNITY_ANDROID
                resultCode = NativeBindings.MLAudioReleaseInputBuffer(captureHandle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLAudioReleaseInputBuffer));
#endif
            }

#if UNITY_ANDROID
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
#endif
        }
    }
}
