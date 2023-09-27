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
    public partial class MLAudioInput
    {
        public class StreamingClip : Clip
        {
            public AudioClip UnityAudioClip { get; private set; }

            private CircularBuffer<float> circularBuffer;
            private readonly object bufferLock = new object();
            private readonly int _sampleCount;
            private readonly int _channels;
            private readonly int _sampleRate;

            public StreamingClip(MicCaptureType captureType, int lengthSec, int frequency) : this(captureType, (uint)lengthSec, (uint)frequency, (uint)GetChannels(captureType)) { }

            public StreamingClip(MicCaptureType captureType, uint samplesLengthInSeconds, uint sampleRate, uint channels) : base(captureType, samplesLengthInSeconds, channels)
            {
                _sampleCount = (int)samplesLengthInSeconds * (int)sampleRate;
                circularBuffer = new CircularBuffer<float>((uint)_sampleCount);
                _sampleRate = (int)sampleRate;
                _channels = (int)channels;
                UnityAudioClip = AudioClip.Create("ML Streamed AudioClip", _sampleCount, _channels, _sampleRate, true, OnAudioRead);
            }

            protected override void OnReceiveSamples(float[] samples)
            {
                lock (bufferLock)
                {
                    circularBuffer.Enqueue(samples);
                }
            }

            void OnAudioRead(float[] data)
            {
                lock (bufferLock)
                {
                    circularBuffer.Dequeue(data);
                }
            }

            public void ClearBuffer()
            {
                UnityAudioClip = AudioClip.Create("ML Streamed AudioClip", _sampleCount, _channels, _sampleRate, true, OnAudioRead);

                lock (bufferLock)
                {
                    circularBuffer.Clear();
                    circularBuffer.Reset();
                }
            }
        }
    }
}
