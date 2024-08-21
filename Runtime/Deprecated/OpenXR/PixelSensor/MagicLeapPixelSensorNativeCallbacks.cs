using System;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapPixelSensorFeature
    {
        internal unsafe class PixelSensorNativeFunctions : MagicLeapFutureNativeFunctions
        {
            internal delegate* unmanaged [Cdecl] <ulong, ref XrPixelSensorConfigInfo, ref ulong, XrResult> XrConfigurePixelSensorAsync;
            internal delegate* unmanaged [Cdecl] <ulong, ulong, out XrPixelSensorConfigureCompletion, XrResult> XrConfigurePixelSensorComplete;
            internal delegate* unmanaged [Cdecl] <ulong, ref XrPixelSensorCreateInfo, out ulong, XrResult> XrCreatePixelSensor;
            internal delegate* unmanaged [Cdecl] <ulong, ref XrPixelSensorCreateSpaceInfo, out ulong, XrResult> XrCreatePixelSensorSpace;
            internal delegate* unmanaged [Cdecl] <ulong, XrResult> XrDestroyPixelSensor;
            internal delegate* unmanaged [Cdecl] <ulong, uint, uint, out uint, XrPixelSensorCapability*, XrResult> XrEnumeratePixelSensorCapabilities;
            internal delegate* unmanaged [Cdecl] <ulong, uint, uint, out uint, PixelSensorMetaDataType*, XrResult> XrEnumeratePixelSensorMetadata;
            internal delegate* unmanaged [Cdecl] <ulong, uint, out uint, ulong*, XrResult> XrEnumeratePixelSensors;
            internal delegate* unmanaged [Cdecl] <ulong, ref XrPixelSensorBufferPropertiesInfo, out XrPixelSensorBufferProperties, XrResult> XrGetPixelSensorBufferProperties;
            internal delegate* unmanaged [Cdecl] <ulong, ref XrPixelSensorDataGetInfo, out XrPixelSensorBuffer, out XrPixelSensorData, XrResult> XrGetPixelSensorData;
            internal delegate* unmanaged [Cdecl] <ulong, out uint, XrResult> XrGetPixelSensorStreamCount;
            internal delegate* unmanaged [Cdecl] <ulong, ref XrPixelSensorCapabilityQueryInfo, uint, IntPtr*, IntPtr, XrResult> XrQueryPixelSensorCapabilities;
            internal delegate* unmanaged [Cdecl] <ulong, ref XrPixelSensorStartInfo, out ulong, XrResult> XrStartPixelSensorAsync;
            internal delegate* unmanaged [Cdecl] <ulong, ulong, out XrPixelSensorStartCompletion, XrResult> XrStartPixelSensorComplete;
            internal delegate* unmanaged [Cdecl] <ulong, ref XrPixelSensorStopInfo, out ulong, XrResult> XrStopPixelSensorAsync;
            internal delegate* unmanaged [Cdecl] <ulong, ulong, out XrPixelSensorStopCompletion, XrResult> XrStopPixelSensorComplete;
            
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

                if (XrQueryPixelSensorCapabilities == null)
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
                XrEnumeratePixelSensors = (delegate* unmanaged[Cdecl]<ulong, uint, out uint, ulong*, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrEnumeratePixelSensors)));
                XrCreatePixelSensor = (delegate* unmanaged[Cdecl]<ulong, ref XrPixelSensorCreateInfo, out ulong, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrCreatePixelSensor)));
                XrDestroyPixelSensor = (delegate* unmanaged[Cdecl]<ulong, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrDestroyPixelSensor)));
                XrGetPixelSensorStreamCount = (delegate* unmanaged[Cdecl]<ulong, out uint, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetPixelSensorStreamCount)));
                XrEnumeratePixelSensorCapabilities = (delegate* unmanaged[Cdecl]<ulong, uint, uint, out uint, XrPixelSensorCapability*, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrEnumeratePixelSensorCapabilities)));
                XrQueryPixelSensorCapabilities = (delegate* unmanaged[Cdecl]<ulong, ref XrPixelSensorCapabilityQueryInfo, uint, IntPtr*, IntPtr, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrQueryPixelSensorCapabilities)));
                XrConfigurePixelSensorAsync = (delegate* unmanaged[Cdecl]<ulong, ref XrPixelSensorConfigInfo, ref ulong, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrConfigurePixelSensorAsync)));
                XrConfigurePixelSensorComplete = (delegate* unmanaged[Cdecl]<ulong, ulong, out XrPixelSensorConfigureCompletion, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrConfigurePixelSensorComplete)));
                XrEnumeratePixelSensorMetadata = (delegate* unmanaged[Cdecl]<ulong, uint, uint, out uint, PixelSensorMetaDataType*, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrEnumeratePixelSensorMetadata)));
                XrStartPixelSensorAsync = (delegate* unmanaged[Cdecl]<ulong, ref XrPixelSensorStartInfo, out ulong, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrStartPixelSensorAsync)));
                XrStartPixelSensorComplete = (delegate* unmanaged[Cdecl]<ulong, ulong, out XrPixelSensorStartCompletion, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrStartPixelSensorComplete)));
                XrStopPixelSensorAsync = (delegate* unmanaged[Cdecl]<ulong, ref XrPixelSensorStopInfo, out ulong, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrStopPixelSensorAsync)));
                XrStopPixelSensorComplete = (delegate* unmanaged[Cdecl]<ulong, ulong, out XrPixelSensorStopCompletion, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrStopPixelSensorComplete)));
                XrGetPixelSensorBufferProperties = (delegate* unmanaged[Cdecl]<ulong, ref XrPixelSensorBufferPropertiesInfo, out XrPixelSensorBufferProperties, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetPixelSensorBufferProperties)));
                XrGetPixelSensorData = (delegate* unmanaged[Cdecl]<ulong, ref XrPixelSensorDataGetInfo, out XrPixelSensorBuffer, out XrPixelSensorData, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetPixelSensorData)));
                XrCreatePixelSensorSpace = (delegate* unmanaged[Cdecl]<ulong, ref XrPixelSensorCreateSpaceInfo, out ulong, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrCreatePixelSensorSpace)));
            }
        }
    }
}
