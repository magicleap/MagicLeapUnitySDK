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
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    using MagicLeapViewConfigurationNativeTypes;
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MagicLeapViewConfigurationNativeFunctions")]
    internal unsafe class MagicLeapViewConfigurationNativeFunctions : MagicLeapNativeFunctionsBase
    {
        internal delegate* unmanaged [Cdecl] <ulong, ulong, XrViewConfigurationType, uint, out uint, XrViewConfigurationView*, XrResult> XrEnumerateViewConfigurationViews;
        protected override void LocateNativeFunctions()
        {
            XrEnumerateViewConfigurationViews = (delegate* unmanaged[Cdecl]<ulong, ulong, XrViewConfigurationType, uint, out uint, XrViewConfigurationView*, XrResult>)LocateNativeFunction("xrEnumerateViewConfigurationViews");
        }

        internal XrResult EnumerateConfigurationViews(ulong instance, ulong systemId, out XrViewConfigurationDepthRange depthRangeResult)
        {
            depthRangeResult = default;
            var xrResult = XrEnumerateViewConfigurationViews(instance, systemId, XrViewConfigurationType.PrimaryStereo, 0, out var viewConfigurationCount, null);
            if (!Utils.DidXrCallSucceed(xrResult, nameof(XrEnumerateViewConfigurationViews)))
            {
                return xrResult;
            }
            var viewConfigurationArray = new NativeArray<XrViewConfigurationView>((int)viewConfigurationCount, Allocator.Temp);
            var depthRange = new XrViewConfigurationDepthRange
            {
                Type = XrMagicLeapFeatureStructTypes.ViewConfigurationDepthRange,
            };
            NativeCopyUtility.FillArrayWithValue(viewConfigurationArray, new XrViewConfigurationView
            {
                Type = XrMagicLeapFeatureStructTypes.ViewConfigurationView,
                Next = new IntPtr(&depthRange),
            });
            xrResult = XrEnumerateViewConfigurationViews(instance, systemId, XrViewConfigurationType.PrimaryStereo, viewConfigurationCount, out viewConfigurationCount, (XrViewConfigurationView*)viewConfigurationArray.GetUnsafePtr());
            if (!Utils.DidXrCallSucceed(xrResult, nameof(XrEnumerateViewConfigurationViews)))
            {
                return xrResult;
            }
            depthRangeResult = depthRange;
            return XrResult.Success;
        }
    }
}
