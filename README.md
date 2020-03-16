# Release Info

## Magic Leap Unity SDK Release 0.24

## Supported Unity Editor Release: 2019.3, (Tested against Magic Leap XR Plugin version 4.0.7-preview.1), with the Lumin Platform installed.

The files in this folder are meant to provide examples, best practices and wrappers to
help the user work and understand the MagicLeap APIs.

These files can change or even be removed from one release to another. If you're planning
on depending or modifying these assets for your own project, we recommend that you duplicate
the files, change the names and move them out of the Assets/MagicLeap folder. This will avoid
issues like your changes being deleted when you upgrade to a new unitypackage.

# Copyright

Copyright (c) 2019-present Magic Leap, Inc. All Rights Reserved.
Use of this file is governed by the Developer Agreement, located
here: https://id.magicleap.com/terms/developer

# Release Notes

## Important Notes
* A major refactor and optimization effort has caused many enums, classes, and structs to be rescoped. A detailed list of changes is in the Deprecation Notices section below.
* The Magic Leap Unity plugin source is now available on github https://github.com/MagicLeap/MagicLeapUnitySDK/
* Non-serializable API code targeting Lumin only is wrapped with #if PLATFORM_LUMIN which will allow developers to build against other platforms.
* The previous MLResult.Code has been renamed to MLResult.Result. MLResult.Code is now the enum of all possible results.
* It is required to start Zero Iteration before importing the support libraries (MagicLeap > MLRemote > Import Support Libraries).
* Please note that Object Recognition APIs are marked as experimental.

## Features

### All examples have been refactored with a common UI, consistent layout, helper scripts (StarterKit) and functionality improvements.

* New example for virtual keyboard included.
* Added new MLAppConnect API for content sharing.
* Added new MLFoundObjects API for object recognition.
* Added new MLBluetoothLE API for Bluetooth Low Energy (BLE) support.
* Updated MLEyes API to retrieve gaze for left and right eyes.
* Updated MLDispatch to include OAuth support.
* Updated MLPersistentCoordinateFrames API to include PCF type.
* Updated MLConnections API to include acknowledgement and selection functionality.
* Updated MLMediaPlayer API to expose power state transitions.
* Updated MLMediaPlayer API to include CEA-708 support.
* Updated MLMediaPlayer API to include media synchronization support.
* Updated MLMediaPlayer API to support AC3 5.1 audio for DASH media streams.

## Bug Fixes
* AudioCapture example audio crackling issue has been fixed.

## Known Issues
* MLAppConnect API OnPipeEvent is not triggered for MicAudioPipe.
* Running Device Stream while MLCamera is started will cause MLCamera to lose connection.
* Deprecation Notice (Deprecation(Warning), Obsolete(Error), Removed)
* MLMediaPlayer.Cea608CaptionLine.DisplayChars has been marked deprecated and replaced with DisplayString.

* MLResult.Code.NotCompatible is marked deprecated and is replaced with NotImplemented.

* MLCameraMetadataControlAWBMode, MLCameraMetadataControlAWBLock, MLCameraMetadataColorCorrectionMode, MLCameraMetadataControlAEAntibandingMode, MLCameraMetadataScalerAvailableFormats, MLCameraMetadataScalerAvailableStreamConfigurations, MLCameraMetadataControlAEState, MLCameraMetadataControlAWBState, MLCameraPlaneInfo, MLCameraOutput, YUVBuffer, YUVFrameInfo, MLCameraResultExtras, MLCameraFrameMetadata, MLCameraColorCorrectionTransform, MLCameraColorCorrectionGains, MLCameraControlAETargetFPSRange, and MLCameraScalerCropRegion enums and structs are deprecated and have been rescoped to be under MLCamera.

* MLConnectionsInviteeFilter is deprecated and replaced with MLConnections.InviteeFilter.

* MLContacts.DEFAULT_FETCH_LIMIT is deprecated and replaced with MLContacts.DefaultFetchLimit.

* MLContentBindingType, MLContentBinding, MLContentBindings, and MLContentBinder are deprecated and replaced with IMLPCFBinding to create bindings instead, see TransformBinding.cs in the Unity Package provided.

* MLHand is deprecated and replaced with MLHandTracking.Hand.

* MLHandMesh and MLHandMeshBlock are deprecated and replaced with MLHandMeshing.Meshing.Mesh and MLHandMeshing.Meshing.Mesh.Block.

* MLHands is deprecated and replaced with MLHandTracking.

* MLHandKeyPose, MLKeyPointFilterLevel, MLPoseFilterLevel, and MLHandType are deprecated and rescoped underneath MLHandTracking.

