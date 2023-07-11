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
    public partial class MLGazeRecognition
    {
        /// <summary>
        /// Static information about the Gaze Recognition system.  Populate with MLGazeRecognitionGetStaticData().
        /// </summary>
        public readonly struct StaticData
        {
            /// <summary>
            /// The maximum value for the height of the eye_left and eye_right vector.
            /// </summary>
            public readonly float EyeHeightMax;

            /// <summary>
            /// The maximum value for the width of the eye_left and eye_right vector.
            /// </summary>
            public readonly float EyeWidthMax;

            /// <summary>
            /// Location of the 3D vergence point, intersection of 3D gaze vectors.
            /// </summary>
            public readonly Pose Vergence;

            public StaticData(Pose pose, float eyeHeightMax, float eyeWidthMax)
            {
                this.Vergence = pose;
                this.EyeHeightMax = eyeHeightMax;
                this.EyeWidthMax = eyeWidthMax;
            }
        };

        /// <summary>
        /// Information about the state of the Gaze Recognition system.
        /// </summary>
        public readonly struct State
        {
            /// <summary>
            /// The timestamp accociated with all data fields in this struct, giving the headset clock.
            /// </summary>
            public readonly MLTime TimeStamp;

            /// <summary>
            /// Represents what gaze Recognition error (if any) is present.
            /// </summary>
            public readonly Error Error;

            /// <summary>
            /// Represents what known gaze Recognition behavior is present.
            /// </summary>
            public readonly Behavior Behavior;

            /// <summary>
            /// A vector for eye-in-skull position of left eye, even if right eye is closed
            /// </summary>
            public readonly Vector2 EyeLeft;

            /// <summary>
            /// A vector for eye-in-skull position of right eye, even if left eye is closed.
            /// </summary>
            public readonly Vector2 EyeRight;

            /// <summary>
            /// Metadata field for onset of the current behavior, in seconds. Onset applies to all behaviors and marks the time when the current behavior began.
            /// </summary>
            public readonly float OnsetS;

            /// <summary>
            /// Metadata field for duration of the current behavior (gaze type), in seconds.  Duration applies to all gaze types
            /// </summary>
            public readonly float DurationS;

            /// <summary>
            /// Metadata field for velocity of the current movement, in degrees per second.  Velocity field applies to saccades and
            /// pursuit, otherwise NaN
            /// </summary>
            public readonly float VelocityDegps;

            /// <summary>
            /// Metadata field for amplitude of the current movement, which is eye-position displacement in degrees of visual angle. 
            /// Amplitude applies to saccades and pursuit, otherwise NaN.
            /// </summary>
            public readonly float AmplitudeDeg;

            /// <summary>
            /// Metadata field for direction of the current movement, in radial degrees (0-360).  Direction field applies to saccades
            /// and pursuit, otherwise NaN
            /// </summary>
            public readonly float DirectionRadial;

            public override string ToString() => $"Behavior: {Behavior}\nTimeStamp: {TimeStamp}\nAmplitudeDeg: {AmplitudeDeg}\nDirectionRadial: {DirectionRadial}\nDurationS: {DurationS}\nEyeLeft: {EyeLeft}\nEyeRight: {EyeRight}\nOnsetS: {OnsetS}\nVelocityDegps: {VelocityDegps}\n";

            internal State(NativeBindings.MLGazeRecognitionState nativeState)
            {
                this.TimeStamp = nativeState.Timestamp;
                this.Error = nativeState.Error;
                this.Behavior = nativeState.Behavior;
                this.EyeLeft = Native.MLConvert.ToUnity(nativeState.EyeLeft);
                this.EyeRight = Native.MLConvert.ToUnity(nativeState.EyeRight);
                this.OnsetS = nativeState.OnsetS;
                this.DurationS = nativeState.DurationS;
                this.VelocityDegps = nativeState.VelocityDegps;
                this.AmplitudeDeg = nativeState.AmplitudeDeg;
                this.DirectionRadial = nativeState.DirectionRadial;
            }
        }
    }
}
