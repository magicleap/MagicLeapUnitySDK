using System;
using UnityEngine;

namespace MagicLeap.OpenXR.Features.EyeTracker
{
    internal enum XrEye
    {
        Left = 1,
        Right = 2
    }

    internal enum XrEyeTrackerConfidence
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    internal enum XrEyeTrackerGazeBehaviorType
    {
        Unknown = 0,
        EyesClosed = 1,
        Blink = 2,
        BlinkLeft = 3,
        BlinkRight = 4,
        Fixation = 5,
        Pursuit = 6,
        Saccade = 7
    }

    internal enum XrEyeTrackerObjectTypes : ulong
    {
        XrObjectTypeEyeTracker = 1000484000U
    }

    internal enum XrEyeTrackerStructTypes : ulong
    {
        XrTypeEyeTrackerCreateInfo = 1000484001U,
        XrTypeEyeTrackerStaticDataGetInfo = 1000484002U,
        XrTypeEyeTrackerStaticData = 1000484003U,
        XrTypeEyeTrackerGazeBehaviorGetInfo = 1000484004U,
        XrTypeEyeTrackerGazeBehavior = 1000484005U,
        XrTypeEyeTrackerGazeBehaviorMetaData = 1000484006U,
        XrTypeEyeTrackerGeometricDataGetInfo = 1000484007U,
        XrTypeEyeTrackerGeometricData = 1000484008U,
        XrTypeEyeTrackerPupilDataGetInfo = 1000484009U,
        XrTypeEyeTrackerPupilData = 1000484010U,
        XrTypeEyeTrackerMetaData = 1000484011U,
        XrTypeSystemEyeTrackerProperties = 1000484012U
    }

    internal struct XrSystemEyeTrackerProperties
    {
        internal XrEyeTrackerStructTypes Type;
        internal IntPtr Next;
        internal XrBool32 SupportsEyeTracking;
    }

    internal struct XrEyeTrackerCreateInfo
    {
        internal XrEyeTrackerStructTypes Type;
        internal IntPtr Next;
    }

    internal struct XrEyeTrackerStaticDataGetInfo
    {
        internal XrEyeTrackerStructTypes Type;
        internal IntPtr Next;
    }

    internal struct XrEyeTrackerStaticData
    {
        internal XrEyeTrackerStructTypes Type;
        internal IntPtr Next;
        internal float EyeWidthMax;
        internal float EyeHeightMax;
    }

    internal struct XrEyeTrackerGeometricDataGetInfo
    {
        internal XrEyeTrackerStructTypes Type;
        internal IntPtr Next;
    }

    internal struct XrEyeTrackerGeometricData
    {
        internal XrEyeTrackerStructTypes Type;
        internal IntPtr Next;
        internal XrEye Eye;
        internal long Time;
        internal XrBool32 Valid;
        internal float EyeOpenness;
        internal Vector2 EyeInSkullPosition;
    }

    internal struct XrEyeTrackerPupilDataGetInfo
    {
        internal XrEyeTrackerStructTypes Type;
        internal IntPtr Next;
    }

    internal struct XrEyeTrackerPupilData
    {
        internal XrEyeTrackerStructTypes Type;
        internal IntPtr Next;
        internal XrEye Eye;
        internal long Time;
        internal XrBool32 Valid;
        internal float PupilDiameter;
    }

    internal struct XrEyeTrackerGazeBehaviorGetInfo
    {
        internal XrEyeTrackerStructTypes Type;
        internal IntPtr Next;
    }

    internal struct XrEyeTrackerGazeBehavior
    {
        internal XrEyeTrackerStructTypes Type;
        internal IntPtr Next;
        internal long Time;
        internal XrBool32 Valid;
        internal XrEyeTrackerGazeBehaviorType GazeBehaviorType;
        internal long OnsetTime;
        internal ulong Duration;
    }

    internal struct XrEyeTrackerGazeBehaviorMetaData
    {
        internal XrEyeTrackerStructTypes Type;
        internal IntPtr Next;
        internal XrBool32 Valid;
        internal float Amplitude;
        internal float Direction;
        internal float Velocity;
    }

    internal struct XrEyeTrackerMetadata
    {
        internal XrEyeTrackerStructTypes Type;
        internal IntPtr Next;
        internal XrBool32 Valid;
        internal long Time;
        internal XrEyeTrackerConfidence Confidence;
    }
}
