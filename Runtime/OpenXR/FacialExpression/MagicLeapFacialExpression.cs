#if UNITY_OPENXR_1_9_0_OR_NEWER
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap;

using NativeBindings = UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapFacialExpressionFeature.NativeBindings;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapFacialExpressionFeature
    {
        private ulong facialExpressionClient;

        public void CreateClient(FacialBlendShape[] requestedFacialBlendShapes)
        {
            var resultCode = NativeBindings.MLOpenXRCreateFacialExpressionClient((uint)requestedFacialBlendShapes.Length, requestedFacialBlendShapes, out ulong facialExpressionClient);
            Utils.DidXrCallSucceed(resultCode, nameof(NativeBindings.MLOpenXRCreateFacialExpressionClient));

            this.facialExpressionClient = facialExpressionClient;
        }

        public void DestroyClient()
        {
            var resultCode = NativeBindings.MLOpenXRDestroyFacialExpressionClient(facialExpressionClient);
            Utils.DidXrCallSucceed(resultCode, nameof(NativeBindings.MLOpenXRDestroyFacialExpressionClient));
        }

        public void GetBlendShapesInfo(ref BlendShapeProperties[] blendShapes)
        {
            var resultCode = NativeBindings.MLOpenXRGetFacialExpressionBlendShapesInfo(facialExpressionClient, (uint)blendShapes.Length, blendShapes);
            Utils.DidXrCallSucceed(resultCode, nameof(NativeBindings.MLOpenXRGetFacialExpressionBlendShapesInfo));
        }       
    }
}
#endif