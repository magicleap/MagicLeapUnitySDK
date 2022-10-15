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
    public sealed partial class MLAudioOutput
    {
        //Profiler markers for AudioOutput Native Bindings calls

        private readonly static ProfilerMarker nativeGetOutputDevicePerfMarker = new ProfilerMarker("MLAudioOutputNativeBindings.MLAudioGetOutputDevice");
        private readonly static ProfilerMarker nativeGetMasterVolumePerfMarker = new ProfilerMarker("MLAudioOutputNativeBindings.MLAudioGetMasterVolume");
        private readonly static ProfilerMarker nativeSetMasterVolumeCallbackPerfMarker = new ProfilerMarker("MLAudioOutputNativeBindings.MLAudioSetMasterVolumeCallback");
        private readonly static ProfilerMarker nativeMediaEventCallbackPerfMarker = new ProfilerMarker("MLAudioOutputNativeBindings.MLAudioMediaEventCallback");
        private readonly static ProfilerMarker nativeGetResultStringPerfMarker = new ProfilerMarker("MLAudioOutputNativeBindings.MLAudioGetResultString");

        //Profiler markers for AudiOutput calls.

        private readonly static ProfilerMarker startAPIPerfMarker = new ProfilerMarker("MLAudioOutput.StartAPI");
        private readonly static ProfilerMarker updatePerfMarker = new ProfilerMarker("MLAudioOutput.Update");
        private readonly static ProfilerMarker audioOutputDevicePerfMarker = new ProfilerMarker("MLAudioOutput.AudioOutputDevice");
    }
}
