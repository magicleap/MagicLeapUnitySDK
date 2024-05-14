using System;
using System.Diagnostics;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapOpenXRFeatureNativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    internal abstract class MagicLeapNativeFunctionsBase
    {
        protected GetInstanceProcAddr GetInstanceProcAddr;
        protected ulong XrInstance;
        
        public static T Create<T>(GetInstanceProcAddr instanceProcAddr, ulong xrInstance) where T : MagicLeapNativeFunctionsBase, new()
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
