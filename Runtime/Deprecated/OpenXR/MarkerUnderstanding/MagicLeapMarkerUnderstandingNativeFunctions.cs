using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MarkerUnderstandingNativeTypes;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MagicLeapMarkerUnderstandingNativeFunctions")]
    internal unsafe class MagicLeapMarkerUnderstandingNativeFunctions : MagicLeapSpaceInfoNativeFunctions
    {
        internal delegate* unmanaged [Cdecl] <ulong, in XrMarkerDetectorCreateInfo, out ulong, XrResult> XrCreateMarkerDetector;
        internal delegate* unmanaged [Cdecl] <ulong, XrResult> XrDestroyMarkerDetector;
        internal delegate* unmanaged [Cdecl] <ulong, ref XrMarkerDetectorSnapshotInfo, XrResult> XrSnapshotMarkerDetector;
        internal delegate* unmanaged [Cdecl] <ulong, out XrMarkerDetectorState, XrResult> XrGetMarkerDetectorState;
        internal delegate* unmanaged [Cdecl] <ulong, uint, out uint, ulong*, XrResult> XrGetMarkers;
        internal delegate* unmanaged [Cdecl] <ulong, ulong, out float, XrResult> XrGetMarkerReprojectionError;
        internal delegate* unmanaged [Cdecl] <ulong, ulong, out float, XrResult> XrGetMarkerLength;
        internal delegate* unmanaged [Cdecl] <ulong, ulong, out ulong, XrResult> XrGetMarkerNumber;
        internal delegate* unmanaged [Cdecl] <ulong, ulong, uint, out uint, byte*, XrResult> XrGetMarkerString;
        internal delegate* unmanaged [Cdecl] <ulong, in XrMarkerSpaceCreateInfo, out ulong, XrResult> XrCreateMarkerSpace;

        protected override void LocateNativeFunctions()
        {
            base.LocateNativeFunctions();
            XrCreateMarkerDetector = (delegate* unmanaged [Cdecl]<ulong, in XrMarkerDetectorCreateInfo, out ulong, XrResult>)LocateNativeFunction("xrCreateMarkerDetectorML");
            XrDestroyMarkerDetector = (delegate* unmanaged [Cdecl]<ulong, XrResult>)LocateNativeFunction("xrDestroyMarkerDetectorML");
            XrSnapshotMarkerDetector = (delegate* unmanaged [Cdecl]<ulong, ref XrMarkerDetectorSnapshotInfo, XrResult>)LocateNativeFunction("xrSnapshotMarkerDetectorML");
            XrGetMarkerDetectorState = (delegate* unmanaged [Cdecl]<ulong, out XrMarkerDetectorState, XrResult>)LocateNativeFunction("xrGetMarkerDetectorStateML");
            XrGetMarkers = (delegate* unmanaged [Cdecl]<ulong, uint, out uint, ulong*, XrResult>)LocateNativeFunction("xrGetMarkersML");
            XrGetMarkerReprojectionError = (delegate* unmanaged [Cdecl]<ulong, ulong, out float, XrResult>)LocateNativeFunction("xrGetMarkerReprojectionErrorML");
            XrGetMarkerLength = (delegate* unmanaged [Cdecl]<ulong, ulong, out float, XrResult>)LocateNativeFunction("xrGetMarkerLengthML");
            XrGetMarkerNumber = (delegate* unmanaged [Cdecl]<ulong, ulong, out ulong, XrResult>)LocateNativeFunction("xrGetMarkerNumberML");
            XrGetMarkerString = (delegate* unmanaged [Cdecl]<ulong, ulong, uint, out uint, byte*, XrResult>)LocateNativeFunction("xrGetMarkerStringML");
            XrCreateMarkerSpace = (delegate* unmanaged [Cdecl]<ulong, in XrMarkerSpaceCreateInfo, out ulong, XrResult>)LocateNativeFunction("xrCreateMarkerSpaceML");
        }
    }
}
