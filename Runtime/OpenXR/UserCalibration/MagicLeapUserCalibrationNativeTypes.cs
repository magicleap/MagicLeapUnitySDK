using System;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    namespace MagicLeapUserCalibrationNativeTypes
    {
        internal enum XrUserCalibrationStructTypes : ulong
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
}
