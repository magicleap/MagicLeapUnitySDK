namespace MagicLeap.OpenXR.Features.Planes
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
