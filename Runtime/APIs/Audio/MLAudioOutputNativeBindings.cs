// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

// Disable warnings about missing documentation for native interop.
#pragma warning disable 1591
namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    using MagicLeap.Native;

    /// <summary>
    /// Manages Audio.
    /// </summary>
    public sealed partial class MLAudioOutput
    {
        /// <summary>
        /// See ml_audio.h for additional comments.
        /// </summary>
        internal class NativeBindings : MagicLeapNativeBindings
        {
            /// <summary>
            /// The callback that occurs when the master volume changes.
            /// </summary>
            /// <param name="volume">The value of the master volume.</param>
            /// <param name="callback">A pointer to the callback.</param>
            public delegate void MLAudioMasterVolumeChangedCallback(float volume, IntPtr callback);

            /// <summary>
            /// Callback to notify when a media event has occurred.
            /// </summary>
            /// <param name="volume">The new media event.</param>
            /// <param name="callback">A pointer to the callback.</param>
            public delegate void MLAudioMediaEventCallback(MediaEvent mediaEvent, IntPtr callback);

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
            public static extern MLResult.Code MLAudioGetOutputDevice(out MLAudioOutput.Device device);

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
            /// Gets the result string for a MLResult.Code.
            /// </summary>
            /// <param name="result">The MLResult.Code to be requested.</param>
            /// <returns>A pointer to the result string.</returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLAudioGetResultString(MLResult.Code result);

            /// <summary>
            ///  Enables/disables 3D audio processing for a sound output.
            /// </summary>
            /// <param name="handle">MLHandle used to identify the sound output.</param>
            /// <param name="enable">Determines whether 3D processing is on or off.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.AudioHandleNotFound </c> if handle was not found.
            /// MLResult.Result will be <c>MLResult.Code.AudioInternalConfigError </c> if internal config error.
            /// MLResult.Result will be <c>MLResult.Code.NotImplemented</c>.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioSetSpatialSoundEnable(ulong handle, bool enable);

            /// <summary>
            ///  Enables/disables 3D audio processing for a sound output.
            /// </summary>
            /// <param name="handle">MLHandle used to identify the sound output.</param>
            /// <param name="enabled">Indicates whether 3D processing is on or off.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.AudioHandleNotFound </c> if handle was not found.
            /// MLResult.Result will be <c>MLResult.Code.AudioInternalConfigError </c> if internal config error.
            /// MLResult.Result will be <c>MLResult.Code.NotImplemented</c>.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioGetSpatialSoundEnable(ulong handle, out bool enabled);

            /// <summary>
            ///  Sets the 3D position for one channel of a sound output.
            /// </summary>
            /// <param name="handle">MLHandle used to identify the sound output.</param>
            /// <param name="channel">Selects the channel whose position is being set.</param>
            /// <param name="position">Struct to set the position</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.AudioHandleNotFound </c> if handle was not found.
            /// MLResult.Result will be <c>MLResult.Code.AudioInternalConfigError </c> if internal config error.
            /// MLResult.Result will be <c>MLResult.Code.NotImplemented</c>.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioSetSpatialSoundPosition(ulong handle, uint channel, [In] ref MagicLeapNativeBindings.MLVec3f position);
            /// <summary>
            ///  Returns the 3D position for one channel of a sound output.
            /// </summary>
            /// <param name="handle">MLHandle used to identify the sound output.</param>
            /// <param name="channel">Selects the channel whose position is being set.</param>
            /// <param name="position">Struct to set the position</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.AudioHandleNotFound </c> if handle was not found.
            /// MLResult.Result will be <c>MLResult.Code.AudioInternalConfigError </c> if internal config error.
            /// MLResult.Result will be <c>MLResult.Code.NotImplemented</c>.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioGetSpatialSoundPosition(ulong handle, uint channel, out MagicLeapNativeBindings.MLVec3f position);

            /// <summary>
            /// Registers a callback for user-issued media control events.  Events that can result in a callback being called include:
            /// pressing a play button; pressing a stop button; pressing a pause button; pressing a next track button, pressing a
            /// previous track button; The particular event type (MLAudioMediaEvent) that caused the callback is passed to the callback as its
            /// first argument.  The 'context' argument is passed back to the callback as its second argument.
            /// </summary>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioSetMediaEventCallback(MLAudioMediaEventCallback callback, IntPtr context);

            /// <summary>
            /// Setting this option on a sound output causes its output to bypass master volume, making
            /// it effectively "always audible" (assuming it is neither muted nor set to zero volume
            /// on a per-sound basis). This option can only be set on medical-enabled devices (60601
            /// compliant), and will only work for non-spatial sounds.Non-spatial sound parameters
            /// such as volume, mute, pitch and looping are still in effect for sounds that are
            /// bypassing master volume.
            /// </summary>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioSetSoundBypassesMasterVolume(ulong audioHandle, [MarshalAs(UnmanagedType.I1)] bool isBypassing);

            /// <summary>
            /// Queries whether a sound output is exempt from attenuation due to master volume.
            /// This call reports whether the output from a sound output is bypassing master volume,
            /// making it effectively "always audible" (assuming it is neither muted nor set to zero volume
            /// on a per-sound basis). This option can only be set on medical-enabled devices(60601
            /// compliant), and will only work for non-spatial sounds.Non-spatial sound parameters
            /// such as volume, mute, pitch and looping are still in effect for sounds that are
            /// bypassing master volume.
            /// </summary>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioGetSoundBypassesMasterVolume(ulong audioHandle, [MarshalAs(UnmanagedType.I1)] out bool isBypassing);

            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            internal static extern ulong MLUnityAudioGetHandle();

            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLAudioBufferFormat
            {
                public readonly uint ChannelCount;
                public readonly uint SamplesPerSecond;
                public readonly uint BitsPerSample;
                public readonly uint ValidBitsPerSample;
                public readonly MLAudioOutput.SampleFormatType SampleFormat;
                public readonly MLAudioOutput.ChannelFormatType ChannelFormat;

                public MLAudioBufferFormat(MLAudioOutput.BufferFormat bufferFormat)
                {
                    this.ChannelCount = bufferFormat.ChannelCount;
                    this.SamplesPerSecond = bufferFormat.SamplesPerSecond;
                    this.BitsPerSample = bufferFormat.BitsPerSample;
                    this.ValidBitsPerSample = bufferFormat.ValidBitsPerSample;
                    this.SampleFormat = bufferFormat.SampleFormat;
                    this.ChannelFormat = bufferFormat.ChannelFormat;
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MLAudioBuffer
            {
                public IntPtr Ptr;
                public uint Size;

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
