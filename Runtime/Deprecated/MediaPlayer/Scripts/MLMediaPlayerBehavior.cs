// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
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
    [Obsolete("MLMedia API is being deprecated and will be removed from the MagicLeap SDK. Use Unity's VideoPlayer component (requires Unity 2022.3.10) instead.")]
    public class MLMediaPlayerBehavior : MonoBehaviour
    {
        public enum PathSourceType
        {
            Web,
            StreamingAssets,
            LocalPath,
        }

        [SerializeField, Tooltip("MeshRenderer to display media")]
        private Renderer screen = null;

        [SerializeField, Tooltip("A reference of media player's renderer's material.")]
        private Material videoRenderMaterial = null;

        [SerializeField, Tooltip("Used to indicate the stereo mode set for the media player.")]
        private Video3DLayout stereoMode = Video3DLayout.No3D;

        [SerializeField, Tooltip("Used to set media player content loop.")]
        private bool isLooping;

        [Tooltip("Used to indicate the source path for the media player.")]
        public PathSourceType pathSourceType;

        [Tooltip("URI/Path of the media to be played")]
        public string source;

        [SerializeField, Tooltip("A reference of the media player texture.")]
        private RenderTexture mediaPlayerTexture = null;

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
#pragma warning disable CS0067
        public event Action<MLMedia.Player.Info> OnInfo;
#pragma warning restore CS0067
        public event Action OnSeekComplete;
        public event Action<MLMedia.Player.Track> OnTrackFound;
        public event Action<MLMedia.Player.Track> OnTrackSelected;
        public event Action<string> OnCaptionsText;
        public event Action<float> OnUpdateTimeline;
        public event Action<long> OnUpdateElapsedTime;
        public event Action<bool> OnIsBufferingChanged;
        public event Action<MLNativeSurfaceYcbcrRenderer> OnVideoRendererInitialized;

        private long currentPositionInMiliseconds = 0;
        private bool hasSetSourceURI = false;
        private int videoWidth, videoHeight;

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
                    _mediaPlayer.OnTrackFound += HandleOnTrackFound;
                }

                return _mediaPlayer;
            }
        }

        public bool IsPlaying => _mediaPlayer is { IsPlaying: true };
        public bool IsPrepared => _mediaPlayer is { IsPrepared: true };

        private MLMedia.Player _mediaPlayer;

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
            if (_mediaPlayer != null)
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
                _mediaPlayer.OnTrackFound -= HandleOnTrackFound;

                if (mediaPlayerTexture != null)
                {
                    mediaPlayerTexture.Release();
                    Destroy(mediaPlayerTexture);
                }

                StopMLMediaPlayer();
                _mediaPlayer.Reset();
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
                Debug.LogWarning("Media Player is already prepared.");
                return;
            }

            if (screen == null)
            {
                Debug.LogWarning("PrepareMLMediaPlayer failed, no valid screen found");
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
                    if (!hasSetSourceURI)
                    {
                        result = mlPlayer.SetSourceURI(source);
                        hasSetSourceURI = true;
                        // For certain streams e.g. dash, need to set surface before we prepare
                        // width x height will become correct after play begins
                        HandleOnVideoSizeChanged(MediaPlayer, 1, 1);
                    }
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
                // SetStreamingSourcePath() will call PreparePlayerAsync() and always return 
                // MLResult.Code.Pending, so we skip logging that result and return.
                if(result != MLResult.Code.Pending)
                {
                    string message = "PrepareMLMediaPlayer failed, source could not be set for " + source + ": " + result.ToString();
                    MLPluginLog.Error(message);
                }
                return;
            }

            mlPlayer.PreparePlayerAsync();
        }

        /// <summary>
        /// Stops Media Player and destroys it's instance.
        /// </summary>
        public void StopMLMediaPlayer()
        {
            if (_mediaPlayer == null)
                return;

            if (_mediaPlayer.IsPrepared)
            {
                _mediaPlayer.Stop();
            }
        }

        /// <summary>
        /// Uses the texture on the renderer to play the video on <c>Magic Leap</c>.
        /// </summary>
        private void SetRendererTexture(RenderTexture texture)
        {
            if (mediaPlayerTexture != texture)
            {
                mediaPlayerTexture.Release();
                Destroy(mediaPlayerTexture);
                mediaPlayerTexture = null;
            }

            if (mediaPlayerTexture == null)
            {
                // Create texture with given dimensions
                mediaPlayerTexture = texture;
            }

            // Set texture on quad
            screen.material.SetTexture("_MainTex", this.mediaPlayerTexture);

            MediaPlayer.CreateVideoRenderer((uint)texture.width, (uint)texture.height);
            MediaPlayer.VideoRenderer.SetRenderBuffer(this.mediaPlayerTexture);
        }

        /// <summary>
        /// Initializes rendering resources for the ml media player.
        /// </summary>
        private void InitializeMLMediaPlayerRenders(int width, int height)
        {
            if (videoRenderMaterial == null)
            {
                Debug.LogWarning($"MLMediaPlayerBehavior failed to initialize, video render material is missing.");
                return;
            }

            TryApplyVideoRenderMaterial(videoRenderMaterial);
            TryApplyStereoMode();

            if (mediaPlayerTexture == null || (mediaPlayerTexture.width != width || mediaPlayerTexture.height != height))
                CreateTexture(width, height);
            else
                SetRendererTexture(mediaPlayerTexture);

            float aspectRatio = width / (float)height;
            transform.localScale = new Vector3(transform.localScale.y * aspectRatio, transform.localScale.y, 1);
            OnVideoRendererInitialized?.Invoke(MediaPlayer.VideoRenderer);
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
                    Debug.LogWarning(
                        $"MLMediaPlayerBehavior failed to apply {stereoMode} StereoMode, material is missing \"_VideoStereoMode\" property");
                }
            }
        }

        /// <summary>
        /// Utility method used to create if necessary and apply the currently set VideoRenderMaterial material.
        /// </summary>
        private void TryApplyVideoRenderMaterial(Material rendererMaterial)
        {
            if (this.screen != null)
            {
                this.screen.material = rendererMaterial;

                // Accessing the renderer's material automatically instantiates it and makes it unique to this renderer, so keep a reference.
                this.videoRenderMaterial = this.screen.material;
            }
        }

        /// <summary>
        /// Creates the texture on the renderer to play the video on <c>Magic Leap</c>.
        /// </summary>
        private void CreateTexture(int width, int height)
        {
            width = Mathf.Max(width, 1);
            height = Mathf.Max(height, 1);

            if (mediaPlayerTexture != null && (mediaPlayerTexture.width != width || mediaPlayerTexture.height != height))
            {
                mediaPlayerTexture.Release();
                Destroy(mediaPlayerTexture);
                mediaPlayerTexture = null;
            }

            if (mediaPlayerTexture == null)
            {
                // Create texture with given dimensions
                mediaPlayerTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);

                // Set texture on quad
                screen.material.SetTexture("_MainTex", this.mediaPlayerTexture);
            }

            MediaPlayer.CreateVideoRenderer((uint)width, (uint)height);
            MediaPlayer.VideoRenderer.SetRenderBuffer(this.mediaPlayerTexture);
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
        /// Resets the video.
        /// </summary>
        public void Reset()
        {
            MediaPlayer.Reset();
            videoWidth = 0;
            videoHeight = 0;
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

        public void SelectTrack(MLMedia.Player.Track track)
        {
            MediaPlayer.SelectTrack((int)track.Index);
        }

        public void UnselectTrack(MLMedia.Player.Track track)
        {
            MediaPlayer.UnselectTrack(track);
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
            hasSetSourceURI = false;
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
            if (width == videoWidth && height == videoHeight)
                return;

            videoWidth = width;
            videoHeight = height;
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
        /// Callback handler on track found.
        /// </summary>
        /// <param name="mediaPlayer"></param>
        /// <param name="track"></param>
        private void HandleOnTrackFound(MLMedia.Player mediaPlayer, MLMedia.Player.Track track)
        {
            OnTrackFound?.Invoke(track);
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
    }
}
