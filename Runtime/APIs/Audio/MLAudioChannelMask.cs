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
    public sealed partial class MLAudioOutput
    {
        /// <summary>
        /// Audio output channels mask for track type.
        /// Bits order matches the C API. 
        /// These masks are an ORed composite of individual channel masks.
        /// For example MLAudioOutput.ChannelLayout.Stereo is composed of MLAudioOutput.ChannelMask.FrontLeft and MLAudioOutput.ChannelMask.FrontRight.
        /// Reference:
        /// - https://android.googlesource.com/platform/frameworks/base/+/b267554/media/java/android/media/AudioFormat.java
        /// - https://trac.ffmpeg.org/wiki/AudioChannelManipulation#Listchannelnamesandstandardchannellayouts
        /// </summary>
        [Flags]
        public enum ChannelMask
        {
            None = 0,
            FrontLeft = 0x1,
            FrontRight = 0x2,
            FrontCenter = 0x4,
            LowFrequencyEffects = 0x8,
            BackLeft = 0x10,
            BackRight = 0x20,
            FrontLeftOfCenter = 0x40,
            FrontRightOfCenter = 0x80,
            BackCenter = 0x100,
            SideLeft = 0x200,
            SideRight = 0x400,
            TopCenter = 0x800,
            TopFrontLeft = 0x1000,
            TopFrontCenter = 0x2000,
            TopFrontRight = 0x4000,
            TopBackLeft = 0x8000,
            TopBackCenter = 0x10000,
            TopBackRight = 0x20000,
        }

        /// <summary>
        /// Definition of some of the known Channel Layouts. These layouts are based on <seealso cref="ChannelMask"/>
        /// composite of individual channel masks.
        /// </summary>
        public enum ChannelLayouts
        {
            None = ChannelMask.None,

            /// <summary>
            /// Mask shortcut for Mono audio output.
            /// </summary>
            Mono = ChannelMask.FrontLeft,

            /// <summary>
            /// Mask shortcut for Stereo audio output.
            /// </summary>
            Stereo = ChannelMask.FrontLeft | ChannelMask.FrontRight,

            /// <summary>
            /// Mask shortcut for Stereo and Front Center audio output.
            /// </summary>
            StereoFrontCenter = Stereo | ChannelMask.FrontCenter,

            /// <summary>
            /// Mask shortcut for Quad audio output.
            /// </summary>
            Quad = ChannelMask.FrontLeft | ChannelMask.FrontRight |
                   ChannelMask.BackLeft | ChannelMask.BackRight,

            /// <summary>
            /// Mask shortcut for Quad and Front Center audio output.
            /// </summary>
            QuadFrontCenter = Quad | ChannelMask.FrontCenter,

            /// <summary>
            /// Mask shortcut for surround audio output.
            /// </summary>
            Surround = ChannelMask.FrontLeft | ChannelMask.FrontRight | ChannelMask.FrontCenter
                       | ChannelMask.BackCenter,

            /// <summary>
            /// Mask shortcut for 5.1 audio output.
            /// </summary>
            FivePointOne = ChannelMask.FrontLeft | ChannelMask.FrontRight | ChannelMask.FrontCenter
                           | ChannelMask.BackLeft | ChannelMask.BackRight
                           | ChannelMask.LowFrequencyEffects,

            /// <summary>
            /// Mask shortcut for 5.1 and Back Center audio output.
            /// </summary>
            FivePointOneBackCenter = FivePointOne | ChannelMask.BackCenter,

            /// <summary>
            /// Mask shortcut for 7.1 audio output.
            /// </summary>
            SevenPointOne = FivePointOne | ChannelMask.FrontLeftOfCenter | ChannelMask.FrontRightOfCenter,

            /// <summary>
            /// Mask shortcut for 7.1 surround audio output.
            /// </summary>
            SevenPointOneSurround = FivePointOne | ChannelMask.SideLeft | ChannelMask.SideRight,
        }
    }
}
