namespace MagicLeap.OpenXR.Constants
{
    internal static class Values
    {
        internal const ulong InvalidHandle = 0xFFFFFFFFFFFFFFFF;
        internal const ulong NullHandle = 0U;
        internal const long NoDuration = 0U;
        internal const long InfiniteDuration = 0x7fffffffffffffffL;
        
        internal const string PluginName = "MagicLeapXrProvider";
    }
    
    public enum MLXrResult
    {
        GraphicsRequirementsCallMissing = -50,
        RuntimeUnavailable = -51,
        ControllerModelKeyInvalidMSFT = -1000055000,
        ReprojectionModeUnsupportedMSFT = -1000066000,
        ComputeNewSceneNotCompletedMSFT = -1000097000,
        SceneComponentIdInvalidMSFT = -1000097001,
        SceneComponentTypeMismatchMSFT = -1000097002,
        SceneMeshBufferIdInvalidMSFT = -1000097003,
        SceneComputefeatureincompatibleMSFT = -1000097004,
        SceneComputeConsistencyMismatchMSFT = -1000097005,
        DisplayRefreshRateUnsupportedFB = -1000101000,
        ColorSpaceUnsupportedFB = -1000108000,
        SpaceComponentNotSupportedFB = -1000113000,
        SpaceComponentNotEnabledFB = -1000113001,
        SpaceComponentStatusPendingFB = -1000113002,
        SpaceComponentStatusAlreadySetFB = -1000113003,
        UnexpectedStatePassthroughFB = -1000118000,
        FeatureAlreadyCreatedPassthroughFB = -1000118001,
        FeatureRequiredPassthroughFB = -1000118002,
        NotPermittedPassthroughFB = -1000118003,
        InsufficientResourcesPassthroughFB = -1000118004,
        UnknownPassthroughFB = -1000118050,
        RenderModelKeyInvalidFB = -1000119000,
        RenderModelUnavailableFB = 1000119020,
        MarkerNotTrackedVarjo = -1000124000,
        MarkerIdInvalidVarjo = -1000124001,
        SpatialAnchorNameNotFoundMSFT = -1000142001,
        SpatialAnchorNameInvalidMSFT = -1000142002,
        SpaceMappingInsufficientFB = -1000169000,
        SpaceLocalizationFailedFB = -1000169001,
        SpaceNetworkTimeoutFB = -1000169002,
        SpaceNetworkRequestFailedFB = -1000169003,
        SpaceCloudStorageDisabledFB = -1000169004,
        PassthroughColorLutBufferSizeMismatchMETA = -1000266000,
        HintAlreadySetQCOM = -1000306000,
        SpaceNotLocatableEXT = -1000429000,
        PlaneDetectionPermissionDeniedEXT = -1000429001,
        FuturePendingEXT = -1000469001,
        FutureInvalidEXT = -1000469002,
        FacialExpressionPermissionDeniedML = 1000482000,
        LocalizationMapIncompatibleML = -1000139000,
        LocalizationMapUnavailableML = -1000139001,
        LocalizationMapFailML = -1000139002,
        LocalizationMapImportExportPermissionDeniedML = -1000139003,
        LocalizationMapPermissionDeniedML = -1000139004,
        LocalizationMapAlreadyExistsML = -1000139005,
        LocalizationMapCannotExportCloudMapML = -1000139006,
        MarkerDetectorPermissionDeniedML = -1000138000,
        MarkerDetectorLocateFailedML = -1000138001,
        MarkerDetectorInvalidDataQueryML = -1000138002,
        MarkerDetectorInvalidCreateInfoML = -1000138003,
        MarkerInvalidML = -1000138004,
        SpatialAnchorsPermissionDeniedML = -1000140000,
        SpatialAnchorsNotLocalizedML = -1000140001,
        SpatialAnchorsOutOfMapBoundsML = -1000140002,
        SpatialAnchorsAnchorNotFoundML = -1000141000,
        SystemNotificationPermissionDeniedML = -1000473000,
        SystemNotificationIncompatibleSKUML = -1000473001,
        WorldMeshDetectorPermissionDeniedML = -1000474000,
        WorldMeshDetectorSpaceNotLocatableML = -1000474001,
        PixelSensorPermissionDeniedML = -1000475000,
        PixelSensorNotSupportedML = -1000475001,
        PixelSensorNotAvailableML = 1000475002,
        PixelSensorCapabilityNotSupportedML = 1000475003,
        PixelSensorSpaceNotSupportedML = 1000475004,
        LightEstimationPermissionDeniedML = -1000481000,
        LightEstimationRGBSensorNotAvailableML = -1000481001,
        EyeTrackerPermissionDeniedML = -1000484000,
        PupilSizePermissionDeniedML = -1000484001
    }
}
