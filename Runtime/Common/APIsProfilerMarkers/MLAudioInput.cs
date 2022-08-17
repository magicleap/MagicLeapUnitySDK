// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLAudioInput.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
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
