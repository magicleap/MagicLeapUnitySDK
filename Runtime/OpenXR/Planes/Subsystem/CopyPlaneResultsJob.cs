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
using MagicLeap.OpenXR.Features.Planes;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace MagicLeap.OpenXR.Subsystems
{
    public partial class MLXrPlaneSubsystem
    {
        internal unsafe struct CopyPlaneResultsJob : IJobParallelFor
        {
            private static readonly Quaternion MagicLeapToUnityRotation = Quaternion.AngleAxis(-90f, Vector3.right);

            [ReadOnly] public NativeArray<TrackableId> PlaneTrackableIds;

            [ReadOnly, NativeDisableUnsafePtrRestriction] public XrPlaneDetectorLocation* PlanesIn;

            [WriteOnly] public NativeArray<BoundedPlane> PlanesOut;

            private PlaneAlignment ToUnityAlignment(XrPlaneDetectorOrientation flag)
            {
                return flag switch
                {
                    XrPlaneDetectorOrientation.Vertical => PlaneAlignment.Vertical,
                    XrPlaneDetectorOrientation.HorizontalUpward => PlaneAlignment.HorizontalUp,
                    XrPlaneDetectorOrientation.HorizontalDownward => PlaneAlignment.HorizontalDown,
                    XrPlaneDetectorOrientation.Arbitrary => PlaneAlignment.NotAxisAligned,
                    _ => PlaneAlignment.None
                };
            }

            private static PlaneClassification ToUnityClassification(XrPlaneDetectorSemanticTypes semanticType)
            {
                var result = semanticType switch
                {
                    XrPlaneDetectorSemanticTypes.Ceiling => PlaneClassification.Ceiling,
                    XrPlaneDetectorSemanticTypes.Floor => PlaneClassification.Floor,
                    XrPlaneDetectorSemanticTypes.Wall => PlaneClassification.Wall, 
                    XrPlaneDetectorSemanticTypes.Platform => PlaneClassification.Table,
                    _ => PlaneClassification.None
                };
                return result;
            }

            public void Execute(int index)
            {
                var plane = PlanesIn[index];

                var planePose = plane.Pose;
                var position = planePose.Position;
                var rotation = planePose.Rotation * MagicLeapToUnityRotation;

                PlanesOut[index] = new BoundedPlane(PlaneTrackableIds[index], // trackableId
                    TrackableId.invalidId, // subsumedBy
                    new Pose(position.InvertZ(), rotation.InvertXY()), Vector3.zero, // center
                    plane.Extents, // size
                    ToUnityAlignment(plane.Orientation), // alignment
                    TrackingState.Tracking, // tracking state
                    IntPtr.Zero, // native pointer
                    ToUnityClassification(plane.SemanticType) // classification
                );
            }
        }
    }
}
