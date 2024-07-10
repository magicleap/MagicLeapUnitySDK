// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections;
using System.Collections.Generic;
using MagicLeap.OpenXR;
using MagicLeap.OpenXR.Features;
using UnityEngine.XR.MagicLeap.Native;
using UnityEngine.XR.Management;
using Utils = MagicLeap.OpenXR.Utils;
#if UNITY_OPENXR_1_9_0_OR_NEWER
using UnityEngine.XR.OpenXR;
#endif
namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// MagicLeap device class responsible for updating all trackers when they register and are enabled.
    /// </summary>
    [DefaultExecutionOrder(ScriptOrder)]
    public class MLDevice : MonoBehaviour
    {
        /// <summary>
        /// Ensures this MonoBehavior runs its event methods very early before all other scripts.
        /// </summary>
        private const int ScriptOrder = -15001;

        /// <summary>
        /// Create the MLDevice instance object on application startup from the main thread
        /// This avoids race conditions and situations where worker threads need to access it
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InstantiateSingleton()
        {
            if (instance == null)
            {
                GameObject go = new GameObject("(MLDevice Singleton)");
                instance = go.AddComponent<MLDevice>();
#if DEBUG
                MLPluginLog.Debug("Creating MLDevice");
#endif

                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(instance.gameObject);
                }
            }
        }

        /// <summary>
        /// Hardcoded value approximating minimum near clip plane distance allowed by the platform.
        /// </summary>
        public const float MinimumNearClipDistance = 0.37f;

        /// <summary>
        /// Hardcoded value with the default near clip plane distance to use.
        /// </summary>
        public const float DefaultNearClipDistance = 0.37f;

        /// <summary>
        /// MagicLeap platform Unity name.
        /// </summary>
        public const string MagicLeapDeviceName = "MagicLeap";

        /// <summary>
        /// Reference to the active MLDevice instance.
        /// </summary>
        private static MLDevice instance = null;

        /// <summary>
        /// Reference to the active XR MagicLeap gestures subsystem.
        /// </summary>
        private GestureSubsystemComponent gestureSubsystemComponent = null;

        /// <summary>
        /// Contains the value that specifies if the underlying Unity XR MagicLeap subsystem is initialized.
        /// </summary>
        private bool isReady = false;

        /// <summary>
        /// Count of XR MagicLeap gestures subsystem start calls.
        /// </summary>
        private uint gestureSubsystemStartCount = 0;

        /// <summary>
        /// Platform API level.
        /// </summary>
        private uint? platformLevel = null;

        /// <summary>
        /// List of callbacks to trigger at the end of the frame.
        /// </summary>
        private List<Action> endOfFrameCallbacks = new List<Action>();

        /// <summary>
        /// <c>Coroutine</c> used to get the end of frame.
        /// </summary>
        private Coroutine endOfFrameCoroutine = null;

        /// <summary>
        /// Delegate to handle Update calls.
        /// </summary>
        public delegate void OnStartEventDelegate();

        /// <summary>
        /// Delegate to handle Update calls.
        /// </summary>
        public delegate void OnUpdateEventDelegate();

        /// <summary>
        /// Delegate to handle Update calls.
        /// </summary>
        public delegate void OnDestroyEventDelegate();

        /// <summary>
        /// Delegate to handle application pause events.
        /// </summary>
        /// <param name="paused">Whether app is paused or resumed.</param>
        public delegate void OnPauseEventDelegate(bool paused);

        /// <summary>
        /// Delegate to handle application focus events.
        /// </summary>
        /// <param name="hasFocus">Whether app is focus gained or lost.</param>
        public delegate void OnFocusEventDelegate(bool hasFocus);

        /// <summary>
        /// Delegate to handle application quit events.
        /// </summary>
        public delegate void OnQuitEventDelegate();

        /// <summary>
        /// Triggered every Unity Update.
        /// </summary>
        private event OnStartEventDelegate OnStartEvent = delegate { };

        /// <summary>
        /// Triggered every Unity Update.
        /// </summary>
        private event OnUpdateEventDelegate OnUpdateEvent = delegate { };

        /// <summary>
        /// Triggered when Unity's OnDestroy is called.
        /// </summary>
        private event OnDestroyEventDelegate OnDestroyEvent = delegate { };

        /// <summary>
        /// Triggered on application pause and resume.
        /// </summary>
        private event OnPauseEventDelegate OnPauseEvent = delegate { };

        /// <summary>
        /// Triggered on application focus gained and lost.
        /// </summary>
        private event OnFocusEventDelegate OnFocusEvent = delegate { };

        /// <summary>
        /// Triggered on application quit.
        /// </summary>
        private event OnQuitEventDelegate OnQuitEvent = delegate { };

        private int mainThreadId = -1;

        private Camera unityCamera;

