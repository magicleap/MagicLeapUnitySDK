// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
#if UNITY_OPENXR_1_9_0_OR_NEWER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapOpenXRFeatureNativeTypes;
using UnityEngine.XR.OpenXR.NativeTypes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public abstract class MagicLeapOpenXRFeatureBase : OpenXRFeature
    {
        internal ulong XRSession { get; private set; }
        internal ulong XRInstance { get; private set; }
        internal ulong XRSpace { get; private set; }
        internal long NextPredictedDisplayTime => MagicLeapFeature.NativeBindings.MLOpenXRGetNextPredictedDisplayTime();
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal delegate XrResult XrPollEvent(ulong instance, IntPtr eventBuffer);

        protected virtual string GetFeatureId()
        {
            return "";
        }


        internal GetInstanceProcAddr InstanceProcAddr;

        protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
        {
            var featureId = GetFeatureId();
            if (string.IsNullOrEmpty(featureId))
            {
                return base.HookGetInstanceProcAddr(func);
            }
            return MagicLeapFeature.NativeBindings.MLOpenXRInterceptFunctionsForFeature(featureId, func);
        }

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            var result = base.OnInstanceCreate(xrInstance);
            if (!result)
            {
                return false;
            }
            
            XRInstance = xrInstance;
            InstanceProcAddr = Marshal.GetDelegateForFunctionPointer<GetInstanceProcAddr>(xrGetInstanceProcAddr);
            
            var featureId = GetFeatureId();
            if (string.IsNullOrEmpty(featureId))
            {
                return true;
            }
            MagicLeapFeature.NativeBindings.MLOpenXROnFeatureInstanceCreate(featureId, xrInstance, xrGetInstanceProcAddr);
            return true;
        }

        protected override void OnInstanceDestroy(ulong xrInstance)
        {
            base.OnInstanceDestroy(xrInstance);
            var featureId = GetFeatureId();
            if (string.IsNullOrEmpty(featureId))
            {
                return;
            }
            MagicLeapFeature.NativeBindings.MLOpenXRFeatureOnInstanceDestroy(featureId, xrInstance);
        }

        protected override void OnSessionCreate(ulong xrSession)
        {
            base.OnSessionCreate(xrSession);
            XRSession = xrSession;
            var featureId = GetFeatureId();
            if (string.IsNullOrEmpty(featureId))
            {
                return;
            }
            MagicLeapFeature.NativeBindings.MLOpenXROnFeatureSessionCreate(featureId, xrSession);
        }

        protected override void OnSessionBegin(ulong xrSession)
        {
            base.OnSessionBegin(xrSession);
            XRSession = xrSession;
        }
        
        protected override void OnAppSpaceChange(ulong xrSpace)
        {
            base.OnAppSpaceChange(xrSpace);
            XRSpace = xrSpace;
            var featureId = GetFeatureId();
            if (string.IsNullOrEmpty(featureId))
            {
                return;
            }
            MagicLeapFeature.NativeBindings.MLOpenXROnFeatureAppSpaceChange(featureId, xrSpace);
        }
        
        protected void CheckEnabledExtension(string extensionName, bool required = false)
        {
            if (OpenXRRuntime.IsExtensionEnabled(extensionName))
                return;

            if (required)
                throw new Exception($"Required OpenXR extension '{extensionName}' was not enabled!");
            
            Debug.LogWarning($"OpenXR extension '{extensionName}' was not enabled!");
        }
        
        protected virtual IEnumerable<Type> DependsOn => Enumerable.Empty<Type>();

        public bool GetUnityPose(ulong space, out Pose pose)
        {
            pose = default;
            var featureId = GetFeatureId();
            return !string.IsNullOrEmpty(featureId) && MagicLeapFeature.NativeBindings.MLOpenXRGetUnityPoseForFeature(featureId, space, out pose);
        }

        internal T CreateNativeFunctions<T>() where T : MagicLeapNativeFunctionsBase, new()
        {
            return MagicLeapNativeFunctionsBase.Create<T>(InstanceProcAddr, XRInstance);
        }

