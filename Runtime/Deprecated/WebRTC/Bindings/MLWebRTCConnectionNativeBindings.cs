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
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Class that represents a connection used by the MLWebRTC API.
        /// </summary>
        public partial class PeerConnection
        {
            /// <summary>
            /// Native bindings for the MLWebRTC.PeerConnection class. 
            /// </summary>
            internal class NativeBindings : MagicLeapNativeBindings
            {
                /// <summary>
                /// A delegate that describes the requirements of the OnError callback.
                /// </summary>
                /// <param name="message">The error message.</param>
                /// <param name="context">Pointer to a context object.</param>
                public delegate void OnErrorDelegate([MarshalAs(UnmanagedType.LPStr)] string message, IntPtr context);

                /// <summary>
                /// A delegate that describes the requirements of the OnConnectedDelegate callback.
                /// </summary>
                /// <param name="context">Pointer to a context object.</param>
                public delegate void OnConnectedDelegate(IntPtr context);

                /// <summary>
                /// A delegate that describes the requirements of the OnDisconnectedDelegate callback.
                /// </summary>
                /// <param name="context">Pointer to a context object.</param>
                public delegate void OnDisconnectedDelegate(IntPtr context);

                /// <summary>
                /// A delegate that describes the requirements of the OnTrackAddedDelegate callback.
                /// </summary>
                /// <param name="sourceNative">The native source object that was added.</param>
                /// <param name="numStreamIds">The number of streams this track belongs to.</param>
                /// <param name="streamIdsPtr">The names of the streams this track belongs to.</param>
                /// <param name="context">Pointer to a context object.</param>                
                public delegate void OnTrackAddedDelegate(in MLWebRTC.Source.NativeBindings.MLWebRTCSource sourceNative, uint numStreamIds, IntPtr streamIdsPtr, IntPtr context);

                /// <summary>
                /// A delegate that describes the requirements of the OnTrackRemovedDelegate callback.
                /// </summary>
                /// <param name="remoteSourceHandle">The handle to the removed source.</param>
                /// <param name="context">Pointer to a context object.</param>                
                public delegate void OnTrackRemovedDelegate(ulong remoteSourceHandle, IntPtr context);

                /// <summary>
                /// A delegate that describes the requirements of the OnDataChannelReceivedDelegate callback.
                /// </summary>
                /// <param name="remoteDataChanelHandle">The handle to the newly received data channel.</param>
                /// <param name="context">Pointer to a context object.</param>
                public delegate void OnDataChannelReceivedDelegate(ulong remoteDataChanelHandle, IntPtr context);

                /// <summary>
                /// A delegate that describes the requirements of the OnSendOfferDelegate callback.
                /// </summary>
                /// <param name="offer">The <c>json</c> formatted offer string. </param>
                /// <param name="context">Pointer to a context object.</param>
                public delegate void OnLocalOfferCreatedDelegate([MarshalAs(UnmanagedType.LPStr)] string offer, IntPtr context);

                /// <summary>
                /// A delegate that describes the requirements of the OnSendAnswer callback.
                /// </summary>
                /// <param name="answer">The <c>json</c> formatted answer string.</param>
                /// <param name="context">Pointer to a context object.</param>
                public delegate void OnLocalAnswerCreatedDelegate([MarshalAs(UnmanagedType.LPStr)] string answer, IntPtr context);

                /// <summary>
                /// A delegate that describes the requirements of the OnSendIceCandidate callback.
                /// </summary>
                /// <param name="iceCandidate">The ice candidate object that was sent.</param>
                /// <param name="context">Pointer to a context object.</param>
                public delegate void OnLocalIceCandidateFoundDelegate(in MLWebRTCConnectionIceCandidate iceCandidate, IntPtr context);

                /// <summary>
                /// A delegate that describes the requirements of the OnIceGatheringCompleted callback.
                /// </summary>
                /// <param name="context">Pointer to a context object.</param>
                public delegate void OnIceGatheringCompletedDelegate(IntPtr context);

                private static MLResult.Code CreateRemoteConnection(MLWebRTC.IceServer[] iceServers, IntPtr proxyConfig, out PeerConnection connection)
                {
                    connection = new PeerConnection();

                    // Converts into native ice server structs
                    MLWebRTCConnectionIceServer[] nativeIceServers = new MLWebRTCConnectionIceServer[iceServers.Length];
                    for (int i = 0; i < iceServers.Length; ++i)
                    {
                        nativeIceServers[i].Data = iceServers[i];
                    }

                    IntPtr arrayPtr = Marshal.AllocHGlobal(Marshal.SizeOf<MLWebRTCConnectionIceServer>() * nativeIceServers.Length);
                    IntPtr walkPtr = arrayPtr;

                    for (int i = 0; i < nativeIceServers.Length; ++i)
                    {
                        Marshal.StructureToPtr(nativeIceServers[i], walkPtr, false);
                        walkPtr = new IntPtr(walkPtr.ToInt64() + Marshal.SizeOf<MLWebRTCConnectionIceServer>());
                    }

                    NativeBindings.MLWebRTCConnectionConfig config = new NativeBindings.MLWebRTCConnectionConfig();
                    config.IceServers = arrayPtr;
                    config.NumIceServers = (uint)nativeIceServers.Length;
                    config.ProxyConfig = proxyConfig;

                    NativeBindings.MLWebRTCConnectionEventCallbacks callbacks = new NativeBindings.MLWebRTCConnectionEventCallbacks();
                    callbacks.OnError = NativeBindings.OnError;
                    callbacks.OnConnected = NativeBindings.OnConnected;
                    callbacks.OnDisconnected = NativeBindings.OnDisconnected;
                    callbacks.OnLocalOfferCreated = NativeBindings.OnLocalOfferCreated;
                    callbacks.OnLocalAnswerCreated = NativeBindings.OnLocalAnswerCreated;
                    callbacks.OnLocalIceCandidateFound = NativeBindings.OnLocalIceCandidateFound;
                    callbacks.OnIceGatheringCompleted = NativeBindings.OnIceGatheringCompleted;
                    callbacks.OnTrackRemoved = NativeBindings.OnTrackRemoved;
                    callbacks.OnTrackAdded = NativeBindings.OnTrackAdded;
                    callbacks.OnDataChannelReceived = NativeBindings.OnDataChannelReceived;

                    connection.gcHandle = GCHandle.Alloc(connection);
                    IntPtr gcHandlePtr = GCHandle.ToIntPtr(connection.gcHandle);
                    callbacks.Context = gcHandlePtr;

                    ulong connectionHandle = MagicLeapNativeBindings.InvalidHandle;

                    MLResult.Code resultCode = NativeBindings.MLWebRTCConnectionCreate(in config, in callbacks, out connectionHandle);
                    Marshal.FreeHGlobal(arrayPtr);

                    if (!MLResult.IsOK(resultCode))
                    {
                        connection.gcHandle.Free();
                        return resultCode;
                    }

                    connection.Handle = connectionHandle;
                    return resultCode;
                }

                /// <summary>
                /// Creates a remote connection and sets up the native callbacks.
                /// </summary>
                /// <param name="iceServers">The ice servers to create the connection with.</param>
                /// <param name="connection">The created connection object.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the connection was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                public static MLResult.Code CreateRemoteConnection(MLWebRTC.IceServer[] iceServers, out PeerConnection connection)
                {
                    return CreateRemoteConnection(iceServers, IntPtr.Zero, out connection);
                }

                /// <summary>
                /// Creates a remote connection with a forward proxy configuration and sets up the native callbacks.
                /// </summary>
                /// <param name="iceServers">The ice servers to create the connection with.</param>
                /// <param name="proxyConfig">The forward proxy configuration.</param>
                /// <param name="connection">The created connection object.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the connection was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                public static MLResult.Code CreateRemoteConnection(MLWebRTC.IceServer[] iceServers, MLWebRTC.ProxyConfig proxyConfig, out PeerConnection connection)
                {
                    NativeBindings.MLWebRTCConnectionProxyConfig nativeProxyConfig = new MLWebRTCConnectionProxyConfig
                    {
                        Type = proxyConfig.Type,
                        HostAddress = proxyConfig.HostAddress,
                        HostPort = proxyConfig.HostPort,
                        Username = proxyConfig.Username,
                        Password = proxyConfig.Password,
                        AutoDetect = proxyConfig.AutoDetect,
                        AutoConfigUrl = proxyConfig.AutoConfigUrl,
                        BypassList = proxyConfig.BypassList
                    };

                    IntPtr proxyConfigPtr = Marshal.AllocHGlobal(Marshal.SizeOf<MLWebRTCConnectionProxyConfig>());
                    Marshal.StructureToPtr(nativeProxyConfig, proxyConfigPtr, false);

                    MLResult.Code result = CreateRemoteConnection(iceServers, proxyConfigPtr, out connection);

                    Marshal.FreeHGlobal(proxyConfigPtr);
                    return result;
                }

                /// <summary>
                /// Creates a WebRTC connection.
                /// </summary>
                /// <param name="config">The configuration that holds the ice servers.</param>
                /// <param name="callbacks">The object that holds all the callbacks that will be invoked natively.</param>
                /// <param name="connectionHandle">The handle of the connection to return to the caller.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the connection was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCConnectionCreate(in MLWebRTCConnectionConfig config, in MLWebRTCConnectionEventCallbacks callbacks, out ulong connectionHandle);

                /// <summary>
                /// Processes events of a connection, responsible for callbacks being invoked natively.
                /// </summary>
                /// <param name="connectionHandle">The handle to the connection to process events from.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the connection had it's events successfully processed.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCConnectionProcessEvents(ulong connectionHandle);

                /// <summary>
                /// Creates an offer with a connection.
                /// </summary>
                /// <param name="connectionHandle">The handle to the connection creating the offer.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the offer was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCConnectionCreateOffer(ulong connectionHandle);

                /// <summary>
                /// Sets an offer from a remote connection.
                /// </summary>
                /// <param name="connectionHandle">The handle to the connection responding to the offer.</param>
                /// <param name="offer">The offer string.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the offer was successfully set.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCConnectionSetRemoteOffer(ulong connectionHandle, [MarshalAs(UnmanagedType.LPStr)] string offer);

                /// <summary>
                /// Sets the answer to an offer from a remote connection.
                /// </summary>
                /// <param name="connectionHandle">The handle to the connection answering to the offer.</param>
                /// <param name="answer">The <c>json</c> formatted answer string to set.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the answer was successfully set.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCConnectionSetRemoteAnswer(ulong connectionHandle, [MarshalAs(UnmanagedType.LPStr)] string answer);

                /// <summary>
                /// Adds a remote ice candidate from a connection.
                /// </summary>
                /// <param name="connectionHandle">The handle to the connection to send the ice candidate to.</param>
                /// <param name="candidate">The ice candidate to send.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the ice candidate was successfully sent.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCConnectionAddRemoteIceCandidate(ulong connectionHandle, in MLWebRTCConnectionIceCandidate candidate);

                /// <summary>
                /// Gets if a connection is currently connected.
                /// </summary>
                /// <param name="connectionHandle">The handle to the connection in question.</param>
                /// <param name="isConnected">Used to return to the user to determine if the connection is connected or not.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the connection status was successfully queried.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCConnectionIsConnected(ulong connectionHandle, [MarshalAs(UnmanagedType.I1)] out bool isConnected);

                /// <summary>
                /// Gets if a connection has failed
                /// </summary>
                /// <param name="connectionHandle">The handle to the connection in question.</param>
                /// <param name="hasFailed">Used to return to the user to determine if the connection has failed or not.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the connection status was successfully queried.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCConnectionHasFailed(ulong connectionHandle, [MarshalAs(UnmanagedType.I1)] out bool hasFailed);

                /// <summary>
                /// Add a local source as a media track to the connection.
                /// </summary>
                /// <param name="connectionHandle">The handle to the connection to set the local source to.</param>
                /// <param name="localSourceHandle">The handle to the local source to set onto the connection.</param>
                /// <param name="trackInfo">Information about the track to be added</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the source was successfully set on the connection.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCConnectionAddLocalSourceTrackEx(ulong connectionHandle, ulong localSourceHandle, [In] ref MLWebRTCTrackInfo trackInfo);

                /// <summary>
                /// Sets the local source of a connection.
                /// </summary>
                /// <param name="connectionHandle">The handle to the connection to set the local source to.</param>
                /// <param name="localSourceHandle">The handle to the local source to set onto the connection.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the source was successfully set on the connection.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCConnectionRemoveLocalSourceTrack(ulong connectionHandle, ulong localSourceHandle);

                /// <summary>
                /// Destroys a WebRTC connection.
                /// </summary>
                /// <param name="connectionHandle">The handle to the connection to destroy.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the connection was successfully destroyed.
                /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> if necessary permission is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(MLWebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCConnectionDestroy(ulong connectionHandle);

                /// <summary>
                /// Callback that is invoked when an error is made.
                /// </summary>
                /// <param name="message">The error message.</param>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnErrorDelegate))]
                private static void OnError([MarshalAs(UnmanagedType.LPStr)] string message, IntPtr context)
                {
                    MLThreadDispatch.ScheduleMain(() =>
                    {
                        GCHandle gcHandle = GCHandle.FromIntPtr(context);
                        PeerConnection connection = gcHandle.Target as PeerConnection;
                        connection?.OnError?.Invoke(connection, message);
                    });
                }

                /// <summary>
                /// Callback that is invoked when a connection is established.
                /// </summary>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnConnectedDelegate))]
                private static void OnConnected(IntPtr context)
                {
                    MLThreadDispatch.ScheduleMain(() =>
                    {
                        GCHandle gcHandle = GCHandle.FromIntPtr(context);
                        PeerConnection connection = gcHandle.Target as PeerConnection;
                        connection?.OnConnected?.Invoke(connection);
                    });
                }

                /// <summary>
                /// Callback that is for OnDisconnected.
                /// </summary>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnDisconnectedDelegate))]
                private static void OnDisconnected(IntPtr context)
                {
                    MLThreadDispatch.ScheduleMain(() =>
                    {
                        GCHandle gcHandle = GCHandle.FromIntPtr(context);
                        PeerConnection connection = gcHandle.Target as PeerConnection;
                        connection?.OnDisconnected?.Invoke(connection);
                    });
                }

                /// <summary>
                /// Callback that is for OnTrackAdded.
                /// </summary>
                /// <param name="sourceNative">The native source object that was added.</param>
                /// <param name="numStreamIds">The number of streams this track belongs to.</param>
                /// <param name="streamIdsPtr">The names of the streams this track belongs to.</param>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnTrackAddedDelegate))]
                private static void OnTrackAdded(in MLWebRTC.Source.NativeBindings.MLWebRTCSource sourceNative, uint numStreamIds, IntPtr streamIdsPtr, IntPtr context)
                {
                    if (sourceNative.Handle == MagicLeapNativeBindings.InvalidHandle)
                    {
                        Debug.LogError("Received invalid source handle in NativeBindings.OnTrackAdded() callback");
                        return;
                    }

                    MLWebRTC.MediaStream.Track addedTrack = sourceNative.Data;

                    const string defaultStreamId = "unknown_remote";

                    int ptrSize = Marshal.SizeOf(typeof(IntPtr));
                    // TODO : marshal directly as a string array instead of IntPtr.
                    string[] streamIds = new string[numStreamIds];
                    for (int i = 0; i < numStreamIds; ++i)
                    {
                        IntPtr ptr = Marshal.ReadIntPtr(streamIdsPtr, i * ptrSize);
                        streamIds[i] = (ptr != IntPtr.Zero) ? Marshal.PtrToStringAnsi(ptr) : defaultStreamId;
                    }

                    if (streamIds.Length == 0)
                    {
                        streamIds = new string[1];
                        streamIds[0] = defaultStreamId;
                    }

                    MLThreadDispatch.ScheduleMain(() =>
                    {
                        GCHandle gcHandle = GCHandle.FromIntPtr(context);
                        PeerConnection connection = gcHandle.Target as PeerConnection;
                        addedTrack.ParentConnection = connection;
                        List<MediaStream> mediaStreams = new List<MediaStream>();

                        foreach (string streamId in streamIds)
                        {
                            MLWebRTC.MediaStream mediaStream = null;
                            if (!connection.remoteMediaStreams.ContainsKey(streamId))
                            {
                                mediaStream = MLWebRTC.MediaStream.Create(connection, streamId);
                                connection.remoteMediaStreams.Add(mediaStream.Id, mediaStream);
                            }
                            else
                            {
                                mediaStream = connection.remoteMediaStreams[streamId];
                            }

                            mediaStream.Tracks.Add(addedTrack);
                            // Mark this track as "selected" if this is the first track of its type in this stream.
                            if (addedTrack.TrackType == MediaStream.Track.Type.Audio && mediaStream.ActiveAudioTrack == null)
                            {
                                mediaStream.SelectTrack(addedTrack);
                            }
                            else if (addedTrack.TrackType == MediaStream.Track.Type.Video && mediaStream.ActiveVideoTrack == null)
                            {
                                mediaStream.SelectTrack(addedTrack);
                            }

                            mediaStreams.Add(mediaStream);
                        }

                        connection.OnTrackAddedMultipleStreams?.Invoke(mediaStreams, addedTrack);
                    });
                }

                /// <summary>
                /// Callback that is for OnTrackRemoved.
                /// </summary>
                /// <param name="remoteSourceHandle">The handle to the removed source.</param>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnTrackRemovedDelegate))]
                private static void OnTrackRemoved(ulong remoteSourceHandle, IntPtr context)
                {
                    MLThreadDispatch.ScheduleMain(() =>
                    {
                        GCHandle gcHandle = GCHandle.FromIntPtr(context);
                        PeerConnection connection = gcHandle.Target as PeerConnection;

                        if (!MagicLeapNativeBindings.MLHandleIsValid(remoteSourceHandle) || connection == null)
                        {
                            return;
                        }

                        List<MLWebRTC.MediaStream> mediaStreams = new List<MediaStream>();
                        MLWebRTC.MediaStream.Track removedTrack = null;

                        foreach (MLWebRTC.MediaStream remoteMediaStream in connection.remoteMediaStreams.Values)
                        {
                            foreach (MLWebRTC.MediaStream.Track track in remoteMediaStream.Tracks)
                            {
                                if (track.Handle == remoteSourceHandle)
                                {
                                    removedTrack = track;
                                    remoteMediaStream.UnSelectTrack(removedTrack);
                                    remoteMediaStream.Tracks.Remove(removedTrack);
                                    mediaStreams.Add(remoteMediaStream);
                                    break;
                                }
                            }
                        }
                        removedTrack.Cleanup();

                        connection.OnTrackRemovedMultipleStreams?.Invoke(mediaStreams, removedTrack);
                    });
                }

                /// <summary>
                /// Callback that is for OnDataChannelReceived.
                /// </summary>
                /// <param name="dataChannelHandle">The handle to the newly received data channel.</param>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnDataChannelReceivedDelegate))]
                private static void OnDataChannelReceived(ulong dataChannelHandle, IntPtr context)
                {
                    MLThreadDispatch.ScheduleMain(() =>
                    {
                        GCHandle gcHandle = GCHandle.FromIntPtr(context);
                        PeerConnection connection = gcHandle.Target as PeerConnection;
                        if (connection == null)
                        {
                            return;
                        }

                        if (!MagicLeapNativeBindings.MLHandleIsValid(dataChannelHandle))
                        {
                            return;
                        }

                        MLWebRTC.DataChannel dataChannel = new MLWebRTC.DataChannel(dataChannelHandle)
                        {
                            ParentConnection = connection
                        };

                        MLResult.Code resultCode = MLWebRTC.DataChannel.NativeBindings.SetCallbacks(dataChannel);
                        if (!MLResult.IsOK(resultCode))
                        {
                            return;
                        }

                        IntPtr labelPtr = IntPtr.Zero;
                        resultCode = MLWebRTC.DataChannel.NativeBindings.MLWebRTCDataChannelGetLabel(dataChannelHandle, out labelPtr);
                        if (!MLResult.IsOK(resultCode))
                        {
                            return;
                        }

                        if (labelPtr != IntPtr.Zero)
                        {
                            dataChannel.Label = Marshal.PtrToStringAnsi(labelPtr);
                        }

                        resultCode = MLWebRTC.DataChannel.NativeBindings.MLWebRTCDataChannelReleaseLabelMemory(dataChannelHandle, labelPtr);

                        connection.remoteDataChannels.Add(dataChannel);
                        connection.OnDataChannelReceived?.Invoke(connection, dataChannel);
                    });
                }

                /// <summary>
                /// Callback that is invoked when an offer is sent.
                /// </summary>
                /// <param name="sendSdp">The <c>sdp</c> string that was sent.</param>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnLocalOfferCreatedDelegate))]
                private static void OnLocalOfferCreated([MarshalAs(UnmanagedType.LPStr)] string sendSdp, IntPtr context)
                {
                    MLThreadDispatch.ScheduleMain(() =>
                    {
                        GCHandle gcHandle = GCHandle.FromIntPtr(context);
                        PeerConnection connection = gcHandle.Target as PeerConnection;
                        connection?.OnLocalOfferCreated?.Invoke(connection, sendSdp);
                    });
                }

                /// <summary>
                /// Callback that is invoked when an answer is sent.
                /// </summary>
                /// <param name="sendAnswer">The answer string that was sent.</param>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnLocalAnswerCreatedDelegate))]
                private static void OnLocalAnswerCreated([MarshalAs(UnmanagedType.LPStr)] string sendAnswer, IntPtr context)
                {
                    MLThreadDispatch.ScheduleMain(() =>
                    {
                        GCHandle gcHandle = GCHandle.FromIntPtr(context);
                        PeerConnection connection = gcHandle.Target as PeerConnection;
                        connection?.OnLocalAnswerCreated?.Invoke(connection, sendAnswer);
                    });
                }

                /// <summary>
                /// Callback that is invoked when an ice candidate is sent.
                /// </summary>
                /// <param name="nativeIceCandidate">The native ice candidate object that was sent.</param>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnLocalIceCandidateFoundDelegate))]
                private static void OnLocalIceCandidateFound(in NativeBindings.MLWebRTCConnectionIceCandidate nativeIceCandidate, IntPtr context)
                {
                    MLWebRTC.IceCandidate iceCandidate = nativeIceCandidate.Data;

                    MLThreadDispatch.ScheduleMain(() =>
                    {
                        GCHandle gcHandle = GCHandle.FromIntPtr(context);
                        PeerConnection connection = gcHandle.Target as PeerConnection;
                        connection?.OnLocalIceCandidateFound?.Invoke(connection, iceCandidate);
                    });
                }

                /// <summary>
                /// Callback that is invoked when an ice candidate is sent.
                /// </summary>
                /// <param name="nativeIceCandidate">The native ice candidate object that was sent.</param>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnIceGatheringCompletedDelegate))]
                private static void OnIceGatheringCompleted(IntPtr context)
                {
                    MLThreadDispatch.ScheduleMain(() =>
                    {
                        GCHandle gcHandle = GCHandle.FromIntPtr(context);
                        PeerConnection connection = gcHandle.Target as PeerConnection;
                        connection?.OnIceGatheringCompleted?.Invoke(connection);
                    });
                }

                /// <summary>
                /// The native representation of the MLWebRTC connection configuration.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCConnectionConfig
                {
                    /// <summary>
                    /// Version of the struct.
                    /// </summary>
                    public uint Version;

                    /// <summary>
                    /// Pointer to the array of ice servers.
                    /// </summary>
                    public IntPtr IceServers;

                    /// <summary>
                    /// Number of ice servers the above pointer can step through.
                    /// </summary>
                    public uint NumIceServers;

                    /// <summary>
                    /// (Optional) Config for a forward proxy
                    /// </summary>
                    public IntPtr ProxyConfig;
                }

                /// <summary>
                /// The native representation of the MLWebRTC callback events.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCConnectionEventCallbacks
                {
                    /// <summary>
                    /// Version of the struct.
                    /// </summary>
                    public uint Version;

                    /// <summary>
                    /// Context pointer.
                    /// </summary>
                    public IntPtr Context;

                    /// <summary>
                    /// OnError event.
                    /// </summary>
                    public OnErrorDelegate OnError;

                    /// <summary>
                    /// OnConnected event.
                    /// </summary>
                    public OnConnectedDelegate OnConnected;

                    /// <summary>
                    /// OnDisconnected event.
                    /// </summary>
                    public OnDisconnectedDelegate OnDisconnected;

                    /// <summary>
                    /// OnTrackAdded event.
                    /// </summary>
                    public OnTrackAddedDelegate OnTrackAdded;

                    /// <summary>
                    /// OnTrackRemoved event.
                    /// </summary>
                    public OnTrackRemovedDelegate OnTrackRemoved;

                    /// <summary>
                    /// OnDataChannelReceived event.
                    /// </summary>
                    public OnDataChannelReceivedDelegate OnDataChannelReceived;

                    /// <summary>
                    /// OnSendOffer event.
                    /// </summary>
                    public OnLocalOfferCreatedDelegate OnLocalOfferCreated;

                    /// <summary>
                    /// OnSendAnswer event.
                    /// </summary>
                    public OnLocalAnswerCreatedDelegate OnLocalAnswerCreated;

                    /// <summary>
                    /// OnSendIceCandidate event.
                    /// </summary>
                    public OnLocalIceCandidateFoundDelegate OnLocalIceCandidateFound;

                    /// <summary>
                    /// OnIceGatheringCompleted event.
                    /// </summary>
                    public OnIceGatheringCompletedDelegate OnIceGatheringCompleted;
                }

                /// <summary>
                /// The native representation of an ice server.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCConnectionIceServer
                {
                    /// <summary>
                    /// Version of the struct.
                    /// </summary>
                    public uint Version;

                    /// <summary>
                    /// Uri of the ice server.
                    /// </summary>
                    [MarshalAs(UnmanagedType.LPStr)]
                    public string Uri;

                    /// <summary>
                    /// Username of the ice server.
                    /// </summary>
                    [MarshalAs(UnmanagedType.LPStr)]
                    public string UserName;

                    /// <summary>
                    /// Password to the ice server.
                    /// </summary>
                    [MarshalAs(UnmanagedType.LPStr)]
                    public string Password;

                    /// <summary>
                    /// Sets the data of this object from another MLWebRTC.IceServer object.
                    /// </summary>
                    public MLWebRTC.IceServer Data
                    {
                        set
                        {
                            this.Uri = value.Uri;
                            this.UserName = value.UserName;
                            this.Password = value.Password;
                        }
                    }
                }

                /// <summary>
                /// The native representation of an ice candidate.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCConnectionIceCandidate
                {
                    /// <summary>
                    /// Version of the struct.
                    /// </summary>
                    public uint Version;

                    /// <summary>
                    /// The id of the ice candidate.
                    /// </summary>
                    [MarshalAs(UnmanagedType.LPStr)]
                    public string Candidate;

                    /// <summary>
                    /// The <c>sdp mid</c> of the ice candidate.
                    /// </summary>
                    [MarshalAs(UnmanagedType.LPStr)]
                    public string SdpMid;

                    /// <summary>
                    /// The <c>sdp mline index</c> of the ice candidate.
                    /// </summary>
                    public int SdpMlineIndex;

                    /// <summary>
                    /// Gets or sets data from an MLWebRTC.IceCandidate object.
                    /// </summary>
                    public MLWebRTC.IceCandidate Data
                    {
                        get
                        {
                            return MLWebRTC.IceCandidate.Create(this.Candidate, this.SdpMid, this.SdpMlineIndex);
                        }

                        set
                        {
                            this.Candidate = value.Candidate;
                            this.SdpMid = value.SdpMid;
                            this.SdpMlineIndex = value.SdpMLineIndex;
                        }
                    }
                }

                /// <summary>
                /// Native representation of proxy config
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCConnectionProxyConfig
                {
                    /// <summary>
                    /// Type of proxy
                    /// </summary>
                    public MLWebRTC.ProxyType Type;

                    /// <summary>
                    /// Proxy server address
                    /// </summary>
                    [MarshalAs(UnmanagedType.LPStr)]
                    public string HostAddress;

                    /// <summary>
                    /// Proxy server port
                    /// </summary>
                    public int HostPort;

                    /// <summary>
                    /// Proxy server auth username
                    /// </summary>
                    [MarshalAs(UnmanagedType.LPStr)]
                    public string Username;

                    /// <summary>
                    /// Proxy server auth password
                    /// </summary>
                    [MarshalAs(UnmanagedType.LPStr)]
                    public string Password;

                    /// <summary>
                    /// Attempt to automatically detect proxy
                    /// </summary>
                    [MarshalAs(UnmanagedType.I1)]
                    public bool AutoDetect;

                    /// <summary>
                    /// Url to use to download the proxy config
                    /// </summary>
                    [MarshalAs(UnmanagedType.LPStr)]
                    public string AutoConfigUrl;

                    /// <summary>
                    /// List of urls that should bypass the proxy
                    /// </summary>
                    [MarshalAs(UnmanagedType.LPStr)]
                    public string BypassList;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCTrackInfo
                {
                    /// <summary>
                    /// Struct version
                    /// </summary>
                    private readonly uint version;

                    /// <summary>
                    /// The names of each stream this track belongs to
                    /// </summary>
                    private readonly uint streamCount;

                    /// <summary>
                    /// Names of the streams this track belongs to.
                    /// </summary>
                    [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)]
                    private IntPtr[] streams;

                    /// <summary>
                    /// Id of the track.
                    /// </summary>
                    [MarshalAs(UnmanagedType.LPStr)]
                    private readonly string trackId;

                    public MLWebRTCTrackInfo(string[] streams, string trackId)
                    {
                        this.version = 1;
                        this.streamCount = (streams != null) ? (uint)streams.Length : 0;
                        this.trackId = trackId ?? string.Empty;

                        // [MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.LPStr)] doesnt work when char** is in a struct instead of a func param.
                        this.streams = (streams != null) ? new IntPtr[streams.Length] : null;
                        for (int i = 0; i < streamCount; ++i)
                        {
                            this.streams[i] = Marshal.StringToHGlobalAnsi(streams[i]);
                        }
                    }

                    public void FreeUnmanagedMemory()
                    {
                        foreach (IntPtr ptr in streams)
                        {
                            Marshal.FreeHGlobal(ptr);
                        }
                        streams = null;
                    }
                }
            }
        }
    }
}
