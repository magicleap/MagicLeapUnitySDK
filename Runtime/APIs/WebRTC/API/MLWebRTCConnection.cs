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
    using System.Text;
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
            /// A map of the remote media streams for the connection, keys media stream ids.
            /// </summary>
            private Dictionary<string, MLWebRTC.MediaStream> remoteMediaStreams = new Dictionary<string, MLWebRTC.MediaStream>();

            /// <summary>
            /// A list of the local media stream tracks for the connection.
            /// </summary>
            private List<MLWebRTC.MediaStream.Track> localMediaStreamTracks = new List<MLWebRTC.MediaStream.Track>();

            /// <summary>
            /// A list of the remote data channels for the connection.
            /// </summary>
            private List<MLWebRTC.DataChannel> remoteDataChannels = new List<DataChannel>();

            /// <summary>
            /// A list of the local data channels for the connection.
            /// </summary>
            private List<MLWebRTC.DataChannel> localDataChannels = new List<DataChannel>();

            /// <summary>
            /// The handle for this managed object.
            /// </summary>
            private GCHandle gcHandle;

            /// <summary>
            /// Initializes a new instance of the <see cref="PeerConnection" /> class.
            /// </summary>
            internal PeerConnection()
            {
            }

            /// <summary>
            /// Delegate describing the callback necessary to monitor errors.
            /// </summary>
            /// <param name="connection">The connection the event was invoked on.</param>
            /// <param name="message">The error message.</param>
            public delegate void OnErrorDelegate(MLWebRTC.PeerConnection connection, string message);

            /// <summary>
            /// Delegate describing the callback necessary to monitor if a connection has been made.
            /// </summary>
            /// <param name="connection">The connection the event was invoked on.</param>
            public delegate void OnConnectedDelegate(MLWebRTC.PeerConnection connection);

            /// <summary>
            /// Delegate describing the callback necessary to monitor if a disconnection has been made.
            /// </summary>
            /// <param name="connection">The connection the event was invoked on.</param>
            public delegate void OnDisconnectedDelegate(MLWebRTC.PeerConnection connection);

            /// <summary>
            /// A delegate that describes the requirements of the OnTrackAddedDelegate callback.
            /// </summary>
            /// <param name="mediaStreams">The media stream associated with the added track.</param>
            /// <param name="addedTrack">The track that was added to the connection.</param>
            public delegate void OnTrackAddedMultipleStreamsDelegate(List<MLWebRTC.MediaStream> mediaStreams, MLWebRTC.MediaStream.Track addedTrack);

            /// <summary>
            /// A delegate that describes the requirements of the OnTrackRemovedDelegate callback.
            /// </summary>
            /// <param name="mediaStreams">The media stream associated with the removed track.</param>
            /// <param name="removedTrack">The track that was removed from the connection.</param>
            public delegate void OnTrackRemovedMultipleStreamsDelegate(List<MLWebRTC.MediaStream> mediaStreams, MLWebRTC.MediaStream.Track removedTrack);

            /// <summary>
            /// A delegate that describes the requirements of the OnDataChannelReceivedDelegate callback.
            /// </summary>
            /// <param name="connection">The connection the event was invoked on.</param>
            /// <param name="dataChannel">The data channel that was added to the connection.</param>
            public delegate void OnDataChannelReceivedDelegate(MLWebRTC.PeerConnection connection, MLWebRTC.DataChannel dataChannel);

            /// <summary>
            /// Delegate describing the callback necessary to monitor when an offer is sent.
            /// </summary>
            /// <param name="connection">The connection the event was invoked on.</param>
            /// <param name="sdpSend">The <c>json</c> of the offer sent.</param>
            public delegate void OnLocalOfferCreatedDelegate(MLWebRTC.PeerConnection connection, string sdpSend);

            /// <summary>
            /// Delegate describing the callback necessary to monitor when an answer to an offer is sent.
            /// </summary>
            /// <param name="connection">The connection the event was invoked on.</param>
            /// <param name="sendAnswer">The <c>json</c> of the answer sent.</param>
            public delegate void OnLocalAnswerCreatedDelegate(MLWebRTC.PeerConnection connection, string sendAnswer);

            /// <summary>
            /// Delegate describing the callback necessary to monitor when an ice candidate is sent.
            /// </summary>
            /// <param name="connection">The connection the event was invoked on.</param>
            /// <param name="iceCandidate">The ice candidate that was sent.</param>
            public delegate void OnLocalIceCandidateFoundDelegate(MLWebRTC.PeerConnection connection, MLWebRTC.IceCandidate iceCandidate);

            /// <summary>
            /// Delegate describing the callback necessary to notify that all ICEs have been gathered.
            /// </summary>
            /// <param name="connection">The connection the event was invoked on.</param>
            public delegate void OnIceGatheringCompletedDelegate(MLWebRTC.PeerConnection connection);

            /// <summary>
            /// Event invoked for when an error occurs.
            /// </summary>
            public event OnErrorDelegate OnError = delegate { };

            /// <summary>
            /// Event invoked for when a connection between a local and remote peer is established.
            /// </summary>
            public event OnConnectedDelegate OnConnected = delegate { };

            /// <summary>
            /// Event invoked for when a connection between a local and remote peer is destroyed.
            /// </summary>
            public event OnDisconnectedDelegate OnDisconnected = delegate { };

            /// <summary>
            /// Event invoked for when a track is added to a connection.
            /// </summary>
            public event OnTrackAddedMultipleStreamsDelegate OnTrackAddedMultipleStreams = delegate { };

            /// <summary>
            /// Event invoked for when a track is removed from a connection.
            /// </summary>
            public event OnTrackRemovedMultipleStreamsDelegate OnTrackRemovedMultipleStreams = delegate { };

            /// <summary>
            /// Event invoked for when a data channel is received by a connection.
            /// </summary>
            public event OnDataChannelReceivedDelegate OnDataChannelReceived = delegate { };

            /// <summary>
            /// Event invoked for when an offer is sent.
            /// </summary>
            public event OnLocalOfferCreatedDelegate OnLocalOfferCreated = delegate { };

            /// <summary>
            /// Event invoked for when an answer is sent.
            /// </summary>
            public event OnLocalAnswerCreatedDelegate OnLocalAnswerCreated = delegate { };

            /// <summary>
            /// Event invoked for when an ice candidate is sent.
            /// </summary>
            public event OnLocalIceCandidateFoundDelegate OnLocalIceCandidateFound = delegate { };

            /// <summary>
            /// Event invoked for when ice gathering completed.
            /// </summary>
            public event OnIceGatheringCompletedDelegate OnIceGatheringCompleted = delegate { };

            /// <summary>
            /// Gets the ice servers used for the connection.
            /// </summary>
            public MLWebRTC.IceServer[] IceServers { get; private set; }

            /// <summary>
            /// Gets the ice candidate chosen by the connection.
            /// </summary>
            public MLWebRTC.IceCandidate IceCandidate { get; private set; }

            /// <summary>
            /// Gets the handle of the connection.
            /// </summary>
            internal ulong Handle { get; private set; }

            /// <summary>
            /// Gets the remote streams map of the connection.
            /// </summary>
            internal Dictionary<string, MLWebRTC.MediaStream> RemoteMediaStreams { get => this.remoteMediaStreams; }

            /// <summary>
            /// Gets the local tracks map of the connection.
            /// </summary>
            internal List<MLWebRTC.MediaStream.Track> LocalMediaStreamTracks { get => this.localMediaStreamTracks; }

            /// <summary>
            /// Gets the remote data channels map of the connection.
            /// </summary>
            internal List<MLWebRTC.DataChannel> RemoteDataChannels { get => this.remoteDataChannels; }

            /// <summary>
            /// Gets the local data channels map of the connection.
            /// </summary>
            internal List<MLWebRTC.DataChannel> LocalDataChannels { get => this.localDataChannels; }

            /// <summary>
            /// Creates an initialized PeerConnection object.
            /// </summary>
            /// <param name="iceServers">The ice servers to create the connection with.</param>
            /// <param name="result">The MLResult object of the inner platform call(s).</param>
            /// <returns> An initialized PeerConnection object.</returns>
            public static PeerConnection CreateRemote(MLWebRTC.IceServer[] iceServers, out MLResult result)
            {
                var permResult = MLPermissions.CheckPermission(MLPermission.Internet);
                if (!MLResult.DidNativeCallSucceed(permResult.Result, nameof(CreateRemote)))
                {
                    MLPluginLog.Error($"{nameof(CreateRemote)} requires missing permission {MLPermission.Internet}");
                    result = MLResult.Create(MLResult.Code.PermissionDenied);
                    return null;
                }

                List<MLWebRTC.PeerConnection> connections = MLWebRTC.Instance.connections;

                MLResult.Code resultCode = NativeBindings.CreateRemoteConnection(iceServers, out PeerConnection connection);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.CreateRemoteConnection));
                result = MLResult.Create(resultCode);
                if (result.IsOk)
                {
                    connections.Add(connection);
                }
                return connection;
            }

            /// <summary>
            /// Creates an initialized PeerConnection object with a forward proxy configuration.
            /// </summary>
            /// <param name="iceServers">The ice servers to create the connection with.</param>
            /// <param name="proxyConfig">Configuration for the forward proxy.</param>
            /// <param name="result">The MLResult object of the inner platform call(s).</param>
            /// <returns> An initialized PeerConnection object.</returns>
            public static PeerConnection CreateRemote(MLWebRTC.IceServer[] iceServers, MLWebRTC.ProxyConfig proxyConfig, out MLResult result)
            {
                var permResult = MLPermissions.CheckPermission(MLPermission.Internet);
                if (!MLResult.DidNativeCallSucceed(permResult.Result, nameof(CreateRemote)))
                {
                    MLPluginLog.Error($"{nameof(CreateRemote)} requires missing permission {MLPermission.Internet}");
                    result = MLResult.Create(MLResult.Code.PermissionDenied);
                    return null;
                }

                List<MLWebRTC.PeerConnection> connections = MLWebRTC.Instance.connections;

                MLResult.Code resultCode = NativeBindings.CreateRemoteConnection(iceServers, proxyConfig, out PeerConnection connection);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.CreateRemoteConnection));
                result = MLResult.Create(resultCode);
                if (result.IsOk)
                {
                    connections.Add(connection);
                }
                return connection;
            }

            /// <summary>
            /// Gets if the connection is currently connected or not.
            /// </summary>
            /// <param name="connected">True if connected.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
            /// </returns>
            public MLResult IsConnected(out bool connected)
            {
                if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                {
                    connected = false;
                    return MLResult.Create(MLResult.Code.InvalidParam, "Handle is invalid.");
                }
                bool isConnected = false;
                MLResult.Code resultCode = NativeBindings.MLWebRTCConnectionIsConnected(this.Handle, out isConnected);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCConnectionIsConnected));
                connected = isConnected;
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Gets if the connection has failed or not.
            /// </summary>
            /// <param name="failed">True if connection has failed.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
            /// </returns>
            public MLResult HasFailed(out bool failed)
            {
                if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                {
                    failed = false;
                    return MLResult.Create(MLResult.Code.InvalidParam, "Handle is invalid.");
                }

                bool isFailed = false;
                MLResult.Code resultCode = NativeBindings.MLWebRTCConnectionHasFailed(this.Handle, out isFailed);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCConnectionHasFailed));
                failed = isFailed;
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Creates the offer for the connection.
            /// </summary>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            public MLResult CreateOffer()
            {
                if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Handle is invalid.");
                }
                MLResult.Code resultCode = NativeBindings.MLWebRTCConnectionCreateOffer(this.Handle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCConnectionCreateOffer));
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Sets the remote offer for the connection.
            /// </summary>
            /// <param name="remoteOffer">The offer to set.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
            /// </returns>
            public MLResult SetRemoteOffer(string remoteOffer)
            {
                if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Handle is invalid.");
                }

                MLResult.Code resultCode = NativeBindings.MLWebRTCConnectionSetRemoteOffer(this.Handle, remoteOffer);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCConnectionSetRemoteOffer));
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Sets the remote answer for the connection.
            /// </summary>
            /// <param name="remoteAnswer">The answer to set.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
            /// </returns>
            public MLResult SetRemoteAnswer(string remoteAnswer)
            {
                if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Handle is invalid.");
                }

                MLResult.Code resultCode = NativeBindings.MLWebRTCConnectionSetRemoteAnswer(this.Handle, remoteAnswer);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCConnectionSetRemoteAnswer));
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Adds an ice candidate to the connection.
            /// </summary>
            /// <param name="iceCandidate">The ice candidate to add.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
            /// </returns>
            public MLResult AddRemoteIceCandidate(MLWebRTC.IceCandidate iceCandidate)
            {
                if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Handle is invalid.");
                }

                NativeBindings.MLWebRTCConnectionIceCandidate nativeIceCandidate = new NativeBindings.MLWebRTCConnectionIceCandidate();
                nativeIceCandidate.Data = iceCandidate;

                MLResult.Code resultCode = NativeBindings.MLWebRTCConnectionAddRemoteIceCandidate(this.Handle, in nativeIceCandidate);

                if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCConnectionAddRemoteIceCandidate)))
                {
                    this.IceCandidate = iceCandidate;
                }

                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Destroys the connection.
            /// </summary>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            public MLResult Destroy()
            {
                if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Handle is invalid.");
                }

                MLResult.Code resultCode = MLResult.Code.Ok;

                // Remove local tracks from this connection.
                foreach (MLWebRTC.MediaStream.Track mediaStreamTrack in this.localMediaStreamTracks)
                {
                    // Don't call this.RemoveLocalTrack() because that modifies the list we're iterating on.
                    var result = NativeBindings.MLWebRTCConnectionRemoveLocalSourceTrack(this.Handle, mediaStreamTrack.Handle);
                    MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebRTCConnectionRemoveLocalSourceTrack));
                }

                // Invalidate the handles of any remote sources and remove the media stream id from the set of unique ids.
                foreach (MLWebRTC.MediaStream mediaStream in this.remoteMediaStreams.Values)
                {
                    foreach (MediaStream.Track remoteTrack in mediaStream.Tracks)
                    {
                        remoteTrack.Cleanup();
                    }
                    mediaStream.Tracks.Clear();
                    mediaStream.ParentConnections.Remove(this);

                    foreach (MLWebRTC.MediaStream.Track track in mediaStream.Tracks)
                    {
                        track.Handle = MagicLeapNativeBindings.InvalidHandle;
                    }
                }

                // Destroy local data channels.
                foreach (MLWebRTC.DataChannel dataChannel in this.localDataChannels)
                {
                    dataChannel.Destroy();
                }

                // Destroy this connection.
                resultCode = NativeBindings.MLWebRTCConnectionDestroy(this.Handle);

                this.Handle = MagicLeapNativeBindings.InvalidHandle;
                this.localMediaStreamTracks.Clear();
                this.remoteMediaStreams.Clear();
                this.remoteDataChannels.Clear();
                this.localDataChannels.Clear();
                this.IceServers = null;
                this.gcHandle.Free();
                MLWebRTC.Instance.connections.Remove(this);
                return MLResult.Create(resultCode);
            }


            /// <summary>
            /// Adds a local track to the connection.
            /// </summary>
            /// <param name="trackToAdd">Track to add to the connection.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            public MLResult AddLocalTrack(MLWebRTC.MediaStream.Track trackToAdd)
            {
                if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Handle is invalid.");
                }

                if (trackToAdd == null)
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Track is null.");
                }

                if (this.localMediaStreamTracks.Contains(trackToAdd))
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Connection already has this track.");
                }

                string[] streamIds = new string[trackToAdd.Streams.Count];
                for (int i = 0; i < streamIds.Length; ++i)
                {
                    streamIds[i] = trackToAdd.Streams[i].Id;
                    trackToAdd.Streams[i].ParentConnections.Add(this);
                }

                NativeBindings.MLWebRTCTrackInfo trackInfo = new NativeBindings.MLWebRTCTrackInfo(streamIds, trackToAdd.Id);
                MLResult.Code resultCode = NativeBindings.MLWebRTCConnectionAddLocalSourceTrackEx(Handle, trackToAdd.Handle, ref trackInfo);
                trackInfo.FreeUnmanagedMemory();
                if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCConnectionAddLocalSourceTrackEx)))
                {
                    localMediaStreamTracks.Add(trackToAdd);
                }
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Removes a local track to the connection.
            /// </summary>
            /// <param name="trackToRemove">Track to remove to the connection.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            public MLResult RemoveLocalTrack(MLWebRTC.MediaStream.Track trackToRemove)
            {
                if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Handle is invalid.");
                }

                if (trackToRemove == null)
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Track is null.");
                }

                if (!this.localMediaStreamTracks.Contains(trackToRemove))
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Connection does not have this track.");
                }

                MLResult.Code resultCode = MLWebRTC.PeerConnection.NativeBindings.MLWebRTCConnectionRemoveLocalSourceTrack(this.Handle, trackToRemove.Handle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(MLWebRTC.PeerConnection.NativeBindings.MLWebRTCConnectionRemoveLocalSourceTrack));
                this.localMediaStreamTracks.Remove(trackToRemove);
                foreach (MLWebRTC.MediaStream stream in trackToRemove.Streams)
                {
                    stream.ParentConnections.Remove(this);
                }

                return MLResult.Create(resultCode);
            }

            public bool ContainsTrack(MediaStream.Track track)
            {
                return localMediaStreamTracks != null && localMediaStreamTracks.Contains(track);
            }
        }
    }
}
