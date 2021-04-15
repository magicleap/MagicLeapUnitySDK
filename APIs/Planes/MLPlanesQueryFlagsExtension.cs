// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLPlanesQueryFlagsExtension.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// A class to provide an extension for the MLPlanes.QueryFlags enum.
    /// </summary>
    public static class MLPlanesQueryFlagsExtension
    {
        /// <summary>
        /// Indicates if QueryFlags contains Vertical flag
        /// </summary>
        /// <param name="queryFlags">The bitmask of control flags for plane queries.</param>
        /// <returns>True if the MLPlanes.QueryFlags.Vertical flag is true.</returns>
        public static bool IsVertical(this MLPlanes.QueryFlags queryFlags)
        {
            return (queryFlags & MLPlanes.QueryFlags.Vertical) != 0;
        }

        /// <summary>
        /// Indicates if QueryFlags contains Horizontal flag
        /// </summary>
        /// <param name="queryFlags">The bitmask of control flags for plane queries.</param>
        /// <returns>True if the MLPlanes.QueryFlags.Horizontal flag is true.</returns>
        public static bool IsHorizontal(this MLPlanes.QueryFlags queryFlags)
        {
            return (queryFlags & MLPlanes.QueryFlags.Horizontal) != 0;
        }

        /// <summary>
        /// Indicates if QueryFlags contains Arbitrary flag
        /// </summary>
        /// <param name="queryFlags">The bitmask of control flags for plane queries.</param>
        /// <returns>True if the MLPlanes.QueryFlags.Arbitrary flag is true.</returns>
        public static bool IsArbitrary(this MLPlanes.QueryFlags queryFlags)
        {
            return (queryFlags & MLPlanes.QueryFlags.Arbitrary) != 0;
        }

        /// <summary>
        /// Indicates if QueryFlags contains AllOrientations flag
        /// </summary>
        /// <param name="queryFlags">The bitmask of control flags for plane queries.</param>
        /// <returns>True if the MLPlanes.QueryFlags.AllOrientations flag is true.</returns>
        public static bool IsAllOrientations(this MLPlanes.QueryFlags queryFlags)
        {
            return (queryFlags & MLPlanes.QueryFlags.AllOrientations) != 0;
        }

        /// <summary>
        /// Indicates if QueryFlags contains OrientToGravity flag
        /// </summary>
        /// <param name="queryFlags">The bitmask of control flags for plane queries.</param>
        /// <returns>True if the MLPlanes.QueryFlags.OrientToGravity flag is true.</returns>
        public static bool IsOrientedToGravity(this MLPlanes.QueryFlags queryFlags)
        {
            return (queryFlags & MLPlanes.QueryFlags.OrientToGravity) != 0;
        }

        /// <summary>
        /// Indicates if QueryFlags contains Inner flag
        /// </summary>
        /// <param name="queryFlags">The bitmask of control flags for plane queries.</param>
        /// <returns>True if the MLPlanes.QueryFlags.Inner flag is true.</returns>
        public static bool IsInner(this MLPlanes.QueryFlags queryFlags)
        {
            return (queryFlags & MLPlanes.QueryFlags.Inner) != 0;
        }

        /// <summary>
        /// Indicates if QueryFlags contains IgnoreHoles flag
        /// </summary>
        /// <param name="queryFlags">The bitmask of control flags for plane queries.</param>
        /// <returns>True if the MLPlanes.QueryFlags.IgnoreHoles flag is true.</returns>
        public static bool IsIgnoreHoles(this MLPlanes.QueryFlags queryFlags)
        {
            return (queryFlags & MLPlanes.QueryFlags.IgnoreHoles) != 0;
        }

        /// <summary>
        /// Indicates if QueryFlags contains Ceiling flag
        /// </summary>
        /// <param name="queryFlags">The bitmask of control flags for plane queries.</param>
        /// <returns>True if the MLPlanes.QueryFlags.SemanticCeiling flag is true.</returns>
        public static bool IsCeiling(this MLPlanes.QueryFlags queryFlags)
        {
            return (queryFlags & MLPlanes.QueryFlags.SemanticCeiling) != 0;
        }

        /// <summary>
        /// Indicates if QueryFlags contains Floor flag
        /// </summary>
        /// <param name="queryFlags">The bitmask of control flags for plane queries.</param>
        /// <returns>True if the MLPlanes.QueryFlags.SemanticFloor flag is true.</returns>
        public static bool IsFloor(this MLPlanes.QueryFlags queryFlags)
        {
            return (queryFlags & MLPlanes.QueryFlags.SemanticFloor) != 0;
        }

        /// <summary>
        /// Indicates if QueryFlags contains Wall flag
        /// </summary>
        /// <param name="queryFlags">The bitmask of control flags for plane queries.</param>
        /// <returns>True if the MLPlanes.QueryFlags.SemanticWall flag is true.</returns>
        public static bool IsWall(this MLPlanes.QueryFlags queryFlags)
        {
            return (queryFlags & MLPlanes.QueryFlags.SemanticWall) != 0;
        }

        /// <summary>
        /// Indicates if QueryFlags contains All flag
        /// </summary>
        /// <param name="queryFlags">The bitmask of control flags for plane queries.</param>
        /// <returns>True if the MLPlanes.QueryFlags.SemanticAll flag is true.</returns>
        public static bool IsAll(this MLPlanes.QueryFlags queryFlags)
        {
            return (queryFlags & MLPlanes.QueryFlags.SemanticAll) != 0;
        }

        /// <summary>
        /// Indicates if QueryFlags contains Polygons flag
        /// </summary>
        /// <param name="queryFlags">The bitmask of control flags for plane queries.</param>
        /// <returns>True if the MLPlanes.QueryFlags.Polygons flag is true.</returns>
        public static bool IsPolygons(this MLPlanes.QueryFlags queryFlags)
        {
            return (queryFlags & MLPlanes.QueryFlags.Polygons) != 0;
        }
    }
}
