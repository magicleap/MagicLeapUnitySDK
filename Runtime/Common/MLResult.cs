// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using UnityEngine.XR.MagicLeap.Native;
    
    //Disabling WebRTC deprecated warning 
    #pragma warning disable 618

    /// <summary>
    /// ALWAYS INHERIT FROM THIS
    /// </summary>
	public interface IMLRequest<Params, Result>
    {
        MLResult Start(Params parameters);
        MLResult TryGetResult(out Result result);
    }

    /// <summary>
    /// INHERIT FROM THIS IF YOUR REQUEST SHOULD BE A CLASS
    /// </summary>
    public abstract class MLRequest<Params, Result> : IMLRequest<Params, Result>
    {
        protected MLRequest() { }

        public Params Parameters => parameters;

        protected Params parameters;

        protected ulong handle = MagicLeapNativeBindings.InvalidHandle;

        public abstract MLResult Start(Params parameters);

        public abstract MLResult TryGetResult(out Result result);

        // if u also need to dispose of managed data, inherit from IDisposable
        protected abstract void Dispose(bool disposing);

        ~MLRequest() => this.Dispose(false);
    }

    /// <summary>
    /// Magic Leap API return value.
    /// </summary>
    public partial struct MLResult
    {

        /// <summary>
        /// Enables asynchronous native calls to use the same interface as synchronous ones
        /// </summary>
        public static implicit operator Task<MLResult>(MLResult r) => Task.FromResult(r);

        /// <summary>
        /// The code of this MLResult.
        /// Indicates the result status.
        /// </summary>
        public readonly Code Result;

        /// <summary>
        /// Holds a list of all the created MLResults mapped to their specific MLResult.Code.
        /// </summary>
        private static Dictionary<Code, MLResult> existingResults;

        /// <summary>
        /// The message of this MLResult
        /// Provides a readable form of the result status.
        /// </summary>
        private string message;

        /// <summary>
        /// Initializes a new instance of the <see cref="MLResult" /> struct.
        /// </summary>
        /// <param name="result">The result code to give this MLResult.</param>
        /// <param name="msg">The message to give this MLResult.</param>
        private MLResult(Code result, string msg)
        {
            this.Result = result;
            this.message = msg;
        }

        /// <summary>
        /// Identifier of Magic Leap API results
        /// </summary>
        public enum Code : int
        {
            /// <summary>
            /// Operation completed successfully.
            /// </summary>
            Ok = (CodePrefix.MLResultGlobal << 16),

            /// <summary>
            /// Asynchronous operation has not completed
            /// </summary>
            Pending,

            /// <summary>
            /// Operation has timed out.
            /// </summary>
            Timeout,

            /// <summary>
            /// Request to lock a shared resource that is already locked.
            /// </summary>
            Locked,

            /// <summary>
            /// Operation failed due to an unspecified internal error.
            /// </summary>
            UnspecifiedFailure,

            /// <summary>
            /// Operation failed due to an invalid parameter being supplied.
            /// </summary>
            InvalidParam,

            /// <summary>
            /// Operation failed because memory failed to be allocated.
            /// </summary>
            AllocFailed,

            /// <summary>
            /// Operation failed because a required permission has not been granted.
            /// </summary>
            PermissionDenied,

            /// <summary>
            /// Operation failed because it is not currently implemented.
            /// </summary>
            NotImplemented,

            /// <summary>
            /// Operation failed because the client count exceeded the number of clients available.
            /// </summary>
            ClientLimitExceeded,

            /// <summary>
            /// Operation failed because the requested pose is not available.
            /// </summary>
            PoseNotFound,

            /// <summary>
            /// Operation failed because feature isn't supported on current device version.
            /// </summary>
            IncompatibleSKU,

            /// <summary>
            /// Operation failed because the Perception System was not started.
            /// </summary>
            PerceptionSystemNotStarted,

            /// <summary>
            /// Operation failed because it has been invoked at the wrong time.
            /// </summary>
            IllegalState,

            /// <summary>
            /// Operation failed because of missing/incompatible license.
            /// </summary>
            LicenseError,

            /// <summary>
            /// Operation failed because the timestamp parameter was outside the expected range.
            /// </summary>
            InvalidTimestamp,

            // MLAudioResult

            /// <summary>
            /// Audio function not implemented.
            /// </summary>
            AudioNotImplemented = (CodePrefix.MLAudioResult << 16),

            /// <summary>
            /// Not a valid MLHandle for a sound or input.
            /// </summary>
            AudioHandleNotFound,

            /// <summary>
            /// Sample rate not supported.
            /// </summary>
            AudioInvalidSampleRate,

            /// <summary>
            /// Bits per sample not supported.
            /// </summary>
            AudioInvalidBitsPerSample,

            /// <summary>
            /// Valid bits per sample not supported.
            /// </summary>
            AudioInvalidValidBits,

            /// <summary>
            /// Sample format not supported.
            /// </summary>
            AudioInvalidSampleFormat,

            /// <summary>
            /// Channel count not supported.
            /// </summary>
            AudioInvalidChannelCount,

            /// <summary>
            /// Buffer size too small.
            /// </summary>
            AudioInvalidBufferSize,

            /// <summary>
            /// Buffer not ready for read or write.
            /// </summary>
            AudioBufferNotReady,

            /// <summary>
            /// Specified file not found.
            /// </summary>
            AudioFileNotFound,

            /// <summary>
            /// Specified file has unsupported format.
            /// </summary>
            AudioFileNotRecognized,

            /// <summary>
            /// Specified resource is not on the list.
            /// </summary>
            AudioResourceNotFound,

            /// <summary>
            /// Data was unloaded or file was closed.
            /// </summary>
            AudioResourceDiscarded,

            /// <summary>
            /// Requested operation not possible for given item.
            /// </summary>
            AudioOperationUnavailable,

            /// <summary>
            /// Requested operation has not yet been processed.
            /// </summary>
            AudioInternalOperationNotReady,

            /// <summary>
            /// Internal configuration problem was detected.
            /// </summary>
            AudioInternalConfigError,

            /// <summary>
            /// Operation failed because input service is not available.
            /// </summary>
            InputServiceNotAvailable = (CodePrefix.MLInputResult << 16),

            /// <summary>
            /// Operation failed because a required permission has not been granted.
            /// </summary>
            InputPermissionDenied,

            /// <summary>
            /// Operation failed because a required device was not found.
            /// </summary>
            InputDeviceNotFound,

            /// <summary>
            /// Operation failed because the service was in an illegal state.
            /// </summary>
            InputIllegalState,

            /// <summary>
            /// Operation failed because of an internal error.
            /// </summary>
            InputInternalError,

            /// <summary>
            /// Operation failed because a required speech feature is disabled in system settings.
            /// </summary>
            InputSpeechDisabled,

            // MLMediaPlayerResult

            /// <summary>
            /// Media errors (example: Codec not supported).
            /// </summary>
            MediaPlayerServerDied = (CodePrefix.MLMediaPlayerResult << 16),

            /// <summary>
            /// Runtime errors.
            /// </summary>
            MediaPlayerNotValidForProgressivePlayback,

            /// <summary>
            /// Media already connected.
            /// </summary>
            MediaAlreadyConnected = (CodePrefix.MLMediaResult << 16),

            /// <summary>
            /// Media not connected.
            /// </summary>
            MediaNotConnected,

            /// <summary>
            /// Media had unknown host.
            /// </summary>
            MediaUnknownHost,

            /// <summary>
            /// Media cannot connect.
            /// </summary>
            MediaCannotConnect,

            /// <summary>
            /// Media IO.
            /// </summary>
            MediaIO,

            /// <summary>
            /// Media connection was lost.
            /// </summary>
            MediaConnectionLost,

            /// <summary>
            /// Deprecated error.
            /// </summary>
            MediaLegacy1,

            /// <summary>
            /// Media was malformed.
            /// </summary>
            MediaMalformed,

            /// <summary>
            /// Media was out of range.
            /// </summary>
            MediaOutOfRange,

            /// <summary>
            /// Media buffer was too small.
            /// </summary>
            MediaBufferTooSmall,

            /// <summary>
            /// Media not supported.
            /// </summary>
            MediaUnsupported,

            /// <summary>
            /// Media end of stream.
            /// </summary>
            MediaEndOfStream,

            /// <summary>
            /// Media format changed.
            /// </summary>
            MediaFormatChanged,

            /// <summary>
            /// Media discontinuity.
            /// </summary>
            MediaDiscontinuity,

            /// <summary>
            /// Media output buffers changed.
            /// </summary>
            MediaOutputBuffersChanged,

            /// <summary>
            /// Media permission revoked.
            /// </summary>
            MediaPermissionRevoked,

            /// <summary>
            /// Media had an unsupported audio format.
            /// </summary>
            MediaUnsupportedAudioFormat,

            /// <summary>
            /// Media heartbeat requested to be terminated.
            /// </summary>
            MediaHeartbeatTerminateRequested,

            /// <summary>
            /// Unknown Media Codec Resource Request Error.
            /// </summary>
            MediaUnknownResource,

            /// <summary>
            /// Invalid Request on Media Codec Resources.
            /// </summary>
            MediaInvalidResourceRequest,

            /// <summary>
            /// Only Hardware Media Codec Resources are available. This error code 
            /// is a result of requesting for Software Media Codec Resource that is 
            /// not available.
            /// </summary>
            MediaOnlyHWResourceAvailable,

            /// <summary>
            /// Only Software Media Codec Resources are available. This error code 
            /// is a result of requesting for Hardware Media Codec Resource that is 
            /// not available.
            /// </summary>
            MediaOnlySWResourceAvailable,

            /// <summary>
            /// No more (Hardware or Software) Media Codec Resources available.
            /// </summary> 
            MediaNoMoreResources,

            /// <summary>
            /// No more Hardware Media Codec Resources available
            /// </summary>
            MediaNoMoreHWResources,

            /// <summary>
            /// No more Software Media Codec Resources available.
            /// </summary>
            MediaNoMoreSWResources,

            /// <summary>
            /// Media Codec Resource request made with Invalid Codec parameters. 
            /// This error code could be a result of one of these cases (not limited to)
            /// 1) Requested video frame width or height is beyond the maximum 
            /// resolution (width or height) supported. 
            /// 2) Requested Framerate/Sample rate/Bitratre/Channel count is not supported.
            /// </summary>
            MediaInvalidCodecParam,

            // MLMediaDRMResult

            /// <summary>
            /// Error code for undefined type.
            /// </summary>
            MediaDRMUnknown = (CodePrefix.MLMediaDRMResult << 16),

            /// <summary>
            /// Error code for no DRM license.
            /// </summary>
            MediaDRMNoLicense,

            /// <summary>
            /// Error code for DRM license expired.
            /// </summary>
            MediaDRMLicenseExpired,

            /// <summary>
            /// Error code for DRM session not expired.
            /// </summary>
            MediaDRMSessionNotOpened,

            /// <summary>
            /// Error code for DRM when decrypt unit is not initialized.
            /// </summary>
            MediaDRMDecryptUnitNotInitialized,

            /// <summary>
            /// Error code for DRM when failed to decrypt data.
            /// </summary>
            MediaDRMDecrypt,

            /// <summary>
            /// Error code for DRM can not handle the operation.
            /// </summary>
            MediaDRMCannotHandle,

            /// <summary>
            /// Error code for DRM when data is tampered.
            /// </summary>
            MediaDRMTamperDetect,

            /// <summary>
            /// Error Code when an operation on a MLMediaDRM handle is attempted and the device does not have a certificate.
            /// The app should obtain and install a certificate using the MLMediaDRM provisioning methods then retry the operation.
            /// </summary>
            MediaDRMNotProvisioned,

            /// <summary>
            /// Error code for Device License Revoked.
            /// </summary>
            MediaDRMDeviceRevoked,

            /// <summary>
            /// Error code if the MLMediaDRM operation fails when the required resources are in use.
            /// </summary>
            MediaDRMResourceBusy,

            /// <summary>
            /// Error code for insufficient output protection.
            /// </summary>
            MediaDRMInsufficientOutputProtection,

            /// <summary>
            /// Error code for insufficient output protection.
            /// </summary>
            MediaDRMLastUsedErrorCode = MediaDRMInsufficientOutputProtection,

            /// <summary>
            /// Range for vendor specific DRM errors.
            /// </summary>
            MediaDRMVendorMin = (CodePrefix.MLMediaDRMResult << 16) + 500,

            /// <summary>
            /// Range for vendor specific DRM errors.
            /// </summary>
            MediaDRMVendorMax = (CodePrefix.MLMediaDRMResult << 16) + 999,

            // MLMediaGenericResult

            /// <summary>
            /// Media invalid operation.
            /// </summary>
            MediaGenericInvalidOperation = (CodePrefix.MLMediaGenericResult << 16),

            /// <summary>
            /// Media bad type.
            /// </summary>
            MediaGenericBadType,

            /// <summary>
            /// Media name not found.
            /// </summary>
            MediaGenericNameNotFound,

            /// <summary>
            /// Media handle not found.
            /// </summary>
            MediaGenericHandleNotFound,

            /// <summary>
            /// Media <c>NoInit</c>.
            /// </summary>
            MediaGenericNoInit,

            /// <summary>
            /// Media already exists.
            /// </summary>
            MediaGenericAlreadyExists,

            /// <summary>
            /// Media dead object.
            /// </summary>
            MediaGenericDeadObject,

            /// <summary>
            /// Media had a failed transaction.
            /// </summary>
            MediaGenericFailedTransaction,

            /// <summary>
            /// Media had a bad index.
            /// </summary>
            MediaGenericBadIndex,

            /// <summary>
            /// Media not enough data.
            /// </summary>
            MediaGenericNotEnoughData,

            /// <summary>
            /// Media would block.
            /// </summary>
            MediaGenericWouldBlock,

            /// <summary>
            /// Media had an unknown transaction.
            /// </summary>
            MediaGenericUnknownTransaction,

            /// <summary>
            /// Media FDS not allowed.
            /// </summary>
            MediaGenericFDSNotAllowed,

            /// <summary>
            /// Media unexpected null.
            /// </summary>
            MediaGenericUnexpectedNull,

            /// <summary>
            /// Media not available.
            /// </summary>
            MediaGenericNotAvailable,

            // MLSnapshotResult

            /// <summary>
            /// Derivatives not calculated for requested coordinate frame.
            /// </summary>
            SnapshotDerivativesNotCalculated = (CodePrefix.MLSnapshotResult << 16),

            // MLWebRTCResult

            /// <summary>
            /// Instance not created.
            /// </summary>
            WebRTCInstanceNotCreated = (CodePrefix.MLWebRTCResult << 16),

            /// <summary>
            /// Mismatching handle.
            /// </summary>
            WebRTCMismatchingHandle,

            /// <summary>
            /// Invalid frame format.
            /// </summary>
            WebRTCInvalidFrameFormat,

            /// <summary>
            /// Invalid frame plane count.
            /// </summary>
            WebRTCInvalidFramePlaneCount,

            /// <summary>
            /// Native Frame failed to be sent
            /// </summary>
            WebRTCSendNativeFrameError,

            /// <summary>
            /// Data channel is closed.
            /// </summary>
            WebRTCDataChannelIsClosed,

            // MLSpatialAnchorResult

            /// <summary>
            /// The maximum number of anchors for the current space has been reached.
            /// </summary>
            AnchorsMaxAnchorLimitReached = (CodePrefix.MLAnchorsResult << 16),

            /// <summary>
            /// The minimum distance between anchors was not met
            /// </summary>
            AnchorsMinDistanceThresholdExceeded,

            /// <summary>
            ///  The space has not been sufficiently mapped to allow this operation.
            /// </summary>
            AnchorsInsufficientMapping,

            /// <summary>
            /// The provided anchor id was not valid.
            /// </summary>
            AnchorsInvalidId,

            /// <summary>
            ///  The provided expiration suggestion was not valid.
            /// </summary>
            AnchorsInvalidExpirationTimestamp,

            /// <summary>
            /// The operation cannot be completed because the device has not yet localized. 
            /// </summary>
            AnchorsNotLocalized,

            /// <summary>
            ///  There was an error communicating with the server.
            /// </summary>
            AnchorsServerError,

            /// <summary>
            /// The operation failed because the underlying service is not yet available.
            /// </summary>
            AnchorsServiceUnavailable,

            // MLVoiceResult

            /// <summary>
            /// Operation failed because a required voice intent feature is disabled in system settings.
            /// </summary>
            VoiceIntentDisabled = (CodePrefix.MLVoiceResult << 16),

            // MLWebViewResult

            /// <summary>
            /// Operation failed because zoom limit was reached.
            /// </summary>
            WebViewZoomLimitReached = (CodePrefix.MLWebViewResult << 16),

            // MLSpaceResult

            /// <summary>
            /// There was an error communicating with the server.
            /// </summary>
            SpacesServerError = (CodePrefix.MLSpaceResult << 16),

            /// <summary>
            /// The operation failed because the underlying service is not yet available, retry later.
            /// </summary>
            SpacesServiceUnavailable,

            /// <summary>
            /// Space is not available.
            /// </summary>
            UnavailableSpace,

            /// <summary>
            /// Space is not compatible with the current OS version.
            /// </summary>
            IncompatibleSpace,

            /// <summary>
            /// The operation failed because the Space being imported already exists in the device.
            /// </summary>
            SpaceAlreadyExists,

            // Facial Expression

            /// <summary>
            /// Unable to detect the eyes, check MLHeadsetFitStatus.
            /// </summary>
            HeadsetFitIssue = (CodePrefix.MLFacialExpressionResult << 16),

            /// <summary>
            /// Operation failed because enable_eye_expression in MLFacialExpressionSettings is disabled.
            /// </summary>
            EyeExpressionDisabled,

            /// <summary>
            /// Indicates the component is not connected.
            /// </summary>
            PowerManagerNotConnect = (CodePrefix.MLPowerManager << 16),

            /// <summary>
            /// Indicates the component does not currently support transitioning to requested state.
            /// </summary>
            PowerManagerInvalidStateTransition,

            /// <summary>
            /// Indicates the component does not currently support transitioning to a different state.
            /// </summary>
            PowerManagerStateTransitionsDisabled,

            /// <summary>
            /// Indicates the component does not support the requested power state.
            /// </summary>
            PowerManagerUnsupportedState,

            /// <summary>
            /// APIDLLNotFound.
            /// </summary>
            APIDLLNotFound = int.MaxValue - 0,

            /// <summary>
            /// APIDLLSymbolsNotFound.
            /// </summary>
            APISymbolsNotFound = int.MaxValue - 1,

        }

        /// <summary>
        /// this.Result code high order 2 byte prefix values used by the CAPI to group results by functionality. This is a <c>ushort</c> to facilitate bit shifting for final result values.
        /// </summary>
        private enum CodePrefix : ushort
        {
            /// <summary>
            /// Code for global MLResults.
            /// </summary>
            MLResultGlobal = 0x0000,

            /// <summary>
            /// Code for audio related MLResults.
            /// </summary>
            MLAudioResult = 0x9e11,

            /// <summary>
            /// Code for mediaDRM related MLResults.
            /// </summary>
            MLMediaDRMResult = 0x62ce,

            /// <summary>
            /// Code for generic media related MLResults.
            /// </summary>
            MLMediaGenericResult = 0xbf3b,

            /// <summary>
            /// Code for media player related MLResults.
            /// </summary>
            MLMediaPlayerResult = 0xc435,

            /// <summary>
            /// Code for media related MLResults.
            /// </summary>
            MLMediaResult = 0x4184,

            /// <summary>
            /// Code for snapshot related MLResults.
            /// </summary>
            MLSnapshotResult = 0x87b8,

            /// <summary>
            /// Code for web rtc related MLResults.
            /// </summary>
            MLWebRTCResult = 0xefc7,

            /// <summary>
            /// Code for MLAnchors related MLResults.
            /// </summary>
            MLAnchorsResult = 0x439,

            /// <summary>
            /// Code for MLVoice related MLResults.
            /// </summary>
            MLVoiceResult = 0xbe7a,

            /// <summary>
            /// Code for MLInput related MLResults.
            /// </summary>
            MLInputResult = 0x21dc,

            /// <summary>
            /// Code for MLWebView related MLResults.
            /// </summary>
            MLWebViewResult = 0xebf7,

            /// <summary>
            /// Code for MLSpace related MLResults.
            /// </summary>
            MLSpaceResult = 0x10cc,

            /// <summary>
            /// Code for MLPowerManager related MLResults.
            /// </summary>
            MLPowerManager = 0x4c8a,

            /// <summary>
            /// Code for MLFacialExpression related MLResults.
            /// </summary>
            MLFacialExpressionResult = 0x18cd
        }

        /// <summary>
        /// Gets a value indicating whether the result code is MLResult.Code.Ok
        /// Note that in some cases the result can be different than MLResult.Code.Ok
        /// and still valid (e.g. MLResult.Code.Pending, MLResult.Code.Ok)
        /// </summary>
        public bool IsOk
        {
            get
            {
                return Code.Ok == this.Result;
            }
        }

        /// <summary>
        /// Create a new MLResult or retrieve an already initialized MLResult with the result needed.
        /// </summary>
        /// <param name="result">The code to use for the created MLResult.</param>
        /// <param name="msg">The message to use for the created MLResult.</param>
        /// <returns>A new or cached MLResult with the provided code and message.</returns>
        public static MLResult Create(Code result, string msg = null)
        {
            if (string.IsNullOrEmpty(msg))
            {
                MLResult toReturn;

                if (existingResults == null)
                {
                    existingResults = new Dictionary<Code, MLResult>();
                }

                if (existingResults.Count == 0 || !existingResults.TryGetValue(result, out toReturn))
                {
                    toReturn = new MLResult(result, msg);

                    existingResults.Add(result, toReturn);
                }

                return toReturn;
            }
            else
            {
                return new MLResult(result, msg);
            }
        }

        /// <summary>
        /// Provides the string value for any MLResult.Code.
        /// </summary>
        /// <param name="resultCode">The code to convert into a string value.</param>
        /// <returns>The string value of the given MLResult.Code.</returns>
        public static string CodeToString(MLResult.Code resultCode)
        {
            string codeString = string.Empty;

            switch ((CodePrefix)((int)resultCode >> 16))
            {
                case CodePrefix.MLResultGlobal:
                    codeString = MagicLeapNativeBindings.MLGetResultString(resultCode);
                    break;
                case CodePrefix.MLSnapshotResult:
                    codeString = MagicLeapNativeBindings.MLGetSnapshotResultString(resultCode);
                    break;
                case CodePrefix.MLAudioResult:
                    codeString = MLAudioOutput.GetResultString(resultCode);
                    break;
                case CodePrefix.MLMediaDRMResult:
                case CodePrefix.MLMediaGenericResult:
                case CodePrefix.MLMediaPlayerResult:
                case CodePrefix.MLMediaResult:
                    codeString = MLMedia.NativeBindings.GetResultString(resultCode);
                    break;
                case CodePrefix.MLWebRTCResult:
                    codeString = Marshal.PtrToStringAnsi(MLWebRTC.NativeBindings.MLWebRTCGetResultString(resultCode));
                    break;
                case CodePrefix.MLInputResult:
                    codeString = MagicLeapNativeBindings.MLGetInputResultString(resultCode);
                    break;
                case CodePrefix.MLWebViewResult:
                    // No MLWebViewGetResultString in API 
                    codeString = "Web View Result Code - String Not Available";
                    break;
                case CodePrefix.MLAnchorsResult:
                    codeString = "MLResult_" + resultCode;
                    break;
                case CodePrefix.MLPowerManager:
                    codeString = resultCode.ToString();
                    break;
                case CodePrefix.MLFacialExpressionResult:
                    codeString = resultCode.ToString();
                    break;
                default:
                    // This will catch any unknown/invalid return values.
                    codeString = MagicLeapNativeBindings.MLGetResultString(resultCode);
                    break;
            }

            return codeString;
        }

        /// <summary>
        /// Indicates whether the result code is Code.Ok
        /// Note that in some cases the result can be different than MLResult.Code.Ok
        /// and still be valid (e.g. MLResult.Code.Pending, MLResult.Code.Ok)
        /// </summary>
        /// <param name="result">The code to determine if it is Ok.</param>
        /// <returns>True if the provided code is equivalent Code.Ok.</returns>
        public static bool IsOK(Code result)
        {
            return Code.Ok == result;
        }

        /// <summary>
        /// Indicates whether the result code is one of the Pending results.
        /// </summary>
        /// <param name="result">The code to determine if it is pending.</param>
        /// <returns>True if the provided code is equivalent Code.Pending.</returns>
        public static bool IsPending(Code result)
        {
            return (Code.Pending == result);
        }

        public static bool DidNativeCallSucceed(Code resultCode, string functionName = "A native function", Predicate<Code> successCase = null, bool showError = true)
        {
            bool success = successCase?.Invoke(resultCode) ?? IsOK(resultCode);

            if (!success && showError)
                MLPluginLog.ErrorFormat($"{functionName} in the Magic Leap API failed. Reason: {CodeToString(resultCode)} ");
            return success;
        }

        /// <summary>
        /// The equality check to be used for comparing two MLResult structs.
        /// </summary>
        /// <param name="one">The first struct to compare with the second struct. </param>
        /// <param name="two">The second struct to compare with the first struct. </param>
        /// <returns>True if the two provided structs have the same Result value.</returns>
        public static bool operator ==(MLResult one, MLResult.Code two)
        {
            return one.Result == two;
        }

        /// <summary>
        /// The inequality check to be used for comparing two MLResult structs.
        /// </summary>
        /// <param name="one">The first struct to compare with the second struct. </param>
        /// <param name="two">The second struct to compare with the first struct. </param>
        /// <returns>True if the two provided structs do not have the same Result value.</returns>
        public static bool operator !=(MLResult one, MLResult.Code two)
        {
            return !(one.Result == two);
        }

        /// <summary>
        /// The equality check to be used for comparing another object to this one.
        /// </summary>
        /// <param name="obj">The object to compare to this one with. </param>
        /// <returns>True if the the provided object is of the MLResult type and has the same Result values.</returns>
        public override bool Equals(object obj)
        {
            if (obj is MLResult.Code)
            {
                return this.Result == (MLResult.Code)obj;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the hash code to use from this.Result.
        /// </summary>
        /// <returns>The hash code returned by the this.Result field.</returns>
        public override int GetHashCode()
        {
            return this.Result.GetHashCode();
        }

        /// <summary>
        /// Provides the string value of this.Result or the default message given to this MLResult.
        /// </summary>
        /// <returns>the string value of this.Result or the default message given to this MLResult.</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.message))
            {
                this.message = CodeToString(this.Result);
            }

            return this.message;
        }
    }
}
