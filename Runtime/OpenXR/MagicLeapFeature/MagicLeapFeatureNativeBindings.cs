// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if UNITY_OPENXR_1_9_0_OR_NEWER
using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapFeature
    {
        internal class NativeBindings : MagicLeapNativeBindings
        {
            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLOpenXRInterceptFunctions(IntPtr loaderFunc);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool MLOpenXROnInstanceCreate(IntPtr loaderFunc, ulong instance);
            
            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern void MLOpenXROnInstanceDestroy(ulong instance);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern void MLOpenXROnAppSpaceChange(ulong appSpace);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern void MLOpenXROnSessionCreate(ulong session);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern void MLOpenXROnSessionDestroy(ulong session);
            
            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern void MLOpenXROnFeatureInstanceCreate([MarshalAs(UnmanagedType.LPStr)] string featureId, ulong instance, IntPtr xrGetInstanceProcAddr);
            
            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern void MLOpenXROnFeatureSessionCreate([MarshalAs(UnmanagedType.LPStr)] string featureId, ulong session);
            
            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern void MLOpenXROnFeatureAppSpaceChange([MarshalAs(UnmanagedType.LPStr)] string featureId, ulong space);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern void MLOpenXRFeatureOnInstanceDestroy([MarshalAs(UnmanagedType.LPStr)] string featureId, ulong instance);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLOpenXRInterceptFunctionsForFeature([MarshalAs(UnmanagedType.LPStr)] string featureId, IntPtr original);
            
            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool MLOpenXRGetUnityPoseForFeature([MarshalAs(UnmanagedType.LPStr)] string featureId, ulong space, out Pose pose);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern long MLOpenXRGetNextPredictedDisplayTime();
        }
    }
}
#endif //UNITY_OPENXR_1_9_0_OR_NEWER
