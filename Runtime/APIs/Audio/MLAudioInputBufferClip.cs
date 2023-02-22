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
    using System;

    public partial class MLAudioInput
    {
        public class BufferClip : Clip
        {
            public event Action<float[]> OnReceivedSamples;
            private readonly float[] buffer;
            private readonly object bufferLock;
            private readonly int sampleRate;
            private readonly int channels;

            private int bufferSampleCount;

            public int BufferPosition { get; private set; }

            public BufferClip(MicCaptureType captureType, int lengthSec, int frequency) : this(captureType, (uint)lengthSec, (uint)frequency, (uint)GetChannels(captureType)) { }

            public BufferClip(MicCaptureType captureType, uint samplesLengthInSeconds, uint sampleRate, uint channels) : base(captureType, samplesLengthInSeconds, channels)
            {
                buffer = new float[samplesLengthInSeconds * sampleRate];
                bufferLock = new object();
                this.sampleRate = (int)sampleRate;
                this.channels = (int)channels;
                BufferPosition = 0;
                bufferSampleCount = 0;
            }

            protected override void OnReceiveSamples(float[] samples)
            {
                OnReceivedSamples?.Invoke(samples);

                lock (bufferLock)
                {
                    int givenSamplesCount = samples.Length;
                    givenSamplesCount = Mathf.Min(givenSamplesCount, buffer.Length);
                    if (BufferPosition + givenSamplesCount <= buffer.Length)
                    {
                        System.Array.Copy(samples, 0, buffer, BufferPosition, givenSamplesCount);
                        BufferPosition += givenSamplesCount;
                        bufferSampleCount = Mathf.Max(bufferSampleCount, BufferPosition);
                    }
                    else
                    {
                        int bufferSpace = buffer.Length - BufferPosition;
                        System.Array.Copy(samples, 0, buffer, BufferPosition, bufferSpace);
                        System.Array.Copy(samples, bufferSpace, buffer, 0, givenSamplesCount - bufferSpace);
                        BufferPosition = givenSamplesCount - bufferSpace;
                        bufferSampleCount = buffer.Length;
                    }
                }
            }

            // Automatically wraps the data past the BufferPosition index to be at the front of the given samples array.
            public int GetData(float[] samples, int readPosition, out int nextReadPosition)
            {
                lock (bufferLock)
                {
                    if (readPosition > bufferSampleCount || readPosition < 0)
                    {
                        nextReadPosition = 0;
                        return 0;
                    }

                    if (readPosition <= BufferPosition)
                    {
                        int samplesToRead = Mathf.Min(bufferSampleCount - readPosition, samples.Length);

                        nextReadPosition = readPosition + samplesToRead;
                        System.Array.Copy(buffer, readPosition, samples, 0, samplesToRead);
                        return samplesToRead;
                    }
                    else
                    {
                        int samplesToRead = Mathf.Min(buffer.Length - readPosition + BufferPosition, samples.Length);
                        int samplesToReadFromEnd = buffer.Length - readPosition;

                        nextReadPosition = samplesToRead - samplesToReadFromEnd;
                        System.Array.Copy(buffer, readPosition, samples, 0, samplesToReadFromEnd);
                        System.Array.Copy(buffer, 0, samples, samplesToReadFromEnd, nextReadPosition);
                        return samplesToRead;
                    }
                }
            }
            
            // Does not wrap any data, returns exactly what's in the buffer.
            public int GetData(float[] samples, int readPosition)
            {
                lock (bufferLock)
                {
                    if (readPosition > bufferSampleCount || readPosition < 0)
                        return 0;

                    int samplesToRead = Mathf.Min(bufferSampleCount - readPosition, samples.Length);
                    System.Array.Copy(buffer, readPosition, samples, 0, samplesToRead);
                    return samplesToRead;
                }
            }

            public AudioClip FlushToClip()
            {
                lock (bufferLock)
                {
                    var clip = AudioClip.Create("ML Buffered AudioClip", bufferSampleCount, channels, sampleRate, false);
                    var data = new float[bufferSampleCount];
                    if (bufferSampleCount < buffer.Length)
                    {
                        System.Array.Copy(buffer, 0, data, 0, bufferSampleCount);
                    }
                    else
                    {
                        int samplesToCopyFromEnd = buffer.Length - BufferPosition;
                        System.Array.Copy(buffer, BufferPosition, data, 0, samplesToCopyFromEnd);
                        System.Array.Copy(buffer, 0, data, samplesToCopyFromEnd, BufferPosition);
                    }
                    clip.SetData(data, 0);
                    BufferPosition = 0;
                    bufferSampleCount = 0;
                    return clip;
                }
            }
        }
    }
}
