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
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Class that represents an audio sink used by the MLWebRTC API.
        /// </summary>
        public partial class AudioSink : Sink
        {
            private NativeBindings.MLWebRTCAudioSinkPosition nativePosition;
            private NativeBindings.MLWebRTCAudioSinkOrientation nativeOrientation;
            private NativeBindings.MLWebRTCAudioSinkSoundLevels nativeSoundLevels;
            private NativeBindings.MLWebRTCAudioSinkSoundDistance nativeDistance;
            private NativeBindings.MLWebRTCAudioSinkSoundRadiation nativeRadiation;

            /// <summary>
            /// Behavior of the sink if audio buffer data is provided to the app in a callback.
            /// </summary>
            public enum BufferNotifyMode : uint
            {
                /// <summary>
                /// Don't provide app with audio buffers, consume them in the underlying
                /// lib and play directly on the device.
                /// </summary>
                None,

                /// <summary>
                /// Provide the app with audio buffers in a callback, AND also play the
                /// audio directly on the device. Ths is useful where apps want to use
                /// the buffers to calculate something (like RMS) but not take the
                /// responsibility of actually playing the audio.
                /// </summary>
                NotifyAndPlay,

                /// <summary>
                /// Provide the app with audio buffers in a callback. The underlyin
                /// library will NOT play the audio. It is the app's responsibility
                /// to submit the audio buffers to the device.
                /// </summary>
                NotifyOnly
            }

            /// <summary>
            /// Status of the underlying Audio Service the buffers are submitted & soundfield attributes applied to.
            /// </summary>
            public enum ServiceStatus : uint
            {
                /// <summary>
                /// The audio service has been initialized but not ready to accept updates to any attributes (soundfield or volume).
                /// </summary>
                Initial,

                /// <summary>
                /// The audio service has started. It is now safe to update the audio attributes (soundfield and volume).
                /// </summary>
                Started,

                /// <summary>
                /// Audio service has failed. This will cause disruption in audio playback. Audio attributes (soundfield or volume)
                /// should not be set after receiving this status update.
                /// </summary>
                Failed,

                /// <summary>
                /// Audio service has stopped. Audio attributes (soundfield or volume) should not be set after receiving this status update.
                /// </summary>
                Stopped,

                /// <summary>
                /// Audio service status is unknown. Audio attributes (soundfield or volume) should not be set after receiving this status update.
                /// </summary>
                Unknown
            }

            /// <summary>
            /// Delegate to provide audio buffer data received for this audio sink.
            /// </summary>
            /// <param name="bufferFormat">Format of the audio buffer</param>
            /// <param name="buffer">Audio buffer</param>
            public delegate void OnAudioDataAvailableDelegate(MLAudioOutput.Buffer buffer);

            /// <summary>
            /// Delegate to provide the current audio service status. Apps should use
            /// this status to determine whether its safe to set the audio attriutes
            /// like soundfield parameters and volume.
            /// </summary>
            /// <param name="status">The current status of the audio service</param>
            public delegate void OnAudioServiceStatusChangedDelegate(ServiceStatus status);

            /// <summary>
            /// Audio buffer notification mode for this AudioSink
            /// </summary>
            public BufferNotifyMode Mode { get; private set; }

            /// <summary>
            /// Current status of the underlying audio service.
            /// Apps should use this status to determine whether
            /// its safe to set the audio attriutes like soundfield
            /// parameters and volume.
            /// </summary>
            public ServiceStatus CurrentServiceStatus { get; private set; }

            /// <summary>
            /// Raw audio data received in the OnAudioDataAvailable_NativeCallbackThread delegate
            /// should be converted to float samples and copied from unmanaged to managed memory.
            /// Audio data received in OnAudioDataAvailable will always be available in managed memory.
            /// This is set to true by default. Disable this flag if you experience too many
            /// garbage collection invocations for MLAudio.Buffer.Samples & handle the audio data
            /// conversion from the unmanaged to managed and from Int to Float yourself.
            /// </summary>
            public bool CopyRawAudioDataToManagedMemory { get; set; } = true;

            /// <summary>
            /// Delegate invoked on the native callback thread to provide the audio buffer data for this sink.
            /// MLAudio.Buffer.NativeDataPtr is valid for this delegate.
            /// </summary>
            public event OnAudioDataAvailableDelegate OnAudioDataAvailable_NativeCallbackThread;

            /// <summary>
            /// Delegate invoked on the main thread to provide the audio buffer data for this sink.
            /// MLAudio.Buffer.NativeDataPtr is NOT valid for this delegate.
            /// Use the managed MLAudio.Buffer.Samples array instead.
            /// </summary>
            public event OnAudioDataAvailableDelegate OnAudioDataAvailable;

            /// <summary>
            /// Delegate invoked on the main thread to notify the app about the current
            /// status of the underlying audio service. Apps should use this tatus
            /// to determine whether its safe to set the audio attriutes like soundfield
            /// parameters and volume.
            /// </summary>
            public event OnAudioServiceStatusChangedDelegate OnAudioServiceStatusChanged;

            /// <summary>
            /// The handle for this managed object.
            /// </summary>
            private GCHandle gcHandle;

            /// <summary>
            /// Initializes a new instance of the <see cref="AudioSink" /> class.
            /// </summary>
            internal AudioSink()
            {
                this.Type = MediaStream.Track.Type.Audio;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="AudioSink" /> class.
            /// </summary>
            internal AudioSink(ulong handle, BufferNotifyMode mode) : base(handle)
            {
                this.Type = MediaStream.Track.Type.Audio;
                this.Mode = mode;
                this.CurrentServiceStatus = ServiceStatus.Unknown;
                this.gcHandle = GCHandle.Alloc(this);
            }

            /// <summary>
            /// Creates an initialized AudioSink object.
            /// </summary>
            /// <param name="result">The MLResult object of the inner platform call(s).</param>
            /// <returns> An initialized AudioSink object.</returns>
            public static AudioSink Create(out MLResult result, BufferNotifyMode mode = BufferNotifyMode.None)
            {
                AudioSink audioSink = null;

                List<MLWebRTC.Sink> sinks = MLWebRTC.Instance.sinks;
                ulong handle = MagicLeapNativeBindings.InvalidHandle;
                // We have a chicken-and-egg problem here. We need the audioSink object in order to create the
                // userContext ptr from it GCHandle, which will later be used in the audio data callback to
                // invoke the delegate on this particular object.
                // So, create the AudioSink obj with an invalid handle for now, and then update it if the 
                // native sink creation is successful.
                audioSink = new AudioSink(handle, mode);

                NativeBindings.MLWebRTCAudioSinkParams sinkParams = new NativeBindings.MLWebRTCAudioSinkParams(audioSink);
                MLResult.Code resultCode = NativeBindings.MLWebRTCAudioSinkCreateEx(ref sinkParams, out handle);
                if (!MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCAudioSinkCreateEx)))
                {
                    result = MLResult.Create(resultCode);
                    return null;
                }

                audioSink.Handle = handle;

                if (MagicLeapNativeBindings.MLHandleIsValid(audioSink.Handle))
                {
                    sinks.Add(audioSink);
                }

                result = MLResult.Create(resultCode);

                return audioSink;
            }

            /// <summary>
            /// Sets the stream of the audio sink.
            /// </summary>
            /// <param name="stream">The stream to use.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            public MLResult SetStream(MediaStream stream)
            {
                if (this.Stream == stream)
                {
                    return MLResult.Create(MLResult.Code.InvalidParam);
                }

                this.Stream = stream;
                if (this.Stream == null)
                {
                    return this.SetTrack(null);
                }

                return this.SetTrack(this.Stream.ActiveAudioTrack);
            }

            /// <summary>
            /// Sets the volume of the audio sink for the specified channel.
            /// The range of the volume is 0 to 8, with 0 for silence, 1 for unity gain, and 8 for 8x gain.
            /// </summary>
            /// <param name="volume">The volume of the audio sink.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
            /// </returns>
            public MLResult SetVolume(float volume)
            {
                MLResult.Code resultCode = NativeBindings.MLWebRTCAudioSinkSetSoundVolume(this.Handle, volume);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCAudioSinkSetSoundVolume));
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Sets the world position of the audio sink for the specified channel.
            /// </summary>
            /// <param name="position">The position to set the audio sink to.</param>
            /// <param name="channel">Channel to set this spatialization property for. Passing -1 sets it for all available channels.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
            /// </returns>
            public MLResult SetPosition(Vector3 position, int channel = -1)
            {
                nativePosition.Update(position, channel);
                MLResult.Code resultCode = NativeBindings.MLWebRTCAudioSinkSetPositionEx(this.Handle, ref nativePosition);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCAudioSinkSetPositionEx));
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Sets the world orientation of the audio sink for the specified channel.
            /// </summary>
            /// <param name="orientation">The orientation of the audio sink.</param>
            /// <param name="channel">Channel to set this spatialization property for. Passing -1 sets it for all available channels.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
            /// </returns>
            public MLResult SetOrientation(Quaternion orientation, int channel = -1)
            {
                nativeOrientation.Update(orientation, channel);
                MLResult.Code resultCode = NativeBindings.MLWebRTCAudioSinkSetSoundOrientation(this.Handle, ref nativeOrientation);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCAudioSinkSetSoundOrientation));
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Sets the direct send levels of the audio sink for the specified channel.
            /// </summary>
            /// <param name="sendLevels">The send levels of the audio sink.</param>
            /// <param name="channel">Channel to set this spatialization property for. Passing -1 sets it for all available channels.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
            /// </returns>
            public MLResult SetDirectSendLevels(MLAudioOutput.SpatialSound.SendLevels sendLevels, int channel = -1)
            {
                nativeSoundLevels.Update(sendLevels, channel);
                MLResult.Code resultCode = NativeBindings.MLWebRTCAudioSinkSetDirectSoundLevels(this.Handle, ref nativeSoundLevels);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCAudioSinkSetDirectSoundLevels));
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Sets the room send levels of the audio sink for the specified channel.
            /// </summary>
            /// <param name="sendLevels">The send levels of the audio sink.</param>
            /// <param name="channel">Channel to set this spatialization property for. Passing -1 sets it for all available channels.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
            /// </returns>
            public MLResult SetRoomSendLevels(MLAudioOutput.SpatialSound.SendLevels sendLevels, int channel = -1)
            {
                nativeSoundLevels.Update(sendLevels, channel);
                MLResult.Code resultCode = NativeBindings.MLWebRTCAudioSinkSetRoomSoundLevels(this.Handle, ref nativeSoundLevels);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCAudioSinkSetRoomSoundLevels));
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Sets the distance properties of the audio sink for the specified channel.
            /// </summary>
            /// <param name="distanceProperties">The distance properties of the audio sink.</param>
            /// <param name="channel">Channel to set this spatialization property for. Passing -1 sets it for all available channels.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
            /// </returns>
            public MLResult SetDistanceProperties(MLAudioOutput.SpatialSound.DistanceProperties distanceProperties, int channel = -1)
            {
                nativeDistance.Update(distanceProperties, channel);
                MLResult.Code resultCode = NativeBindings.MLWebRTCAudioSinkSetSoundDistanceProperties(this.Handle, ref nativeDistance);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCAudioSinkSetSoundDistanceProperties));
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Sets the radiation properties of the audio sink for the specified channel.
            /// </summary>
            /// <param name="radiationProperties">The radiation properties of the audio sink.</param>
            /// <param name="channel">Channel to set this spatialization property for. Passing -1 sets it for all available channels.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
            /// </returns>
            public MLResult SetRadiationProperties(MLAudioOutput.SpatialSound.RadiationProperties radiationProperties, int channel = -1)
            {
                nativeRadiation.Update(radiationProperties, channel);
                MLResult.Code resultCode = NativeBindings.MLWebRTCAudioSinkSetSoundRadiationProperties(this.Handle, ref nativeRadiation);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCAudioSinkSetSoundRadiationProperties));
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Resets the world position of the audio sink for <c>spatialized</c> audio.
            /// </summary>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            public MLResult ResetPosition()
            {
                MLResult.Code resultCode = NativeBindings.MLWebRTCAudioSinkResetPosition(this.Handle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCAudioSinkResetPosition));
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Sets the number of milliseconds of audio that should be cached in the buffers before dropping the packets.
            /// Dictates the audio latency when app recovers from lifecycle state transitions like standby & reality.
            /// Default is 200ms.
            /// </summary>
            /// <param name="sinkHandle">The handle to the audio sink.</param>
            /// <param name="millisecondsToCache">How many milliseconds worth of audio to cache.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            public MLResult SetCacheSize(uint millisecondsToCache)
            {
                MLResult.Code resultCode = NativeBindings.MLWebRTCAudioSinkSetCacheSize(this.Handle, millisecondsToCache);
                string nativeMethodName = nameof(NativeBindings.MLWebRTCAudioSinkSetCacheSize);
                MLResult.DidNativeCallSucceed(resultCode, $"{nativeMethodName}({millisecondsToCache})");
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Destroys this audio sink object.
            /// </summary>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            public override MLResult Destroy()
            {
                if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Handle is invalid.");
                }

                this.SetStream(null);
                MLResult.Code resultCode = NativeBindings.MLWebRTCAudioSinkDestroy(this.Handle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCAudioSinkDestroy));
                this.InvalidateHandle();

                MLWebRTC.Instance.sinks.Remove(this);
                this.gcHandle.Free();

                nativeRadiation.FreeUnmanagedMemory();
                nativeDistance.FreeUnmanagedMemory();
                nativeSoundLevels.FreeUnmanagedMemory();
                nativeOrientation.FreeUnmanagedMemory();
                nativePosition.FreeUnmanagedMemory();

                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Sets the track of the audio sink.
            /// </summary>
            /// <param name="track">The track to use.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            protected override MLResult SetTrack(MLWebRTC.MediaStream.Track track)
            {
                ulong sourceHandle = track != null ? track.Handle : MagicLeapNativeBindings.InvalidHandle;
                MLResult.Code resultCode = NativeBindings.MLWebRTCAudioSinkSetSource(this.Handle, sourceHandle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCAudioSinkSetSource));
                return MLResult.Create(resultCode);
            }
        }
    }
}
