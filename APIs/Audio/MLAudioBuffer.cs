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

            public override bool Equals(object obj)
            {
                if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                {
                    return false;
                }

                BufferFormat format = (BufferFormat)obj;
                return (this.ChannelCount == format.ChannelCount) &&
                    (this.SamplesPerSecond == format.SamplesPerSecond) &&
                    (this.BitsPerSample == format.BitsPerSample) &&
                    (this.ValidBitsPerSample == format.ValidBitsPerSample) &&
                    (this.SampleFormat == format.SampleFormat);
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

            private static readonly Dictionary<uint, Func<IntPtr, float>> UnmanagedToFloat = new Dictionary<uint, Func<IntPtr, float>>()
            {
                { 8,  (ptr) => Marshal.ReadByte (ptr) / (float)byte.MaxValue  },
                { 16, (ptr) => Marshal.ReadInt16(ptr) / (float)short.MaxValue },
                { 32, (ptr) => Marshal.ReadInt32(ptr) / (float)int.MaxValue   },
                { 64, (ptr) => Marshal.ReadInt64(ptr) / (float)long.MaxValue  }
            };

            /// <summary>
            /// Copy the provided unmanaged audio buffer to managed memory and convert the samples to float.
            /// </summary>
            /// <param name="size">Number of bytes allocated for the unmanaged buffer</param>
            /// <param name="bufferPtr">Pointer to the unmanaged buffer</param>
            /// <param name="format">Audio format for the unmanaged buffer</param>
            /// <returns>Array of managed memory containing float samples</returns>
            public static float[] ConvertToManagedFloatSamples(uint size, IntPtr bufferPtr, BufferFormat format)
            {
                uint bytesPerSample = format.BitsPerSample / 8;
                uint numSamples = size / bytesPerSample;
                float[] samples = new float[numSamples];
                if (format.SampleFormat == SampleFormatType.Float)
                {
                    Marshal.Copy(bufferPtr, samples, 0, (int)numSamples);
                }
                else if (format.SampleFormat == SampleFormatType.Int)
                {
                    var toFloat = UnmanagedToFloat[format.BitsPerSample];
                    for (uint i = 0; i < numSamples; ++i)
                    {
                        samples[i] = toFloat(bufferPtr);
                        bufferPtr = IntPtr.Add(bufferPtr, (int)bytesPerSample);
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
