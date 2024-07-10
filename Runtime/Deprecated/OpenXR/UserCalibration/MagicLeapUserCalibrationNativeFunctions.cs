using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    using MagicLeapUserCalibrationNativeTypes;
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MagicLeapUserCalibrationNativeFunctions")]
    internal unsafe class MagicLeapUserCalibrationNativeFunctions : MagicLeapNativeFunctionsBase
    {
        internal delegate* unmanaged [Cdecl] <ulong, in XrUserCalibrationEnableEventsInfo, XrResult> XrEnableUserCalibrationEvents;
        protected override void LocateNativeFunctions()
        {
            XrEnableUserCalibrationEvents = (delegate* unmanaged[Cdecl]<ulong, in XrUserCalibrationEnableEventsInfo, XrResult>)LocateNativeFunction("xrEnableUserCalibrationEventsML");
        }
    }
}
