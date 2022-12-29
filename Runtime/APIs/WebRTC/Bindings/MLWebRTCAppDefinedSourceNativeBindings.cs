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
        public partial class AppDefinedSource
        {
            /// <summary>
            /// Native bindings for the MLWebRTC.AppDefinedVideoSource class. 
            /// </summary>
            internal class NativeBindings
            {
                /// <summary>
                /// A delegate that describes the requirements of the OnSetEnabled callback.
                /// </summary>
                /// <param name="enabled">True if the source was enabled.</param>
                /// <param name="context">Pointer to a context object.</param>
                public delegate void OnSetEnabledDelegate([MarshalAs(UnmanagedType.I1)] bool enabled, IntPtr context);

                /// <summary>
                /// A delegate that describes the requirements of the OnDestroyed callback.
                /// </summary>
                /// <param name="context">Pointer to a context object.</param>
                public delegate void OnDestroyedDelegate(IntPtr context);

                /// <summary>
                /// The native representation of the MLWebRTC data channel callback events.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCAppDefinedSourceEventCallbacks
                {
                    /// <summary>
                    /// Version of the struct.
                    /// </summary>
                    public uint Version;

                    /// <summary>
                    /// Version of the struct.
                    /// </summary>
                    public IntPtr Context;

                    /// <summary>
                    /// OnSetEnabled event.
                    /// </summary>
                    public OnSetEnabledDelegate OnSetEnabled;

                    /// <summary>
                    /// OnDestroyed event.
                    /// </summary>
                    public OnDestroyedDelegate OnDestroyed;

                    /// <summary>
                    /// Factory method used to create a new MLWebRTCAppDefinedVideoSourceEventCallbacks object.
                    /// </summary>
                    /// <param name="context">Pointer to the context object to use for the callbacks.</param>
                    /// <returns>An MLWebRTCAppDefinedVideoSourceEventCallbacks object with the given handle.</returns>
                    public static MLWebRTCAppDefinedSourceEventCallbacks Create(IntPtr context, OnSetEnabledDelegate onSetEnabled, OnDestroyedDelegate onDestroyed)
                    {
                        MLWebRTCAppDefinedSourceEventCallbacks callbacks = new MLWebRTCAppDefinedSourceEventCallbacks
                        {
                            Version = 1,
                            OnSetEnabled = onSetEnabled,
                            OnDestroyed = onDestroyed,
                            Context = context
                        };
                        return callbacks;
                    }
                }
            }
        }
    }
}
