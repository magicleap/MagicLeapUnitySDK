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
using MagicLeap.OpenXR.Constants;
using MagicLeap.OpenXR.NativeDelegates;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.NativeTypes;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif
namespace MagicLeap.OpenXR.Features
{
    public abstract class MagicLeapOpenXRInteractionFeatureBase : OpenXRInteractionFeature
    {
        internal XrSession AppSession { get; private set; } = Values.NullHandle;
        internal XrInstance AppInstance { get; private set; } = Values.NullHandle;
        internal XrSpace AppSpace { get; private set; } = Values.NullHandle;

        internal XrSessionState SessionState
        {
            get;
            private set;
        }

        internal XrGetInstanceProcAddr InstanceProcAddr;

        /// <summary>
        /// Are any of the extensions used by this feature NOT yet included in the OpenXR Spec (1.0 or 1.1)?
        /// </summary>
        protected virtual bool UsesExperimentalExtensions { get; } = false;

        protected abstract string LayoutName { get; }

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

        protected override void UnregisterDeviceLayout()
        {
            base.UnregisterDeviceLayout();

            UnityEngine.InputSystem.InputSystem.RemoveLayout(LayoutName);
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

        internal T CreateNativeFunctions<T>() where T : NativeFunctionsBase, new()
        {
            return NativeFunctionsBase.Create<T>(InstanceProcAddr, AppInstance);
        }

#if UNITY_EDITOR
        internal string DisplayName
        {
            get
            {
                var attr = GetFeatureAttribute();
                if (attr == null)
                {
                    return Utils.GetNiceTypeName(GetType());
                }
                return attr.UiName;
            }
        }

        internal OpenXRFeatureAttribute GetFeatureAttribute() =>
            (OpenXRFeatureAttribute)Attribute.GetCustomAttribute(GetType(), typeof(OpenXRFeatureAttribute));

        protected override void GetValidationChecks(List<ValidationRule> rules, BuildTargetGroup targetGroup)
        {
            base.GetValidationChecks(rules, targetGroup);
            
            rules.Add(GetDependencyRule<MagicLeapFeature>(targetGroup));
            rules.Add(ExperimentalStatus);
            
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
                message = $"{DisplayName} depends on Feature \"{Utils.GetNiceTypeName(featureType)}\", which is not enabled."
            };
        
        protected ValidationRule GetDependencyRule<TFeature>(BuildTargetGroup group) where TFeature : OpenXRFeature
            => new ValidationRule(this)
            {
                checkPredicate = ()=> Utils.IsFeatureEnabled<TFeature>(group),
                fixIt = () => Utils.TryEnableFeature<TFeature>(group),
                error = true,
                message = $"{DisplayName} depends on Feature \"{Utils.GetNiceTypeName<TFeature>()}\", which is not enabled."
            };

        protected ValidationRule ExperimentalStatus =>
            new ValidationRule(this)
            {
                checkPredicate = ()=> !UsesExperimentalExtensions,
                error = false,
                message = $"NOTE: The OpenXR Feature \"{DisplayName}\" relies on one or more experimental extensions not part of the OpenXR Specification."
            };        
#endif
    }
}
