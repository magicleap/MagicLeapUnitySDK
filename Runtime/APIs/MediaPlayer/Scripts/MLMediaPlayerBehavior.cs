// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLMediaPlayerBehavior.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2021 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap.Core
{
    public class MLMediaPlayerBehavior : MonoBehaviour
    {
        public enum PathSourceType
        {
            Web,
            StreamingAssets,
            LocalPath,
        }

        [SerializeField, Tooltip("MeshRenderer to display media")]
        private MeshRenderer screen = null;

        [SerializeField, Tooltip("A reference of media player's renderer's material.")]
        private Material videoRenderMaterial = null;

        [SerializeField, Tooltip("Used to indicate the stereo mode set for the media player.")]
        private Video3DLayout stereoMode = Video3DLayout.No3D;

        [Tooltip("Used to set media player content loop.")]
        public bool isLooping;

        [Tooltip("Used to indicate the source path for the media player.")]
        public PathSourceType pathSourceType;

        [Tooltip("URI/Path of the media to be played")]
        public string source;

        public bool IsBuffering { get; private set; } = false;
        public bool IsSeeking { get; private set; } = false;
        public long DurationInMiliseconds { get; private set; } = 0;

        public event Action OnPrepared;
        public event Action OnPlay;
        public event Action OnPause;
        public event Action OnStop;
        public event Action OnReset;
        public event Action OnCompletion;
        public event Action<float> OnBufferingUpdate;
        public event Action<MLMedia.Player.Info> OnInfo;
        public event Action OnSeekComplete;
        public event Action<MLMedia.Player.Track> OnTrackSelected;
        public event Action<string> OnCaptionsText;
        public event Action<float> OnUpdateTimeline;
        public event Action<long> OnUpdateElapsedTime;
        public event Action<bool> OnIsBufferingChanged;

        private long currentPositionInMiliseconds = 0;

#if UNITY_MAGICLEAP || UNITY_ANDROID
        /// <summary>
        /// MediaPlayer reference
        /// </summary>
        public MLMedia.Player MediaPlayer
        {
            get
            {
                if (_mediaPlayer == null)
                {
                    var mediaPlayer = new MLMedia.Player(out MLResult result);

                    if (!result.IsOk)
                        throw new Exception($"Media Player initialization result with error {result}.");

                    _mediaPlayer = mediaPlayer;

                    _mediaPlayer.OnPrepared += HandleOnPrepare;
                    _mediaPlayer.OnVideoSizeChanged += HandleOnVideoSizeChanged;
                    _mediaPlayer.OnPlay += HandleOnPlay;
                    _mediaPlayer.OnPause += HandleOnPause;
                    _mediaPlayer.OnStop += HandleOnStop;
                    _mediaPlayer.OnCompletion += HandleOnCompletion;
                    _mediaPlayer.OnBufferingUpdate += HandleOnBufferingUpdate;
                    _mediaPlayer.OnInfo += HandleOnInfo;
                    _mediaPlayer.OnSeekComplete += HandleOnSeekComplete;
                    _mediaPlayer.OnCEA608 += HandleOnCaptionsText;
                    _mediaPlayer.OnCEA708 += HandleOnCaptionsText;
                    _mediaPlayer.OnTimedText += HandleOnCaptionsText;
                    _mediaPlayer.OnTrackSelected += HandleOnTrackSelected;
                    _mediaPlayer.OnResetComplete += HandleOnResetComplete;
                }

                return _mediaPlayer;
            }
        }

        public bool IsPlaying => _mediaPlayer is {IsPlaying: true};
        public bool IsPrepared =>_mediaPlayer is { IsPrepared: true};

        private MLMedia.Player _mediaPlayer;

        /// <summary>
        /// A reference of the media player texture.
        /// </summary>
        private RenderTexture mediaPlayerTexture = null;

        void Update()
        {
            if (!Application.isEditor && MediaPlayer.IsPlaying && MediaPlayer.VideoRenderer != null)
            {
                MediaPlayer.VideoRenderer.Render();
            }
            if (DurationInMiliseconds > 0 && IsPlaying && !IsSeeking && !IsBuffering)
            {
                UpdateTimeline();
            }
        }

        private void OnDestroy()
        {
            if(_mediaPlayer != null)
            {
                _mediaPlayer.OnPrepared -= HandleOnPrepare;
                _mediaPlayer.OnVideoSizeChanged -= HandleOnVideoSizeChanged;
                _mediaPlayer.OnPlay -= HandleOnPlay;
                _mediaPlayer.OnPause -= HandleOnPause;
                _mediaPlayer.OnStop -= HandleOnStop;
                _mediaPlayer.OnCompletion -= HandleOnCompletion;
                _mediaPlayer.OnBufferingUpdate -= HandleOnBufferingUpdate;
                _mediaPlayer.OnInfo -= HandleOnInfo;
                _mediaPlayer.OnSeekComplete -= HandleOnSeekComplete;
                _mediaPlayer.OnCEA608 -= HandleOnCaptionsText;
                _mediaPlayer.OnCEA708 -= HandleOnCaptionsText;
                _mediaPlayer.OnTimedText -= HandleOnCaptionsText;
                _mediaPlayer.OnTrackSelected -= HandleOnTrackSelected;
                _mediaPlayer.OnResetComplete -= HandleOnResetComplete;

                StopMLMediaPlayer(onDestroy: true);

                _mediaPlayer.Destroy();
            }
        }

        /// <summary>
        /// Prepares the MLMediaPlayer with a source.
        /// </summary>
        public void PrepareMLMediaPlayer()
        {
            if (MediaPlayer.IsPrepared)
            {
                MLPluginLog.Warning("Media Player is already prepared.");
                return;
            }

            if (screen == null)
            {
                MLPluginLog.Warning("PrepareMLMediaPlayer failed, no valid screen found");
                return;
            }

            // Create a Url with provided string and test if its a local file
            MLResult result = MLResult.Create(MLResult.Code.UnspecifiedFailure);
            MLMedia.Player mlPlayer = MediaPlayer as MLMedia.Player;

            if (pathSourceType == PathSourceType.Web)
            {
                if (!MLResult.DidNativeCallSucceed(MLPermissions.CheckPermission(MLPermission.Internet).Result, nameof(MLPermissions.CheckPermission)))
                {
                    Debug.LogError($"Using PathSourceType.Web requires {MLPermission.Internet} permission included in AndroidManifest.xml");
                    result = MLResult.Create(MLResult.Code.PermissionDenied);
                }
                else
                {
                    result = mlPlayer.SetSourceURI(source);
                }
            }
            else if (pathSourceType == PathSourceType.StreamingAssets)
            {
                string streamingPath = Path.Combine(Application.streamingAssetsPath, source);
                result = mlPlayer.SetStreamingSourcePath(streamingPath);
            }
            else if (pathSourceType is PathSourceType.LocalPath)
            {
                string persistentDataPath = Path.Combine(Application.persistentDataPath, source);
                result = mlPlayer.SetSourcePath(persistentDataPath);
            }

            if (!result.IsOk)
            {
                string message = "PrepareMLMediaPlayer failed, source could not be set.";
                MLPluginLog.Warning(message);
                return;
            }

            mlPlayer.PreparePlayerAsync();
        }

        /// <summary>
        /// Stops Media Player and destroys it's instance.
        /// </summary>
        public void StopMLMediaPlayer(bool onDestroy = false)
        {
            if (_mediaPlayer == null)
                return;

            if (!onDestroy)
            {
                _mediaPlayer.Reset();
            }

            if (_mediaPlayer.IsPrepared)
            {
                _mediaPlayer.Stop();
            }
        }

        /// <summary>
        /// Initializes rendering resources for the ml media player.
        /// </summary>
        private void InitializeMLMediaPlayerRenders(int width, int height)
        {
            this.TryApplyVideoRenderMaterial();
            this.TryApplyStereoMode();
            this.CreateTexture(width, height);

            float aspectRatio = width / (float)height;
            transform.localScale = new Vector3(transform.localScale.y * aspectRatio, transform.localScale.y, 1);
        }

        /// <summary>
        /// Utility method used to check and apply stereo mode property on the currently set VideoRenderMaterial material.
        /// </summary>
        private void TryApplyStereoMode()
        {
            if (this.videoRenderMaterial != null)
            {
                if (this.videoRenderMaterial.HasProperty("_VideoStereoMode"))
                {
                    this.videoRenderMaterial.SetInt("_VideoStereoMode", (int)this.stereoMode);
                }
                else if (this.stereoMode == Video3DLayout.No3D)
                {
                    //// Only print a warning if trying to do something other than Mono.
                    //// The Mono stereo render mode is the default which requires no addition logic
                    //// to implement while SideBySide and OverUnder require processing that is
                    //// implemented by supporting the _VideoStereoMode property this relies on.
                    //// The example implementation is distributed in the "StereoVideoRender.shader" shader.
                    MLPluginLog.WarningFormat(
                        "MLMediaPlayerBehavior failed to apply {0} StereoMode, material is missing \"_VideoStereoMode\" property", this.stereoMode);
                }
            }
        }

        /// <summary>
        /// Utility method used to create if necessary and apply the currently set VideoRenderMaterial material.
        /// </summary>
        private void TryApplyVideoRenderMaterial()
        {
            if (this.screen != null)
            {
                this.screen.material = this.videoRenderMaterial;

                // Accessing the renderer's material automatically instantiates it and makes it unique to this renderer, so keep a reference.
                this.videoRenderMaterial = this.screen.material;
            }
        }

        /// <summary>
        /// Creates the texture on the renderer to play the video on <c>Lumin</c>.
        /// </summary>
        private bool CreateTexture(int width, int height)
        {
            width = Mathf.Max(width, 1);
            height = Mathf.Max(height, 1);

            if (mediaPlayerTexture != null && (mediaPlayerTexture.width != width || mediaPlayerTexture.height != height))
            {
                Destroy(mediaPlayerTexture);
                mediaPlayerTexture = null;
            }

            if (mediaPlayerTexture == null)
            {
                // Create texture with given dimensions
                mediaPlayerTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

                // Set texture on quad
                screen.material.SetTexture("_MainTex", this.mediaPlayerTexture);
            }

            MediaPlayer.CreateVideoRenderer((uint)width, (uint)height);
            MediaPlayer.VideoRenderer.SetRenderBuffer(this.mediaPlayerTexture);

            return true;
        }

        /// <summary>
        /// Pauses the video.
        /// </summary>
        public void Pause()
        {
            if (!MediaPlayer.IsPrepared)
                MediaPlayer.OnPrepared -= PlayOnPrepared;

            if (MediaPlayer.IsPlaying)
                MediaPlayer.Pause();
        }

        /// <summary>
        /// Plays the video.
        /// </summary>
        public void Play()
        {
            if (!MediaPlayer.IsPrepared)
            {
                MediaPlayer.OnPrepared += PlayOnPrepared;
                PrepareMLMediaPlayer();
            }
            else
                MediaPlayer.Resume();
        }

        /// <summary>
        /// Handler on video prepared.
        /// Plays the video.
        /// </summary>
        /// <param name="mediaplayer"></param>
        private void PlayOnPrepared(MLMedia.Player mediaplayer)
        {
            MediaPlayer.OnPrepared -= PlayOnPrepared;

            MediaPlayer.SetLooping(isLooping);
            MediaPlayer.Play();
        }

        /// <summary>
        /// Handler when Timeline Slider has changed value.
        /// Moves the play head by a specific percentage of the whole duration.
        /// </summary>
        /// <param name="ms">Miliseconds to seek to</param>
        public void Seek(float ms)
        {
            IsSeeking = true;

            // Jump backwards or forwards in the video by 'time'
            float position = Mathf.Clamp(currentPositionInMiliseconds + ms, 0, DurationInMiliseconds);
            float currentPositionRatio = position / DurationInMiliseconds;
            int lastTimeSoughtMs = Mathf.RoundToInt((currentPositionRatio * DurationInMiliseconds));
            OnUpdateTimeline?.Invoke(currentPositionRatio);
            OnUpdateElapsedTime?.Invoke(lastTimeSoughtMs);
            MediaPlayer.Seek((int)position, MLMedia.Player.SeekMode.Closest);
        }

        /// <summary>
        /// Moves the play head to a specific percentage of the whole duration.
        /// </summary>
        /// <param name="ms"></param>
        public void SeekTo(float ms)
        {
            IsSeeking = true;

            float currentPositionRatio = (float)ms / DurationInMiliseconds;
            int lastTimeSoughtMs = Mathf.RoundToInt(currentPositionRatio * DurationInMiliseconds);
            OnUpdateTimeline?.Invoke(currentPositionRatio);
            OnUpdateElapsedTime?.Invoke(lastTimeSoughtMs);
            MediaPlayer.Seek((int)ms, MLMedia.Player.SeekMode.ClosestSync);
        }

        /// <summary>
        /// Updates Timeline.
        /// </summary>
        private void UpdateTimeline()
        {
            // Only poll the position once per frame to prevent seeking by miniscule amounts.
            currentPositionInMiliseconds = MediaPlayer.GetPositionMilliseconds();
            float currentPositionRatio = (float)currentPositionInMiliseconds / DurationInMiliseconds;
            OnUpdateTimeline?.Invoke(currentPositionRatio);
            OnUpdateElapsedTime?.Invoke(currentPositionInMiliseconds);
        }

        /// <summary>
        /// Callback handler on Prepare.
        /// </summary>
        /// <param name="mediaplayer"></param>
        private void HandleOnPrepare(MLMedia.Player mediaplayer)
        {
            DurationInMiliseconds = MediaPlayer.GetDurationMilliseconds();
            OnPrepared?.Invoke();
        }

        /// <summary>
        /// Callback handler on video size changed.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void HandleOnVideoSizeChanged(MLMedia.Player player, int width, int height)
        {
            InitializeMLMediaPlayerRenders(width, height);
        }

        /// <summary>
        /// Callback handler on play.
        /// </summary>
        private void HandleOnPlay(MLMedia.Player mediaPlayer)
        {
            OnPlay?.Invoke();
        }

        /// <summary>
        /// Callback handler on pause.
        /// </summary>
        private void HandleOnPause(MLMedia.Player mediaPlayer)
        {
            OnPause?.Invoke();
        }

        /// <summary>
        /// Callback handler on stop.
        /// </summary>
        /// <param name="mediaplayer"></param>
        private void HandleOnStop(MLMedia.Player mediaplayer)
        {
            OnStop?.Invoke();
        }

        /// <summary>
        /// Callback handler on video completion.
        /// </summary>
        private void HandleOnCompletion(MLMedia.Player mediaPlayer)
        {
            OnCompletion?.Invoke();
        }

        private void HandleOnResetComplete(MLMedia.Player mediaPlayer)
        {
            PrepareMLMediaPlayer();
            OnReset?.Invoke();
        }

        /// <summary>
        /// Callback handler on buffering update.
        /// </summary>
        private void HandleOnBufferingUpdate(MLMedia.Player mediaPlayer, float percent)
        {
            OnBufferingUpdate?.Invoke(percent);
        }

        /// <summary>
        /// Callback handler on media player info.
        /// </summary>
        private void HandleOnInfo(MLMedia.Player mediaPlayer, MLMedia.Player.Info info)
        {
            switch (info)
            {
                case MLMedia.Player.Info.NetworkBandwidth:
                    // source media is not local
                    // the parameter extra would contain bandwidth in kbps
                    break;
                case MLMedia.Player.Info.BufferingStart:
                    IsBuffering = true;
                    OnIsBufferingChanged?.Invoke(true);
                    break;
                case MLMedia.Player.Info.BufferingEnd:
                    IsBuffering = false;
                    OnIsBufferingChanged?.Invoke(false);
                    break;
            }
        }

        /// <summary>
        /// Callback handler on seek complete.
        /// </summary>
        private void HandleOnSeekComplete(MLMedia.Player mediaPlayer)
        {
            IsSeeking = false;
            OnSeekComplete?.Invoke();
        }

        /// <summary>
        /// Callback handler on track selected.
        /// </summary>
        /// <param name="mediaPlayer"></param>
        /// <param name="track"></param>
        private void HandleOnTrackSelected(MLMedia.Player mediaPlayer, MLMedia.Player.Track track)
        {
            OnTrackSelected?.Invoke(track);
        }

        /// <summary>
        /// Callback handler on captions text.
        /// </summary>
        /// <param name="mediaPlayer"></param>
        /// <param name="text"></param>
        private void HandleOnCaptionsText(MLMedia.Player mediaPlayer, string text)
        {
            OnCaptionsText?.Invoke(text);
        }
#endif
    }
}
