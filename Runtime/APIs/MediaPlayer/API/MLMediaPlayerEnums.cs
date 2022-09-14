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
            /// VideoScalingMode.
            /// </summary>
            public enum VideoScalingMode
            {
                /// <summary>
                ///  Scale to fit.
                /// </summary>
                ScaleToFit = 1,

                /// <summary>
                ///  Scale to fit with cropping.
                /// </summary>
                ScaleToFitWithCropping = 2
            };

            /// <summary>
            /// Comment Needed!
            /// </summary>
            [Flags]
            public enum PollingStateFlags : uint
            {

                /// <summary>
                ///  Playing.
                /// </summary>
                IsPlaying = 1 << 0,

                /// <summary>
                ///  Looping.
                /// </summary>
                IsLooping = 1 << 1,

                /// <summary>
                ///  Prepared.
                /// </summary>
                HasBeenPrepared = 1 << 2,

                /// <summary>
                ///  Playback completed.
                /// </summary>
                HasPlaybackCompleted = 1 << 3,

                /// <summary>
                ///  Seek completed.
                /// </summary>
                HasSeekCompleted = 1 << 4,

                /// <summary>
                ///  Size changed.
                /// </summary>
                HasSizeChanged = 1 << 5,

                /// <summary>
                ///  Buffering updated.
                /// </summary>
                HasBufferingUpdated = 1 << 6,

                /// <summary>
                ///  Received info.
                /// </summary>
                HasReceivedInfo = 1 << 7,

                /// <summary>
                ///  Recieved error.
                /// </summary>
                HasReceivedError = 1 << 8,

                /// <summary>
                ///  Timed text updated.
                /// </summary>
                HasTimedTextUpdated = 1 << 9,

                /// <summary>
                ///  Subtitle updated.
                /// </summary>
                HasSubtitleUpdated = 1 << 10,

                /// <summary>
                ///  Metadata updated.
                /// </summary>
                HasMetadataUpdated = 1 << 11,

                /// <summary>
                ///  DRM info updated.
                /// </summary>
                HasDrmInfoUpdated = 1 << 12,

                /// <summary>
                ///  Async Reset completed.
                /// </summary>
                HasResetCompleted = 1 << 13,

                /// <summary>
                ///  All states.
                /// </summary>
                All = ~0u

            };

            /// <summary>
            /// Comment Needed!
            /// </summary>
            public enum BufferingMode
            {

                /// <summary>
                ///  Do not support buffering.
                /// </summary>
                None,

                /// <summary>
                ///  Support only time based buffering.
                /// </summary>
                TimeOnly,

                /// <summary>
                ///  Support only size based buffering.
                /// </summary>
                SizeOnly,

                /// <summary>
                ///     Support both time and size based buffering, time based calculation precedes
                ///    size based. Size based calculation will be used only when time information is not
                ///    available for the stream.
                /// </summary>
                TimeThenSize,
            };

            /// <summary>
            /// Frame Packing Arrangement for stereoscopic videos.
            /// </summary>
            public enum FramePackingMode
            {
                /// <summary>
                /// Not a stereoscopic video.
                /// </summary>
                None = 0,

                /// <summary>
                /// Checkboard.
                /// </summary>
                Checkboard,

                /// <summary>
                /// Column interleaved.
                /// </summary>
                ColumnInterleaved,

                /// <summary>
                /// Row interleaved.
                /// </summary>
                RowInterleaved,

                /// <summary>
                /// Side by side quincunx.
                /// </summary>
                SideBySideQuincunx,

                /// <summary>
                /// Side by side.
                /// </summary>
                SideBySide,

                /// <summary>
                /// Top bottom.
                /// </summary>
                TopBottom,

                /// <summary>
                /// Multiview frame by frame.
                /// </summary>
                MultiviewFrameByFrame,

                /// <summary>
                /// Frame by frame.
                /// </summary>
                FrameByFrame,

                /// <summary>
                /// Anaglyph.
                /// </summary>
                Anaglyph
            }

            /// <summary>
            /// Frame Packing Arrangement flags for stereoscopic videos.
            /// </summary>
            [Flags]
            public enum FramePackingFlags
            {
                /// <summary>
                /// No flags.
                /// </summary>
                None = 0,

                /// <summary>
                /// Right view first.
                /// </summary>
                RightViewFirst = 1,

                /// <summary>
                /// Left view flipped.
                /// </summary>
                LeftFlipped = 2,

                /// <summary>
                /// Right view flipped. 
                /// </summary>
                RightFlipped = 4,

                /// <summary>
                /// Anaglyph red/cyan.
                /// </summary>
                Anaglyph_RedCyan = 8,

                /// <summary>
                /// Anaglyph green/magenta.
                /// </summary>
                AnaglyphGreenMagenta = 16
            }

            /// <summary>
            /// Mode indicating where exactly to seek to.
            /// </summary>
            public enum SeekMode
            {
                /// <summary>
                /// Seek to the previous key frame.
                /// </summary>
                PreviousSync = 0,

                /// <summary>
                /// Seek to the next key frame.
                /// </summary>
                NextSync,

                /// <summary>
                /// Seek to the closest key frame.
                /// </summary>
                ClosestSync,

                /// <summary>
                /// Seek to the closest frame, more accurate but slower.
                /// </summary>
                Closest
            }

            /// <summary>
            /// Indicates various trigger various media player actions.
            /// <c>MLMediaPlayerInfo</c> from <c>ml_media_player.h</c>.
            /// </summary>
            public enum Info
            {
                /// <summary>
                /// Unknown status
                /// </summary>
                Unknown = 1,

                /// <summary>
                /// The player was started because it was used as the next player.
                /// </summary>
                StartedAsNext = 2,

                /// <summary>
                /// The player just pushed the very first video frame for rendering.
                /// </summary>
                RenderingStart = 3,

                /// <summary>
                /// The player just reached EOS and started from beginning loop.
                /// </summary>
                Looping = 4,

                /// <summary>
                /// The player acknowledgement that it has started playing.
                /// </summary>
                Started = 5,

                /// <summary>
                /// The player acknowledgement that it has paused.
                /// </summary>
                Paused = 6,

                /// <summary>
                /// The player acknowledgement that it has stopped playing.
                /// </summary>
                Stopped = 7,

                /// <summary>
                /// The player acknowledgement that it has started playing as result of shared player's request.
                /// </summary>
                StartedBySharedPlayer = 8,

                /// <summary>
                /// The player acknowledgement that it has paused playing as result of shared player's request.
                /// </summary>
                PausedBySharedPlayer = 9,

                /// <summary>
                /// The player acknowledgement that it is seeking as result of shared player's request.
                /// </summary>
                SeekBySharedPlayer = 10,

                /// <summary>
                /// The player acknowledgement that it has stopped playing as result of shared player's request.
                /// </summary>
                StoppedBySharedPlayer = 11,

                /// <summary>
                /// The Media player has started sync'ing with other shared players.
                /// </summary>
                SyncStart = 12,

                /// <summary>
                /// The Media player has completed sync'ing with other shared players.
                /// </summary>
                SyncComplete = 13,

                /// <summary>
                /// The video is too complex for the decoder: it can't decode frames fast enough.
                /// </summary>
                VideoTrackLagging = 700,

                /// <summary>
                /// Media player is temporarily pausing playback.
                /// </summary>
                BufferingStart = 701,

                /// <summary>
                /// Media player is resuming playback after filling buffers.
                /// </summary>
                BufferingEnd = 702,

                /// <summary>
                /// Network bandwidth info.
                /// </summary>
                NetworkBandwidth = 703,

                /// <summary>
                /// The player is Behind Live Window.
                /// </summary>
                BehindLiveWindow = 704,

                /// <summary>
                /// Media player is paused because device is in sleep or standby state.
                /// </summary>
                PowerStatePause = 705,

                /// <summary>
                /// Media player has resumed playback because device has returned from sleep or standby state.
                /// </summary>
                PowerStateResume = 706,

                /// <summary>
                /// Duration in milliseconds of buffered content.
                /// </summary>
                BufferedDuration = 707,

                /// <summary>
                /// Bad interleaving means that a media has been improperly interleaved.
                /// </summary>
                BadInterleaving = 800,

                /// <summary>
                /// The media is not seekable e.g live stream.
                /// </summary>
                NotSeekable = 801,

                /// <summary>
                /// New media metadata is available.
                /// </summary>
                MetadataUpdate = 802,

                /// <summary>
                /// Audio can not be played.
                /// </summary>
                PlayAudioError = 804,

                /// <summary>
                /// Video can not be played.
                /// </summary>
                PlayVideoError = 805,

                /// <summary>
                /// Media timed text error.
                /// </summary>
                TimedTextError = 900,
            }


            /// <summary>
            /// CEA608 caption color code.
            /// <c>MLCea608CaptionColor</c> from <c>ml_media_cea608_caption.h</c>.
            /// </summary>
            public enum Cea608CaptionColor : uint
            {
                /// <summary>
                /// CEA608 caption color is white.
                /// </summary>
                White = 0,

                /// <summary>
                /// CEA608 caption color is green.
                /// </summary>
                Green = 1,

                /// <summary>
                /// CEA608 caption color is blue.
                /// </summary>
                Blue = 2,

                /// <summary>
                /// CEA608 caption color is cyan.
                /// </summary>>
                Cyan = 3,

                /// <summary>
                /// CEA608 caption color is red.
                /// </summary>
                Red = 4,

                /// <summary>
                /// CEA608 caption color is yellow.
                /// </summary>
                Yellow = 5,

                /// <summary>
                /// CEA608 caption color is magenta.
                /// </summary>
                Magenta = 6,

                /// <summary>
                /// CEA608 caption color is invalid.
                /// </summary>
                Invalid = 7
            }

            /// <summary>
            /// CEA608 caption style code.
            /// <c>MLCea608CaptionStyle</c> from <c>ml_media_cea608_caption.h</c>.
            /// </summary>
            public enum Cea608CaptionStyle : uint
            {
                /// <summary>
                /// CEA608 caption style code is normal.
                /// </summary>
                Normal = 0x00000000,

                /// <summary>
                /// CEA608 caption style code is italics.
                /// </summary>
                Italics = 0x00000001,

                /// <summary>
                /// CEA608 caption style code is underline.
                /// </summary>
                Underline = 0x00000002
            }

            /// <summary>
            /// CEA608 caption Dimension constants.
            /// <c>MLCea608CaptionDimension</c> from <c>ml_media_cea608_caption.h</c>.
            /// </summary>
            public enum Cea608CaptionDimension : int
            {
                /// <summary>
                /// Max number of rows.
                /// </summary>
                MaxRows = 15,

                /// <summary>
                /// Max number of columns.
                /// </summary>
                MaxCols = 32,

                /// <summary>
                /// Max number of plus 2.
                /// </summary>
                MaxRowsPlus2 = 17,

                /// <summary>
                /// Max number of columns plus 2.
                /// </summary>
                MaxColsPlus2 = 34
            }

            /// <summary>
            /// CEA708 Caption Pen Size constants.
            /// <c>MLCea708CaptionPenSize</c> from <c>ml_media_cea708_caption.h</c>.
            /// </summary>
            public enum Cea708CaptionPenSize
            {
                /// <summary>
                /// Small pen size.
                /// </summary>
                Small = 0,

                /// <summary>
                /// Standard pen size.
                /// </summary>
                Standard = 1,

                /// <summary>
                /// Large pen size.
                /// </summary>
                Large = 2,
            }

            /// <summary>
            /// CEA708 Caption Pen Offset constants.
            /// <c>MLCea708CaptionPenOffset</c> from <c>ml_media_cea708_caption.h</c>.
            /// </summary>
            public enum Cea708CaptionPenOffset
            {
                /// <summary>
                /// Subscript offset.
                /// </summary>
                Subscript = 0,

                /// <summary>
                /// Normal offset.
                /// </summary>
                Normal = 1,

                /// <summary>
                /// Superscript offset.
                /// </summary>
                Superscript = 2,
            }

            /// <summary>
            /// CEA708 Caption Emit Commands constants.
            /// <c>MLCea708CaptionEmitCommand</c> from <c>ml_media_cea708_caption.h</c>.
            /// </summary>
            public enum Cea708CaptionEmitCommand : int
            {
                /// <summary>
                /// Buffer command.
                /// </summary>
                Buffer = 1,

                /// <summary>
                /// Control command.
                /// </summary>
                Control = 2,

                /// <summary>
                /// SetCurrentWindow tells the caption decoder which window the following commands describe:
                /// - SetWindowAttributes
                /// - SetPenAttributes
                /// - SetPenColor
                /// - SetPenLocation.
                /// If the window specified has not already been created with a DefineWindow command then,
                /// SetCurrentWindow and the window property commands can be safely ignored.
                /// </summary>
                CWX = 3,

                /// <summary>
                /// ClearWindows clears all the windows specified in the 8 bit window bitmap.
                /// </summary>
                CLW = 4,

                /// <summary>
                /// DisplayWindows displays all the windows specified in the 8 bit window bitmap.
                /// </summary>
                DSW = 5,

                /// <summary>
                /// HideWindows hides all the windows specified in the 8 bit window bitmap.
                /// </summary>
                HDW = 6,

                /// <summary>
                /// ToggleWindows hides all displayed windows, and displays all hidden windows specified in the 8 bit window bitmap.
                /// </summary>
                TGW = 7,

                /// <summary>
                /// DeleteWindows deletes all the windows specified in the 8 bit window bitmap.
                /// If the current window, as specified by the last SetCurrentWindow command,
                /// is deleted then the current window becomes undefined and the window attribute commands
                /// should have no effect until after the next SetCurrentWindow or DefineWindow command.
                /// </summary>
                DLW = 8,

                /// <summary>
                /// Delay suspends all processing of the current service, except for DelayCancel and Reset scanning.
                /// </summary>
                DLY = 9,

                /// <summary>
                /// DelayCancel terminates any active delay and resumes normal command processing. DelayCancel should be scanned for during a Delay.
                /// </summary>
                DLC = 10,

                /// <summary>
                /// Reset deletes all windows, cancels any active delay, and clears the buffer before the Reset command. Reset should be scanned for during a Delay.
                /// </summary>
                RST = 11,

                /// <summary>
                /// The SetPenAttributes command specifies how certain attributes of subsequent characters are to be rendered in the current window, until the next SetPenAttributes command.
                /// </summary>
                SPA = 12,

                /// <summary>
                /// SetPenColor sets the foreground, background, and edge color for the subsequent characters.
                /// </summary>
                SPC = 13,

                /// <summary> SetPenLocation sets the location of for the next bit of appended text in the current window. It has two parameters, row and column.
                /// </summary>
                SPL = 14,

                /// <summary>
                /// SetWindowAttributes Sets the window attributes of the current window.
                /// </summary>
                SWA = 15,

                /// <summary>
                /// DefineWindow0-7 creates one of the eight windows used by a caption decoder.
                /// </summary>
                DFX = 16
            }

            /// <summary>
            /// CEA708 Caption Emit Command Control constants.
            /// <c>MLCea708CaptionEmitCommandControl</c> from <c>ml_media_cea708_caption.h</c>.
            /// </summary>
            public enum Cea708CaptionEmitCommandControl : int
            {
                /// <summary>
                /// End of text.
                /// </summary>
                ETX = 0x03,

                /// <summary>
                /// Back space.
                /// </summary>
                BS = 0x08,

                /// <summary>
                /// This code is equivalent to CEA708 CLW command
                /// </summary>
                FF = 0x0c,

                /// <summary>
                /// Carriage return.
                /// </summary>
                HCR = 0x0e
            }

            /// <summary>
            /// CEA708 Caption Color Opacity constants.
            /// <c>MLCea708CaptionColorOpacity</c> from <c>ml_media_cea708_caption.h</c>.
            /// </summary>
            public enum Cea708CaptionColorOpacity : int
            {
                /// <summary>
                /// Solid opacity.
                /// </summary>
                Solid = 0,

                /// <summary>
                /// Flashing opacity.
                /// </summary>
                Flash = 1,

                /// <summary>
                /// Translucent opacity.
                /// </summary>
                Translucent = 2,

                /// <summary>
                /// Transparent opacity.
                /// </summary>
                Transparent = 3
            }

            /// <summary>
            /// Enumeration of the available audio channel indices in 5.1 SMPTE order.
            /// </summary>
            public enum AudioChannel : uint
            {
                /// <summary>
                /// Front left channel index.
                /// </summary>
                FrontLeft = 0,

                /// <summary>
                /// Front right channel index.
                /// </summary>
                FrontRight = 1,

                /// <summary>
                /// Front center channel index.
                /// </summary>
                FrontCenter = 2,

                /// <summary>
                /// Low frequency effects channel index.
                /// </summary>
                LowFrequencyEffects = 3,

                /// <summary>
                /// Surround left channel index.
                /// </summary>
                SurroundLeft = 4,

                /// <summary>
                /// Surround right channel index.
                /// </summary>
                SurroundRight = 5
            }
        }
    }
}
