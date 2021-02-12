# Changelog

## [Unreleased]
- [New changes here]

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

### Bug Fixes
- Fixed a crash in UserInterface.cs on app termination and on ending play mode in the editor.
- Fixed a bug in MLCamera where its apis would fail to work after recovering from a system-level eviction. Priority processes like Device Capture and Device Stream evict app's hold on the camera and return it back once those processes are done. The fix now auto-resumes the camera state that was before the eviction.
- Fixed a bug in MLPlanes where the min plane area query setting was not respected.
- Switched PoseSource to 'CenterEye' in the MainCamera prefab to get rid of the editor warning.

### Updates
- Input, Eyes, Privileges, Hand Tracking, Camera, CV Camera, Raycast, Planes, and Image Tracking APIs are now AutoAPIs (see this page for more info: https://developer.magicleap.com/en-us/learn/guides/auto-api-changes).
- CVCamera APIs have been moved from MLCamera into its own MLCVCamera class, removing the requirement from MLCamera users to specify the ComputerVision privilege if they don't intend on using that feature.
- Exposed an event named OnRawVideoFrameAvailableYUV_NativeCallbackThread in the MLCamera class which provides the frame data via pointers to unmanaged memory. This event is more efficient to use in systems where the camera frames need to be sent from Unity to an unmanaged C/C++ library like WebRTC, OpenCV or any kind of media encoder.
- Exposed asynchronous variants of expensive MLCamera functions. Each of these is accompanied with an event which is invoked when the asynchronous operation is completed.
- The VideoCapture example has been split into VideoCapture (showcasing video capture to file) and RawVideoCapture (showcasing video capture to YUV frames delivered to the app).

### Deprecations & Removals
- All APIs marked deprecated in the 0.24.2 release have been removed.
- `Start()` and `Stop()` methods of Input, Eyes, Privileges, Hand Tracking, Camera, CV Camera, Raycast, Planes, and Image Tracking APIs have been deprecated (see this page for more info: https://developer.magicleap.com/en-us/learn/guides/auto-api-changes).
- `MLCamera.IntrinsicCalibrationParameters`, `MLCamera.GetIntrinsicCalibrationParameters()` and `MLCamera.GetFramePose()` have been deprecated in favor of their `MLCVCamera` counterparts.

### Known Issues
- With Unity 2020.2 on Windows, sometimes the XR Plug-in Management settings will be grayed out. This can be fixed by closing the editor, using `regedit` to delete 'Computer\HKEY_CURRENT_USER\Software\Unity Technologies\Unity Editor 5.x\XRMGT Rebuilding Cache'.
- Unity cannot build packages on macOS Big Sur without manual steps. See README-macOS-Big-Sur.md in the root of the SDK for instructions.
- When using Zero Iteration, Eye Tracking does not work with the Magic Leap XR Plugin (6.1.0-preview.2).
- MLCamera and MRCamera operations are not implemented asynchronously in this release and can thus cause hitches during startup and shutdown.
