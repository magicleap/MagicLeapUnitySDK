// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Native;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.NativeInterop;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapFeature
    {
        internal class NativeBindings : MagicLeapNativeBindings
        {
            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern void MLOpenXRGetLifecycleListener(ref FeatureLifecycleNativeListener listener);
        }
    }
}
