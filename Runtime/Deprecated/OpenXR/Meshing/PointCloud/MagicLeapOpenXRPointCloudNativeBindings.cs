#if UNITY_OPENXR_1_9_0_OR_NEWER
using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Native;

namespace OpenXR.PointCloud
{
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MagicLeapOpenXRPointCloudNativeBindings")]
    internal abstract class MagicLeapOpenXRPointCloudNativeBindings : MagicLeapNativeBindings
    {
        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRBeginPointCloudDetection();

        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRStopPointCloudDetection();
        
        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRGetCurrentMeshes(ref IntPtr meshIdBuffer, out int count);
    }
}
#endif
