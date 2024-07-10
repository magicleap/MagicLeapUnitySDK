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
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace MagicLeap.OpenXR.Subsystems
{
    public partial class MLXrPlaneSubsystem
    {
        /// <summary>
        ///     Container for the boundary of a detected planar surface. This is specific
        ///     to Magic Leap because the polygon describing the boundary may be concave,
        ///     and may contain holes.
        /// </summary>
        public class PlaneBoundary
        {
            /// <summary>
            ///     Whether this <see cref="PlaneBoundary" /> is valid. You should check
            ///     for validity before invoking 
            ///     <see cref="GetPolygon(Allocator)" /> <see cref="GetHole(int, Allocator)" />, or
            /// </summary>
            private bool Valid => Polygon.VertexCountOutput > 0;
            
            private static void AssignAtomicSafetyHandle<T>(ref NativeArray<T> array, Allocator allocator) where T : struct
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                var safetyHandle = allocator == Allocator.Temp ? AtomicSafetyHandle.GetTempUnsafePtrSliceHandle() : AtomicSafetyHandle.Create();
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref array, safetyHandle);
#endif
            }

            /// <summary>
            ///     Gets the polygon representing a plane's boundary, and, if successful, copies it to <paramref name="polygonOut" />.
            ///     <paramref name="polygonOut" /> is resized or created using <paramref cref="allocator" /> if necessary.
            ///     The 2D vertices are in plane-space.
            /// </summary>
            /// <param name="allocator">
            ///     The Allocator to use if <paramref name="polygonOut" /> must be recreated.
            ///     Must be <c>Allocator.TempJob</c> or <c>Allocator.Persistent</c>.
            /// </param>
            /// <param name="polygonOut">
            ///     A NativeArray to fill with boundary points. If the array is not the correct size, it is
            ///     disposed and recreated.
            /// </param>
            /// <exception cref="System.InvalidOperationException">Thrown if <see cref="Valid" /> is <c>false</c>.</exception>
            /// <exception cref="System.InvalidOperationException">
            ///     Thrown if <paramref name="allocator" /> is <c>Allocator.Temp</c> or
            ///     <c>Allocator.None</c>.
            /// </exception>
            private unsafe void GetPolygon(Allocator allocator, ref NativeArray<Vector2> polygonOut)
            {
                if (!Valid)
                {
                    throw new InvalidOperationException("This plane boundary is not valid.");
                }

                CreateOrResizeNativeArrayIfNecessary(PolygonVertexCount, allocator, ref polygonOut);
                polygonOut = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector2>(Polygon.Vertices, PolygonVertexCount, allocator);
                AssignAtomicSafetyHandle(ref polygonOut, allocator);
            }

            /// <summary>
            ///     The number of vertices in this boundary's polygon.
            /// </summary>
            public int PolygonVertexCount => (int)Polygon.VertexCountOutput;

            /// <summary>
            ///     Gets the polygon representing this boundary. The 2D vertices are in plane-space.
            /// </summary>
            /// <param name="allocator">
            ///     The allocator to use for the returned NativeArray. Must be <c>Allocator.TempJob</c> or
            ///     <c>Allocator.Persistent</c>.
            /// </param>
            /// <returns>
            ///     A new NativeArray containing a set of 2D points in plane-space representing a boundary for a plane.
            ///     The caller is responsible for disposing the NativeArray.
            /// </returns>
            /// <exception cref="System.InvalidOperationException">Thrown if <see cref="Valid" /> is <c>false</c>.</exception>
            /// <exception cref="System.InvalidOperationException">
            ///     Thrown if <paramref name="allocator" /> is <c>Allocator.Temp</c> or
            ///     <c>Allocator.None</c>.
            /// </exception>
            internal NativeArray<Vector2> GetPolygon(Allocator allocator)
            {
                var polygon = new NativeArray<Vector2>();
                GetPolygon(allocator, ref polygon);
                return polygon;
            }

            /// <summary>
            ///     The number of holes in this boundary.
            /// </summary>
            private int HoleCount => Holes.Length;

            /// <summary>
            ///     Get the polygon representing a hole in this boundary. The 2D vertices are in plane-space.
            /// </summary>
            /// <param name="index">The index of the hole. Must be less than <see cref="HoleCount" />.</param>
            /// <param name="allocator">
            ///     The allocator to use for the returned NativeArray.
            ///     Must be <c>Allocator.TempJob</c> or <c>Allocator.Persistent</c>.
            /// </param>
            /// <returns>
            ///     A new NativeArray allocated with <paramref name="allocator" /> containing a set of 2D vertices
            ///     in plane-space describing the hole at <paramref name="index" />.
            /// </returns>
            /// <exception cref="System.InvalidOperationException">Thrown if <see cref="Valid" /> is false.</exception>
            /// <exception cref="System.InvalidOperationException">
            ///     Thrown if <paramref name="allocator" /> is <c>Allocator.Temp</c> or
            ///     <c>Allocator.None</c>.
            /// </exception>
            /// <exception cref="System.ArgumentOutOfRangeException">
            ///     Thrown if <paramref name="index" /> is less than 0 or greater than
            ///     or equal to <see cref="HoleCount" />.
            /// </exception>
            internal NativeArray<Vector2> GetHole(int index, Allocator allocator)
            {
                var hole = new NativeArray<Vector2>();
                GetHole(index, allocator, ref hole);
                return hole;
            }

            /// <summary>
            ///     Get the polygon representing a hole in this boundary. The 2D vertices are in plane-space.
            /// </summary>
            /// <param name="index">The index of the hole. Must be less than <see cref="HoleCount" />.</param>
            /// <param name="allocator">
            ///     The allocator to use if <paramref name="polygonOut" /> must be resized.
            ///     Must be <c>Allocator.TempJob</c> or <c>Allocator.Persistent</c>.
            /// </param>
            /// <param name="polygonOut">The resulting polygon describing the hole at <paramref name="index" />.</param>
            /// <exception cref="System.InvalidOperationException">Thrown if <see cref="Valid" /> is false.</exception>
            /// <exception cref="System.InvalidOperationException">
            ///     Thrown if <paramref name="allocator" /> is <c>Allocator.Temp</c> or
            ///     <c>Allocator.None</c>.
            /// </exception>
            /// <exception cref="System.ArgumentOutOfRangeException">
            ///     Thrown if <paramref name="index" /> is less than 0 or greater than
            ///     or equal to <see cref="HoleCount" />.
            /// </exception>
            private unsafe void GetHole(int index, Allocator allocator, ref NativeArray<Vector2> polygonOut)
            {
                if (!Valid)
                    throw new InvalidOperationException("This plane boundary is not valid.");

                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Hole index must be greater than zero.");
                }

                if (index >= HoleCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"Hole index must be less than or equal to holeCount ({HoleCount}).");
                }

                var holes = Holes[index];
                CreateOrResizeNativeArrayIfNecessary((int)holes.VertexCountOutput, allocator,ref polygonOut);
                polygonOut = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector2>(holes.Vertices, (int)holes.VertexCountOutput, allocator);
                AssignAtomicSafetyHandle(ref polygonOut, allocator);
            }


            internal NativeArray<XrPlaneDetectorPolygonBuffer> Holes;
            internal XrPlaneDetectorPolygonBuffer Polygon;
            internal Pose PlanePose;
            internal TrackableId Id;
        }
    }
}
