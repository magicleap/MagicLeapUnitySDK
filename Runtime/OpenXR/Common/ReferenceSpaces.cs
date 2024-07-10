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
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.ReferenceSpaces
{
    internal enum XrReferenceSpaceStructTypes : ulong
    {
        XRTypeReferenceSpaceCreateInfo = 37
    }

    internal enum XrReferenceSpaceTypeExtended : ulong
    {
        LocalizationMap = 1000139000,
        LocalFloor = 1000426000
    }

    internal struct XrReferenceSpaceCreateInfo
    {
        internal XrReferenceSpaceStructTypes Type;
        internal IntPtr Next;
        internal XrReferenceSpaceType ReferenceSpaceType;
        internal XrPose PoseInReferenceSpace;
    }

    internal unsafe class ReferenceSpaceNativeFunctions : NativeFunctionsBase
    {
        internal delegate* unmanaged[Cdecl]<ulong, in XrReferenceSpaceCreateInfo, ulong*, XrResult> XrCreateReferenceSpace;

        protected override void LocateNativeFunctions()
        {
            XrCreateReferenceSpace = (delegate* unmanaged[Cdecl]<ulong, in XrReferenceSpaceCreateInfo, ulong*, XrResult>)LocateNativeFunction("xrCreateReferenceSpace");
        }

        // Will also accept XrReferenceSpaceTypeExtended cast to XrReferenceSpaceType
        internal ulong CreateReferenceSpace(ulong session, XrReferenceSpaceType referenceSpacetype)
        {
            var createInfo = new XrReferenceSpaceCreateInfo
            {
                Type = XrReferenceSpaceStructTypes.XRTypeReferenceSpaceCreateInfo,
                ReferenceSpaceType = referenceSpacetype,
                PoseInReferenceSpace = XrPose.IdentityPose
            };

            ulong originSpace = 0u;

            var xrResult = XrCreateReferenceSpace(session, in createInfo, &originSpace);
            if (!Utils.DidXrCallSucceed(xrResult, nameof(XrCreateReferenceSpace)))
            {
                return 0u;
            }

            return originSpace;
        }
    }
}
