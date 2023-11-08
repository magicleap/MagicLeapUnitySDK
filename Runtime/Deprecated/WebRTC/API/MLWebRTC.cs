// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;

namespace UnityEngine.XR.MagicLeap
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    
    [Obsolete("MLWebRTC is being deprecated and will be removed from the MagicLeap SDK. Use Unity's native WebRTC instead")]
    public partial class MLWebRTC : MLAutoAPISingleton<MLWebRTC>
    {
        /// <summary>
        /// Contains all the unique Id's of the media streams for this connection.
        /// </summary>
        private HashSet<string> uniqueMediaStreamIds = new HashSet<string>();

        /// <summary>
        /// List of all local tracks created in one session.
        /// </summary>
        private List<MLWebRTC.MediaStream.Track> localTracks = new List<MLWebRTC.MediaStream.Track>();

        /// <summary>
        /// List of  all sinks created in one session.
        /// </summary>
        private List<MLWebRTC.Sink> sinks = new List<MLWebRTC.Sink>();

        /// <summary>
        /// List of  all connections created in one session.
        /// </summary>
        private List<MLWebRTC.PeerConnection> connections = new List<MLWebRTC.PeerConnection>();

        /// <summary>
        /// Calls Disconnect(), destroys the WebRTC sinks and WebRTC instance.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
        /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
        /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
        /// </returns>
        protected override MLResult.Code StopAPI()
        {
            // Destroy connections.
            PeerConnection[] remainingConnections = new PeerConnection[connections.Count];
            // PeerConnection.Destroy() removes the connection from Instance.connections list.
            connections.CopyTo(remainingConnections);
            foreach (PeerConnection connection in remainingConnections)
            {
                connection.Destroy();
            }

            // Destroy sinks.
            Sink[] remainingSinks = new Sink[sinks.Count];
            // Sink.Destroy() removes the sink from Instance.sinks list.
            sinks.CopyTo(remainingSinks);
            foreach (Sink sink in remainingSinks)
            {
                sink.Destroy();
            }

            // Destroy local tracks.
            MediaStream.Track[] remainingTracks = new MediaStream.Track[localTracks.Count];
            // MediaStream.Track.Destroy() removes the track from Instance.localTracks list.
            localTracks.CopyTo(remainingTracks);
            foreach (MediaStream.Track localTracks in remainingTracks)
            {
                localTracks.DestroyLocal();
            }

            Instance.connections.Clear();
            Instance.sinks.Clear();
            Instance.localTracks.Clear();

            MLResult.Code resultCode = NativeBindings.MLWebRTCInstanceDestroy();
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCInstanceDestroy));
            return resultCode;
        }

        /// <summary>
        /// Creates a WebRTC instance.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the MLWebRTC instance was created successfully.
        /// </returns>
        protected override MLResult.Code StartAPI()
        {
            NativeBindings.MLWebRTCDebugUtils debugUtils = NativeBindings.MLWebRTCDebugUtils.Create();
            MLResult.Code resultCode = NativeBindings.MLWebRTCInstanceCreateWithDebugUtils(in debugUtils);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWebRTCInstanceCreateWithDebugUtils));
            return resultCode;
        }

        /// <summary>
        /// Update loop used to process any connection events.
        /// </summary>
        protected override void Update()
        {
            for (int i = 0; i < Instance.connections.Count; ++i)
            {
                MLWebRTC.PeerConnection connection = Instance.connections[i];
                if (MagicLeapNativeBindings.MLHandleIsValid(connection.Handle))
                {
                    // Polls for connection events.
                    MLResult.DidNativeCallSucceed(MLWebRTC.PeerConnection.NativeBindings.MLWebRTCConnectionProcessEvents(connection.Handle), "MLWebRTCConnectionProcessEvents()");
                }
            }
        }
    }
}
