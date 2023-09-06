// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
#if UNITY_OPENXR_1_7_0_OR_NEWER
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
            public enum XrHandEXT
            {
                Left = 1,
                Right = 2
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct XrPosef
            {
                public Quaternion orientation;
                public Vector3 position;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct XrHandJointLocationEXT
            {
                public UInt64 locationFlags;
                public XrPosef pose;
                public float radius;
            }

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
            public static extern MLResult.Code MLOpenXRStartXRHandTracking();

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern unsafe MLResult.Code MLOpenXRGetXRHandTrackingJoints(XrHandEXT hand, XrHandJointLocationEXT* joints);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLOpenXRStopXRHandTracking();

            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLZIPermissionsStart();

            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLZIPermissionsStop();
        }
    }
}
#endif //UNITY_OPENXR_1_7_0_OR_NEWER
