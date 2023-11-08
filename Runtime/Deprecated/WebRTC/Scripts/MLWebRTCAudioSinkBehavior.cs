// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap.Core
{
    //Disabling WebRTC deprecated warning 
    #pragma warning disable 618
    
    [RequireComponent(typeof(AudioSource))]
    public class MLWebRTCAudioSinkBehavior : MonoBehaviour
    {
        public MLWebRTC.AudioSink AudioSink
        {
            get
            {
                return audioSink;
            }

            set
            {
                if (audioSink != null)
                {
                    if (audioSource == null)
                    {
                        audioSink.ResetPosition();
                    }

                    if (sinkBufferNotifyMode == MLWebRTC.AudioSink.BufferNotifyMode.NotifyOnly)
                    {
                        audioSink.OnAudioDataAvailable_NativeCallbackThread -= OnAudioDataAvailable_NativeCallbackThread;
                    }
                }

                audioSink = value;
                if (audioSink != null)
                {
                    SetupAudioSink();
                }
            }
        }

        [SerializeField, Tooltip("Used for the audio sink that is created by this behavior. When choosing BufferNotifyMode.NotifyOnly, apps should also provide an AudioSource to feed the incoming audio buffers to.")]
        private MLWebRTC.AudioSink.BufferNotifyMode sinkBufferNotifyMode;

        /// <summary>
        /// Used to play the audio coming into the sink when the selected buffer notify mode is 'NotifyOnly'
        /// </summary>
        private AudioSource audioSource;

        /// <summary>
        /// Used to play the audio coming into the sink when the selected buffer notify mode is 'NotifyOnly'
        /// and the audio sample rate is NOT the same as that of Unity's audio engine.
        /// </summary>
        private AudioClip audioClip;

        private MLWebRTC.AudioSink audioSink;

        /// <summary>
        /// Lock object to synchronize Unity's main thread, Unity's Audio thread & MLWebRTC's callback thread.
        /// </summary>
        private readonly object lockObject = new object();

        private CircularBuffer<float> queue = null;
        private MLAudioOutput.BufferFormat lastBuferFormat;
        private bool recreateAudioClipOrSource = false;
        private bool isReadyToSubmitAudio = false;
        private float[] intermediateCache;

        void Start()
        {
            if (audioSink != null)
                return;
            audioSource = gameObject.GetComponent<AudioSource>();
            audioSink = MLWebRTC.AudioSink.Create(out MLResult result, sinkBufferNotifyMode);
            SetupAudioSink();
        }

        public void Setup()
        {
            if (audioSink != null)
                return;
            audioSource = gameObject.GetComponent<AudioSource>();
            audioSink = MLWebRTC.AudioSink.Create(out MLResult result, sinkBufferNotifyMode);
            SetupAudioSink();
        }

        private void OnAudioDataAvailable_NativeCallbackThread(MLAudioOutput.Buffer buffer)
        {
            lock (lockObject)
            {
                if (queue == null)
                {
                    lastBuferFormat = buffer.Format;
                    recreateAudioClipOrSource = true;

                    queue = new CircularBuffer<float>(GetDesiredBufferSizeForAudioFormat(lastBuferFormat));
                }

                if (queue.Count > 0)
                {
                    if (!buffer.Format.Equals(lastBuferFormat))
                    {
                        lastBuferFormat = buffer.Format;
                        recreateAudioClipOrSource = true;
                        isReadyToSubmitAudio = false;

                        queue.Clear();
                        queue.Resize(GetDesiredBufferSizeForAudioFormat(buffer.Format));
                    }
                }

                queue.Enqueue(buffer.Samples);
            }
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            int numDequeued = 0;

            lock (lockObject)
            {
                if (!(isReadyToSubmitAudio && audioClip == null))
                {
                    return;
                }

                if (lastBuferFormat.ChannelCount == channels)
                {
                    numDequeued = queue.Dequeue(data);
                }
                else
                {
                    // Calculate number of samples to dequeue based on the difference in number of channels.
                    int maxDequeueSize = (lastBuferFormat.ChannelCount < channels) ? data.Length / channels : data.Length * channels;
                    if (intermediateCache == null || intermediateCache.Length != maxDequeueSize)
                    {
                        intermediateCache = new float[maxDequeueSize];
                    }

                    numDequeued = queue.Dequeue(intermediateCache);

                    // If we only have 1 channel in the input audio, use the same value
                    // for all the channels in the output audio.
                    if (lastBuferFormat.ChannelCount == 1)
                    {
                        for (int i = 0; i < numDequeued; i++)
                        {
                            for (int ch = 0; ch < channels; ch++)
                            {
                                // interleaved channels
                                data[(i * channels) + ch] = intermediateCache[i];
                            }
                        }
                    }
                    // If the input audio has differnt num of channels than the output audio,
                    // but more than 1 channel, copy the valid channels as is and
                    // 0 out the remainder, if any.
                    else
                    {
                        int minChannels = Math.Min((int)lastBuferFormat.ChannelCount, channels);
                        for (int i = 0; i < numDequeued; i++)
                        {
                            // Channels for which we have data available in input audio
                            for (int ch = 0; ch < minChannels; ch++)
                            {
                                // interleaved channels
                                data[(i * channels) + ch] = intermediateCache[i];
                            }

                            // extra channels in output audio
                            for (int ch = minChannels; ch < channels; ch++)
                            {
                                // interleaved channels
                                data[(i * channels) + ch] = 0.0f;
                            }
                        }
                    }

                    // set numDequeued to how much of the 'data' array has been filled
                    // so zeroing out doesnt reset valid samples.
                    numDequeued *= channels;
                }
            }

            for (int i = numDequeued; i < data.Length; ++i)
            {
                // avoid noise
                data[i] = 0.0f;
            }
        }

        private void OnAudioClipRead(float[] data)
        {
            int numDequeued = 0;
            lock (lockObject)
            {
                if (isReadyToSubmitAudio)
                {
                    numDequeued = queue.Dequeue(data);
                }
            }

            for (int i = numDequeued; i < data.Length; ++i)
            {
                // avoid noise
                data[i] = 0.0f;
            }
        }

        /// <summary>
        /// Unused because we continously get new data for the audio clip,
        /// so the position has no meaning for us here.
        /// </summary>
        /// <param name="newPosition"></param>
        private void OnAudioClipSetPosition(int newPosition) { }

        void Update()
        {
            if (transform.hasChanged && CanSetWebRTCSpatializationSettings())
            {
                audioSink.SetPosition(transform.position);
                transform.hasChanged = false;
            }

            lock (lockObject)
            {
                if (recreateAudioClipOrSource)
                {
                    recreateAudioClipOrSource = false;

                    if (audioSource.isPlaying)
                    {
                        audioSource.Stop();
                    }

                    if (lastBuferFormat.SamplesPerSecond != AudioSettings.outputSampleRate)
                    {
                        string audioClipName = $"{gameObject.name}_MLWebRTCAudioSinkBehavior";

                        // WebRTC sends data in 10ms packets
                        int tenMsData = (int)(10 * lastBuferFormat.SamplesPerSecond * lastBuferFormat.ChannelCount / 1000);
                        audioClip = AudioClip.Create(audioClipName, tenMsData, (int)lastBuferFormat.ChannelCount, (int)lastBuferFormat.SamplesPerSecond, true, OnAudioClipRead, OnAudioClipSetPosition);
                    }
                    else
                    {
                        audioClip = null;
                    }

                    audioSource.clip = audioClip;
                    // Looping needs to be set to true so that Unity contines to call OnAudioClipRead
                    // even after the speicified duration in AudioClip.Create() expires.
                    audioSource.loop = true;
                    audioSource.Play();
                    isReadyToSubmitAudio = true;
                }
            }
        }

        private void OnDestroy()
        {
            if (sinkBufferNotifyMode == MLWebRTC.AudioSink.BufferNotifyMode.NotifyOnly && audioSink != null)
            {
                audioSink.OnAudioDataAvailable_NativeCallbackThread -= OnAudioDataAvailable_NativeCallbackThread;
            }
        }

        private void SetupAudioSink()
        {
            // Only use webrtc-spatialization functions if we're not doing it via Unity's AudioSource.
            if (CanSetWebRTCSpatializationSettings())
            {
                audioSink.SetPosition(transform.position);
            }

            if (sinkBufferNotifyMode == MLWebRTC.AudioSink.BufferNotifyMode.NotifyOnly)
            {
                if (audioSource != null)
                {
                    audioSink.CopyRawAudioDataToManagedMemory = true;
                    audioSink.OnAudioDataAvailable_NativeCallbackThread += OnAudioDataAvailable_NativeCallbackThread;
                }
                else
                {
                    Debug.LogError($"MLWebRTCAudioSinkBehavior.sinkBufferNotifyMode for {gameObject.name} is 'NotifyOnly' but no AudioSource was provided. Audio for the track associated with this sink will NOT be played.");
                }
            }
        }

        private uint GetDesiredBufferSizeForAudioFormat(MLAudioOutput.BufferFormat format)
        {
            // 2 second cache
            // TODO : make configurable with AudioCacheSize?
            return (uint)(2 * format.SamplesPerSecond * format.ChannelCount);
        }

        private bool CanSetWebRTCSpatializationSettings()
        {
            return (audioSink != null) &&
                (audioSink.CurrentServiceStatus == MLWebRTC.AudioSink.ServiceStatus.Started) &&
                (sinkBufferNotifyMode != MLWebRTC.AudioSink.BufferNotifyMode.NotifyOnly);
        }
    }
}
