using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityEngine.XR.MagicLeap
{
    public partial class InputSubsystem
    {
        public static partial class Extensions
        {
            public static class MLGestureClassification
            {
                public const string LeftGestureInputDeviceName = "MagicLeapHandGesture - Left";
                public const string RightGestureInputDeviceName = "MagicLeapHandGesture - Right";

                public enum FingerType
                {
                    Thumb = 0,
                    Index,
                    Middle,
                    Ring,
                    Pinky
                }

                public enum PostureType
                {
                    None,
                    Pinch,
                    Point,
                    Grasp,
                    Open
                }

                public enum KeyPoseType
                {
                    None,
                    OK,
                    C,
                    Pinch,
                    Finger,
                    L,
                    Thumb,
                    Fist,
                    Open
                }

                /// <summary>
                /// Gesture classification posture related finger data.
                /// 
                /// Pinch values are measured between the finger and thumb.
                /// In case thumb is the finger, values are same with index's values.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct PostureFingerData
                {
                    /// <summary>
                    /// Normalized pinch angle (range 0.0 ~ 1.0). -1.0 if the value is not valid.
                    /// </summary>
                    public float PinchNormalAngle;
                    /// <summary>
                    /// Absolute pinch angle (angular) in degrees. -1.0 if the value is not valid.
                    /// </summary>
                    public float PinchAngleOfSeparationDeg;
                    /// <summary>
                    /// Normalized pinch distance (range 0.0 ~ 1.0). -1.0 if the value is not valid.
                    /// </summary>
                    public float PinchNormalizedDistance;
                    /// <summary>
                    /// Absolute pinch distance (euclidean) in meters. -1.0 if the value is not valid.
                    /// </summary>
                    public float PinchDistanceM;
                }

                /// <summary>
                /// Gesture classification state of a single finger.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct FingerState
                {
                    /// <summary>
                    /// State of finger.
                    /// </summary>
                    [MarshalAs(UnmanagedType.I1)]
                    public bool IsExtended;
                    /// <summary>
                    /// Finger length in meters.
                    /// </summary>
                    public float LengthM;
                    /// <summary>
                    /// State of finger extension range 0.0 to 1.0. (0.0 folded, 1.0 fully extended)
                    /// </summary>
                    public float NormalizedExtension;
                    /// <summary>
                    /// Absolute angle to palm (angular). -1.0 if the value is not valid.
                    /// </summary>
                    public float AngleToPalm;
                    /// <summary>
                    /// Gesture classification posture related finger data.
                    /// </summary>
                    public PostureFingerData PostureData;
                }

                /// <summary>
                /// The Angles between two fingers in degrees.
                /// </summary>
                public struct FingerAngleDeg
                {
                    /// <summary>
                    /// Angle between thumb and index (angular) in degrees.
                    /// </summary>
                    public float ThumbIndexDeg;
                    /// <summary>
                    /// Angle between index and middle (angular) in degrees.
                    /// </summary>
                    public float IndexMiddleDeg;
                    /// <summary>
                    /// Angle between middle and ring (angular) in degrees.
                    /// </summary>
                    public float MiddleRingDeg;
                    /// <summary>
                    /// Angle between ring and pinky (angular) in degrees.
                    /// </summary>
                    public float RingPinkyDeg;
                }

                public static void StartTracking()
                {
                    MagicLeapXrProviderNativeBindings.StartGestureTracking();
                }

                public static void StopTracking()
                {
                    MagicLeapXrProviderNativeBindings.StopGestureTracking();
                }

                public static bool TryGetFingerState(InputDevice handDevice, FingerType finger, out FingerState fingerStateData) => NativeBindings.TryGetFingerState(handDevice, finger, out fingerStateData);

                public static bool TryGetFingerAngles(InputDevice handDevice, out FingerAngleDeg allFingerAngles) => NativeBindings.TryGetFingerAngles(handDevice, out allFingerAngles);

                public static bool TryGetHandPosture(InputDevice handDevice, out PostureType Posture) => NativeBindings.TryGetHandPosture(handDevice, out Posture);

                public static bool TryGetHandKeyPose(InputDevice handDevice, out KeyPoseType KeyPose) => NativeBindings.TryGetHandKeyPose(handDevice, out KeyPose);

                internal static class NativeBindings
                {
                    public static bool TryGetFingerState(InputDevice handDevice, FingerType finger, out FingerState fingerStateData)
                    {
                        byte[] newFingerData = new byte[Marshal.SizeOf<FingerState>()];
                        fingerStateData = new FingerState();

                        switch (finger)
                        {
                            case FingerType.Thumb:
                                if (!handDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.HandGesture.GestureThumbState, newFingerData))
                                    return false;
                                break;
                            case FingerType.Index:
                                if (!handDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.HandGesture.GestureIndexState, newFingerData))
                                    return false;
                                break;
                            case FingerType.Middle:
                                if (!handDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.HandGesture.GestureMiddleState, newFingerData))
                                    return false;
                                break;
                            case FingerType.Ring:
                                if (!handDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.HandGesture.GestureRingState, newFingerData))
                                    return false;
                                break;
                            case FingerType.Pinky:
                                if (!handDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.HandGesture.GesturePinkyState, newFingerData))
                                    return false;
                                break;
                        }

                        try
                        {
                            GCHandle handle = GCHandle.Alloc(newFingerData, GCHandleType.Pinned);
                            var nativeStruct = Marshal.PtrToStructure<FingerState>(handle.AddrOfPinnedObject());
                            handle.Free();
                            fingerStateData = nativeStruct;
                            return true;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("TryGetGestureFingerState failed with the exception: " + e);
                            return false;
                        }
                    }

                    public static bool TryGetFingerAngles(InputDevice handDevice, out FingerAngleDeg allFingerAngles)
                    {
                        allFingerAngles = new FingerAngleDeg();
                        if (!handDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.HandGesture.AngleThumbIndexDeg, out float TID))
                            return false;
                        if (!handDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.HandGesture.AngleIndexMiddleDeg, out float IMD))
                            return false;
                        if (!handDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.HandGesture.AngleMiddleRingDeg, out float MRD))
                            return false;
                        if (!handDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.HandGesture.AngleRingPinkyDeg, out float RPD))
                            return false;

                        allFingerAngles.ThumbIndexDeg = TID;
                        allFingerAngles.IndexMiddleDeg = IMD;
                        allFingerAngles.MiddleRingDeg = MRD;
                        allFingerAngles.RingPinkyDeg = RPD;
                        return true;
                    }

                    public static bool TryGetHandPosture(InputDevice handDevice, out PostureType Posture)
                    {
                        if (!handDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.HandGesture.GesturePosture, out uint PostureInt))
                        {
                            Posture = PostureType.None;
                            return false;
                        }

                        Posture = (PostureType)PostureInt;
                        return true;
                    }

                    public static bool TryGetHandKeyPose(InputDevice handDevice, out KeyPoseType KeyPose)
                    {
                        if (!handDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.HandGesture.GestureKeyPose, out uint KeyPoseInt))
                        {
                            KeyPose = KeyPoseType.None;
                            return false;
                        }

                        KeyPose = (KeyPoseType)KeyPoseInt;
                        return true;
                    }

                }
            }
        }
    }
}
