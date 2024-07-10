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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapOpenXRNativeDelegates;
using UnityEngine.XR.OpenXR.NativeTypes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features")]
    public abstract class MagicLeapOpenXRFeatureBase : OpenXRFeature
    {
        internal ulong AppSession { get; private set; } = Utils.XrConstants.NullHandle;
        internal ulong AppInstance { get; private set; } = Utils.XrConstants.NullHandle;
        internal ulong AppSpace { get; private set; } = Utils.XrConstants.NullHandle;

        internal XrSessionState SessionState
        {
            get;
            private set;
        }

        internal static long PredictedDisplayTime;

        internal long NextPredictedDisplayTime => PredictedDisplayTime;

        internal XrGetInstanceProcAddr InstanceProcAddr;
        

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            var result = base.OnInstanceCreate(xrInstance);
            if (!result)
            {
                return false;
            }
            
            AppInstance = xrInstance;
            InstanceProcAddr = Marshal.GetDelegateForFunctionPointer<XrGetInstanceProcAddr>(xrGetInstanceProcAddr);
            return true;
        }
        

        protected override void OnSessionCreate(ulong xrSession)
        {
            base.OnSessionCreate(xrSession);
            AppSession = xrSession;
        }

        protected override void OnSessionStateChange(int oldState, int newState)
        {
            base.OnSessionStateChange(oldState, newState);
            SessionState = (XrSessionState)newState;
        }

        protected override void OnSessionBegin(ulong xrSession)
        {
            base.OnSessionBegin(xrSession);
            AppSession = xrSession;
        }

        protected override void OnSessionDestroy(ulong xrSession)
        {
            base.OnSessionDestroy(xrSession);
            AppSession = 0u;
        }

        protected override void OnAppSpaceChange(ulong xrSpace)
        {
            base.OnAppSpaceChange(xrSpace);
            AppSpace = xrSpace;
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
        
        internal T CreateNativeFunctions<T>() where T : MagicLeapNativeFunctionsBase, new()
        {
            return MagicLeapNativeFunctionsBase.Create<T>(InstanceProcAddr, AppInstance);
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
}
