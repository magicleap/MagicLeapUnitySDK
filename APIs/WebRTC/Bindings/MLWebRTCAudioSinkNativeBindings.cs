// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCAudioSinkNativeBindings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
#if PLATFORM_LUMIN
    using UnityEngine.XR.MagicLeap.Native;
#endif

    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Class that represents an audio sink used by the MLWebRTC API.
        /// </summary>
        public partial class AudioSink
        {
            /// <summary>
            /// Native bindings for the MLWebRTC.AudioSink class. 
            /// </summary>
            internal class NativeBindings
            {
                /// <summary>
                /// Creates an audio sink.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink to return to the caller.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the audio sink was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkCreate(out ulong sinkHandle);

                /// <summary>
                /// Creates an audio sink with the provided params.
                /// </summary>
                /// <param name="sinkParams">Parameters to use to create the audio sink.</param>
                /// <param name="sinkHandle">The handle to the audio sink to return to the caller.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the audio sink was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkCreateEx([In] ref MLWebRTCAudioSinkParams sinkParams, out ulong sinkHandle);

                /// <summary>
                /// Sets the source of an audio sink.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink.</param>
                /// <param name="sourceHandle">The handle to the source to set onto the audio sink.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkSetSource(ulong sinkHandle, ulong sourceHandle);

#if PLATFORM_LUMIN
                /// <summary>
                /// Sets the world position of an audio sink for <c>spatialized</c> audio.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink.</param>
                /// <param name="position">The position to set the audio sink to.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkSetPosition(ulong sinkHandle, in MagicLeapNativeBindings.MLVec3f position);
#endif

                /// <summary>
                /// Sets the world position of an audio sink channel.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink.</param>
                /// <param name="position">The position & channel to set the audio sink spatialization for.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if spatialization settings were set successfully.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an incorrect param was sent.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkSetPositionEx(ulong sinkHandle, [In] ref MLWebRTCAudioSinkPosition position);

                /// <summary>
                /// Sets the volume of an audio sink.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink.</param>
                /// <param name="volume">The sink volume.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the volume was set successfully.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an incorrect param was sent.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkSetSoundVolume(ulong sinkHandle, float volume);

                /// <summary>
                /// Sets the world orientation of an audio sink channel.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink.</param>
                /// <param name="orientation">The orientation & channel to set the audio sink spatialization for.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if spatialization settings were set successfully.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an incorrect param was sent.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkSetSoundOrientation(ulong sinkHandle, [In] ref MLWebRTCAudioSinkOrientation orientation);

                /// <summary>
                /// Sets the direct spatial sound send levels of an audio sink channel.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink.</param>
                /// <param name="soundLevels">The sound levels & channel to set the audio sink spatialization for.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if spatialization settings were set successfully.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an incorrect param was sent.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkSetDirectSoundLevels(ulong sinkHandle, [In] ref MLWebRTCAudioSinkSoundLevels soundLevels);

                /// <summary>
                /// Sets the room spatial sound send levels of an audio sink channel.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink.</param>
                /// <param name="soundLevels">The sound levels & channel to set the audio sink spatialization for.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if spatialization settings were set successfully.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an incorrect param was sent.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkSetRoomSoundLevels(ulong sinkHandle, [In] ref MLWebRTCAudioSinkSoundLevels soundLevels);

                /// <summary>
                /// Sets the distance properties of an audio sink channel.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink.</param>
                /// <param name="soundDistance">The sound distance & channel to set the audio sink spatialization for.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if spatialization settings were set successfully.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an incorrect param was sent.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkSetSoundDistanceProperties(ulong sinkHandle, [In] ref MLWebRTCAudioSinkSoundDistance soundDistance);

                /// <summary>
                /// Sets the radiation properties of an audio sink channel.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink.</param>
                /// <param name="soundRadiation">The sound radiation & channel to set the audio sink spatialization for.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if spatialization settings were set successfully.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an incorrect param was sent.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkSetSoundRadiationProperties(ulong sinkHandle, [In] ref MLWebRTCAudioSinkSoundRadiation soundRadiation);

                /// <summary>
                /// Resets the world position of an audio sink for <c>spatialized</c> audio.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkResetPosition(ulong sinkHandle);

                /// <summary>
                /// Sets the number of milliseconds of audio that should be cached in the buffers before dropping the packets.
                /// Dictates the audio latency when app recovers from lifecycle state transitions like standby & reality.
                /// Default is 200ms.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink.</param>
                /// <param name="millisecondsToCache">How many milliseconds worth of audio to cache.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkSetCacheSize(ulong sinkHandle, uint millisecondsToCache);

                /// <summary>
                /// Destroys an audio sink.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkDestroy(ulong sinkHandle);

#if PLATFORM_LUMIN
                private delegate void OnAudioSinkDataAvailableDelegate(in MLAudio.NativeBindings.MLAudioBuffer audioBuffer, in MLAudio.NativeBindings.MLAudioBufferFormat audioBufferFormat, IntPtr userContext);

                [AOT.MonoPInvokeCallback(typeof(OnAudioSinkDataAvailableDelegate))]
                private static void OnAudioSinkDataAvailable(in MLAudio.NativeBindings.MLAudioBuffer audioBuffer, in MLAudio.NativeBindings.MLAudioBufferFormat audioBufferFormat, IntPtr userContext)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(userContext);
                    AudioSink audioSink = gcHandle.Target as AudioSink;

                    if (audioSink == null)
                    {
                        return;
                    }

                    MLAudio.Buffer buffer = new MLAudio.Buffer(audioBuffer, audioBufferFormat, audioSink.CopyRawAudioDataToManagedMemory || (audioSink.OnAudioDataAvailable != null));

                    if (audioSink.OnAudioDataAvailable_NativeCallbackThread != null)
                    {
                        audioSink.OnAudioDataAvailable_NativeCallbackThread.Invoke(buffer);
                    }

                    MLThreadDispatch.ScheduleMain(() =>
                    {
                        audioSink.OnAudioDataAvailable?.Invoke(buffer);
                    });
                }
#endif

                private delegate void OnAudioServiceStatusDelegate(ServiceStatus status, IntPtr userContext);

                [AOT.MonoPInvokeCallback(typeof(OnAudioServiceStatusChangedDelegate))]
                private static void OnAudioServiceStatus(ServiceStatus status, IntPtr userContext)
                {
#if PLATFORM_LUMIN
                    GCHandle gcHandle = GCHandle.FromIntPtr(userContext);
                    AudioSink audioSink = gcHandle.Target as AudioSink;

                    if (audioSink == null)
                    {
                        return;
                    }

                    MLThreadDispatch.ScheduleMain(() =>
                    {
                        audioSink.CurrentServiceStatus = status;
                        audioSink.OnAudioServiceStatusChanged?.Invoke(status);
                    });
#endif
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCAudioSinkParams
                {
                    private readonly uint version;
                    private readonly AudioSink.BufferNotifyMode mode;
                    private readonly IntPtr userContext;
#if PLATFORM_LUMIN
                    private readonly OnAudioSinkDataAvailableDelegate audioSinkCallback;
#endif
                    private readonly OnAudioServiceStatusDelegate audioServiceStatusCallback;

                    public MLWebRTCAudioSinkParams(AudioSink audioSink)
                    {
                        this.version = 1;
                        this.mode = audioSink.Mode;
#if PLATFORM_LUMIN
                        if (audioSink.Mode != BufferNotifyMode.None)
                        {
                            this.audioSinkCallback = OnAudioSinkDataAvailable;
                        }
                        else
                        {
                            this.audioSinkCallback = null;
                        }
#endif
                        this.audioServiceStatusCallback = OnAudioServiceStatus;
                        this.userContext = GCHandle.ToIntPtr(audioSink.gcHandle);
                    }
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCAudioSinkPosition
                {
                    /// <summary>
                    /// Struct version
                    /// </summary>
                    private uint version;

                    /// <summary>
                    /// Ptr to position (MLVec3f) of the audio channel
                    /// </summary>
                    private IntPtr positionPtr;

                    /// <summary>
                    /// Channel to set this position for. Set to -1 to set for all avaialble channels.
                    /// </summary>
                    private int channel;

                    public void Update(Vector3 position, int channel = -1)
                    {
#if PLATFORM_LUMIN
                        if (this.positionPtr == IntPtr.Zero)
                        {
                            this.version = 1;
                            this.positionPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MagicLeapNativeBindings.MLVec3f)));
                        }
                        Marshal.StructureToPtr(MLConvert.FromUnity(position), this.positionPtr, false);
                        this.channel = channel;
#endif
                    }

                    public void FreeUnmanagedMemory()
                    {
                        if (positionPtr != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(positionPtr);
                            positionPtr = IntPtr.Zero;
                        }
                    }
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCAudioSinkOrientation
                {
                    /// <summary>
                    /// Struct version
                    /// </summary>
                    private uint version;

                    /// <summary>
                    /// Ptr to orientation (MLQuatf) of the audio channel
                    /// </summary>
                    private IntPtr orientationPtr;

                    /// <summary>
                    /// Channel to set this position for. Set to -1 to set for all avaialble channels.
                    /// </summary>
                    private int channel;

                    public void Update(Quaternion orientation, int channel = -1)
                    {
#if PLATFORM_LUMIN
                        if (this.orientationPtr == IntPtr.Zero)
                        {
                            this.version = 1;
                            this.orientationPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MagicLeapNativeBindings.MLQuaternionf)));
                        }
                        Marshal.StructureToPtr(MLConvert.FromUnity(orientation), this.orientationPtr, false);
                        this.channel = channel;
