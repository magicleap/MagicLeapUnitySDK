// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
// Disabling deprecated warning for the internal project
#pragma warning disable 618

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;
using Media = UnityEngine.XR.MagicLeap.MLMedia;

namespace UnitySDKPlayTests
{
    public partial class MLMedia
    {
        public partial class PlayerTests
        {
            private const string WebSource = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";

            public Media.Player MediaPlayer
            {
                get
                {
                    if (_mediaPlayer == null)
                    {
                        var mediaPlayer = new Media.Player(out MLResult result);

                        if (!result.IsOk)
                            Assert.Fail($"Media Player initialization result with error {result}.");

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

            private MLPermissions.Callbacks permissionCallbacks = new MLPermissions.Callbacks();

            private Media.Player _mediaPlayer;

#pragma warning disable CS0414 // Remove unused private members
            private readonly long currentPositionInMiliseconds = 0;
            private readonly int videoWidth, videoHeight;
            private bool renderVideo = false;
#pragma warning restore CS0414 // Remove unused private members

            public bool IsBuffering { get; private set; } = false;
            public bool IsSeeking { get; private set; } = false;
            public long DurationInMiliseconds { get; private set; } = 0;

            public bool IsPlaying => _mediaPlayer is { IsPlaying: true };
            public bool IsPrepared => _mediaPlayer is { IsPrepared: true };

            [Test]
            public void MediaPlayer_Create()
            {
                SetUp();
                TearDown();
            }

            [Test]
            public void MediaPlayer_Prepare()
            {
                SetUp();
                if (!MLResult.DidNativeCallSucceed(MLPermissions.CheckPermission(MLPermission.Internet).Result))
                {
                    Assert.Fail($"Using PathSourceType.Web requires {MLPermission.Internet} permission included in AndroidManifest.xml");
                }
                var result = MediaPlayer.SetSourceURI(WebSource);
                if (!result.IsOk)
                {
                    Assert.Fail($"Couldn't set source, reason: {result}");
                }
                TearDown();
            }

            /// <summary>
            /// Callback handler on Prepare.
            /// </summary>
            /// <param name="mediaplayer"></param>
            private void HandleOnPrepare(Media.Player mediaplayer)
            {
                DurationInMiliseconds = MediaPlayer.GetDurationMilliseconds();
            }

            /// <summary>
            /// Callback handler on video size changed.
            /// </summary>
            /// <param name="player"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            private void HandleOnVideoSizeChanged(Media.Player player, int width, int height)
            {

            }

            /// <summary>
            /// Callback handler on play.
            /// </summary>
            private void HandleOnPlay(Media.Player mediaPlayer)
            {

            }

            /// <summary>
            /// Callback handler on pause.
            /// </summary>
            private void HandleOnPause(Media.Player mediaPlayer)
            {

            }

            /// <summary>
            /// Callback handler on stop.
            /// </summary>
            /// <param name="mediaplayer"></param>
            private void HandleOnStop(Media.Player mediaplayer)
            {

            }

            /// <summary>
            /// Callback handler on video completion.
            /// </summary>
            private void HandleOnCompletion(Media.Player mediaPlayer)
            {

            }

            private void HandleOnResetComplete(Media.Player mediaPlayer)
            {

            }

            /// <summary>
            /// Callback handler on buffering update.
            /// </summary>
            private void HandleOnBufferingUpdate(Media.Player mediaPlayer, float percent)
            {

            }

            /// <summary>
            /// Callback handler on media player info.
            /// </summary>
            private void HandleOnInfo(Media.Player mediaPlayer, Media.Player.Info info)
            {
                switch (info)
                {
                    case Media.Player.Info.NetworkBandwidth:
                        // source media is not local
                        // the parameter extra would contain bandwidth in kbps
                        break;
                    case Media.Player.Info.BufferingStart:
                        IsBuffering = true;
                        break;
                    case Media.Player.Info.BufferingEnd:
                        IsBuffering = false;
                        break;
                    case Media.Player.Info.RenderingStart:
                        renderVideo = true;
                        break;
                    case Media.Player.Info.Stopped:
                        renderVideo = false;
                        break;
                }
            }

            /// <summary>
            /// Callback handler on seek complete.
            /// </summary>
            private void HandleOnSeekComplete(Media.Player mediaPlayer)
            {
                IsSeeking = false;
            }

            /// <summary>
            /// Callback handler on track selected.
            /// </summary>
            /// <param name="mediaPlayer"></param>
            /// <param name="track"></param>
            private void HandleOnTrackSelected(Media.Player mediaPlayer, Media.Player.Track track)
            {

            }

            /// <summary>
            /// Callback handler on captions text.
            /// </summary>
            /// <param name="mediaPlayer"></param>
            /// <param name="text"></param>
            private void HandleOnCaptionsText(Media.Player mediaPlayer, string text)
            {

            }

            private void SetUp()
            {
                MLResult result = MLPermissions.RequestPermission(UnityEngine.Android.Permission.ExternalStorageRead, permissionCallbacks);
                var _ = MediaPlayer;
            }

            private void TearDown()
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

                    StopMLMediaPlayer();
                    _mediaPlayer.Reset();
                    _mediaPlayer.Destroy();
                }
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
                    var result = _mediaPlayer.Stop();

                    if (!result.IsOk)
                    {
                        Assert.Fail($"Couldn't stop Media Player. Reason: {result}");
                    }
                }
            }
        }
    }
}
