// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCAppDefinedVideoSource.cs" company="Magic Leap, Inc">
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
    using System.Runtime.InteropServices;
#if UNITY_MAGICLEAP || UNITY_ANDROID
    using UnityEngine.XR.MagicLeap.Native;
#endif

    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Class that represents an app defined video source that can be used by the MLWebRTC API.
        /// </summary>
        public abstract partial class AppDefinedAudioSource : AppDefinedSource
        {
            private bool isEnabled = false;

#if UNITY_MAGICLEAP || UNITY_ANDROID
            private MLAudioOutput.NativeBindings.MLAudioBuffer audioBufferNative;
#endif
            private int audioBufferCapacity;
            private short[] intSamples;
            private int nextSampleIndex;

            protected AppDefinedAudioSource(string trackId)
                : base(trackId)
            {
                this.TrackType = Type.Audio;
            }

#if UNITY_MAGICLEAP || UNITY_ANDROID
            /// <summary>
            /// Initializes the given AppDefinedVideoSource object.
            /// </summary>
            /// <param name="appDefinedAudioSource">The AppDefinedVideoSource object to initialize.</param>
            /// <param name="result">The MLResult object of the inner platform call(s).</param>
            /// <returns>An AppDefinedVideoSource object with the given handle.</returns>
            public static MLResult InitializeLocal(AppDefinedAudioSource appDefinedAudioSource)
            {
                MLWebRTC.Instance.localTracks.Add(appDefinedAudioSource);
                MLResult.Code resultCode = NativeBindings.InitializeAppDefinedAudioSource(appDefinedAudioSource);
                if (MLResult.DidNativeCallSucceed(resultCode, "InitializeAppDefinedVideoSource()"))
                {
                    appDefinedAudioSource.audioBufferNative = new MLAudioOutput.NativeBindings.MLAudioBuffer();
                    appDefinedAudioSource.audioBufferCapacity = 0;
                    appDefinedAudioSource.intSamples = null;
                    appDefinedAudioSource.nextSampleIndex = 0;
                    appDefinedAudioSource.isEnabled = true;
                }

                return MLResult.Create(resultCode);
            }

            public MLResult PushData(short[] audioBuffer, in MLAudioOutput.BufferFormat audioBufferFormat)
            {
                if (!isEnabled)
                {
                    // TODO : nope nope nope nope
                    return MLResult.Create(MLResult.Code.UnspecifiedFailure);
                }

                if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Handle is invalid.");
                }

                int numBytes = audioBuffer.Length * sizeof(short);

                // This is number of bytes we're sending this time
                audioBufferNative.Size = (uint)numBytes;

                if (audioBufferCapacity < numBytes)
                {
                    if (audioBufferNative.Ptr != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(audioBufferNative.Ptr);
                    }

                    audioBufferNative.Ptr = Marshal.AllocHGlobal(numBytes);
                    audioBufferCapacity = numBytes;
                }

                Marshal.Copy(audioBuffer, 0, audioBufferNative.Ptr, audioBuffer.Length);
                MLResult.Code resultCode = MLResult.Code.Ok;
                MLAudioOutput.NativeBindings.MLAudioBufferFormat audioBufferFormatNative = new MLAudioOutput.NativeBindings.MLAudioBufferFormat(audioBufferFormat);
                resultCode = NativeBindings.MLWebRTCSourceAppDefinedAudioSourcePushData(Handle, in audioBufferNative, in audioBufferFormatNative);
                return MLResult.Create(resultCode);
            }

            public override MLResult DestroyLocal()
            {
                MLResult result = base.DestroyLocal();
                if (audioBufferNative.Ptr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(audioBufferNative.Ptr);
                    audioBufferNative.Ptr = IntPtr.Zero;
                }
                return result;
            }
#endif
        }
    }
}