#endif
                    }

                    public void FreeUnmanagedMemory()
                    {
                        if (orientationPtr != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(orientationPtr);
                            orientationPtr = IntPtr.Zero;
                        }
                    }
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCAudioSinkSoundLevels
                {
                    private uint version;

                    private IntPtr sendLevelsPtr;

                    private int channel;

                    public void Update(MLAudio.SpatialSound.SendLevels sendLevels, int channel = -1)
                    {
                        if (this.sendLevelsPtr == IntPtr.Zero)
                        {
                            this.version = 1;
                            this.sendLevelsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MLAudio.NativeBindings.MLAudioSpatialSoundSendLevels)));
                        }
                        Marshal.StructureToPtr(new MLAudio.NativeBindings.MLAudioSpatialSoundSendLevels(sendLevels), this.sendLevelsPtr, false);
                        this.channel = channel;
                    }

                    public void FreeUnmanagedMemory()
                    {
                        if (sendLevelsPtr != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(sendLevelsPtr);
                            sendLevelsPtr = IntPtr.Zero;
                        }
                    }
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCAudioSinkSoundDistance
                {
                    private uint version;

                    private IntPtr distancePropertiesPtr;

                    private int channel;

                    public void Update(MLAudio.SpatialSound.DistanceProperties distanceProperties, int channel = -1)
                    {
                        if (this.distancePropertiesPtr == IntPtr.Zero)
                        {
                            this.version = 1;
                            this.distancePropertiesPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MLAudio.NativeBindings.MLAudioSpatialSoundDistanceProperties)));
                        }
                        Marshal.StructureToPtr(new MLAudio.NativeBindings.MLAudioSpatialSoundDistanceProperties(distanceProperties), this.distancePropertiesPtr, false);
                        this.channel = channel;
                    }

                    public void FreeUnmanagedMemory()
                    {
                        if (distancePropertiesPtr != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(distancePropertiesPtr);
                            distancePropertiesPtr = IntPtr.Zero;
                        }
                    }
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCAudioSinkSoundRadiation
                {
                    private uint version;

                    private IntPtr radiationPropertiesPtr;

                    private int channel;

                    public void Update(MLAudio.SpatialSound.RadiationProperties radiationProperties, int channel = -1)
                    {
                        if (this.radiationPropertiesPtr == IntPtr.Zero)
                        {
                            this.version = 1;
                            this.radiationPropertiesPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MLAudio.NativeBindings.MLAudioSpatialSoundRadiationProperties)));
                        }
                        Marshal.StructureToPtr(new MLAudio.NativeBindings.MLAudioSpatialSoundRadiationProperties(radiationProperties), this.radiationPropertiesPtr, false);
                        this.channel = channel;
                    }

                    public void FreeUnmanagedMemory()
                    {
                        if (radiationPropertiesPtr != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(radiationPropertiesPtr);
                            radiationPropertiesPtr = IntPtr.Zero;
                        }
                    }
                }
            }
        }
    }
}
