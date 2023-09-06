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
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.XR.MagicLeap;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MLXrPlaneSubsystem
    {
        internal static class ConvexHullGenerator
        {
            // Get a single static reference to AngleComparer to avoid additional GC allocs
            private static readonly Comparison<Vector2> PolarAngleComparer = AngleComparer;

            // Used by AngleComparer
            private static Vector2 pivot;

            // Reusable List to avoid additional GC alloc
            private static readonly List<Vector2> Points = new();

            /// <summary>
            ///     Used to sort a collection of points by the polar angle
            ///     made with <see cref="pivot" /> against the +x axis.
            /// </summary>
            /// <param name="lhs">The first point to compare.</param>
            /// <param name="rhs">The second point to compare.</param>
            /// <returns>
            ///     -1 if the vector from
            ///     <see cref="pivot" /> to <paramref name="lhs" /> makes a larger
            ///     angle against the +x axis than <see cref="pivot" /> to <paramref name="rhs" />,
            ///     +1 if the angle is smaller, and 0 if they are equal.
            /// </returns>
            private static int AngleComparer(Vector2 lhs, Vector2 rhs)
            {
                // Compute the angle against the pivot
                var u = lhs - pivot;
                var v = rhs - pivot;
                var cross = u.x * v.y - u.y * v.x;

                // cross > 0 => lhs is more to the right than rhs
                return Math.Sign(cross);
            }

            /// <summary>
            ///     returns true if a, b, c form a clockwise turn
            /// </summary>
            private static bool ClockwiseTurn(Vector2 a, Vector2 b, Vector2 c)
            {
                var u = a - b;
                var v = c - b;
                return u.x * v.y - u.y * v.x > 0f;
            }

            /// <summary>
            ///     Computes convex hull using the Graham Scan method.
            /// </summary>
            /// <param name="points">An arbitrary collection of 2D points.</param>
            /// <param name="allocator">The allocator to use for the returned array.</param>
            /// <returns>
            ///     A new NativeArray containing the convex hull. The allocated Length of the array will always
            ///     be the same as <paramref name="points" />. <paramref /> contains the true number of
            ///     points in the hull, which will always be less than <paramref name="points" />.Length.
            /// </returns>
            private static NativeFixedList<Vector2> GrahamScan(NativeArray<Vector2> points, Allocator allocator)
            {
                // Step 1: Find the lowest y-coordinate and leftmost point,
                //         called the pivot
                var pivotIndex = 0;
                for (var i = 1; i < points.Length; ++i)
                {
                    var point = points[i];
                    var pointPivot = points[pivotIndex];
                    if (point.y < pointPivot.y)
                    {
                        pivotIndex = i;
                    }
                    else if (Mathf.Approximately(point.y, pointPivot.y) && point.x < pointPivot.x)
                    {
                        pivotIndex = i;
                    }
                }

                pivot = points[pivotIndex];

                // Step 2: Copy all points except the pivot into a List
                Points.Clear();
                for (var i = 0; i < pivotIndex; ++i)
                    Points.Add(points[i]);
                for (var i = pivotIndex + 1; i < points.Length; ++i)
                    Points.Add(points[i]);

                // Step 3: Sort points by polar angle with the pivot
                Points.Sort(PolarAngleComparer);

                // Step 4: Compute the hull
                var length = 0;
                var hull = new NativeArray<Vector2>(points.Length, allocator);
                hull[length++] = pivot;
                foreach (var point in Points)
                {
                    while (length > 1 && !ClockwiseTurn(hull[length - 2], hull[length - 1], point))
                    {
                        --length;
                    }

                    hull[length++] = point;
                }

                return new NativeFixedList<Vector2>(hull, length);
            }

            public static void GrahamScan(NativeArray<Vector2> points, Allocator allocator, ref NativeArray<Vector2> convexHullOut)
            {
                // We need to make a copy because GrahamScan doesn't know how big the result will be.
                using var hull = GrahamScan(points, Allocator.Temp);
                CreateOrResizeNativeArrayIfNecessary(hull.Length, allocator, ref convexHullOut);
                hull.CopyTo(convexHullOut);
            }

            private static bool IsPointLeftOfLine(Vector2 point, Vector2 lA, Vector2 lB)
            {
                var u = lB - lA;
                var v = point - lA;
                return u.x * v.y - u.y * v.x > 0f;
            }

            /// <summary>
            ///     Computes a convex hull using the Gift Wrap method.
            /// </summary>
            /// <param name="points"></param>
            /// <param name="allocator"></param>
            /// <param name="convexHullOut"></param>
            /// <returns></returns>
            public static void GiftWrap(NativeArray<Vector2> points, Allocator allocator, ref NativeArray<Vector2> convexHullOut)
            {
                if (!points.IsCreated)
                    throw new ArgumentException("Array has been disposed.", nameof(points));

                // pointOnHull is initialized to the leftmost point
                // which is guaranteed to be part of the convex hull
                var pointOnHull = 0;
                for (var i = 1; i < points.Length; ++i)
                {
                    if (points[i].x < points[pointOnHull].x || (Mathf.Approximately(points[i].x, points[pointOnHull].x) && points[i].y < points[pointOnHull].y))
                    {
                        pointOnHull = i;
                    }
                }

                using var hullIndices = new NativeFixedList<int>(points.Length, Allocator.Temp);
                int endpoint;
                do
                {
                    var endpointAlreadyOnHull = false;
                    foreach (var t in hullIndices)
                    {
                        if (t != pointOnHull) continue;
                        endpointAlreadyOnHull = true;
                        break;
                    }

                    if (endpointAlreadyOnHull) break;

                    hullIndices.Add(pointOnHull);
                    endpoint = 0; // initial endpoint for a candidate edge on the hull
                    for (var j = 1; j < points.Length; ++j) endpoint = endpoint == pointOnHull || IsPointLeftOfLine(points[j], points[pointOnHull], points[endpoint]) ? j : endpoint;
                    pointOnHull = endpoint;
                } while (endpoint != hullIndices[0] && hullIndices.Length < hullIndices.Capacity); // wrapped around to first hull point

                CreateOrResizeNativeArrayIfNecessary(hullIndices.Length, allocator, ref convexHullOut);
                for (var i = 0; i < hullIndices.Length; ++i)
                {
                    convexHullOut[i] = points[hullIndices[i]];
                }
            }
        }
    }
}
