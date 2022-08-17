// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLAudioOutput.cs" company="Magic Leap, Inc">
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
