#if UNITY_OPENXR_1_9_0_OR_NEWER

using System.Runtime.InteropServices;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Native;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.NativeInterop;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MagicLeapXrMeshingNativeBindings")]
    internal abstract class MagicLeapXrMeshingNativeBindings : MagicLeapNativeBindings
    {
        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRMeshingUpdateSettings(in MagicLeapMeshingFeature.MeshingQuerySettings settings);

        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe void MLOpenXRAcquireMeshIds(ref TrackableId* trackableIds, out int trackableCount);
        
        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe bool MLOpenXRAcquireMeshData(in TrackableId trackableId, ref Vector3* positions, out int positionCount, ref Vector3* normals, out int normalCount, ref float* confidence, out int confidenceCount);
        
        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRSetMeshQueryBounds(in Vector3 position,in Quaternion rotation, in Vector3 scale);

        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRSetMeshRenderMode(MagicLeapMeshingFeature.MeshingMode mode);

        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRSetMeshBoundsOrigin(in Vector3 position);
        
        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRSetMeshBoundsScale(in Vector3 scale);
        
        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRSetMeshBoundsOrientation(in Quaternion rotation);
        
        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRSetMeshDensity(float density);

        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRInvalidateMeshes();

        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRGetCurrentFrameMeshData(out MagicLeapMeshingFeature.FrameMeshInfo frameMeshInfo);

        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRGetMeshingFeatureListener(ref FeatureLifecycleNativeListener lifecycleNativeListener);

    }
}
#endif
