// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

// Disabling deprecated warning for the internal project
#pragma warning disable 618

namespace UnityEngine.XR.MagicLeap
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// APIs for the voice service system.
    ///	
    /// To use the voice feature (with a specific application's intent list), the application should send
    /// a Scriptable Object called MLVoiceIntentsConfiguration, Located under Assets/Magic Leap/Voice Intents Configuration,
    /// with the application's voice intents data to MLVoice.SetupVoiceIntents().
    /// 
    /// Name, id, and value should be unique. Value is the phrase needed to be spoken out loud after "Hey Magic Leap"
    /// " | " may be used in the configuration file's value field to indicate multiple values tied to the same name and id.
    /// </summary>
    public partial class MLVoice : MLAutoAPISingleton<MLVoice>
    {
        /// <summary>
        /// Voice state in voice event.
        /// </summary>
        public enum State
        {
            /// <summary>
            ///  Intent hotword detected, Automatic Speech Recognition (ASR) session is going to start.
            /// </summary>
            HotwordDetected,

            /// <summary>
            ///  Voice UI is stopped, ASR session is going to stop.
            /// </summary>
            SessionEnded,

            /// <summary>
            ///  ASR session is stopped due to abort.
            /// </summary>
            ISessionEndedAborted,

            /// <summary>
            ///  ASR session is stopped without detecting an intent.
            /// </summary>
            SessionEndedNoIntent,

            /// <summary>
            ///  ASR session is stopped due to timeout.
            /// </summary>
            SessionEndedTimeout,

            /// <summary>
            ///  ASR session is stopped due to error.
            /// </summary>
            SessionEndedError,

            /// <summary>
            ///  Intent mode is started and ready.
            /// </summary>
            Ready,

            /// <summary>
            ///  Intent mode is stopped and not available.
            /// </summary>
            NotAvailable
        };

        /// <summary>
        /// No intent reason code in voice event.
        /// </summary>
        public enum NoIntentReason
        {
            /// <summary>
            ///  In case of success
            /// </summary>
            NoReason,
            /// <summary>
            ///  When the State in IntentEvent is either MLVoice.State.SessionEndedTimeout or MLVoice.State.SessionEndedError.
            /// </summary>
            Silence,
            /// <summary>
            ///  When the State in IntentEvent is MLVoice.State.SessionEndedNoIntent.
            /// </summary>
            NoMatch
        };

        /// <summary>
        /// Slot Data in voice event.
        /// </summary>
        public struct EventSlot
        {
            /// <summary>
            /// Name of slot used.
            /// </summary>
            public string SlotName;
            /// <summary>
            /// Value of the specific slot used.
            /// </summary>
            public string SlotValue;

            public EventSlot(string name, string value)
            {
                this.SlotName = name;
                this.SlotValue = value;
            }
        }

        /// <summary>
        /// A structure containing voice intent event information.
        /// </summary>
        public struct IntentEvent
        {
            /// <summary>
            /// Voice state when generating the voice intent event.
            /// </summary>
            public State State;

            /// <summary>
            /// If intent is not detected, it contains the reason,
            /// otherwise the value is MLVoiceIntentNoIntentReason.NoReason.
            /// </summary>
            public NoIntentReason NoIntentReason;

            /// <summary>
            /// The Event Name
            /// </summary>
            public string EventName;

            /// <summary>
            /// User defined intent index which is detected.
            /// </summary>
            public uint EventID;

            // <summary>
            /// List of Slot Data used in this specific event.
            /// </summary>
            public List<EventSlot> EventSlotsUsed;
        };

        /// <summary>
        /// This callback will be invoked whenever a voice intent event is detected.
        /// </summary>
        /// /// <param name="wasSuccessful">Voice event handling result. In case of false, voiceEvent member variables should be ignored. (voiceEvent.EventName will be NULL)</param>
        /// <param name="voiceEvent">Voice intent event.</param>
        public delegate void OnVoiceEventDelegate(in bool wasSuccessful, in IntentEvent voiceEvent);

        /// <summary>
        /// Event invoked for when a data channel opens.
        /// </summary>
        public static event OnVoiceEventDelegate OnVoiceEvent
        {
            add
            {
                OnVoiceEventInternal += value;
            }

            remove
            {
                OnVoiceEventInternal -= value;
            }
        }

        private static event OnVoiceEventDelegate OnVoiceEventInternal = delegate { };

        /// <summary>
        /// Checks voice intent feature is enabled in the system.
        /// </summary>
        public static bool VoiceEnabled => Instance.isEnabled();

        protected override MLResult.Code StartAPI()
        {
            if (!MLResult.DidNativeCallSucceed(MLPermissions.CheckPermission(MLPermission.VoiceInput).Result, nameof(StartAPI)))
            {
                MLPluginLog.Error($"{nameof(MLVoice)} requires missing permission {MLPermission.VoiceInput}");
                return MLResult.Code.PermissionDenied;
            }

            var resultCode = MLVoice.NativeBindings.MLVoiceIntentCreate(out this.Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(MLVoice.NativeBindings.MLVoiceIntentCreate));
            return resultCode;
        }

        protected override MLResult.Code StopAPI()
        {
            Stop();
            var result = MLVoice.NativeBindings.MLVoiceIntentDestroy(this.Handle);
            if (MLResult.IsOK(result))
            {
                this.Handle = Native.MagicLeapNativeBindings.InvalidHandle;
            }
            return result;
        }

        /// <summary>
        /// Configures Settings sent, Sets the callbacks for voice intent events, and starts processing.
        /// 
        /// Max Voice Intents Supported: 100
        /// </summary>
        /// <param name="voiceConfiguration">MLVoiceIntentsConfiguration scriptable object that contains the App Intents data.
        /// Memory of this variable is managed by user.
        /// </param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to an invalid param.
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if Successfully set settings, callbacks, and starts processing.
        /// MLResult.Result will be <c>MLResult.Code.MLVoiceResult_IntentDisabled</c> if failed to set the callbacks or settings because required voice intent feature is disabled in system settings.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
        /// </returns>
        public static MLResult SetupVoiceIntents(MLVoiceIntentsConfiguration voiceConfiguration)
        {
            string JSONString = voiceConfiguration.GetJSONString();

            MLResult result = MLResult.Create(Instance.ConfigureSettings(JSONString));
            if (!result.IsOk)
            {
                MLPluginLog.Error("MLVoice failed to ConfigureSettings: " + result);
                return result;
            }

            result = MLResult.Create(Instance.SetCallbacks(false));
            if (!result.IsOk)
            {
                MLPluginLog.Error("MLVoice failed to SetCallbacks: " + result);
                return result;
            }

            var resultCode = NativeBindings.MLVoiceIntentStartProcessing(Instance.Handle);

            if (!MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLVoiceIntentStartProcessing)))
            {
                MLPluginLog.Error("MLVoice failed to StartProcessing: " + result);
            }

            return result;
        }

        /// <summary>
        /// Configures Settings sent, Sets the callbacks for voice intent events, and starts processing.
        /// 
        /// When providing a string instead of the MLVoiceIntentsConfiguration Scriptable Object no
        /// validation is done to confirm proper format.
        /// 
        /// Max Voice Intents Supported: 100
        /// </summary>
        /// <param name="JSONString">The JSON file with the voice intents data in the proper format.
        /// </param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to an invalid param.
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if Successfully set settings, callbacks, and starts processing.
        /// MLResult.Result will be <c>MLResult.Code.MLVoiceResult_IntentDisabled</c> if failed to set the callbacks or settings because required voice intent feature is disabled in system settings.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
        /// </returns>
        public static MLResult SetupVoiceIntents(string JSONString)
        {
            MLResult result = MLResult.Create(Instance.ConfigureSettings(JSONString));
            if (!result.IsOk)
            {
                MLPluginLog.Error("MLVoice failed to ConfigureSettings: " + result);
                return result;
            }

            result = MLResult.Create(Instance.SetCallbacks(false));
            if (!result.IsOk)
            {
                MLPluginLog.Error("MLVoice failed to SetCallbacks: " + result);
                return result;
            }

            var resultCode = NativeBindings.MLVoiceIntentStartProcessing(Instance.Handle);

            if (!MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLVoiceIntentStartProcessing)))
            {
                MLPluginLog.Error("MLVoice failed to StartProcessing: " + result);
            }

            return result;
        }

        /// <summary>
        /// Stop unsets callbacks and stops processing. This is not necessary for shutdown. 
        /// This is only needed if a user wants to stop processing at runtime after call MLVoice.SetupVoiceIntents.
        /// MLVoice can be restarted by calling MLVoice.SetupVoiceIntents.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to an invalid param.
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if Successfully unset callbacks and stops processing.
        /// MLResult.Result will be <c>MLResult.Code.MLVoiceResult_IntentDisabled</c> if failed because required voice intent feature is disabled in system settings.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
        /// </returns>
        public static MLResult Stop()
        {
            MLResult.Code resultCode = MLResult.Code.Ok;
            if (Instance.Handle != Native.MagicLeapNativeBindings.InvalidHandle)
            {
                MLResult result = MLResult.Create(Instance.SetCallbacks(true));

                if (!MLResult.IsOK(result.Result))
                {
                    MLPluginLog.Error("MLVoice.Stop failed to unregister MLVoiceIntentSetCallbacks: " + result);
                }

                resultCode = NativeBindings.MLVoiceIntentStopProcessing(Instance.Handle);

                if (!MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLVoiceIntentStopProcessing)))
                {
                    MLPluginLog.Error("MLVoice.Stop failed to MLVoiceIntentStopProcessing: " + result);
                }
            }

            return MLResult.Create(resultCode);
        }
    }
}
