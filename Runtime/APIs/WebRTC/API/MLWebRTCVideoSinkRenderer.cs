// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCVideoSink.cs" company="Magic Leap, Inc">
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
    using System;
    using System.Threading;
    using System.Runtime.InteropServices;
    using UnityEngine;
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
                    Initialize(ColorSpace.Linear);
                    this.videoSink = videoSink;
                }

                bool INativeBufferProvider.AcquireNextAvailableBuffer(out ulong nativeBufferHandle)
                {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                    nativeBufferHandle = MagicLeapNativeBindings.InvalidHandle;
                    bool result = videoSink.AcquireNextAvailableFrame(out latestFrame);
                    if (result)
                    {
                        nativeBufferHandle = latestFrame.NativeFrame.NativeBufferHandle;
                    }
                    return result;
#else
                    nativeBufferHandle = 0;
                    return false;
#endif
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
