#if UNITY_OPENXR_1_9_0_OR_NEWER
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Native;

namespace OpenXR.PointCloud
{
    internal abstract class MagicLeapOpenXRPointCloudNativeBindings : MagicLeapNativeBindings
    {
        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRBeginPointCloudDetection();

        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRStopPointCloudDetection();

        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe void MLOpenXRPointCloudGetChanges(ref TrackableId* meshIds, out int count);
    }
}
#endif
