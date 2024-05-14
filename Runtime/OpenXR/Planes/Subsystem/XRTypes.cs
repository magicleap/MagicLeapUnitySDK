using System;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MLXrPlaneSubsystem
    {
        public static class XrTypes
        { 
            [Obsolete("MLXrPlaneDetectorOrientation will be deprecated. Please use MagicLeapPlanesTypes.XrPlaneDetectorOrientation instead")]
            public enum MLXrPlaneDetectorOrientation : uint
            {
                HorizontalUpward,
                HorizontalDownward,
                Vertical,
                Arbitrary,
            }
            
            [Obsolete("MLXrPlaneDetectorSemanticType will be deprecated. Please use MagicLeapPlanesTypes.XrPlaneDetectorSemanticTypes instead")]
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
