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
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    ///	APIs for the voice service system.
    /// </summary>
    public partial class MLVoice
    {
        private bool isEnabled()
        {
            bool enableCheck = false;
            MLResult.Code result = MLVoice.NativeBindings.MLVoiceIntentIsEnabled(this.Handle, out enableCheck);

            if (!MLResult.DidNativeCallSucceed(result, nameof(MLVoice.NativeBindings.MLVoiceIntentIsEnabled)))
            {
                MLPluginLog.Error("MLVoice failed to check MLVoiceIntentIsEnabled: " + result);
            }
            return enableCheck;
        }

        protected override void OnApplicationPause(bool pauseStatus)
        {
            if (this.Handle == Native.MagicLeapNativeBindings.InvalidHandle)
            {
                // dont do anything because the API isn't running.
                return;
            }

            if (!isEnabled())
            {
                // dont do anything because the voice intent system isn't enabled.
                return;
            }

            MLResult.Code result;
            if (pauseStatus)
            {
                result = SetCallbacks(true);
                if (!MLResult.IsOK(result))
                {
                    MLPluginLog.Error("MLVoice during pause failed to unregister MLVoiceIntentSetCallbacks: " + result);
                }
                result = MLVoice.NativeBindings.MLVoiceIntentStopProcessing(this.Handle);

                if (!MLResult.DidNativeCallSucceed(result, nameof(MLVoice.NativeBindings.MLVoiceIntentStopProcessing)))
                {
                    MLPluginLog.Error("MLVoice during pause failed to MLVoiceIntentStopProcessing: " + result);
                }
            }
            else
            {
                result = SetCallbacks(false);
                if (!MLResult.IsOK(result))
                {
                    MLPluginLog.Error("MLVoice during resume failed to register MLVoiceIntentSetCallbacks: " + result);
                }
                result = MLVoice.NativeBindings.MLVoiceIntentStartProcessing(this.Handle);

                if (!MLResult.DidNativeCallSucceed(result, nameof(MLVoice.NativeBindings.MLVoiceIntentStartProcessing)))
                {
                    MLPluginLog.Error("MLVoice during resume failed to MLVoiceIntentStartProcessing: " + result);
                }
            }
        }

        private MLResult.Code ConfigureSettings(string JSONString)
        {
            NativeBindings.IntentSettings newSettings = NativeBindings.IntentSettings.Create();
            newSettings.AppIntent = JSONString;

            var resultCode = NativeBindings.MLVoiceIntentConfigureSettings(this.Handle, in newSettings);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLVoiceIntentConfigureSettings));
            return resultCode;
        }

        private MLResult.Code SetCallbacks(bool unregister)
        {
            NativeBindings.IntentCallbacks newCallbacks = NativeBindings.IntentCallbacks.Create();

            if (unregister)
            {
                newCallbacks.OnEvent = null;
            }

            var resultCode = MLVoice.NativeBindings.MLVoiceIntentSetCallbacks(this.Handle, newCallbacks, System.IntPtr.Zero);
            MLResult.DidNativeCallSucceed(resultCode, nameof(MLVoice.NativeBindings.MLVoiceIntentSetCallbacks));
            return resultCode;
        }
    }
}
