// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
﻿using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapLocateSpacesNativeTypes;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapOpenXRFeatureNativeTypes;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MagicLeapLocateSpacesNativeFunctions")]
    internal unsafe class MagicLeapLocateSpacesNativeFunctions : MagicLeapNativeFunctionsBase
    {
        internal delegate* unmanaged [Cdecl] <ulong, in XrSpaceLocateInfo, out XrSpaceLocations, XrResult> XrLocateSpaces;
        
        protected override void LocateNativeFunctions()
        {
            XrLocateSpaces = (delegate* unmanaged[Cdecl]<ulong, in XrSpaceLocateInfo, out XrSpaceLocations, XrResult>)LocateNativeFunction("xrLocateSpacesKHR");
        }

        internal XrPose[] LocateSpaces(ulong baseSpace, long predictedDisplayTime, ulong session, ulong[] spaces)
        {
            var result = new XrPose[spaces.Length];
            Array.Fill(result, XrPose.IdentityPose);
            
            var spaceLocateInfo = new XrSpaceLocateInfo
            {
                Type = XrLocateSpaceStructTypes.XRTypeSpacesLocateInfo,
                BaseSpace = baseSpace,
                Time = predictedDisplayTime,
                SpaceCount = (uint)spaces.Length
            };
            var spacesArray = new NativeArray<ulong>(spaces, Allocator.Temp);
            spaceLocateInfo.Spaces = (ulong*) spacesArray.GetUnsafePtr();

            var spaceLocationsArray = new NativeArray<XrSpaceLocationData>(spaces.Length, Allocator.Temp);
            var spaceLocations = new XrSpaceLocations
            {
                Type = XrLocateSpaceStructTypes.XRTypeSpaceLocations,
                LocationCount = (uint) spaces.Length,
                Locations = (XrSpaceLocationData*) spaceLocationsArray.GetUnsafePtr()
            };

            var xrResult = XrLocateSpaces(session, in spaceLocateInfo, out spaceLocations);
            if(!Utils.DidXrCallSucceed(xrResult, nameof(XrLocateSpaces)))
            {
                return result;
            }

            result = new XrPose[spaces.Length];
            for (var i = 0; i < spaces.Length; i++)
            {
                result[i] = spaceLocationsArray[i].GetValidPose();
            }

            return result;
        }
    }
}
