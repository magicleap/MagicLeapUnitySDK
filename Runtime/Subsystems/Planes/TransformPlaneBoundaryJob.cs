// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="TransformPlaneBoundaryJob.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2021 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.MagicLeap
{
    public partial class PlanesSubsystem
    {
        internal struct TransformPlaneBoundaryJob : IJobParallelFor
        {
            public Quaternion m_InvRotation;

            public Vector3 m_Position;

            [ReadOnly]
            public NativeArray<Vector3> m_VerticesIn;

            [WriteOnly]
            public NativeArray<Vector2> m_VerticesOut;

            public void Execute(int vertexIndex)
            {
                var rhVertex = m_VerticesIn[vertexIndex];
                var lhVertex = new Vector3(rhVertex.x, rhVertex.y, -rhVertex.z) - m_Position;
                var vertex2d = m_InvRotation * lhVertex;

                m_VerticesOut[vertexIndex] = new Vector2(vertex2d.x, vertex2d.y);
            }
        }
    }
}
