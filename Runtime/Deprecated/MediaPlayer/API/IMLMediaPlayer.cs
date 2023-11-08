// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// Interface for target classes used by MLMediaPlayer
    /// Each function must be implemented by the child player type
    /// </summary>
    [Obsolete("MLMedia API is being deprecated and will be removed from the MagicLeap SDK. Use Unity's VideoPlayer component (requires Unity 2022.3.10) instead.")]
    public interface IMLMediaPlayer
    {
        /// <summary>
        /// Gets a bool indicating if the media player is currently playing or not.
        /// </summary>
        public bool IsPlaying { get; }

        /// <summary>
        /// Sets the source path that the media player will play content from.
        /// </summary>
        /// <param name="source">URI of the media.</param>
        /// <returns>
        /// <c>MLResult.Result</c> will be <c>MLResult.Code.Ok</c> if operation succeeded.
        /// <c>MLResult.Result</c> will be <c>MLResult.Code.InvalidParam</c> if failed due to an invalid input parameter.
        /// <c>MLResult.Result</c> will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// <c>MLResult.Result</c> will be <c>MLResult.Code.MediaGenericNoInit</c> if media player was not properly built or initialized.
        /// <c>MLResult.Result</c> will be <c>MLResult.Code.PermissionDenied</c> if attempting to access web content without appropriate network permissions
        /// </returns>
        MLResult SetSourceURI(string source);

        /// <summary>
        /// Sets the source path that the media player will play content from.
        /// </summary>
        /// <param name="source">Path of the media that's on the device.</param>
        /// <returns>
        /// <c>MLResult.Result</c> will be <c>MLResult.Code.Ok</c> if operation succeeded.
        /// <c>MLResult.Result</c> will be <c>MLResult.Code.InvalidParam</c> if failed due to an invalid input parameter.
        /// <c>MLResult.Result</c> will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// <c>MLResult.Result</c> will be <c>MLResult.Code.MediaGenericNoInit</c> if media player was not properly built or initialized.
        /// <c>MLResult.Result</c> will be <c>MLResult.Code.PermissionDenied</c> if attempting to access web content without appropriate network permissions
        /// </returns>
        MLResult SetSourcePath(string source);

        /// <summary>
        /// Plays the video.
        /// </summary>
        /// <returns>
        /// <c>MLResult.Result</c> will be <c>MLResult.Code.Ok</c> if operation succeeded.
        /// </returns>
        MLResult Play();

        /// <summary>
        /// Pauses the video.
        /// </summary>
        /// <returns>
        /// <c>MLResult.Result</c> will be <c>MLResult.Code.Ok</c> if operation succeeded.
        /// </returns>
        MLResult Pause();

        /// <summary>
        /// Stops the video in the editor.
        /// </summary>
        /// <returns>
        /// <c>MLResult.Result</c> will be <c>MLResult.Code.Ok</c> if operation succeeded.
        /// </returns>
        MLResult Stop();

        /// <summary>
        /// Resume the video.
        /// </summary>
        /// <returns>
        /// <c>MLResult.Result</c> will be <c>MLResult.Code.Ok</c> if operation succeeded.
        /// </returns>
        MLResult Resume();

        /// <summary>
        /// Seeks the specified time in the video.
        /// </summary>
        /// <param name="positionSeconds">Absolute time to seek to.</param>
        /// <returns>
        /// <c>MLResult.Result</c> will be <c>MLResult.Code.Ok</c> if operation succeeded.
        /// </returns>
        MLResult Seek(int positionSeconds, MLMedia.Player.SeekMode seekMode);

        /// <summary>
        /// Sets the volume of the video.
        /// </summary>
        /// <param name="vol">Volume between 0 and 1.</param>
        /// <returns>
        /// <c>MLResult.Result</c> will be <c>MLResult.Code.Ok</c> if operation succeeded.
        /// </returns>
        MLResult SetVolume(float vol);

        /// <summary>
        /// Sets the loop flag for the video.
        /// </summary>
        /// <param name="loop">Flag to loop</param>
        /// <returns>
        /// <c>MLResult.Result</c> will be <c>MLResult.Code.Ok</c> if operation succeeded.
        /// </returns>
        MLResult SetLooping(bool loop);

        /// <summary>
        /// Gets the duration of the video in milliseconds
        /// </summary>
        /// <returns>
        /// Duration of the video, -1 on failure.
        /// </returns>
        int GetDurationMilliseconds();

        /// <summary>
        /// Gets the current position of the video in milliseconds
        /// </summary>
        /// <returns>
        /// Position of the playback of the video, -1 on failure.
        /// </returns>
        int GetPositionMilliseconds();

        /// <summary>
        /// Get the width of the video in pixels
        /// </summary>
        /// <returns>
        /// The width of the video, -1 on failure.
        /// </returns>
        int GetWidth();

        /// <summary>
        /// Get the height of the video in pixels
        /// </summary>
        /// <returns>
        /// The height of the video, -1 on failure.
        /// </returns>
        int GetHeight();
    }
}
