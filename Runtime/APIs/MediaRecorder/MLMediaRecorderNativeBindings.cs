// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// MLMediaRecorder description goes here.
    /// </summary>
    /// 
    public partial class MLMediaRecorder
    {
        /// <summary>
        /// See ml_media_recorder.h for additional comments.
        /// </summary>
        internal class NativeBindings : Native.MagicLeapNativeBindings
        {
            /// <summary>
            /// Callback whenever MediaRecorder received a general info/warning message.
            /// </summary>
            public delegate void OnInfoDelegate(ulong handle, ref MLMediaRecorderOnInfo info);

            /// <summary>
            /// Callback whenever MediaRecorder received a track-related info/warning message.
            /// </summary>
            public delegate void OnTrackInfoDelegate(ulong handle, ref MLMediaRecorderOnInfo trackInfo);

            /// <summary>
            /// Callback whenever MediaRecorder received a general error message.
            /// </summary>
            public delegate void OnErrorDelegate(ulong handle, ref MLMediaRecorderOnError error);

            /// <summary>
            /// Callback whenever MediaRecorder received a track-related error message.
            /// </summary>
            public delegate void OnTrackErrorDelegate(ulong handle, ref MLMediaRecorderOnError trackError);

            /// <summary>
            /// When the error or info type is track specific, it has the following layout:
            /// The left-most 16-bit is meant for error or info type and the right-most 4-bit is meant for track id.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLMediaRecorderOnError
            {
                /// <summary>
                /// Holds valid track id only for track specific info (returned via on_track_info callback.).
                /// Holds 0 for the generic info (returned via on_info callback).
                /// </summary>
                public uint TrackId;

                /// <summary>
                /// The error of MLMediaRecorderError
                /// </summary>
                public Error Error;

                /// <summary>
                /// The extra info
                /// </summary>
                public int Extra;

                /// <summary>
                /// User data as passed to MLMediaRecorderSetEventCallbacks.
                /// </summary>
                public IntPtr Data;

                /// <summary>
                /// Create and return an initialized version of this struct.
                /// </summary>
                /// <returns>A new instance of this struct.</returns>
                public static MLMediaRecorderOnError Create()
                {
                    return new MLMediaRecorderOnError()
                    {
                    };
                }
            };

            /// <summary>
            /// When the error or info type is track specific, it has the following layout:
            /// The left-most 16-bit is meant for error or info type and the right-most 4-bit is meant for track id.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLMediaRecorderOnInfo
            {
                /// <summary>
                /// Holds valid track id only for track specific info (returned via on_track_info callback.).
                /// Holds 0 for the generic info (returned via on_info callback).
                /// </summary>
                public uint TrackId;

                /// <summary>
                /// The info of MLMediaRecorderInfo
                /// </summary>
                public Info Info;

                /// <summary>
                /// The extra info
                /// </summary>
                public int Extra;

                /// <summary>
                /// User data as passed to MLMediaRecorderSetEventCallbacks.
                /// </summary>
                public IntPtr Data;

                /// <summary>
                /// Create and return an initialized version of this struct.
                /// </summary>
                /// <returns>A new instance of this struct.</returns>
                public static MLMediaRecorderOnInfo Create()
                {
                    return new MLMediaRecorderOnInfo()
                    {
                    };
                }
            };

            /// <summary>
            /// Comment Needed!
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLMediaRecorderEventCallbacks
            {
                /// <summary>
                /// Struct version.
                /// </summary>
                private uint version;

                /// <summary>
                /// MediaRecorder received a general info/warning message.
                /// </summary>
                public OnInfoDelegate OnInfo;

                /// <summary>
                /// MediaRecorder received a track-related info/warning message.
                /// </summary>
                public OnTrackInfoDelegate OnTrackInfo;

                /// <summary>
                /// MediaRecorder received a general error message.
                /// </summary>
                public OnErrorDelegate OnError;

                /// <summary>
                /// MediaRecorder received a track-related error message.
                /// </summary>
                public OnTrackErrorDelegate OnTrackError;

                /// <summary>
                /// Create and return an initialized version of this struct.
                /// </summary>
                /// <returns>A new instance of this struct.</returns>
                public static MLMediaRecorderEventCallbacks Create()
                {
                    return new MLMediaRecorderEventCallbacks()
                    {
                        version = 1,
                        OnInfo = NativeBindings.OnInfo,
                        OnTrackInfo = NativeBindings.OnTrackInfo,
                        OnError = NativeBindings.OnError,
                        OnTrackError = NativeBindings.OnTrackError,
                    };
                }
            };

            /// <summary>
            /// Sets the path of the output file to be produced.  Call this after MLMediaRecorderSetOutputFormat() but before
            /// MLMediaRecorder prepare().
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderSetOutputFileForPath(ulong handle,
                [MarshalAs(unmanagedType: UnmanagedType.LPStr)] string path);

            /// <summary>
            /// Prepares the recorder to begin capturing and encoding data for input mediaformat.  This method must be called after
            /// setting up the desired audio and video sources, encoders, but before start().
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderPrepare(ulong handle, ulong formatHandle);

            /// <summary>
            /// Stops recording.  Call this after MLMediaRecorderStart().  Once recording is stopped, you will have to configure it
            /// again as if it has just been constructed.
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderStop(ulong handle);

            /// <summary>
            /// Create a new
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderCreate(out ulong outHandle);

            /// <summary>
            /// Destroy a MediaRecorder object.
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderDestroy(ulong handle);


            /// <summary>
            /// Pass in the file descriptor of the file to be written.  Call this after MLMediaRecorderSetOutputFormat() but before
            /// MLMediaRecorderprepare().
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderSetOutputFileForFD(ulong handle, int Fd);

            /// <summary>
            /// Sets the MediaRecorderListener object that will receive MediaRecorder notifications.
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderSetEventCallbacks(ulong handle, [In] ref MLMediaRecorderEventCallbacks callbacks,
                IntPtr data);

            /// <summary>
            /// Sets MediaRecorder default video source.  cannot be called twice (without calling MLMediaRecorderReset() in between)
            /// because it triggers internal initialization.  Current implementation supports only camera as video source from
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderSetVideoSource(ulong handle, VideoSource inVideoSource);

            /// <summary>
            /// Set MediaRecorder audio source.  cannot be called twice (without calling MLMediaRecorderReset() in between) because it
            /// triggers internal initialization.
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderSetAudioSource(ulong handle, AudioSource inAudioSource);

            /// <summary>
            /// Sets the format of the output file produced during recording.
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderSetOutputFormat(ulong handle, OutputFormat inFormat);

            /// <summary>
            /// Sets the video encoder to be used for recording.  If this method is not called, the output file will not contain an
            /// video track.  Call this after MLMediaRecorderSetOutputFormat() and before MLMediaRecorderPrepare().  The video source is
            /// always set to camera by default.
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderSetVideoEncoder(ulong handle, VideoEncoder inVideoEncoder);

            /// <summary>
            /// Sets the audio encoder to be used for recording.  If this method is not called, the output file will not contain an
            /// audio track.  Call this after MLMediaRecorderSetOutputFormat() and before MLMediaRecorderPrepare().
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderSetAudioEncoder(ulong handle, AudioEncoder inAudioEncoder);

            /// <summary>
            /// Sets the maximum duration (in ms) of the recording session.  Call this after MLMediaRecorderSetOutputFormat() and before
            /// MLMediaRecorderPrepare().  After recording reaches the specified duration, a notification will be sent via the callback
            /// with a MLMediaRecorderInfo code of MLMediaRecorderInfo_MaxDurationReached and recording will be stopped.  Stopping
            /// happens asynchronously, there is no guarantee that the recorder will have stopped by the time the listener is notified.
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderSetMaxDuration(ulong handle, int inMaxDurationMsec);

            /// <summary>
            /// Sets the maximum file size (in bytes) of the recording session.
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderSetMaxFileSize(ulong handle, long inMaxFileSize);

            /// <summary>
            /// Sets the GEO location for recording.
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderSetGeoLocation(ulong handle, long inLatitude10000,
                long inLongitude10000);

            /// <summary>
            /// Begins capturing and encoding data to the specified file.  Call this after MLMediaRecorderPrepare().  The apps should
            /// not start another recording session during recording.
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderStart(ulong handle);

            /// <summary>
            /// Gets the input surface to record from when using SURFACE video source.  May only be called after MLMediaRecorderPrepare.
            /// Frames rendered to the producer before MLMediaRecorderStart() is called will be discarded.  When using an input
            /// surface, there are no accessible input buffers, as buffers are automatically passed from the other modules to this surface.
            /// The returned input surface can also be passed as a destination surface to - a video/mixed reality video capture session
            /// when calling MLCameraPrepareCapture().  Captured raw video frames will be consumed directly as input to an encoder
            /// without copying.  Caller of this API should release the surface using #MLMediaRecorderReleaseInputSurface() on the Surface
            /// handle after usage.
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderGetInputSurface(ulong handle, out ulong outInputSurfaceHandle);

            /// <summary>
            /// Returns the maximum absolute amplitude that was sampled since the last call to this method.  Call this only after the
            /// MLMediaRecorderSetAudioSource().
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderGetMaxAmplitude(ulong handle, out int MaxAmp);

            /// <summary>
            /// Release the Surface that was returned by #MLMediaRecorderGetInputSurface.
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderReleaseInputSurface(ulong handle, ulong inputSurfaceHandle);

            /// <summary>
            /// Restarts the MediaRecorder to its idle state.  After calling this method, you will have to configure it again as if it
            /// had just been constructed.
            /// </summary>
            [DllImport(MLMediaRecorderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaRecorderReset(ulong handle);

            /// <summary>
            /// Converts native callback <seealso cref="OnInfoDelegate"/> to public.
            /// </summary>
            [AOT.MonoPInvokeCallback(typeof(OnInfoDelegate))]
            internal static void OnInfo(ulong handle, ref MLMediaRecorderOnInfo info)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(info.Data);
                MLMediaRecorder mediaRecorder = gcHandle.Target as MLMediaRecorder;

                OnInfoData mediaRecorderInfo = new OnInfoData()
                {
                    Info = info.Info,
                    Extra = info.Extra,
                };

                MLThreadDispatch.Call(mediaRecorderInfo, mediaRecorder.OnInfo);
            }

            /// <summary>
            /// Converts native callback <seealso cref="OnTrackInfoDelegate"/> to public.
            /// </summary>
            [AOT.MonoPInvokeCallback(typeof(OnTrackInfoDelegate))]
            private static void OnTrackInfo(ulong handle, ref MLMediaRecorderOnInfo trackInfo)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(trackInfo.Data);
                MLMediaRecorder mediaRecorder = gcHandle.Target as MLMediaRecorder;

                OnTrackInfoData mediaRecorderInfo = new OnTrackInfoData()
                {
                    TrackId = trackInfo.TrackId,
                    Info = trackInfo.Info,
                    Extra = trackInfo.Extra,
                };

                MLThreadDispatch.Call(mediaRecorderInfo, mediaRecorder.OnTrackInfo);
            }

            /// <summary>
            /// Converts native callback <seealso cref="OnErrorDelegate"/> to public.
            /// </summary>
            [AOT.MonoPInvokeCallback(typeof(OnErrorDelegate))]
            private static void OnError(ulong handle, ref MLMediaRecorderOnError error)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(error.Data);
                MLMediaRecorder mediaRecorder = gcHandle.Target as MLMediaRecorder;

                OnErrorData mediaRecorderError = new OnErrorData()
                {
                    Error = error.Error,
                    Extra = error.Extra,
                };

                MLThreadDispatch.Call(mediaRecorderError, mediaRecorder.OnError);
            }

            /// <summary>
            /// Converts native callback <seealso cref="OnTrackErrorDelegate"/> to public.
            /// </summary>
            [AOT.MonoPInvokeCallback(typeof(OnTrackErrorDelegate))]
            private static void OnTrackError(ulong handle, ref MLMediaRecorderOnError trackError)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(trackError.Data);
                MLMediaRecorder mediaRecorder = gcHandle.Target as MLMediaRecorder;

                OnTrackErrorData mediaRecorderError = new OnTrackErrorData()
                {
                    TrackId = trackError.TrackId,
                    Error = trackError.Error,
                    Extra = trackError.Extra,
                };

                MLThreadDispatch.Call(mediaRecorderError, mediaRecorder.OnTrackError);
            }
        }
    }
}
