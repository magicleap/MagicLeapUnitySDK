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
        public partial class VideoSink
        {
            /// <summary>
            /// Native bindings for the MLWebRTC.VideoSink class. 
            /// </summary>
            internal class NativeBindings : MagicLeapNativeBindings
            {
                /// <summary>
                /// Creates a video sink.
                /// </summary>
                /// <param name="sinkHandle">The handle to the video sink to return to the caller.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the video sink was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCVideoSinkCreate(out ulong sinkHandle);

                /// <summary>
                /// Sets the source of a video sink.
                /// </summary>
                /// <param name="sinkHandle">The handle to the video sink to set the source to.</param>
                /// <param name="sourceHandle">The handle to the source to set onto the video sink.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the source was successfully set onto the video sink.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCVideoSinkSetSource(ulong sinkHandle, ulong sourceHandle);

                /// <summary>
                /// Gets if a new frame is available for a video sink.
                /// </summary>
                /// <param name="sinkHandle">The handle to the video sink to check a new frame for.</param>
                /// <param name="newFrameAvailable">Used to return to the caller if a new frame is available.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the video sink was successfully queried.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCVideoSinkIsNewFrameAvailable(ulong sinkHandle, [MarshalAs(UnmanagedType.I1)] out bool newFrameAvailable);

                /// <summary>
                /// Gets a newly available frame from a video sink.
                /// </summary>
                /// <param name="sinkHandle">The handle to the video sink to get a new frame from.</param>
                /// <param name="frameHandle">The handle to the new frame.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the new frame was successfully acquired.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCVideoSinkAcquireNextAvailableFrame(ulong sinkHandle, out ulong frameHandle);

                /// <summary>
                /// Releases a frame from a video sink.
                /// </summary>
                /// <param name="sinkHandle">The handle to the video sink to release the frame from.</param>
                /// <param name="frameHandle">The handle to the frame to release.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the frame was successfully released.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCVideoSinkReleaseFrame(ulong sinkHandle, ulong frameHandle);

                /// <summary>
                /// Destroys a video sink.
                /// </summary>
                /// <param name="sinkHandle">The handle to the video sink to destroy.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the video sink was successfully destroyed.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCVideoSinkDestroy(ulong sinkHandle);
            }
        }
    }
}
