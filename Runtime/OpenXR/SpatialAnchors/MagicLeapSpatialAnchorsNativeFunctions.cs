using MagicLeap.OpenXR.Futures;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Features.SpatialAnchors
{
    internal unsafe class MagicLeapSpatialAnchorsNativeFunctions : FuturesNativeFunctions
    {
        internal delegate* unmanaged[Cdecl]<ulong, XrSpatialAnchorsCreateInfoBaseHeader*, out ulong, XrResult> XrCreateSpatialAnchorsAsync;
        internal delegate* unmanaged[Cdecl]<ulong, ulong, out XrCreateSpatialAnchorsCompletion, XrResult> XrCreateSpatialAnchorsComplete;
        internal delegate* unmanaged[Cdecl]<ulong, out XrSpatialAnchorState, XrResult> XrGetSpatialAnchorState;
        private string SanitizeName(string name) => $"{name.Replace("Xr", "xr")}ML";

        protected override void LocateNativeFunctions()
        {
            base.LocateNativeFunctions();
            XrCreateSpatialAnchorsAsync = (delegate* unmanaged[Cdecl]<ulong, XrSpatialAnchorsCreateInfoBaseHeader*, out ulong, XrResult>)LocateNativeFunction(SanitizeName(nameof(XrCreateSpatialAnchorsAsync)));
            XrCreateSpatialAnchorsComplete = (delegate* unmanaged[Cdecl]<ulong, ulong, out XrCreateSpatialAnchorsCompletion, XrResult>)LocateNativeFunction(SanitizeName(nameof(XrCreateSpatialAnchorsComplete)));
            XrGetSpatialAnchorState = (delegate* unmanaged[Cdecl]<ulong, out XrSpatialAnchorState, XrResult>)LocateNativeFunction(SanitizeName(nameof(XrGetSpatialAnchorState)));
        }
    }
}
