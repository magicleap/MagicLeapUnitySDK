namespace MagicLeap.Android.NDK.Camera
{
    using System;
    using System.Diagnostics;

    using static CameraConstants;

    internal enum CameraStatus
    {
        /// <summary>
        /// Camera operation has succeeded.
        /// </summary>
        Ok = 0,
        /// <summary>
        /// Camera operation has failed due to an unspecified cause.
        /// </summary>
        UnknownError = kErrorBase,
        /// <summary>
        /// Camera operation has failed due to an invalid parameter being passed to the method.
        /// </summary>
        InvalidParameter = kErrorBase - 1,
        /// <summary>
        /// Camera operation has failed because the camera device has been closed,
        /// possibly because a higher-priority client has taken ownership of the camera device.
        /// </summary>
        CameraDisconnected   = kErrorBase - 2,
        /// <summary>
        /// Camera operation has failed due to insufficient memory.
        /// </summary>
        NotEnoughMemory     = kErrorBase - 3,
        /// <summary>
        /// Camera operation has failed due to the requested metadata tag cannot be found
        /// in input {@link ACameraMetadata} or {@link ACaptureRequest}.
        /// </summary>
        MetadataNotFound    = kErrorBase - 4,
        /// <summary>
        /// Camera operation has failed and the camera device has encountered a fatal error
        /// and needs to be re-opened before it can be used again.
        /// </summary>
        FatalDeviceError         = kErrorBase - 5,
        /// <summary>
        /// Camera operation has failed and the camera service has encountered a fatal error.
        /// <p>The Android device may need to be shut down and restarted to restore camera function, or there may be a persistent hardware problem.</p>
        /// <p>An attempt at recovery may be possible by closing the
        /// ACameraDevice and the ACameraManager, and trying to acquire all resources again from scratch.</p>
        /// </summary>
        FatalServiceError        = kErrorBase - 6,
        /// <summary>
        /// The {@link ACameraCaptureSession} has been closed and cannnot perform any operation other than {@link ACameraCaptureSession_close}.
        /// </summary>
        SessionClosed        = kErrorBase - 7,
        /// <summary>
        /// Camera operation has failed due to an invalid internal operation. Usually this is due to a low-level problem that may resolve itself on retry
        /// </summary>
        InvalidOperation     = kErrorBase - 8,
        /// <summary>
        /// Camera device does not support the stream configuration provided by application in {@link ACameraDevice_createCaptureSession} or {@link ACameraDevice_isSessionConfigurationSupported}.
        /// </summary>
        StreamConfigurationFailure = kErrorBase - 9,
        /// <summary>
        /// Camera device is being used by another higher priority camera API client.
        /// </summary>
        CameraInUse         = kErrorBase - 10,
        /// <summary>
        /// The system-wide limit for number of open cameras or camera resources has been reached, and more camera devices cannot be opened until previous instances are closed.
        /// </summary>
        MaximumCamerasInUse     = kErrorBase - 11,
        /// <summary>
        /// The camera is disabled due to a device policy, and cannot be opened.
        /// </summary>
        CameraDisabled       = kErrorBase - 12,
        /// <summary>
        /// The application does not have permission to open camera.
        /// </summary>
        PermissionDenied     = kErrorBase - 13,
        /// <summary>
        /// The operation is not supported by the camera device.
        /// </summary>
        UnsupportedOperation = kErrorBase - 14,
    }

    internal static class CameraStatusExtensions
    {
        [Conditional("DEVELOPMENT_BUILD")]
        public static void CheckReturnValueAndThrow(this CameraStatus actual, CameraStatus expected = CameraStatus.Ok)
        {
            if (expected != actual)
                throw new Exception($"Unexpected result from native call. Expected: '{expected}', Actual: '{actual}'");
        }
    }
}
