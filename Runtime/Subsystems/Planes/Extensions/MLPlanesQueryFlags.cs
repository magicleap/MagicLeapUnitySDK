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
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.MagicLeap
{
    public partial class PlanesSubsystem
    {
        public static partial class Extensions
        {
            [Flags]
            public enum MLPlanesQueryFlags : uint
            {
                /// <summary>
                ///  Include no planes.
                /// </summary>
                None = 0,
                /// <summary>
                ///  Include planes whose normal is perpendicular to gravity.
                /// </summary>
                Vertical = 1 << 0,
                /// <summary>
                /// Include planes whose normal is parallel to gravity.
                /// </summary>
                Horizontal = 1 << 1,
                /// <summary>
                /// Include planes with arbitrary normals.
                /// </summary>
                Arbitrary = 1 << 2,
                /// <summary>
                /// Include all plane orientations.
                /// </summary>
                AllOrientations = Vertical |
                                   Horizontal |
                                   Arbitrary,
                /// <summary>
                /// For non-horizontal planes, setting this flag will result in the top of
                /// the plane rectangle being perpendicular to gravity.
                /// </summary>
                OrientToGravity = 1 << 3,
                /// <summary>
                /// If this flag is set, inner planes will be returned; if it is not set,
                /// outer planes will be returned.
                /// </summary>
                Inner = 1 << 4,
                /// <summary>
                /// Include planes semantically tagged as ceiling.
                /// </summary>
                Semantic_Ceiling = 1 << 6,
                /// <summary>
                /// Include planes semantically tagged as floor.
                /// </summary>
                Semantic_Floor = 1 << 7,
                /// <summary>
                /// Include planes semantically tagged as wall.
                /// </summary>
                Semantic_Wall = 1 << 8,
                /// <summary>
                /// Include all planes that are semantically tagged.
                /// </summary>
                Semantic_All = Semantic_Ceiling |
                                   Semantic_Floor |
                                   Semantic_Wall,
                /// <summary>
                /// Include polygonal planes. (Planes that have more than 4 edges.
                /// </summary>
                /// <remarks>
                /// <note>
                /// The <b>MLPlanesReleaseBoundariesList</b>, <b>MLPlanesQueryGetResultsWithBoundaries</b>, <b>MLPlanesQueryGetResults</b>
                /// calls are handled automatically when using the Magic Leap Plane subsystem
                /// </note>
                /// When this flag is set:
                /// <list type="bullet">
                ///     <item>
                ///         <term>MLPlanesQueryGetResultsWithBoundaries</term>
                ///             <description>
                ///             returns polygons along with
                ///             other applicable rectangular planes. MLPlanesReleaseBoundariesList
                ///             MUST be called before the next call to MLPlanesQueryGetResultsWithBoundaries
                ///             or MLPlanesQueryGetResults, otherwise UnspecifiedFailure will be returned.
                ///         </description>
                ///     </item>
                ///     <item>
                ///         <term>MLPlanesQueryGetResults</term>
                ///             <description>
                ///             returns just the rectangular planes.
                ///             polygons (if any) extracted during the query will be discarded.
                ///             No need to call MLPlanesReleaseBoundariesList before the
                ///             next MLPlanesQueryGetResultsWithBoundaries or MLPlanesQueryGetResults.
                ///         </description>
                ///     </item>
                /// </list>
                ///     When this flag is not set:
                /// <list type="bullet">
                ///     <item>
                ///         <term>both the APIs</term>
                ///             <description>
                ///             MLPlanesQueryGetResultsWithBoundaries and
                ///             MLPlanesQueryGetResults returns just rectangular planes.
                ///             No need to call MLPlanesReleaseBoundariesList.
                ///         </description>
                ///     </item>
                /// </list>
                ///</remarks>
                Polygons = 1 << 9,
            }
        }
    }

    public static class MLPlanesQueryFlagsAndPlaneDetectionModeExtensions
    {
        public static PlaneDetectionMode ToPlaneDetectionMode(this PlanesSubsystem.Extensions.MLPlanesQueryFlags MLPlanesQueryFlags)
        {
            var outDetectionMode = PlaneDetectionMode.None;
            if ((MLPlanesQueryFlags & PlanesSubsystem.Extensions.MLPlanesQueryFlags.Horizontal) != 0)
                outDetectionMode |= PlaneDetectionMode.Horizontal;
            if ((MLPlanesQueryFlags & PlanesSubsystem.Extensions.MLPlanesQueryFlags.Vertical) != 0)
                outDetectionMode |= PlaneDetectionMode.Vertical;
            return outDetectionMode;
        }

        public static PlanesSubsystem.Extensions.MLPlanesQueryFlags ToMLQueryFlags(this PlaneDetectionMode planeDetectionMode)
        {
            var outFlags = PlanesSubsystem.Extensions.MLPlanesQueryFlags.None;
            if ((planeDetectionMode & PlaneDetectionMode.Horizontal) != 0)
                outFlags |= PlanesSubsystem.Extensions.MLPlanesQueryFlags.Horizontal;
            if ((planeDetectionMode & PlaneDetectionMode.Vertical) != 0)
                outFlags |= PlanesSubsystem.Extensions.MLPlanesQueryFlags.Vertical;
            return outFlags;
        }
    }

}
