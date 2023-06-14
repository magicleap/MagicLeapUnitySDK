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
                    [Obsolete]
                    public static InputFeatureUsage<byte[]> TrackingState = new InputFeatureUsage<byte[]>("MLHeadTrackingState");
                    public static InputFeatureUsage<byte[]> TrackingStateEx = new InputFeatureUsage<byte[]>("MLHeadTrackingStateEx");
                    public static InputFeatureUsage<byte[]> TrackingMapEvents = new InputFeatureUsage<byte[]>("MLHeadTrackingMapEvents");
                }

                public static class Eyes
                {
                    public static InputFeatureUsage<byte[]> TrackingState = new InputFeatureUsage<byte[]>("MLEyeTrackingStateEx");
                }

                public static class Controller
                {
                    public static InputFeatureUsage<float> Touch1Force = new InputFeatureUsage<float>("MLControllerTouch1Force");
                    public static InputFeatureUsage<byte[]> State = new InputFeatureUsage<byte[]>("MLInputControllerStateEx");
                }

                public static class Hand
                {
                    public static InputFeatureUsage<float> Confidence = new InputFeatureUsage<float>("MLHandConfidence");
                    public static InputFeatureUsage<Bone> WristCenter = new InputFeatureUsage<Bone>("MLHandWristCenter");
                    public static InputFeatureUsage<Bone> WristUlnar = new InputFeatureUsage<Bone>("MLHandWristUlnar");
                    public static InputFeatureUsage<Bone> WristRadial = new InputFeatureUsage<Bone>("MLHandWristRadial");

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
