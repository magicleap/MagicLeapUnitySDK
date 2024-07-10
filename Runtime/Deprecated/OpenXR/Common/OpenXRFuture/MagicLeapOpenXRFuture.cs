using System;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MagicLeapFutureNativeFunctions")]
    internal unsafe class MagicLeapFutureNativeFunctions : MagicLeapNativeFunctionsBase
    {
        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.XrFutureStructTypes")]
        private static class XrFutureStructTypes
        {
            public const ulong XRTypeFutureCancelInfoExt = 1000469000U;
            public const ulong XRTypeFuturePollInfoExt = 1000469001U;
            public const ulong XRTypeFutureCompletionExt = 1000469002U;
            public const ulong XRTypeFuturePollResultExt = 1000469003U;
        }

        private struct XrFutureCancelInfo
        {
            internal ulong Type;
            internal IntPtr Next;
            internal ulong Future;
        }

        private struct XrFuturePollInfo
        {
            internal ulong Type;
            internal IntPtr Next;
            internal ulong Future;
        }

        private struct XrFuturePollResult
        {
            internal ulong Type;
            internal IntPtr Next;
            internal XrFutureState State;
        }

        private delegate* unmanaged [Cdecl] <ulong, in XrFuturePollInfo, out XrFuturePollResult, XrResult> XrPollFuture;
        private delegate* unmanaged [Cdecl] <ulong, in XrFutureCancelInfo, XrResult> XrCancelFuture;

        protected override void LocateNativeFunctions()
        {
            XrPollFuture = (delegate* unmanaged[Cdecl]<ulong, in XrFuturePollInfo, out XrFuturePollResult, XrResult>)LocateNativeFunction("xrPollFutureEXT");
            XrCancelFuture = (delegate* unmanaged[Cdecl]<ulong, in XrFutureCancelInfo, XrResult>)LocateNativeFunction("xrCancelFutureEXT");
        }

        internal bool PollFuture(in ulong future, out XrFutureState outputState)
        {
            outputState = XrFutureState.Pending;
            XrResult resultCode;

            return PollFuture(future, out outputState, out resultCode);
        }

        internal bool PollFuture(in ulong future, out XrFutureState outputState, out XrResult pollResult)
        {
            outputState = XrFutureState.Pending;
            pollResult = (XrResult)Utils.MLXrResult.FuturePendingEXT;
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

        internal XrResult CancelFuture(in ulong future)
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

    internal enum XrFutureState : uint
    {
        Pending = 1,
        Ready
    }
}
