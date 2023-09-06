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
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MLXrPlaneSubsystem
    {
        internal unsafe struct CopyPlaneResultsJob : IJobParallelFor
        {
            private static readonly Quaternion MagicLeapToUnityRotation = Quaternion.AngleAxis(-90f, Vector3.right);

            [ReadOnly] public NativeArray<TrackableId> PlaneTrackableIds;

            [ReadOnly] public XrTypes.MLXrPlaneDetectorLocation* PlanesIn;

            [WriteOnly] public NativeArray<BoundedPlane> PlanesOut;

            private PlaneAlignment ToUnityAlignment(XrTypes.MLXrPlaneDetectorOrientation flag)
            {
                return flag switch
                {
                    XrTypes.MLXrPlaneDetectorOrientation.Vertical => PlaneAlignment.Vertical,
                    XrTypes.MLXrPlaneDetectorOrientation.HorizontalUpward => PlaneAlignment.HorizontalUp,
                    XrTypes.MLXrPlaneDetectorOrientation.HorizontalDownward => PlaneAlignment.HorizontalDown,
                    XrTypes.MLXrPlaneDetectorOrientation.Arbitrary => PlaneAlignment.NotAxisAligned,
                    _ => PlaneAlignment.None
                };
            }

            private static PlaneClassification ToUnityClassification(XrTypes.MLXrPlaneDetectorSemanticType semanticType)
            {
                var result = semanticType switch
                {
                    XrTypes.MLXrPlaneDetectorSemanticType.Ceiling => PlaneClassification.Ceiling,
                    XrTypes.MLXrPlaneDetectorSemanticType.Floor => PlaneClassification.Floor,
                    XrTypes.MLXrPlaneDetectorSemanticType.Wall => PlaneClassification.Wall,
                    XrTypes.MLXrPlaneDetectorSemanticType.Platform => PlaneClassification.Table,
                    _ => PlaneClassification.None
                };
                return result;
            }

            public void Execute(int index)
            {
                var plane = PlanesIn[index];

                var planePose = plane.pose;
                var position = planePose.position;
                var rotation = planePose.rotation * MagicLeapToUnityRotation;

                PlanesOut[index] = new BoundedPlane(PlaneTrackableIds[index], // trackableId
                    TrackableId.invalidId, // subsumedBy
                    new Pose(position, rotation), Vector3.zero, // center
                    new Vector2(plane.extents.x, plane.extents.y), // size
                    ToUnityAlignment(plane.orientation), // alignment
                    TrackingState.Tracking, // tracking state
                    IntPtr.Zero, // native pointer
                    ToUnityClassification(plane.semanticType) // classification
                );
            }
        }
    }
}