* MLImageTrackerImageFormat and MLImageTargetTrackingStatus are deprecated and replaced with MLImageTracker.ImageFormat and MLImageTracker.Target.TrackingStatus.

* MLInputControllerTouchpadGestureState, MLInputControllerTouchpadGestureType, MLInputControllerButton, MLInputControllerType, MLInputControllerTouchpadGestureDirection, MLInputControllerFeedbackPatternLED, MLInputControllerFeedbackEffectLED, MLInputControllerFeedbackColorLED, MLInputControllerFeedbackEffectSpeedLED, MLInputControllerFeedbackPatternVibe, and MLInputControllerFeedbackIntensity are deprecated and have bene rescoped to be under MLInput.Controller.

* MLInputTabletDeviceStateMask is deprecated and replaced with MLInput.TabletDeviceStateMask.

* MLKeyPoint is deprecated and replaced with MLHandTracking.KeyPoint.

* MLKeyPoseManager is deprecated and replaced with MLHandTracking.KeyposeManager.

* MLLightingTracker and MLLightingTrackingCamera are deprecated and replaced with MLLightingTracking and MLLightingTracking.Camera.

* MLLocalization is deprecated and replaced with MLLocale.

* MLLocationData is deprecated and replaced with MLLocation.Location.

* MLMediaPlayer.OnSubtitleDataFound has been deprecated and replaced with MLMediaPlayer.OnSubtitle608DataFound.

* MLMediaPlayerInfo, MLMediaDRMKeyType, MLMediaDRMTrack, MLMediaPlayerTrackType, MLCea608CaptionColor, MLCea608CaptionStyle, MLCea608CaptionDimension, TrackData, MediaPlayerTracks, MLCea608CaptionStyleColor, MLCea608CaptionPAC, MLCea608CaptionLineInternal, MLCea608CaptionSegmentInternal, MLCea608CaptionLine, and MLCea608CaptionSegment are deprecated and have bene rescoped to be under MLMediaPlayer.

* MLMovementSettings, MLMovement3DofControls, MLMovement6DofControls, MLMovementObject, MLMovement3DofSettings, and MLMovement6DofSettings are deprecated and rescoped to be under MLMovement.

* MLNetworkingWifiData is deprecated and replaced with MLNetworking.WifiData.

* MLPCF is deprecated and replaced with MLPersistentCoordinateFrames.PCF.

* MLPCF.Orientation is deprecated and replaced with MLPersistentCoordinateFrames.PCF.Rotation.

* MLPersistentCoordinateFrames.DelayBetweenAllPCFsQueryInSeconds and StartTimeOutInSeconds are depreated and scheduled for removal.

* MLPersistentCoordinateFrames.IsReady, OnInitialized, and GetAllPCFs have been deprecated and replaced with MLPersistentCoordinateFrames.IsLocalized, OnLocalized, and FindAllPCFs.

* MLPersistentCoordinateFrames.GetPCFPose is deprecated. Please directly call Update() using a reference to an MLPersistentCoordinateFrames.PCF object instead.

* MLPersistentStore is deprecated and replaced with MLPersistentCoordinateFrames.PCF.BindingsLocalStorage.

* MLResult.ResultOk and MLResult constructors are deprecated as MLResult.Create should be used instead.

* MLResultCode is deprecated and replaced with MLResult.Code.

* MLThumb is deprecated and replaced with MLHandTracking.Thumb.

* MLWorldPlanes is deprecated and replaced with MLPlanes.

* MLWorldPlanesQueryFlags, MLWorldPlanesQueryParams, MLWorldPlane, MLWorldPlaneBoundaries, MLWorldPlaneBoundary, and MLWorldPolygon are deprecated and re-scoped to be under MLPlanes.

* MLWorldRays is deprecated and replaced with MLRaycast.

* MLWrist is deprecated and replaced with MLHandTracking.Wrist.

* MagicLeapBitMask is deprecated and replaced with MLBitMask.

* All MLGetResultString functions have been marked obsolete and replaced with MLResult.CodeToString.

* MLPrivileges PwFoundObjRead and ScreensProvider are now obsolete.

* MLResult.Codes ScreensServiceNotAvailable, ScreensPermissionDenied, ScreensInvalidScreenId are now obsolete.

* MagicLeapDevice is immediately obsolete and replaced with MLDevice and its Head tracking items have been moved to MLHeadTracking.

* MLCameraCaptureSettings is obsolete and replaced with MLCamera.CaptureSettings.

* MLCameraError, MLCameraCaptureType, MLCameraOutputFormat, MLCameraDeviceStatusFlag, MLCameraCaptureStatusFlag, MLCameraMetadataControlAEMode, MLCameraMetadataColorCorrectionAberrationMode, and MLCameraMetadataControlAELock enums have been marked obsolete and rescoped to under MLCamera.

