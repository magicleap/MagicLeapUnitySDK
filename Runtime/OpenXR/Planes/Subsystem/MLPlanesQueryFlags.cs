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

namespace MagicLeap.OpenXR.Subsystems
{
    public partial class MLXrPlaneSubsystem
    {
        [Flags]
        public enum MLPlanesQueryFlags : uint
        {
            /// <summary>
            ///     Include no planes.
            /// </summary>
            None = 0,

            /// <summary>
            ///     Include planes whose normal is perpendicular to gravity.
            /// </summary>
            Vertical = 1 << 0,

            /// <summary>
            ///     Include planes whose normal is parallel to gravity.
            /// </summary>
            Horizontal = 1 << 1,

            /// <summary>
            ///     Include planes with arbitrary normals.
            /// </summary>
            Arbitrary = 1 << 2,

            /// <summary>
            ///     Include all plane orientations.
            /// </summary>
            AllOrientations = Vertical | Horizontal | Arbitrary,

            /// <summary>
            ///     Include planes semantically tagged as ceiling.
            /// </summary>
            SemanticCeiling = 1 << 6,

            /// <summary>
            ///     Include planes semantically tagged as floor.
            /// </summary>
            SemanticFloor = 1 << 7,

            /// <summary>
            ///     Include planes semantically tagged as wall.
            /// </summary>
            SemanticWall = 1 << 8,
            
            /// <summary>
            /// Include planes semantically tagged as platforms
            /// </summary>
            SemanticPlatform = 1 << 9,

            /// <summary>
            ///     Include all planes that are semantically tagged.
            /// </summary>
            SemanticAll = SemanticCeiling | SemanticFloor | SemanticWall | SemanticPlatform
        }
    }
}
