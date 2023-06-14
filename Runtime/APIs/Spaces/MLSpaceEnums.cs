// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2023) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

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


    }
}
