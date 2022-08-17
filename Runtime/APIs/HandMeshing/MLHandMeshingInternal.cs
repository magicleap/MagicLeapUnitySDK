// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLHandMeshing.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if UNITY_MAGICLEAP || UNITY_ANDROID

namespace UnityEngine.XR.MagicLeap
{
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// The MLHandMeshing API is used to request for the mesh information of the hands.
    /// </summary>
    public sealed partial class MLHandMeshing : MLAutoAPISingleton<MLHandMeshing>
    {
        /// <summary>
        /// Handle to the native Hand Meshing tracker.
        /// </summary>
        private ulong nativeTracker = MagicLeapNativeBindings.InvalidHandle;

        /// <summary>
        /// Requests the hand mesh.
        /// </summary>
        /// <param name="handle">Handle to the created Hand Meshing client.</param>
        /// <param name="requestHandle">Handle to the current query request.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if there was an invalid handle.
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the hand mesh was requested successfully.
        /// </returns>
        private MLResult.Code RequestMesh(ulong tracker, ref ulong requestHandle)
        {
            nativeMLHandMeshingRequestMeshPerfMarker.Begin();
            MLResult.Code result = NativeBindings.MLHandMeshingRequestMesh(tracker, ref requestHandle);
            nativeMLHandMeshingRequestMeshPerfMarker.End();
            return result;
        }

        /// <summary>
        /// Gets the Result of a previous hand mesh request
        /// </summary>
        /// <param name="handle">Handle to the created Hand Meshing client.</param>
        /// <param name="requestHandle">Handle received from a previous MLHandMeshingRequestMesh call.</param>
        /// <param name="mesh">The mesh object which will be populated only if the result is successful.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if there was an invalid parameter.
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the mesh object was populated successfully.
        /// MLResult.Result will be <c>MLResult.Code.Pending</c> if the mesh result is pending a update.
        /// </returns>
        private MLResult.Code GetResult(ulong handle, ulong requestHandle, ref NativeBindings.MeshNative mesh)
        {
            nativeMLHandMeshingGetResultPerfMarker.Begin();
            MLResult.Code result = NativeBindings.MLHandMeshingGetResult(handle, requestHandle, ref mesh);
            nativeMLHandMeshingGetResultPerfMarker.End();
            return result;
        }

        /// <summary>
        /// Free the resources created by the hand meshing API. Needs to be called whenever MLHandMeshingGetResult returns a success.
        /// </summary>
        /// <param name="handle">Handle to the created Hand Meshing client.</param>
        /// <param name="requestHandle">Handle received from a previous MLHandMeshingRequestMesh call.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if there was an invalid handle.
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the resources were freed successfully.
        /// </returns>
        private MLResult.Code FreeResource(ulong tracker, ref ulong requestHandle)
        {
            nativeMLHandMeshingFreeResourcePerfMarker.Begin();
            MLResult.Code result = NativeBindings.MLHandMeshingFreeResource(tracker, ref requestHandle);
            nativeMLHandMeshingFreeResourcePerfMarker.End();
            return result;
        }

#if !DOXYGENSHOULDSKIPTHIS
        /// <summary>
        /// Initializes the HandMeshing API.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        protected override MLResult.Code StartAPI()
        {
            startAPIPerfMarker.Begin();
            MLResult.Code resultCode = this.InitNativeTracker();
            MLResult.DidNativeCallSucceed(resultCode, "StartAPI()");

            startAPIPerfMarker.End();
            return resultCode;
        }
#endif // DOXYGENSHOULDSKIPTHIS

        /// <summary>
        /// Destroys the native Hand Meshing client.
        /// </summary>
        private void DestroyNativeTracker()
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(this.nativeTracker))
            {
                return;
            }

            nativeMLHandMeshingDestroyClientPerfMarker.Begin();
            MLResult.Code result = NativeBindings.MLHandMeshingDestroyClient(ref this.nativeTracker);
            nativeMLHandMeshingDestroyClientPerfMarker.End();

            MLResult.DidNativeCallSucceed(result, "DestroyNativeTracker()");

            this.nativeTracker = MagicLeapNativeBindings.InvalidHandle;
        }

        /// <summary>
        /// Initializes the native Hand Meshing client.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if there was an invalid handle.
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the Hand Meshing client was created successfully.
        /// MLResult.Result will be <c>MLResult.Code.PermissionDenied</c> > if there was a lack of permission.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if there was an internal error.
        /// </returns>
        private MLResult.Code InitNativeTracker()
        {
            this.nativeTracker = MagicLeapNativeBindings.InvalidHandle;
            nativeMLHandMeshingCreateClientPerfMarker.Begin();
            MLResult.Code code = NativeBindings.MLHandMeshingCreateClient(ref this.nativeTracker);
            nativeMLHandMeshingCreateClientPerfMarker.End();
            return code;
        }

        /// <summary>
        /// Cleans up unmanaged memory. Frees up resources of pending queries.
        /// </summary>
        protected override MLResult.Code StopAPI()
        {
            var result = MLResult.Code.Ok;
            cleanupAPIPerfMarker.Begin();
            this.DestroyNativeTracker();
            cleanupAPIPerfMarker.End();
            return result;
        }
    }
}

#endif
