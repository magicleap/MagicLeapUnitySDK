using MagicLeap.OpenXR.Futures;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Features.SpatialAnchors
{
    internal unsafe class MagicLeapSpatialAnchorsStorageNativeFunctions : FuturesNativeFunctions
    {
        internal delegate* unmanaged[Cdecl]<ulong, XrSpatialAnchorsCreateStorageInfo*, out ulong, XrResult> XrCreateSpatialAnchorsStorage;
        internal delegate* unmanaged[Cdecl]<ulong, XrResult> XrDestroySpatialAnchorsStorage;
        internal delegate* unmanaged[Cdecl]<ulong, XrSpatialAnchorsQueryInfoBaseHeader*, out ulong, XrResult> XrQuerySpatialAnchorsAsync;
        internal delegate* unmanaged[Cdecl]<ulong, ulong, out XrSpatialAnchorsQueryCompletion, XrResult> XrQuerySpatialAnchorsComplete;
        internal delegate* unmanaged[Cdecl]<ulong, XrSpatialAnchorsPublishInfo*, out ulong, XrResult> XrPublishSpatialAnchorsAsync;
        internal delegate* unmanaged[Cdecl]<ulong, ulong, out XrSpatialAnchorsPublishCompletion, XrResult> XrPublishSpatialAnchorsComplete;
        internal delegate* unmanaged[Cdecl]<ulong, XrSpatialAnchorsDeleteInfo*, out ulong, XrResult> XrDeleteSpatialAnchorsAsync;
        internal delegate* unmanaged[Cdecl]<ulong, ulong, out XrSpatialAnchorsDeleteCompletion, XrResult> XrDeleteSpatialAnchorsComplete;
        internal delegate* unmanaged[Cdecl]<ulong, XrSpatialAnchorsUpdateExpirationInfo*, out ulong, XrResult> XrUpdateSpatialAnchorsExpirationAsync;
        internal delegate* unmanaged[Cdecl]<ulong, ulong, out XrSpatialAnchorsUpdateExpirationCompletion, XrResult> XrUpdateSpatialAnchorsExpirationComplete;
        private string SanitizeName(string name) => $"{name.Replace("Xr", "xr")}ML";

        protected override void LocateNativeFunctions()
        {
            base.LocateNativeFunctions();
            XrCreateSpatialAnchorsStorage = (delegate* unmanaged[Cdecl]<ulong, XrSpatialAnchorsCreateStorageInfo*, out ulong, XrResult>)LocateNativeFunction(SanitizeName(nameof(XrCreateSpatialAnchorsStorage)));
            XrDestroySpatialAnchorsStorage = (delegate* unmanaged[Cdecl]<ulong, XrResult>)LocateNativeFunction(SanitizeName(nameof(XrDestroySpatialAnchorsStorage)));
            XrQuerySpatialAnchorsAsync = (delegate* unmanaged[Cdecl]<ulong, XrSpatialAnchorsQueryInfoBaseHeader *, out ulong, XrResult>)LocateNativeFunction(SanitizeName(nameof(XrQuerySpatialAnchorsAsync)));
            XrQuerySpatialAnchorsComplete = (delegate* unmanaged[Cdecl]<ulong, ulong, out XrSpatialAnchorsQueryCompletion, XrResult>)LocateNativeFunction(SanitizeName(nameof(XrQuerySpatialAnchorsComplete)));
            XrPublishSpatialAnchorsAsync = (delegate* unmanaged[Cdecl]<ulong, XrSpatialAnchorsPublishInfo*, out ulong, XrResult>)LocateNativeFunction(SanitizeName(nameof(XrPublishSpatialAnchorsAsync)));
            XrPublishSpatialAnchorsComplete = (delegate* unmanaged[Cdecl]<ulong, ulong, out XrSpatialAnchorsPublishCompletion, XrResult>)LocateNativeFunction(SanitizeName(nameof(XrPublishSpatialAnchorsComplete)));
            XrDeleteSpatialAnchorsAsync = (delegate* unmanaged[Cdecl]<ulong, XrSpatialAnchorsDeleteInfo*, out ulong, XrResult>)LocateNativeFunction(SanitizeName(nameof(XrDeleteSpatialAnchorsAsync)));
            XrDeleteSpatialAnchorsComplete = (delegate* unmanaged[Cdecl]<ulong, ulong, out XrSpatialAnchorsDeleteCompletion, XrResult>)LocateNativeFunction(SanitizeName(nameof(XrDeleteSpatialAnchorsComplete)));
            XrUpdateSpatialAnchorsExpirationAsync = (delegate* unmanaged[Cdecl]<ulong, XrSpatialAnchorsUpdateExpirationInfo*, out ulong, XrResult>)LocateNativeFunction(SanitizeName(nameof(XrUpdateSpatialAnchorsExpirationAsync)));
            XrUpdateSpatialAnchorsExpirationComplete = (delegate* unmanaged[Cdecl]<ulong, ulong, out XrSpatialAnchorsUpdateExpirationCompletion, XrResult>)LocateNativeFunction(SanitizeName(nameof(XrUpdateSpatialAnchorsExpirationComplete)));
        }
    }
}
