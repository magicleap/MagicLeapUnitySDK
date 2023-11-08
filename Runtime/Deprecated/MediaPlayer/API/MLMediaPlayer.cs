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
using System.IO;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Networking;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// MLMedia APIs.
    /// </summary>
    [Obsolete("MLMedia API is being deprecated and will be removed from the MagicLeap SDK. Use Unity's VideoPlayer component (requires Unity 2022.3.10) instead.")]
    public partial class MLMedia
    {
        /// <summary>
        /// Media player script that allows playback of a streaming video (either from file or web URL)
        /// This script will update the main texture parameter of the Renderer attached as a sibling
        /// with the video frame from playback. Audio is also handled through this class and will
        /// playback audio from the file.
        /// </summary>
        public partial class Player : IMLMediaPlayer
        {
            // TODO : find a better place for these
            public const string VTTMime = "text/vtt";
            public const string TTMLMime = "application/ttml+xml";

            private IntPtr WebVTTDataPtr;
            private IntPtr TTMLDataPtr;

            /// <summary>
            /// Handle for the unmanaged media player object.
            /// </summary>
            private ulong handle = MagicLeapNativeBindings.InvalidHandle;

            /// <summary>
            /// Handle for spatialized audio.
            /// </summary>
            private ulong audioHandle = MagicLeapNativeBindings.InvalidHandle;

            /// <summary>
            /// Handle for the managed media player object to pass to and from unmanaged code.
            /// </summary>
            private GCHandle gcHandle;

            /// <summary>
            /// Indicates if the media player was active before entering standby or pause.
            /// </summary>
            private bool wasPlayingBeforePause = false;

            /// <summary>
            /// Parser object for 608 captions.
            /// </summary>
            private ParserCEA608 parser608;

            /// <summary>
            /// Parser object for 708 captions.
            /// </summary>
            private ParserCEA708 parser708;

            /// <summary>
            /// Indicates if the media player is currently playing.
            /// </summary>
            public bool IsPlaying
            {
                get { return GetState(PollingStateFlags.IsPlaying); }
            }

            /// <summary>
            /// Indicates if the media player has been prepared.
            /// </summary>
            public bool IsPrepared
            {
                get;
                private set;
            }

            /// <summary>
            /// The uri/path that the media player is currently set with.
            /// </summary>
            public string Source
            {
                get;
                private set;
            }

            /// <summary>
            /// List of info objects for the different tracks that need DRM preparation.
            /// </summary>
            private List<MLMedia.Player.Track.DRM.Info> drmTrackInfos = new List<Track.DRM.Info>();

            /// <summary>
            /// List of all tracks found since media player preparation.
            /// </summary>
            private List<Track> allTracks = new List<Track>();

            /// <summary>
            /// List of all containers for managing the different track types and which one is currently selected by the media player.
            /// </summary>
            private Dictionary<MLMedia.Player.Track.Type, TracksContainer> trackContainers;

            /// <summary>
            /// Media player video renderer
            /// </summary>
            public MLNativeSurfaceYcbcrRenderer VideoRenderer
            {
                get;
                private set;
            }

            /// <summary>
            /// Constructor for the media player object.
            /// </summary>
            /// <param name="result">The native result for creating the media player.</param>
            /// <returns>An initialized media player object.</returns>
            public Player(out MLResult result)
            {
#if UNITY_EDITOR
                // media player not supported under Magic Leap App Simulator
                MLResult.Code resultCode = MLResult.Code.NotImplemented;
#else 
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerCreate(out this.handle);
#endif
                if (!MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerCreate)))
                {
                    result = MLResult.Create(resultCode);
                    return;
                }

                WebVTTDataPtr = Marshal.AllocHGlobal(Marshal.SizeOf<NativeBindings.WebVTTData>());
                TTMLDataPtr = Marshal.AllocHGlobal(Marshal.SizeOf<NativeBindings.TTMLData>());

                MLDevice.RegisterApplicationPause(this.OnApplicationPause);

                NativeBindings.MLMediaPlayerEventCallbacksEx callbacks = NativeBindings.MLMediaPlayerEventCallbacksEx.Create();
                this.gcHandle = GCHandle.Alloc(this, GCHandleType.Weak);
                IntPtr gcHandlePtr = GCHandle.ToIntPtr(this.gcHandle);
                resultCode = NativeBindings.MLMediaPlayerSetEventCallbacksEx(this.handle, ref callbacks, gcHandlePtr);
                if (!MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerSetEventCallbacksEx)))
                {
                    result = MLResult.Create(resultCode);
                    return;
                }

                this.parser608 = new ParserCEA608();
                this.parser608.OnText += On608Text;

                this.parser708 = new ParserCEA708();
                this.parser708.OnText += On708Text;

                // register to the captionings callbacks
                resultCode = NativeBindings.MLMediaPlayerSetOnMediaTimedTextUpdateCallback(this.handle, NativeBindings.OnTimedTextUpdate, gcHandlePtr);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerSetOnMediaTimedTextUpdateCallback));

                NativeBindings.MLMediaPlayerSetOnMediaSubtitleUpdateCallback(this.handle, NativeBindings.OnMediaSubtitleUpdate, gcHandlePtr);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerSetOnMediaSubtitleUpdateCallback));

                // timedtext and subtitles containers must be shared
                TracksContainer captioningTracksContainer = new TracksContainer();
                this.trackContainers = new Dictionary<Track.Type, TracksContainer>()
                                    {
                                        { Track.Type.Video, new TracksContainer() },
                                        { Track.Type.Audio, new TracksContainer() },
                                        { Track.Type.TimedText, captioningTracksContainer },
                                        { Track.Type.Subtitle, captioningTracksContainer },
                                        { Track.Type.Metadata, new TracksContainer() }
                                    };

                result = MLResult.Create(MLResult.Code.Ok);
            }

            /// <summary>
            /// Finalizer for the media player object.
            /// </summary>
            ~Player()
            {
                Marshal.FreeHGlobal(WebVTTDataPtr);
                Marshal.FreeHGlobal(TTMLDataPtr);

                Destroy();
            }

            public void Destroy()
            {
                if (handle == MagicLeapNativeBindings.InvalidHandle)
                    return;

                MLDevice.UnregisterApplicationPause(this.OnApplicationPause);

                this.parser608.OnText -= On608Text;
                this.parser708.OnText -= On708Text;

                var resultCode = NativeBindings.MLMediaPlayerSetOnMediaSubtitleUpdateCallback(this.handle, null, IntPtr.Zero);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerSetOnMediaSubtitleUpdateCallback));

                resultCode = NativeBindings.MLMediaPlayerSetOnMediaTimedTextUpdateCallback(this.handle, null, IntPtr.Zero);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerSetOnMediaTimedTextUpdateCallback));

                if (VideoRenderer != null)
                {
                    this.VideoRenderer.Cleanup();
                }

                AudioDRM = null;
                VideoDRM = null;

                NativeBindings.MLUnityQueueMediaPlayerResetAndDestroy(this.handle);
                handle = MagicLeapNativeBindings.InvalidHandle;

                this.gcHandle.Free();
            }

            public MLResult Play()
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerStart(this.handle);
                bool started = MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerStart));

                if (started)
                    MLThreadDispatch.Call(this, this.OnPlay);

                return MLResult.Create(resultCode);
            }

            public MLResult Pause()
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerPause(this.handle);
                bool paused = MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerPause));
                if (paused)
                    MLThreadDispatch.Call(this, this.OnPause);
                return MLResult.Create(resultCode);
            }

            public MLResult Resume() => this.Play();

            public MLResult PollStates(out PollingStateFlags states)
            {
                // Construct flag data.
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerPollStates(this.handle, PollingStateFlags.All, out states);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerPollStates));
                return MLResult.Create(resultCode);
            }

            public MLResult PollState(PollingStateFlags stateFlags, out PollingStateFlags state)
            {
                // Construct flag data.
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerPollStates(this.handle, stateFlags, out state);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerPollStates));
                return MLResult.Create(resultCode);
            }

            public bool GetState(PollingStateFlags stateFlags)
            {
                return (PollState(stateFlags, out var state).IsOk && state.HasFlag(stateFlags));
            }

            public MLResult Seek(int seekMS, SeekMode seekMode)
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerSeekTo(this.handle, seekMS, SeekMode.ClosestSync);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerSeekTo));
                return MLResult.Create(resultCode);
            }

            public void UnlinkDataSource()
            {
                streamingAssetDownloadHandler?.Dispose();
                streamingAssetDownloadHandler = null;
                dataSource?.Destroy();
                dataSource = null;
            }

            public MLResult Stop()
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerStop(this.handle);
                bool stopped = MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerStop));
                if (stopped)
                    MLThreadDispatch.Call(this, this.OnStop);
                return MLResult.Create(resultCode);
            }

            public MLResult SetLooping(bool loop)
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerSetLooping(this.handle, loop);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerSetLooping));
                return MLResult.Create(resultCode);
            }

            public MLResult SetVolume(float volume)
            {
                volume = Mathf.Clamp01(volume);
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerSetVolume(this.handle, volume);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerSetVolume));
                return MLResult.Create(resultCode);
            }

            public MLResult GetDurationMS(out int duration)
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerGetDuration(this.handle, out duration);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerGetDuration));
                return MLResult.Create(resultCode);
            }

            public MLResult GetPositionMS(out int position)
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerGetCurrentPosition(this.handle, out position);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerGetCurrentPosition));
                return MLResult.Create(resultCode);
            }

            public MLResult GetVideoSize(out int width, out int height)
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerGetVideoSize(this.handle, out width, out height);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerGetVideoSize));
                return MLResult.Create(resultCode);
            }

            public MLResult SetSpatialAudioEnable(bool enable)
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerGetAudioHandle(this.handle, out this.audioHandle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerGetAudioHandle));
                resultCode = MLAudioOutput.NativeBindings.MLAudioSetSpatialSoundEnable(this.audioHandle, enable);
                MLResult.DidNativeCallSucceed(resultCode, nameof(MLAudioOutput.NativeBindings.MLAudioSetSpatialSoundEnable));
                return MLResult.Create(resultCode);
            }

            public MLResult GetSpatialAudioEnable(out bool enabled)
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerGetAudioHandle(this.handle, out this.audioHandle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerGetAudioHandle));
                resultCode = MLAudioOutput.NativeBindings.MLAudioGetSpatialSoundEnable(this.audioHandle, out enabled);
                MLResult.DidNativeCallSucceed(resultCode, nameof(MLAudioOutput.NativeBindings.MLAudioGetSpatialSoundEnable));
                return MLResult.Create(resultCode);
            }

            public MLResult SetSpatialAudioChannelPosition(uint channel, Vector3 position)
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerGetAudioHandle(this.handle, out this.audioHandle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerGetAudioHandle));
                MagicLeapNativeBindings.MLVec3f mlPosition = MLConvert.FromUnity(position);
                resultCode = MLAudioOutput.NativeBindings.MLAudioSetSpatialSoundPosition(this.handle, channel, ref mlPosition);
                MLResult.DidNativeCallSucceed(resultCode, nameof(MLAudioOutput.NativeBindings.MLAudioSetSpatialSoundPosition));
                return MLResult.Create(resultCode);
            }

            public MLResult GetSpatialAudioChannelPosition(uint channel, out Vector3 position)
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerGetAudioHandle(this.handle, out this.audioHandle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerGetAudioHandle));
                resultCode = MLAudioOutput.NativeBindings.MLAudioGetSpatialSoundPosition(this.handle, channel, out MagicLeapNativeBindings.MLVec3f mlPosition);
                MLResult.DidNativeCallSucceed(resultCode, nameof(MLAudioOutput.NativeBindings.MLAudioGetSpatialSoundPosition));
                position = MLConvert.ToUnity(mlPosition);
                return MLResult.Create(resultCode);
            }

            public MLResult SetSourceURI(string uri)
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerSetDataSourceForURI(this.handle, uri);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerSetDataSourceForURI));
                this.Source = MLResult.IsOK(resultCode) ? uri : string.Empty;
                this.IsPrepared = false;

                return MLResult.Create(resultCode);
            }

            public MLResult SetSourcePath(string path)
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerSetDataSourceForPath(this.handle, path);
                var nativeMethodName = nameof(NativeBindings.MLMediaPlayerSetDataSourceForPath);
                MLResult.DidNativeCallSucceed(resultCode, $"{nativeMethodName} with parameter: {path}");
                this.Source = MLResult.IsOK(resultCode) ? path : string.Empty;
                this.IsPrepared = false;
                return MLResult.Create(resultCode);
            }

            private UnityWebRequest streamingAssetWebRequest;
            private UnityWebRequestAsyncOperation streamingAssetWebRequestAsyncOp;
            private DownloadHandlerBuffer streamingAssetDownloadHandler;
            private DataSource dataSource;
            public MLResult SetStreamingSourcePath(string path)
            {
                try
                {
                    streamingAssetWebRequest = new UnityWebRequest(path, "GET", new DownloadHandlerBuffer(), null);
                    if (streamingAssetWebRequest == null)
                    {
                        Debug.LogError($"Failed to create web request for {path}");
                        return MLResult.Create(MLResult.Code.AllocFailed);
                    }

                    streamingAssetWebRequestAsyncOp = streamingAssetWebRequest.SendWebRequest();
                    streamingAssetWebRequestAsyncOp.completed += (AsyncOperation asyncOp) =>
                    {
                        UnityWebRequestAsyncOperation webRequestAsyncOp = asyncOp as UnityWebRequestAsyncOperation;
                        if (webRequestAsyncOp == null || webRequestAsyncOp.webRequest.result != UnityWebRequest.Result.Success)
                        {
                            Debug.LogError($"MLMediaPlayer.SetSourcePath({webRequestAsyncOp?.webRequest.url}) failed, " +
                                           $"Reason : {webRequestAsyncOp?.webRequest.error}");
                            return;
                        }

                        streamingAssetDownloadHandler = webRequestAsyncOp.webRequest.downloadHandler as DownloadHandlerBuffer;
                        dataSource = new DataSource(streamingAssetDownloadHandler.data);
                        var result = NativeBindings.MLMediaPlayerSetMediaDataSource(this.handle, dataSource.Handle);
                        MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLMediaPlayerSetMediaDataSource));

                        if (result != MLResult.Code.Ok)
                        {
                            Debug.LogError($"Failed to create dataSource for {path}");
                            return;
                        }
                        this.Source = path;
                        PreparePlayerAsync();
                    };
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return MLResult.Create(MLResult.Code.AllocFailed);
                }

                return MLResult.Create(MLResult.Code.Pending);
            }

            public MLResult PreparePlayer()
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerPrepare(this.handle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerPrepare));
                this.IsPrepared = true;
                ClearTrackContainers();
                return MLResult.Create(resultCode);
            }

            public MLResult PreparePlayerAsync()
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerPrepareAsync(this.handle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerPrepareAsync));
                ClearTrackContainers();
                return MLResult.Create(resultCode);
            }

            private void PostReset()
            {
                IsPrepared = false;
                VideoRenderer?.Cleanup();
                UnlinkDataSource();
            }

            public MLResult Reset()
            {
                var resultCode = NativeBindings.MLMediaPlayerReset(handle);
                if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerReset)))
                {
                    PostReset();
                }
                return MLResult.Create(resultCode);
            }

            public MLResult ResetAsync()
            {
                var resultCode = NativeBindings.MLMediaPlayerResetAsync(this.handle);
                if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerResetAsync)))
                {
                    PostReset();
                }
                return MLResult.Create(resultCode);
            }

            public int GetPositionMilliseconds()
            {
                NativeBindings.MLMediaPlayerGetCurrentPosition(this.handle, out int ms);
                return ms;
            }

            public int GetWidth()
            {
                this.GetVideoSize(out int width, out int height);
                return width;
            }

            public int GetHeight()
            {
                this.GetVideoSize(out int width, out int height);
                return height;
            }

            public int GetVideoBitrate()
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerGetMetrics(this.handle, out NativeBindings.MLMediaPlayerMetrics metrics);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerGetMetrics));
                return metrics.AverageVideoBitrate;
            }

            public ulong GetFrameDropThresholdMs()
            {
                return 0;
            }

            public void SetFrameDropThresholdMs(ulong threshold)
            {

            }

            public int GetDurationMilliseconds()
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerGetDuration(this.handle, out int duration);
                return duration;
            }

            private void OnApplicationPause(bool paused)
            {
                if (paused)
                {
                    this.wasPlayingBeforePause = this.IsPlaying;
                    if (this.IsPlaying)
                    {
                        this.Pause();
                    }
                }
                else
                {
                    if (this.wasPlayingBeforePause)
                    {
                        this.Play();
                    }
                }
            }

            public class TracksContainer
            {
                internal List<Track> Tracks = new List<Track>();
                internal Track SelectedTrack;
            }

            public void PollForMetaData()
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerPollStates(this.handle, PollingStateFlags.HasMetadataUpdated, out PollingStateFlags polledStates);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerPollStates));
                if (polledStates.HasFlag(PollingStateFlags.HasMetadataUpdated))
                {
                    GetTracks();
                }
            }

            public MLResult SelectTrack(int index)
            {
                if (index >= allTracks.Count)
                    return MLResult.Create(MLResult.Code.InvalidParam);

                return SelectTrack(allTracks[index]);
            }

            public MLResult SelectTrack(Track track)
            {
                TracksContainer container = this.trackContainers[track.TrackType];
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerSelectTrack(this.handle, track.Index);
                if (!MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerSelectTrack)))
                {
                    return MLResult.Create(resultCode);
                }

                container.SelectedTrack = track;
                OnTrackSelected(this, track);

                return MLResult.Create(resultCode);
            }

            public MLResult UnselectTrack(Track track)
            {
                TracksContainer container = this.trackContainers[track.TrackType];
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerUnselectTrack(this.handle, track.Index);
                if (!MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerUnselectTrack)))
                {
                    return MLResult.Create(resultCode);
                }

                container.SelectedTrack = null;
                OnTrackSelected(this, track);

                return MLResult.Create(resultCode);
            }


            public void CreateVideoRenderer(uint width, uint height)
            {
                this.VideoRenderer?.Cleanup();
                this.VideoRenderer = new MLNativeSurfaceYcbcrRenderer(width, height);
                // TODO : see if we need to add checks to confirm if source has been set.
                // Technically it would be, becayse we get width and height only after that.
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerSetSurface(this.handle, VideoRenderer.Surface.Handle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerSetSurface));
            }

            public Track.DRM AudioDRM { get; private set; }
            public Track.DRM VideoDRM { get; private set; }

            // How to determine which track is selected at the beginning?
            private void GetTracks()
            {
                MLResult.Code resultCode = NativeBindings.MLMediaPlayerGetTrackCount(this.handle, out uint trackCount);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaPlayerGetTrackCount));

                // Per the Media team as new tracks come in the old tracks are still valid so we start processing after the current last track
                for (int i = allTracks.Count; i < trackCount; ++i)
                {
                    Track track = new Track(this.handle, (uint)i);
                    allTracks.Add(track);

                    TracksContainer container = this.trackContainers[track.TrackType];
                    container.Tracks.Add(track);

                    OnTrackFound(this, track);

                    if (container.Tracks.Count == 1)
                    {
                        container.SelectedTrack = track;
                        OnTrackSelected(this, track);
                    }
                }
            }

            private void On608Text(string text)
            {
                OnCEA608(this, text);
            }

            private void On708Text(string text)
            {
                OnCEA708(this, text);
            }

            private void ClearTrackContainers()
            {
                foreach (TracksContainer container in this.trackContainers.Values)
                {
                    container.Tracks.Clear();
                    container.SelectedTrack = null;
                }

                this.allTracks.Clear();
                this.drmTrackInfos.Clear();
            }
        }
    }
}

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLMedia
    {
        public partial class DataSource
        {
            public ulong Handle { get; private set; }

            private GCHandle gcHandle;
            private NativeArray<byte>.ReadOnly fileBuffer;
            private byte[] byteBuffer;

            public DataSource(NativeArray<byte>.ReadOnly fileBuffer)
            {
                this.gcHandle = GCHandle.Alloc(this, GCHandleType.Weak);
                this.fileBuffer = fileBuffer;
                this.byteBuffer = null;

                // media player not supported in Magic Leap App Simulator
#if !UNITY_EDITOR
                MLResult.Code result = NativeBindings.Create(this, out ulong handle);
                if (MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLMediaDataSourceCreate)))
                {
                    Handle = handle;
                }
#endif
            }

            public DataSource(byte[] byteBuffer)
            {
                this.gcHandle = GCHandle.Alloc(this, GCHandleType.Weak);
                this.byteBuffer = byteBuffer;

                // media player not supported in Magic Leap App Simulator
#if !UNITY_EDITOR
                MLResult.Code result = NativeBindings.Create(this, out ulong handle);
                if (MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLMediaDataSourceCreate)))
                {
                    Handle = handle;
                }
