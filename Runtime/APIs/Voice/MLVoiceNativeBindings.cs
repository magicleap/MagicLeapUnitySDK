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
    using Native;

    /// <summary>
    ///	APIs for the voice service system.
    /// </summary>
    public partial class MLVoice
    {
        /// <summary>
        /// See ml_voice.h for additional comments.
        /// </summary>
        public class NativeBindings : MagicLeapNativeBindings
        {
            /// <summary>
            /// This callback will be invoked whenever a voice intent event is detected.
            /// </summary>
            /// <param name="intentEvent">Voice intent event.</param>
            /// <param name="data">User data as passed to MLVoiceIntentSetCallbacks().</param>
            public delegate void OnVoiceEventDelegate(in IntentEventInternal intentEvent, IntPtr data);

            /// <summary>
            /// Native callback that is invoked when a data channel receives a message.
            /// </summary>
            /// <param name="intentEvent">Voice intent event.</param>
            /// <param name="data">User data as passed to MLVoiceIntentSetCallbacks().</param>
            [AOT.MonoPInvokeCallback(typeof(OnVoiceEventDelegate))]
            private static void OnEvent(in IntentEventInternal intentEvent, IntPtr data)
            {
                IntentEvent newEvent = new IntentEvent();
                newEvent.State = intentEvent.State;
                newEvent.NoIntentReason = intentEvent.NoIntentReason;
                newEvent.EventName = intentEvent.Text;
                newEvent.EventID = intentEvent.IntentID;

                bool eventSuccessful = intentEvent.IsSuccess;
#if UNITY_MAGICLEAP || UNITY_ANDROID
                MLThreadDispatch.ScheduleMain(() =>
                {
                    OnVoiceEventInternal?.Invoke(eventSuccessful, newEvent);
                });
#endif
            }

            /// <summary>
            /// A structure containing voice intent event data.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct IntentEventInternal
            {
                /// <summary>
                /// Voice event handling result in the voice intent event.
                /// In case of false, other member variables should be ignored. (text will be NULL)
                /// </summary>
                [MarshalAs(UnmanagedType.I1)]
                public bool IsSuccess;

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
                /// The string carrying identifier data which is UTF-8 and null terminated.
                /// And the memory of this variable is managed by system.
                /// </summary>
                [MarshalAs(UnmanagedType.LPStr)]
                public string Text;

                /// <summary>
                /// User defined intent index which is detected.
                /// </summary>
                public uint IntentID;
            };

            /// <summary>
            /// A structure containing callbacks for voice intent events.
            /// The final parameter to all the callbacks is a IntPtr, which will point to
            /// whatever payload data the user provides in MLVoiceIntentSetCallbacks().
            /// Individual callbacks which are not required by the client can be NULL.
            /// 
            /// This structure must be initialized by calling IntentCallbacks.Create()
            /// before use.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct IntentCallbacks
            {
                /// <summary>
                /// Version of the struct.
                /// </summary>
                public uint Version;

                /// <summary>
                /// Native OnVoiceEvent.
                /// </summary>
                public OnVoiceEventDelegate OnEvent;

                /// <summary>
                /// Creates an initialized IntentCallbacks object.
                /// </summary>
                /// <returns>An initialized IntentCallbacks object.</returns>
                public static IntentCallbacks Create()
                {
                    IntentCallbacks callbacks = new IntentCallbacks();
                    callbacks.Version = 1;
#if UNITY_MAGICLEAP || UNITY_ANDROID
                    callbacks.OnEvent = MLVoice.NativeBindings.OnEvent;
#endif
                    return callbacks;
                }
            }

            /// <summary>
            /// A structure containing settings for voice app intent.
            /// This structure must be initialized by calling IntentSettings.Create() before use.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct IntentSettings
            {
                /// <summary>
                /// Version of the struct.
                /// </summary>
                public uint Version;

                /// <summary>
                /// The C string carrying app intent made by JSON format, which is UTF-8 and null terminated.
                /// The app intent should contain pre-defined fields, names, and values.
                /// Memory of this variable is managed by user.
                /// </summary>
                [MarshalAs(UnmanagedType.LPStr)]
                public string AppIntent;

                /// <summary>
                /// Creates an initialized IntentSettings object.
                /// </summary>
                /// <returns>An initialized IntentSettings object.</returns>
                public static IntentSettings Create()
                {
                    IntentSettings settings = new IntentSettings();
                    settings.Version = 1;
                    settings.AppIntent = null;

                    return settings;
                }
            }

            /// <summary>
            /// Creates a voice intent client.
            /// </summary>
            /// <param name="handle">A handle to the created voice intent client. Only valid if the return value is MLResult.Code.Ok.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to create the voice intent client due to an invalid input parameter
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully created the voice intent client.
            /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if failed to create client due to permission (permission).
            /// 
            /// Permissions: VOICE_INPUT
            /// </returns>
            [DllImport(MLVoiceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLVoiceIntentCreate(out ulong handle);

            /// <summary>
            /// Destroy a previously created voice intent client.
            /// </summary>
            /// <param name="handle">Handle to the voice intent client.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to destroy the voice intent client due to an invalid input parameter
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully destroyed the voice intent client.
            /// </returns>
            [DllImport(MLVoiceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLVoiceIntentDestroy(ulong handle);

            /// <summary>
            /// Sets the settings for voice intent events.
            /// 
            /// App needs to call MLVoiceIntentConfigureSettings at least once before it calls MLVoiceIntentStartProcessing.
            /// App can call MLVoiceIntentConfigureSettings anytime it needs to update the voice intents.
            /// But such call will trigger updating intent grammar inside speech engine, which may result in a short delay (could be a few seconds depending on the device's workload).
            /// So it is not recommended to call the function frequently.
            /// </summary>
            /// <param name="handle">Handle to the voice intent client.</param>
            /// <param name="voiceSettings">Pointer to MLVoiceIntentSettings structure.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to set settings for voice intent events due to an invalid param.
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if Successfully set settings for voice intent events.
            /// MLResult.Result will be <c>MLResult.Code.MLVoiceResult_IntentDisabled</c> if failed to set the settings because required voice intent feature is disabled in system settings.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
            /// </returns>
            [DllImport(MLVoiceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLVoiceIntentConfigureSettings(ulong handle, in IntentSettings voiceSettings);

            /// <summary>
            /// Sets the callbacks for voice intent events.
            /// </summary>
            /// <param name="handle">Handle to the voice intent client.</param>
            /// <param name="callbacks">Pointer to MLVoiceIntentCallbacks structure. Set this to NULL to unregister callbacks.</param>
            /// <param name="data">Pointer to user payload data (can be NULL).</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to set callbacks for voice intent events due to an invalid param.
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if Successfully set callbacks for voice intent events.
            /// MLResult.Result will be <c>MLResult.Code.MLVoiceResult_IntentDisabled</c> if failed to set the callbacks because required voice intent feature is disabled in system settings.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
            /// </returns>
            [DllImport(MLVoiceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLVoiceIntentSetCallbacks(ulong handle, in IntentCallbacks callbacks, in IntPtr data);

            /// <summary>
            /// Starts voice intent processing. This affects system voice service directly and permanently while system is running.
            /// Application should use this function carefully.
            /// It is strongly encourage to call MLVoiceIntentStopProcessing when the application is paused to avoid unexpected system behavior.
            /// </summary>
            /// <param name="handle">Handle to the voice intent client.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to start processing due to an invalid parameter.
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if Successfully started intent processing.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
            /// </returns>
            [DllImport(MLVoiceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLVoiceIntentStartProcessing(ulong handle);

            /// <summary>
            /// Stops voice intent processing. This affects system voice service directly and permanently while system is running.
            /// Application should use this function carefully.
            /// It is strongly encourage to call this when the application is paused to avoid unexpected system behavior.
            /// </summary>
            /// <param name="handle">Handle to the voice intent client.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to stop processing due to an invalid parameter.
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if Successfully stopped intent processing.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
            /// </returns>
            [DllImport(MLVoiceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLVoiceIntentStopProcessing(ulong handle);

            /// <summary>
            /// Checks voice intent feature is enabled in the system.
            /// </summary>
            /// <param name="handle">Handle to the voice intent client.</param>
            /// <param name="isEnabled">Result whether voice intent feature is enabled.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to check due to an invalid parameter.
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if Successfully checked voice intent feature is enabled.
            /// </returns>
            [DllImport(MLVoiceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLVoiceIntentIsEnabled(ulong handle, [MarshalAs(UnmanagedType.I1)] out bool isEnabled);
        }
    }
}
