using System;
using System.Diagnostics;
using MagicLeap.OpenXR.NativeDelegates;

namespace MagicLeap.OpenXR
{
    internal abstract class NativeFunctionsBase
    {
        protected XrGetInstanceProcAddr GetInstanceProcAddr;
        protected ulong XrInstance;
        
        public static T Create<T>(XrGetInstanceProcAddr instanceProcAddr, ulong xrInstance) where T : NativeFunctionsBase, new()
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
