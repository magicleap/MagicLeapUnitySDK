// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using UnityEngine;

namespace MagicLeap.OpenXR.Features.Reprojection
{
    internal enum XrReprojectionStructTypes : uint
    {
        XrCompositionLayerReprojectionInfo = 1000066000,
        XrCompositionLayerReprojectionPlaneOverride = 1000066001
    }

    internal enum XrReprojectionMode : uint
    {
        None,
        Depth = 1,
        PlanarFromDepth,
        PlanarManual,
        OrientationOnly
    }

    internal struct XrCompositionLayerReprojectionInfo
    {
        internal XrReprojectionStructTypes Type;
        internal IntPtr Next;
        internal XrReprojectionMode ReprojectionMode;

        internal void Initialize()
        {
            Type = XrReprojectionStructTypes.XrCompositionLayerReprojectionInfo;
        }
    }

    internal struct XrCompositionLayerReprojectionPlaneOverride
    {
        internal XrReprojectionStructTypes Type;
        internal IntPtr Next;
        internal Vector3 Position;
        internal Vector3 Normal;
        internal Vector3 Velocity;

        internal void Initialize()
        {
            Type = XrReprojectionStructTypes.XrCompositionLayerReprojectionPlaneOverride;
            Position = Vector3.forward;
            Normal = Vector3.forward;
        }
    }
}
