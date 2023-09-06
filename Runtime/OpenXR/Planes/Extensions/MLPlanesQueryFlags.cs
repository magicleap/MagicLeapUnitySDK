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
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
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

    public static class MLPlanesQueryFlagsAndPlaneDetectionModeExtensions
    {
        public static PlaneDetectionMode ToPlaneDetectionMode(this MLXrPlaneSubsystem.MLPlanesQueryFlags planesQueryFlags)
        {
            var outDetectionMode = PlaneDetectionMode.None;
            if ((planesQueryFlags & MLXrPlaneSubsystem.MLPlanesQueryFlags.Horizontal) != 0)
            {
                outDetectionMode |= PlaneDetectionMode.Horizontal;
            }

            if ((planesQueryFlags & MLXrPlaneSubsystem.MLPlanesQueryFlags.Vertical) != 0)
            {
                outDetectionMode |= PlaneDetectionMode.Vertical;
            }

            return outDetectionMode;
        }

        public static MLXrPlaneSubsystem.MLPlanesQueryFlags ToMLXrQueryFlags(this PlaneDetectionMode planeDetectionMode)
        {
            var outFlags = MLXrPlaneSubsystem.MLPlanesQueryFlags.None;
            if ((planeDetectionMode & PlaneDetectionMode.Horizontal) != 0)
            {
                outFlags |= MLXrPlaneSubsystem.MLPlanesQueryFlags.Horizontal;
            }

            if ((planeDetectionMode & PlaneDetectionMode.Vertical) != 0)
            {
                outFlags |= MLXrPlaneSubsystem.MLPlanesQueryFlags.Vertical;
            }

            return outFlags;
        }

        public static void ToMLXrOrientationsAndSemanticTypes(this MLXrPlaneSubsystem.MLPlanesQueryFlags flags, out IList<MLXrPlaneSubsystem.XrTypes.MLXrPlaneDetectorOrientation> orientations, out IList<MLXrPlaneSubsystem.XrTypes.MLXrPlaneDetectorSemanticType> semanticTypes)
        {
            orientations = new List<MLXrPlaneSubsystem.XrTypes.MLXrPlaneDetectorOrientation>();
            semanticTypes = new List<MLXrPlaneSubsystem.XrTypes.MLXrPlaneDetectorSemanticType>();

            if ((flags & MLXrPlaneSubsystem.MLPlanesQueryFlags.Horizontal) != 0)
            {
                orientations.Add(MLXrPlaneSubsystem.XrTypes.MLXrPlaneDetectorOrientation.HorizontalDownward);
                orientations.Add(MLXrPlaneSubsystem.XrTypes.MLXrPlaneDetectorOrientation.HorizontalUpward);
            }

            if ((flags & MLXrPlaneSubsystem.MLPlanesQueryFlags.Arbitrary) != 0)
            {
                orientations.Add(MLXrPlaneSubsystem.XrTypes.MLXrPlaneDetectorOrientation.Arbitrary);
            }

            if ((flags & MLXrPlaneSubsystem.MLPlanesQueryFlags.Vertical) != 0)
            {
                orientations.Add(MLXrPlaneSubsystem.XrTypes.MLXrPlaneDetectorOrientation.Vertical);
            }

            //Semantic types

            if ((flags & MLXrPlaneSubsystem.MLPlanesQueryFlags.SemanticCeiling) != 0)
            {
                semanticTypes.Add(MLXrPlaneSubsystem.XrTypes.MLXrPlaneDetectorSemanticType.Ceiling);
            }

            if ((flags & MLXrPlaneSubsystem.MLPlanesQueryFlags.SemanticFloor) != 0)
            {
                semanticTypes.Add(MLXrPlaneSubsystem.XrTypes.MLXrPlaneDetectorSemanticType.Floor);
            }

            if ((flags & MLXrPlaneSubsystem.MLPlanesQueryFlags.SemanticPlatform) != 0)
            {
                semanticTypes.Add(MLXrPlaneSubsystem.XrTypes.MLXrPlaneDetectorSemanticType.Platform);
            }

            if ((flags & MLXrPlaneSubsystem.MLPlanesQueryFlags.SemanticWall) != 0)
            {
                semanticTypes.Add(MLXrPlaneSubsystem.XrTypes.MLXrPlaneDetectorSemanticType.Wall);
            }
        }
    }
}
