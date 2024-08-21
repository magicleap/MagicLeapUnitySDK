// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using System;
using MagicLeap.OpenXR.SystemInfo;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Features.Reprojection
{
    internal unsafe class MagicLeapReprojectionNativeFunctions : SystemInfoNativeFunctions
    {
        internal delegate* unmanaged [Cdecl] <XrInstance, XrSystemId, XrViewConfigurationType, uint, out uint, XrReprojectionMode*, XrResult> XrEnumerateReprojectionModes;
        protected override void LocateNativeFunctions()
        {
            base.LocateNativeFunctions();
            XrEnumerateReprojectionModes = (delegate* unmanaged[Cdecl]<XrInstance, XrSystemId, XrViewConfigurationType, uint, out uint, XrReprojectionMode*, XrResult>)LocateNativeFunction("xrEnumerateReprojectionModesMSFT");
        }

        internal XrResult GetSupportedReprojectionModes(XrInstance instance, XrSystemId systemId, out XrReprojectionMode[] reprojectionModes, XrViewConfigurationType viewConfigurationType = XrViewConfigurationType.PrimaryStereo)
        {
            reprojectionModes = Array.Empty<XrReprojectionMode>();
            var xrResult = XrEnumerateReprojectionModes(instance, systemId, viewConfigurationType, 0, out var supportedModeCount, null);
            if (!Utils.DidXrCallSucceed(xrResult, nameof(XrEnumerateReprojectionModes)))
            {
                return xrResult;
            }

            var nativeArray = new NativeArray<XrReprojectionMode>((int)supportedModeCount, Allocator.Temp);
            xrResult = XrEnumerateReprojectionModes(instance, systemId, viewConfigurationType, supportedModeCount, out supportedModeCount, (XrReprojectionMode*)nativeArray.GetUnsafePtr());
            if (!Utils.DidXrCallSucceed(xrResult, nameof(XrEnumerateReprojectionModes)))
            {
                return xrResult;
            }

            reprojectionModes = nativeArray.ToArray();
            return xrResult;
        }
    }
}