#if UNITY_EDITOR
        protected override void GetValidationChecks(List<ValidationRule> rules, BuildTargetGroup targetGroup)
        {
            base.GetValidationChecks(rules, targetGroup);
            
            rules.Add(GetDependencyRule<MagicLeapFeature>(targetGroup));
            
            foreach (var depends in DependsOn)
            {
                rules.Add(GetDependencyRule(targetGroup, depends));
            }
        }

        protected ValidationRule GetDependencyRule(BuildTargetGroup group, Type featureType)
            => new ValidationRule(this)
            {
                checkPredicate = ()=> Utils.IsFeatureEnabled(group, featureType),
                fixIt = () => Utils.TryEnableFeature(group, featureType),
                error = true,
                message = $"{Utils.GetNiceTypeName(GetType())} depends on the {Utils.GetNiceTypeName(featureType)}, which is not enabled. Please enable this feature"
            };
        
        protected ValidationRule GetDependencyRule<TFeature>(BuildTargetGroup group) where TFeature : OpenXRFeature
            => new ValidationRule(this)
            {
                checkPredicate = ()=> Utils.IsFeatureEnabled<TFeature>(group),
                fixIt = () => Utils.TryEnableFeature<TFeature>(group),
                error = true,
                message = $"{Utils.GetNiceTypeName(GetType())} depends on the {Utils.GetNiceTypeName<TFeature>()}, which is not enabled. Please enable this feature"
            };
        
#endif
    }
    
    /// <summary>
    ///  The abstract base class of OpenXR features that needs to intercept OpenXR functions
    /// <para>Note: The static fields in generics are shared based on the type (T). This way each feature gets its own set of interception pointers and callbacks </para>
    /// </summary>
    /// <typeparam name="T">The type of the feature implemented</typeparam>
    public abstract class MagicLeapOpenXRFeatureWithEvents<T> : MagicLeapOpenXRFeatureBase where T : MagicLeapOpenXRFeatureBase
    {
        private static GetInstanceProcAddr OrigProcAddr;
        private static GetInstanceProcAddr InterceptedProcAddr;
        private static XrPollEvent OrigPollEvent;
        private static XrPollEvent InterceptedPollEvent;
        
        private static event Action<IntPtr> OnPollEventReceived;
        
        protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
        {
            var orig = base.HookGetInstanceProcAddr(func);
            OrigProcAddr = Marshal.GetDelegateForFunctionPointer<GetInstanceProcAddr>(orig);
            InterceptedProcAddr = InterceptedInstanceProcAddr;
            OnPollEventReceived += OnPollEvent;
            return Marshal.GetFunctionPointerForDelegate(InterceptedProcAddr);
        }
        
        [MonoPInvokeCallback(typeof(GetInstanceProcAddr))]
        private static XrResult InterceptedInstanceProcAddr(ulong instance, [MarshalAs(UnmanagedType.LPStr)] string functionName, ref IntPtr pointer)
        {
            var result = OrigProcAddr(instance, functionName, ref pointer);
            if (functionName != "xrPollEvent")
            {
                return result;
            }
            Debug.Log($"{typeof(T)} intercepted xrPollEvent");
            OrigPollEvent = Marshal.GetDelegateForFunctionPointer<XrPollEvent>(pointer);
            InterceptedPollEvent = PollEventImpl;
            pointer = Marshal.GetFunctionPointerForDelegate(InterceptedPollEvent);
            return result;
        }
        
        [MonoPInvokeCallback(typeof(XrPollEvent))]
        private static XrResult PollEventImpl(ulong instance, IntPtr eventBuffer)
        {
            var result = OrigPollEvent(instance, eventBuffer);
            if (result != XrResult.Success)
            {
                return result;
            }
            OnPollEventReceived?.Invoke(eventBuffer);
            return result;
        }
        
        internal virtual void OnPollEvent(IntPtr eventBuffer)
        {
        }

        protected override void OnInstanceDestroy(ulong xrInstance)
        {
            base.OnInstanceDestroy(xrInstance);
            OnPollEventReceived -= OnPollEvent;
        }
    }
}
#endif
