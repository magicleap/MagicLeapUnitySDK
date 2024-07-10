using System;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapOpenXRFeatureNativeTypes;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    namespace MagicLeapRenderingFeatureNativeTypes
    {
        internal enum XrRenderingStructTypes : ulong
        {
            XrTypeFrameEndInfo = 12,
            XrTypeFrameEndInfoML = 1000135000U,
            XrTypeGlobalDimmerFrameEndInfo = 1000136000U,
            XrTypeCompositionLayerProjectionView = 48,
            XrTypeCompositionLayerProjection = 35,
        }

        internal enum XrCompositionLayerFlags : ulong
        {
            CorrectChomaticAberrationBit = 0x00001,
            BlendTextureSourceAlpha = 2,
            UnPreMultipliedAlpha = 4,
        }

        [Flags]
        internal enum XrFrameEndInfoFlagsML : ulong
        {
            Protected = 1,
            Vignette = 2
        }

        [Flags]
        internal enum XrGlobalDimmerFrameEndInfoFlags
        {
            Enabled = 1,
        }
        
        internal struct XrFrameWaitInfo
        {
            internal XrRenderingStructTypes Type;
            internal IntPtr Next;
        }

        internal struct XrFrameBeginInfo
        {
            internal XrRenderingStructTypes Type;
            internal IntPtr Next;
        }

        internal struct XrFrameState
        {
            internal XrRenderingStructTypes Type;
            internal IntPtr Next;
            internal long PredictedDisplayTime;
            internal ulong PredictedDisplayPeriod;
            internal XrBool32 ShouldRender;
        }

        internal struct XrCompositionLayerBaseHeader
        {
            internal XrRenderingStructTypes Type;
            internal IntPtr Next;
            internal XrCompositionLayerFlags LayerFlags;
            internal ulong Space;
        }

        internal unsafe struct XrFrameEndInfo
        {
            internal XrRenderingStructTypes Type;
            internal IntPtr Next;
            internal long DisplayTime;
            internal XrEnvironmentBlendMode EnvironmentBlendMode;
            internal uint LayerCount;
            internal XrCompositionLayerBaseHeader* Layers;
        }

        internal struct XrFrameEndInfoML
        {
            internal XrRenderingStructTypes Type;
            internal IntPtr Next;
            internal float FocusDistance;
            internal XrFrameEndInfoFlagsML Flags;
        }

        internal struct XrGlobalDimmerFrameEndInfoML
        {
            internal XrRenderingStructTypes Type;
            internal IntPtr Next;
            internal float DimmerValue;
            internal XrGlobalDimmerFrameEndInfoFlags Flags;
        }

        internal struct XrSwapChainSubImage
        {
            internal ulong SwapChain;
            internal XrRect2Di ImageRect;
            internal uint ImageArrayIndex;
        }

        internal struct XrCompositionLayerProjectionView
        {
            internal XrRenderingStructTypes Type;
            internal IntPtr Next;
            internal XrPose Pose;
            internal XrFOV FOV;
        }

        internal unsafe struct XrCompositionLayerProjection
        {
            internal XrRenderingStructTypes Type;
            internal IntPtr Next;
            internal XrCompositionLayerFlags LayerFlags;
            internal ulong Space;
            internal uint ViewCount;
            internal XrCompositionLayerProjectionView* Views;
        }
    }
}
