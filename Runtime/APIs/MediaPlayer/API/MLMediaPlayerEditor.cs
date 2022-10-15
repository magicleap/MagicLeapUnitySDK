// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
#if UNITY_MAGICLEAP || UNITY_ANDROID

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// MLMedia APIs.
    /// </summary>
    public partial class MLMedia
    {
        public partial class Player
        {
            // TODO : Video module is blacklisted for Relish. Re-enable once we add vulkan media player

            /// <summary>
            /// This class is the video player used in Editor. It implements the IMLMediaPlayer interface.
            /// This version of the media player does not support DRM videos.
            /// </summary>
            // public class Editor : MonoBehaviour, IMLMediaPlayer
            // {
            //     /// <summary>
            //     /// Starting volume of the media player.
            //     /// </summary>
            //     private const float STARTING_VOLUME = 30;
            //
            //     /// <summary>
            //     /// Stores a reference to the audio source.
            //     /// </summary>
            //     private AudioSource audioSource;
            //
            //     /// <summary>
            //     /// Texture used to render the video frame.
            //     /// </summary>
            //     private RenderTexture texture;
            //
            //     public bool IsPlaying => false;
            //
            //     void Awake()
            //     {
            //         this.audioSource = gameObject.AddComponent<AudioSource>();
            //     }
            //
            //
            //     /// Initiate asynchronous reset of media player. Use <see cref="OnResetCompleted"/> event to know when reset completes,
            //     /// the player will be in a pre-prepared state. This method can be called anytime except while asynchronously preparing.
            //     /// </summary>
            //     /// <returns>
            //     /// MLResult.Result will be MLResult.Code.NotImplemented
            //     /// </returns>
            //     public MLResult ResetAsync()
            //     {
            //         MLPluginLog.Error("MLMedia.Player.ResetAsync is only required on device");
            //         return MLResult.Create(MLResult.Code.NotImplemented);
            //     }
            //
            //     public MLResult SetSourceURI(string source)
            //     {
            //         // Create a Url with provided string and test if its a local file
            //         Uri uri;
            //         bool result = Uri.TryCreate(source, UriKind.Absolute, out uri);
            //         this.audioSource.playOnAwake = false;
            //         this.SetVolume(STARTING_VOLUME);
            //         return MLResult.Create(MLResult.Code.Ok);
            //     }
            //
            //     public MLResult SetSourcePath(string source)
            //     {
            //         Path.Combine(Application.streamingAssetsPath, source);
            //         this.audioSource.playOnAwake = false;
            //         this.SetVolume(STARTING_VOLUME);
            //         return MLResult.Create(MLResult.Code.Ok);
            //     }
            //
            //     /// <summary>
            //     /// Plays the video in the editor.
            //     /// </summary>
            //     /// <returns>
            //     /// <c>MLResult.Result</c> will be <c>MLResult.Code.Ok</c>
            //     /// </returns>
            //     public MLResult Play()
            //     {
            //         this.audioSource.Play();
            //         return MLResult.Create(MLResult.Code.Ok);
            //     }
            //
            //     /// <summary>
            //     /// Pauses the video in the editor.
            //     /// </summary>
            //     /// <returns>
            //     /// <c>MLResult.Result</c> will be <c>MLResult.Code.Ok</c>.
            //     /// </returns>
            //     public MLResult Pause()
            //     {
            //         this.audioSource.Pause();
            //         return MLResult.Create(MLResult.Code.Ok);
            //     }
            //
            //     /// <summary>
            //     /// Seeks the specified time in the video in the editor
            //     /// </summary>
            //     /// <param name="positionMilliseconds">Absolute time to seek to</param>
            //     /// <returns>
            //     /// <c>MLResult.Result</c> will be <c>MLResult.Code.Ok</c>
            //     /// </returns>
            //     public MLResult Seek(int positionMilliseconds)
            //     {
            //         const float MSToSeconds = 0.001f;
            //         int seconds = (int)(positionMilliseconds * MSToSeconds);
            //         return MLResult.Create(MLResult.Code.Ok);
            //     }
            //
            //     /// <summary>
            //     /// Sets the volume of the video in the editor
            //     /// </summary>
            //     /// <param name="vol">Volume to be set.</param>
            //     /// <returns>
            //     /// <c>MLResult.Result</c> will be <c>MLResult.Code.Ok</c>
            //     /// </returns>
            //     public MLResult SetVolume(float vol)
            //     {
            //         this.audioSource.volume = vol;
            //         return MLResult.Create(MLResult.Code.Ok);
            //     }
            //
            //     /// <summary>
            //     /// Stops the video in the editor
            //     /// </summary>
            //     /// <returns>
            //     /// <c>MLResult.Result</c> will be <c>MLResult.Code.Ok</c>
            //     /// </returns>
            //     public MLResult Stop()
            //     {
            //         // this.videoPlayer.Stop();
            //         return MLResult.Create(MLResult.Code.Ok);
            //     }
            //
            //     /// <summary>
            //     /// Resumes the video in the editor
            //     /// </summary>
            //     /// <returns>
            //     /// <c>MLResult.Result</c> will be <c>MLResult.Code.Ok</c>
            //     /// </returns>
            //     public MLResult Resume()
            //     {
            //         this.audioSource.Play();
            //         return MLResult.Create(MLResult.Code.Ok);
            //     }
            //
            //     /// <summary>
            //     /// Sets the loop flag for the video in the editor
            //     /// </summary>
            //     /// <param name="loop">Flag to loop</param>
            //     /// <returns>
            //     /// <c>MLResult.Result</c> will be <c>MLResult.Code.Ok</c>
            //     /// </returns>
            //     public MLResult SetLooping(bool loop)
            //     {
            //         this.audioSource.loop = loop;
            //         return MLResult.Create(MLResult.Code.Ok);
            //     }
            //
            //     /// <summary>
            //     /// Releases any resource used by this media player ID.
            //     /// </summary>
            //     /// <returns>
            //     /// <c>MLResult.Result</c> will be <c>MLResult.Code.Ok</c>
            //     /// </returns>
            //     public MLResult Cleanup()
            //     {
            //         UnityEngine.Object.Destroy(this.audioSource);
            //         return MLResult.Create(MLResult.Code.Ok);
            //     }
            //
            //     /// <summary>
            //     /// Selects the subtitle track, not available for editor.
            //     /// </summary>
            //     /// <param name="trackID">(unused) track id to be selected</param>
            //     /// <returns>
            //     /// <c>MLResult.Result</c> will be <c>MLResult.Code.NotImplemented</c>
            //     /// </returns>
            //     public MLResult SelectSubtitleTrack(uint trackID)
            //     {
            //         MLPluginLog.Warning("MLMedia.Player.SelectSubtitleTrack is only available on device");
            //         return MLResult.Create(MLResult.Code.NotImplemented);
            //     }
            //
            //     /// <summary>
            //     /// Unselects the subtitle track, not available for editor.
            //     /// </summary>
            //     /// <param name="trackID">(unused) track id to be selected</param>
            //     /// <returns>
            //     /// <c>MLResult.Result</c> will be <c>MLResult.Code.NotImplemented</c>
            //     /// </returns>
            //     public MLResult UnselectSubtitleTrack(uint trackID)
            //     {
            //         MLPluginLog.Warning("MLMedia.Player.UnselectSubtitleTrack is only available on device");
            //         return MLResult.Create(MLResult.Code.NotImplemented);
            //     }
            //
            //     /// <summary>
            //     /// Gets active audio channel count.
            //     /// </summary>
            //     /// <param name="outAudioChannelCount">(unused) Return channel count.</param>
            //     /// <returns>
            //     /// <c>MLResult.Result</c> will be <c>MLResult.Code.NotImplemented</c>
            //     /// </returns>
            //     public MLResult GetAudioChannelCount(out int outAudioChannelCount)
            //     {
            //         outAudioChannelCount = this.audioSource.clip?.channels ?? 1;
            //         return MLResult.Create(MLResult.Code.Ok);
            //     }
            //
            //     /// <summary>
            //     /// Sets spatial audio state.
            //     /// </summary>
            //     /// <param name="isEnabled">(unused) Desired state of spatial audio.</param>
            //     /// <returns>
            //     /// <c>MLResult.Result</c> will be <c>MLResult.Code.NotImplemented</c>
            //     /// </returns>
            //     public MLResult SetSpatialAudio(bool isEnabled)
            //     {
            //         this.audioSource.spatialize = isEnabled;
            //         return MLResult.Create(MLResult.Code.Ok);
            //     }
            //
            //     /// <summary>
            //     /// Gets spatial audio state.
            //     /// </summary>
            //
            //     /// <param name="outIsEnabled">(unused) Return state of spatial audio.</param>
            //     /// <returns>
            //     /// <c>MLResult.Result</c> will be <c>MLResult.Code.NotImplemented</c>
            //     /// </returns>
            //     public MLResult GetSpatialAudio(out bool outIsEnabled)
            //     {
            //         outIsEnabled = this.audioSource.spatialize;
            //         return MLResult.Create(MLResult.Code.Ok);
            //     }
            //
            //     /// <summary>
            //     /// Sets world position of requested audio channel.
            //     /// </summary>
            //     /// <param name="channel">(unused) Selects the channel whose position is being set.</param>
            //     /// <param name="position">(unused) Set selected channel's world position</param>
            //     /// <returns>
            //     /// <c>MLResult.Result</c> will be <c>MLResult.Code.NotImplemented</c>
            //     /// </returns>
            //     public MLResult SetAudioChannelPosition(MLMedia.Player.AudioChannel channel, Vector3 position)
            //     {
            //         MLPluginLog.Warning("MLMedia.Player.SetAudioChannelPosition is only available on device");
            //         return MLResult.Create(MLResult.Code.NotImplemented);
            //     }
            //
            //     /// <summary>
            //     /// Gets world position of requested audio channel.
            //     /// </summary>
            //     /// <param name="channel">(unused) Selects the channel whose position is being read.</param>
            //     /// <param name="position">(unused) Return selected channel's world position</param>
            //     /// <returns>
            //     /// <c>MLResult.Result</c> will be <c>MLResult.Code.NotImplemented</c>
            //     /// </returns>
            //     public MLResult GetAudioChannelPosition(MLMedia.Player.AudioChannel channel, out Vector3 position)
            //     {
            //         position = new Vector3(0f, 0f, 0f);
            //         MLPluginLog.Warning("MLMedia.Player.GetAudioChannelPosition is only available on device");
            //         return MLResult.Create(MLResult.Code.NotImplemented);
            //     }
            //
            //     /// <summary>
            //     /// Register a request to get the bytes used for a DRM key request.
            //     /// </summary>
            //     /// <param name="drmUUIDBytes">Bytes identifying the desired DRM type.</param>
            //     /// <param name="callback">Callback to be called when successfully retrieved request data.</param>
            //     /// <returns>
            //     /// True if request was successfully registered.
            //     /// </returns>
            //     public bool RequestActivationKeyRequest(byte[] drmUUIDBytes, Action<MLResult, byte[], string> callback)
            //     {
            //         MLPluginLog.Warning("MLMedia.PlayerEditor.RequestActivationKeyRequest failed, editor version of MLMedia.Player does not support DRM.");
            //         return false;
            //     }
            //
            //     /// <summary>
            //     /// Get the video track bitrate
            //     /// </summary>
            //     /// <returns>The bitrate of the video track</returns>
            //     public int GetVideoBitrate()
            //     {
            //         MLPluginLog.Warning("MLMedia.PlayerEditor.GetVideoBitrate failed, editor version of MLMedia.Player does not support bitrate.");
            //         return 0;
            //     }
            //
            //     /// <summary>
            //     /// Gets the duration of the video in milliseconds.
            //     /// </summary>
            //     /// <returns>Duration of the video</returns>
            //     public TimeSpan GetDurationMilliseconds()
            //     {
            //         return TimeSpan.Zero;
            //     }
            //
            //     /// <summary>
            //     /// Gets the current position of the video in milliseconds
            //     /// </summary>
            //     /// <returns>Position of the playback of the video</returns>
            //     public TimeSpan GetPositionMilliseconds()
            //     {
            //         // return (int)this.videoPlayer.time * SecondsToMS;
            //         return TimeSpan.Zero;
            //     }
            //
            //     /// <summary>
            //     /// Get the width of the video in pixels
            //     /// </summary>
            //     /// <returns>The width of the video</returns>
            //     public int GetWidth()
            //     {
            //         // return this.videoPlayer.targetTexture.width;
            //         return this.texture.width;
            //     }
            //
            //     /// <summary>
            //     /// Get the height of the video in pixels
            //     /// </summary>
            //     /// <returns>The height of the video</returns>
            //     public int GetHeight()
            //     {
            //         // return this.videoPlayer.targetTexture.height;
            //         return this.texture.height;
            //     }
            //
            //     /// <summary>
            //     /// Sets the license server for DRM videos (should not be called)
            //     /// </summary>
            //     /// <param name="licenseServer">(unused) URL of the License Server</param>
            //     public void SetLicenseServer(string licenseServer)
            //     {
            //         if (!string.IsNullOrEmpty(licenseServer))
            //         {
            //             MLPluginLog.Warning("MLMedia.PlayerEditor.SetLicenseServer failed, editor version of MLMedia.Player does not support DRM.");
            //         }
            //     }
            //
            //     /// <summary>
            //     /// Set custom header key-value pairs to use in addition to default of <c>"User-Agent : Widevine CDM v1.0"</c>
            //     /// when performing key request to the DRM license server.
            //     /// </summary>
            //     /// <param name="headerData">(unused) Dictionary of custom header key-value pairs</param>
            //     public void SetCustomLicenseHeaderData(Dictionary<string, string> headerData)
            //     {
            //         if (headerData != null)
            //         {
            //             MLPluginLog.Warning("MLMedia.PlayerEditor.SetCustomLicenseHeaderData failed, editor version of MLMedia.Player does not support DRM.");
            //         }
            //     }
            //
            //     /// <summary>
            //     /// Set custom key request key-value pair parameters used when generating default key request.
            //     /// </summary>
            //     /// <param name="messageData">(unused) Dictionary of optional key-value pair parameters</param>
            //     public void SetCustomLicenseMessageData(Dictionary<string, string> messageData)
            //     {
            //         if (messageData != null)
            //         {
            //             MLPluginLog.Warning("MLMedia.PlayerEditor.SetCustomLicenseMessageData failed, editor version of MLMedia.Player does not support DRM.");
            //         }
            //     }
            //
            //     /// <summary>
            //     /// Gets the frame drop threshold.
            //     /// </summary>
            //     /// <returns>The currently set millisecond threshold.</returns>
            //     public ulong GetFrameDropThresholdMs()
            //     {
            //         MLPluginLog.Warning("MLMedia.PlayerEditor.GetFrameDropThresholdMs is only available on device.");
            //         return long.MaxValue;
            //     }
            //
            //     /// <summary>
            //     /// Sets a threshold to drop video frames if they are older than specified value.
            //     /// Setting this to 0 will not drop any frames, this is the default behavior.
            //     /// </summary>
            //     /// <param name="threshold">(unused) New threshold in milliseconds.</param>
            //     public void SetFrameDropThresholdMs(ulong threshold)
            //     {
            //         MLPluginLog.Warning("MLMedia.PlayerEditor.SetFrameDropThresholdMs is only available on device.");
            //     }
            // }
        }
    }
}

#endif
