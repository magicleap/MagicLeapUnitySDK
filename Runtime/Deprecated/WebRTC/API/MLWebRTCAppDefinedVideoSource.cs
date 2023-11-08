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
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine.XR.MagicLeap.Native;

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
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.InitializeAppDefinedVideoSource));
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

                MLResult.Code resultCode = MLResult.Code.UnspecifiedFailure;

                var frameNative = VideoSink.Frame.NativeBindings.MLWebRTCFrame.Create(frame);

                try
                {
                    resultCode = NativeBindings.MLWebRTCSourceAppDefinedVideoSourcePushFrame(Handle, in frameNative);

                    MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCSourceAppDefinedVideoSourcePushFrame));
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
                finally
                {
                    frameNative.FreeUnmanagedMemory();
                }

                pushFrameEvent.Set();

                return MLResult.Create(resultCode);
            }

            public override MLResult DestroyLocal()
            {
                pushFrameEvent.WaitOne(250);
                MLResult result = base.DestroyLocal();
                return result;
            }
        }
    }
}
