﻿using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    using static MagicLeapPlanesNativeTypes;
    internal unsafe class MagicLeapPlanesNativeFunctions : MagicLeapNativeFunctionsBase
    {
        internal delegate* unmanaged [Cdecl] <ulong, in XrPlaneDetectorCreateInfo, out ulong, XrResult> XrCreatePlaneDetector;
        internal delegate* unmanaged [Cdecl] <ulong, XrResult> XrDestroyPlaneDetector;
        internal delegate* unmanaged [Cdecl] <ulong, in XrPlaneDetectorBeginInfo, XrResult> XrBeginPlaneDetection;
        internal delegate* unmanaged [Cdecl] <ulong, out XrPlaneDetectionState, XrResult> XrGetPlaneDetectionState;
        internal delegate* unmanaged [Cdecl] <ulong, in XrPlaneDetectorGetInfo, out XrPlaneDetectorLocations, XrResult> XrGetPlaneDetections;
        internal delegate* unmanaged [Cdecl] <ulong, ulong, uint, out XrPlaneDetectorPolygonBuffer, XrResult> XrGetPlanePolygonBuffer;
        
        private static string SanitizeFunctionName(string input) => $"{input.Replace("Xr", "xr")}EXT";
        
        protected override void LocateNativeFunctions()
        {
            XrCreatePlaneDetector = (delegate* unmanaged[Cdecl]<ulong, in XrPlaneDetectorCreateInfo, out ulong, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrCreatePlaneDetector)));
            XrDestroyPlaneDetector = (delegate* unmanaged[Cdecl]<ulong, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrDestroyPlaneDetector)));
            XrBeginPlaneDetection = (delegate* unmanaged[Cdecl]<ulong, in XrPlaneDetectorBeginInfo, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrBeginPlaneDetection)));
            XrGetPlaneDetectionState = (delegate* unmanaged[Cdecl]<ulong, out XrPlaneDetectionState, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetPlaneDetectionState)));
            XrGetPlaneDetections = (delegate* unmanaged[Cdecl]<ulong, in XrPlaneDetectorGetInfo, out XrPlaneDetectorLocations, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetPlaneDetections)));
            XrGetPlanePolygonBuffer = (delegate* unmanaged[Cdecl]<ulong, ulong, uint, out XrPlaneDetectorPolygonBuffer, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetPlanePolygonBuffer)));
        }

        protected override void Validate()
        {
            base.Validate();
            if (XrCreatePlaneDetector == null)
            {
                Debug.LogError($"{nameof(XrCreatePlaneDetector)} is null");
            }

            if (XrDestroyPlaneDetector == null)
            {
                Debug.LogError($"{nameof(XrDestroyPlaneDetector)} is null");
            }

            if (XrBeginPlaneDetection == null)
            {
                Debug.LogError($"{nameof(XrBeginPlaneDetection)} is null");
            }

            if (XrGetPlaneDetectionState == null)
            {
                Debug.LogError($"{nameof(XrGetPlaneDetectionState)} is null");
            }

            if (XrGetPlaneDetections == null)
            {
                Debug.LogError($"{nameof(XrGetPlaneDetections)} is null");
            }

            if (XrGetPlanePolygonBuffer == null)
            {
                Debug.LogError($"{nameof(XrGetPlanePolygonBuffer)} is null");
            }
        }
    }
}
