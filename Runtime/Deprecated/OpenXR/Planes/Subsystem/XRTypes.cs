using System;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MLXrPlaneSubsystem
    {
        public static class XrTypes
        { 
            [Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.Planes.XrPlaneDetectorOrientation instead")]
            public enum MLXrPlaneDetectorOrientation : uint
            {
                HorizontalUpward,
                HorizontalDownward,
                Vertical,
                Arbitrary,
            }
            
            [Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.Planes.XrPlaneDetectorSemanticTypes instead")]
            public enum MLXrPlaneDetectorSemanticType : uint
            {
                Ceiling = 1U,
                Floor,
                Wall,
                Platform,
            }
            
            [Obsolete("MLXrPlaneDetectionState will be deprecated.")]
            public enum MLXrPlaneDetectionState : uint
            {
                None = 0,
                Pending,
                Done,
                Error,
                Fatal
            }
        }
    }
}
