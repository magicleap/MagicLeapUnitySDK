// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MagicLeap.OpenXR.Features.Meshing
{
    internal abstract class MagicLeapXrMeshingNativeBindings
    {
        private const string DllName = "MagicLeapXrProvider";

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRMeshingUpdateSettings(in MeshingQuerySettings settings);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe void MLOpenXRAcquireMeshIds(ref XrMeshId* trackableIds, out int trackableCount);
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe bool MLOpenXRAcquireMeshData(in XrMeshId trackableId, ref Vector3* positions, out int positionCount, ref Vector3* normals, out int normalCount, ref float* confidence, out int confidenceCount);
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRSetMeshQueryBounds(in Vector3 position,in Quaternion rotation, in Vector3 scale);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRSetMeshRenderMode(MeshingMode mode);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRSetMeshBoundsOrigin(in Vector3 position);
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRSetMeshBoundsScale(in Vector3 scale);
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRSetMeshBoundsOrientation(in Quaternion rotation);
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRSetMeshDensity(float density);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRInvalidateMeshes();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRGetCurrentFrameMeshData(out FrameMeshInfo frameMeshInfo);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern FeatureLifecycleNativeListenerInternal MLOpenXRGetMeshingFeatureListener();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRBeginPointCloudDetection();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRStopPointCloudDetection();
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MLOpenXRGetCurrentMeshes(ref IntPtr meshIdBuffer, out int count);
    }
}
