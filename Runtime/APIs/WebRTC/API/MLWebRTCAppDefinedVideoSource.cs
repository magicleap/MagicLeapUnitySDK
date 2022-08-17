// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCAppDefinedVideoSource.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Runtime.InteropServices;
#if UNITY_MAGICLEAP || UNITY_ANDROID
    using UnityEngine.XR.MagicLeap.Native;
#endif

    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Class that represents an app defined video source that can be used by the MLWebRTC API.
        /// </summary>
        public abstract partial class AppDefinedVideoSource : AppDefinedSource
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AppDefinedVideoSource" /> class.
            /// </summary>
            protected internal AppDefinedVideoSource(string trackId)
                : base(trackId)
            {
                this.TrackType = Type.Video;
            }

#if UNITY_MAGICLEAP || UNITY_ANDROID
            /// <summary>
            /// Initializes the given AppDefinedVideoSource object.
            /// </summary>
            /// <param name="appDefinedVideoSource">The AppDefinedVideoSource object to initialize.</param>
            /// <param name="result">The MLResult object of the inner platform call(s).</param>
            /// <returns>An AppDefinedVideoSource object with the given handle.</returns>
            public static MLResult InitializeLocal(AppDefinedVideoSource appDefinedVideoSource)
            {
                MLWebRTC.Instance.localTracks.Add(appDefinedVideoSource);
                MLResult.Code resultCode = NativeBindings.InitializeAppDefinedVideoSource(appDefinedVideoSource);
                MLResult.DidNativeCallSucceed(resultCode, "InitializeAppDefinedVideoSource()");
                return MLResult.Create(resultCode);
            }

            private AutoResetEvent pushFrameEvent = new AutoResetEvent(true);

            /// <summary>
            /// Pushes a frame into the defined video source.
            /// </summary>
            /// <param name="frame">The frame to push to the video source.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInvalidFrameFormat</c> if an invalid frame format was specified.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInvalidFramePlaneCount</c> if an invalid plane count was specified for the given frame format.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCSendNativeFrameError</c> pushing a native format frame fails.
            /// </returns>
            protected MLResult PushFrame(VideoSink.Frame frame)
            {
                if (!MagicLeapNativeBindings.MLHandleIsValid(Handle))
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Handle is invalid.");
                }

                pushFrameEvent.Reset();

                var frameNative = VideoSink.Frame.NativeBindings.MLWebRTCFrame.Create(frame);

                try
                {
                    var resultCode = NativeBindings.MLWebRTCSourceAppDefinedVideoSourcePushFrame(Handle, in frameNative);

                    MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCSourceAppDefinedVideoSourcePushFrame));

                    return MLResult.Create(resultCode);
                }
                catch(System.Exception e)
                {
                    Debug.LogException(e);
                }
                pushFrameEvent.Set();

                return MLResult.Create(MLResult.Code.InvalidParam);
            }

            public override MLResult DestroyLocal()
            {
                pushFrameEvent.WaitOne(250);
                MLResult result = base.DestroyLocal();
                return result;
            }
#endif
        }
    }
}
