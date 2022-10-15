// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using Unity.Profiling;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLHandTracking 
    {
        //Profiler markers for MLHandTracking Native Bindings calls

        private readonly static ProfilerMarker nativeUpdateConfigurationPerfMarker = new ProfilerMarker("MLHandTrackingNativeBindings.UpdateConfiguration");
        private readonly static ProfilerMarker nativeIsHandGesturesEnabledPerfMarker = new ProfilerMarker("MLHandTrackingNativeBindings.IsHandGesturesEnabled");
        private readonly static ProfilerMarker nativeSetHandGesturesEnabledPerfMarker = new ProfilerMarker("MLHandTrackingNativeBindings.SetHandGesturesEnabled");

        //Profiler markers for MLHandTracking calls

        private readonly static ProfilerMarker startAPIPerfMarker = new ProfilerMarker("MLHandTracking.StartAPI");
        private readonly static ProfilerMarker stopAPIPerfMarker = new ProfilerMarker("MLHandTracking.StopAPI");
        private readonly static ProfilerMarker updatePerfMarker = new ProfilerMarker("MLHandTracking.Update");

        //Profiler markers for KeyposeManager calls

        private readonly static ProfilerMarker disableAllKeyPosesPerfMarker = new ProfilerMarker("KeyposeManager.DisableAllKeyPoses");
        private readonly static ProfilerMarker EnableKeyPosesPerfMarker = new ProfilerMarker("KeyposeManager.EnableKeyPoses");
        private readonly static ProfilerMarker disposePerfMarker = new ProfilerMarker("KeyposeManager.Dispose");

        //Profiler markers for MLHandTrackingFingers calls

        private readonly static ProfilerMarker fingerUpdatePerfMarker = new ProfilerMarker("Finger.Update");

        //Profiler markers for MLHandTrackingHand calls

        private readonly static ProfilerMarker handUpdatePerfMarker = new ProfilerMarker("Hand.Update");
    }
}
