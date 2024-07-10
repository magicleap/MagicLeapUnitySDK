using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Features.UserCalibration
{
    internal unsafe class UserCalibrationNativeFunctions : NativeFunctionsBase
    {
        internal delegate* unmanaged [Cdecl] <ulong, in XrUserCalibrationEnableEventsInfo, XrResult> XrEnableUserCalibrationEvents;
        protected override void LocateNativeFunctions()
        {
            XrEnableUserCalibrationEvents = (delegate* unmanaged[Cdecl]<ulong, in XrUserCalibrationEnableEventsInfo, XrResult>)LocateNativeFunction("xrEnableUserCalibrationEventsML");
        }
    }
}
