// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
﻿using System;

namespace MagicLeap.OpenXR.Features.UserCalibration
{
    internal enum XrUserCalibrationStructTypes : uint
    {
        EventDataHeadsetFitChanged = 1000472000U,
        EventDataEyeCalibrationChanged = 1000472001U,
        UserCalibrationEnableEventsInfo = 1000472002U
    }

    internal enum XrHeadsetFitStatus : uint
    {
        HeadsetFitStatusUnknown = 0,
        HeadsetFitStatusNotWorn = 1,
        HeadsetFitStatusGoodFit = 2,
        HeadsetFitStatusBadFit = 3
    }

    internal enum XrEyeCalibrationStatus : uint
    {
        EyeCalibrationStatusUnknown = 0,
        EyeCalibrationStatusNone = 1,
        EyeCalibrationStatusCoarse = 2,
        EyeCalibrationStatusFine = 3
    }

    internal struct XrEventDataHeadsetFitChanged
    {
        internal XrUserCalibrationStructTypes Type;
        internal IntPtr Next;
        internal XrHeadsetFitStatus Status;
        internal long Time;
    }

    internal struct XrEventDataEyeCalibrationChanged
    {
        internal XrUserCalibrationStructTypes Type;
        internal IntPtr Next;
        internal XrEyeCalibrationStatus Status;
    }

    internal struct XrUserCalibrationEnableEventsInfo
    {
        internal XrUserCalibrationStructTypes Type;
        internal IntPtr Next;
        internal uint Enabled;
    }
}
