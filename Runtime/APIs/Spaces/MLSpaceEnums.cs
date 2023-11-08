// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2023) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLSpace
    {
        /// <summary>
        /// Indicates the type of the Space.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// On device space.
            /// </summary>
            OnDevice,

            /// <summary>
            /// AR Cloud based space.
            /// </summary>
            ARCloud
        }

        /// <summary>
        /// The current localization status.
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// The device is currently not localized.
            /// </summary>
            NotLocalized,

            /// <summary>
            /// The device has localized successfully.
            /// </summary>
            Localized,

            /// <summary>
            /// Localization attempt is currently in progress.
            /// </summary>
            LocalizationPending,

            /// <summary>
            /// The device will attempt to localize after sleep duration is complete.
            /// </summary>
            SleepingBeforeRetry,
        }

        /// <summary>
        /// The confidence of the current localization.
        /// </summary>
        public enum LocalizationConfidence
        {
            /// <summary>
            /// Localization confidence is very poor and should be reattempted.
            /// </summary>
            Poor,

            /// <summary>
            /// The confidence is low, current environmental conditions may adversely affect localization.
            /// </summary>
            Fair,

            /// <summary>
            /// The confidence is high, persistent content should be stable.
            /// </summary>
            Good,

            /// <summary>
            /// This is a very high-confidence localization, persistent content will be very stable.
            /// </summary>
            Excellent
        }

        /// <summary>
        /// A set of possible reasons that a localization attempt may be unsuccessful.
        /// </summary>
        [Flags]
        public enum LocalizationErrorFlag
        {
            /// <summary>
            /// No error, localization was successful.
            /// </summary>
            None = 0,

            /// <summary>
            /// Localization failed for an unknown reason.
            /// </summary>
            Unknown = 1 << 0,

            /// <summary>
            /// Localization failed becuase the user is outside of the mapped area.
            /// </summary>
            OutOfMappedArea = 1 << 1,

            /// <summary>
            /// There are not enough features in the environment to successfully localize.
            /// </summary>
            LowFeatureCount = 1 << 2,

            /// <summary>
            /// Localization failed due to excessive motion.
            /// </summary>
            ExcessiveMotion = 1 << 3,

            /// <summary>
            /// Localization failed because the lighting levels are too low in the environment.
            /// </summary>
            LowLight = 1 << 4,

            /// <summary>
            /// A headpose failure caused localization to be unsuccessful.
            /// </summary>
            HeadposeFailure = 1 << 5,

            /// <summary>
            /// There was an internal algorithm failure that prevented localization.
            /// </summary>
            AlgorithmFailure = 1 << 6
        }
    }
}
