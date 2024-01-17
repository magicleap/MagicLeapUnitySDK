// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.InteractionSubsystems;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// MagicLeap implementation of the <c>XRGestureSubsystem</c>. Do not create this directly. Use the <c>SubsystemManager</c> instead.
    /// </summary>
    public partial class GestureSubsystem
    {
        public static partial class Extensions
        {
            /// <summary>
            /// The event data related to a Magic Leap Touchpad gesture
            /// </summary>
            /// <seealso cref="XRGestureSubsystem"/>
            [StructLayout(LayoutKind.Sequential)]
            public struct TouchpadGestureEvent : IEquatable<TouchpadGestureEvent>
            {
                /// <summary>
                /// The <see cref="GestureId"/> associated with this gesture.
                /// </summary>
                public GestureId id { get { return m_Id; } }

                /// <summary>
                /// The <see cref="state"/> of the gesture.
                /// </summary>
                public GestureState state { get { return m_State; } }

                /// <summary>
                /// The controller id associated with this gesture.
                /// </summary>
                public byte controllerId { get { return m_ControllerId; } }

                /// <summary>
                /// Angle from the center of the touchpad to the finger.
                /// </summary>
                public float angle { get { return m_Angle; } }

                /// <summary>
                /// Direction of gesture
                /// </summary>
                public InputSubsystem.Extensions.TouchpadGesture.Direction direction { get { return m_Direction; } }

                /// <summary>
                /// For radial gestures, this is the absolute value of the angle. For scroll and pinch gestures, this is the absolute distance traveled in touchpad distance. The touchpad is defined as having extents of [-1.0,1.0] so this distance has a range of [0.0,2.0].
                /// </summary>
                public float distance { get { return m_Distance; } }

                /// <summary>
                /// Gesture position (x,y) and force (z). Position is in the [-1.0,1.0] range and force is in the [0.0,1.0] range.
                /// </summary>
                public Vector3 positionAndForce { get { return m_PositionAndForce; } }

                /// <summary>
                /// For radial gestures, this is the radius of the gesture. The touchpad is defined as having extents of [-1.0,1.0] so this radius has a range of [0.0,2.0].
                /// </summary>
                public float radius { get { return m_Radius; } }

                /// <summary>
                /// Speed of gesture. Note that this takes on different meanings depending on the gesture type being performed:
                /// </summary>
                public float speed { get { return m_Speed; } }

                /// <summary>
                /// Type of gesture.
                /// </summary>
                public InputSubsystem.Extensions.TouchpadGesture.Type type { get { return m_Type; } }

                /// <summary>
                /// Gets a default-initialized <see cref="TouchpadGestureEvent"/>.
                /// </summary>
                /// <returns>A default <see cref="TouchpadGestureEvent"/>.</returns>
                public static TouchpadGestureEvent GetDefault()
                {
                    return new TouchpadGestureEvent(GestureId.invalidId, GestureState.Invalid,
                        0, 0.0f, InputSubsystem.Extensions.TouchpadGesture.Direction.None, 0.0f, Vector3.zero,
                        0.0f, 0.0f, InputSubsystem.Extensions.TouchpadGesture.Type.None);
                }

                /// <summary>
                /// Constructs a new <see cref="TouchpadGestureEvent"/>.
                /// </summary>
                /// <param name="id">The <see cref="GestureId"/> associated with the gesture.</param>
                /// <param name="state">The <see cref="GestureState"/> associated with the gesture.</param>
                /// <param name="controllerId">The controller id associated with this gesture.</param>
                /// <param name="angle">The angel of the touch of the gesture.</param>
                /// <param name="direction">The direction of the touch of the gesture.</param>
                /// <param name="distance">The distance of the gesture.</param>
                /// <param name="positionAndForce">The <see cref="Vector3"/> position and force of the gesture.</param>
                /// <param name="radius">The radius of the touch of the gesture.</param>
                /// <param name="speed">The speed of the gesture.</param>
                /// <param name="type">The <see cref="MLInput.Controller.TouchpadGesture.GestureType"/> type of the gesture.</param>
                public TouchpadGestureEvent(GestureId id, GestureState state, byte controllerId, float angle,
                    InputSubsystem.Extensions.TouchpadGesture.Direction direction, float distance, Vector3 positionAndForce,
                    float radius, float speed, InputSubsystem.Extensions.TouchpadGesture.Type type)
                {
                    m_Id = id;
                    m_State = state;
                    m_ControllerId = controllerId;
                    m_Angle = angle;
                    m_Direction = direction;
                    m_Distance = distance;
                    m_PositionAndForce = positionAndForce;
                    m_Radius = radius;
                    m_Speed = speed;
                    m_Type = type;
                }

                /// <summary>
                /// Generates a new string describing the gestures's properties suitable for debugging purposes.
                /// </summary>
                /// <returns>A string describing the gestures's properties.</returns>
                public override string ToString()
                {
                    if(MLDevice.IsOpenXRLoaderActive())
                    {
                        return "[Gestures currently unsupported under OpenXR!]";
                    }
                    return string.Format(
                        "Touchpad Gesture:\n\tgestureId: {0}\n\tgestureState: {1}\n\tcontrollerId: {2}\n\tangle: {3}\n\tdirection: {4}\n\tdistance: {5}\\n\tpositionAndForce: {6}\n\tradius: {7}\n\tspeed: {8}\n\ttype: {9}",
                        id, state, controllerId, angle, direction, distance, positionAndForce, radius, speed, type);
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    return obj is TouchpadGestureEvent && Equals((TouchpadGestureEvent)obj);
                }

                public override int GetHashCode()
                {
                    unchecked
                    {
                        var hashCode = m_Id.GetHashCode();
                        hashCode = (hashCode * 486187739) + ((int)m_State).GetHashCode();
                        hashCode = (hashCode * 486187739) + ((int)m_ControllerId).GetHashCode();
                        hashCode = (hashCode * 486187739) + ((int)m_Angle).GetHashCode();
                        hashCode = (hashCode * 486187739) + ((int)m_Direction).GetHashCode();
                        hashCode = (hashCode * 486187739) + ((int)m_Distance).GetHashCode();
                        hashCode = (hashCode * 486187739) + m_PositionAndForce.GetHashCode();
                        hashCode = (hashCode * 486187739) + ((int)m_Radius).GetHashCode();
                        hashCode = (hashCode * 486187739) + ((int)m_Speed).GetHashCode();
                        hashCode = (hashCode * 486187739) + ((int)m_Type).GetHashCode();

                        return hashCode;
                    }
                }

                public static bool operator ==(TouchpadGestureEvent lhs, TouchpadGestureEvent rhs)
                {
                    return lhs.Equals(rhs);
                }

                public static bool operator !=(TouchpadGestureEvent lhs, TouchpadGestureEvent rhs)
                {
                    return !lhs.Equals(rhs);
                }

                public bool Equals(TouchpadGestureEvent other)
                {
                    return
                        m_Id == other.id &&
                        m_State == other.state &&
                        m_ControllerId == other.controllerId &&
                        m_Angle == other.angle &&
                        m_Direction == other.direction &&
                        m_Distance == other.distance &&
                        m_PositionAndForce == other.positionAndForce &&
                        m_Radius == other.radius &&
                        m_Speed == other.speed &&
                        m_Type == other.type;
                }

                GestureId m_Id;
                GestureState m_State;
                byte m_ControllerId;
                float m_Angle;
                InputSubsystem.Extensions.TouchpadGesture.Direction m_Direction;
                float m_Distance;
                Vector3 m_PositionAndForce;
                float m_Radius;
                float m_Speed;
                InputSubsystem.Extensions.TouchpadGesture.Type m_Type;
            }
        }



    }
}
