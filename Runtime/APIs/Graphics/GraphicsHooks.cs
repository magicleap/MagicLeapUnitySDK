// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2022 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    public static partial class MLGraphicsHooks
    {
        public static ulong ClientHandle => NativeBindings.MLUnityGraphicsGetHandle();

        public delegate void OnPreBeginRenderFrameDelegate();

        public static event OnPreBeginRenderFrameDelegate OnPreBeginRenderFrame
        {
            add
            {
                internalOnPreBeginRenderFrame += value;
                RegisterCallbacks();
            }
            remove
            {
                internalOnPreBeginRenderFrame -= value;
            }
        }

        public static void RequestAlphaBlendFrameRendering(bool useAlphaBlend)
        {
            preferAlphaBlendMode = useAlphaBlend;
            if (useAlphaBlend)
            {
                RegisterCallbacks();
            }
            else
            {
                CleanUp();
            }
        }

        public static void RequestPredictedSnapshots(bool useSnapshots)
        {
            usePredictedSnapshots = useSnapshots;
            RegisterCallbacks();
        }

        // clean up.
        public static void Shutdown()
        {
            usePredictedSnapshots = false;
            CleanUp();
        }

        private static void RegisterCallbacks()
        {
            if (!registeredForRenderCallbacks)
            {
                registeredForRenderCallbacks = true;
                NativeBindings.MLUnityGraphicsCallbacks callbacks = NativeBindings.MLUnityGraphicsCallbacks.Create();
                NativeBindings.MLUnityGraphicsRegisterCallbacks(ref callbacks);
            }
        }

        private static void CleanUp()
        {
            if (registeredForRenderCallbacks)
            {
                NativeBindings.ClearCallbacks();
            }
            registeredForRenderCallbacks = false;
        }

        private static event OnPreBeginRenderFrameDelegate internalOnPreBeginRenderFrame = delegate { };
        private static bool registeredForRenderCallbacks;
        private static bool preferAlphaBlendMode = false;
        private static bool usePredictedSnapshots = false;
    }
}
