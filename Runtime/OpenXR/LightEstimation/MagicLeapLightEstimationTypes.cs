// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using UnityEngine;

namespace MagicLeap.OpenXR.Features.LightEstimation
{
    /// <summary>
    /// The width and height dimension resolution of an HDR Cubemap.
    /// </summary>
    public enum HDRCubemapFaceResolution
    {
        Resolution_64x64,
        Resolution_128x128,
        Resolution_256x256
    }

    /// <summary>
    /// The data of the HDR cubemap as it is obtained from a light estimation estimate.
    /// </summary>
    public struct HDRCubemapData
    {
        /// <summary>
        /// An array of pixel data of the cubemap. The data is in the order of (R, G, B, A) with values 
        /// between 0 and 1, and is and organized into 6 faces in the order of (+X, -X, +Y, -Y, +Z, -Z).
        /// </summary>
        public float[] Pixels;

        /// <summary>
        ///  The width/height dimension resolution of each face on the cubemap.
        /// </summary>
        public uint FaceDimension;
    }

    /// <summary>
    /// The direction and color of the main directional light data obtained from the light estimation estimate.
    /// </summary>
    public struct MainDirectionalLight
    {
        /// <summary>
        /// A normalized vector pointing in the direction that the main light faces.
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        /// The color value for the given main light.
        /// </summary>
        public Color Color;
    }

    /// <summary>
    /// All of the data obtained from a light estimation estimate.
    /// </summary>
    public struct EstimateData
    {
        /// <summary>
        /// The data of the HDR cubemap.
        /// </summary>
        public HDRCubemapData CubeMap;

        /// <summary>
        /// The direction and color of the main directional light data.
        /// </summary>
        public MainDirectionalLight DirectionalLight;

        /// <summary>
        /// The spherical harmonics coefficients data.
        /// </summary>
        public float[] HarmonicsCoefficients;

        /// <summary>
        /// The timestamp of the light estimation estimate in nanoseconds.
        /// </summary>
        public long TimeStampNanoSeconds;
    }
}
