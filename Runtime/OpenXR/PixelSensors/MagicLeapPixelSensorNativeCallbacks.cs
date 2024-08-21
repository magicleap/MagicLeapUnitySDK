using System;
using MagicLeap.OpenXR.Futures;
using UnityEngine;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Features.PixelSensors
{
    internal unsafe class PixelSensorNativeFunctions : FuturesNativeFunctions
    {
        internal delegate* unmanaged [Cdecl] <XrSession, uint, out uint, XrPath*, XrResult> XrEnumeratePixelSensors;
        internal delegate* unmanaged [Cdecl] <XrSession, ref XrPixelSensorCreateInfo, out XrPixelSensor, XrResult> XrCreatePixelSensor;
        internal delegate* unmanaged [Cdecl] <XrPixelSensor, XrResult> XrDestroyPixelSensor;
        internal delegate* unmanaged [Cdecl] <XrPixelSensor, out uint, XrResult> XrGetPixelSensorStreamCount;
        internal delegate* unmanaged [Cdecl] <XrPixelSensor, uint, uint, out uint, XrPixelSensorCapability*, XrResult> XrEnumeratePixelSensorCapabilities;
        internal delegate* unmanaged [Cdecl] <XrPixelSensor, ref XrPixelSensorCapabilityQueryInfo, uint, IntPtr*, IntPtr, XrResult> XrQueryPixelSensorCapabilityRange;
        internal delegate* unmanaged [Cdecl] <XrPixelSensor, ref XrPixelSensorConfigInfo, out XrFuture, XrResult> XrConfigurePixelSensorAsync;
        internal delegate* unmanaged [Cdecl] <XrPixelSensor, XrFuture, out XrPixelSensorConfigureCompletion, XrResult> XrConfigurePixelSensorComplete;
        internal delegate* unmanaged [Cdecl] <XrPixelSensor, uint, uint, out uint, PixelSensorMetaDataType*, XrResult> XrEnumeratePixelSensorMetadata;
        internal delegate* unmanaged [Cdecl] <XrPixelSensor, ref XrPixelSensorStartInfo, out XrFuture, XrResult> XrStartPixelSensorAsync;
        internal delegate* unmanaged [Cdecl] <XrPixelSensor, XrFuture, out XrPixelSensorStartCompletion, XrResult> XrStartPixelSensorComplete;
        internal delegate* unmanaged [Cdecl] <XrPixelSensor, ref XrPixelSensorStopInfo, out XrFuture, XrResult> XrStopPixelSensorAsync;
        internal delegate* unmanaged [Cdecl] <XrPixelSensor, XrFuture, out XrPixelSensorStopCompletion, XrResult> XrStopPixelSensorComplete;
        internal delegate* unmanaged [Cdecl] <XrPixelSensor, ref XrPixelSensorBufferPropertiesInfo, out XrPixelSensorBufferProperties, XrResult> XrGetPixelSensorBufferProperties;
        internal delegate* unmanaged [Cdecl] <XrPixelSensor, ref XrPixelSensorDataGetInfo, out XrPixelSensorBuffer, out XrPixelSensorData, XrResult> XrGetPixelSensorData;
        internal delegate* unmanaged [Cdecl] <XrSession, ref XrPixelSensorCreateSpaceInfo, out XrSpace, XrResult> XrCreatePixelSensorSpace;

        protected override void Validate()
        {
            base.Validate();
            if (XrEnumeratePixelSensors == null)
            {
                Debug.LogError("Unable to find XrEnumeratePixelSensors");
            }

            if (XrCreatePixelSensor == null)
            {
                Debug.LogError("Unable to find XrCreatePixelSensor");
            }

            if (XrDestroyPixelSensor == null)
            {
                Debug.LogError("Unable to find XrDestroyPixelSensor");
            }

            if (XrGetPixelSensorStreamCount == null)
            {
                Debug.LogError("Unable to find XrGetPixelSensorStreamCount");
            }

            if (XrEnumeratePixelSensorCapabilities == null)
            {
                Debug.LogError("Unable to find XrEnumeratePixelSensorCapabilities");
            }

            if (XrQueryPixelSensorCapabilityRange == null)
            {
                Debug.LogError("Unable to find XrQueryPixelSensorCapabilityRange");
            }

            if (XrConfigurePixelSensorAsync == null)
            {
                Debug.LogError("Unable to find XrConfigurePixelSensorAsync");
            }

            if (XrConfigurePixelSensorComplete == null)
            {
                Debug.LogError("Unable to find XrConfigurePixelSensorComplete");
            }

            if (XrEnumeratePixelSensorMetadata == null)
            {
                Debug.LogError("Unable to find XrEnumeratePixelSensorMetaData");
            }

            if (XrStartPixelSensorAsync == null)
            {
                Debug.LogError("Unable to find XrStartPixelSensorAsync");
            }

            if (XrStartPixelSensorComplete == null)
            {
                Debug.LogError("Unable to find XrStartPixelSensorComplete");
            }

            if (XrStopPixelSensorAsync == null)
            {
                Debug.LogError("Unable to find XrStopPixelSensorAsync");
            }

            if (XrStopPixelSensorComplete == null)
            {
                Debug.LogError("Unable to find XrStopPixelSensorComplete");
            }

            if (XrGetPixelSensorBufferProperties == null)
            {
                Debug.LogError("Unable to find XrGetPixelSensorBufferProperties");
            }

            if (XrGetPixelSensorData == null)
            {
                Debug.LogError("Unable to find XrGetPixelSensorData");
            }

            if (XrCreatePixelSensorSpace == null)
            {
                Debug.LogError("Unable to find XrCreatePixelSensorSpace");
            }
        }

        private string SanitizeFunctionName(string input)
        {
            return $"{input.Replace("Xr", "xr")}ML";
        }

        protected override void LocateNativeFunctions()
        {
            base.LocateNativeFunctions();
            XrEnumeratePixelSensors = (delegate* unmanaged[Cdecl]<XrSession, uint, out uint, XrPath*, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrEnumeratePixelSensors)));
            XrCreatePixelSensor = (delegate* unmanaged[Cdecl]<XrSession, ref XrPixelSensorCreateInfo, out XrPixelSensor, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrCreatePixelSensor)));
            XrDestroyPixelSensor = (delegate* unmanaged[Cdecl]<XrPixelSensor, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrDestroyPixelSensor)));
            XrGetPixelSensorStreamCount = (delegate* unmanaged[Cdecl]<XrPixelSensor, out uint, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetPixelSensorStreamCount)));
            XrEnumeratePixelSensorCapabilities = (delegate* unmanaged[Cdecl]<XrPixelSensor, uint, uint, out uint, XrPixelSensorCapability*, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrEnumeratePixelSensorCapabilities)));
            XrQueryPixelSensorCapabilityRange = (delegate* unmanaged[Cdecl]<XrPixelSensor, ref XrPixelSensorCapabilityQueryInfo, uint, IntPtr*, IntPtr, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrQueryPixelSensorCapabilityRange)));
            XrConfigurePixelSensorAsync = (delegate* unmanaged[Cdecl]<XrPixelSensor, ref XrPixelSensorConfigInfo, out XrFuture, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrConfigurePixelSensorAsync)));
            XrConfigurePixelSensorComplete = (delegate* unmanaged[Cdecl]<XrPixelSensor, XrFuture, out XrPixelSensorConfigureCompletion, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrConfigurePixelSensorComplete)));
            XrEnumeratePixelSensorMetadata = (delegate* unmanaged[Cdecl]<XrPixelSensor, uint, uint, out uint, PixelSensorMetaDataType*, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrEnumeratePixelSensorMetadata)));
            XrStartPixelSensorAsync = (delegate* unmanaged[Cdecl]<XrPixelSensor, ref XrPixelSensorStartInfo, out XrFuture, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrStartPixelSensorAsync)));
            XrStartPixelSensorComplete = (delegate* unmanaged[Cdecl]<XrPixelSensor, XrFuture, out XrPixelSensorStartCompletion, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrStartPixelSensorComplete)));
            XrStopPixelSensorAsync = (delegate* unmanaged[Cdecl]<XrPixelSensor, ref XrPixelSensorStopInfo, out XrFuture, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrStopPixelSensorAsync)));
            XrStopPixelSensorComplete = (delegate* unmanaged[Cdecl]<XrPixelSensor, XrFuture, out XrPixelSensorStopCompletion, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrStopPixelSensorComplete)));
            XrGetPixelSensorBufferProperties = (delegate* unmanaged[Cdecl]<XrPixelSensor, ref XrPixelSensorBufferPropertiesInfo, out XrPixelSensorBufferProperties, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetPixelSensorBufferProperties)));
            XrGetPixelSensorData = (delegate* unmanaged[Cdecl]<XrPixelSensor, ref XrPixelSensorDataGetInfo, out XrPixelSensorBuffer, out XrPixelSensorData, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetPixelSensorData)));
            XrCreatePixelSensorSpace = (delegate* unmanaged[Cdecl]<XrSession, ref XrPixelSensorCreateSpaceInfo, out XrSpace, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrCreatePixelSensorSpace)));
        }
    }
}
