// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLAnchorNativeBindings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%


namespace UnityEngine.XR.MagicLeap
{
    using UnityEngine;
    using System.Runtime.InteropServices;

    /// <summary>
    ///	APIs for the voice service system.
    /// </summary>
    public partial class MLVoice
    {
        private bool isProcessing = false;

        private bool isEnabled()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            bool enableCheck = false;
            MLResult.Code result = MLVoice.NativeBindings.MLVoiceIntentIsEnabled(this.Handle, out enableCheck);
            if (!MLResult.IsOK(result))
                MLPluginLog.Error("MLVoice failed to check MLVoiceIntentIsEnabled: " + result);
            return enableCheck;
#else
            return false;
#endif
        }

#if UNITY_MAGICLEAP || UNITY_ANDROID
        protected override void OnApplicationPause(bool pauseStatus)
        {
            MLResult.Code result;
            if (pauseStatus)
            {
                result = result = SetCallbacks(true);
                if (!MLResult.IsOK(result))
                    MLPluginLog.Error("MLVoice during pause failed to unregister MLVoiceIntentSetCallbacks: " + result);
                result = MLVoice.NativeBindings.MLVoiceIntentStopProcessing(this.Handle);
                if (!MLResult.IsOK(result))
                    MLPluginLog.Error("MLVoice during pause failed to MLVoiceIntentStopProcessing: " + result);
                else
                    isProcessing = false;
            }
            else
            {
                result = SetCallbacks(false);
                if (!MLResult.IsOK(result))
                    MLPluginLog.Error("MLVoice during resume failed to register MLVoiceIntentSetCallbacks: " + result);
                result = MLVoice.NativeBindings.MLVoiceIntentStartProcessing(this.Handle);
                if (!MLResult.IsOK(result))
                    MLPluginLog.Error("MLVoice during resume failed to MLVoiceIntentStartProcessing: " + result);
                else
                    isProcessing = true;
            }
    }
#endif

        private MLResult.Code ConfigureSettings(string JSONString)
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            NativeBindings.IntentSettings newSettings = NativeBindings.IntentSettings.Create();
            newSettings.AppIntent = JSONString;
            return NativeBindings.MLVoiceIntentConfigureSettings(this.Handle, in newSettings);
#else
            return MLResult.Code.APIDLLNotFound;
#endif
        }

        private MLResult.Code SetCallbacks(bool unregister)
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            NativeBindings.IntentCallbacks newCallbacks = NativeBindings.IntentCallbacks.Create();
            
            if(unregister)
            {
                newCallbacks.OnEvent = null;
            }
            return MLVoice.NativeBindings.MLVoiceIntentSetCallbacks(this.Handle, newCallbacks, System.IntPtr.Zero);
#else
            return MLResult.Code.APIDLLNotFound;
#endif
        }
    }
}
