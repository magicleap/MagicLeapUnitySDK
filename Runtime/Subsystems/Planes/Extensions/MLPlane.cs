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
            internal readonly struct MLPlane
        {
            /*! Plane center. */
            internal readonly Vector3 position;
            /*! Plane rotation. */
            internal readonly Quaternion rotation;
            /*! Plane width. */
            internal readonly float width;
            /*! Plane height. */
            internal readonly float height;
            /*! Flags which describe this plane. */
            internal readonly MLPlanesQueryFlags flags;
            /*! Plane ID. All inner planes within an outer plane will have the
                same ID (outer plane's ID). These IDs are persistent across
                plane queries unless a map merge occurs. On a map merge, IDs
                could be different. */
            internal readonly ulong id;
        }
    }

    }
}
