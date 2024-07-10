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
using System.Diagnostics;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapOpenXRFeatureNativeTypes;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapOpenXRNativeDelegates;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    internal abstract class MagicLeapNativeFunctionsBase
    {
        protected XrGetInstanceProcAddr GetInstanceProcAddr;
        protected ulong XrInstance;
        
        public static T Create<T>(XrGetInstanceProcAddr instanceProcAddr, ulong xrInstance) where T : MagicLeapNativeFunctionsBase, new()
        {
            var result = new T
            {
                GetInstanceProcAddr = instanceProcAddr,
                XrInstance = xrInstance
            };
            result.LocateNativeFunctions();
            result.Validate();
            return result;
        }

        protected IntPtr LocateNativeFunction(string functionName)
        {
            if (GetInstanceProcAddr == null)
            {
                return IntPtr.Zero;
            }
            var targetPtr = IntPtr.Zero;
            GetInstanceProcAddr(XrInstance, functionName, ref targetPtr);
            return targetPtr;
        }
        protected abstract void LocateNativeFunctions();
        
        [Conditional("DEVELOPMENT_BUILD")]
        protected virtual void Validate()
        {
        }
    }
}
