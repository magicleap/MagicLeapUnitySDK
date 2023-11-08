// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
namespace UnityEngine.XR.MagicLeap
{
	/// <summary>
	/// MLMedia APIs.
	/// </summary>
	public partial class MLMedia
	{
		/// <summary>
		/// Media player script that allows playback of a streaming video (either from file or web URL)
		/// This script will update the main texture parameter of the Renderer attached as a sibling
		/// with the video frame from playback. Audio is also handled through this class and will
		/// playback audio from the file.
		/// </summary>
		public partial class Player
		{
			/// <summary>
			/// Delegate for the any OnMediaPlayerGeneral events (events that only return the mediaPlayer reference)
			/// </summary>
			public delegate void OnMediaPlayerGeneralDelegate(MLMedia.Player mediaPlayer);

			/// <summary>
			/// Delegate for the any OnDRMTrackInfo event.
			/// </summary>
			public delegate void OnDRMTrackInfoDelegate(MLMedia.Player mediaPlayer, MLMedia.Player.Track.DRM.Info trackDRMInfo);

			/// <summary>
			/// Delegate for the any OnTrackSelected event.
			/// </summary>
			public delegate void OnTrackSelectedDelegate(MLMedia.Player mediaPlayer, Track selectedTrack);

			/// <summary>
			/// Delegate for the any OnTrackFound event.
			/// </summary>
			public delegate void OnTrackFoundDelegate(MLMedia.Player mediaPlayer, Track track);

			/// <summary>
			/// Delegate for the any OnCaptionsText event.
			/// </summary>
			public delegate void OnCaptionsTextDelegate(MLMedia.Player mediaPlayer, string text);

			/// <summary>
			/// Delegate for the any OnVideoSizeChanged event.
			/// </summary>
			public delegate void OnVideoSizeChangedDelegate(MLMedia.Player mediaPlayer, int width, int height);

			/// <summary>
			/// Delegate for the any OnError event.
			/// </summary>
			public delegate void OnErrorDelegate(MLMedia.Player mediaPlayer, MLResult.Code errorCode);

			/// <summary>
			/// Delegate for the the OnTimedTextUpdate event.
			/// </summary>
			public delegate void OnTimedTextUpdateDelegate(MLMedia.Player mediaPlayer, ulong timedText);

			/// <summary>
			/// Delegate for the the OnInfo event.
			/// </summary>
			/// <param name="info">The info event that has occured.</param>
			public delegate void OnInfoDelegate(MLMedia.Player mediaPlayer, MLMedia.Player.Info info);

			/// <summary>
			/// Delegate for the the OnBufferingUpdate event.
			/// </summary>
			/// <param name="percent">The percent of buffering completed for the set source.</param>
			public delegate void OnBufferingUpdateDelegate(MLMedia.Player mediaPlayer, float percent);

			/// <summary>
			/// Event used to listen for when Play() has been called.
			/// </summary>
			public event OnMediaPlayerGeneralDelegate OnPlay = delegate { };

			/// <summary>
			/// Event used to listen for when Pause() has been called.
			/// </summary>
			public event OnMediaPlayerGeneralDelegate OnPause = delegate { };

			/// <summary>
			/// Event used to listen for when Stop() has been called.
			/// </summary>
			public event OnMediaPlayerGeneralDelegate OnStop = delegate { };

			public event OnTrackFoundDelegate OnTrackFound = delegate { };

			public event OnTrackSelectedDelegate OnTrackSelected = delegate { };

			public event OnDRMTrackInfoDelegate OnDRMTrackInfo = delegate { };

			public event OnCaptionsTextDelegate OnCEA608 = delegate { };

			public event OnCaptionsTextDelegate OnCEA708 = delegate { };

			/// <summary>
			/// Event used to listen for when timed text apppears.
			/// </summary>
			public event OnCaptionsTextDelegate OnTimedText = delegate { };

			/// <summary>
			/// Event used to listen for when subtitles are updated.
			/// </summary>
			public event OnMediaPlayerGeneralDelegate OnMediaSubtitleUpdate = delegate { };

			/// <summary>
			/// Event used to listen for when the media player frame is available (used for rendering purposes).
			/// </summary>
			public event OnMediaPlayerGeneralDelegate OnFrameAvailable = delegate { };

			/// <summary>
			/// Event used to listen for when the video size changes.
			/// </summary>
			public event OnVideoSizeChangedDelegate OnVideoSizeChanged = delegate { };

			/// <summary>
			/// Event used to listen for when the frames are packed.
			/// </summary>
			public event OnMediaPlayerGeneralDelegate OnFramePacking = delegate { };

			/// <summary>
			/// Event used to listen for when information about the media player, see the MLMedia.Player.Info enum for the types of information given.
			/// </summary>
			public event OnInfoDelegate OnInfo = delegate { };

			/// <summary>
			/// Event used to listen for when the media player has finished preparing.
			/// </summary>
			public event OnMediaPlayerGeneralDelegate OnPrepared = delegate { };

			/// <summary>
			/// Event used to listen for when the media player is buffering.
			/// </summary>
			public event OnBufferingUpdateDelegate OnBufferingUpdate = delegate { };

			/// <summary>
			/// Event used to listen for the media player has completed playing it's source.
			/// </summary>
			public event OnMediaPlayerGeneralDelegate OnCompletion = delegate { };

			/// <summary>
			/// Event used to listen for when the media player has an error.
			/// </summary>
			public event OnErrorDelegate OnError = delegate { };

			/// <summary>
			/// Event used to listen for when the media player has finished seeking.
			/// </summary>
			public event OnMediaPlayerGeneralDelegate OnSeekComplete = delegate { };

			/// <summary>
			/// Event used to listen for when the media player resets.
			/// </summary>
			public event OnMediaPlayerGeneralDelegate OnResetComplete = delegate { };
		}
	}
}
