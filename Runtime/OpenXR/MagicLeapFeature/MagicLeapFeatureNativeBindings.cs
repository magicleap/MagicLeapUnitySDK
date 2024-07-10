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
using MagicLeap.OpenXR.Constants;

namespace MagicLeap.OpenXR.Features
{
    public partial class MagicLeapFeature
    {
        internal abstract class NativeBindings
        {
            [DllImport(Values.PluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern FeatureLifecycleNativeListenerInternal MLOpenXRGetLifecycleListener();
        }
    }
}
