// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLAudioOutputPluginNativeBindings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2021 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System.Runtime.InteropServices;
    using Native;

#if UNITY_ANDROID
    public partial class MLAudioPlayback
    {
        public class NativeBindings : MagicLeapNativeBindings
        {
            [DllImport(AudioOutputPluginDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool CreateAudioOutput();

            [DllImport(AudioOutputPluginDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool DestroyAudioOutput();

            [DllImport(AudioOutputPluginDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool OnUnityAudio(float[] data, uint length);

            [DllImport(AudioOutputPluginDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool CreateOutputBuffer(uint bufferSizeInBytes, uint channels, uint samplesPerSecond, uint bitsPerSample, uint validBitsPerSample, float maxPitch);
        }
    }
#endif
}
