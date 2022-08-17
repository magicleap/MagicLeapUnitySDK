// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLPlaneBoundary.cs" company="Magic Leap, Inc">
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
using System.Runtime.InteropServices;

namespace UnityEngine.XR.MagicLeap
{
    public partial class PlanesSubsystem
    {
        public static partial class Extensions
        {
            [StructLayout(LayoutKind.Sequential)]
            internal unsafe readonly struct MLPlaneBoundary : IEquatable<MLPlaneBoundary>
            {
                internal bool valid
                {
                    get { return polygon != null; }
                }

                public override int GetHashCode()
                {
                    unchecked
                    {
                        var hash = (new IntPtr(polygon)).GetHashCode();
                        hash = hash * 486187739 + (new IntPtr(holes)).GetHashCode();
                        hash = hash * 486187739 + holes_count.GetHashCode();
                        return hash;
                    }
                }

                public bool Equals(MLPlaneBoundary other)
                {
                    return
                        (polygon == other.polygon) &&
                        (holes == other.holes) &&
                        (holes_count == other.holes_count);
                }

                /*!
                    \brief The polygon that defines the region.
                */
                internal readonly MLPolygon* polygon;
                /*!
                    \brief A polygon may contains multiple holes.
                */
                internal readonly MLPolygon* holes;
                /*!
                    \brief Count of the holes.
                */
                internal readonly uint holes_count;
            }
        }



    }
}
