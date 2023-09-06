using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public sealed partial class MLXrPlaneSubsystem
    {
        internal abstract class NativeBindings : MagicLeapNativeBindings
        {
            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLOpenXRCreatePlaneTracker(out ulong planesTracker);
            
            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLOpenXRDestroyPlaneTracker(ulong planesTracker);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLBeginPlaneDetection(ulong planesTracker, in XrTypes.MLXrPlaneDetectorBeginInfo beginInfo);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLGetPlaneDetections(ulong planesDetector, out XrTypes.MLXrPlaneDetectorLocations planeLocations);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLGetPlaneDetectionState(ulong planesTracker, out XrTypes.MLXrPlaneDetectionState state);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLGetPlanePolygonBuffer(ulong planeDetector, ulong planeId, uint polygonBufferIndex, out XrTypes.MLXrPlaneDetectorPolygonBuffer polygonBuffer);
        }
    }
}
