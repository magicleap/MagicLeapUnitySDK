using System;
using UnityEngine;

namespace MagicLeap.OpenXR.Features.EyeTracker
{
    internal enum XrEye
    {
        Left = 1,
        Right = 2
    }
    
    internal enum XrEyeTrackerPoseType
    {
        Gaze = 0,
        Left = 1,
        Right = 2,
        Fixation = 3,
    }

    internal enum XrEyeTrackerConfidence : uint
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

    [Flags]
    internal enum XrEyeTrackerPoseFlags : ulong
    {
        Gaze = 0x00000001,
        Left = 0x00000002,
        Right = 0x00000004,
        Fixation = 0x00000008
    }

    internal enum XrEyeTrackerObjectTypes : uint
    {
        XrObjectTypeEyeTracker = 1000484000U
    }

    internal enum XrEyeTrackerStructTypes : uint
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
        XrTypeSystemEyeTrackerProperties = 1000484011U,
        XrTypeEyeTrackerPosesGetInfo = 1000484012U,
        XrTypeEyeTrackerPoses = 1000484013U,
        XrTypeEyeTrackerPose = 1000484014U,
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
        internal float Angle;
        internal float Velocity;
    }

    internal struct XrEyeTrackerPosesGetInfo
    {
        internal XrEyeTrackerStructTypes Type;
        internal IntPtr Next;
        internal XrSpace BaseSpace;
        internal long Time;
        internal XrEyeTrackerPoseFlags PoseFlags;
    }

    internal struct XrEyeTrackerPose
    {
        internal XrEyeTrackerStructTypes Type;
        internal IntPtr Next;
        internal XrBool32 Valid;
        internal XrPose Pose;
        internal long Time;
        internal XrEyeTrackerConfidence Confidence;
    }

    internal struct XrEyeTrackerPoses
    {
        internal XrEyeTrackerStructTypes Type;
        internal IntPtr Next;
        internal XrEyeTrackerPose GazePose;
        internal XrEyeTrackerPose LeftPose;
        internal XrEyeTrackerPose RightPose;
        internal XrEyeTrackerPose FixationPose;
        
        internal static XrEyeTrackerPoses Create()
        {
            var result = new XrEyeTrackerPoses()
            {
                Type = XrEyeTrackerStructTypes.XrTypeEyeTrackerPoses,
            };
            var pose = new XrEyeTrackerPose()
            {
                Type = XrEyeTrackerStructTypes.XrTypeEyeTrackerPose
            };

            result.GazePose = pose;
            result.LeftPose = pose;
            result.RightPose = pose;
            result.FixationPose = pose;
            
            return result;
        }
    }
}
