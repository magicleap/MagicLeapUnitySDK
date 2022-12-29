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
    /// Manages Audio.
    /// </summary>
    public sealed partial class MLAudioOutput : MLAutoAPISingleton<MLAudioOutput>
    {
        /// <summary>
        /// The current audio device.
        /// </summary>
        private Device audioDevice = Device.Wearable;

        /// <summary>
        /// The last audio device.
        /// </summary>
        private Device lastAudioDevice = Device.Wearable;

        /// <summary>
        /// The value of the master volume.
        /// </summary>
        private float masterVolume;

        /// <summary>
        /// The delegate for the master volume changed event.
        /// </summary>
        /// <param name="volume">The new master volume value.</param>
        public delegate void OnMasterVolumeChangedDelegate(float volume);

        /// <summary>
        /// The delegate for audio output device changed event.
        /// </summary>
        /// <param name="device">The new audio output device.</param>
        public delegate void OnAudioOutputDeviceChangedDelegate(Device device);

        /// <summary>
        /// The delegate for audio output media event.
        /// </summary>
        /// <param name="device">The new media event.</param>
        public delegate void MLAudioMediaEventDelegate(MediaEvent mediaEvent);

        /// <summary>
        /// Raised whenever the master volume gets changed.
        /// </summary>
        public static event OnMasterVolumeChangedDelegate OnMasterVolumeChanged = delegate { };

        /// <summary>
        /// Raised whenever the audio output device gets changed.
        /// </summary>
        public static event OnAudioOutputDeviceChangedDelegate OnAudioOutputDeviceChanged = delegate { };

        /// <summary>
        /// Raised whenever the media event happens.
        /// </summary>
        public static event MLAudioMediaEventDelegate OnMediaEvent = delegate { };

        /// <summary>
        /// The currently active output device.
        /// </summary>
        public enum Device : uint
        {
            /// <summary>
            /// Built-in speakers in the wearable.
            /// </summary>
            Wearable,

            /// <summary>
            /// 3.5mm jack on the belt pack.
            /// </summary>
            AnalogJack
        }

        /// <summary>
        /// Possible media control events initiated by the user.
        /// </summary>
        public enum MediaEvent
        {

            /// <summary>
            ///  Indicates a user command to play
            /// </summary>
            Play,

            /// <summary>
            ///  Indicates a user command to stop.
            /// </summary>
            Stop,

            /// <summary>
            ///  Indicates a user command to pause.
            /// </summary>
            Pause,

            /// <summary>
            ///  Indicates a user command to go to next track.
            /// </summary>
            NextTrack,

            /// <summary>
            ///  Indicates a user command to go to previous track.
            /// </summary>
            PrevTrack,

        };

        /// <summary>
        /// Gets the audio output device.
        /// </summary>
        public static Device AudioOutputDevice
        {
            get
            {
                audioOutputDevicePerfMarker.Begin();
                Device device = Instance.InternalGetOutputDevice();
                audioOutputDevicePerfMarker.End();
                return device;
            }
        }

        /// <summary>
        /// Gets the master volume for the device.
        /// </summary>
        public static float MasterVolume
        {
            get { return Instance.masterVolume; }
        }

        public static MLResult SetSoundBypassesMasterVolume(bool isBypassing) => MLResult.Create(Instance.SetSoundBypassesMasterVolumeInternal(isBypassing));

        public static MLResult GetSoundBypassesMasterVolume(out bool isBypassing) => MLResult.Create(Instance.GetSoundBypassesMasterVolumeInternal(out isBypassing));

        /// <summary>
        /// Setting this option on a sound output causes its output to bypass master volume, making
        /// it effectively "always audible" (assuming it is neither muted nor set to zero volume
        /// on a per-sound basis). This option can only be set on medical-enabled devices (60601
        /// compliant), and will only work for non-spatial sounds.Non-spatial sound parameters
        /// such as volume, mute, pitch and looping are still in effect for sounds that are
        /// bypassing master volume.
        /// </summary>
        private MLResult.Code SetSoundBypassesMasterVolumeInternal(bool isBypassing)
        {
            ulong audioHandle = NativeBindings.MLUnityAudioGetHandle();
            var resultCode = NativeBindings.MLAudioSetSoundBypassesMasterVolume(audioHandle, isBypassing);
            MLResult.DidNativeCallSucceed(resultCode, nameof(MLAudioOutput.NativeBindings.MLAudioSetSoundBypassesMasterVolume));
            return resultCode;
        }

        /// <summary>
        /// Queries whether a sound output is exempt from attenuation due to master volume.
        /// This call reports whether the output from a sound output is bypassing master volume,
        /// making it effectively "always audible" (assuming it is neither muted nor set to zero volume
        /// on a per-sound basis). This option can only be set on medical-enabled devices(60601
        /// compliant), and will only work for non-spatial sounds.Non-spatial sound parameters
        /// such as volume, mute, pitch and looping are still in effect for sounds that are
        /// bypassing master volume.
        /// </summary>
        private MLResult.Code GetSoundBypassesMasterVolumeInternal(out bool isBypassing)
        {
            ulong audioHandle = NativeBindings.MLUnityAudioGetHandle();
            var resultCode = NativeBindings.MLAudioGetSoundBypassesMasterVolume(audioHandle, out isBypassing);
            MLResult.DidNativeCallSucceed(resultCode, nameof(MLAudioOutput.NativeBindings.MLAudioGetSoundBypassesMasterVolume));
            return resultCode;
        }

        /// <summary>
        /// Gets the result string for a MLResult.Code.
        /// </summary>
        /// <param name="resultCode">The MLResult.Code to be requested.</param>
        /// <returns>The result string.</returns>
        internal static string GetResultString(MLResult.Code resultCode)
        {
            try
            {
                return Marshal.PtrToStringAnsi(NativeBindings.MLAudioGetResultString(resultCode));
            }
            catch (System.DllNotFoundException)
            {
                MLPluginLog.Error("MLAudioOutput.GetResultString failed. Reason: MLAudio API is currently available only on device.");
            }
            catch (System.EntryPointNotFoundException)
            {
                MLPluginLog.Error("MLAudioOutput.GetResultString failed. Reason: API symbols not found");
            }

            return string.Empty;
        }

#if !DOXYGEN_SHOULD_SKIP_THIS
        /// <summary>
        /// Called by MLAutoAPISingleton to start the API
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if a parameter is invalid.
        /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if <c>AudioCaptureMic</c> permission is denied.
        /// MLResult.Result will be <c>MLResult.Code.AudioNotImplemented</c> if the function is not implemented.
        /// </returns>
        protected override MLResult.Code StartAPI()
        {
            startAPIPerfMarker.Begin();
            // Set the initial audio device.
            Instance.lastAudioDevice = Instance.InternalGetOutputDevice();

            MLResult.Code resultCode;

            // Master Volume Callback
            resultCode = this.RegisterCallbacks();
            if (resultCode != MLResult.Code.Ok)
            {
                startAPIPerfMarker.End();
                return resultCode;
            }

            // Get the initial MasterVolume value.
            resultCode = Instance.GetMasterVolume(out this.masterVolume);
            if (resultCode != MLResult.Code.Ok)
            {
                startAPIPerfMarker.End();
                return resultCode;
            }

            startAPIPerfMarker.End();
            return resultCode;
        }
#endif // DOXYGEN_SHOULD_SKIP_THIS

        /// <summary>
        /// Called by MLAutoAPISingleton on destruction
        /// </summary>
        /// <param name="isSafeToAccessManagedObjects">Not Implemented</param>
        protected override MLResult.Code StopAPI()
        {
            try
            {
                return UnregisterCallbacks();
            }
            catch (System.DllNotFoundException)
            {
                MLPluginLog.Error(MLResult.Code.APIDLLNotFound);
                throw;
            }
        }

        /// <summary>
        /// Called every device frame
        /// </summary>
        protected override void Update()
        {
            updatePerfMarker.Begin();
            if (this.lastAudioDevice != Instance.InternalGetOutputDevice())
            {
                Instance.lastAudioDevice = Instance.audioDevice;

                // Notify event listeners.
                // Callback is not needed to be in the queue because it is in Update.
                OnAudioOutputDeviceChanged?.Invoke(Instance.lastAudioDevice);
            }
            updatePerfMarker.End();
        }

        /// <summary>
        /// Handles the callback for MLAudioSetMasterVolume.
        /// </summary>
        /// <param name="volume">The volume value.</param>
        /// <param name="callback">A pointer to the callback.</param>
        [AOT.MonoPInvokeCallback(typeof(NativeBindings.MLAudioMasterVolumeChangedCallback))]
        private static void HandleOnMLAudioSetMasterVolumeCallback(float volume, IntPtr callback)
        {
            Instance.masterVolume = volume;

            MLThreadDispatch.Call(volume, OnMasterVolumeChanged);
        }

        /// <summary>
        /// Handles the callback for MLAudioSetMediaEventCallback.
        /// </summary>
        /// <param name="mediaEvent">The media event.</param>
        /// <param name="callback">A pointer to the callback.</param>
        [AOT.MonoPInvokeCallback(typeof(NativeBindings.MLAudioMediaEventCallback))]
        private static void HandleOnMediaEventCallback(MediaEvent mediaEvent, IntPtr callback)
        {
            MLThreadDispatch.Call(mediaEvent, OnMediaEvent);
        }

        /// <summary>
        /// Returns the master volume for the audio system.
        /// The range of the volume is 0-10, with 0 being silent and 10 being full volume.
        /// </summary>
        /// <param name="volume">The current volume value.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if a parameter is invalid.
        /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if <c>AudioCaptureMic</c> permission is denied.
        /// MLResult.Result will be <c>MLResult.Code.AudioNotImplemented</c> if the function is not implemented.
        /// </returns>
        private MLResult.Code GetMasterVolume(out float volume)
        {
            MLResult.Code result;

            try
            {
                nativeGetMasterVolumePerfMarker.Begin();
                result = NativeBindings.MLAudioGetMasterVolume(out volume);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLAudioGetMasterVolume));
                nativeGetMasterVolumePerfMarker.End();
                if (result != MLResult.Code.Ok)
                {
                    MLPluginLog.ErrorFormat("MLAudioOutput.GetMasterVolume failed to get the volume. Reason: {0}", result);
                }
            }
            catch (System.DllNotFoundException)
            {
                // Exception is caught in the Singleton BaseStart().
                throw;
            }

            return result;
        }

        /// <summary>
        /// Registers a callback for the device volume change event.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if a parameter is invalid.
        /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if <c>AudioCaptureMic</c> permission is denied.
        /// MLResult.Result will be <c>MLResult.Code.AudioNotImplemented</c> if the function is not implemented.
        /// </returns>
        private MLResult.Code RegisterCallbacks()
        {
            MLResult.Code result;

            nativeSetMasterVolumeCallbackPerfMarker.Begin();
            // Attempt to register the native callback for the volume change event.
            result = NativeBindings.MLAudioSetMasterVolumeCallback(HandleOnMLAudioSetMasterVolumeCallback, IntPtr.Zero);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLAudioSetMasterVolumeCallback));
            nativeSetMasterVolumeCallbackPerfMarker.End();

            if (result != MLResult.Code.Ok)
            {
                MLPluginLog.ErrorFormat("MLAudioOutput.RegisterOnVolumeChangeCallback failed to register callback. Reason: {0}", result);
                return result;
            }

            nativeMediaEventCallbackPerfMarker.Begin();
            // Attempt to register the native callback for the media event change event.
            result = NativeBindings.MLAudioSetMediaEventCallback(HandleOnMediaEventCallback, IntPtr.Zero);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLAudioSetMediaEventCallback));
            nativeMediaEventCallbackPerfMarker.End();

            if (result != MLResult.Code.Ok)
            {
                MLPluginLog.ErrorFormat("MLAudioOutput.MLAudioSetMediaEventCallback failed to register callback. Reason: {0}", result);
                return result;
            }

            return result;
        }

        /// <summary>
        /// Unregisters a previously registered callback for the device volume change event.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if a parameter is invalid.
        /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if <c>AudioCaptureMic</c> permission is denied.
        /// MLResult.Result will be <c>MLResult.Code.AudioNotImplemented</c> if the function is not implemented.
        /// </returns>
        private MLResult.Code UnregisterCallbacks()
        {
            MLResult.Code result;

            nativeSetMasterVolumeCallbackPerfMarker.Begin();
            // Unregister the native callback for the volume change event.
            result = NativeBindings.MLAudioSetMasterVolumeCallback(null, IntPtr.Zero);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLAudioSetMasterVolumeCallback));
            nativeSetMasterVolumeCallbackPerfMarker.End();

            if (result != MLResult.Code.Ok)
            {
                MLPluginLog.ErrorFormat("MLAudioOutput.UnregisterOnVolumeChangeCallback failed to register callback. Reason: {0}", result);
            }

            nativeMediaEventCallbackPerfMarker.Begin();
            // Attempt to register the native callback for the volume change event.
            result = NativeBindings.MLAudioSetMediaEventCallback(null, IntPtr.Zero);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLAudioSetMediaEventCallback));
            nativeMediaEventCallbackPerfMarker.End();

            if (result != MLResult.Code.Ok)
            {
                MLPluginLog.ErrorFormat("MLAudioOutput.RegisterOnMediaEventCallback failed to register callback. Reason: {0}", result);
            }

            return result;
        }

        /// <summary>
        /// Get the current audio output device.
        /// </summary>
        /// <returns>The audio output device.</returns>
        private MLAudioOutput.Device InternalGetOutputDevice()
        {
            try
            {
                nativeGetOutputDevicePerfMarker.Begin();
                MLResult.Code result = NativeBindings.MLAudioGetOutputDevice(out Instance.audioDevice);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLAudioGetOutputDevice));
                nativeGetOutputDevicePerfMarker.End();
                if (result != MLResult.Code.Ok)
                {
                    MLPluginLog.ErrorFormat("MLAudioOutpu.InternalGetOutputDevice failed to get the audio output device. Reason: {0}", result);
                }
            }
            catch (System.DllNotFoundException)
            {
                MLPluginLog.Error(this.DllNotFoundError);
                throw;
            }

            return Instance.audioDevice;
        }
    }
}
