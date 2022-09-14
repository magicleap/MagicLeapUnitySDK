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

    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Class that represents an app defined source that can be used by the MLWebRTC API.
        /// </summary>
        public abstract partial class AppDefinedSource : MediaStream.Track
        {
            /// <summary>
            /// Gets the GCHandle of this managed object.
            /// </summary>
            protected GCHandle gcHandle;

            /// <summary>
            /// Instance method that is called when the source is destroyed via <c>MLWebRTC.MediaStream.Track.DestroyLocal()</c>.
            /// </summary>
            protected abstract void OnSourceDestroy();

            /// <summary>
            /// Instance method that is called when the source is enabled or disabled via <c>MLWebRTC.MediaStream.Track.SetEnabled()</c>
            /// </summary>
            /// <param name="enabled">True if enabled.</param>
            protected abstract void OnSourceSetEnabled(bool enabled);

            public AppDefinedSource(string trackId)
                : base(trackId)
            {}
        }
    }
}
