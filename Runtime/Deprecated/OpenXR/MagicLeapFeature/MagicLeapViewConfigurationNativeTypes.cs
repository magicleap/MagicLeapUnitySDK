// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    namespace MagicLeapViewConfigurationNativeTypes
    {
        internal enum XrMagicLeapFeatureStructTypes : ulong
        {
            ViewConfigurationDepthRange = 1000046000,
            ViewConfigurationView = 41,
        }
        
        internal struct XrViewConfigurationProperties
        {
            internal XrMagicLeapFeatureStructTypes Type;
            internal IntPtr Next;
            internal XrViewConfigurationType ConfigurationType;
        }

        internal struct XrViewConfigurationView
        {
            internal XrMagicLeapFeatureStructTypes Type;
            internal IntPtr Next;
            internal uint RecommendedImageRectWidth;
            internal uint MaxImageRectWidth;
            internal uint RecommendedImageRectHeight;
            internal uint MaxImageRectHeight;
            internal uint RecommendedSwapchainSampleCount;
            internal uint MaxSwapchainSampleCount;
        }

        internal struct XrViewConfigurationDepthRange
        {
            internal XrMagicLeapFeatureStructTypes Type;
            internal IntPtr Next;
            internal float RecommendedNearZ;
            internal float MinNearZ;
            internal float RecommendedFarZ;
            internal float MaxFarZ;
        }
    }
}
