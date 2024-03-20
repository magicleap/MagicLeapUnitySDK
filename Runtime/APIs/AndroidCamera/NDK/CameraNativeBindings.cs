// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace MagicLeap.Android.NDK.Camera
{
    using System.Runtime.InteropServices;
    using NativeWindow;

    using static CameraConstants;

    internal static class CameraNativeBindings
    {
        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe CameraStatus ACameraCaptureSession_capture(ACameraCaptureSession session,
            ACameraCaptureSession.CaptureCallbacks* callbacks, int numRequests, ACaptureRequest* requests,
            out int outCaptureSequenceId);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ACameraCaptureSession_close(ACameraCaptureSession session);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern CameraStatus ACameraCaptureSession_getDevice(ACameraCaptureSession session,
            out ACameraDevice outDevice);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe CameraStatus ACameraCaptureSession_setRepeatingRequest(ACameraCaptureSession session,
            ACameraCaptureSession.CaptureCallbacks* callbacks, int numRequests, ACaptureRequest* requests,
            out int outSequenceId);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern CameraStatus ACameraCaptureSession_stopRepeating(ACameraCaptureSession session);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern CameraStatus ACameraDevice_close(ACameraDevice camera);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern CameraStatus ACameraDevice_createCaptureRequest(ACameraDevice device,
            ACaptureRequest.Template template, out ACaptureRequest outRequest);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe CameraStatus ACameraDevice_createCaptureSession(ACameraDevice device,
            ACaptureSessionOutputContainer container, ACameraCaptureSession.StateCallbacks* callbacks,
            out ACameraCaptureSession session);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* ACameraDevice_getId(ACameraDevice camera);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern ACameraManager ACameraManager_create();

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ACameraManager_delete(ACameraManager manager);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ACameraManager_deleteCameraIdList(ACameraIdList list);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern CameraStatus ACameraManager_getCameraCharacteristics(ACameraManager manager,
            string cameraId, out ACameraMetadata metadata);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "ACameraManager_getCameraCharacteristics")]
        public static extern unsafe CameraStatus ACameraManager_getCameraCharacteristicsNonAlloc(ACameraManager manager,
            byte* cameraId, out ACameraMetadata metadata);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern CameraStatus ACameraManager_getCameraIdList(ACameraManager manager, out ACameraIdList list);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern unsafe CameraStatus ACameraManager_openCamera(ACameraManager manager, string cameraId,
            ACameraDevice.StateCallbacks* callbacks, out ACameraDevice camera);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ACameraManager_openCamera")]
        public static extern unsafe CameraStatus ACameraManager_openCameraNonAlloc(ACameraManager manager, byte* cameraId,
            ACameraDevice.StateCallbacks* callbacks, out ACameraDevice camera);

        // NB:
        // The following two signatures use 'ref' for the callback parameter instead of a pointer. The reasoning behind
        // this is that in other signatures, the callback parameter is optional, thus a bare pointer is required.
        // In this case, however, the call itself is rather nonsensical without a valid callbacks pointer, so we can
        // somewhat enforce that at the C# level by using a 'ref' param (which translates to a pointer on the ABI level anyways).
        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern CameraStatus ACameraManager_registerAvailabilityCallback(ACameraManager manager,
            ref ACameraManager.AvailabilityCallbacks callbacks);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern CameraStatus ACameraManager_unregisterAvailabilityCallback(ACameraManager manager,
            ref ACameraManager.AvailabilityCallbacks callbacks);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern CameraStatus ACameraMetadata_getConstEntry(ACameraMetadata metadata, uint tag,
            out ACameraMetadata.Entry.ReadOnly outEntry);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe CameraStatus ACameraMetadata_getAllTags(ACameraMetadata metadata, out int outNumEntries,
            out uint* outTags);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern ACameraMetadata ACameraMetadata_copy(ACameraMetadata metadata);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ACameraMetadata_free(ACameraMetadata metadata);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern CameraStatus ACameraOutputTarget_create(ANativeWindow window,
            out ACameraOutputTarget outTarget);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ACameraOutputTarget_free(ACameraOutputTarget target);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern CameraStatus ACaptureRequest_addTarget(ACaptureRequest request, ACameraOutputTarget target);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ACaptureRequest_free(ACaptureRequest request);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe CameraStatus ACaptureRequest_getAllTags(ACaptureRequest request, ref int numTags,
            uint** tags);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern CameraStatus ACaptureRequest_getConstEntry(ACaptureRequest request, uint tag,
            out ACameraMetadata.Entry.ReadOnly outEntry);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe CameraStatus ACaptureRequest_getUserContext(ACaptureRequest request,
            out System.IntPtr context);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern CameraStatus ACaptureRequest_removeTarget(ACaptureRequest request,
            ACameraOutputTarget target);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe CameraStatus ACaptureRequest_setEntry_u8(ACaptureRequest request, uint tag,
            uint count, byte* data);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe CameraStatus ACaptureRequest_setEntry_i32(ACaptureRequest request, uint tag,
            uint count, int* data);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe CameraStatus ACaptureRequest_setEntry_float(ACaptureRequest request, uint tag,
            uint count, float* data);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe CameraStatus ACaptureRequest_setEntry_i64(ACaptureRequest request, uint tag,
            uint count, long* data);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe CameraStatus ACaptureRequest_setEntry_double(ACaptureRequest request, uint tag,
            uint count, double* data);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe CameraStatus ACaptureRequest_setEntry_rational(ACaptureRequest request, uint tag,
            uint count, ACameraMetadata.Rational* data);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe CameraStatus ACaptureRequest_setUserContext(ACaptureRequest request,
            System.IntPtr context);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern CameraStatus ACaptureSessionOutput_create(ANativeWindow window,
            out ACaptureSessionOutput output);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ACaptureSessionOutput_free(ACaptureSessionOutput output);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern CameraStatus ACaptureSessionOutputContainer_add(
            ACaptureSessionOutputContainer container, ACaptureSessionOutput output);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern CameraStatus ACaptureSessionOutputContainer_create(
            out ACaptureSessionOutputContainer outContainer);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ACaptureSessionOutputContainer_free(ACaptureSessionOutputContainer container);

        [DllImport(kCameraLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern CameraStatus ACaptureSessionOutputContainer_remove(
            ACaptureSessionOutputContainer container, ACaptureSessionOutput output);
    }
}
