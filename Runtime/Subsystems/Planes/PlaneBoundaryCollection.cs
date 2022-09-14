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
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.XR.MagicLeap
{
    public partial class PlanesSubsystem
    {
        /// <summary>
        /// Represents a collection of concave <c>BoundedPlane</c> boundaries obtained from
        /// <see cref="MagicLeapPlaneSubsystem.GetAllBoundariesForPlane(TrackableId)"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Each Magic Leap plane can have multiple plane boundaries. This collection
        /// represents the set of all boundaries associated with a particular plane.
        /// Note that unlike most boundaries associated with <c>BoundedPlane</c>s,
        /// these are not necessarily convex.
        /// </para><para>
        /// The plane boundaries are tied to native resources which are managed
        /// by the <see cref="MagicLeapPlaneSubsystem"/>. Typically, a <c>PlaneBoundaryCollection</c>
        /// is only valid until the next call to
        /// <see cref="MagicLeapPlaneSubsystem.GetChanges(BoundedPlane, Allocator)"/>,
        /// so you should not hold onto an instance of this struct past a frame boundary.
        /// </para>
        /// </remarks>
        /// <seealso cref="MagicLeapPlaneSubsystem.GetPlaneBoundaries(TrackableId)"/>.
        public struct PlaneBoundaryCollection : IEquatable<PlaneBoundaryCollection>
        {
            /// <summary>
            /// Whether this collection is valid or not. Check for validity before using the index operator.
            /// </summary>
            public bool valid
            {
                get { return m_Boundaries.IsCreated; }
            }

            /// <summary>
            /// The number of boundaries in this collection.
            /// </summary>
            public int count
            {
                get { return m_Boundaries.Length; }
            }

            /// <summary>
            /// Attempts to get the plane boundary at index <paramref cref="index"/> and, if successful, copies it to <paramref name="boundaryOut"/>.
            /// <paramref name="boundaryOut"/> is resized or created using <paramref cref="allocator"/> if necessary.
            /// </summary>
            /// <param name="index">The index of the boundary to retrieve.</param>
            /// <param name="allocator">The Allocator to use if <paramref name="boundaryOut"/> must be recreated.
            /// Must be <c>Allocator.TempJob</c> or <c>Allocator.Persistent</c>.</param>
            /// <param name="boundaryOut">A NativeArray to fill with boundary points. If the array is not the correct size, it is disposed and recreated.
            /// If this method returns <c>false</c>, <c>boundaryOut</c> is unchanged.</param>
            /// <returns><c>true</c> if the boundary was successfully retrieved and <paramref name="boundaryOut"/> was populated; otherwise, <c>false</c>.</returns>
            /// <exception cref="System.InvalidOperationException">Thrown if <see cref="valid"/> is <c>false</c>.</exception>
            /// <exception cref="System.InvalidOperationException">Thrown if <paramref name="allocator"/> is <c>Allocator.Temp</c> or <c>Allocator.None</c>.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than zero or greater than or equal to <see cref="count"/>.</exception>
            public PlaneBoundary this[int index]
            {
                get
                {
                    if (!valid)
                        throw new InvalidOperationException("This boundary collection is not valid.");

                    if (index < 0)
                        throw new ArgumentOutOfRangeException(nameof(index), "Boundary index must be greater than zero.");

                    if (index >= count)
                        throw new ArgumentOutOfRangeException(nameof(index), $"Boundary index must be less than the boundary count ({count}).");

                    return new PlaneBoundary(m_Boundaries[index], m_Pose);
                }
            }

            /// <summary>
            /// Get an enumerator, compatible with a duck-typed foreach. You typically would not call
            /// this directly, but is used by the compiler in a <c>foreach</c> statement.
            /// </summary>
            /// <returns>An Enumerator compatible with a duck-typed foreach.</returns>
            public Enumerator GetEnumerator()
            {
                return new Enumerator(this);
            }

            /// <summary>
            /// An enumerator which can be used by a <c>foreach</c> statement to iterate over
            /// the elements in a <see cref="PlaneBoundaryCollection"/>.
            /// </summary>
            public struct Enumerator
            {
                internal Enumerator(PlaneBoundaryCollection collection)
                {
                    m_Index = -1;
                    m_Collection = collection;
                }

                /// <summary>
                /// Moves to the next element in the collection.
                /// </summary>
                /// <returns><c>true</c> if the next element is valid, or <c>false</c> if the Enumerator is already at the end of the collection.</returns>
                public bool MoveNext()
                {
                    return ++m_Index < m_Collection.count;
                }

                /// <summary>
                /// Resets the enumerator.
                /// </summary>
                public void Reset()
                {
                    m_Index = -1;
                }

                /// <summary>
                /// The current element in the enumerator.
                /// </summary>
                public PlaneBoundary Current
                {
                    get
                    {
                        return m_Collection[m_Index];
                    }
                }

                /// <summary>
                /// Disposes of the enumerator.
                /// </summary>
                public void Dispose()
                {
                    m_Collection = default(PlaneBoundaryCollection);
                    m_Index = -1;
                }

                int m_Index;

                PlaneBoundaryCollection m_Collection;
            }

            /// <summary>
            /// Computes a hash code suitable for use in a <c>Dictionary</c> or <c>HashSet</c>.
            /// </summary>
            /// <returns>A hash code suitable for use in a <c>Dictionary</c> or <c>HashSet</c>.</returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    var hash = m_Boundaries.GetHashCode();
                    hash = hash * 486187739 + m_Pose.GetHashCode();
                    return hash;
                }
            }

            /// <summary>
            /// IEquatable interface. Compares for equality.
            /// </summary>
            /// <param name="obj">The object to compare for equality.</param>
            /// <returns><c>true</c> if <paramref name="obj"/> is of type <see cref="PlaneBoundaryCollection"/> and compares equal with <see cref="Equals(PlaneBoundaryCollection)"/>.</returns>
            public override bool Equals(object obj)
            {
                return ((obj is PlaneBoundaryCollection) && Equals((PlaneBoundaryCollection)obj));
            }

            /// <summary>
            /// IEquatable interface. Comapres for equality.
            /// </summary>
            /// <param name="other">The <see cref="PlaneBoundaryCollection"/> to compare against.</param>
            /// <returns><c>true</c> if all fields of this <see cref="PlaneBoundaryCollection"/> compare equal to <paramref name="other"/>.</returns>
            public bool Equals(PlaneBoundaryCollection other)
            {
                return
                    m_Boundaries.Equals(other.m_Boundaries) &&
                    m_Pose.Equals(other.m_Pose);
            }

            /// <summary>
            /// Comapres for equality. Same as <see cref="Equals(PlaneBoundaryCollection)"/>.
            /// </summary>
            /// <param name="lhs">The left-hand side of the comparison.</param>
            /// <param name="rhs">The right-hand side of the comparison.</param>
            /// <returns><c>true</c> if all fields of this <see cref="PlaneBoundaryCollection"/> compare equal to <paramref name="other"/>.</returns>
            public static bool operator ==(PlaneBoundaryCollection lhs, PlaneBoundaryCollection rhs)
            {
                return lhs.Equals(rhs);
            }

            /// <summary>
            /// Comapres for inequality. Same as <c>!</c><see cref="Equals(PlaneBoundaryCollection)"/>.
            /// </summary>
            /// <param name="lhs">The left-hand side of the comparison.</param>
            /// <param name="rhs">The right-hand side of the comparison.</param>
            /// <returns><c>true</c> if any of the fields of this <see cref="PlaneBoundaryCollection"/> are not equal to <paramref name="other"/>.</returns>
            public static bool operator !=(PlaneBoundaryCollection lhs, PlaneBoundaryCollection rhs)
            {
                return !lhs.Equals(rhs);
            }

            internal unsafe PlaneBoundaryCollection(Extensions.MLPlaneBoundaries planeBoundaries, Pose planePose)
            {
                m_Boundaries = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Extensions.MLPlaneBoundary>(
                    planeBoundaries.boundaries,
                    (int)planeBoundaries.boundaries_count,
                    Allocator.None);

#if UNITY_EDITOR
            var safetyHandle = AtomicSafetyHandle.Create();
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref m_Boundaries, safetyHandle);
#endif

                m_Pose = planePose;
            }

            NativeArray<Extensions.MLPlaneBoundary> m_Boundaries;

            Pose m_Pose;
        }
    }
}