* MLCameraResultSettings is obsolete and replaced with MLCamera.ResultSettings.

* MLCameraSettings and SensorInfoSensitivtyRangeValues are obsolete and replaced with MLCamera.GeneralSettings and MLCamera.SensorInfoSensitivityRangeValues.

* MLConnections.OnRequestComplete event is obsolete and replaced with MLConnections.OnInvitationResult.

* MLConnectionsInviteStatus is obsolete and replaced with MLConnections.InviteStatus.

* MLContactsOperationStatus, MLContactsSearchField, MLContactsSelectionField, MLContactsOperationResult, MLContactsListResult, MLContactsContactList, MLContactsTaggedAttribute, MLContactsTaggedAttributeList, and MLContactsContact are all obsolete and rescoped to under MLContacts.

* MLContactsListPage is obsolete and replaced with MLContacts.ListPage.

* MLContactsSearchPage is obsolete and replaced with MLContacts.SearchPage.

* MLContactsSelectionPage is obsolete and replaced with MLContacts.SelectionPage.

* MLControllerMode and MLControllerCalibAccuracy are obsolete and rescoped under MLInput.Controller.

* MLDispatcher class is obsolete and replaced with MLDispatch.

* MLEye and MLEyeTrackingCalibrationStatus are obsolete and replaced with  MLEyes.Eye and MLEyes.Calibration.

* MLHand.KeyPoseConfidence, MLHand.KeyPoseConfidenceFiltered, MLHand.OnKeyPoseBegin, MLHand.OnKeyPoseEnd, MLHand.HandType, MLHand.BeginKeyPose, and MLHand.EndKeyPose are obsolete and are now rescoped under MLHandTracking.Hand.

* MLHandMeshing.HandMeshRequestCallback and MLHandMeshing.RequestHandMesh are obsolete and replaced with MLHandMeshing.RequestHandMeshCallback and MLHandMeshing.RequestHandMesh.

* MLHeadTrackingError, MLHeadTrackingMode, MLHeadTrackingMapEvent, MLHeadTrackingState, and MapEventsExtension are obsolete and have been rescoped to be under MLHeadTracking.

* MLIdentityAttribute and MLIdentityProfile are obsolete and replaced with MLIdentity.Profile.Attribute and MLIdentity.Profile.

* MLImageTarget and MLImageTargetResult are obsolete and replaced with MLImageTracker.Target and MLImageTracker.Target.Result.

* MLImageTrackerSettings is obsolete and replaced with MLImageTracker.Settings.

* MLInput.MLTabletState is obsolete and replaced with MLInput.TabletState.

* MLInputConfiguration is obsolete and replaced with MLInput.Configuration.

* MLInputController is obsolete and has been replaced with MLInput.Controller.

* MLInputControllerTouchpadGesture is obsolete and replaced with MLInput.Controller.TouchpadGesture.

* MLInputControllerDof, MLInputTabletDeviceType, MLInputTabletDeviceToolType, and MLInputTabletDeviceButton are obsolete and have been rescoped to be under MLInput.

* MLMusicServiceErrorType, MLMusicServiceStatus, MLMusicServicePlaybackState, MLMusicServiceRepeatState, MLMusicServiceShuffleState, MLMusicServiceTrackType, MLMusicServiceMetadata, and MLMusicServiceError are obsolete and have been rescoped under MLMusicService.

* MLPCF events OnCreate, OnUpdate, OnLost, and OnRegain are obsolete and replaced with MLPersistentCoordinateFrames.PCF.OnStatusChange.

* MLPersistentCoordinateFrames.GetPCFPosition is now obsolete. Please directly call Update() using a reference to an MLPersistentCoordinateFrames.PCF object instead.

* MLPrivilegeId is obsolete and replaced with MLPrivileges.Id.

* MLRuntimeRequestPrivilegeId is obsolete and replaced with MLPrivileges.RuntimeRequestId.

* MLScreens, MLScreensScreenInfo, and MLScreensWatchHistoryEntry are obsolete and scheduled for removal.

* MLVerbosity and MLVerbosity.Level are obsolete and replaced with MLPluginLog and MLPluginLog.Level.

* MLFinger.DIP, MLThumb.CMC, MLInputDevice.ChangedThisFrame, MLMusicService.GetMetaData(MLMusicServiceTrackType trackType, ref MLMusicServiceMetadata metadata), MLHandKeyPose.OpenHandBack, MLPrivilegeId.Occlusion, and MLPrivilegeId.AudioRecognizer have been fully removed.