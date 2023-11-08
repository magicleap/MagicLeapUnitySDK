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
    using System;
    using System.Runtime.InteropServices;
    using MagicLeap.Native;
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
            /// Track from the prepared source that can be selected by the media player.
            /// </summary>
            public partial class Track
            {
                private Track() { }

                internal Track(ulong mediaPlayerHandle, uint trackIndex)
                {
                    this.Index = trackIndex;
                    this.Language = NativeBindings.GetTrackLanguage(mediaPlayerHandle, trackIndex);

                    MLResult.Code resultCode = NativeBindings.MLMediaPlayerGetTrackMediaFormat(mediaPlayerHandle, trackIndex, out ulong formatHandle);
                    if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerGetTrackMediaFormat)))
                    {
                        this.MediaFormat = new MLMediaFormat(formatHandle);
                        MLResult result = this.MediaFormat.GetValue(MLMediaFormatKey.Mime, out string mime);
                        if (result.IsOk)
                        {
                            this.MimeType = mime;
                            if (mime.Contains("audio/"))
                            {
                                result = this.MediaFormat.GetValue(MLMediaFormatKey.Channel_Mask, out int channelMask);
                                if (result.IsOk)
                                {
                                    AudioChannelLayout = (MLAudioOutput.ChannelLayouts)channelMask;
                                }
                            }
                        }
                    }

                    resultCode = NativeBindings.MLMediaPlayerGetTrackType(mediaPlayerHandle, trackIndex, out Type type);
                    MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerGetTrackType));
                    this.TrackType = type;
                }

                public MLAudioOutput.ChannelLayouts AudioChannelLayout
                {
                    get;
                    private set;
                }

                public uint Index
                {
                    get;
                    private set;
                }

                public string Language
                {
                    get;
                    private set;
                }

                public Type TrackType
                {
                    get;
                    private set;
                }

                public string MimeType
                {
                    get;
                    private set;
                }

                public MLMediaFormat MediaFormat
                {
                    get;
                    private set;
                }

                public override string ToString() => $"Index: {this.Index}, Language: {this.Language}, Type: {this.TrackType}, Mime: {this.MimeType}, format: {this.MediaFormat}";

                /// <summary>
                /// Media player track types.
                /// </summary>
                public enum Type
                {
                    /// <summary>
                    ///  Unknown.
                    /// </summary>
                    Unknown = 0,

                    /// <summary>
                    ///  Video.
                    /// </summary>
                    Video,

                    /// <summary>
                    ///  Audio.
                    /// </summary>
                    Audio,

                    /// <summary>
                    ///  Timed text.
                    /// </summary>
                    TimedText,

                    /// <summary>
                    ///  Subtitle.
                    /// </summary>
                    Subtitle,

                    /// <summary>
                    ///  Metadata.
                    /// </summary>
                    Metadata,
                };
            }
        }
    }
}
