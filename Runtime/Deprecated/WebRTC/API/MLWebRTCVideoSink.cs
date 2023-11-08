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
    using System.Collections.Generic;
    using System.Threading;
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Class that represents a video sink used by the MLWebRTC API.
        /// Video sinks are fed data by media sources and produces frames to render.
        /// </summary>
        public partial class VideoSink : Sink
        {
            /// <summary>
            /// Buffer for the image planes array to use to hold the image plane data.
            /// </summary>
            private CircularBuffer<Frame.PlaneInfo[]> imagePlanesBuffer = CircularBuffer<Frame.PlaneInfo[]>.Create(new Frame.PlaneInfo[Frame.PlaneInfo.MaxImagePlanes], new Frame.PlaneInfo[Frame.PlaneInfo.MaxImagePlanes], new Frame.PlaneInfo[Frame.PlaneInfo.MaxImagePlanes]);

            /// <summary>
            /// The newest frame handle that the video sink knows of.
            /// </summary>
            private ulong newFrameHandle;

            private uint streamWidth = 0;
            private uint streamHeight = 0;

            private AutoResetEvent updateVideoEvent = new AutoResetEvent(true);

            public delegate void OnFrameResolutionChangedDelegate(uint newWidth, uint newHeight);
            public delegate void OnDestroySinkDelegate(VideoSink videoSink);
            public delegate void OnStreamChangedDelegate(MediaStream stream);

            public event OnDestroySinkDelegate OnDestroySink = delegate { };
            public event OnFrameResolutionChangedDelegate OnFrameResolutionChanged = delegate { };
            public event OnStreamChangedDelegate OnStreamChanged;

            /// <summary>
            /// Initializes a new instance of the <see cref="VideoSink" /> class.
            /// </summary>
            internal VideoSink()
            {
                this.Type = MediaStream.Track.Type.Video;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="VideoSink" /> class.
            /// </summary>
            /// <param name="Handle">The Handle of the video sink.</param>
            internal VideoSink(ulong Handle) : base(Handle)
            {
                this.Type = MediaStream.Track.Type.Video;
            }

            /// <summary>
            /// Creates an initialized VideoSink object.
            /// </summary>
            /// <param name="result">The MLResult object of the inner platform call(s).</param>
            /// <returns> An initialized VideoSink object.</returns>
            public static VideoSink Create(out MLResult result)
            {
                VideoSink videoSink = null;

                List<MLWebRTC.Sink> sinks = MLWebRTC.Instance.sinks;
                ulong Handle = MagicLeapNativeBindings.InvalidHandle;
                MLResult.Code resultCode = NativeBindings.MLWebRTCVideoSinkCreate(out Handle);
                if (!MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCVideoSinkCreate)))
                {
                    result = MLResult.Create(resultCode);
                    return videoSink;
                }

                videoSink = new VideoSink(Handle);
                if (MagicLeapNativeBindings.MLHandleIsValid(videoSink.Handle))
                {
                    sinks.Add(videoSink);
                }

                result = MLResult.Create(resultCode);
                return videoSink;
            }

            public bool IsNewFrameAvailable()
            {
                MLResult.Code resultCode = NativeBindings.MLWebRTCVideoSinkIsNewFrameAvailable(this.Handle, out bool newFrameAvailable);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCVideoSinkIsNewFrameAvailable));
                return newFrameAvailable;
            }

            public bool AcquireNextAvailableFrame(out Frame newFrame)
            {
                newFrame = new Frame();

                ulong frameHandle = MagicLeapNativeBindings.InvalidHandle;
                MLResult.Code resultCode = NativeBindings.MLWebRTCVideoSinkAcquireNextAvailableFrame(this.Handle, out frameHandle);
                if (!MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCVideoSinkAcquireNextAvailableFrame)))
                {
                    return false;
                }

                Frame.NativeBindings.MLWebRTCFrame nativeFrame = Frame.NativeBindings.MLWebRTCFrame.Create(Frame.OutputFormat.YUV_420_888);
                resultCode = Frame.NativeBindings.MLWebRTCFrameGetData(frameHandle, ref nativeFrame);
                if (MLResult.DidNativeCallSucceed(resultCode, nameof(Frame.NativeBindings.MLWebRTCFrameGetData)))
                {
                    newFrameHandle = frameHandle;
                    newFrame = Frame.Create(frameHandle, nativeFrame, imagePlanesBuffer.Get());

                    if (streamWidth != newFrame.NativeFrame.Width || streamHeight != newFrame.NativeFrame.Height)
                    {
                        streamWidth = newFrame.NativeFrame.Width;
                        streamHeight = newFrame.NativeFrame.Height;
                        MLThreadDispatch.Call(newFrame.NativeFrame.Width, newFrame.NativeFrame.Height, OnFrameResolutionChanged);
                        Debug.LogWarning($"new frame acquired has new size: {streamWidth} x {streamHeight}");
                    }
                    return true;
                }

                return false;
            }

            public void ReleaseFrame()
            {
                if (MagicLeapNativeBindings.MLHandleIsValid(newFrameHandle))
                {
                    var resultCode = NativeBindings.MLWebRTCVideoSinkReleaseFrame(Handle, newFrameHandle);
                    MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCVideoSinkReleaseFrame));
                    newFrameHandle = MagicLeapNativeBindings.InvalidHandle;
                }
            }

            /// <summary>
            /// Sets the track of the video sink.
            /// </summary>
            /// <param name="track">The track to use.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            protected override MLResult SetTrack(MediaStream.Track track)
            {
                ulong sourceHandle = track != null ? track.Handle : MagicLeapNativeBindings.InvalidHandle;
                MLResult.Code resultCode = NativeBindings.MLWebRTCVideoSinkSetSource(this.Handle, sourceHandle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCVideoSinkSetSource));
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Sets the stream of the video sink sink.
            /// </summary>
            /// <param name="stream">The stream to use.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            public MLResult SetStream(MediaStream stream)
            {
                if (stream == Stream)
                {
                    return MLResult.Create(MLResult.Code.Ok);
                }

                Stream = stream;
                if (OnStreamChanged != null)
                {
                    OnStreamChanged(Stream);
                }

                if (Stream == null)
                {
                    return SetTrack(null);
                }

                return SetTrack(Stream.ActiveVideoTrack);
            }

            /// <summary>
            /// Destroys the video sink.
            /// </summary>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            public override MLResult Destroy()
            {
                if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Handle is invalid.");
                }

                OnDestroySink(this);

                updateVideoEvent.WaitOne(250);
                this.SetStream(null);

                // TODO : synchronize with renderer
                MLResult.Code resultCode = NativeBindings.MLWebRTCVideoSinkDestroy(this.Handle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCVideoSinkDestroy));
                this.InvalidateHandle();
                MLWebRTC.Instance.sinks.Remove(this);

                return MLResult.Create(resultCode);
            }
        }
    }
}
