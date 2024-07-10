using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace MagicLeap.OpenXR.Features.FacialExpressions
{
    public partial class MagicLeapFacialExpressionFeature
    {
        private ulong facialExpressionClient;
        private FacialExpressionNativeFunctions nativeFunctions;
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

                var xrResult = nativeFunctions.XrGetFacialExpressionBlendShapeProperties(facialExpressionClient, in getInfo, 
                    (uint)nativeShapePropertiesArray.Length, (XrFacialExpressionBlendShapeProperties*)nativeShapePropertiesArray.GetUnsafePtr());
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
