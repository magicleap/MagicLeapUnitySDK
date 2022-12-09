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

    public abstract class MLAPIBase
    {
        /// <summary>
        /// The native handle ID for this API instance. Will be invalid until the API is started.
        /// </summary>
        protected ulong Handle = Native.MagicLeapNativeBindings.InvalidHandle;

        /// <summary>
        /// Lock used to keep api calls synchronous.
        /// </summary>
        protected readonly object APILock = new object();

        private bool isStarted = false;

        public MLAPIBase()
        {
            StartInternal();
        }

        ~MLAPIBase()
        {
            StopInternal();
        }

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

        private void StartInternal()
        {
            MLDevice.RegisterUpdate(Update);
            MLDevice.RegisterApplicationPause(OnApplicationPause);
            MLDevice.RegisterApplicationFocus(OnApplicationFocus);
            MLDevice.RegisterDestroy(StopInternal);
            isStarted = true;
        }

        private void StopInternal()
        {
            if (isStarted)
            {
                MLDevice.UnregisterUpdate(Update);
                MLDevice.UnregisterApplicationPause(OnApplicationPause);
                MLDevice.UnregisterApplicationFocus(OnApplicationFocus);
                MLDevice.UnregisterDestroy(StopInternal);
                isStarted = false;
            }
        }
    }
}
