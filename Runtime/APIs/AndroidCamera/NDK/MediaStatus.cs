

namespace MagicLeap.Android.NDK.Media
{
    using System;
    using System.Diagnostics;
    using static MediaConstants;

    internal enum MediaStatus
    {
        /// <summary>
        /// The requested media operation completed successfully.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// This indicates required resource was not able to be allocated.
        /// </summary>
        InsufficientResource = 1100,

        /// <summary>
        /// This indicates the resource manager reclaimed the media resource used by the codec. With this error, the codec must be released, as it has moved to terminal state.
        /// </summary>
        Reclaimed             = 1101,

        /// <summary>
        /// The called media function failed with an unknown error.
        /// </summary>
        Unknown               = kMediaErrorBase,

        /// <summary>
        /// The input media data is corrupt or incomplete.
        /// </summary>
        Malformed             = kMediaErrorBase - 1,

        /// <summary>
        /// The required operation or media formats are not supported.
        /// </summary>
        Unsupported           = kMediaErrorBase - 2,

        /// <summary>
        /// An invalid (or already closed) object is used in the function call.
        /// </summary>
        InvalidObject        = kMediaErrorBase - 3,

        /// <summary>
        /// At least one of the invalid parameters is used.
        /// </summary>
        InvalidParameter     = kMediaErrorBase - 4,

        /// <summary>
        /// The media object is not in the right state for the required operation.
        /// </summary>
        InvalidOperation     = kMediaErrorBase - 5,

        /// <summary>
        /// Media stream ends while processing the requested operation.
        /// </summary>
        EndOfStream         = kMediaErrorBase - 6,

        /// <summary>
        /// An Error occurred when the Media object is carrying IO operation.
        /// </summary>
        IOError                    = kMediaErrorBase - 7,

        /// <summary>
        /// The required operation would have to be blocked (on I/O or others), but blocking is not enabled.
        /// </summary>
        WouldBlock           = kMediaErrorBase - 8,

        DRMNotProvisioned         = kMediaDRMErrorBase - 1,
        DRMResourceBusy           = kMediaDRMErrorBase - 2,
        DRMDeviceRemoved          = kMediaDRMErrorBase - 3,
        DRMShortBuffer            = kMediaDRMErrorBase - 4,
        DRMSessionNotOpened      = kMediaDRMErrorBase - 5,
        DRMTamperDetected         = kMediaDRMErrorBase - 6,
        DRMVerifyFailed           = kMediaDRMErrorBase - 7,
        DRMNeedKey                = kMediaDRMErrorBase - 8,
        DRMLicenseExpired         = kMediaDRMErrorBase - 9,

        /// <summary>
        /// There are no more image buffers to read/write image data.
        /// </summary>
        ImageReaderNoBufferAvailable = kMediaImageReaderErrorBase - 1,

        /// <summary>
        /// The AImage object has used up the allowed maximum image buffers.
        /// </summary>
        ImageReaderMaxImagesAcquired = kMediaImageReaderErrorBase - 2,

        /// <summary>
        /// The required image buffer could not be locked to read.
        /// </summary>
        ImageReaderCannotLockImage   = kMediaImageReaderErrorBase - 3,

        /// <summary>
        /// The media data or buffer could not be unlocked.
        /// </summary>
        ImageReaderCannotUnlockImage = kMediaImageReaderErrorBase - 4,

        /// <summary>
        /// The media/buffer needs to be locked to perform the required operation.
        /// </summary>
        ImageReaderImageNotLocked    = kMediaImageReaderErrorBase - 5,
    }

    internal static class MediaStatusExtensions
    {
        [Conditional("DEVELOPMENT_BUILD")]
        public static void CheckReturnValueAndThrow(this MediaStatus actual, MediaStatus expected = MediaStatus.Ok)
        {
            if (expected != actual)
                throw new Exception($"Unexpected result from native call. Expected: '{expected}', Actual: '{actual}'");
        }
    }
}
