using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Features.LocalizationMaps
{
    internal unsafe class LocalizationMapNativeFunctions : NativeFunctionsBase
    {
        internal delegate* unmanaged [Cdecl] <ulong, in XrLocalizationEnableEventsInfo, XrResult> XrEnableLocalizationEvents;
        internal delegate* unmanaged [Cdecl] <ulong, in XrLocalizationMapQueryInfoBaseHeader*, uint, out uint, XrLocalizationMap*, XrResult> XrQueryLocalizationMaps;
        internal delegate* unmanaged [Cdecl] <ulong, in XrMapLocalizationRequestInfo, XrResult> XrRequestMapLocalization;
        internal delegate* unmanaged [Cdecl] <ulong, in XrLocalizationMapImportInfo, out XrUUID, XrResult> XrImportLocalizationMap;
        internal delegate* unmanaged [Cdecl] <ulong, in XrUUID, out ulong, XrResult> XrCreateExportedLocalizationMap;
        internal delegate* unmanaged [Cdecl] <ulong, XrResult> XrDestroyExportedLocalizationMap;
        internal delegate* unmanaged [Cdecl] <ulong, uint, out uint, byte*, XrResult> XrGetExportedLocalizationMapData;

        private string SanitizeFunctionName(string name) => $"{name.Replace("Xr", "xr")}ML";

        protected override void LocateNativeFunctions()
        {
            XrEnableLocalizationEvents = (delegate* unmanaged[Cdecl]<ulong, in XrLocalizationEnableEventsInfo, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrEnableLocalizationEvents)));
            XrQueryLocalizationMaps = (delegate* unmanaged[Cdecl]<ulong, in XrLocalizationMapQueryInfoBaseHeader*, uint, out uint, XrLocalizationMap*, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrQueryLocalizationMaps)));
            XrRequestMapLocalization = (delegate* unmanaged[Cdecl]<ulong, in XrMapLocalizationRequestInfo, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrRequestMapLocalization)));
            XrImportLocalizationMap = (delegate* unmanaged[Cdecl]<ulong, in XrLocalizationMapImportInfo, out XrUUID, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrImportLocalizationMap)));
            XrCreateExportedLocalizationMap = (delegate* unmanaged[Cdecl]<ulong, in XrUUID, out ulong, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrCreateExportedLocalizationMap)));
            XrDestroyExportedLocalizationMap = (delegate* unmanaged[Cdecl]<ulong, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrDestroyExportedLocalizationMap)));
            XrGetExportedLocalizationMapData = (delegate* unmanaged[Cdecl]<ulong, uint, out uint, byte*, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetExportedLocalizationMapData)));
        }
    }
}
