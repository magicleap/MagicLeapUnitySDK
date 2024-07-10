using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapLightEstimationFeature
    {
        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MagicLeapLightEstimationNativeFunctions")]
        internal unsafe class MagicLeapLightEstimationNativeFunctions : MagicLeapNativeFunctionsBase
        {
            internal delegate* unmanaged[Cdecl]<ulong, ref XrLightEstimationCreateInfo, out ulong, XrResult> XrCreateLightEstimation;
            internal delegate* unmanaged[Cdecl]<ulong, out ulong, XrResult> XrCreateLightEstimationEstimate;
            internal delegate* unmanaged[Cdecl]<ulong, ref XrLightEstimationState, XrResult> XrGetLightEstimationState;
            internal delegate* unmanaged[Cdecl]<ulong, out long, XrResult> XrGetLightEstimationTimestamp;
            internal delegate* unmanaged[Cdecl]<ulong, ref XrLightEstimationHDRCubemap, XrResult> XrGetLightEstimationHDRCubemap;
            internal delegate* unmanaged[Cdecl]<ulong, ref XrLightEstimationMainDirectionalLight, XrResult> XrGetLightEstimationMainDirectionalLight;
            internal delegate* unmanaged[Cdecl]<ulong, ref XrLightEstimationSphericalHarmonics, XrResult> XrGetLightEstimationSphericalHarmonics;
            internal delegate* unmanaged[Cdecl]<ulong, XrResult> XrDestroyLightEstimationEstimate;
            internal delegate* unmanaged[Cdecl]<ulong, XrResult> XrDestroyLightEstimation;

            protected override void Validate()
            {
                base.Validate();
                if (XrCreateLightEstimation == null)
                {
                    Debug.LogError("Unable to find XrCreateLightEstimation");
                }

                if (XrCreateLightEstimationEstimate == null)
                {
                    Debug.LogError("Unable to find XrCreateLightEstimationEstimate");
                }

                if (XrGetLightEstimationState == null)
                {
                    Debug.LogError("Unable to find XrGetLightEstimationState");
                }

                if (XrGetLightEstimationTimestamp == null)
                {
                    Debug.LogError("Unable to find XrGetLightEstimationTimestamp");
                }

                if (XrGetLightEstimationHDRCubemap == null)
                {
                    Debug.LogError("Unable to find XrGetLightEstimationHDRCubemap");
                }

                if (XrGetLightEstimationMainDirectionalLight == null)
                {
                    Debug.LogError("Unable to find XrGetLightEstimationMainDirectionalLight");
                }

                if (XrGetLightEstimationSphericalHarmonics == null)
                {
                    Debug.LogError("Unable to find XrGetLightEstimationSphericalHarmonics");
                }

                if (XrDestroyLightEstimationEstimate == null)
                {
                    Debug.LogError("Unable to find XrDestroyLightEstimationEstimate");
                }

                if (XrDestroyLightEstimation == null)
                {
                    Debug.LogError("Unable to find XrDestroyLightEstimation");
                }
            }

            private string SanitizeFunctionName(string input)
            {
                return $"{input.Replace("Xr", "xr")}ML";
            }

            protected override void LocateNativeFunctions()
            {
                XrCreateLightEstimation = (delegate* unmanaged[Cdecl]<ulong, ref XrLightEstimationCreateInfo, out ulong, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrCreateLightEstimation)));
                XrCreateLightEstimationEstimate = (delegate* unmanaged[Cdecl]<ulong, out ulong, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrCreateLightEstimationEstimate)));
                XrGetLightEstimationState = (delegate* unmanaged[Cdecl]<ulong, ref XrLightEstimationState, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetLightEstimationState)));
                XrGetLightEstimationTimestamp = (delegate* unmanaged[Cdecl]<ulong, out long, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetLightEstimationTimestamp)));
                XrGetLightEstimationHDRCubemap = (delegate* unmanaged[Cdecl]<ulong, ref XrLightEstimationHDRCubemap, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetLightEstimationHDRCubemap)));
                XrGetLightEstimationMainDirectionalLight = (delegate* unmanaged[Cdecl]<ulong, ref XrLightEstimationMainDirectionalLight, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetLightEstimationMainDirectionalLight)));
                XrGetLightEstimationSphericalHarmonics = (delegate* unmanaged[Cdecl]<ulong, ref XrLightEstimationSphericalHarmonics, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetLightEstimationSphericalHarmonics)));
                XrDestroyLightEstimationEstimate = (delegate* unmanaged[Cdecl]<ulong, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrDestroyLightEstimationEstimate)));
                XrDestroyLightEstimation = (delegate* unmanaged[Cdecl]<ulong, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrDestroyLightEstimation)));
            }
        }
    }
}
