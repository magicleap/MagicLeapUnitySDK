using System;
using UnityEngine;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Features.EyeTracker
{
    internal unsafe class EyeTrackerNativeFunctions : NativeFunctionsBase
    {
        internal delegate* unmanaged[Cdecl]<ulong, in XrEyeTrackerCreateInfo, out ulong, XrResult> XrCreateEyeTracker;
        internal delegate* unmanaged[Cdecl]<ulong, XrResult> XrDestroyEyeTracker;
        internal delegate* unmanaged[Cdecl]<ulong, in XrEyeTrackerStaticDataGetInfo, ref XrEyeTrackerStaticData, XrResult> XrGetEyeTrackerStaticData;
        internal delegate* unmanaged[Cdecl]<ulong, in XrEyeTrackerGeometricDataGetInfo, uint, out uint, XrEyeTrackerGeometricData*, XrResult> XrGetEyeTrackerGeometricData;
        internal delegate* unmanaged[Cdecl]<ulong, in XrEyeTrackerPupilDataGetInfo, uint, out uint, XrEyeTrackerPupilData*, XrResult> XrGetEyeTrackerPupilData;
        internal delegate* unmanaged[Cdecl]<ulong, in XrEyeTrackerGazeBehaviorGetInfo, ref XrEyeTrackerGazeBehavior, XrResult> XrGetEyeTrackerGazeBehavior;
        internal delegate* unmanaged[Cdecl]<ulong, in XrEyeTrackerPosesGetInfo, ref XrEyeTrackerPoses, XrResult> XrGetEyeTrackerPoses;
        
        protected override void Validate()
        {
            base.Validate();
            if (XrCreateEyeTracker == null)
            {
                Debug.LogError($"Unable to find {nameof(XrCreateEyeTracker)}");
            }

            if (XrDestroyEyeTracker == null)
            {
                Debug.LogError($"Unable to find {nameof(XrDestroyEyeTracker)}");
            }

            if (XrGetEyeTrackerStaticData == null)
            {
                Debug.LogError($"Unable to find {nameof(XrGetEyeTrackerStaticData)}");
            }

            if (XrGetEyeTrackerGeometricData == null)
            {
                Debug.LogError($"Unable to find {nameof(XrGetEyeTrackerGeometricData)}");
            }

            if (XrGetEyeTrackerPupilData == null)
            {
                Debug.LogError($"Unable to find {nameof(XrGetEyeTrackerPupilData)}");
            }

            if (XrGetEyeTrackerGazeBehavior == null)
            {
                Debug.LogError($"Unable to find {nameof(XrGetEyeTrackerGazeBehavior)}");
            }
            
            if (XrGetEyeTrackerPoses == null)
            {
                Debug.LogError($"Unable to find {nameof(XrGetEyeTrackerPoses)}");
            }
        }

        private string SanitizeFunctionName(string input)
        {
            return $"{input.Replace("Xr", "xr")}ML";
        }

        protected override void LocateNativeFunctions()
        {
            XrCreateEyeTracker = (delegate* unmanaged[Cdecl]<ulong, in XrEyeTrackerCreateInfo, out ulong, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrCreateEyeTracker)));
            XrDestroyEyeTracker = (delegate* unmanaged[Cdecl]<ulong, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrDestroyEyeTracker)));
            XrGetEyeTrackerStaticData = (delegate* unmanaged[Cdecl]<ulong, in XrEyeTrackerStaticDataGetInfo, ref XrEyeTrackerStaticData, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetEyeTrackerStaticData)));
            XrGetEyeTrackerGeometricData = (delegate* unmanaged[Cdecl]<ulong, in XrEyeTrackerGeometricDataGetInfo, uint, out uint, XrEyeTrackerGeometricData*, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetEyeTrackerGeometricData)));
            XrGetEyeTrackerPupilData = (delegate* unmanaged[Cdecl]<ulong, in XrEyeTrackerPupilDataGetInfo, uint, out uint, XrEyeTrackerPupilData*, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetEyeTrackerPupilData)));
            XrGetEyeTrackerGazeBehavior = (delegate* unmanaged[Cdecl]<ulong, in XrEyeTrackerGazeBehaviorGetInfo, ref XrEyeTrackerGazeBehavior, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetEyeTrackerGazeBehavior)));
            XrGetEyeTrackerPoses = (delegate* unmanaged[Cdecl]<ulong, in XrEyeTrackerPosesGetInfo, ref XrEyeTrackerPoses, XrResult>)LocateNativeFunction(SanitizeFunctionName(nameof(XrGetEyeTrackerPoses)));
        }
    }
}