#if UNITY_OPENXR_1_9_0_OR_NEWER
        private MagicLeapFeature mlOpenXrFeature;
#endif

        /// <summary>
        /// Gets the platform API level that the OS supports.
        /// </summary>
        public static uint PlatformLevel
        {
            get
            {
                if (Instance.platformLevel == null)
                {
                    Instance.GetPlatformLevel();
                }

                return Instance.platformLevel.Value;
            }
        }

        /// <summary>
        /// Gets the active XR MagicLeap gestures subsystem.
        /// </summary>
        public static GestureSubsystemComponent GestureSubsystemComponent
        {
            get => Instance.gestureSubsystemComponent;
        }

        public static int MainThreadId => Instance.mainThreadId;

        public static bool UnityVideoPlayerSupported =>
#if UNITY_2022_3_10_OR_NEWER
            true;
#else
            false;
#endif

        /// <summary>
        /// Gets the MLDevice singleton instance.
        /// </summary>
        public static MLDevice Instance
        {
            get { return instance; }
            private set { instance = value; }
        }

        /// <summary>
        /// Check if the underlying Unity XR MagicLeap subsystem is initialized.
        /// </summary>
        /// <returns>Value indicating whether the XR MagicLeap subsystem is initialized.</returns>
        public static bool IsReady()
        {
            return UnityEngine.XR.XRSettings.enabled && (IsMagicLeapLoaderActive() || IsOpenXRLoaderActive());
        }

        public static bool IsMagicLeapLoaderActive()
        {
#if UNITY_XR_MAGICLEAP_PROVIDER
            if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
            {
                return XRGeneralSettings.Instance.Manager.ActiveLoaderAs<MagicLeapLoader>() != null;
            }
#endif
            return false;
        }

        public static bool IsOpenXRLoaderActive()
        { 
#if UNITY_OPENXR_1_9_0_OR_NEWER
            return Utils.TryGetOpenXRLoader(out _);
#endif
#pragma warning disable CS0162
            return false;
#pragma warning restore CS0162
        }

        public static bool IsMagicLeapOrOpenXRLoaderActive()
        {
            if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
            {
                bool isOpenXRLoaderActive = IsOpenXRLoaderActive();
                bool isXRSDKLoaderActive = IsMagicLeapLoaderActive();
                return isOpenXRLoaderActive || isXRSDKLoaderActive;
            }
            return false;
        }

        /// <summary>
        /// Register a MagicLeap API Update callback to be called on Start of this behavior.
        /// </summary>
        /// <param name="callback">Callback to register.</param>
        public static void RegisterStart(OnStartEventDelegate callback)
        {
            Instance.OnStartEvent += callback;
        }

        /// <summary>
        /// Register a MagicLeap API Update callback to be called on Update of this behavior.
        /// </summary>
        /// <param name="callback">Callback to register.</param>
        public static void RegisterUpdate(OnUpdateEventDelegate callback)
        {
            Instance.OnUpdateEvent += callback;
        }

        /// <summary>
        /// Register a MagicLeap API Update callback to be called on Destroy of this behavior.
        /// </summary>
        /// <param name="callback">Callback to register.</param>
        public static void RegisterDestroy(OnDestroyEventDelegate callback)
        {
            Instance.OnDestroyEvent += callback;
        }

        /// <summary>
        /// Unrgister a MagicLeap API application pause callback to be called OnApplicationPause of this behavior.
        /// </summary>
        /// <param name="callback">Callback to register.</param>
        public static void RegisterApplicationPause(OnPauseEventDelegate callback)
        {
            Instance.OnPauseEvent += callback;
        }

        /// <summary>
        /// Unrgister a MagicLeap API application focus callback to be called OnApplicationFocus of this behavior.
        /// </summary>
        /// <param name="callback">Callback to register.</param>
        public static void RegisterApplicationFocus(OnFocusEventDelegate callback)
        {
            Instance.OnFocusEvent += callback;
        }

        /// <summary>
        /// Register a MagicLeap API application quit callback to be called on OnApplicationQuit of this behavior.
        /// </summary>
        /// <param name="callback">Callback to register</param>
        public static void RegisterApplicationQuit(OnQuitEventDelegate callback)
        {
            Instance.OnQuitEvent += callback;
        }

        /// <summary>
        /// Unregister a previously registered MagicLeap API Update callback.
        /// </summary>
        /// <param name="callback">Callback to unregister.</param>
        public static void UnregisterStart(OnStartEventDelegate callback)
        {
            Instance.OnStartEvent -= callback;
        }

        /// <summary>
        /// Unregister a previously registered MagicLeap API Update callback.
        /// </summary>
        /// <param name="callback">Callback to unregister.</param>
        public static void UnregisterUpdate(OnUpdateEventDelegate callback)
        {
            // Check instance instead of the Instance property to prevent
            // creating an instance to unregister something that won't be there.
            if (instance != null)
            {
                instance.OnUpdateEvent -= callback;
            }
        }

        /// <summary>
        /// Unregister a MagicLeap API from being called when OnDestroy for this behavior is called.
        /// </summary>
        /// <param name="callback">Callback to register.</param>
        public static void UnregisterDestroy(OnDestroyEventDelegate callback)
        {
            // Check instance instead of the Instance property to prevent
            // creating an instance to unregister something that won't be there.
            if (instance != null)
            {
                instance.OnDestroyEvent -= callback;
            }
        }

        /// <summary>
        /// Unregister a previously registered MagicLeap API application pause callback.
        /// </summary>
        /// <param name="callback">Callback to unregister.</param>
        public static void UnregisterApplicationPause(OnPauseEventDelegate callback)
        {
            // Check instance instead of the Instance property to prevent
            // creating an instance to unregister something that won't be there.
            if (instance != null)
            {
                instance.OnPauseEvent -= callback;
            }
        }

        /// <summary>
        /// Unregister a previously registered MagicLeap API application focus callback.
        /// </summary>
        /// <param name="callback">Callback to unregister.</param>
        public static void UnregisterApplicationFocus(OnFocusEventDelegate callback)
        {
            // Check instance instead of the Instance property to prevent
            // creating an instance to unregister something that won't be there.
            if (instance != null)
            {
                instance.OnFocusEvent -= callback;
            }
        }

        /// <summary>
        /// Unregister a previously registered MagicLeapAPI application quit callback.
        /// </summary>
        /// <param name="callback">Callback to unregister</param>
        public static void UnregisterApplicationQuit(OnQuitEventDelegate callback)
        {
            // Check instance instead of the Instance property to prevent
            // creating an instance to unregister something that won't be there.
            if (instance != null)
            {
                instance.OnQuitEvent -= callback;
            }
        }

        /// <summary>
        /// Attempts to register the Unity GestureSubsystem.
        /// </summary>
        public static void RegisterGestureSubsystem()
        {
            // Instance is used on register, to ensure we have a valid instance.
            Instance.gestureSubsystemStartCount++;

            if (instance != null && instance.gestureSubsystemComponent == null)
            {
                instance.gestureSubsystemComponent = instance.gameObject.AddComponent<GestureSubsystemComponent>();
            }
        }

        /// <summary>
        /// Attempts to unregister the GestureSubsystem.
        /// </summary>
        public static void UnregisterGestureSubsystem()
        {
            if (instance == null)
            {
                return;
            }

            if (instance.gestureSubsystemStartCount > 0)
            {
                instance.gestureSubsystemStartCount--;

                // Only destroy the component when all instances have been removed.
                if (instance.gestureSubsystemStartCount == 0)
                {
                    if (instance.gestureSubsystemComponent != null)
                    {
                        Destroy(instance.gestureSubsystemComponent);
                        instance.gestureSubsystemComponent = null;
                    }
                }
            }
        }

        /// <summary>
        /// Register a function to be executed per frame at the end of every frame, after all cameras and GUI is rendered
        /// but before displaying the frame on screen
        /// </summary>
        /// <param name="endOfFrameFunction">The function.</param>
        public static void RegisterEndOfFrameUpdate(Action endOfFrameFunction)
        {
            if (endOfFrameFunction != null)
            {
                Instance.endOfFrameCallbacks.Add(endOfFrameFunction);
            }
        }

        /// <summary>
        /// Unregister a function to no longer be executed at the end of the frame
        /// </summary>
        /// <param name="endOfFrameFunction">The function.</param>
        public static void UnregisterEndOfFrameUpdate(Action endOfFrameFunction)
        {
            if (instance != null && endOfFrameFunction != null)
            {
                instance.endOfFrameCallbacks.Remove(endOfFrameFunction);
            }
        }

        /// <summary>
        /// Initializes the Magic Leap device state.
        /// </summary>
        protected void Awake()
        {
            this.mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;

#if UNITY_XR_MAGICLEAP_PROVIDER
            if (!Application.isEditor)
            {
                using (var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    var context = actClass.GetStatic<AndroidJavaObject>("currentActivity");
                    AndroidJavaClass systemGlobal = new AndroidJavaClass("android.provider.Settings$Secure");
                    var setting = systemGlobal.CallStatic<float>("getFloat", context.Call<AndroidJavaObject>("getContentResolver"), "clipping_plane_distance", 0f);
                    if (setting != 0f)
                    {
                        // if we got something other than 0 that means the setting is available and we should disable the cumbersome enforcement in the XR plugin
                        Rendering.RenderingSettings.enforceNearClip = false;
                        Debug.Log("System setting clipping_plane_distance found, disabling Unity XR enforcement.");
                    }
                    else
                    {
                        Rendering.RenderingSettings.enforceNearClip = true;
                        Debug.LogWarning("System setting clipping_plane_distance could not be found. Unity XR will enforce constant value.");
                    }
                }
            }
#endif

#if UNITY_OPENXR_1_9_0_OR_NEWER
            if (IsOpenXRLoaderActive())
            {
                mlOpenXrFeature = OpenXRSettings.Instance.GetFeature<MagicLeapFeature>();
            }
#endif

        }

        /// <summary>
        /// Cleans up any resources the object has open.
        /// </summary>
        protected void OnDestroy()
        {
            if (this.endOfFrameCoroutine != null)
            {
                this.StopCoroutine(this.endOfFrameCoroutine);
            }
            OnDestroyEvent?.Invoke();
        }

        /// <summary>
        /// Calls OnUpdateActions event and dispatches all queued callbacks.
        /// </summary>
        protected void Update()
        {
            if (!this.isReady && !(this.isReady = MLDevice.IsReady()))
            {
                return;
            }

            this.OnUpdateEvent?.Invoke();

            MLThreadDispatch.DispatchAll();
        }

        /// <summary>
        /// Starts the EndOfFrameUpdate coroutine and dispatches the Start event.
        /// </summary>
        protected void Start()
        {
            this.endOfFrameCoroutine = this.StartCoroutine(this.EndOfFrameUpdate());
            OnStartEvent();
        }

        /// <summary>
        /// Callback sent to all game objects when the player pauses.
        /// </summary>
        /// <param name="pauseStatus">The pause state of the application.</param>
        private void OnApplicationPause(bool pauseStatus)
        {
            this.OnPauseEvent?.Invoke(pauseStatus);
            MLThreadDispatch.DispatchAll();
        }

        /// <summary>
        /// Callback sent to all game objects when the player loses or gains focus.
        /// </summary>
        /// <param name="hasFocus">The focus state of the application.</param>
        private void OnApplicationFocus(bool hasFocus)
        {
            this.OnFocusEvent?.Invoke(hasFocus);
            MLThreadDispatch.DispatchAll();
        }

        /// <summary>
        /// Callback sent to all game objects when the player quits.
        /// </summary>
        private void OnApplicationQuit()
        {
            this.OnQuitEvent?.Invoke();
        }

        /// <summary>
        /// Retrieves the platform API level that the OS supports.
        /// </summary>
        private void GetPlatformLevel()
        {
            try
            {
                uint level = 0;
                MLResult.Code resultCode = MagicLeapNativeBindings.MLPlatformGetAPILevel(ref level);
                if (resultCode != MLResult.Code.Ok)
                {
                    MLPluginLog.ErrorFormat("MLDevice.GetPlatformLevel failed to get platform level. Reason: {0}", resultCode);
                }
                else
                {
                    this.platformLevel = level;
                }
            }
            catch (DllNotFoundException)
            {
                MLPluginLog.ErrorFormat("MLDevice.GetPlatformLevel failed. Reason: {0} library not found", MagicLeapNativeBindings.MLPlatformDll);
            }

            if (this.platformLevel == null)
            {
                this.platformLevel = 0;
            }
        }

        /// <summary>
        /// Update function that gets called at the end of frame.
        /// </summary>
        /// <returns>End of frame <c>IEnumetaor</c></returns>
        private IEnumerator EndOfFrameUpdate()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                foreach (Action callback in this.endOfFrameCallbacks)
                {
                    callback?.Invoke();
                }
            }
        }
    }
}
