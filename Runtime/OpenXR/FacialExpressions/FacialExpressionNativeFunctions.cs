using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Features.FacialExpressions
{
    internal unsafe class FacialExpressionNativeFunctions : NativeFunctionsBase
    {
        internal delegate* unmanaged [Cdecl] <ulong, in XrFacialExpressionClientCreateInfo, out ulong, XrResult> XrCreateFacialExpressionClient;
        internal delegate* unmanaged [Cdecl] <ulong, XrResult> XrDestroyFacialExpressionClient;
        internal delegate* unmanaged [Cdecl] <ulong, in XrFacialExpressionShapeGetInfo, uint, XrFacialExpressionBlendShapeProperties*, XrResult> XrGetFacialExpressionBlendShapeProperties;
        
        private string SanitizeName(string name) => $"{name.Replace("Xr", "xr")}ML";

        protected override void LocateNativeFunctions()
        {
            XrCreateFacialExpressionClient = (delegate* unmanaged[Cdecl]<ulong, in XrFacialExpressionClientCreateInfo, out ulong, XrResult>)LocateNativeFunction(SanitizeName(nameof(XrCreateFacialExpressionClient)));
            XrDestroyFacialExpressionClient = (delegate* unmanaged[Cdecl]<ulong, XrResult>)LocateNativeFunction(SanitizeName(nameof(XrDestroyFacialExpressionClient)));
            XrGetFacialExpressionBlendShapeProperties = (delegate* unmanaged[Cdecl]<ulong, in XrFacialExpressionShapeGetInfo, uint, XrFacialExpressionBlendShapeProperties*, XrResult>)LocateNativeFunction(SanitizeName(nameof(XrGetFacialExpressionBlendShapeProperties)));
        }
    }
}
