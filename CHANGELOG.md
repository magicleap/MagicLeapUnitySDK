# Changelog

## [2.5.0]

### Bugfixes
- Fixed an issue with `MagicLeapSpatialAnchorsFeature` where anchor creation would sometimes fail.

### Deprecations & Removals
- Deprecated `UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MLXrPlaneSubsystem`. Use `MagicLeap.OpenXR.Subsystems.MLXrPlaneSubsystem` instead.

### Features
- Removed interaction profile data from `MagicLeapEyeTrackerFeature` and added ability to query tracker metadata.

## [2.4.0]

### Bugfixes 
- Fixed an issue with `MagicLeapMarkerUnderstandingFeature` where a marker's pose reverts to a default value instead of null.
- Fixed an issue with `MagicLeapLightEstimationFeature` where a crash occurs when switching between cube map resolutions.
- Fixed an issue with `MagicLeapPixelSensorFeature.ConfigureSensorWithDefaultCapabilities` function where it would fail for sensors with AutoExposure capabilities.
- Fixed a crash when using `Magic Leap 2 Physical Occlusion` with editor versions starting from `2022.3.25f1` onward.
- Fixed an issue with `MagicLeapUserCalibrationFeature` where it reports incorrect `EyeCalibrationData.Status`.


### Deprecations & Removals
- Deprecated `YcbcrRenderer` and its associated classes. Please use `YCbCrRenderer` instead.

### Features
- Added `Magic Leap 2 Physical Occlusion` OpenXR feature.
- Added `Magic Leap 2 Reprojection` OpenXR feature.
- Samples have been added for select OpenXR features.
- Added `Magic Leap 2 Eye Tracker` OpenXR feature.
- Added `com.magicleap.permission.USB_SIPSOP` to the `MagicLeap/Permissions` section of Project Settings. 
- Added `YCbCrHardwareBufferRenderer` that uses `AHardwareBuffer` as its input.
- Added `MagicLeapPixelSensorRealityMode` enum to the Pixel Sensor API.
- Undeprecated `MLCamera` API. 
- Added `EstimateData.HarmonicsCoefficients` to the Light Estimation feature.

### Misc.
- Changed default value for Blend Mode in `Magic Leap 2 Rendering Extensions` to `Additive`. 

## [2.3.0]

### Features
- Update OpenXR Plugin dependency to `1.11.0`.
- Added `GetMapOrigin()` method to `MagicLeapLocalizationMapFeature` to allow users to get the pose of the map's origin.
- Added `GetPixelSensorFromXrPath()` to MagicLeapPixelSensorFeature to get a `PixelSensorId` based on an XrPath string.
- Added `Magic Leap 2 Secondary View Support` OpenXR Feature.
- Migrated the `XR_ML_view_configuration_depth_range_change` extension into its own separate OpenXR Feature as the extension is experimental.

### Improvements
- Updated Magic Leap Feature sets to visually distinguish in the UI between experimental and non-experimental OpenXR extensions. 
- Reorganized classes and types into new `MagicLeap.OpenXR` namespaces, separating Magic Leap code from `UnityEngine` and `UnityEditor` code.
- Adjusted `MagicLeapSpatialAnchorsStorageFeature` to work with the `XRAnchorSubsystem` automatically and no longer depends on manual adding and deleting. All code that currently does this should be adjusted and it is encouraged to use the built in `ARAnchorManager.anchorsChanged` event.
- Added `CaptureTime` to `PixelSensorFrame` to get the capture time of the corresponding frame for `MagicLeapPixelSensorFeature`.

### Bugfixes
- `MagicLeapSpatialAnchorStorageFeature.OnQueryComplete` will now be invoked when 0 anchors are found for a query.
- Fixed issue where a DllNotFoundException would prevent an application from launching if the `Magic Leap XR Plugin` package is installed but its XR Provider is not enabled.
- Fixed issue of potentially corrupted Cubemap for `MagicLeapLightEstimationFeature.GetEstimateCubemap`

### Deprecations & Removals
- All classes inside `UnityEngine.XR.OpenXR.Features.MagicLeapSupport` namespaces have been marked Obsolete
- Deprecated `MagicLeapMeshingFeature.GetMeshIds(out TrackableId[])`. Use `MagicLeapMeshingFeature.GetMeshIds(out MeshId[])` instead.
- Deprecated `MagicLeapMeshingFeature.GetMeshData(in TrackableId,...)`. Use `MagicLeapMeshingFeature.GetMeshData(in MeshId,...)` instead.
- Deprecated `MLXrAnchorSubsystem.GetAnchorPoseFromID`. Use `MLXrAnchorSubsystem.GetAnchorPoseFromId` instead.
- Deprecated `MagicLeapSpatialAnchorsStorageFeature.DeleteStoredSpatialAnchor`. Use `MagicLeapSpatialAnchorsStorageFeature.DeleteStoredSpatialAnchors` instead.
- Temporarily Removing `MagicLeap.OpenXR.Features.LightEstimation.EstimateData.HarmonicsCoefficients` until fully implemented.

### Misc.
- Increased minimum Unity version to `2022.3` to be in line with the oldest LTS supported for Magic Leap by Unity.
- Updated LICENSE

## [2.2.0]

### Features
- Added `Magic Leap 2 Light Estimation` OpenXR Feature.
- Added `.GetTimestamps()` method to `AndroidCamera` API to allow users to determine/validate capture rate.
- Added `Magic Leap 2 Pixel Sensor` OpenXR Feature
- Added new `MagicLeapController.cs` example script to the `ML Rig & Inputs` package sample.

### Bugfixes
- Fixed code paths in MagicLeapCamera script to better support legacy Magic Leap XR Plugin workflow.
- Resolved issue where sometimes Marker Detectors would receive `MarkerInvalidML` errors.
- Fixed issue where the `Permissions` section of Magic Leap's Project Settings would fail to render and mention a missing MLSDK. 
- Resolved unnecessary warnings from AR Foundation about missing Camera and Raycast subsystems.
- Fixed issue where `MarkerData.MarkerString` was limited to 100 characters.
- Made DestroyClient method in `Magic Leap 2 Facial Expressions` OpenXR Feature get called automatically to prevent error if done manually in OnDestroy.

## [2.1.0]

### Features
- Update OpenXR Plugin dependency to `1.10.0`.
- Added `Magic Leap 2 Facial Expressions` OpenXR Feature.
- Added `Magic Leap 2 Environmental Meshing` OpenXR feature.
- Added `Magic Leap 2 Spatial Anchors` OpenXR Feature
- Added `Magic Leap 2 Spatial Anchors Storage` OpenXR Feature
- Added `MagicLeap.Android.Permissions`, a new and improved Permissions API.
- Included an AprilTagSettings struct to the `Magic Leap 2 Marker Understanding` OpenXR Feature

### Experimental
- Added `AndroidCamera` APIs for performing basic YUV and JPEG Camera capture.

### Bugfixes
- Fixed issue where JPEG screen capture with `MLCamera` was not displaying an image.
- Fixed legacy `MLPlanes` subsystem not being initialized when using Magic Leap XR Provider.
- Added project validation rules to check for Player Settings required by Magic Leap 2.
- Fixed `MLCVCamera` being unable to query Headpose from the MLSDK when the OpenXR PRovider is active.
- Fixed `MLMarkerTracker` issue where detected `AprilTag` markers are flipped.

