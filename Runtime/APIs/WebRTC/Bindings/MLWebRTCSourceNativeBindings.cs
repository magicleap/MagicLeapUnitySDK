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
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Class that represents a source used by the MLWebRTC API.
        /// </summary>
        public partial class Source
        {
            /// <summary>
            /// Native bindings for the MLWebRTC.Source struct. 
            /// </summary>
            internal class NativeBindings : MagicLeapNativeBindings
            {
                /// <summary>
                /// Creates the local source that links to the user's MLCamera.
                /// </summary>
                /// <param name="sourceHandle">The handle to the local source to return to the caller.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the local source was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceCreateLocalSourceForCamera(in MLCamera.NativeBindings.MLCameraConnectContext inputContext, out ulong sourceHandle);

                /// <summary>
                /// Creates the local source with the specified track name that links to the user's microphone.
                /// </summary>
                /// <param name="trackName">Track name</param>
                /// <param name="sourceHandle">The handle to the local source to return to the caller.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the local source was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceCreateLocalSourceForMicrophoneEx([MarshalAs(UnmanagedType.LPStr)] string trackName, out ulong sourceHandle);

                /// <summary>
                /// Creates the local source with the specified parameters.
                /// </summary>
                /// <param name="sourceParams">Audio source parameters</param>
                /// <param name="sourceHandle">The handle to the local source to return to the caller.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the local source was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceCreateAppDefinedAudioSourceEx(ref MLWebRTCAppDefinedSourceParams sourceParams, out ulong sourceHandle);


                /// <summary>
                /// Checks if an audio source is currently enabled.
                /// </summary>
                /// <param name="sourceHandle">The handle of the source.</param>
                /// <param name="enabled">True if source is enabled.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the audio source status was queried successfully.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceIsEnabled(ulong sourceHandle, [MarshalAs(UnmanagedType.I1)] out bool enabled);

                /// <summary>
                /// Enables or disables a audio source.
                /// </summary>
                /// <param name="sourceHandle">The handle of the audio source.</param>
                /// <param name="enabled">Sets the audio source to be enabled or disabled.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the audio source was enabled/disabled successfully.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceSetEnabled(ulong sourceHandle, [MarshalAs(UnmanagedType.I1)] bool enabled);

                /// <summary>
                /// Checks if an video source is currently enabled.
                /// </summary>
                /// <param name="sourceHandle">The handle of the video source.</param>
                /// <param name="sourceType">Type of the source.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the video source status was queried successfully.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceGetType(ulong sourceHandle, out MLWebRTC.MediaStream.Track.Type sourceType);

                /// <summary>
                /// Gets the track Id of a source, call MLWebRTCSourceReleaseTrackId after.
                /// </summary>
                /// <param name="sourceHandle">The handle of the media source.</param>
                /// <param name="trackIdPtr">Double-pointer to the unmanaged trackId string.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceGetTrackId(ulong sourceHandle, out IntPtr trackIdPtr);

                /// <summary>
                /// Releases the memory created when calling MLWebRTCSourceGetTrackId.
                /// </summary>
                /// <param name="sourceHandle">The handle of the data channel.</param>
                /// <param name="trackId">Pointer to the unmanaged trackId string.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceReleaseTrackId(ulong sourceHandle, IntPtr trackId);

                /// <summary>
                /// Destroys the local source.
                /// </summary>
                /// <param name="sourceHandle">The handle to the local source to destroy.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the local source was successfully destroyed.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceDestroy(ulong sourceHandle);

                /// <summary>
                /// The native representation of an MLWebRTC source.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCSource
                {
                    /// <summary>
                    /// Version of the struct.
                    /// </summary>
                    public uint Version;

                    /// <summary>
                    /// Type of the struct.
                    /// </summary>
                    public MLWebRTC.MediaStream.Track.Type Type;

                    /// <summary>
                    /// Handle of the struct.
                    /// </summary>
                    public ulong Handle;

                    /// <summary>
                    /// Gets an MLWebRTC.Source object from the data of this object.
                    /// </summary>
                    public MLWebRTC.MediaStream.Track Data
                    {
                        get
                        {
                            string trackId = string.Empty;
                            MLResult.Code resultCode = NativeBindings.MLWebRTCSourceGetTrackId(this.Handle, out IntPtr trackIdPtr);

                            if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCSourceGetTrackId)))
                            {
                                if (trackIdPtr != IntPtr.Zero)
                                {
                                    trackId = Marshal.PtrToStringAnsi(trackIdPtr);
                                    resultCode = NativeBindings.MLWebRTCSourceReleaseTrackId(this.Handle, trackIdPtr);
                                    MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCSourceReleaseTrackId));
                                }
                            }

                            MLWebRTC.MediaStream.Track track = new MLWebRTC.MediaStream.Track(trackId)
                            {
                                Handle = this.Handle,
                                TrackType = this.Type
                            };
                            return track;
                        }
                    }

                    /// <summary>
                    /// Creates and returns an initialized version of this struct from a native MLWebRTCSource object.
                    /// </summary>
                    /// <returns>An initialized version of this struct.</returns>
                    public static MLWebRTCSource Create()
                    {
                        MLWebRTCSource source = new MLWebRTCSource();
                        source.Version = 1;
                        source.Handle = MagicLeapNativeBindings.InvalidHandle;
                        return source;
                    }
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCAppDefinedSourceParams
                {
                    public uint Version;

                    public IntPtr Callbacks;

                    [MarshalAs(UnmanagedType.LPStr)]
                    public string TrackName;

                    public static MLWebRTCAppDefinedSourceParams Create(string trackName)
                    {
                        MLWebRTCAppDefinedSourceParams sourceParams = new MLWebRTCAppDefinedSourceParams();
                        sourceParams.Version = 1;
                        sourceParams.Callbacks = IntPtr.Zero;
                        sourceParams.TrackName = trackName;
                        return sourceParams;
                    }

                    public static MLWebRTCAppDefinedSourceParams Create(string trackName, AppDefinedSource.NativeBindings.MLWebRTCAppDefinedSourceEventCallbacks callbacks)
                    {
                        MLWebRTCAppDefinedSourceParams sourceParams = new MLWebRTCAppDefinedSourceParams();
                        sourceParams.Version = 1;
                        IntPtr callbacksPtr = Marshal.AllocHGlobal(Marshal.SizeOf(callbacks));
                        Marshal.StructureToPtr(callbacks, callbacksPtr, false);
                        sourceParams.Callbacks = callbacksPtr;
                        sourceParams.TrackName = trackName;
                        return sourceParams;
                    }
                }
            }
        }
    }
}
