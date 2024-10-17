using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Features.EyeTracker
{
    /// <summary>
    /// The type of eye (left or right).
    /// </summary> 
    public enum Eye
    {
        Left = 1,
        Right = 2
    }

    /// <summary>
    /// The confidence level of an eye tracker.
    /// </summary>
    public enum Confidence
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    /// <summary>
    /// The type of behavior for an eye gaze.
    /// </summary>
    public enum GazeBehaviorType
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

    /// <summary>
    /// Static data for a given hardware, such as width and height of the camera sensors.
    /// </summary>
    public struct StaticData
    {
        public XrResult Result;
        public float EyeWidthMax;
        public float EyeHeightMax;
    }

    /// <summary>
    /// Provides information about geometric data such as openness of the eye.
    /// </summary>
    public struct GeometricData
    {
        public XrResult Result;
        public Eye Eye;
        public long Time;
        public bool Valid;
        public float EyeOpenness;
        public Vector2 EyeInSkullPosition;
    }

    /// <summary>
    /// Provides information about pupil size. This data is gated by a dedicated 
    /// permission, different from permission required for the rest of the eye tracking system.
    /// </summary>
    public struct PupilData
    {
        public XrResult Result;
        public Eye Eye;
        public long Time;
        public bool Valid;
        public float PupilDiameter;
    }

    /// <summary>
    /// Provides information about eye behaviors.
    /// </summary>
    public struct GazeBehavior
    {
        public XrResult Result;
        public GazeBehaviorMetaData MetaData;
        public long Time;
        public bool Valid;
        public GazeBehaviorType GazeBehaviorType;
        public long OnsetTime;
        public ulong Duration;
    }

    /// <summary>
    /// Metadata about the gaze behavior, such as amplitude, direction, and velocity.
    /// </summary>
    public struct GazeBehaviorMetaData
    {
        public bool Valid;
        public float Amplitude;
        public float Direction;
        public float Velocity;
    }
    
    /// <summary>
    /// Pose data associated with a pose (e.g. gaze, left, right, vergence).
    /// </summary>
    public struct PoseData
    {
        public bool Valid;
        public Pose Pose;
        public long Time;
        public Confidence Confidence;
    }

    /// <summary>
    /// Poses data of an eye tracker.
    /// </summary>
    public struct PosesData
    {
        public XrResult Result;
        public PoseData GazePose;
        public PoseData LeftPose;
        public PoseData RightPose;
        public PoseData FixationPose;
    }

    /// <summary>
    /// All of the types of data associated with an eye tracker.
    /// </summary>
    public struct EyeTrackerData
    {
        public StaticData StaticData;
        public GeometricData[] GeometricData;
        public PupilData[] PupilData;
        public GazeBehavior GazeBehaviorData;
        public PosesData PosesData;
    }
}