### Deprecations & Removals
- Removed Preferences>External Tools>Magic Leap.
- Marked `MLPermissions` Obsolete. Use `MagicLeap.Android.Permissions` instead.
- Marked `MagicLeapLocalizationMapFeature.ExportLocalizatioMap` obsolete. Use `MagicLeapLocalizationMapFeature.ExportLocalizationMap` instead.
- Marked `MagicLeapUserCalibrationFeature.HeadsetFitStatus.status` obsolete. Use `MagicLeapUserCalibrationFeature.HeadsetFitStatus.Status` instead.
- Marked `MagicLeapUserCalibrationFeature.HeadsetFitStatus.time` obsolete. Use `MagicLeapUserCalibrationFeature.HeadsetFitStatus.Time` instead.
- Marked `MagicLeapUserCalibrationFeature.EyeCalibrationData.status` obsolete. Use `MagicLeapUserCalibrationFeature.EyeCalibrationData.Status` instead.
- Marked `MLXrPlaneSubsystem.XrTypes.MLXrPlaneDetectorOrientation` obsolete. Use `MagicLeapPlanesTypes.XrPlaneDetectorOrientation` instead.
- Marked `MLXrPlaneSubsystem.XrTypes.MLXrPlaneDetectorSemanticType` obsolete. Use `MagicLeapPlanesTypes.XrPlaneDetectorSemanticTypes` instead.
- Marked `MagicLeapSpatialAnchorsStorageFeature.UpdateExpirationonStoredSpatialAnchor` Obsolete. Use `MagicLeapSpatialAnchorsStorageFeature.UpdateExpirationForStoredSpatialAnchor` instead.
- Marked `MagicLeapFeature.ConvertTimestampToXrTime` obsolete. Use `MagicLeapFeature.ConvertSystemTimeToXrTime` instead.
- Marked `MagicLeapFeature.ConvertXrTimeToTimestamp` obsolete. Use `MagicLeapFeature.ConvertXrTimeToSystemTime` instead.

### Known Issues
- `MLAnchors` API does not work when the `Magic Leap 2 Spatial Anchors` or `Magic Leap 2 Spatial Anchors Storage` OpenXR Features are enabled.

### Misc.
- Renamed the ML Rig & Inputs Sample input actions asset as well as the old `MagicLeapInputs` asset to make it clear what the differences are.

## [2.0.0]

### Features
- Replace the Magic Leap XR Plugin with the OpenXR Plugin as the direct XR provider dependency package. If you'd wish to continue using the Magic Leap XR Plugin, install it from the Package Manager, and ensure it is enabled as the sole XR provider in Project Settings/XR Plug-in Management.
- Added `Magic Leap 2 Support` OpenXR Feature
- Added `Magic Leap 2 Localization Maps` OpenXR Feature
- Added `Magic Leap 2 Marker Understanding` OpenXR Feature
- Added `Magic Leap 2 Plane Detection` OpenXR Feature
- Added `Magic Leap 2 Reference Spaces` OpenXR Feature
- Added `Magic Leap 2 Rendering Extensions` OpenXR Feature
- Added `Magic Leap 2 System Notification Control` OpenXR Feature
- Added `Magic Leap 2 User Calibration` OpenXR Feature
- Added `Magic Leap 2 Controller Interaction Profile` for OpenXR
- Added `ML Rig & Inputs` Sample
- Changed `MLMarkerTracker` default settings behavior to not begin marker tracking immediately. Also set the default MarkerType to None when creating the `MLMarkerTracker.TrackerSettings` struct.

### Experimental
- Added `Magic Leap 2 Spatial Anchors` and `Magic Leap 2 Spatial Anchors Storage` OpenXR Features

### Bugfixes
- Fixed an issue in `MLGestureClassification` where hand transform and interaction points would freeze in place when Posture type was None.
- Added missing comments describing parameters and returns to functions in `MLSpace`.
- Fixed issue in `MLMediaPlayerBehavior` where HLS streams would not play after resolution changes.
- Fixed issue in `MLMediaPlayerBehavior` where DASH streams would not display video or audio.
- Fixed issue in `MLCamera` that resulted in crashes & artifacts when rendering YUV capture to a RenderTexture.
- Fixed issue in `MLCamera` that resulted in crashes when waking from sleep.
- Fixed issue with legacy Hands subsystem causing compilation errors in MRTK projects.
- Fixed issue with Segmented Dimmer not functioning correctly
- Improved performance of `MLEyeCamera`
- Fixed issue with MagicLeapCamera.cs performing expensive JNI operations every frame leading to an eventual crash in some apps.

### Deprecations & Removals
- `MLCamera`: Deprecated `MLCamera` API. Developers will need to utilize the Android SDK Camera2 API or the NDK Camera API instead.

## [1.12.0]

### Features
- Added Localization Confidence and Localization Errors to `MLSpace` API
- Added Quality Confidence to `MLAnchors` API
- Added the ability to change the `nearClipPlane` of the Unity Camera with respect to the minimum distance selected in the device's system settings.
- `MLMarkerTracker` function returns changed from `Task` to `Task<MLResult>`. Exposed boolean `IsScanning` to determine if markers are currently being scanned.

### Bugfixes
- Fixed an issue where Vulkan would fail to initialize if Unity's WebRTC package (`com.unity.webrtc`) is installed in the project.
- Fixed issue where MLMediaPlayerBehavior wasn't outputting warning logs

