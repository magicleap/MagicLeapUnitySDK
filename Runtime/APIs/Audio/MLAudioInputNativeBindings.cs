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

    /// <summary>
    /// Manages Audio.
    /// </summary>
    public sealed partial class MLAudioInput
    {

        /// <summary>
        /// See ml_audio.h for additional comments.
        /// </summary>
        private class NativeBindings : Native.MagicLeapNativeBindings
        {
            /// <summary>
            /// The callback that occurs when an input buffer is available.
            /// </summary>
            /// <param name="handle">Handle used to identify the sound input.</param>
            /// <param name="callback">A pointer to the callback.</param>
            public delegate void MLAudioOnBufferDelegate(ulong handle, IntPtr context);

            /// <summary>
            /// The callback that occurs when the mute state changes for the microphone.
            /// </summary>
            /// <param name="muted">The mute state of the microphone.</param>
            /// <param name="callback">A pointer to the callback.</param>
            public delegate void MLAudioMicMuteCallback([MarshalAs(UnmanagedType.I1)] bool muted, IntPtr callback);

            /// <summary>
            /// Sets the mute state of the microphone.
            /// </summary>
            /// <param name="muted">The mute state of the microphone.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if <c>AudioCaptureMic</c> permission is denied.
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
            /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if <c>AudioCaptureMic</c> permission is denied.
            /// MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.NotImplemented</c>.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioGetMicMute([MarshalAs(UnmanagedType.I1)] out bool isMuted);

            /// <summary>
            /// Register a callback for when the mute state changes for the microphone.
            /// </summary>
            /// <param name="callback">A pointer to the callback.</param>
            /// <param name="data">A generic data pointer passed back to the callback.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
            /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if <c>AudioCaptureMic</c> permission is denied.
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

            /// <summary>
            /// Returns the preferred (i.e. best performing) settings for buffered input.
            /// </summary>
            /// <param name="channelCount">Number of audio channels (e.g. 2 for stereo).</param>
            /// <param name="samplesPerSecond">Sample rate for the buffered input.</param>
            /// <param name="format">Recommended Settings for MLAudioBufferFormat.</param>
            /// <param name="recommendedSizeInBytes">Recommended size for the buffers.</param>
            /// <param name="minimumSizeInBytes">Minimum allowable size for the buffers.</param>
            /// <returns>
            /// MLResult.Result will be MLResult.AllocFailed due to memory allocation failure.
            /// MLResult.Result will be MLResult.InvalidParam due to an invalid parameter.
            /// MLResult.Result will be MLResult.Ok Successfully created new sound input that provides a "mixed capture" stream.
            /// MLResult.Result will be MLResult.PermissionDenied due to lack of permission.
            /// MLResult.Result will be MLResult.UnspecifiedFailure due to an unknown error.
            /// MLResult.Result will be MLAudioResult.InternalConfigError due to an internal configuration error.
            /// MLResult.Result will be MLAudioResult.InvalidBitsPerSample due to an invalid bits per sample.
            /// MLResult.Result will be MLAudioResult.InvalidBufferSize due to an invalid buffer size.
            /// MLResult.Result will be MLAudioResult.InvalidChannelCount due to an invalid channel count.
            /// MLResult.Result will be MLAudioResult.InvalidSampleFormat due to an invalid sample format.
            /// MLResult.Result will be MLAudioResult.InvalidSampleRate due to an invalid sample rate.
            /// MLResult.Result will be MLAudioResult.InvalidValidBits due to an invalid valid bits per sample.
            /// MLResult.Result will be MLAudioResult.NotImplemented  because feature has not been implemented yet.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioGetBufferedInputDefaults(uint channelCount, uint samplesPerSecond, out MLAudioOutput.NativeBindings.MLAudioBufferFormat format, out uint recommendedSizeInBytes, out uint minimumSizeInBytes);

            /// <summary>
            /// Creates a new sound input that provides audio from the mics on the wearable.
            /// </summary>
            /// <param name="captureType">Specifies the type of mic capture created.</param>
            /// <param name="format">MLAudioBufferFormat specifying the audio format of the stream.</param>
            /// <param name="bufferSizeInBytes">Requested size in bytes for each of the two stream buffers.</param>
            /// <param name="callback">Function to use for callback.</param>
            /// <param name="context">A generic data pointer passed back to the callback.</param>
            /// <param name="handle">Handle used in subsequent calls for this sound input.</param>
            /// <returns>
            /// MLResult.Result will be MLResult.AllocFailed due to memory allocation failure.
            /// MLResult.Result will be MLResult.InvalidParam due to an invalid parameter.
            /// MLResult.Result will be MLResult.Ok Successfully created new sound input that provides a mic capture stream.
            /// MLResult.Result will be MLResult.PermissionDenied due to lack of permission.
            /// MLResult.Result will be MLResult.UnspecifiedFailure due to an unknown error.
            /// MLResult.Result will be MLAudioResult.InternalConfigError due to an internal configuration error.
            /// MLResult.Result will be MLAudioResult.InvalidBitsPerSample due to an invalid bits per sample.
            /// MLResult.Result will be MLAudioResult.InvalidBufferSize due to an invalid buffer size.
            /// MLResult.Result will be MLAudioResult.InvalidChannelCount due to an invalid channel count.
            /// MLResult.Result will be MLAudioResult.InvalidSampleFormat due to an invalid sample format.
            /// MLResult.Result will be MLAudioResult.InvalidSampleRate due to an invalid sample rate.
            /// MLResult.Result will be MLAudioResult.InvalidValidBits due to an invalid valid bits per sample.
            /// MLResult.Result will be MLAudioResult.NotImplemented  because feature has not been implemented yet.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioCreateInputFromMicCapture(MicCaptureType captureType, in MLAudioOutput.NativeBindings.MLAudioBufferFormat format, uint bufferSizeInBytes, MLAudioOnBufferDelegate callback, IntPtr context, out ulong handle);

            /// <summary>
            /// Returns a full buffer containing captured audio data.
            /// </summary>
            /// <param name="handle">Used to identify the sound input.</param>
            /// <param name="buffer">Contains the buffer to read from.</param>
            /// <returns>
            /// MLResult.Result will be MLResult.InvalidParam due to an invalid parameter.
            /// MLResult.Result will be MLResult.Ok Successfully returned full buffer containing captured audio data.
            /// MLResult.Result will be MLResult.UnspecifiedFailure due to an unknown error.
            /// MLResult.Result will be MLResult.AudioBufferNotReady  because buffer was not ready.
            /// MLResult.Result will be MLResult.AudioHandleNotFound due to a missing handle.
            /// MLResult.Result will be MLResult.AudioInternalConfigError due to an internal configurations error.
            /// MLResult.Result will be MLResult.NotImplemented  because feature has not been implemented yet.
            /// MLResult.Result will be MLResult.OperationUnavailable because operation is unavailable.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioGetInputBuffer(ulong handle, out MLAudioOutput.NativeBindings.MLAudioBuffer buffer);


            /// <summary>
            /// Releases a buffer once it has been read.
            /// </summary>
            /// <param name="handle">Used to identify the sound input.</param>
            /// <returns>
            /// MLResult.Result will be MLResult.Ok Successfully released buffer.
            /// MLResult.Result will be MLResult.UnspecifiedFailure due to an unknown error.
            /// MLResult.Result will be MLResult.AudioBufferNotReady  because buffer was not ready.
            /// MLResult.Result will be MLResult.AudioHandleNotFound due to a missing handle.
            /// MLResult.Result will be MLResult.AudioInternalConfigError due to an internal configurations error.
            /// MLResult.Result will be MLResult.NotImplemented  because feature has not been implemented yet.
            /// MLResult.Result will be MLResult.OperationUnavailable  because operation is unavailable.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioReleaseInputBuffer(ulong handle);

            /// <summary>
            /// Starts capture for a sound input.
            /// </summary>
            /// <param name="handle">Used to identify the sound input.</param>
            /// <returns>
            /// MLResult.Result will be MLResult.Ok Successfully started capture for sound input.
            /// MLResult.Result will be MLResult.PermissionDenied due to lack of permission.
            /// MLResult.Result will be MLResult.UnspecifiedFailure due to an unknown error.
            /// MLResult.Result will be MLResult.AudioHandleNotFound due to a missing handle.
            /// MLResult.Result will be MLResult.NotImplemented because feature has not been implemented yet.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioStartInput(ulong handle);

            /// <summary>
            /// Stops capture for a sound input.
            /// </summary>
            /// <param name="handle">Used to identify the sound input.</param>
            /// <returns>
            /// MLResult.Result will be MLResult.Ok Successfully stopped capture for sound input.
            /// MLResult.Result will be MLResult.PermissionDenied due to lack of permission.
            /// MLResult.Result will be MLResult.UnspecifiedFailure due to an unknown error.
            /// MLResult.Result will be MLResult.AudioHandleNotFound due to a missing handle.
            /// MLResult.Result will be MLResult.NotImplemented because feature has not been implemented yet.
            /// </returns>
            [DllImport(AudioPlayerDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLAudioStopInput(ulong handle);

        }
    }
}
