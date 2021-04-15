// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCConnection.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;

namespace UnityEngine.XR.MagicLeap
{
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
            /// A delegate that describes the requirements of the OnTrackAddedDelegate callback.
            /// </summary>
            /// <param name="mediaStream">The media stream associated with the added track.</param>
            /// <param name="addedTrack">The track that was added to the connection.</param>
            [Obsolete("OnTrackAddedDelegate is deprecated. Use OnTrackAddedMultipleStreamsDelegate instead.")]
            public delegate void OnTrackAddedDelegate(MLWebRTC.MediaStream mediaStream, MLWebRTC.MediaStream.Track addedTrack);

            /// <summary>
            /// A delegate that describes the requirements of the OnTrackRemovedDelegate callback.
            /// </summary>
            /// <param name="mediaStream">The media stream associated with the removed track.</param>
            /// <param name="removedTrack">The track that was removed from the connection.</param>
            [Obsolete("OnTrackRemovedDelegate is deprecated. Use OnTrackRemovedMultipleStreamsDelegate instead.")]
            public delegate void OnTrackRemovedDelegate(MLWebRTC.MediaStream mediaStream, MLWebRTC.MediaStream.Track removedTrack);


            /// <summary>
            /// Event invoked for when a track is added to a connection.
            /// </summary>
            [Obsolete("OnTrackAdded is deprecated. Use OnTrackAddedMultipleStreams instead.")]
            public event OnTrackAddedDelegate OnTrackAdded = delegate { };

            /// <summary>
            /// Event invoked for when a track is removed from a connection.
            /// </summary>
            [Obsolete("OnTrackRemoved is deprecated. Use OnTrackRemovedMultipleStreams instead.")]
            public event OnTrackRemovedDelegate OnTrackRemoved = delegate { };
        }
    }
}
