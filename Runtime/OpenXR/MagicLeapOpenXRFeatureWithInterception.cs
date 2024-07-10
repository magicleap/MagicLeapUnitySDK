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
using System.Runtime.InteropServices;
using AOT;
using MagicLeap.OpenXR.NativeDelegates;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Features
{
    internal struct MagicLeapOpenXRFeatureInterceptor<TDelegateType>
    {
        internal bool ShouldIntercept;
        internal TDelegateType OriginalDelegate;
        internal TDelegateType InterceptedDelegate;

        internal void AssignFromPtr(ref IntPtr pointer)
        {
            if (!ShouldIntercept)
            {
                return;
            }

            OriginalDelegate = Marshal.GetDelegateForFunctionPointer<TDelegateType>(pointer);
            pointer = Marshal.GetFunctionPointerForDelegate(InterceptedDelegate);
        }
    }

    public abstract unsafe class MagicLeapOpenXRFeatureWithInterception<T> : MagicLeapOpenXRFeatureBase where T : MagicLeapOpenXRFeatureBase
    {
        private static MagicLeapOpenXRFeatureInterceptor<XrGetInstanceProcAddr> InterceptorHookInstanceProcAddr;
        private static MagicLeapOpenXRFeatureInterceptor<XrPollEvent> InterceptorPollEvent;
        private static MagicLeapOpenXRFeatureInterceptor<XrBeginFrame> InterceptorBeginFrame;
        private static MagicLeapOpenXRFeatureInterceptor<XrEndFrame> InterceptorEndFrame;
        private static MagicLeapOpenXRFeatureInterceptor<XrWaitFrame> InterceptorWaitFrame;

        private delegate XrResult FeaturePollEvent(ulong instance, IntPtr eventBuffer, XrPollEvent origPollEvent);
        private delegate XrResult FeatureBeginFrame(ulong session, XrFrameBeginInfo* beginFrameInfo, XrBeginFrame origBeginFrame);
        private delegate XrResult FeatureEndFrame(ulong session, XrFrameEndInfo* endFrameInfo, XrEndFrame origEndFrame);
        private delegate XrResult FeatureWaitFrame(ulong session, XrFrameWaitInfo* waitFrameInfo, XrFrameState* frameState, XrWaitFrame origWaitFrame);

        private static FeaturePollEvent PollEventImpl;
        private static FeatureBeginFrame BeginFrameImpl;
        private static FeatureEndFrame EndFrameImpl;
        private static FeatureWaitFrame WaitFrameImpl;
        
        internal ref bool InterceptPollEvent => ref InterceptorPollEvent.ShouldIntercept;
        internal ref bool InterceptBeginFrame => ref InterceptorBeginFrame.ShouldIntercept;
        internal ref bool InterceptWaitFrame => ref InterceptorWaitFrame.ShouldIntercept;
        internal ref bool InterceptEndFrame => ref InterceptorEndFrame.ShouldIntercept;
        
        protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
        {
            MarkFunctionsToIntercept();
            var orig = base.HookGetInstanceProcAddr(func);
            
            InterceptorHookInstanceProcAddr.ShouldIntercept = true;
            InterceptorHookInstanceProcAddr.InterceptedDelegate = GetInstanceProcAddr;
            InterceptorHookInstanceProcAddr.OriginalDelegate = Marshal.GetDelegateForFunctionPointer<XrGetInstanceProcAddr>(orig);

            return Marshal.GetFunctionPointerForDelegate(InterceptorHookInstanceProcAddr.InterceptedDelegate);
        }

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            var result = base.OnInstanceCreate(xrInstance);
            if (!result)
            {
                return false;
            }
            
            PollEventImpl += OnPollEvent;
            BeginFrameImpl += OnBeginFrame;
            EndFrameImpl += OnEndFrame;
            WaitFrameImpl += OnWaitFrame;
            return true;
        }

        protected override void OnInstanceDestroy(ulong xrInstance)
        {
            base.OnInstanceDestroy(xrInstance);
            
            PollEventImpl -= OnPollEvent;
            BeginFrameImpl -= OnBeginFrame;
            EndFrameImpl -= OnEndFrame;
            WaitFrameImpl -= OnWaitFrame;
        }

        protected abstract void MarkFunctionsToIntercept();
        
        [MonoPInvokeCallback(typeof(XrGetInstanceProcAddr))]
        private static XrResult GetInstanceProcAddr(ulong instance, [MarshalAs(UnmanagedType.LPStr)] string functionName, ref IntPtr funcAddr)
        {
            var result = InterceptorHookInstanceProcAddr.OriginalDelegate(instance, functionName, ref funcAddr);
            switch (functionName)
            {
                case "xrBeginFrame":
                {
                    InterceptorBeginFrame.InterceptedDelegate = BeginFrame;
                    InterceptorBeginFrame.AssignFromPtr(ref funcAddr);
                    break;
                }

                case "xrEndFrame":
                {
                    InterceptorEndFrame.InterceptedDelegate = EndFrame;
                    InterceptorEndFrame.AssignFromPtr(ref funcAddr);
                    break;
                }

                case "xrWaitFrame":
                {
                    InterceptorWaitFrame.InterceptedDelegate = WaitFrame;
                    InterceptorWaitFrame.AssignFromPtr(ref funcAddr);
                    break;
                }

                case "xrPollEvent":
                {
                    InterceptorPollEvent.InterceptedDelegate = PollEvent;
                    InterceptorPollEvent.AssignFromPtr(ref funcAddr);
                    break;
                }
            }
            return result;
        }
        
        [MonoPInvokeCallback(typeof(XrGetInstanceProcAddr))]
        private static XrResult PollEvent(ulong instance, IntPtr eventBuffer)
        {
            return PollEventImpl?.Invoke(instance, eventBuffer, InterceptorPollEvent.OriginalDelegate) ?? InterceptorPollEvent.OriginalDelegate(instance, eventBuffer);
        }

        [MonoPInvokeCallback(typeof(XrGetInstanceProcAddr))]
        private static XrResult BeginFrame(ulong session, XrFrameBeginInfo* frameBeginInfo)
        {
            return BeginFrameImpl?.Invoke(session, frameBeginInfo, InterceptorBeginFrame.OriginalDelegate) ?? InterceptorBeginFrame.OriginalDelegate(session, frameBeginInfo);
        }
        
        [MonoPInvokeCallback(typeof(XrGetInstanceProcAddr))]
        private static XrResult WaitFrame(ulong session, XrFrameWaitInfo* frameWaitInfo, XrFrameState* frameState)
        {
            return WaitFrameImpl?.Invoke(session, frameWaitInfo, frameState, InterceptorWaitFrame.OriginalDelegate) ?? InterceptorWaitFrame.OriginalDelegate(session, frameWaitInfo, frameState);
        }

        [MonoPInvokeCallback(typeof(XrGetInstanceProcAddr))]
        private static XrResult EndFrame(ulong session, XrFrameEndInfo* frameEndInfo)
        {
            return EndFrameImpl?.Invoke(session, frameEndInfo, InterceptorEndFrame.OriginalDelegate) ?? InterceptorEndFrame.OriginalDelegate(session, frameEndInfo);
        }
        
        internal virtual XrResult OnPollEvent(ulong instance, IntPtr eventBuffer, XrPollEvent origPollEvent)
        {
            return origPollEvent(instance, eventBuffer);
        }

        internal virtual XrResult OnBeginFrame(ulong session, XrFrameBeginInfo* beginFrameInfo, XrBeginFrame origBeginFrame)
        {
            return origBeginFrame(session, beginFrameInfo);
        }

        internal virtual XrResult OnWaitFrame(ulong session, XrFrameWaitInfo* frameWaitInfo, XrFrameState* frameState, XrWaitFrame origWaitFrame)
        {
            return origWaitFrame(session, frameWaitInfo, frameState);
        }

        internal virtual XrResult OnEndFrame(ulong session, XrFrameEndInfo* frameEndInfo, XrEndFrame origEndFrame)
        {
            return origEndFrame(session, frameEndInfo);
        }
    }
}
