// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2022-2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;

    public sealed partial class MLEyeCamera
    {
        /// <summary>
        /// Enumeration of all the available eye camera sensors.
        /// </summary>
        [Flags]
        public enum MLEyeCameraIdentifier
        {
            /// <summary>
            /// None.
            /// </summary>
            None = 0,

            /// <summary>
            /// Left temple eye camera.
            /// </summary>
            LeftTemple = 1 << 0,

            /// <summary>
            /// Left nasal eye camera.
            /// </summary>
            LeftNasal = 1 << 1,

            /// <summary>
            /// Right nasal eye camera.
            /// </summary>
            RightNasal = 1 << 2,

            /// <summary>
            /// Right temple eye camera.
            /// </summary>
            RightTemple = 1 << 3,

            /// <summary>
            /// All Eye cameras.
            /// </summary>
            All = LeftTemple | LeftNasal | RightNasal | RightTemple
        }
    }
}
