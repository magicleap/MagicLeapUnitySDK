// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
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
            internal unsafe readonly struct MLPlaneBoundariesList
            {
                internal MLPlaneBoundariesList(uint version = 1)
                {
                    this.version = version;
                    plane_boundaries = null;
                    plane_boundaries_count = 0;
                }

                internal static MLPlaneBoundariesList Create()
                {
                    // Same as MLPlaneBoundariesListInit in ml_planes.h
                    return new MLPlaneBoundariesList(1);    
                }

                internal bool valid
                {
                    get
                    {
                        return
                            (plane_boundaries != null);
                    }
                }

                internal readonly uint version;
                /*!
                    \brief List of #MLPlaneBoundaries.
                    \apilevel 2
                */
                internal readonly MLPlaneBoundaries* plane_boundaries;
                /*!
                    \brief Count of #MLPlaneBoundaries in the array.
                    \apilevel 2
                */
                internal readonly uint plane_boundaries_count;
            }
        }

    }
}
