// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLAudio.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityEngine.XR.MagicLeap
{
    public sealed partial class MLAudio
    {
        /// <summary>
        /// Possible sample formats for input and output streams.
        /// </summary>
        public enum SampleFormatType : uint
        {
            Int,
            Float
        }

        /// <summary>
        /// Buffer format settings for input and output streams.
        /// </summary>
        public struct BufferFormat
        {
            /// <summary>
            /// Number of channels.
            /// </summary>
            public uint ChannelCount { get; set; }

            /// <summary>
            /// Sample rate.
            /// </summary>
            public uint SamplesPerSecond { get; set; }

            /// <summary>
            /// Number of bits per sample.
            /// </summary>
            public uint BitsPerSample { get; set; }

            /// <summary>
            /// Number of bits used per sample.
            /// </summary>
            public uint ValidBitsPerSample { get; set; }

            /// <summary>
            /// Integer or float.
            /// </summary>
            public SampleFormatType SampleFormat { get; set; }

#if PLATFORM_LUMIN
            internal BufferFormat(NativeBindings.MLAudioBufferFormat bufferFormatNative)
            {
                this.ChannelCount = bufferFormatNative.ChannelCount;
                this.SamplesPerSecond = bufferFormatNative.SamplesPerSecond;
                this.BitsPerSample = bufferFormatNative.BitsPerSample;
                this.ValidBitsPerSample = bufferFormatNative.ValidBitsPerSample;
                this.SampleFormat = bufferFormatNative.SampleFormat;
            }
#endif

            public BufferFormat(uint channelCount, uint samplesPerSecond, uint bitsPerSample, SampleFormatType sampleFormat = SampleFormatType.Float)
            {
                this.ChannelCount = channelCount;
                this.SamplesPerSecond = samplesPerSecond;
                this.BitsPerSample = bitsPerSample;
                this.ValidBitsPerSample = bitsPerSample;
                this.SampleFormat = sampleFormat;
            }

            public override string ToString()
            {
                return $"ChannelCount: {ChannelCount}, SamplesPerSec: {SamplesPerSecond}, BitsPerSample: {BitsPerSample}, ValidBitsPerSample: {ValidBitsPerSample}, SampleFormat: {SampleFormat}";
            }
        }

        /// <summary>
        /// An audio buffer for passing data from input streams or to output streams.
        /// </summary>
        public struct Buffer
        {
            /// <summary>
            /// Pointer to audio data. Only valid when received on native callback threads.
            /// </summary>
            public IntPtr NativeDataPtr;

            /// <summary>
            /// Size of audio data in bytes. Only valid when received on native callback threads.
            /// </summary>
            public uint Size;

            /// <summary>
            /// Audio samples in this buffer.
            /// </summary>
            public float[] Samples;

            /// <summary>
            /// Audio buffer format.
            /// </summary>
            public BufferFormat Format;

#if PLATFORM_LUMIN
            internal Buffer(NativeBindings.MLAudioBuffer buffer, NativeBindings.MLAudioBufferFormat bufferFormat, bool copyToManagedMemory = false)
            {
                this.NativeDataPtr = buffer.Ptr;
                this.Size = buffer.Size;
                this.Format = new BufferFormat(bufferFormat);
                this.Samples = null;

                if (copyToManagedMemory)
                {
                    this.Samples = ConvertToManagedFloatSamples(Size, NativeDataPtr, Format);
                }
            }

            /// <summary>
            /// Copy the provided unmanaged audio buffer to managed memory and convert the samples to float.
            /// </summary>
            /// <param name="size">Number of bytes allocated for the unmanaged buffer</param>
            /// <param name="bufferPtr">Pointer to the unmanaged buffer</param>
            /// <param name="format">Audio format for the unmanaged buffer</param>
            /// <returns>Array of managed memory containing float samples</returns>
            public static float[] ConvertToManagedFloatSamples(uint size, IntPtr bufferPtr, BufferFormat format)
            {
                uint numSamples = size / (format.BitsPerSample / 8);
                float[] samples = new float[numSamples];
                if (format.SampleFormat == SampleFormatType.Float)
                {
                    Marshal.Copy(bufferPtr, samples, 0, (int)numSamples);
                }
                else if (format.SampleFormat == SampleFormatType.Int)
                {
                    if (format.BitsPerSample == 8)
                    {
                        byte[] origSamples = new byte[numSamples];
                        Marshal.Copy(bufferPtr, origSamples, 0, (int)numSamples);

                        for (uint i = 0; i < numSamples; ++i)
                        {
                            samples[i] = origSamples[i] / (float)byte.MaxValue;
                        }
                    }
                    else if (format.BitsPerSample == 16)
                    {
                        short[] origSamples = new short[numSamples];
                        Marshal.Copy(bufferPtr, origSamples, 0, (int)numSamples);

                        for (uint i = 0; i < numSamples; ++i)
                        {
                            samples[i] = origSamples[i] / (float)short.MaxValue;
                        }
                    }
                    else if (format.BitsPerSample == 32)
                    {
                        int[] origSamples = new int[numSamples];
                        Marshal.Copy(bufferPtr, origSamples, 0, (int)numSamples);

                        for (uint i = 0; i < numSamples; ++i)
                        {
                            samples[i] = origSamples[i] / (float)int.MaxValue;
                        }
                    }
                }

                return samples;
            }
#endif

            public Buffer(BufferFormat format, float[] samples)
            {
                this.NativeDataPtr = IntPtr.Zero;
                this.Size = 0;
                this.Samples = samples;
                this.Format = format;
            }
        }
    }
}
