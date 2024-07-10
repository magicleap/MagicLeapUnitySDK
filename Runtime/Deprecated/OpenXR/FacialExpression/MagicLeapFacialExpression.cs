// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    using MagicLeapFacialExpressionNativeTypes;

    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.FactialExpressions")]
    public partial class MagicLeapFacialExpressionFeature
    {
        private ulong facialExpressionClient;
        private MagicLeapFacialExpressionNativeFunctions nativeFunctions;
        private bool validClientCreated;

        public void CreateClient(FacialBlendShape[] requestedFacialBlendShapes)
        {
            unsafe
            {
                DestroyClient();
                var blendShapeArray = new NativeArray<XrFacialBlendShapeML>(requestedFacialBlendShapes.Length, Allocator.Temp);
                for (var i = 0; i < requestedFacialBlendShapes.Length; i++)
                {
                    blendShapeArray[i] = (XrFacialBlendShapeML)requestedFacialBlendShapes[i];
                }
                var createInfo = new XrFacialExpressionClientCreateInfo
                {
                    Type = XrFacialExpressionStructTypes.XrTypeFacialExpressionClientCreateInfo,
                    Next = default,
                    RequestedCount = (uint)requestedFacialBlendShapes.Length,
                    RequestedFacialBlendShapes = (XrFacialBlendShapeML*)blendShapeArray.GetUnsafePtr()
                };
                var xrResult = nativeFunctions.XrCreateFacialExpressionClient(AppSession, in createInfo, out facialExpressionClient);
                validClientCreated = Utils.DidXrCallSucceed(xrResult, nameof(nativeFunctions.XrCreateFacialExpressionClient));
            }
        }  

        public void DestroyClient()
        {
            unsafe
            {
                if (!validClientCreated)
                {
                    return;
                }
                
                var resultCode = nativeFunctions.XrDestroyFacialExpressionClient(facialExpressionClient);
                validClientCreated = !Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrDestroyFacialExpressionClient));
                facialExpressionClient = 0;
            }
        }        

        public void GetBlendShapesInfo(ref BlendShapeProperties[] blendShapes)
        {
            unsafe
            {
                var getInfo = new XrFacialExpressionShapeGetInfo
                {
                    Type = XrFacialExpressionStructTypes.XrTypeFacialExpressionBlendShapeGetInfo
                };
                var nativeShapePropertiesArray = new NativeArray<XrFacialExpressionBlendShapeProperties>(blendShapes.Length, Allocator.Temp);
                for (var i = 0; i < blendShapes.Length; i++)
                {
                    nativeShapePropertiesArray[i] = XrFacialExpressionBlendShapeProperties.CreateFromBlendShapeProperties(in blendShapes[i], NextPredictedDisplayTime);
                }

                var xrResult = nativeFunctions.XrGetFacialExpressionBlendShapeProperties(facialExpressionClient, in getInfo, (uint)nativeShapePropertiesArray.Length, (XrFacialExpressionBlendShapeProperties*)nativeShapePropertiesArray.GetUnsafePtr());
                if (!Utils.DidXrCallSucceed(xrResult, nameof(nativeFunctions.XrGetFacialExpressionBlendShapeProperties)))
                {
                    return;
                }

                for (var i = 0; i < nativeShapePropertiesArray.Length; i++)
                {
                    nativeShapePropertiesArray[i].AssignBlendShapeProperties(ref blendShapes[i]);
                }
            }
        }         
    }
}
