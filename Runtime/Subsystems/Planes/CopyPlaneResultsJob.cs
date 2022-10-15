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
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    public partial class PlanesSubsystem
    {
        internal struct CopyPlaneResultsJob : IJobParallelFor
        {
            static readonly Quaternion k_MagicLeapToUnityRotation = Quaternion.AngleAxis(-90f, Vector3.right);

            static readonly Quaternion k_UnityToMagicLeapRotation = Quaternion.Inverse(k_MagicLeapToUnityRotation);

            static public Quaternion TransformMLRotationToUnity(Quaternion rotation)
            {
                return rotation * k_MagicLeapToUnityRotation;
            }

            static public Quaternion TransformUnityRotationToML(Quaternion rotation)
            {
                return rotation * k_UnityToMagicLeapRotation;
            }

            [ReadOnly]
            public NativeArray<TrackableId> planeTrackableIds;

            [ReadOnly]
            public NativeArray<Extensions.MLPlane> planesIn;

            [WriteOnly]
            public NativeArray<BoundedPlane> planesOut;

            PlaneAlignment ToUnityAlignment(Extensions.MLPlanesQueryFlags flags, Quaternion rotation)
            {
                if ((flags & Extensions.MLPlanesQueryFlags.Vertical) != 0)
                {
                    return PlaneAlignment.Vertical;
                }
                else if ((flags & Extensions.MLPlanesQueryFlags.Horizontal) != 0)
                {
                    var normal = rotation * Vector3.up;
                    return (normal.y > 0f) ? PlaneAlignment.HorizontalUp : PlaneAlignment.HorizontalDown;
                }
                else
                {
                    return PlaneAlignment.NotAxisAligned;
                }
            }

            PlaneClassification ToUnityClassification(Extensions.MLPlanesQueryFlags flags)
            {
                if ((flags & Extensions.MLPlanesQueryFlags.Semantic_Ceiling) != 0)
                {
                    return PlaneClassification.Ceiling;
                }
                else if ((flags & Extensions.MLPlanesQueryFlags.Semantic_Floor) != 0)
                {
                    return PlaneClassification.Floor;
                }
                else if ((flags & Extensions.MLPlanesQueryFlags.Semantic_Wall) != 0)
                {
                    return PlaneClassification.Wall;
                }
                else
                {
                    return PlaneClassification.None;
                }
            }

            public void Execute(int index)
            {
                var plane = planesIn[index];

                // we cannot use the MLConvert methods here since they reference camera.main
                // so perform the same computations to convert to unity coordiante space 
                // world scale is not applied as it is handled by the AR Foundation code 
                var position = new Vector3(plane.position.x, plane.position.y, -plane.position.z);
                var rotation = new Quaternion(plane.rotation.x, plane.rotation.y, -plane.rotation.z, -plane.rotation.w);
                rotation = TransformMLRotationToUnity(rotation);

                planesOut[index] = new BoundedPlane
                (
                    planeTrackableIds[index], // trackableId
                    TrackableId.invalidId, // subsumedBy
                    new Pose(position, rotation),
                    Vector3.zero, // center
                    new Vector2(plane.width, plane.height), // size
                    ToUnityAlignment(plane.flags, rotation), // alignment
                    TrackingState.Tracking, // tracking state
                    IntPtr.Zero, // native pointer
                    ToUnityClassification(plane.flags) // classification
                );
            }
        }
    }
}
