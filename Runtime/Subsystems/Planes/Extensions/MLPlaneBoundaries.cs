// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLPlaneBoundaries.cs" company="Magic Leap, Inc">
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
            internal unsafe readonly struct MLPlaneBoundaries
            {
                internal bool valid
                {
                    get { return boundaries != null; }
                }

                /*!
                    \brief Plane ID, the same value associating to the ID in #MLPlane if they
                    belong to the same plane.
                */
                internal readonly ulong id;
                /*!
                    \brief The boundaries in a plane.
                */
                internal readonly MLPlaneBoundary* boundaries;
                /*!
                    \brief Count of boundaries. A plane may contain multiple boundaries
                    each of which defines a region.
                */
                internal readonly uint boundaries_count;
            }
        }

    }
}