### Deprecations & Removals
- `MLMediaPlayer`: Deprecated `MLMedia` API and `MLMediaPlayerBehavior`. Developers needing to play video files are encouraged to upgrade their project to Unity 2022.3.10 and use the [VideoPlayer component](https://docs.unity3d.com/Manual/class-VideoPlayer.html).

## [1.11.0]

### Features
- Remove requirement to be called from main thread from MLAudioPlayback's CreateAudioBuffer method.

## [1.10.0]

### Features
- Added `MLOcclusion` API.
- Added public properties `MagicLeapCamera.EnforceFarClip` and `MagicLeapCamera.RecenterXROriginAtStart`

### Bugfixes
- `MLSegmentedDimmer`: Fixed crash when playing scene twice in editor.
- `MLMediaPlayerBehavior`: Fixed unresponsive UI after pressing stop button in `MediaPlayer` example.
- Added missing dropdown for short-range depth camera in `DepthCamera` example.
- `MLCamera`: Fixed error on sleep mode and doze mode.
- `MLCameraBase`: Fixed failure to render preview capture more than once.
- `MLNativeSurface`: Fixed AccessRenderBufferTexture() rendering failure when reusing same player.
- `MLMediaPlayer`: Fixed erroneuous error logging on pending result from Streaming Assets path prepare.
- Fixed `MLMeshing` on map reset.
- `MLMeshing`: Fixed Null Reference Exception on `Meshing` example scene start up.
- Fixed second disconnect attempt in `WebRTC` example.
- `HandTrackingExample`: Fixed `PoseNotFound` errors that might occur after `HandTracking` scene changes.
- `MLGestureClassification`: Fixed errors thrown when hands not detected.
- `MLUnityNativeLogging`: Conditionally reduced log level of snapshot errors based on build configuration.
- `GraphicsHook`: Added cleanup logic that resets the snapshot prediction state of the input subsystem.
- `MLCamera`: Fixed `InvalidParam` error that occur when trying to capture after application pause.
- `CameraCaptureExample`: Fixed `NullReferenceException` when pausing example scene.

### Deprecations & Removals
- `MLWebRTC`: Deprecated `MLWebRTC`

### Known Issues

## [1.9.0]

### Features
- `MLDepthCamera`: Added support for switching to Short Range streaming mode and to change FPS and exposure values.
- `MLCamera`: Added events for notifying when the camera is paused and resumed.

### Bugfixes
- `MLWebRTC`: Fixed camera disconnection when using `MLNativeSurface` based buffers for rendering.
- Fixed `MLWorldCamera` error on application quit.

## [1.8.0]
### Features
- `MLEyes`: Updated `leftEyeOpenAmount` and `rightEyeOpenAmount` on `UnityEngine.InputSystem.XR.Eyes` to return values between 0.0 and 1.0.
- Added support for `EyeHeightMax` and `EyeWidthMax` in `MLGazeRecognitionStaticData`
- `MLSpace`: Added a new API for importing and exporting spaces without the need for scanning the environment.
- `MLDepthCamera`: Added `RawDepthImage` to support raw camera stream with IR Projector ON.
- Added `MLEyeCamera` API.
- `MLPowerManager`: Added a new API to obtain current controller state and control controller state transitions.
- `MLFacialExpression`: Added a new API to obtain eye expressions data.
- `MLHeadTracking`: Modified headpose lost notifications with new API for MLHeadTrackingStateEx.
- Added `MLPerceptionGetPredictedSnapshot` and `EnableSnapshotPrediction` to allow controller pose prediction.

### Bugfixes

### Deprecations & Removals
- `MLHeadTracking`: MLHeadTrackingState and related dependencies marked Obsolete.

### Known Issues
- `MLWebRTC`: When disconnecting from a session, the camera does not shut down cleanly if the NativeSurface buffer format was used, causing the application to hang for as much as 30 seconds.

## [1.7.0]
### Features

### Bugfixes
- Fixed compiler errors when the package `com.unity.xr.openxr` is also installed.
- Fixed `MLDepthCamera` errors when closing the application.
- Improved performance of `MLWebRTC.MLCameraVideoSource` by making it start the video capture asynchronously.
- Fixed enforcement of MLCamera's near clip distance when "Fix problems on startup" is enabled
- Fixed occasional crash when changing scenes while `MLMediaPlayer` is playing local video file
- Addressed `MLAnchors` query timing to reduce likelihood of missing pose errors.
- Fixed CV camera errors when resuming from sleep mode
- Fixed CV camera errors when resuming from menu
- Fixed `MLWorldCamera` Invalid Parameter error spams.
- Renamed Tests assembly definition.
- Fixed caching logic for `CustomHapticsPattern`s to avoid using incorrect cached patterns.
- Fixed `MLVoice.IntentEvent`'s EventSlotsUsed list within the `OnVoiceEvent` so it properly lists all Slots used in the voice command.
- Optimized `MLMeshing` API and components to reduce memory usage.
- Fixed spamming errors caused by not detecting eyes in eye tracking.

### Deprecations & Removals
- The MLMediaDRM API has been marked as `Obsolete` and will be removed in a future release.
- `MLWebView.GetScrollSize()` and `MLWebView.GetScrollOffset()` have been marked as `Obsolete` and will be removed in a future release.
- Removed the ability to turn off near clipping plane enforcement in `MagicLeapCamera`.

### Known Issues
- `MLWebRTC`: When disconnecting from a session, the camera does not shut down cleanly if the NativeSurface buffer format was used, causing the application to hang for as much as 30 seconds.

## [1.6.1]
### Known Issues
- `MLWebRTC`: When disconnecting from a session, the camera does not shut down cleanly if the NativeSurface buffer format was used, causing the application to hang for as much as 30 seconds.
- `MLAnchors` API returns the same anchor poses after a headpose reset

### Deprecations & Removals
- Removed the ability to turn off near clipping plane enforcement in `MagicLeapCamera`.

## [1.6.0]
### Features
- Added `MLNotifications` API to suppress default notifications in medical SKUs.
- Added support for handling and selecting multiple language tracks for subtitles.
- `MLWebView`: added open and close callbacks for popup tabs.

### Bugfixes
- Fixed the controller ray not following the totem right after starting Unity app.
- Fixed broken link in README ([#8](https://github.com/magicleap/MagicLeapUnitySDK/issues/8))
- Fixed issue with `MLMediaPlayerBehavior` where calling `StopMLMediaPlayer()` would make videos unplayable.
- Fixed camera errors after the device enters sleep mode.
- Fixed issue with `MLMarkerTracker` not working  after device enters sleep mode.
- Fixed issue with creating spatial anchors with an expiration value of `0` causing errors.
- Fixed issue where meshing system would render excess data when returning to the application if head tracking was reset in another application.
- Fixed issue with `MLAnchors` update fails when localized to shared space.
- Fixed issue where `EnforcedNearClipDistance` was enforced in `MagicLeapCamera` even when disabled if `FixProblemsOnStartup` was enabled

### Known Issues
- `MLWebRTC`: When disconnecting from a session, the camera does not shut down cleanly if the NativeSurface buffer format was used, causing the application to hang for as much as 30 seconds.

## [1.5.0]
### Features
- Added support for `XRHandSubsystem`
- Added slots to `MLVoiceIntents`
- Update Magic Leap XR Plugin requirement to version `7.0.0`

### Bugfixes
- Fixed enforcement of Main Camera's near clip distance to respect user setting.
- Fixed a NullReferenceException being thrown when stopping and replaying video with `MLMediaPlayer`
- Fixed invalid handle check with `MLMeshing` subsystem

### Known Issues
- `MLWebRTC`: When disconnecting from a session, the camera does not shut down cleanly if the NativeSurface buffer format was used, causing the application to hang for as much as 30 seconds.

## [1.4.0]
### Features
- Added `MLNativeBindings.MLUnitySdkGetMinApiLevel()` method which reports the minimum Magic Leap API level supported by the package.
- Added new `MLCameraBase` class to the `MLCamera` API which can be used to make camera API calls synchronously.
- Added suite of automated API marshalling tests in Test Runner.
- Added new public `OnReceivedSamples` callback to the `MLAudioInput` API.
- Added new capability to `MLSegmentedDimmer` to enable using the camera's depth buffer to automatically apply segmented dimming to 3D meshes in a scene, instead of using URP. This feature will require an upcoming update to the `com.unity.xr.magicleap` package.
- Added `WinkLeft` and `WinkRight` values to the `MLGazeRecognition.Behavior` enum.
- Added `Reset()` method to the `MLMediaPlayerBehavior` script.
- Added `Controller.State` struct to the Input subsystem get state info of the controller, such as handedness. Use `InputSubsystem.Extensions.Controller.GetState()` to query the current state.
- Added `MagicLeapCamera.recenterXROriginAtStart` boolean to determine if the app should recenter the XROrigin object so that the Main Camera is at the scene's origin on start. This is set to `true` by default but can be toggled off within the inspector.

### Bugfixes
 - Fixed bug where `MLMarkerTracker` would not shut down cleanly, causing a "PerceptionSystemNotStarted" error to fill the log output.
 - Fixed a low reproducible bug where the app would crash when consistently pausing and unpausing a scene by making the Pause/Resume methods synchronous in MLCamera

### Known Issues
- `MLWebRTC`: When disconnecting from a session, the camera does not shut down cleanly if the NativeSurface buffer format was used, causing the application to hang for as much as 30 seconds.

### Deprecations & Removals
- Several `MLSegmentedDimmer` properties which had previously been deprecated have now been removed.
- `MLSegmentedDimmer.SetEnabled()` has been marked `Obsolete` and will be removed in a future release.

## [1.3.0]
### Features
- Added `MLDepthCamera` and `MLWorldCamera` APIs

### Bugfixes
- Fixed null reference exception in Meshing subsystem which was causing applications to crash.

### Known Issues
- Quitting a scene while using `MLMediaPlayer` will cause a timeout error to be logged.

## [1.2.0]
### Features
- Added `GetData` method to `AudioInputBufferClip` that does not automatically wrap the audio data and instead sends you exactly what is in the audio buffer.
- Added `MLCamera.FlipCameraVertically` method to allow easily flipping camera frames instead of needing to invert Unity Renderer.
- Added Pause and Resume capability to `MLWebView`
- Added new result code `MLResult.Code.IllegalState`
- Added new struct `MLMarkerTracker.TrackerSettings` that introduces tracker profiles to `MLMarkerTracker` as a more modular way to configure the marker tracker hints.
- `MLMarkerTracker.MarkerData` objects now have their pose correctly rotated before being given to the developer.
- Exposed a callback `MeshingSubsystem.Extensions.MLMeshing.Config.OnMeshBlockRequests` that provides the mesh block info to allow for setting LOD overrides on a per mesh block basis. The callback is set with `MeshingSubsystem.Extensions.MLMeshing.Config.SetCustomMeshBlockRequests(...)` or `MeshingSubsystemComponent.SetCustomMeshBlockRequests(...)`
- Exposed system intents in `MLVoiceIntentsConfiguration`. This is curretly an experimental feature on the OS.
- Exposed funcs in `MLAudioOutput` to allow bypassing device's master volume. These functions will only work on 60601 compliant devices.

### Bugfixes
- `MagicLeapHandDevice` is no longer derived from `XRController`
- Fixed AccessRenderBufferTexture bug in `MLWebRTCVideoSinkBehavior` which occasionally prevented frame from rendering correctly.
- Fixed UnspecifiedFailure in `MLMarkerTracker` API caused by change in `MLMarkerTrackerSettings` structure.
- Fixed bug in `MLMediaPlayer` where `Reset()` was being incorrectly called in place of `Stop()`.
- Fixed `MLMediaPlayer` blocking main thread with `Reset()` and `Destroy()` methods. These now execute on a separate detatched thread.
- Fixed a crash in `MLMediaPlayer` when switching scenes.
- Fixed bug in MRCamera RGBA image format rendering.

### Deprecations & Removals
- `MeshingSubsystemComponent.LevelOfDetail`, `MeshingSubsystemComponent.LevelOfDetailToDensity()`, and `MeshingSubsystemComponent.LevelOfDetailToDensity()` have been marked `Obsolete` and will be removed in a future release in favor of `MeshingSubsystem.Extensions.MLMeshing.LevelOfDetail`, `MeshingSubsystemComponent.FromDensityToLevelOfDetail()`, and `MeshingSubsystemComponent.FromLevelOfDetailToDensity()`

### Known Issues / Limitations
- Hand Center and Interaction Point are not valid in the Gesture Classification API.

## [1.1.0]

### Features
- Unity Magic Leap XR Plugin dependency updated to 7.0.0-pre.1
- Using Segmented Dimmer now requires an explicit `MLSegmentedDimmer.Activate()` call in order to request the Graphics API provide AlphaBlend frames.
- Per-frame intrinsics via for Mixed Reality Configured MLWebRTC/MLCamera are now available.
- Controller 6dof state filtering is now available.
- Unity XRI haptics now supported.
- `SettingsIntentsLauncher` can be used to open certain Android Settings views directly from Unity.
- Added new `MLMediaPlayer` `OnVideoRendererInitialized` callback for when video renderer is fully initialized

### Bugfixes
- Fixed bug in WebRTC where toggling local video off and then back on during a connected session did not work.
- Fixed bug in WebRTC which prevented a session from persisting between scene changes.
- Fixed bug in WebRTC where Mixed Reality capture showed a transparent vertical bar on the side of the image.
- Fixed `MLSegmentedDimmer` behavior where disabling dimmers using the `IsEnabled` property did not work as expected. The property has been marked `Obsolete` and replaced with a `SetEnabled()` method which turns on and off the MeshRenderers for objects on your Segmented Dimmer layer.
- Fixed issue with WebView's first tab loading before service is connected.
- Fixed Meshing Subsystem using invalid handles if device headpose gets reset
- Corrected some `MLCamera` event delegates being incorrectly defined.
- Fixed issue in WebView where adding too many tabs caused them to begin rendering outside of UI bounds.

### Deprecations & Removals
- Numerous MLSegmentedDimmer API properties have been marked `Obsolete` as they are non-functional and will be removed in a future release.
- `MLMarkerTracker.Settings`, `MLMarkerTracker.SetSettingsAsync(Settings settings)`, and  `MLMarkerTracker.StartScanningAsync(Settings settings)` have been marked `Obsolete` and will be removed in a future release.

## [1.0.0]

### Features
- Added controller pose derivative values to Unity Input system.
- Added ability to save Magic Leap diagnostic logs from within Unity.
- Updated license agreements and copyright headers.
- Added Anchors space origin transform.
- Added MLAudioInput support for streaming audio clips.
- Updated WebRTC API struct format and improved memory management.
- Added new `Auto Resize Native Renderer` option to `MLWebRTCVideoSinkBehavior` which if enabled will allow the Native Renderer to automatically be adjusted in response to changes in WebRTC stream size.
- Added ability for developers to override `VkSamplerYcbcrConversionCreateInfo` struct.

### Bugfixes
- Fixed resource leak in WebViewTabBehavior leaving web view tabs open after changing scenes.
- Fixed cleanup of YcbcrRenderer when MediaPlayer is reset.
- Fixed MLAudioInput capture delay.
- Fixed use of TryGetBestFitStreamCapability.
- Fixed multiple warnings due to unused variables.
- Fixed errors with MLCamera asynchronous APIs by adding locking mechanisms.
- Fixed performance issues with executing LabDriver to get relative paths for Magic Leap App Sim.
- Fixed cleanup of callback handlers on video player teardown.
- Fixed RenderTexture read-write settings and YcbcrRenderer output color space determination.
- Fixed MLWebView link click precision issues.

### Deprecations & Removals
- Removed WorldScale support. This was causing issues with transforms coming from the Magic Leap SDK and was not uniformally applied. Recommended to use custom scaling of objects instead of camera parent scale.
- Removed APIs for deprecated features (Hand Meshing, Raycast, Image Tracking)
- Removed deprecated Planes setting Max Hole Size.

### Known Issues / Limitations
- Eye blinking state is not reported either by the eye tracking API or the gaze recognition API (awaiting platform support).
- WebRTC LocalAppDefinedAudioSourceBehavior is restricted to 1 audio channel.
- To use Geometry Shaders, Force Multipass must be set in Project Settings -> XR Plug-in Management -> Magic Leap Settings -> Force Multipass. Otherwise geometry shader passes cause vulkan exception in Unity player.
- Keypoint mask values in ML App Sim are temporarily ignored and overridden to true.
- XR Framework Meshing subsystem crashes when attempting to load mesh blocks for rendering.
- Detecting simultaneous controller input buttons does not work in Unity Input System 1.2.
- Marker tracker transforms are upside down requiring users to rotate them by 180 degrees about the forward vector.
- Camera capture can freeze app after multiple captures.
- MLCamera.CaptureVideoStop fails with UnspecifiedFailure when called by WebRTC CameraVideoSource using NativeBuffers. When using YUV CaptureVideoStop returns successfully.
- Some configurations of camera capture can produce distorted images.
- WebRTC video sink rendering fails when non-white material is assigned.
- MLGestureClassification's GestureTransformRotation and GestureInteractionRotation are not implemented yet and data will not be guaranteed accurate. Currently only the Positions of the Hand Transform and Interaction Point will be recommended to use.
- MLWebView first tab creation causes framerate drop.
- If HandTracking is enabled, the Controller position/rotation actions fail to work properly when binding with the generic XRController and Right XRController input devices. The work around is to have your actions bind to the MagicLeapController input device instead. The MagicLeapInputs input asset already does this with it's action fallbacks.
- MLAudio is not fully supported in the 2022.2.0b8 version of the Unity Engine, make sure you don't check the "MLAudio" check box in Magic Leap XR settings (to utilize the Java AudioTrack fallback). Also use the following audio settings: Sample rate to 48000, buffer size to Good Latency.
- When changing audio settings Unity crashes often or starts making noises.
- Unity applications currently experience aproximately 19 MB/hr - 190 MB/hr memeory leak (varies by app). This appears to be true for all Android based Unity applications.
- WebRTC session can not be reconnected to if user navigates to another scene during an active session.

## [0.53.3]

### Features
- Added support for camera auto focus API.
- Added support for MLHaptics API.
- Added async support to MLCamera API to allow developers to avoid blocking on Camera operations.
- Added MLCameraIntrinsics OnPreviewBufferAvailable callback.
- Added better support for OnApplicationPause in MLWebRTCCameraVideoSource to manage camera resouces during pause and resume.
- Added Trigger Hold action to MagicLeap input actions.
- Updated MagicLeapXrProvider with normal permission checks.
- Added boot settings for OpenXR.
- Added support for high precision marker tracking.
- Added MediaPlayer.VideoRenderer.OnFrameRendered callback to media player renderer.
- Added Magic Leap 2 interaction profile for OpenXR.
- Added TargetSpaceOrigin to MLAnchor.LocalizationInfo.
- Added support for indefinite MLAnchor duration registration.
- Updated integration branding with the Magic Leap Hub (formerly The Lab 2.0) and Magic Leap App Simulator (formerly Zero Iteration).

### Bugfixes
- Fixed OnSourceEnabled when not using native buffers for WebRTC.
- Fixed hand tracking keypoint detection under ML App Sim.
- Fixed native platform error logging.
- Fixed event delegate initialization in MLCamera.
- Fixed excessive Audio Playback allocations.
- Fixed developer build crashes caused by WorldScale computations.
- Fixed memory leak when pausing/resuming unity applications (requires Unity XR Package update).
- Fixed collision mesh generation on mesh blocks generated from MeshingSubsystemComponent + Mesh prefab.
- Fixed Voice Intents configuration asset creation (fixed in 2022.2.0b4 of Unity Engine).
- Fixed crash caused by MLDevice instance race conndition.
- Refactored MLWebView mouse input functions to simplify parameters.
- Fixed MLWebView mouse drag support.
- Fixed MLWebView component null reference checks.
- Fixed MLAnchor duration checks and updated documentation.
- Fixed controller Menu button and touchpad actions.
- Refactored controller action layout to remove touch point 2 and cleanup supported actions.
- Fixed YcbcrRenderer.Cleanup() not fully cleaning up resources.

### Deprecations & Removals
- Removed MLAutoAPISingleton inheritance from MLAudioPlayback. Uses normal singleton pattern. Callers still need to drive its lifecycle functions.
- Removed automatic disabling of Strip Engine Code, this has been fixed in the 2022.2.0b4 Unity Engine.
- Removed permissions for HEAD_POSE and CONTROLLER_POSE, these are no longer required.
- Removed remaining references to Lumin platform. Magic Leap 2 is a full AOSP based platform.

### Known Issues / Limitations
- Image tracking, World Raycast & Hand Meshing support has been temporarily disabled in this release. None of these are currently supported on the device. Once re-enabled, developers can use some of these in ML App Sim.
- Eye blinking state is not reported either by the eye tracking API or the gaze recognition API (awaiting platform support).
- WebRTC LocalAppDefinedAudioSourceBehavior is restricted to 1 audio channel.
- To use Geometry Shaders, Force Multipass must be set in Project Settings -> XR Plug-in Management -> Magic Leap Settings -> Force Multipass. Otherwise geometry shader passes cause vulkan exception in Unity player.
- Keypoint mask values in ML App Sim are temporarily ignored and overridden to true.
- XR Framework Meshing subsystem crashes when attempting to load mesh blocks for rendering.
- Detecting simultaneous controller input buttons does not work in Unity Input System 1.2.
- Marker tracker transforms are upside down requiring users to rotate them by 180 degrees about the forward vector.
- Camera capture can freeze app after multiple captures.
- MLCamera.CaptureVideoStop fails with UnspecifiedFailure when called by WebRTC CameraVideoSource using NativeBuffers. When using YUV CaptureVideoStop returns successfully.
- Some configurations of camera capture can produce distorted images.
- WebRTC video sink rendering fails when non-white material is assigned.
- MLGestureClassification's GestureTransformRotation and GestureInteractionRotation are not implemented yet and data will not be guaranteed accurate. Currently only the Positions of the Hand Transform and Interaction Point will be recommended to use.
- MLWebView first tab creation causes framerate drop.
- MLWebView has challenges clicking on web links on page due to noisy controller position cancelling click operations (treats it as a drag operation).
- If HandTracking is enabled, the Controller position/rotation actions fail to work properly when binding with the generic XRController and Right XRController input devices. The work around is to have your actions bind to the MagicLeapController input device instead. The MagicLeapInputs input asset already does this with it's action fallbacks.
- MLAudio is not fully supported in the 2022.2.0b5 version of the Unity Engine, make sure you don't check the "MLAudio" check box in Magic Leap XR settings (to utilize the Java AudioTrack fallback). Also use the following audio settings: Sample rate to 48000, buffer size to Good Latency.
- When changing audio settings Unity crashes often or starts making noises.
- Unity applications currently experience aproximately 190 MB/hr memeory leak.
- Black texture can be seen during the first few frames of video playback with MLMediaPlayerBehavior.
- Trying to get the pose of the space origin after calling MLSpatialAnchorGetLocalizationInfo fails the first time.
- Unity XRI haptics currently not supported.

## [0.53.2]

### Features
- Reorganized package structure to more align with Unity conventions.
- Added support for Eye Fit and Calibration API.
- Renamed LuminUnity asmdef to MagicLeap.SDK.
- Moved WorldScale to MagicLeapCamera.cs and now ensures it is updated if the main camera parent transform changes.
- Updated to support 2022.2 a19 editor and render pipeline 14.0.3.
- Added WebRTCSendNativeFrameError result code.
- Post build step now checks for XR Loader toggle to determine if build is for ML2.
- Changed input device name from MagicLeapLightwear to MagicLeapHeadset.
- Updated XRRig prefab to use action-based controllers.
- Refactored MLCamera to support per-frame camera intrinsics parameters as callbacks.
- Added support for getting audio data and position data for MLAudioInput.
- Added support to MLAudioInput to create non-streamed audio clip.
- Added AudioInput.Clip class to record an AudioClip from the Microphone.
- Added EyeCalibration and HeadsetFit APIs.
- Added API for client to call MLCameraDeinit() as needed.
- Updated MagicLeapInput action map to rename Touchpad1 to Touchpad.

### Bugfixes
- No longer receive warnings about camera transform if its not set to identity now that XRRig is recommended.
- MLDevice no longer derives from TrackedPoseDriver as it was unnecessary.
- MLPermissions handles logging errors internally and no longer errors on pending requests.
- Fixed cleanup errors of MLDevice under Zero Iteration.
- SDK Loader now uses compile time constructed library names.
- Fixed meshing collider generation caused by incorrect current mesh type value override in session subsystem.
- Fixed platform wrappings for MLAudioInputClip.
- Fixed release of render target when YcbcrRenderer cleanup is complete.
- Marked IsBuffering, IsSeeking & Duration as c# properties to make them read-only in MLMediaPlayer class.
- Set near clip plane on Main Camera prefab to remove warning about clip plane being too close.
- Fixed inner planes tracker id and boundary generation.
- Fixed rectangular boundary generation.
- Added epsilon check to convex hull generation point on left hand side check to avoid infinite loops on straight edges.
- Fixed permission prompts to no longer hang UI when using Zero Iteration Frontend.
- Fixed SDK loader library path sent to xr package.
- Fixed MLCamera VirtualOnly camera mode orientation in WebRTC.
- Fixed browse button for picking ML SDK.
- Renamed The Lab launch method and comments.
- Fixed The Lab launch root option and arguments.
- Added additional comment to MLCamera.CreateAndConnect method.
- Added more information to MLCamera.GetStreamCapabilities function.
- Fixed performance hit in MLTime by removing inheritance of MLAPIBase.

### Deprecations & Removals
- Removed deprecated MLSceneOptimizerBehavior.cs as this has been merged into MagicLeapCamera.cs as of 0.53.0.
- Removed deprecated MLPermissionsRequesterBehavior.cs as this is no longer the recommended permissions requesting method.
- Removed AndroidTargetDevices.RelishDevicesOnly, using MagicLeapLoader toggle instead.
- Remove MLTime inheritance to MLAPIBase and fix audio function call.
- Moved WorldScale to MagicLeapCamera.cs from MLDevice and removed unnecessary references.

### Known Issues / Limitations
- Image tracking, Cloud Anchors & World Raycast & Hand Meshing support has been temporarily disabled in this release. None of these are currently supported on the device. Once re-enabled, developers can use some of these in Zero Iteration.
- Media player currently only supports playback for web URLs. Support for files packaged in the apk will be added later.
- Eye blinking state is not reported either by the eye tracking API or the gaze recognition API (awaiting platform support).
- WebRTC LocalAppDefinedAudioSourceBehavior is restricted to 1 audio channel.
- Projects built with MSA plugin fail to build with linker errors if Strip Engine Code setting is enabled.
- XR Framework Meshing subsystem crashes when attempting to load mesh blocks for rendering.
- Shaders using geometry shader passes cause vulkan exception in Unity player (meshing wireframe shader specifically).
- Frame rate can deteriorate after pausing and resuming an app multiple times.
- Detecting simultaneous controller input buttons does not work in Unity Input System 1.2.
- Marker tracker transforms are upside down requiring users to rotate them by 180 degrees about the forward vector.
- Camera capture can freeze app after multiple captures.
- MLCamera.CaptureVideoStop fails with UnspecifiedFailure when called by WebRTC CameraVideoSource.
- Collision mesh not properly generated by MeshingSubsystemComponent.
- YcbcrRenderer.Cleanup() not fully cleaning up resources.
- Create Voice Configuration asset menu item creates the wrong asset type.
- Spatial anchors report incorrect location for a single frame.
- Some configurations of camera capture can produce distorted images.
- WebRTC video sync rendering fails when non-white material is assigned.
- MLGestureClassification's GestureTransformRotation and GestureInteractionRotation are not implemented yet and data will not be guaranteed accurate. Currently only the Positions of the Hand Transform and Interaction Point will be recommended to use.

## [0.53.1]

### Bugfixes
- `MLAudioPlayback` class has been restricted to submit audio buffers only in ZI mode, not when running the app directly on the device.

## [0.53.0]

### Features
- Exposed `Headlock` API, which locks left and right eye projection matrix to origin and disabled timewarping.
- Refactored `MLWebView` API to be more Object Oriented.
- Added support for canceling Certificate Error operation in `MLWebView`.
- Added support for `MLMeshingDestroyClient` parameter change.
- Relocated `MagicLeapCamera.cs` from com.unity.xr.magicleap package to the com.magicleap.unitysdk package as it has been deprecated and marked for removal in the xr package but is still required.
- Added callbacks to `MLPermissoins` API to make it match closer to Android permissions API
- Added universal library builds for Mac OSX to support x86_64 and arm64 architectures (M1 support).
- Isolated `MLAudioOutput` plugin code into separate class to be used by other systems.
- Added callback that returns list of markers for `MLMarkerTracking` API.
- Added com.magicleap.permissions.SPATIAL_MAPPING dangerous permission for `MLMeshing` and `MLPlanes` APIs.
- Added validation checks to `MLVoiceIntents` API.
- Added `MLGestureClassification` API to Unity Input system.
- Added a `OnFrameResolutionChanged` delegate to the `MLWebRTC.VideoSink` class which triggers whenever the video frame resolution changes. The `MLWebRTCVideoSinkBehavior` uses this delegate to update the `RenderTarget` the video gets rendered on.

### Bugfixes
- Forced adding com.magicleap.permissions.HEAD_POSE to AndroidManifest.xml if not present.
- Fixed WebRTC inverted video rendering.
- Fixed WebRTC Local video and preview inverted rendering.
- Fixed `VideoSyncRenderer` inverted video rendering.
- Simplified `MeshingSubsystemComponent` code to no longer track default value struct.
- Fixed polling rate of `MeshingSubsystem` to accurately reflect configured rate value.
- Fixed initializaton of Magic Leap API version dropdown in manifest settings.
- Forced setting min Android API version to 29.
- Fixed some DLL Not Found exceptions when running Zero Iteration.
- Fixed checking for com.magicleap.permissions.MARKER_TRACKING when using marker tracking API.
- Unload all loaded libraries in library loader when changing lib paths to improve compatibility with Zero Iteration.
- Improved Zero Iteration "Not Running" error output.
- Fixed additional calls to `YcbcrRenderer.SetRenderBuffer()`.
- Fixed trying to run Zero Iteration when Magic Leap Zero Iteration provider is unchecked in XR Management settings.
- Fixed crash on camera capture preview under Zero Iteration.
- Fixed crash on requesting mesh vertex confidence.
- Fixed texture memory leak when pausing and resuming apps.

### Deprecations & Removals
- Merged `MLSceneOptimizerBehavior.cs` into `MagicLeapCamera.cs`.
- Removed `StabilizationMode` and `StabilizationDistance` options from `MagicLeapCamera.cs`
- Removed com.magicleap.permissions.WORLD_RECOGNITION normal permission.

### Known Issues / Limitations
- Audio playback does not play at a constant speed.
- Marker tracker transforms are upside down requiring users to rotate them by 180 degrees about the forward vector.
- Camera capture can freeze app after multiple captures.
- `MLCamera.CaptureVideoStop` fails with UnspecifiedFailure when called by WebRTC `CameraVideoSource`.
- Collision mesh not properly generated by `MeshingSubsystemComponent`.
- `YcbcrRenderer.Cleanup()` not fully cleaning up resources.
- Create Voice Configuration asset menu item creates the wrong asset type.
- Spatial anchors report incorrect location for a single frame.
- Some configurations of camera capture can produce distored images.
- WebRTC video sync rendering fails when non-white material is assigned.
- MLGestureClassification's GestureTransformRotation and GestureInteractionRotation are not implemented yet and data will not be guaranteed accurate. Currently only the Positions of the Hand Transform and Interaction Point are recommended to use.



## [0.52.2]

### Features
- Update to Unity 2022.2.
- Update XR Plugin dependency to `v7.0.0-preview.1`.
- Exposed `MLSegmentedDimmer` API, which provides basic functions to manipulate the Segmented Dimmer URP feature in scenes.
- Exposed `MLGlobalDimmer` API, which provides a function to set the global dimmer intensity.
- Added support to stream any Unity AudioSource via WebRTC.
- Hand tracking API has been updated to match what is offered by the platform i.e. static gesture detection has been removed and only hand keypoints are offered via this API now. To enable hand tracking, make sure to add the "com.magicleap.permission.HAND_TRACKING" permission to your project's AndroidManifest.xml and call `InputSubsystem.Extensions.MLHandTracking.StartTracking()` before using the feature.
- Eye tracking is protected under a dangerous level permission. To enable eye tracking, make sure to add the "com.magicleap.permission.EYE_TRACKING" permission to your project's AndroidManifest.xml, request the said permission at runtime, and then call `InputSubsystem.Extensions.MLEyes.StartTracking()` once the permission has been granted before using the feature. The eye tracking example in the MagicLeapExamples project has been updated to reflect this required workflow.
- Gaze recognition is protected under a dangerous level permission. To enable gaze recognition, make sure to add the "com.magicleap.permission.EYE_TRACKING" permission to your project's AndroidManifest.xml, and request the said permission at runtime. Make sure the permission has been granted before using the feature.
- `MLCamera` class now offers utility methods like `GetImageStreamCapabilitiesForCamera()` and `GetBestFitStreamCapabilityFromCollection()`.
- `MLAudioInput` and `MLAudioOutput` classes are now `MLAutoAPISingletons`. Developers no longer need to explicitely call `Start()` and `Stop()` on these APIs.
- Added multi-tab support in WebView.
- Added support for meshing API.
- Added permission checks within some API calls.
- Added permission checks to input subsystem.

### Bugfixes
- Fixed workflow involving permissions declaration in AndroidManifest.xml. Permissions are no longer added or removed at build time, and are fully in the developer's control.
- Renamed some `MLResults` to match what they're now called for ML2.
- WebRTC: Using `MLCamera` as `AppDefinedVideoSource` now allows for displaying local video stream using Preview capture mode.
- WebRTC: Fixed issue where audio playback was sometimes accelerated.
- Updated XR Rig prefab to use new XR Origin script.
- Fixed padding bug with marshalling of native struct in `MLAnchors` API.
- `MLAudioInput` and `MLAudioOutput` are now `MLAutoAPISingletons`.
- Fixed issue in media player where a grey bar renderered at the bottom of the video screen.

### Deprecations & Removals
- "Relish" platform is no longer supported. Developers now deploy Android apps to ML2 by setting `Target Devices` to "Relish Devices Only" under Player Settings.
- Deprecated `MagicLeapManifestSettings.asset` and simplified the manifest interface in Project Settings. Developers now directly add necessary permissions to `Assets/Plugins/AndroidManifest.xml` like in normal Android development.
- Removed deprecated ML1 `MLResult` names.
- Replaced all remaining instances of "Privilege" with "Permission".

### Known Issues / Limitations
- WebRTC: `LocalAppDefinedAudioSourceBehavior` is restricted to 1 audio channel (KARROT-359).

## [0.52.1]

### Features
- Exposed WebView API. Refer to the WebView sample scene in the MagicLeapUnityExamples project for usage.
- Exposed `MLTime` API, which provides a standardized way for all ML APIs to provide a timestamp. Camera, CVCamera, eye tracking, gaze recognition, meshing and hand tracking have been updated to provide an MLTime object instead of the usual uint or ulong timestamp values of varying units. The `MLTime` API provides helper methods get a `timespec` system time from an `MLTime` object and vice versa.
- Permission enforcement has been enabled in the OS paired with this SDK release and thus `MLPermissions` has been updated to actually request permissions instead of simply returning "true".
- Audio playback is now supported in ZI.
- MLMeshing is now functional under Zero Iteration but is still not enabled on platform.

### Bugfixes
- Fixed `MLCamera` rendering for YUV buffers.
- Fixed crash when performing consecutive MLCamera captures with different resolutions.
- Controller.fbx heirarchy has been flattened and extraneous transforms have been remove from the prefab.
- Fixed issue where `MLMarkerTrackerDecodedBinaryData` restricted data size to 100.
- Fixed issue in MLWebRTC where video source could not be changed without restarting the app.
- Fixed DllNotFoundException being thrown for MLWebRTC APIs when running in ZI mode. While WebRTC is still not supported in ZI, these API calls now fail more gracefully with an `MLResult.Code.NotImplemented` error.
- Fixed media player crashing when calling `MLMediaPlayerBehavior.StopMLMediaPlayer()`.

### Deprecations & Removals
- Deprecated `MLCamera.MRBlendType` values of `Alpha` and `Hybrid` have been removed. `Additive` is the only supported blend type now.
- Hand tracking has been moved from the `GestureSubsystem` to the `InputSubsystem` i.e. all `GestureSubsystem.Extensions.MLHandTracking` references now need to be updated to `InputSubsystem.Extensions.MLHandTracking`.
- Eye pupil size has been removed from the MLEyes API.

### Known Issues / Limitations
- WebRTC: local video rendering does not work when using native buffers
- WebRTC: using app-defined video source with CPU buffers and YUV frame format sends black-and-white frames (awaiting platform support)
- Eye blinking state is not reported either by the eye tracking API or the gaze recognition API (awaiting platform support).
- Labdriver is run even when MagicLeapLoader is not selected in XR Plugin Management.
- Launching / terminating ZI after having played 1 editor session already, is not picked up by Unity and it continues to assume the original state of ZI for subsequent plays.
- Media player render with a small grey-block at the bottom.
- VoiceIntents: if a system popup/notification happens within an application while Voice Intents are processing, it will stop working. Stop and restart the API to resolve this.
- Apps sometimes don't resume from paused state and switching between home menu and unity app repeatedly.
- WebView platform callbacks are known to trigger twice, WebViewExample.cs has TODO: workaround to avoid registering keyboard callbacks twice as a result.
- WebView platform functions `MLWebViewGetScrollSize` and `MLWebViewGetScrollOffset` always return values of (2, 2) regardless of actual webpage size or scroll location
- Under Relish builds `EventSystem.current.IsPointerOverGameObject` does not properly report if the controller pointer is pointing at a UI (This causes Web view screen to react to controller trigger pulls when trying to interact with the virtual keyboard if they are overlapping). This will be fixed once we move under ML2 support under the Unity Android platform umbrella.

## [0.52.0]

### Features
- Exposed Anchors API. Refer to the Anchors sample scene in the MagicLeapUnityExamples project for useage.
- Exposed Voice Intents API. Refer to the VoiceIntents sample scene in the MagicLeapUnityExamples project for useage.
- Added support for AprilTags under the MLMarkerTracker API.
- Re-enabled WebRTC support in Unity.
- Hand tracking has been updated to only start when an app calls `GestureSubsystem.Extensions.MLHandTracking.SetConfiguration()`. This is done to save on resources and only enable tracking if the app is using the feature.
- Timestamp data is now provided as part of the gaze recognition state.
- Added MLCamera preview rendering support.
- Added a utility UI under Project Settings > Magic Leap > Manifest Settings to help include required permissions in the app's AndroidManifest.xml file.
- All native libs shipped with the SDK package have been marked to work on the Android x86_64 target and all platform code wrapped under `#if UNITY_MAGICLEAP || UNITY_ANDROID` in preparation for bringing Relish under the Unity Android platform umbrella. Developers building their own native libs for Relish or using the `UNITY_MAGICLEAP` preprocessor flag are advised to update their library setup and code accordingly.

### Bugfixes
- Fixed issue with Manifest Settings throwing error on first access
- Fixed MediaPlayer Stop functionality
- Fixed MediaPlayer local playback
- Fixed C-API enum mismatch in GazeRecognition API
- Updated WebRTC API to support deprecation of distinct MLMRCamera class
- Updated `MagicLeapController/isTracked` input action binding to be a passthrough instead of a button for better detection of connection status at scene startup
- Fixed using the PlayerInput component for Magic Leap Controller events.

### Deprecations & Removals
- Eye tracking calibration status has been removed.
- `WinkLeft`, `WinkRight`, `Push` & `Pull` behaviors have been removed from the MLGazeRecognition API.
- MLBluetooth APIs have been removed. Apps are expected to use Android Java APIs for bluetooth support.
- Moved the prefab `AR Default Plane` and material `DebugPlane` from the SDK into the example project.

### Known Issues / Limitations
- WebRTC: local video rendering does not work when using native buffers
- WebRTC: using app-defined video source with CPU buffers and YUV frame format sends black-and-white frames (awaiting platform support)
- Eye blinking state is not reported either by the eye tracking API or the gaze recognition API (awaiting platform support).
- Eye pupil size is always reported as 4.0 (awaiting platform support).
- Labdriver is run even when MagicLeapLoader is not selected in XR Plugin Management.
- Launching / terminating ZI after having played 1 editor session already, is not picked up by Unity and it continues to assume the original state of ZI for subsequent plays.
- Media player render with a small grey-block at the bottom.
- MLMarkerTrackerDecodedBinaryData restricts data size to 100.
- VoiceIntents: if a system popup/notification happens within an application while Voice Intents are processing, it will stop working. Stop and restart the API to resolve this.

## [0.51.0]

### Features
- Exposed gaze recognition API.
- MediaPlayer support has been re-enabled in the SDK package. Only basic playback has been tested yet, support for DRM, subtitles, stereo videos etc needs to be verified.
- Added a new function to `MLMedia.Player.Track.DRM` class to generate a signature using the provided algorithm and message data.
- Added support for TTML subtitles to `MLMedia.Player`.
- Controller touchpad gestures have been added to the MagicLeapInput action map and also exposed via the XR Gesture Subsystem extensions.
- `MLMediaFormat` API integration has been updated to no longer be a singleton but instead be a regular C# class with different factory methods like `CreateVideo()`, `CreateAudio()`, `CreateSubtitle()` and `CreateEmpty()`. The format key specific functions have been removed in favor of general key-value getters and setter with overloads for each possible data type. All possible media format keys have been exposed in the `MLMediaFormatKey` class.
- ArucoTracker & BarcodeScanner APIs have been combined into a single MLMarkerTracker API.
- Numerous rendering performance improvements.
- Hand tracking example has been updated to use the gesture subsystem.
- All examples that used MLInput for controller interaction have been updated to use the new Input System.
- Eye tracking example has been added which showcases how to use this feature via the new InputSystem, the XR Input Subsystem and the Magic Leap Extensions for the XR Input Subsystem.
- Zero Iteration now works without needing to import the SDK libraries into the Unity project.
- Exposed APIs for MLCamera (includes MRCamera), MLCameraMetadata and MLMediaRecorder. CameraCapture & MediaRecorder scenes have been added in the examples project to showcase the use the new APIs.
- Exposed media events like play, pause, stop, next / prev track in the MLAudioOutput API.
- Namespace for all scripts in the examples project has been changed to MagicLeap.Examples.

### Bugfixes
- Fixed menu button binding in the MagicLeapInput action map.
- Fixed eye calibration status not being reported correctly.
- Fixed API reference docs.
- Fixed broken references to virtual keyboard in the examples project which prevented Build & Run.
- Fixed exception thrown when attempting to use any head tracking extension functions.

### Deprecations & Removals
- MLHandTrackingm MLHeadTracking, MLMeshing, MLPlanes, MLInput APIs have been removed in favor of their respective XR subsystems. MagicLeap specific functionality has been exposed via the extension classes in the `UnityEngine.XR.MagicLeap.Extensions` namespace.
- MCA / MLMA / MLMCA support has been removed from MLInput along with removal of touchpad gestures which were specific to it (LongHold, Scroll, Pinch)
MLAppIdentifier, MLDispatch, MLIdentity, MLIMU, MLLocale, MLLocation, MLNetworking, MLSecureStorage, MLTokenAgent APIs have been removed in favor of their existing Android counterparts.
- MLAppConnect, MLContacts, MLConnections, MLMediaPlayer Sharing, MLController LED, MLMusicService APIs have been removed and will not be supported on the ML2.

### Known Issues / Limitations
- Image tracking, Anchors (PCFs) & World Raycast support has been temporarily disabled in this release. None of these are currently supported on the device. Once re-enabled, developers can use these on ZI.
- Media player currently only supports playback for web URLs. Support for files packaged in the apk will be added later.
- Eye blinking state is not reported either by the eye tracking API or the gaze recognition API (awaiting platform support).
- Eye pupil size is always reported as 4.0 (awaiting platform support).
- Labdriver is run even when MagicLeapLoader is not selected in XR Plugin Management.
- Launching / terminating ZI after having played 1 editor session already, is not picked up by Unity and it continues to assume the original state of ZI for subsequent plays.
- You must have Assets/Plugins/Relish/MagicLeapManifestSettings.asset present or else APKs will fail to build. Workaround: Open Project Settings and select MagicLeap → Manifest Settings, this will auto-generate the file.

## [0.50.0]
Please see the "SDK Refactor" note in the README.

### Features
- The following features are supported in this package, in addition to the ones supported in the engine -
  - Headpose
  - Controller Input
  - Eye Tracking
  - Hand Tracking
  - Media Player

### Deprecations & Removals
- The following APIs will not be supported on Relish & have been removed
  - MLAppConnect
  - MLController LED
  - MLContacts
  - MLConnections
  - MLMediaPlayer Sharing (only the sharing APIs have been removed, general media player will still be supported)
- The following APIs will be supported via the Android SDK instead of ML-specific APIs & have thus been removed
  - MLAppIdentifier
  - MLDispatch
  - MLIdentity
  - MLLifecycle
  - MLLocale
  - MLSecureStorage
  - MLPrivileges
  - MLIMU
  - MLBattery
  - MLInput Tablet

### Known Issues
- Media player only supports remote URLs right now, not videos packaged in the apk itself.

## [0.26.0]
### New Features
- Exposed APIs to optionally set/get the Ids for WebRTC media tracks. Ids can be specified when creating the tracks and the `MLWebRTC.MediaStream.Track.Id` property is already set upon receiving a track from the remote peer.
- Exposed callbacks in `MLWebRTC.AudioSink` to provide the app with the incoming audio buffers. Pass in the appropriate `BufferNotifyMode` to `MLWebRTC.AudioSink.Create()` to set whether the app will receive the audio buffers or not and whether the underlying WebRTC platform will play the audio or leave it to the app to do so.
- Exposed functions in `MLWebRTC.AudioSink` to set various soundfield parameters (position, orientation, direct send levels, room send levels, distance & radiation properties and sound volume). These properties should only be set after `MLWebRTC.AudioSink.CurrentServiceStatus` is `ServiceStatus.Started` or `MLWebRTC.AudioSink.OnAudioServiceStatusChanged` delegate is fired with `ServiceStatus.Started`.

### Updates
- Added extensions for `MLPlanes.QueryFlags` enum. You can now check which flags are set by calling convenience functions like `IsCeiling()`, `IsFloor()` etc to check which planes the flag represents.
- Image capture now uses memory from a circular buffer to avoid over-allocations in case of multiple capture requests.
- Added `OnTrackAddedMultipleStreams` & `OnTrackRemovedMultipleStreams` delegates to the `MLWebRTC.PeerConnection` class. These delegates are similar to the old `OnTrackAdded` & `OnTrackRemoved` delegates but report a full list of streams that added/removed track belongs to.
- Upgraded Magic Leap XR Plugin support to 6.2.2.
- Clarified the usage of `MLEyes.Timestamp` property that it won't automatically initialize the eye tracking API and returns 0 in that case.

### Bug Fixes
- Fixed a crash in MLWebRTC when the remote peer does not provide a stream ID for its media tracks. Use a default stream ID of "unknown_remote" in such cases.
- Fixed the string returned from the scanned QR-code. The string had a null-terminator in the end.
- Fixed MLWebRTCVideoSinkBehavior to scale the game object according to dyamic changes in video resolution.

### Deprecations & Removals
- `OnTrackAdded` & `OnTrackRemoved` delegates in the `MLWebRTC.PeerConnection` class have been deprecated in favor of `OnTrackAddedMultipleStreams` & `OnTrackRemovedMultipleStreams` delegates.

## [0.25.0]
### New Features
- Exposed a subset of WebRTC APIs to allow apps to send & receive audio, video and data streams between multiple peers over the network. This also includes helper implementations to stream video feed from the color camera or Mixed Reality Camera and audio feed from the device microphone.
- New Barcode Scanner API allows for QR code detection.
- New IMU API for reading IMU data.
- New ID API for determining device ID.
- Eye tracking API (MLEyes) now exposes pupil size.
- New Found Objects example.
- New Aruco tracking example.
- New MRCamera API allows capture of the composited video the color camera and app content.
- New Virtual Keyboard example.

### Bug Fixes
- Fixed a crash in UserInterface.cs on app termination and on ending play mode in the editor.
- Fixed a bug in MLCamera where its APIs would fail to work after recovering from a system-level eviction. Priority processes like Device Capture and Device Stream evict app's hold on the camera and return it back once those processes are done. The fix now auto-resumes the camera state that was before the eviction.
- Fixed a bug in MLPlanes where the min plane area query setting was not respected.
- Switched PoseSource to 'CenterEye' in the MainCamera prefab to get rid of the editor warning.

### Updates
- Input, Eyes, Privileges, Hand Tracking, Camera, CV Camera, Raycast, Planes, and Image Tracking APIs are now AutoAPIs (see this page for more info: https://developer.magicleap.com/en-us/learn/guides/auto-API-changes).
- CVCamera APIs have been moved from MLCamera into its own MLCVCamera class, removing the requirement from MLCamera users to specify the ComputerVision privilege if they don't intend on using that feature.
- Exposed an event named OnRawVideoFrameAvailableYUV_NativeCallbackThread in the MLCamera class which provides the frame data via pointers to unmanaged memory. This event is more efficient to use in systems where the camera frames need to be sent from Unity to an unmanaged C/C++ library like WebRTC, OpenCV or any kind of media encoder.
- Exposed asynchronous variants of expensive MLCamera functions. Each of these is accompanied with an event which is invoked when the asynchronous operation is completed.
- The VideoCapture example has been split into VideoCapture (showcasing video capture to file) and RawVideoCapture (showcasing video capture to YUV frames delivered to the app).

### Deprecations & Removals
- All APIs marked deprecated in the 0.24.2 release have been removed.
- `Start()` and `Stop()` methods of Input, Eyes, Privileges, Hand Tracking, Camera, CV Camera, Raycast, Planes, and Image Tracking APIs have been deprecated (see this page for more info: https://developer.magicleap.com/en-us/learn/guides/auto-API-changes).
- `MLCamera.IntrinsicCalibrationParameters`, `MLCamera.GetIntrinsicCalibrationParameters()` and `MLCamera.GetFramePose()` have been deprecated in favor of their `MLCVCamera` counterparts.

### Known Issues
- With Unity 2020.2 on Windows, sometimes the XR Plug-in Management settings will be grayed out. This can be fixed by closing the editor, using `regedit` to delete 'Computer\HKEY_CURRENT_USER\Software\Unity Technologies\Unity Editor 5.x\XRMGT Rebuilding Cache'.
- Unity cannot build packages on macOS Big Sur without manual steps. See README-macOS-Big-Sur.md in the root of the SDK for instructions.
- When using Zero Iteration, Eye Tracking does not work with the Magic Leap XR Plugin (6.1.0-preview.2).
- MLCamera and MRCamera operations are not implemented asynchronously in this release and can thus cause hitches during startup and shutdown.
