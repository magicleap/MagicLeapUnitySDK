namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    namespace MagicLeapPlanesTypes
    {
        public enum XrPlaneDetectorOrientation : uint
        {
            HorizontalUpward,
            HorizontalDownward,
            Vertical,
            Arbitrary
        }

        public enum XrPlaneDetectorSemanticTypes : uint
        {
            Undefined,
            Ceiling,
            Floor,
            Wall,
            Platform
        }
    }
   
}
