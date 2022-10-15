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
    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Class that represents an ice candidate used by the MLWebRTC API.
        /// </summary>
        public partial struct IceCandidate
        {
            /// <summary>
            /// Gets the candidate id.
            /// </summary>
            public string Candidate { get; private set; }

            /// <summary>
            /// Gets the id of the source media component from which the candidate draws data.
            /// </summary>
            public string SdpMid { get; private set; }

            /// <summary>
            /// Gets the index that indicates which media source is associated with the candidate.
            /// </summary>
            public int SdpMLineIndex { get; private set; }

            /// <summary>
            /// Factory method used to create a new IceCandidate object.
            /// </summary>
            /// <param name="candidate">The candidate id.</param>
            /// <param name="sdpMid">The id of the source media component from which the candidate draws data.</param>
            /// <param name="sdpMLineIndex">Index that indicates which media source is associated with a candidate.</param>
            /// <returns>An ice candidate object with the given handle.</returns>
            public static IceCandidate Create(string candidate = "", string sdpMid = "", int sdpMLineIndex = 0)
            {
                IceCandidate iceCandidate = new IceCandidate()
                {
                    Candidate = candidate,
                    SdpMid = sdpMid,
                    SdpMLineIndex = sdpMLineIndex,
                };

                return iceCandidate;
            }
        }
    }
}
