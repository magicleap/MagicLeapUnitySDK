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

namespace UnityEngine.XR.MagicLeap
{
    public partial class PlanesSubsystem
    {
        internal static class ConvexHullGenerator
        {
            // Get a single static reference to AngleComparer to avoid additional GC allocs
            static Comparison<Vector2> s_PolarAngleComparer = AngleComparer;

            // Used by AngleComparer
            static Vector2 s_Pivot;

            // Reusable List to avoid additional GC allocs
            static List<Vector2> s_Points = new List<Vector2>();

            /// <summary>
            /// Used to sort a collection of points by the polar angle
            /// made with <see cref="s_Pivot"/> against the +x axis.
            /// </summary>
            /// <param name="lhs">The first point to compare.</param>
            /// <param name="rhs">The second point to compare.</param>
            /// <returns>
            /// -1 if the vector from
            /// <see cref="s_Pivot"/> to <paramref name="lhs"/> makes a larger
            /// angle against the +x axis than <see cref="s_Pivot"/> to <paramref name="rhs"/>,
            /// +1 if the angle is smaller, and 0 if they are equal.
            /// </returns>
            static int AngleComparer(Vector2 lhs, Vector2 rhs)
            {
                // Compute the angle against the pivot
                var u = (lhs - s_Pivot);
                var v = (rhs - s_Pivot);
                var cross = (u.x * v.y - u.y * v.x);

                // cross > 0 => lhs is more to the right than rhs
                return Math.Sign(cross);
            }

            /// <summary>
            /// returns true if a, b, c form a clockwise turn
            /// </summary>
            static bool ClockwiseTurn(Vector2 a, Vector2 b, Vector2 c)
            {
                var u = a - b;
                var v = c - b;
                return (u.x * v.y - u.y * v.x) > 0f;
            }

            /// <summary>
            /// Computes convex hull using the Graham Scan method.
            /// </summary>
            /// <param name="points">An arbitrary collection of 2D points.</param>
            /// <param name="allocator">The allocator to use for the returned array.</param>
            /// <param name="length">The number of points in the resulting convex hull.</param>
            /// <returns>A new NativeArray containing the convex hull. The allocated Length of the array will always
            /// be the same as <paramref name="points"/>. <paramref name="length"/> contains the true number of
            /// points in the hull, which will always be less than <paramref name="points"/>.Length.</returns>
            static NativeFixedList<Vector2> GrahamScan(NativeArray<Vector2> points, Allocator allocator)
            {
                // Step 1: Find the lowest y-coordinate and leftmost point,
                //         called the pivot
                int pivotIndex = 0;
                for (int i = 1; i < points.Length; ++i)
                {
                    var point = points[i];
                    var pivot = points[pivotIndex];
                    if (point.y < pivot.y)
                    {
                        pivotIndex = i;
                    }
                    else if (point.y == pivot.y && point.x < pivot.x)
                    {
                        pivotIndex = i;
                    }
                }
                s_Pivot = points[pivotIndex];

                // Step 2: Copy all points except the pivot into a List
                s_Points.Clear();
                for (int i = 0; i < pivotIndex; ++i)
                    s_Points.Add(points[i]);
                for (int i = pivotIndex + 1; i < points.Length; ++i)
                    s_Points.Add(points[i]);

                // Step 3: Sort points by polar angle with the pivot
                s_Points.Sort(s_PolarAngleComparer);

                // Step 4: Compute the hull
                int length = 0;
                var hull = new NativeArray<Vector2>(points.Length, allocator);
                hull[length++] = s_Pivot;
                foreach (var point in s_Points)
                {
                    while (length > 1 && !ClockwiseTurn(hull[length - 2], hull[length - 1], point))
                    {
                        --length;
                    }

                    hull[length++] = point;
                }

                return new NativeFixedList<Vector2>(hull, length);
            }

            static void CreateOrResizeNativeArrayIfNecessary<T>(
                int length,
                Allocator allocator,
                ref NativeArray<T> array) where T : struct
            {
                if (array.IsCreated)
                {
                    if (array.Length != length)
                    {
                        array.Dispose();
                        array = new NativeArray<T>(length, allocator);
                    }
                }
                else
                {
                    array = new NativeArray<T>(length, allocator);
                }
            }

            public static void GrahamScan(NativeArray<Vector2> points, Allocator allocator, ref NativeArray<Vector2> convexHullOut)
            {
                // We need to make a copy because GrahamScan doesn't know how big the result will be.
                using (var hull = ConvexHullGenerator.GrahamScan(points, Allocator.Temp))
                {
                    CreateOrResizeNativeArrayIfNecessary<Vector2>(hull.Length, allocator, ref convexHullOut);
                    hull.CopyTo(convexHullOut);
                }
            }

            static bool IsPointLeftOfLine(Vector2 point, Vector2 lA, Vector2 lB)
            {
                var u = lB - lA;
                var v = point - lA;
                return (u.x * v.y - u.y * v.x) > 0f;
            }

            /// <summary>
            /// Computes a convex hull using the Gift Wrap method.
            /// </summary>
            /// <param name="points"></param>
            /// <param name="allocator"></param>
            /// <returns></returns>
            public static void Giftwrap(NativeArray<Vector2> points, Allocator allocator, ref NativeArray<Vector2> convexHullOut)
            {
                if (!points.IsCreated)
                    throw new ArgumentException("Array has been disposed.", nameof(points));

                // pointOnHull is initialized to the leftmost point
                // which is guaranteed to be part of the convex hull
                int pointOnHull = 0;
                for (int i = 1; i < points.Length; ++i)
                {
                    if (points[i].x < points[pointOnHull].x || (points[i].x == points[pointOnHull].x && points[i].y < points[pointOnHull].y))
                    {
                        pointOnHull = i;
                    }
                }

                using (var hullIndices = new NativeFixedList<int>(points.Length, Allocator.Temp))
                {
                    int endpoint = 0;
                    do
                    {
                        bool endpointAlreadyOnHull = false;
                        for (int j = 0; j < hullIndices.Length; j++)
                        {
                            if (hullIndices[j] == pointOnHull)
                            {
                                endpointAlreadyOnHull = true;
                                break;
                            }
                        }
                        if (endpointAlreadyOnHull)
                        {
                            break;
                        }

                        hullIndices.Add(pointOnHull);
                        endpoint = 0;      // initial endpoint for a candidate edge on the hull
                        for (int j = 1; j < points.Length; ++j)
                        {
                            endpoint = (endpoint == pointOnHull || IsPointLeftOfLine(points[j], points[pointOnHull], points[endpoint])) ? j : endpoint;
                        }
                        pointOnHull = endpoint;
                    } while (endpoint != hullIndices[0] && hullIndices.Length < hullIndices.Capacity);  // wrapped around to first hull point

                    CreateOrResizeNativeArrayIfNecessary<Vector2>(hullIndices.Length, allocator, ref convexHullOut);
                    for (int i = 0; i < hullIndices.Length; ++i)
                    {
                        convexHullOut[i] = points[hullIndices[i]];
                    }
                }
            }
        }

    }
}
