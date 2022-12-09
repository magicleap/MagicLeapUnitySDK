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
    using System.Threading;
    using UnityEngine;
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
            public class Renderer : YcbcrRenderer, YcbcrRenderer.INativeBufferProvider, YcbcrRenderer.IFrameTransformMatrixProvider, YcbcrRenderer.IFrameAvailabilityProvider
            {
                private VideoSink videoSink;
                private Frame latestFrame;

                public Renderer(VideoSink videoSink)
                {
                    Initialize();
                    this.videoSink = videoSink;
                }

                bool INativeBufferProvider.AcquireNextAvailableBuffer(out ulong nativeBufferHandle)
                {
                    nativeBufferHandle = MagicLeapNativeBindings.InvalidHandle;
                    bool result = videoSink.AcquireNextAvailableFrame(out latestFrame);
                    if (result)
                    {
                        nativeBufferHandle = latestFrame.NativeFrame.NativeBufferHandle;
                    }
                    return result;
                }

                void INativeBufferProvider.ReleaseBuffer(ulong nativeBufferHandle)
                {
                    // we dont make use of the nativeBufferHandle here because
                    // in webrtc the video sink can only have 1 frame acquired
                    // at a time.
                    videoSink.ReleaseFrame();
                }

                bool IFrameTransformMatrixProvider.GetFrameTransformMatrix(float[] frameTransformMatColMajor)
                {
                    Array.Copy(latestFrame.NativeFrame.Transform, frameTransformMatColMajor, frameTransformMatColMajor.Length);
                    Native.MLConvert.FlipTransformMatrixVertically(frameTransformMatColMajor);
                    return true;
                }

                bool IFrameAvailabilityProvider.IsNewFrameAvailable()
                {
                    return videoSink.IsNewFrameAvailable();
                }
            }
        }
    }
}
