// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="DeviceFeatureUsages.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2021 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine.XR;

namespace UnityEngine.XR.MagicLeap
{
    public static partial class InputSubsystem
    {
        public static partial class Extensions
        {
            public static class DeviceFeatureUsages
            {
                public static class Head
                {
                    public static InputFeatureUsage<float> Confidence = new InputFeatureUsage<float>("MLHeadConfidence");
                    public static InputFeatureUsage<byte[]> TrackingState = new InputFeatureUsage<byte[]>("MLHeadTrackingState");
                    public static InputFeatureUsage<byte[]> TrackingMapEvents = new InputFeatureUsage<byte[]>("MLHeadTrackingMapEvents");
                }

                public static class Eyes
                {
                    public static InputFeatureUsage<byte[]> TrackingState = new InputFeatureUsage<byte[]>("MLEyeTrackingStateEx");
                }

                public static class Controller
                {
                    public static InputFeatureUsage<uint> Type = new InputFeatureUsage<uint>("MLControllerType");
                    public static InputFeatureUsage<uint> DOF = new InputFeatureUsage<uint>("MLControllerDOF");
                    public static InputFeatureUsage<uint> CalibrationAccuracy = new InputFeatureUsage<uint>("MLControllerCalibrationAccuracy");

                    public static InputFeatureUsage<float> Touch1Force = new InputFeatureUsage<float>("MLControllerTouch1Force");
                    public static InputFeatureUsage<float> Touch2Force = new InputFeatureUsage<float>("MLControllerTouch2Force");

                    public static InputFeatureUsage<bool> Secondary2DAxisTouch = new InputFeatureUsage<bool>("Secondary2DAxisTouch");
                    public static InputFeatureUsage<byte[]> State = new InputFeatureUsage<byte[]>("MLInputControllerState");
                }

                public static class Hand
                {
                    public static InputFeatureUsage<float> Confidence = new InputFeatureUsage<float>("MLHandConfidence");

                    public static InputFeatureUsage<Vector3> WristCenter = new InputFeatureUsage<Vector3>("MLHandWristCenter");
                    public static InputFeatureUsage<Vector3> WristUlnar = new InputFeatureUsage<Vector3>("MLHandWristUlnar");
                    public static InputFeatureUsage<Vector3> WristRadial = new InputFeatureUsage<Vector3>("MLHandWristRadial");

                    public static InputFeatureUsage<byte[]> KeyPointsMask = new InputFeatureUsage<byte[]>("MLHandKeyPointsMask");
                }

                public static class HandGesture
                {
                    public static InputFeatureUsage<bool> GesturesEnabled = new InputFeatureUsage<bool>("MLHandGestureEnabled");

                    public static InputFeatureUsage<Vector3> GestureInteractionPosition = new InputFeatureUsage<Vector3>("MLHandGestureInteractionPosition");
                    public static InputFeatureUsage<Vector3> GestureTransformPosition = new InputFeatureUsage<Vector3>("MLHandGestureTransformPosition");

                    public static InputFeatureUsage<Quaternion> GestureInteractionRotation = new InputFeatureUsage<Quaternion>("MLHandGestureInteractionRotation");
                    public static InputFeatureUsage<Quaternion> GestureTransformRotation = new InputFeatureUsage<Quaternion>("MLHandGestureTransformRotation");

                    public static InputFeatureUsage<uint> GesturePosture = new InputFeatureUsage<uint>("MLHandGesturePosture");
                    public static InputFeatureUsage<uint> GestureKeyPose = new InputFeatureUsage<uint>("MLHandGestureKeyPose");

                    public static InputFeatureUsage<byte[]> GestureThumbState = new InputFeatureUsage<byte[]>("MLGestureThumbState");
                    public static InputFeatureUsage<byte[]> GestureIndexState = new InputFeatureUsage<byte[]>("MLGestureIndexState");
                    public static InputFeatureUsage<byte[]> GestureMiddleState = new InputFeatureUsage<byte[]>("MLGestureMiddleState");
                    public static InputFeatureUsage<byte[]> GestureRingState = new InputFeatureUsage<byte[]>("MLGestureRingState");
                    public static InputFeatureUsage<byte[]> GesturePinkyState = new InputFeatureUsage<byte[]>("MLGesturePinkyState");

                    public static InputFeatureUsage<float> AngleThumbIndexDeg = new InputFeatureUsage<float>("MLAngleThumbIndexDeg");
                    public static InputFeatureUsage<float> AngleIndexMiddleDeg = new InputFeatureUsage<float>("MLAngleIndexMiddleDeg");
                    public static InputFeatureUsage<float> AngleMiddleRingDeg = new InputFeatureUsage<float>("MLAngleMiddleRingDeg");
                    public static InputFeatureUsage<float> AngleRingPinkyDeg = new InputFeatureUsage<float>("MLAngleRingPinkyDeg");
                }
            }

        }

    }
}
