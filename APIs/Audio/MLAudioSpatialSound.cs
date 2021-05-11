// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLAudio.cs" company="Magic Leap, Inc">
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
    /// Manages Audio.
    /// </summary>
    public sealed partial class MLAudio
    {
        /// <summary>
        /// Structs and functions releated to spatial audio.
        /// </summary>
        public sealed partial class SpatialSound
        {
            /// <summary>
            /// Properties specifying send levels for a spatial sound.
            /// </summary>
            [Serializable]
            public sealed class SendLevels
            {
                /// <summary>
                /// Volume scale (0-1) for all freqs.
                /// </summary>
                [SerializeField]
                [Range(0.0f, 1.0f)]
                public float Gain;

                /// <summary>
                /// Volume scale (0-1) for low freqs.
                /// </summary>
                [SerializeField]
                [Range(0.0f, 1.0f)]
                public float GainLF;

                /// <summary>
                /// Volume scale (0-1) for mid freqs.
                /// </summary>
                [SerializeField]
                [Range(0.0f, 1.0f)]
                public float GainMF;

                /// <summary>
                /// Volume scale (0-1) for high freqs.
                /// </summary>
                [SerializeField]
                [Range(0.0f, 1.0f)]
                public float GainHF;

                public override string ToString()
                {
                    return $"Gain: {Gain}, GainLF: {GainLF}, GainMF: {GainMF}, GainHF: {GainHF}";
                }
            }

            /// <summary>
            /// Properties specifying the distance response of a spatial sound.
            /// </summary>
            [Serializable]
            public sealed class DistanceProperties
            {
                /// <summary>
                /// Distance where sound is at full volume.
                /// </summary>
                [SerializeField]
                public float MinDistance;

                /// <summary>
                /// Distance beyond which sound gets no quieter.
                /// </summary>
                [SerializeField]
                public float MaxDistance;

                /// <summary>
                /// Modification to real-world distance response.
                /// </summary>
                [SerializeField]
                public float RolloffFactor;

                public override string ToString()
                {
                    return $"MinDistance: {MinDistance}, MaxDistance: {MaxDistance}, RolloffFactor: {RolloffFactor}";
                }
            }

            /// <summary>
            /// Properties specifying the directivity of a spatial sound.
            /// </summary>
            [Serializable]
            public sealed class RadiationProperties
            {
                /// <summary>
                /// Inner cone angle (0-360); radiation unaffected.
                /// </summary>
                [SerializeField]
                [Range(0.0f, 360.0f)]
                public float InnerAngle;

                /// <summary>
                /// Outer cone angle (0-360); directivity at maximum.
                /// </summary>
                [SerializeField]
                [Range(0.0f, 360.0f)]
                public float OuterAngle;

                /// <summary>
                /// Volume scale (0-1) beyond outer cone for all freqs.
                /// </summary>
                [SerializeField]
                [Range(0.0f, 1.0f)]
                public float OuterGain;

                /// <summary>
                /// Volume scale (0-1) beyond outer cone for low freqs.
                /// </summary>
                [SerializeField]
                [Range(0.0f, 1.0f)]
                public float OuterGainLF;

                /// <summary>
                /// Volume scale (0-1) beyond outer cone for mid freqs.
                /// </summary>
                [SerializeField]
                [Range(0.0f, 1.0f)]
                public float OuterGainMF;

                /// <summary>
                /// Volume scale (0-1) beyond outer cone for high freqs.
                /// </summary>
                [SerializeField]
                [Range(0.0f, 1.0f)]
                public float OuterGainHF;

                public override string ToString()
                {
                    return $"InnerAngle: {InnerAngle}, OuterAngle: {OuterAngle}, OuterGain: {OuterGain}, OuterGainLF: {OuterGainLF}, OuterGainMF: {OuterGainMF}, OuterGainHF: {OuterGainHF}";
                }
            }
        }
    }
}