#endif
            }

            public void Destroy()
            {
                if (MagicLeapNativeBindings.MLHandleIsValid(Handle))
                {
                    var resultCode = NativeBindings.MLMediaDataSourceDestroy(Handle);
                    MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDataSourceDestroy));
                    Handle = MagicLeapNativeBindings.InvalidHandle;
                }
            }

            ~DataSource()
            {
                if (MagicLeapNativeBindings.MLHandleIsValid(Handle))
                {
                    var resultCode = NativeBindings.MLMediaDataSourceDestroy(Handle);
                    MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDataSourceDestroy));
                    Handle = MagicLeapNativeBindings.InvalidHandle;
                }
            }
        }
    }
}

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLMedia
    {
        public partial class DataSource
        {
            public class NativeBindings : MagicLeapNativeBindings
            {
                public delegate long ReadAtDelegate(ulong dataSourceHandle, ulong position, ulong size, IntPtr buffer, IntPtr context);
                public delegate long GetSizeDelegate(ulong dataSourceHandle, IntPtr context);
                public delegate void CloseDelegate(ulong dataSourceHandle, IntPtr context);

                [AOT.MonoPInvokeCallback(typeof(ReadAtDelegate))]
                unsafe private static long OnReadAt(ulong dataSourceHandle, ulong position, ulong size, IntPtr buffer, IntPtr context)
                {
                    // Debug.Log($"OnReadAt({position}, {size})");

                    try
                    {
                        GCHandle gcHandle = GCHandle.FromIntPtr(context);
                        DataSource dataSource = gcHandle.Target as DataSource;

                        if (dataSource.byteBuffer != null)
                        {
                            Marshal.Copy(dataSource.byteBuffer, (int)position, buffer, (int)size);
                        }
                        else
                        {
                            NativeArray<byte> externalBuffer = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(buffer.ToPointer(), (int)size, Allocator.None);
                            NativeArray<byte>.Copy(dataSource.fileBuffer, (int)position, externalBuffer, 0, (int)size);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        Debug.LogError(e.Message);
                        return -1;
                    }

                    return (long)size;
                }

                [AOT.MonoPInvokeCallback(typeof(GetSizeDelegate))]
                private static long OnGetSize(ulong dataSourceHandle, IntPtr context)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(context);
                    DataSource dataSource = gcHandle.Target as DataSource;

                    return dataSource.byteBuffer != null ? dataSource.byteBuffer.LongLength : dataSource.fileBuffer.Length;
                }

                [AOT.MonoPInvokeCallback(typeof(CloseDelegate))]
                private static void OnClose(ulong dataSourceHandle, IntPtr context)
                {
                    Debug.Log($"OnClose()");
                    GCHandle gcHandle = GCHandle.FromIntPtr(context);
                    DataSource dataSource = gcHandle.Target as DataSource;

                    // TODO : we can invoke some delegate here which can bubble up to
                    // free the NativeArray<byte> held by the download handler.
                }

                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaDataSourceCreate(ReadAtDelegate readAt, GetSizeDelegate getSize, CloseDelegate close, IntPtr context, out ulong handle);

                [DllImport(MLMediaPlayerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaDataSourceDestroy(ulong handle);

                public static MLResult.Code Create(DataSource dataSource, out ulong handle)
                {
                    return MLMediaDataSourceCreate(OnReadAt, OnGetSize, OnClose, GCHandle.ToIntPtr(dataSource.gcHandle), out handle);
                }
            }
        }
    }
}
