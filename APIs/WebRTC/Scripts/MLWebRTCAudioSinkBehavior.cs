// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Developer Agreement, located
// here: https://auth.magicleap.com/terms/developer
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
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
                if (audioSink != null && audioSource == null)
                {
#if PLATFORM_LUMIN
                    audioSink.ResetPosition();
#endif
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

        [SerializeField, Tooltip("Used to play the audio coming into the sink when the selected buffer notify mode is 'NotifyOnly'")]
        private AudioSource audioSource;

        private MLWebRTC.AudioSink audioSink;
        private AudioClip audioClip;

        private readonly object lockObject = new object();

        private CircularBuffer<float> queue = null;
        private MLAudio.BufferFormat lastBuferFormat;
        private bool recreateAudioClip = false;

        void Awake()
        {
#if PLATFORM_LUMIN
            audioSink = MLWebRTC.AudioSink.Create(out MLResult _, sinkBufferNotifyMode);
            SetupAudioSink();
#endif
        }

        private void OnAudioDataAvailable_NativeCallbackThread(MLAudio.Buffer buffer)
        {
            lock (lockObject)
            {
                if (queue == null)
                {
                    queue = new CircularBuffer<float>(GetDesiredBufferSizeForAudioFormat(buffer.Format));

                    lastBuferFormat = buffer.Format;
                    recreateAudioClip = true;
                }

                if (queue.Count > 0)
                {
                    if (!buffer.Format.Equals(lastBuferFormat))
                    {
                        Debug.Log($"Audio buffer format changed. Need to recreate audio clip.");
                        Debug.Log($"Old Format : {lastBuferFormat}.");
                        Debug.Log($"New Format : {buffer.Format}.");

                        lastBuferFormat = buffer.Format;
                        recreateAudioClip = true;

                        queue.Clear();
                        queue.Resize(GetDesiredBufferSizeForAudioFormat(buffer.Format));
                    }
                }

                queue.Enqueue(buffer.Samples);
            }
        }

        private void OnAudioClipRead(float[] data)
        {
            lock (lockObject)
            {
                // Underflow
                if (queue.Count == 0)
                {
                    for (int i = 0; i < data.Length; ++i)
                    {
                        // avoid noise
                        data[i] = 0;
                    }
                    return;
                }

                int numDequeued = queue.Dequeue(data);
                for (int i = numDequeued; i < data.Length; ++i)
                {
                    // avoid noise
                    data[i] = 0;
                }
            }
        }

        /// <summary>
        /// Unused because we continously get new data for the audio clip,
        /// so the position has no meaning for us here.
        /// </summary>
        /// <param name="newPosition"></param>
        private void OnAudioClipSetPosition(int newPosition) {}

        void Update()
        {
            if (transform.hasChanged && CanSetWebRTCSpatializationSettings())
            {
                audioSink.SetPosition(transform.position);
                transform.hasChanged = false;
            }

            lock (lockObject)
            {
                if (recreateAudioClip)
                {
                    recreateAudioClip = false;
                    string audioClipName = $"{gameObject.name}_MLWebRTCAudioSinkBehavior";
                    Debug.Log($"Creating audio clip {audioClipName} with format {lastBuferFormat}");

                    // WebRTC sends data in 10ms packets
                    int tenMsData = (int)(10 * lastBuferFormat.SamplesPerSecond * lastBuferFormat.ChannelCount / 1000);
                    audioClip = AudioClip.Create(audioClipName, tenMsData, (int)lastBuferFormat.ChannelCount, (int)lastBuferFormat.SamplesPerSecond, true, OnAudioClipRead, OnAudioClipSetPosition);
                    audioSource.clip = audioClip;
                    // Looping needs to be set to true so that Unity contines to call OnAudioClipRead
                    // even after the speicified duration in AudioClip.Create() expires.
                    audioSource.loop = true;
                    audioSource.Play();
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
            else if (audioSource != null)
            {
                Debug.LogWarning($"MLWebRTCAudioSinkBehavior.sinkBufferNotifyMode for {gameObject.name} is '{sinkBufferNotifyMode}' and yet an AudioSource was provided. Audio for the track associated with this sink will be played directly by the underlying WebRTC system. AudioSource settings will NOT be used. Change the sinkBufferNotifyMode to 'NotifyOnly' in order to use the Unity AudioSource for playback.");
            }
        }

        private uint GetDesiredBufferSizeForAudioFormat(MLAudio.BufferFormat format)
        {
            // Ideally this should be AudioCacheSizeMs * format.SamplesPerSecond * format.ChannelCount / 1000;
            // but using size less than the current setting results in choppy playback.
            return format.SamplesPerSecond * format.ChannelCount;
        }

        private bool CanSetWebRTCSpatializationSettings()
        {
            return (audioSink != null) &&
                (audioSink.CurrentServiceStatus == MLWebRTC.AudioSink.ServiceStatus.Started) &&
                (sinkBufferNotifyMode != MLWebRTC.AudioSink.BufferNotifyMode.NotifyOnly);
        }
    }
}
