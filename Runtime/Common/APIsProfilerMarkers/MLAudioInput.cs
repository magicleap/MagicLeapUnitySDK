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
    public partial class MLAudioInput
    {
        //Profiler markers for AudioInput Native Bindings calls

        private readonly static ProfilerMarker nativeSetMicMutePerfMarker = new ProfilerMarker("MLAudioInputNativeBindings.MLAudioSetMicMute");
        private readonly static ProfilerMarker nativeIsMicMutedPerfMarker = new ProfilerMarker("MLAudioInputNativeBindings.MLAudioIsMicMuted");
        private readonly static ProfilerMarker nativeSetMicMuteCallbackPerfMarker = new ProfilerMarker("MLAudioInputNativeBindings.MLAudioSetMicMuteCallback");
        private readonly static ProfilerMarker nativeGetResultStringPerfMarker = new ProfilerMarker("MLAudioInputNativeBindings.MLAudioGetResultString");

        //Profiler markers for AudioInput calls.

        private readonly static ProfilerMarker startAPIPerfMarker = new ProfilerMarker("MLAudioInput.StartAPI");
    }
}
