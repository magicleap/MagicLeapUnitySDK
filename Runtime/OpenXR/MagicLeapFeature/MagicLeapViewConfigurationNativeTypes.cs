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
