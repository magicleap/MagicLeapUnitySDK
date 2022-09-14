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
    /// <summary>
    /// Performance markers for MLMediaPlayer.
    /// </summary>
    public sealed partial class MLMediaPlayer : MonoBehaviour
    {
        //Performance markers for MLMediaPlayer Error Native Bindings calls

        private readonly static ProfilerMarker nativeMLMediaResultGetStringPerfMarker = new ProfilerMarker("MLMediaErrorNativeBindings.MLMediaResultGetString");

        //Performance markers for MLMediaPlayer Native Bindings calls

        private readonly static ProfilerMarker nativeInstantiateMediaPlayerContextPerfMarker = new ProfilerMarker("MLMediaNativeBindings.InstantiateMediaPlayerContext");
        private readonly static ProfilerMarker nativeMediaPlayerResetAsyncPerfMarker = new ProfilerMarker("MLMediaNativeBindings.MediaPlayerResetAsync");
        private readonly static ProfilerMarker nativeGetEventIdForPluginRenderingEventPerfMarker = new ProfilerMarker("MLMediaNativeBindings.GetEventIdForPluginRenderingEvent");
        private readonly static ProfilerMarker nativeGetPluginCallbackForEventPerfMarker = new ProfilerMarker("MLMediaNativeBindings.GetPluginCallbackForEvent");
        private readonly static ProfilerMarker nativeOpenDRMSessionPerfMarker = new ProfilerMarker("MLMediaNativeBindings.OpenDRMSession");
        private readonly static ProfilerMarker nativeCloseDRMSessionPerfMarker = new ProfilerMarker("MLMediaNativeBindings.CloseDRMSession");
        private readonly static ProfilerMarker nativeGetProvisionRequestPerfMarker = new ProfilerMarker("MLMediaNativeBindings.GetProvisionRequest");
        private readonly static ProfilerMarker nativeProvideProvisionResponsePerfMarker = new ProfilerMarker("MLMediaNativeBindings.ProvideProvisionResponse");
        private readonly static ProfilerMarker nativeGetActivationProvisionRequestPerfMarker = new ProfilerMarker("MLMediaNativeBindings.GetActivationProvisionRequest");
        private readonly static ProfilerMarker nativeProvideActivationProvisionResponsePerfMarker = new ProfilerMarker("MLMediaNativeBindings.ProvideActivationProvisionResponse");
        private readonly static ProfilerMarker nativeSetKeyRequestCustomDataPerfMarker = new ProfilerMarker("MLMediaNativeBindings.SetKeyRequestCustomData");
        private readonly static ProfilerMarker nativeGetActivationKeyRequestPerfMarker = new ProfilerMarker("MLMediaNativeBindings.GetActivationKeyRequest");
        private readonly static ProfilerMarker nativeGetKeyRequestPerfMarker = new ProfilerMarker("MLMediaNativeBindings.GetKeyRequest");
        private readonly static ProfilerMarker nativeProvideKeyResponsePerfMarker = new ProfilerMarker("MLMediaNativeBindings.ProvideKeyResponse");
        private readonly static ProfilerMarker nativePrepareDRMPerfMarker = new ProfilerMarker("MLMediaNativeBindings.PrepareDRM");
        private readonly static ProfilerMarker nativePlayPerfMarker = new ProfilerMarker("MLMediaNativeBindings.Play");
        private readonly static ProfilerMarker nativePauseVideoPerfMarker = new ProfilerMarker("MLMediaNativeBindings.PauseVideo");
        private readonly static ProfilerMarker nativeSeekPerfMarker = new ProfilerMarker("MLMediaNativeBindings.Seek");
        private readonly static ProfilerMarker nativeResumePerfMarker = new ProfilerMarker("MLMediaNativeBindings.Resume");
        private readonly static ProfilerMarker nativeStopPerfMarker = new ProfilerMarker("MLMediaNativeBindings.Stop");
        private readonly static ProfilerMarker nativeSetLoopingPerfMarker = new ProfilerMarker("MLMediaNativeBindings.SetLooping");
        private readonly static ProfilerMarker nativeSetVolumePerfMarker = new ProfilerMarker("MLMediaNativeBindings.SetVolume");
        private readonly static ProfilerMarker nativeGetDurationMillisecondsPerfMarker = new ProfilerMarker("MLMediaNativeBindings.GetDurationMilliseconds");
        private readonly static ProfilerMarker nativeGetPositionMillisecondsPerfMarker = new ProfilerMarker("MLMediaNativeBindings.GetPositionMilliseconds");
        private readonly static ProfilerMarker nativeGetWidthPerfMarker = new ProfilerMarker("MLMediaNativeBindings.GetWidth");
        private readonly static ProfilerMarker nativeGetHeightPerfMarker = new ProfilerMarker("MLMediaNativeBindings.GetHeight");
        private readonly static ProfilerMarker nativeGetVideoBitratePerfMarker = new ProfilerMarker("MLMediaNativeBindings.GetVideoBitrate");
        private readonly static ProfilerMarker nativeGetFrameDropThresholdMsPerfMarker = new ProfilerMarker("MLMediaNativeBindings.GetFrameDropThresholdMs");
        private readonly static ProfilerMarker nativeSetFrameDropThresholdMsPerfMarker = new ProfilerMarker("MLMediaNativeBindings.SetFrameDropThresholdMs");
        private readonly static ProfilerMarker nativeGetTrackDataPerfMarker = new ProfilerMarker("MLMediaNativeBindings.GetTrackData");
        private readonly static ProfilerMarker nativeSelectSubtitleTrackPerfMarker = new ProfilerMarker("MLMediaNativeBindings.SelectSubtitleTrack");
        private readonly static ProfilerMarker nativeUnselectSubtitleTrackPerfMarker = new ProfilerMarker("MLMediaNativeBindings.UnselectSubtitleTrack");
        private readonly static ProfilerMarker nativeGetAudioChannelCountPerfMarker = new ProfilerMarker("MLMediaNativeBindings.GetAudioChannelCount");
        private readonly static ProfilerMarker nativeSetSpatialAudioEnablePerfMarker = new ProfilerMarker("MLMediaNativeBindings.SetSpatialAudioEnable");
        private readonly static ProfilerMarker nativeSetAudioChannelPositionPerfMarker = new ProfilerMarker("MLMediaNativeBindings.SetAudioChannelPosition");
        private readonly static ProfilerMarker nativeGetAudioChannelPositionPerfMarker = new ProfilerMarker("MLMediaNativeBindings.GetAudioChannelPosition");
        private readonly static ProfilerMarker nativeCleanupPerfMarker = new ProfilerMarker("MLMediaNativeBindings.Cleanup");
        private readonly static ProfilerMarker nativeSetSharingInfoPerfMarker = new ProfilerMarker("MLMediaNativeBindings.SetSharingInfo");

        //Performance markers for MLMediaPlayer calls

        private readonly static ProfilerMarker setSharingInfoPerfMarker = new ProfilerMarker("MLMediaPlayer.SetSharingInfo");
        private readonly static ProfilerMarker setIDPerfMarker = new ProfilerMarker("MLMediaPlayer.SetID");
        private readonly static ProfilerMarker prepareVideoPerfMarker = new ProfilerMarker("MLMediaPlayer.PrepareVideo");
        private readonly static ProfilerMarker resetAsyncPerfMarker = new ProfilerMarker("MLMediaPlayer.ResetAsync");
        private readonly static ProfilerMarker getAudioChannelCountPerfMarker = new ProfilerMarker("MLMediaPlayer.GetAudioChannelCount");
        private readonly static ProfilerMarker setSpatialAudioPerfMarker = new ProfilerMarker("MLMediaPlayer.SetSpatialAudio");
        private readonly static ProfilerMarker getSpatialAudioPerfMarker = new ProfilerMarker("MLMediaPlayer.GetSpatialAudio");
        private readonly static ProfilerMarker setAudioChannelPositionPerfMarker = new ProfilerMarker("MLMediaPlayer.SetAudioChannelPosition");
        private readonly static ProfilerMarker getAudioChannelPositionPerfMarker = new ProfilerMarker("MLMediaPlayer.GetAudioChannelPosition");
        private readonly static ProfilerMarker seekPerfMarker = new ProfilerMarker("MLMediaPlayer.Seek");
        private readonly static ProfilerMarker setVolumePerfMarker = new ProfilerMarker("MLMediaPlayer.SetVolume");
        private readonly static ProfilerMarker getElapsedTimeMsPerfMarker = new ProfilerMarker("MLMediaPlayer.GetElapsedTimeMs");
        private readonly static ProfilerMarker playPerfMarker = new ProfilerMarker("MLMediaPlayer.Play");
        private readonly static ProfilerMarker pausePerfMarker = new ProfilerMarker("MLMediaPlayer.Pause");
        private readonly static ProfilerMarker stopPerfMarker = new ProfilerMarker("MLMediaPlayer.Stop");
        private readonly static ProfilerMarker selectSubtitleTrackPerfMarker = new ProfilerMarker("MLMediaPlayer.SelectSubtitleTrack");
        private readonly static ProfilerMarker unselectSubtitleTrackPerfMarker = new ProfilerMarker("MLMediaPlayer.UnselectSubtitleTrack");
        private readonly static ProfilerMarker getAllTrackInfoPerfMarker = new ProfilerMarker("MLMediaPlayer.GetAllTrackInfo");
        private readonly static ProfilerMarker requestActivationKeyRequestPerfMarker = new ProfilerMarker("MLMediaPlayer.RequestActivationKeyRequest");
        private readonly static ProfilerMarker getResolutionPerfMarker = new ProfilerMarker("MLMediaPlayer.GetResolution");
        private readonly static ProfilerMarker getVideoBitratePerfMarker = new ProfilerMarker("MLMediaPlayer.GetVideoBitrate");
        private readonly static ProfilerMarker onDestroyPerfMarker = new ProfilerMarker("MLMediaPlayer.OnDestroy");
    }
}
