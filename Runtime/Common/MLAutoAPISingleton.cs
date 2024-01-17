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
    using System.Reflection;

    /// <summary>
    /// Place this attribute on a child of MLAutoAPISingleton to prevent its initialization
    /// until the Magic Leap XR package is loaded.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequireXRLoader : Attribute { }

    /// <summary>
    /// MLAutoAPISingleton class contains a template for singleton APIs
    /// </summary>
    /// <typeparam name="T"> The type of the class to create a singleton for. </typeparam>
    public abstract class MLAutoAPISingleton<T> : MLLazySingleton<T> where T : MLAutoAPISingleton<T>, new()
    {
        /// <summary>
        /// The native handle ID for this API instance. Will be invalid until the API is started.
        /// </summary>
        protected ulong Handle = Native.MagicLeapNativeBindings.InvalidHandle;

        /// <summary>
        /// Lock used to keep api calls synchronous.
        /// </summary>
        protected readonly object APILock = new object();

        private static readonly bool requiresXRLoader = typeof(T).GetCustomAttribute<RequireXRLoader>() != null;

        /// <summary>
        /// This is the only way to initialize this class.
        /// </summary>
        sealed protected override void Initialize()
        {
            if (requiresXRLoader && !MLDevice.IsReady())
            {
                MLPluginLog.Error($"Magic Leap XR Loader is not initialized, and the {typeof(T).Name} API must be started afterwards.");
                return;
            }
            else
            {
                Instance.StartInternal();
#if !UNITY_OPENXR_1_9_0_OR_NEWER
                MagicLeapXrProviderNativeBindings.InputSetOnPerceptionShutdownCallback(OnPerceptionShutdown);
#endif
            }
        }

        /// <summary>
        /// Indicates if the API has started successfully.
        /// </summary>
        public static bool IsStarted { get; private set; } = false;

        protected readonly string DllNotFoundError = $"Failed to start {typeof(T).Name} API. This API is only available on device or when running inside the Unity editor with Magic Leap App Simulator enabled.";

        /// <summary>
        /// Do API-specific creation/initialization of ML resources for this API, such as
        /// creating trackers, etc. Called automatically the first time <c>Instance</c> is accessed.
        /// Error checking on the return value is performed in the base class.
        /// </summary>
        protected abstract MLResult.Code StartAPI();

        /// <summary>
        /// API-specific cleanup. Will be called whenever MLDevice is destroyed
        /// (at the latest, when the application is shutting down).
        /// Error checking on the return value is performed in the base class.
        /// </summary>
        protected abstract MLResult.Code StopAPI();

        /// <summary>
        /// Update function that will run once per Unity frame.
        /// </summary>
        protected virtual void Update() { }

        /// <summary>
        /// Callback sent to all MagicLeap APIs on application pause.
        /// </summary>
        /// <param name="pauseStatus"> True if the application is paused, else False. </param>
        protected virtual void OnApplicationPause(bool pauseStatus) { }

        /// <summary>
        /// Callback sent to all MagicLeap APIs on application focus event.
        /// </summary>
        /// <param name="hasFocus"> True if the application has focus, else False. </param>
        protected virtual void OnApplicationFocus(bool hasFocus) { }

        /// <summary>
        /// Callback sent to all MagicLeap APIs when device enters standby mode.
        /// </summary>
        protected virtual void OnDeviceStandby() { }

        /// <summary>
        /// Callback sent to all MagicLeap APIs when device enters reality mode.
        /// </summary>
        protected virtual void OnDeviceReality() { }

        /// <summary>
        /// Callback sent to all MagicLeap APIs when device enters active mode.
        /// </summary>
        protected virtual void OnDeviceActive() { }

        [AOT.MonoPInvokeCallback(typeof(MagicLeapXrProviderNativeBindings.CallOnPerceptionShutdownDelegate))]
        private static void OnPerceptionShutdown() => Instance.StopInternal();

        private void StartInternal()
        {
            MLPluginLog.Debug($"Initializing {typeof(T).Name} API...");

            if (MLResult.DidNativeCallSucceed(StartAPI(), $"{typeof(T).Name} Start"))
            {
                IsStarted = true;
                MLDevice.RegisterUpdate(Instance.Update);
                MLDevice.RegisterApplicationPause(Instance.OnApplicationPause);
                MLDevice.RegisterApplicationFocus(Instance.OnApplicationFocus);
                MLDevice.RegisterDestroy(Instance.StopInternal);
                MLPluginLog.Debug($"{typeof(T).Name} API initialized.");
            }
        }

        private void StopInternal()
        {
            if (IsStarted)
            {
                MLDevice.UnregisterUpdate(Instance.Update);
                MLDevice.UnregisterApplicationPause(Instance.OnApplicationPause);
                MLDevice.UnregisterApplicationFocus(Instance.OnApplicationFocus);
                MLDevice.UnregisterDestroy(Instance.StopInternal);

                MLResult.Code resultCode = Instance.StopAPI();

                if (MLResult.DidNativeCallSucceed(resultCode, $"{typeof(T).Name} Stop"))
                    MLPluginLog.Debug($"{typeof(T).Name} API stopped successfully");

                IsStarted = false;
            }
        }
    }
}
