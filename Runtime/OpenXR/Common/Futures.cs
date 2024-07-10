using System;
using MagicLeap.OpenXR.Constants;
using UnityEngine;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Futures
{
    internal enum XrFutureState : uint
    {
        Pending = 1,
        Ready
    }
    
    internal struct XrFuture
    {
        private ulong handle;
        public static implicit operator XrFuture(ulong handle) =>
            new()
            {
                handle = handle
            };

        public static implicit operator ulong(XrFuture xrFuture) => xrFuture.handle;
    }
    
    internal static class XrFutureStructTypes
    {
        public const ulong XRTypeFutureCancelInfoExt = 1000469000U;
        public const ulong XRTypeFuturePollInfoExt = 1000469001U;
        public const ulong XRTypeFutureCompletionExt = 1000469002U;
        public const ulong XRTypeFuturePollResultExt = 1000469003U;
    }

    internal struct XrFutureCancelInfo
    {
        internal ulong Type;
        internal IntPtr Next;
        internal ulong Future;
    }

    internal struct XrFuturePollInfo
    {
        internal ulong Type;
        internal IntPtr Next;
        internal ulong Future;
    }

    internal struct XrFuturePollResult
    {
        internal ulong Type;
        internal IntPtr Next;
        internal XrFutureState State;
    }
    
    internal unsafe class FuturesNativeFunctions : NativeFunctionsBase
    {
        private delegate* unmanaged [Cdecl] <ulong, in XrFuturePollInfo, out XrFuturePollResult, XrResult> XrPollFuture;
        private delegate* unmanaged [Cdecl] <ulong, in XrFutureCancelInfo, XrResult> XrCancelFuture;

        protected override void LocateNativeFunctions()
        {
            XrPollFuture = (delegate* unmanaged[Cdecl]<ulong, in XrFuturePollInfo, out XrFuturePollResult, XrResult>)LocateNativeFunction("xrPollFutureEXT");
            XrCancelFuture = (delegate* unmanaged[Cdecl]<ulong, in XrFutureCancelInfo, XrResult>)LocateNativeFunction("xrCancelFutureEXT");
        }

        internal bool PollFuture(in XrFuture future, out XrFutureState outputState)
        {
            outputState = XrFutureState.Pending;
            XrResult resultCode;

            return PollFuture(future, out outputState, out resultCode);
        }

        internal bool PollFuture(in XrFuture future, out XrFutureState outputState, out XrResult pollResult)
        {
            outputState = XrFutureState.Pending;
            pollResult = (XrResult)MLXrResult.FuturePendingEXT;
            if (XrPollFuture == null)
            {
                Debug.LogError("Unable to find the Future extension");
                return false;
            }

            var pollInfo = new XrFuturePollInfo
            {
                Type = XrFutureStructTypes.XRTypeFuturePollInfoExt,
                Future = future
            };

            var futureResult = new XrFuturePollResult
            {
                Type = XrFutureStructTypes.XRTypeFuturePollResultExt,
                State = XrFutureState.Pending
            };

            pollResult = XrPollFuture(XrInstance, in pollInfo, out futureResult);
            if (!Utils.DidXrCallSucceed(pollResult, nameof(XrPollFuture)))
            {
                return false;
            }

            outputState = futureResult.State;
            return true;
        }

        internal XrResult CancelFuture(in XrFuture future)
        {
            if (XrCancelFuture == null)
            {
                return XrResult.FunctionUnsupported;
            }

            var cancelInfo = new XrFutureCancelInfo
            {
                Type = XrFutureStructTypes.XRTypeFutureCancelInfoExt,
                Next = IntPtr.Zero,
                Future = future
            };
            var xrResult = XrCancelFuture(XrInstance, in cancelInfo);
            Utils.DidXrCallSucceed(xrResult, nameof(XrCancelFuture));
            return xrResult;
        }
    }
}
