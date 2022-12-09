// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System.Runtime.InteropServices;
    using Native;

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
            public static extern bool CreateOutputBuffer(uint bufferSizeInBytes, uint channels, uint samplesPerSecond, uint bitsPerSample, uint validBitsPerSample, float maxPitch, uint cacheDuration);
        }
    }
}
