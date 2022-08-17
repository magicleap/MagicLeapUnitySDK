// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLPlanesQuery.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2021 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Runtime.InteropServices;

namespace UnityEngine.XR.MagicLeap
{
    public partial class PlanesSubsystem
    {
        public static partial class Extensions
        {
            [StructLayout(LayoutKind.Sequential)]
            internal readonly struct MLPlanesQuery
            {
                /*! The flags to apply to this query. */
                internal readonly PlanesSubsystem.Extensions.MLPlanesQueryFlags flags;
                /*! The center of the bounding box which defines where planes extraction should occur.
                    In world scale units - meters by default. */
                internal readonly Native.MagicLeapNativeBindings.MLVec3f bounds_center;
                /*! The rotation of the bounding box where planes extraction will occur. */
                internal readonly Native.MagicLeapNativeBindings.MLQuaternionf bounds_rotation;
                /*! The size of the bounding box where planes extraction will occur. 
                    In world scale units - meters by default.*/
                internal readonly Native.MagicLeapNativeBindings.MLVec3f bounds_extents;
                /*! The maximum number of results that should be returned. This is also
                    the minimum expected size of the array of results passed to the
                    MLPlanesGetResult function. */
                internal readonly uint max_results;
                /*!
                \brief If #MLPlanesQueryFlag_IgnoreHoles is set to false, holes with a perimeter
                    (in world scale units - meters by default) smaller than this value will be ignored, 
                    and can be part of the plane. This value cannot be lower than 0 (lower values will be capped to
                    this minimum). A good default value is 0.5.
                \deprecated Deprecated since 0.15.0.
                */
                internal readonly float min_hole_length;
                /*! The minimum area (in squared world scale units - meters by default) of planes to be returned. 
                    This value cannot be lower than 0.04 (lower values will be capped to this minimum).
                    A good default value is 0.25. */
                internal readonly float min_plane_area;

                internal MLPlanesQuery(PlanesSubsystem.Extensions.PlanesQuery planesQuery)
                {
                    this.flags = planesQuery.Flags;
                    this.bounds_center = Native.MLConvert.FromUnity(planesQuery.BoundsCenter);
                    this.bounds_rotation = Native.MLConvert.FromUnity(planesQuery.BoundsRotation);
                    this.bounds_extents = Native.MLConvert.FromUnity(planesQuery.BoundsExtents);
                    this.max_results = planesQuery.MaxResults;
                    this.min_hole_length = Native.MLConvert.FromUnity(planesQuery.MinHoleLength);
                    this.min_plane_area = Native.MLConvert.FromUnity(planesQuery.MinPlaneArea);
                }
            }
        }
    }
}
