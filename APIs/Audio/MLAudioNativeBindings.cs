// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLAudioNativeBindings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

// Disable warnings about missing documentation for native interop.
#pragma warning disable 1591
namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Manages Audio.
    /// </summary>
    public sealed partial class MLAudio : MLAPISingleton<MLAudio>
    {
        /// <summary>
        /// See ml_audio.h for additional comments.
        /// </summary>
        internal class NativeBindings
        {
            /// <summary>
            /// MLAudio library name.
            /// </summary>
            private const string AudioPlayerDLL = "ml_audio";

            /// <summary>
            /// The callback that occurs when the mute state changes for the microphone.
            /// </summary>
            /// <param name="muted">The mute state of the microphone.</param>
            /// <param name="callback">A pointer to the callback.</param>
            public delegate void MLAudioMicMuteCallback([MarshalAs(UnmanagedType.I1)] bool muted, IntPtr callback);

            /// <summary>
            /// The callback that occurs when the master volume changes.
            /// </summary>
            /// <param name="volume">The value of the master volume.</param>
            /// <param name="callback">A pointer to the callback.</param>
            public delegate void MLAudioMasterVolumeChangedCallback(float volume, IntPtr callback);

            /// <summary>
            /// Gets the current audio output device.
            /// </summary>
            /// <param name="device">The audio output device.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if input parameter is invalid.
            /// MLResult.Result will be <c>MLResult.Code.NotImplemented</c>.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioGetOutputDevice(out MLAudio.Device device);

            /// <summary>
            /// Gets the value of the master volume.
            /// </summary>
            /// <param name="volume">The value of the master volume.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if input parameter is invalid.
            /// MLResult.Result will be <c>MLResult.Code.NotImplemented</c>.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioGetMasterVolume(out float volume);

            /// <summary>
            /// Sets the mute state of the microphone.
            /// </summary>
            /// <param name="muted">The mute state of the microphone.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if <c>AudioCaptureMic</c> privilege is denied.
            /// MLResult.Result will be <c>MLResult.Code.NotImplemented</c>.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioSetMicMute([MarshalAs(UnmanagedType.I1)] bool muted);

            /// <summary>
            /// Gets the mute state of the microphone.
            /// </summary>
            /// <param name="isMuted">The mute state of the microphone.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if input parameter is invalid.
            /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if <c>AudioCaptureMic</c> privilege is denied.
            /// MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.NotImplemented</c>.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioIsMicMuted([MarshalAs(UnmanagedType.I1)] out bool isMuted);

            /// <summary>
            /// Registers a callback for when the master volume changes.
            /// </summary>
            /// <param name="callback">A pointer to the callback.</param>
            /// <param name="data">A generic data pointer passed back to the callback.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.NotImplemented</c>.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioSetMasterVolumeCallback(MLAudioMasterVolumeChangedCallback callback, IntPtr data);

            /// <summary>
            /// Register a callback for when the mute state changes for the microphone.
            /// </summary>
            /// <param name="callback">A pointer to the callback.</param>
            /// <param name="data">A generic data pointer passed back to the callback.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if <c>AudioCaptureMic</c> privilege is denied.
            /// MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.NotImplemented</c>.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioSetMicMuteCallback(MLAudioMicMuteCallback callback, IntPtr data);

            /// <summary>
            /// Gets the result string for a MLResult.Code.
            /// </summary>
            /// <param name="result">The MLResult.Code to be requested.</param>
            /// <returns>A pointer to the result string.</returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLAudioGetResultString(MLResult.Code result);

            [StructLayout(LayoutKind.Sequential)]
            public struct MLAudioBufferFormat
            {
                public readonly uint ChannelCount;
                public readonly uint SamplesPerSecond;
                public readonly uint BitsPerSample;
                public readonly uint ValidBitsPerSample;
                public readonly MLAudio.SampleFormatType SampleFormat;
                public readonly uint Reserved;

                public MLAudioBufferFormat(MLAudio.BufferFormat bufferFormat)
                {
                    this.ChannelCount = bufferFormat.ChannelCount;
                    this.SamplesPerSecond = bufferFormat.SamplesPerSecond;
                    this.BitsPerSample = bufferFormat.BitsPerSample;
                    this.ValidBitsPerSample = bufferFormat.ValidBitsPerSample;
                    this.SampleFormat = bufferFormat.SampleFormat;
                    this.Reserved = 0;
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MLAudioBuffer
            {
                public readonly IntPtr Ptr;
                public readonly uint Size;

                public MLAudioBuffer(IntPtr ptr, uint size)
                {
                    this.Ptr = ptr;
                    this.Size = size;
                }
            }

            /// <summary>
            /// Properties specifying send levels for a spatial sound.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLAudioSpatialSoundSendLevels
            {
                /// <summary>
                /// Volume scale (0-1) for all freqs.
                /// </summary>
                private readonly float gain;

                /// <summary>
                /// Volume scale (0-1) for low freqs.
                /// </summary>
                private readonly float gainLF;

                /// <summary>
                /// Volume scale (0-1) for mid freqs.
                /// </summary>
                private readonly float gainMF;

                /// <summary>
                /// Volume scale (0-1) for high freqs.
                /// </summary>
                private readonly float gainHF;

                public MLAudioSpatialSoundSendLevels(SpatialSound.SendLevels sendLevels)
                {
                    this.gain = sendLevels.Gain;
                    this.gainLF = sendLevels.GainLF;
                    this.gainMF = sendLevels.GainMF;
                    this.gainHF = sendLevels.GainHF;
                }
            }

            /// <summary>
            /// Properties specifying the distance response of a spatial sound.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLAudioSpatialSoundDistanceProperties
            {
                /// <summary>
                /// Distance where sound is at full volume.
                /// </summary>
                private readonly float minDistance;

                /// <summary>
                /// Distance beyond which sound gets no quieter.
                /// </summary>
                private readonly float maxDistance;

                /// <summary>
                /// Modification to real-world distance response.
                /// </summary>
                private readonly float rolloffFactor;

                public MLAudioSpatialSoundDistanceProperties(SpatialSound.DistanceProperties distanceProperties)
                {
                    this.minDistance = distanceProperties.MinDistance;
                    this.maxDistance = distanceProperties.MaxDistance;
                    this.rolloffFactor = distanceProperties.RolloffFactor;
                }
            }

            /// <summary>
            /// Properties specifying the directivity of a spatial sound.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLAudioSpatialSoundRadiationProperties
            {
                /// <summary>
                /// Inner cone angle (0-360); radiation unaffected.
                /// </summary>
                private readonly float innerAngle;

                /// <summary>
                /// Outer cone angle (0-360); directivity at maximum.
                /// </summary>
                private readonly float outerAngle;

                /// <summary>
                /// Volume scale (0-1) beyond outer cone for all freqs.
                /// </summary>
                private readonly float outerGain;

                /// <summary>
                /// Volume scale (0-1) beyond outer cone for low freqs.
                /// </summary>
                private readonly float outerGainLF;

                /// <summary>
                /// Volume scale (0-1) beyond outer cone for mid freqs.
                /// </summary>
                private readonly float outerGainMF;

                /// <summary>
                /// Volume scale (0-1) beyond outer cone for high freqs.
                /// </summary>
                private readonly float outerGainHF;

                public MLAudioSpatialSoundRadiationProperties(SpatialSound.RadiationProperties radiationProperties)
                {
                    this.innerAngle = radiationProperties.InnerAngle;
                    this.outerAngle = radiationProperties.OuterAngle;
                    this.outerGain = radiationProperties.OuterGain;
                    this.outerGainLF = radiationProperties.OuterGainLF;
                    this.outerGainMF = radiationProperties.OuterGainMF;
                    this.outerGainHF = radiationProperties.OuterGainHF;
                }
            }
        }
    }
}
