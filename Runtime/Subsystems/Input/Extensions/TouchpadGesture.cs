// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    public static partial class InputSubsystem
    {
        public static partial class Extensions
        {
            public static class TouchpadGesture
            {
                /// <summary>
                /// Gesture state. Links to MLInputControllerTouchpadGestureState in ml_input.h.
                /// </summary>
                public enum State : uint
                {
                    /// <summary>
                    /// State: End
                    /// </summary>
                    End = 0,

                    /// <summary>
                    /// State: Continue
                    /// </summary>
                    Continue,

                    /// <summary>
                    /// State: Start
                    /// </summary>
                    Start
                }

                /// <summary>
                /// Recognized touchpad gesture types. Links to MLInputControllerTouchpadGestureType in ml_input.h.
                /// </summary>
                public enum Type : uint
                {
                    /// <summary>
                    /// Type: None
                    /// </summary>
                    None = 0,

                    /// <summary>
                    /// Type: Tap
                    /// </summary>
                    Tap,

                    /// <summary>
                    /// Type: Force Tap Down
                    /// </summary>
                    ForceTapDown,

                    /// <summary>
                    /// Type: Force Tap Up
                    /// </summary>
                    ForceTapUp,

                    /// <summary>
                    /// Type: Force Dwell
                    /// </summary>
                    ForceDwell,

                    /// <summary>
                    /// Type: Second Force Down
                    /// </summary>
                    SecondForceDown,

                    /// <summary>
                    /// Type: Radial Scroll
                    /// </summary>
                    RadialScroll,

                    /// <summary>
                    /// Type: Swipe
                    /// </summary>
                    Swipe,
                }

                /// <summary>
                /// Direction of touchpad gesture. Links to MLInputControllerTouchpadGestureDirection in ml_input.h.
                /// </summary>
                public enum Direction : uint
                {
                    /// <summary>
                    /// Direction: None
                    /// </summary>
                    None = 0,

                    /// <summary>
                    /// Direction: Up
                    /// </summary>
                    Up,

                    /// <summary>
                    /// Direction: Down
                    /// </summary>
                    Down,

                    /// <summary>
                    /// Direction: Left
                    /// </summary>
                    Left,

                    /// <summary>
                    /// Direction: Right
                    /// </summary>
                    Right,

                    /// <summary>
                    /// Direction: Clockwise
                    /// </summary>
                    Clockwise,

                    /// <summary>
                    /// Direction: Counter Clockwise
                    /// </summary>
                    CounterClockwise
                }

                public struct Data
                {
                    public Vector3 PositionAndForce;

                    public Type Type;

                    public Direction Direction;

                    public float Speed;

                    public float Distance;

                    public float Radius;

                    public float Angle;

                    public override string ToString() => $"PositionAndForce: {PositionAndForce},  Type: {Type}, Direction: {Direction}, Speed: {Speed}, Distance: {Distance}, Radius: {Radius}, Angle: {Angle}";
                }
            }
        }
    }
}
