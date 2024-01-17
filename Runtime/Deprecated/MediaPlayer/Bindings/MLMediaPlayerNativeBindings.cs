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
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// MLMedia APIs.
    /// </summary>
    public sealed partial class MLMedia
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
            /// See the MLMediaPlayer native plugin <c>"ml_mediaplayer_plugin.cpp"</c> for additional comments.
            /// </summary>
            internal partial class NativeBindings : Native.MagicLeapNativeBindings
            {
                /// <summary>
                /// Delegate for the OnTimedTextUpdate event.
                /// </summary>
                public delegate void OnTimedTextUpdateDelegate(ulong mediaPlayerHandle, ulong timedTextHandle, IntPtr data);

                /// <summary>
                /// Delegate for the OnMediaSubtitleUpdate event.
                /// </summary>
                public delegate void OnMediaSubtitleUpdateDelegate(ulong mediaPlayerHandle, MLMediaPlayerSubtitleData subtitleData, IntPtr data);

                /// <summary>
                /// Delegate for the OnFrameAvailable event
                /// </summary>
                public delegate void OnFrameAvailableDelegate(ulong mediaPlayerHandle, IntPtr Data);

                /// <summary>
                /// Delegate for the OnVideoSizeChanged event
                /// </summary>
                public delegate void OnVideoSizeChangedDelegate(ulong mediaPlayerHandle, ref MLMediaPlayerOnVideoSizeChangedInfo videoSizeInfo);

                /// <summary>
                /// Delegate for the OnFramePacking event
                /// </summary>
                public delegate void OnFramePackingDelegate(ulong mediaPlayerHandle, IntPtr framePackingInfo);

                /// <summary>
                /// Delegate for the OnInfo event
                /// </summary>
                public delegate void OnInfoDelegate(ulong mediaPlayerHandle, ref MLMediaPlayerOnInfoInfo info);

                /// <summary>
                /// Delegate for the OnPrepared event
                /// </summary>
                public delegate void OnPreparedDelegate(ulong mediaPlayerHandle, ref MLMediaPlayerOnPreparedInfo preparedInfo);

                /// <summary>
                /// Delegate for the OnBufferingUpdate event
                /// </summary>
                public delegate void OnBufferingUpdateDelegate(ulong mediaPlayerHandle, ref MLMediaPlayerOnBufferingUpdateInfo updateInfo);

                /// <summary>
                /// Delegate for the OnCompletion event
                /// </summary>
                public delegate void OnCompletionDelegate(ulong mediaPlayerHandle, ref MLMediaPlayerOnCompletionInfo completionInfo);

                /// <summary>
                /// Delegate for the OnError event
                /// </summary>
                public delegate void OnErrorDelegate(ulong mediaPlayerHandle, ref MLMediaPlayerOnErrorInfo errorInfo);

                /// <summary>
                /// Delegate for the OnSeekComplete event
                /// </summary>
                public delegate void OnSeekCompleteDelegate(ulong mediaPlayerHandle, ref MLMediaPlayerOnSeekCompleteInfo seekCompleteInfo);

                /// <summary>
                /// Delegate for the OnTrackDRMInfo event
                /// </summary>
                public delegate void OnTrackDRMInfoDelegate(ulong mediaPlayerHandle, ref MLMediaPlayerOnTrackDRMInfo trackDrmInfo);

                /// <summary>
                /// Delegate for the OnResetComplete event
                /// </summary>
                public delegate void OnResetCompleteDelegate(ulong mediaPlayerHandle, ref MLMediaPlayerOnResetCompleteInfo resetCompleteInfo);


                /// <summary>
                /// MediaPlayer Video Size Changed callback Info.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaPlayerOnVideoSizeChangedInfo
                {
                    /// <summary>
                    /// The new surface width.
                    /// </summary>
                    public int Width;

                    /// <summary>
                    /// The new surface height.
                    /// </summary>
                    public int Height;

                    /// <summary>
                    /// User data as passed to MLMediaPlayerSetEventCallbacksEx().
                    /// </summary>
                    public IntPtr Data;
                };

                /// <summary>
                /// MediaPlayer Track Information.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaPlayerTrackInfo
                {
                    /// <summary>
                    /// Struct version.
                    /// </summary>
                    public uint Version;

                    /// <summary>
                    /// Media track type, can be either audio or video.
                    /// </summary>
                    public Track.Type TrackType;

                    public MLMediaPlayerTrackInfo(Track.Type type)
                    {
                        this.Version = 1;
                        this.TrackType = type;
                    }
                };

                /// <summary>
                /// MediaPlayer DRM Info for a Media Track.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaPlayerTrackDRMInfo
                {
                    /// <summary>
                    /// Number of PSSH entries.
                    /// </summary>
                    public ulong PsshInfoCount;

                    /// <summary>
                    /// Pointer to array of #MLMediaPlayerPSSHEntry of size pssh_info_count.
                    /// </summary>
                    public IntPtr PsshInfo;

                    /// <summary>
                    /// Number of supported DRM UUID entries.
                    /// </summary>
                    public ulong UuidCount;

                    /// <summary>
                    /// Pointer to array of #MLUUID of size uuid_count.
                    /// </summary>
                    public IntPtr UUIDs;

                    /// <summary>
                    /// Media track type, can be either audio or video.
                    /// </summary>
                    public Track.Type TrackType;

                    public MLMedia.Player.Track.DRM.Info Data
                    {
                        get
                        {
                            var drmTrackInfo = new MLMedia.Player.Track.DRM.Info();
                            drmTrackInfo.TrackType = this.TrackType;
                            drmTrackInfo.UUIDS = ConvertArray<MagicLeapNativeBindings.MLUUID>(this.UUIDs, this.UuidCount);
                            drmTrackInfo.PSSHEntries = ConvertArray<Player.NativeBindings.MLMediaPlayerPSSHEntry>(this.PsshInfo, this.PsshInfoCount);
                            return drmTrackInfo;
                        }
                    }
                };

                /// <summary>
                /// PSSH entry.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaPlayerPSSHEntry
                {
                    /// <summary>
                    /// 16 bytes UUID specifying crypto scheme.
                    /// </summary>
                    public MagicLeapNativeBindings.MLUUID uuid;

                    /// <summary>
                    /// Size of the data payload.
                    /// </summary>
                    public ulong size;

                    /// <summary>
                    /// Data specific to that scheme.
                    /// </summary>
                    public IntPtr data;
                };

                /// <summary>
                /// MediaPlayer Playback prepared callback Info.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaPlayerOnPreparedInfo
                {
                    /// <summary>
                    /// User data as passed to MLMediaPlayerSetEventCallbacksEx().
                    /// </summary>
                    public IntPtr Data;

                };

                /// <summary>
                /// MediaPlayer subtitle data.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaPlayerSubtitleData
                {
                    /// <summary>
                    /// Index of the track the subtitle is on.
                    /// </summary>
                    public uint TrackIndex;

                    /// <summary>
                    /// Start time of the subtitle.
                    /// </summary>
                    public ulong StartTimeUs;

                    /// <summary>
                    /// Duration of the subtitle.
                    /// </summary>
                    public ulong DurationUs;

                    /// <summary>
                    /// Size of the Data array.
                    /// </summary>
                    public uint DataSize;

                    /// <summary>
                    /// Data of the subtitle.
                    /// </summary>
                    public IntPtr Data;
                };

                /// <summary>
                /// Indicate the buffering settings.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaPlayerBufferingSettings
                {
                    /// <summary>
                    /// For prepare.
                    /// </summary>
                    public BufferingMode InitialBufferingMode;

                    /// <summary>
                    /// For playback.
                    /// </summary>
                    public BufferingMode RebufferingMode;

                    /// <summary>
                    /// Time based.
                    /// </summary>
                    public int InitialWatermarkMs;

                    /// <summary>
                    /// Size based.
                    /// </summary>
                    public int InitialWatermarkKb;

                    /// <summary>
                    /// When cached data is below this mark, playback will be paused for buffering until data reaches
                    /// |mRebufferingWatermarkHighMs| or end of stream.
                    /// </summary>
                    public int RebufferingWatermarkLowMs;

                    /// <summary>
                    /// When cached data is above this mark, buffering will be paused.
                    /// </summary>
                    public int RebufferingWatermarkHighMs;

                    /// <summary>
                    /// When cached data is below this mark, playback will be paused for buffering until data reaches
                    /// |mRebufferingWatermarkHighKB| or end of stream.
                    /// </summary>
                    public int RebufferingWatermarkLowKb;

                    /// <summary>
                    /// When cached data is above this mark, buffering will be paused.
                    /// </summary>
                    public int RebufferingWatermarkHighKb;
                };

                /// <summary>
                /// MediaPlayer Buffering update callback Info.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaPlayerOnBufferingUpdateInfo
                {
                    /// <summary>
                    /// The new percentage of buffered content.
                    /// </summary>
                    public int Percent;

                    /// <summary>
                    /// User data as passed to MLMediaPlayerSetEventCallbacksEx().
                    /// </summary>
                    public IntPtr Data;
                };

                /// <summary>
                /// MediaPlayer Playback completion callback Info.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaPlayerOnCompletionInfo
                {
                    /// <summary>
                    /// User data as passed to MLMediaPlayerSetEventCallbacksEx().
                    /// </summary>
                    public IntPtr Data;
                };

                /// <summary>
                /// MediaPlayer Error callback Info.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaPlayerOnErrorInfo
                {
                    /// <summary>
                    /// Error/result code indicating failure reason.
                    /// </summary>
                    public MLResult.Code Result;

                    /// <summary>
                    /// User data as passed to MLMediaPlayerSetEventCallbacksEx().
                    /// </summary>
                    public IntPtr Data;
                };

                /// <summary>
                /// MediaPlayer Information callback Info.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaPlayerOnInfoInfo
                {
                    /// <summary>
                    /// Type of informational event.
                    /// </summary>
                    public Info Info;

                    /// <summary>
                    /// MLMediaPlayerInfo type specific extra information.  When info is MLMediaPlayerInfo_NetworkBandwidth, this holds
                    /// bandwidth in kbps.  It is 0 for others.
                    /// </summary>
                    public int Extra;

                    /// <summary>
                    /// User data as passed to MLMediaPlayerSetEventCallbacksEx().
                    /// </summary>
                    public IntPtr Data;
                };

                /// <summary>
                /// MediaPlayer Playback seek completion callback Info.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaPlayerOnSeekCompleteInfo
                {
                    /// <summary>
                    /// User data as passed to MLMediaPlayerSetEventCallbacksEx().
                    /// </summary>
                    public IntPtr Data;
                };

                /// <summary>
                /// MediaPlayer Track DRM Info callback Info.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaPlayerOnTrackDRMInfo
                {
                    /// <summary>
                    /// Pointer to MLMediaPlayerTrackDRMInfo.
                    /// </summary>
                    public IntPtr TrackInfo;

                    /// <summary>
                    /// User data as passed to MLMediaPlayerSetEventCallbacksEx().
                    /// </summary>
                    public IntPtr Data;
                };

                /// <summary>
                /// MediaPlayer reset completion callback Info.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaPlayerOnResetCompleteInfo
                {
                    /// <summary>
                    /// User data as passed to MLMediaPlayerSetEventCallbacksEx().
                    /// </summary>
                    public IntPtr Data;
                };

                /// <summary>
                /// MediaPlayer frame packing callback Info.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaPlayerOnFramePackingInfo
                {

                    /// <summary>
                    /// Frame packing more.
                    /// </summary>
                    public FramePackingMode Mode;

                    /// <summary>
                    /// Frame packing flag.
                    /// </summary>
                    public FramePackingFlags Flags;

                    /// <summary>
                    /// User data as passed to MLMediaPlayerSetEventCallbacksEx().
                    /// </summary>
                    public IntPtr Data;
                };

                /// <summary>
                /// Callbacks for notifying client about
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaPlayerEventCallbacksEx
                {
                    /// <summary>
                    /// Struct version.
                    /// </summary>
                    private uint version;

                    /// <summary>
                    /// This callback function is invoked when buffered contents percentage changed.
                    /// </summary>
                    public OnBufferingUpdateDelegate OnBufferingUpdate;

                    /// <summary>
                    /// This callback is invoked when media player played back until end of media and has now come to a stop.  Note that this
                    /// callback does not fire when 'looping = true', because
                    /// </summary>
                    public OnCompletionDelegate OnCompletion;

                    /// <summary>
                    /// This callback function is invoked when media player encounters an error.
                    /// </summary>
                    public OnErrorDelegate OnError;

                    /// <summary>
                    /// This callback function is invoked when
                    /// </summary>
                    public OnInfoDelegate OnInfo;

                    /// <summary>
                    /// This callback is invoked when the player has finished preparing media and is ready to playback.
                    /// </summary>
                    public OnPreparedDelegate OnPrepared;

                    /// <summary>
                    /// This callback function is invoked when a seek operation has completed.
                    /// </summary>
                    public OnSeekCompleteDelegate OnSeekComplete;

                    /// <summary>
                    /// This callback function is invoked when the internal surface has changed size.
                    /// </summary>
                    public OnVideoSizeChangedDelegate OnVideoSizeChanged;

                    /// <summary>
                    /// This callback function is invoked when source has DRM protected media track(s).
                    /// </summary>
                    public OnTrackDRMInfoDelegate OnTrackDRMInfo;

                    /// <summary>
                    /// This callback function is invoked when an async reset operation has completed.
                    /// </summary>
                    public OnResetCompleteDelegate OnResetComplete;

                    /// <summary>
                    /// This callback function is invoked when a stereoscopic video frame packing change.
                    /// </summary>
                    public OnFramePackingDelegate OnFramePacking;

                    /// <summary>
                    /// Create and return an initialized version of this struct.
                    /// </summary>
                    /// <returns>A new instance of this struct.</returns>
                    public static MLMediaPlayerEventCallbacksEx Create()
                    {
                        return new MLMediaPlayerEventCallbacksEx()
                        {
                            version = 2,
                            OnBufferingUpdate = NativeBindings.OnBufferingUpdate,
                            OnCompletion = NativeBindings.OnCompletion,
                            OnError = NativeBindings.OnError,
                            OnInfo = NativeBindings.OnInfo,
                            OnPrepared = NativeBindings.OnPrepared,
                            OnSeekComplete = NativeBindings.OnSeekComplete,
                            OnVideoSizeChanged = NativeBindings.OnVideoSizeChanged,
                            OnTrackDRMInfo = NativeBindings.OnTrackDRMInfo,
                            OnResetComplete = NativeBindings.OnResetComplete,
                            OnFramePacking = NativeBindings.OnFramePacking
                        };
                    }
                };


                /// <summary>
                /// DRM Session information for a MediaPlayer Track.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaPlayerTrackDRMSessionInfo
                {

                    internal MLMediaPlayerTrackDRMSessionInfo(Track.Type type)
                    {
                        this.Version = 1;
                        this.TrackType = type;
                        this.Uuid = IntPtr.Zero;
                        this.SessionId = IntPtr.Zero;
                    }

                    internal void AllocResources(MagicLeapNativeBindings.MLUUID uuid, Player.Track.DRM.NativeBindings.MLMediaDRMByteArray sessionId)
                    {
                        this.Uuid = Marshal.AllocHGlobal(Marshal.SizeOf<MagicLeapNativeBindings.MLUUID>());
                        Marshal.StructureToPtr(uuid, this.Uuid, true);
                        this.SessionId = Marshal.AllocHGlobal(Marshal.SizeOf<Player.Track.DRM.NativeBindings.MLMediaDRMByteArray>());
                        Marshal.StructureToPtr(sessionId, this.SessionId, true);
                    }

                    internal void FreeResources()
                    {
                        Marshal.FreeHGlobal(this.Uuid);
                        Marshal.FreeHGlobal(this.SessionId);
                    }

                    public uint Version;

                    /// <summary>
                    /// Media track type, can be either audio or video.
                    /// </summary>
                    public Track.Type TrackType;

                    /// <summary>
                    /// UUID of the DRM Scheme of type #MLUUID.
                    /// </summary>
                    public IntPtr Uuid;

                    /// <summary>
                    /// DRM Session ID of type #MLMediaDRMByteArray.
                    /// </summary>
                    public IntPtr SessionId;
                };

                /// <summary>
                /// MediaPlayer metrics data.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaPlayerMetrics
                {

                    /// <summary>
                    /// The average framerate (fps) of video playback
                    /// </summary>
                    public float AverageFramerate;

                    /// <summary>
                    /// The average bitrate (bps) of video playbac
                    /// </summary>
                    public int AverageVideoBitrate;

                    /// <summary>
                    /// The average bitrate (bps) of audio playbac
                    /// </summary>
                    public int AverageAudioBitrate;
                };

                public enum TTMLLayoutAlignment : uint
                {
                    Unspecified = 0xFFFFFFFF,
                    Normal = 0,
                    Center = 1,
                    Opposite = 2
                }

                public enum TTMLLineType : uint
                {
                    Unset = 0xFFFFFFFF,
                    Fraciton = 0,
                    Number = 1
                }
                public enum TTMLAnchorType : uint
                {
                    Unset = 0xFFFFFFFF,
                    Start = 0,
                    Middle = 1,
                    End = 2
                }

                public enum TTMLTextSizeType : uint
                {
                    Unset = 0xFFFFFFFF,
                    Fractional = 0,
                    FractionalIgnorePadding = 1,
                    Absolute = 2
                }

                /// <summary>
                /// TTMLData data structure.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct TTMLImage
                {
                    /// <summary>
                    /// Image size in bytes.
                    /// </summary>
                    public ushort Size;

                    /// <summary>
                    /// Byte data of the image. 
                    /// </summary>
                    public IntPtr Data;
                }


                /// <summary>
                /// TTMLData data structure.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct TTMLData
                {
                    /// <summary>
                    /// Track cue start time in milliseconds. 
                    /// </summary>
                    long startTimeMs;

                    /// <summary>
                    /// Track cue end time in milliseconds. 
                    /// </summary>
                    long endTimeMs;

                    /// <summary>
                    /// The cue text encoded as UTF-8. Or null if this is an image cue.
                    /// </summary>
                    public string text;

                    /// <summary>
                    /// The alignment of the cue text within the cue box.
                    /// </summary>
                    TTMLLayoutAlignment textAlignment;

                    /// <summary>
                    /// The cue image if this is an image cue, an empty image otherwise. 
                    /// </summary>
                    TTMLImage bitmap;

                    /// <summary>
                    /// The cue position within the viewport in the direction orthogonal to the writing direction.
                    /// </summary>
                    float line;

                    /// <summary>
                    /// The cue line type.
                    /// </summary>
                    TTMLLineType lineType;

                    /// <summary>
                    /// The cue box anchor in the direction of line.
                    /// </summary>
                    TTMLAnchorType lineAnchor;

                    /// <summary>
                    ///  The cue position within the viewport in the direction orthogonal to line.
                    /// </summary>
                    float position;

                    /// <summary>
                    /// The cue box anchor in the direction of position.
                    /// </summary>
                    TTMLAnchorType positionAnchor;

                    /// <summary>
                    /// The cue box size in the writing direction, as a fraction of the viewport size.
                    /// </summary>
                    float size;

                    /// <summary>
                    /// The cue bitmap height as a fraction of the viewport size.
                    /// </summary>
                    float bitmapHeight;

                    /// <summary>
                    /// Whether the cue box has a window color.
                    /// </summary>
                    bool windowColorSet;

                    /// <summary>
                    /// The cue window fill color in ARGB format.
                    /// </summary>
                    uint windowColor;

                    /// <summary>
                    /// The cue default text size type, or MLTTMLTextSizeType_Unset if this cue has no default text size.
                    /// </summary>
                    TTMLTextSizeType textSizeType;

                    /// <summary>
                    /// The cue default text size, or MLTTMLDimen_Unset if this cue has no default.
                    /// </summary>
                    float textSize;
                }

                public enum WebVTTOrientation
                {
                    Horizontal = 0,
                    Vertical
                }

                public enum WebVTTDirection
                {
                    Default = 0,
                    LeftToRight,
                    RightToLeft
                }
                public enum WebVTTAlign
                {
                    Start = 0,
                    Middle,
                    End,
                    Left,
                    Right
                }

                /// <summary>
                /// WebVTT data structure.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct WebVTTData
                {
                    /// <summary>
                    /// Track cue start time in milliseconds.
                    /// </summary>
                    public long StartTimeMS;

                    /// <summary>
                    /// Track cue end time in milliseconds.
                    /// </summary>
                    public long EndTimeMs;

                    /// <summary>
                    /// WebVTT file body encoded as UTF-8.
                    /// </summary>
                    [MarshalAs(UnmanagedType.LPStr)]
                    public string Body;

                    /// <summary>
                    /// A sequence of characters unique amongst all the WebVTT cue identifiers.
                    /// </summary>
                    [MarshalAs(UnmanagedType.LPStr)]
                    public string Id;

                    /// <summary>
                    /// A boolean indicating whether the line is an integer number of lines.
                    /// </summary>
                    [MarshalAs(UnmanagedType.I1)]
                    public bool SnapToLines;

                    /// <summary>
                    /// Orientation of the cue.
                    /// </summary>
                    public WebVTTOrientation Orientation;

                    /// <summary>
                    /// The writing direction.
                    /// </summary>
                    public WebVTTDirection Direction;

                    /// <summary>
                    /// Relative cue line position.
                    /// </summary>
                    public float RelativeLinePosition;

                    /// <summary>
                    /// WebVTT cue line number.
                    /// </summary>
                    public int LineNumber;

                    /// <summary>
                    /// The indent of the cue box in the direction defined by the writing direction.
                    /// </summary>
                    public float TextPosition;

                    /// <summary>
                    /// WebVTT cue size.
                    /// </summary>
                    public float Size;

                    /// <summary>
                    /// WebVTT cue text alignment.
                    /// </summary>
                    public WebVTTAlign align;
                };

                /// <summary>
                /// Converts an unmanged array to a managed array of type T.
                /// </summary>
                public static T[] ConvertArray<T>(IntPtr arrayPtr, ulong count)
                {
                    T[] convertedArray = new T[count];
                    IntPtr walkPtr = arrayPtr;
                    for (ulong i = 0; i < count; ++i)
                    {
                        convertedArray[i] = Marshal.PtrToStructure<T>(walkPtr);
                        walkPtr = new IntPtr(walkPtr.ToInt64() + Marshal.SizeOf<IntPtr>());
                    }

                    return convertedArray;
                }

                /// <summary>
                /// Set callbacks to notify clients about player events.  This structure must be initialized by calling
                /// MLMediaPlayerEventCallbacksExInit() before use.  Client needs to implement the callback functions declared in
                /// #MLMediaPlayerEventCallbacksEx, but can set to NULL the ones he does not care about.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerSetEventCallbacksEx(ulong mediaPlayerHandle, [In] ref MLMediaPlayerEventCallbacksEx callbacks, IntPtr Data);

                /// <summary>
                /// Create a new
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerCreate(out ulong handle);

                /// <summary>
                /// Reset the
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerReset(ulong mediaPlayerHandle);

                /// <summary>
                /// Set the 'next'
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerSetNextPlayer(ulong mediaPlayerHandle, ulong NextMediaPlayer);

                /// <summary>
                /// Stop playback after playback has been stopped or paused.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerStop(ulong mediaPlayerHandle);

                /// <summary>
                /// Set the looping mode of the player.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerSetLooping(ulong mediaPlayerHandle, [MarshalAs(UnmanagedType.I1)] bool loop);

                /// <summary>
                /// Return current position of playback.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerGetCurrentPosition(ulong mediaPlayerHandle, out int ms);

                /// <summary>
                /// Get the handle of the audio stream.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerGetAudioHandle(ulong mediaPlayerHandle, out ulong handle);

                /// <summary>
                /// Prepare DRM for the selected media (either audio or video) track
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerPrepareTrackDRM(ulong mediaPlayerHandle, [In] ref MLMediaPlayerTrackDRMSessionInfo drmSessionInfo);

                /// <summary>
                /// Add a timedtext source from a URI.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerAddTimedTextSourceForURI(ulong mediaPlayerHandle, [MarshalAs(UnmanagedType.LPStr)] string Uri, [MarshalAs(UnmanagedType.LPStr)] string MimeType);

                /// <summary>
                /// Destroy a MediaPlayer object.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerDestroy(ulong mediaPlayerHandle);

                [DllImport(CUtilsDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern void MLUnityQueueMediaPlayerResetAndDestroy(ulong mediaPlayerHandle);

                /// <summary>
                /// Set a file descriptor as the data source.  The file descriptor must be seekable.  It is the caller's responsibility to
                /// close the file descriptor.  It is safe to do so as soon as this call returns.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerSetDataSourceForFD(ulong mediaPlayerHandle, int Fd);

                /// <summary>
                /// Set a file descriptor as the data source with offset.  The file descriptor must be seekable.  It is the caller's
                /// responsibility to close the file descriptor.  It is safe to do so as soon as this call returns.  This API is useful for
                /// specifying playable media located in resource files.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerSetOffsetDataSourceForFD(ulong mediaPlayerHandle, int Fd, long Offset, long Length);

                /// <summary>
                /// Set a local file path as the data source.  The path should be an absolute path and should reference a world-readable
                /// file.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerSetDataSourceForPath(ulong mediaPlayerHandle, [MarshalAs(UnmanagedType.LPStr)] string Path);

                /// <summary>
                /// Set a URI string as the data source.  Supported URI schemes are `file`, `http`, `https`, and `rtsp`.  If looking to
                /// provide headers, use MLMediaPlayerSetRemoteDataSourceForURI().
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerSetDataSourceForURI(ulong mediaPlayerHandle, [MarshalAs(UnmanagedType.LPStr)] string Uri);

                /// <summary>
                /// Return last info received by internal
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerGetInfo(ulong mediaPlayerHandle, IntPtr OutCode, IntPtr OutExtra);

                /// <summary>
                /// Set a URI string as the remote data source.  Supported URI schemes are `file`, `http`, `https`, and `rtsp`.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerSetRemoteDataSourceForURI(ulong mediaPlayerHandle, [MarshalAs(UnmanagedType.LPStr)] string Uri, out IntPtr Headers, uint Len);

                /// <summary>
                /// Set the data source to use.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerSetMediaStreamSource(ulong mediaPlayerHandle, ulong MediaStreamSource);

                /// <summary>
                /// Set the data source to use.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerSetMediaDataSource(ulong mediaPlayerHandle, ulong MediaDataSource);

                /// <summary>
                /// Add a timedtext source from a file descriptor.  The file descriptor must be seekable.  It is the caller's responsibility
                /// to close the file descriptor.  It is safe to do so as soon as this call returns.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerAddTimedTextSourceForFD(ulong mediaPlayerHandle, int Fd, long Offset, long Length, [MarshalAs(UnmanagedType.LPStr)] string MimeType);

                /// <summary>
                /// Return last timed text event information.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerGetTimedText(ulong mediaPlayerHandle, IntPtr OutText, IntPtr OutStart, IntPtr OutEnd);

                /// <summary>
                /// Reset the
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerResetAsync(ulong mediaPlayerHandle);


                /// <summary>
                /// Prepare the player for playback, synchronously.  After setting the data source and the #GraphicBufferProducer, you need
                /// to either call prepare() or prepareAsync().  For files, it is OK to call prepare(), which blocks until
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerPrepare(ulong mediaPlayerHandle);

                /// <summary>
                /// Prepare the player for playback, asynchronously.  After setting the data's source and the #GraphicBufferProducer, you
                /// need to either call prepare() or prepareAsync().  For streams, you should call prepareAsync(), which returns immediately,
                /// rather than blocking until enough data has been buffered.  Prepared state will then be obtained via the 'on_prepared'
                /// callback if already registered (see MLMediaPlayerSetEventCallbacksEx), or polled for via the MLMediaPlayerPollStates()
                /// call with the MLMediaPlayerPollingStateFlag_HasBeenPrepared flag set.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerPrepareAsync(ulong mediaPlayerHandle);

                /// <summary>
                /// Start or resumes playback.  If playback had previously been paused, playback will continue from where it was paused.  If
                /// playback had been stopped, or never started before, playback will start at the beginning.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerStart(ulong mediaPlayerHandle);

                /// <summary>
                /// Pause playback.  Calling pause() is a NOOP if
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerPause(ulong mediaPlayerHandle);

                /// <summary>
                /// Seek to specified time position.  Note that SeekTo is an async.  function and returns immediately.  Successful seek
                /// result has to be obtained either via the 'on_seek_complete' if already registered (see MLMediaPlayerSetEventCallbacksEx) or
                /// by polling for the flag 'MLMediaPlayerPollingStateFlag_HasSeekCompleted' when calling MLMediaPlayerPollStates().
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerSeekTo(ulong mediaPlayerHandle, int Msec, SeekMode Mode);

                /// <summary>
                /// Return the size of the video frame.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerGetVideoSize(ulong mediaPlayerHandle, out int width, out int height);

                /// <summary>
                /// Return media duration.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerGetDuration(ulong mediaPlayerHandle, out int durationMS);

                /// <summary>
                /// Return current buffering percentage.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerGetBufferingUpdate(ulong mediaPlayerHandle, IntPtr OutPercentage);

                /// <summary>
                /// Return last error received by internal
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerGetError(ulong mediaPlayerHandle, IntPtr OutResult);

                /// <summary>
                /// Return last subtitle event information.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerGetSubtitleEx(ulong mediaPlayerHandle, IntPtr OutSubtitleData);

                /// <summary>
                /// Release last subtitle event information.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerReleaseSubtitleEx(ulong mediaPlayerHandle);

                /// <summary>
                /// Set callback to get notified when a subtitle update is available along with its data.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerSetOnMediaSubtitleUpdateCallback(ulong mediaPlayerHandle, OnMediaSubtitleUpdateDelegate OnMediaSubtitleCallback, IntPtr Data);

                /// <summary>
                /// Return last metadata event information.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerGetMetadata(ulong mediaPlayerHandle, IntPtr OutTime, IntPtr OutSize, IntPtr OutBuffer);

                /// <summary>
                /// Set video scaling mode.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerSetVideoScalingMode(ulong mediaPlayerHandle, VideoScalingMode Mode);

                /// <summary>
                /// Set the volume on this player.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerSetVolume(ulong mediaPlayerHandle, float Volume);

                /// <summary>
                /// Poll the desired states from internal
                /// The "is_XXX"  states can be polled multiples times and the return value will
                /// be the same if internal state hasn't changed since last call.
                /// When polling the "has_XXX" states however, internal state is set to false,
                /// since they mean : has <state> happened/changed since I last polled <state>
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerPollStates(ulong mediaPlayerHandle, PollingStateFlags flags, out PollingStateFlags polledStates);

                /// <summary>
                /// Get the DRM info of the selected media (either audio or video) track. This function has to be called only after
                /// DataSource has been set and the MediaPlayer is completely prepared.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerGetTrackDRMInfo(ulong mediaPlayerHandle, ref MLMediaPlayerTrackInfo trackInfo, ref IntPtr drmInfo);

                /// <summary>
                /// Get WebVTT data represented by a timed text handle.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerGetWebVTTData(ulong mediaPlayerHandle, ulong TimedText, ref IntPtr webVTTData);

                /// <summary>
                /// Release DRM.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerReleaseDRM(ulong mediaPlayerHandle);

                /// <summary>
                /// Get default Buffering settings.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerGetDefaultBufferingSettings(ulong mediaPlayerHandle, IntPtr OutBufSettings);

                /// <summary>
                /// Get current Buffering settings.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerGetBufferingSettings(ulong mediaPlayerHandle, IntPtr OutBufSettings);

                /// <summary>
                /// Set Buffering settings.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerSetBufferingSettings(ulong mediaPlayerHandle, NativeBindings.MLMediaPlayerBufferingSettings BufSettings);

                /// <summary>
                /// Set callback to get invoked when a Timed Text update is available along with its data.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerSetOnMediaTimedTextUpdateCallback(ulong mediaPlayerHandle, OnTimedTextUpdateDelegate OnMediaTimedTextCallback, IntPtr Data);

                /// <summary>
                /// Retrieve metrics recorded on the MediaPlayer.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerGetMetrics(ulong mediaPlayerHandle, out MLMediaPlayerMetrics metrics);

                /// <summary>
                /// Select a track.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerSelectTrack(ulong mediaPlayerHandle, uint trackIndex);

                /// <summary>
                /// Unselect a track.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerUnselectTrack(ulong mediaPlayerHandle, uint trackIndex);

                /// <summary>
                /// Count the number of tracks found in the data source.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerGetTrackCount(ulong mediaPlayerHandle, out uint trackCount);


                /// <summary>
                /// Get TTML data represented by a timed text handle.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerGetTTMLData(ulong mediaPlayerHandle, ulong timedTextHandle, ref IntPtr ttmlData);

                /// <summary>
                /// Sets the Native surface to be used as the sink for displaying the video portion of the media.
                /// </summary>
                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaPlayerSetSurface(ulong mediaPlayerHandle, ulong surface);

                /// <summary>
                /// Callback for the OnVideoSizeChanged event.
                /// </summary>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnVideoSizeChangedDelegate))]
                private static void OnVideoSizeChanged(ulong mediaPlayerHandle, ref NativeBindings.MLMediaPlayerOnVideoSizeChangedInfo videoSizeInfo)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(videoSizeInfo.Data);
                    Player mediaPlayer = gcHandle.Target as Player;
                    var resultCode = NativeBindings.MLMediaPlayerGetVideoSize(mediaPlayer.handle, out int width, out int height);
                    MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerGetVideoSize));
                    MLThreadDispatch.Call(mediaPlayer, width, height, mediaPlayer.OnVideoSizeChanged);
                }

                /// <summary>
                /// Callback for the OnFramePacking event.
                /// </summary>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnFramePackingDelegate))]
                private static void OnFramePacking(ulong mediaPlayerHandle, IntPtr FramePackingInfo)
                {
                    throw new System.NotImplementedException();
                }

                /// <summary>
                /// Callback for the OnInfo event.
                /// </summary>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnInfoDelegate))]
                private static void OnInfo(ulong mediaPlayerHandle, ref NativeBindings.MLMediaPlayerOnInfoInfo info)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(info.Data);
                    Player mediaPlayer = gcHandle.Target as Player;
                    if (info.Info == Info.MetadataUpdate)
                    {
                        MLThreadDispatch.Call(mediaPlayer.GetTracks);
                    }

                    MLThreadDispatch.Call(mediaPlayer, info.Info, mediaPlayer.OnInfo);
                }

                /// <summary>
                /// Callback for the OnPrepared event.
                /// </summary>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnPreparedDelegate))]
                private static void OnPrepared(ulong mediaPlayerHandle, ref NativeBindings.MLMediaPlayerOnPreparedInfo preparedInfo)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(preparedInfo.Data);
                    Player mediaPlayer = gcHandle.Target as Player;
                    mediaPlayer.IsPrepared = true;

                    MLThreadDispatch.Call(mediaPlayer.GetTracks);
                    MLThreadDispatch.Call(mediaPlayer, mediaPlayer.OnPrepared);
                    foreach (MLMedia.Player.Track.DRM.Info info in mediaPlayer.drmTrackInfos)
                        MLThreadDispatch.Call(mediaPlayer, info, mediaPlayer.OnDRMTrackInfo);
                }

                /// <summary>
                /// Callback for the OnBufferingUpdate event.
                /// </summary>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnBufferingUpdateDelegate))]
                private static void OnBufferingUpdate(ulong mediaPlayerHandle, ref NativeBindings.MLMediaPlayerOnBufferingUpdateInfo updateInfo)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(updateInfo.Data);
                    Player mediaPlayer = gcHandle.Target as Player;
                    float percent = updateInfo.Percent;
                    MLThreadDispatch.Call(mediaPlayer, percent, mediaPlayer.OnBufferingUpdate);
                }

                /// <summary>
                /// Callback for the OnCompletion event.
                /// </summary>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnCompletionDelegate))]
                private static void OnCompletion(ulong mediaPlayerHandle, ref NativeBindings.MLMediaPlayerOnCompletionInfo completionInfo)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(completionInfo.Data);
                    Player mediaPlayer = gcHandle.Target as Player;
                    MLThreadDispatch.Call(mediaPlayer, mediaPlayer.OnCompletion);
                }

                /// <summary>
                /// Callback for the OnError event.
                /// </summary>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnErrorDelegate))]
                private static void OnError(ulong mediaPlayerHandle, ref NativeBindings.MLMediaPlayerOnErrorInfo errorInfo)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(errorInfo.Data);
                    Player mediaPlayer = gcHandle.Target as Player;
                    MLThreadDispatch.Call(mediaPlayer, errorInfo.Result, mediaPlayer.OnError);
                }

                /// <summary>
                /// Callback for the OnSeekComplete event.
                /// </summary>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnSeekCompleteDelegate))]
                private static void OnSeekComplete(ulong mediaPlayerHandle, ref NativeBindings.MLMediaPlayerOnSeekCompleteInfo seekCompleteInfo)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(seekCompleteInfo.Data);
                    Player mediaPlayer = gcHandle.Target as Player;
                    MLThreadDispatch.Call(mediaPlayer, mediaPlayer.OnSeekComplete);
                }

                /// <summary>
                /// Callback for the OnTrackDRMInfo event. This happens before media player is fully prepared.
                /// </summary>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnTrackDRMInfoDelegate))]
                private static void OnTrackDRMInfo(ulong mediaPlayerHandle, ref NativeBindings.MLMediaPlayerOnTrackDRMInfo trackDrmInfo)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(trackDrmInfo.Data);
                    Player mediaPlayer = gcHandle.Target as Player;
                    MLMediaPlayerTrackDRMInfo trackInfo = Marshal.PtrToStructure<MLMediaPlayerTrackDRMInfo>(trackDrmInfo.TrackInfo);
                    mediaPlayer.drmTrackInfos.Add(trackInfo.Data);
                }

                /// <summary>
                /// Callback for the OnResetComplete event.
                /// </summary>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnResetCompleteDelegate))]
                private static void OnResetComplete(ulong mediaPlayerHandle, ref NativeBindings.MLMediaPlayerOnResetCompleteInfo resetCompleteInfo)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(resetCompleteInfo.Data);
                    Player mediaPlayer = gcHandle.Target as Player;
                    MLThreadDispatch.Call(mediaPlayer, mediaPlayer.OnResetComplete);
                }

                /// <summary>
                /// Callback for the OnTimedTextUpdate event.
                /// </summary>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnTimedTextUpdateDelegate))]
                public static void OnTimedTextUpdate(ulong mediaPlayerHandle, ulong timedTextHandle, IntPtr data)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(data);
                    Player mediaPlayer = gcHandle.Target as Player;
                    Track selectedTrack = mediaPlayer.trackContainers[Track.Type.TimedText].SelectedTrack;

                    switch (selectedTrack.MimeType)
                    {
                        case VTTMime:
                            {
                                var resultCode = NativeBindings.MLMediaPlayerGetWebVTTData(mediaPlayerHandle, timedTextHandle, ref mediaPlayer.WebVTTDataPtr);
                                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerGetWebVTTData));
                                WebVTTData webVTTData = Marshal.PtrToStructure<WebVTTData>(mediaPlayer.WebVTTDataPtr);
                                MLThreadDispatch.Call(mediaPlayer, webVTTData.Body, mediaPlayer.OnTimedText);
                                break;
                            }

                        case TTMLMime:
                            {
                                var resultCode = NativeBindings.MLMediaPlayerGetTTMLData(mediaPlayerHandle, timedTextHandle, ref mediaPlayer.TTMLDataPtr);
                                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerGetTTMLData));
                                TTMLData ttmlData = Marshal.PtrToStructure<TTMLData>(mediaPlayer.TTMLDataPtr);
                                MLThreadDispatch.Call(mediaPlayer, ttmlData.text, mediaPlayer.OnTimedText);
                                break;
                            }
                    }
                }

                /// <summary>
                /// Callback for the OnMediaSubtitleUpdate event.
                /// </summary>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnMediaSubtitleUpdateDelegate))]
                public static void OnMediaSubtitleUpdate(ulong mediaPlayerHandle, MLMediaPlayerSubtitleData subtitleData, IntPtr data)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(data);
                    Player mediaPlayer = gcHandle.Target as Player;
                    Track selectedTrack = mediaPlayer.trackContainers[Track.Type.Subtitle].SelectedTrack;

                    // TODO : improve this. We can probably determine the available subtitle track formats once the media is prepared
                    // and we receive the respective MLMediaFormat objects and only initialize the apt parser.
                    switch (selectedTrack.MimeType)
                    {
                        case ParserCEA608.Mime:
                            {
                                mediaPlayer.parser608.ParseAsync(subtitleData.Data, subtitleData.DataSize);
                                break;
                            }
                        case ParserCEA708.Mime:
                            {
                                mediaPlayer.parser708.ParseAsync(subtitleData.Data, subtitleData.DataSize);
                                break;
                            }
                    }
                }
            }
        }

    }
}
