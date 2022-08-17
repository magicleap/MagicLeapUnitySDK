// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLRaycast.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{

#if UNITY_MAGICLEAP || UNITY_ANDROID
    using UnityEngine.XR.MagicLeap.Native;
#endif
    /// <summary>
    /// Sends requests to create Rays intersecting world geometry and returns results through callbacks.
    /// </summary>
    public partial class MLRaycast : MLAutoAPISingleton<MLRaycast>
    {
#if UNITY_MAGICLEAP || UNITY_ANDROID
        /// <summary>
        /// Stores the ray cast system tracker.
        /// </summary>
        private ulong trackerHandle = MagicLeapNativeBindings.InvalidHandle;
#endif

#if UNITY_MAGICLEAP || UNITY_ANDROID
        /// <summary>
        /// Create the ray cast system. This function must be called with the the required settings prior to <c>MLRaycastRequest()</c>.
        /// </summary>
        /// <param name="trackerHandle">Handle to the created ray cast system. Only valid if the return value is MLResult_Ok.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to invalid input parameter.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        private MLResult.Code Create(ref ulong trackerHandle)
        {
            nativeMLRaycastCreatePerfMarker.Begin();
            MLResult.Code resultCode = NativeBindings.MLRaycastCreate(ref trackerHandle);
            nativeMLRaycastCreatePerfMarker.End();
            return resultCode;
        }

        /// <summary>
        /// Begin a query to a ray cast.
        /// </summary>
        /// <param name="request">Query parameters for the ray cast.</param>
        /// <param name="queryHandle">A handle to an ongoing request.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to invalid input parameter.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        private MLResult.Code RequestInternal(Request.Params requestParams, ref ulong queryHandle)
        {
            nativeMLRaycastRequestPerfMarker.Begin();
            NativeBindings.MLRaycastQueryNative request = new NativeBindings.MLRaycastQueryNative(requestParams);
            MLResult.Code resultCode = NativeBindings.MLRaycastRequest(this.trackerHandle, ref request, ref queryHandle);
            MLResult.DidNativeCallSucceed(resultCode, "MLRaycastRequest");
            nativeMLRaycastRequestPerfMarker.End();
            return resultCode;
        }

        /// <summary>
        /// Get the result of a call to <c>MLRaycastRequest()</c>.
        /// After this function has returned successfully, the handle is invalid.
        /// </summary>
        /// <param name="trackerHandle">Handle to the tracker created by <c>MLRaycastRequest()</c>.</param>
        /// <param name="raycastRequest">A handle to the ray cast request.</param>
        /// <param name="result">The target to populate the result.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to invalid input parameter.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.Pending</c> if request has not completed. This does not indicate a failure.
        /// </returns>
        private MLResult.Code GetResult(ulong trackerHandle, ulong raycastRequest, out NativeBindings.MLRaycastResultNative result)
        {
            nativeMLRaycastGetResultPerfMarker.Begin();
            MLResult.Code resultCode = NativeBindings.MLRaycastGetResult(trackerHandle, raycastRequest, out result);
            nativeMLRaycastGetResultPerfMarker.End();
            return resultCode;
        }


        /// <summary>
        /// Destroy a ray cast tracker.
        /// </summary>
        /// <param name="trackerHandle">Handle to the tracker created by <c>MLRaycastRequest()</c>.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        private MLResult.Code Destroy(ulong trackerHandle)
        {
            nativeMLRaycastDestroyPerfMarker.Begin();
            MLResult.Code resultCode = NativeBindings.MLRaycastDestroy(trackerHandle);
            nativeMLRaycastDestroyPerfMarker.End();
            return resultCode;
        }

#if !DOXYGEN_SHOULD_SKIP_THIS
        /// <summary>
        /// Creates a new ray cast tracker.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to invalid input parameter.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        protected override MLResult.Code StartAPI()
        {
            startAPIPerfMarker.Begin();
            this.trackerHandle = MagicLeapNativeBindings.InvalidHandle;

            MLResult.Code resultCode = Create(ref this.trackerHandle);

            MLResult.DidNativeCallSucceed(resultCode, "MLRaycastCreate");

            startAPIPerfMarker.End();
            return resultCode;
        }

#endif // DOXYGEN_SHOULD_SKIP_THIS

        /// <summary>
        /// Cleans up memory and destroys the ray cast tracker.
        /// </summary>
        protected override MLResult.Code StopAPI()
        {
            stopAPIPerfMarker.Begin();
            //this.pendingQueries.Clear();
            this.DestroyNativeTracker();
            stopAPIPerfMarker.End();

            return MLResult.Code.Ok;
        }

        /// <summary>
        /// Destroys the native ray cast tracker.
        /// </summary>
        private void DestroyNativeTracker()
        {
            if (NativeBindings.MLHandleIsValid(this.trackerHandle))
            {
                MLResult.Code resultCode = Destroy(this.trackerHandle);
                if (resultCode != MLResult.Code.Ok)
                {
                    MLPluginLog.ErrorFormat("MLRaycast.DestroyNativeTracker failed to destroy raycast tracker. Reason: {0}", NativeBindings.MLGetResultString(resultCode));
                }

                this.trackerHandle = MagicLeapNativeBindings.InvalidHandle;
            }
        }
#endif
    }
}
