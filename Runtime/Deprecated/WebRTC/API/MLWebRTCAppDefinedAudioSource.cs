// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap.Native;

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
            private MLAudioOutput.NativeBindings.MLAudioBuffer audioBufferNative;
            private int audioBufferCapacity;

            protected AppDefinedAudioSource(string trackId)
                : base(trackId)
            {
                this.TrackType = Type.Audio;
            }

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
                if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.InitializeAppDefinedAudioSource)))
                {
                    appDefinedAudioSource.audioBufferNative = new MLAudioOutput.NativeBindings.MLAudioBuffer();
                    appDefinedAudioSource.audioBufferCapacity = 0; 
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
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCSourceAppDefinedAudioSourcePushData));
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
        }
    }
}
