#if UNITY_OPENXR_1_9_0_OR_NEWER
using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Native;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public sealed partial class MLXrPlaneSubsystem
    {
        internal abstract class NativeBindings : MagicLeapNativeBindings
        {
            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern XrResult MLOpenXRCreatePlaneTracker(out ulong planesTracker);
            
            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern XrResult MLOpenXRDestroyPlaneTracker(ulong planesTracker);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern XrResult MLBeginPlaneDetection(ulong planesTracker, in XrTypes.MLXrPlaneDetectorBeginInfo beginInfo);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern XrResult MLGetPlaneDetections(ulong planesDetector, out XrTypes.MLXrPlaneDetectorLocations planeLocations);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern XrResult MLGetPlaneDetectionState(ulong planesTracker, out XrTypes.MLXrPlaneDetectionState state);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern XrResult MLGetPlanePolygonBuffer(ulong planeDetector, ulong planeId, uint polygonBufferIndex, out XrTypes.MLXrPlaneDetectorPolygonBuffer polygonBuffer);
        }
    }
}

#endif
