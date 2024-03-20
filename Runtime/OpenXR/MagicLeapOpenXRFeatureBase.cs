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

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public abstract class MagicLeapOpenXRFeatureBase : OpenXRFeature
    {
        protected virtual string GetFeatureId()
        {
            return "";
        }
        
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
            var featureId = GetFeatureId();
            if (string.IsNullOrEmpty(featureId))
            {
                return;
            }
            MagicLeapFeature.NativeBindings.MLOpenXROnFeatureSessionCreate(featureId, xrSession);
        }
        
        protected override void OnAppSpaceChange(ulong xrSpace)
        {
            base.OnAppSpaceChange(xrSpace);
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
        
        protected virtual IEnumerable<Type> dependsOn => Enumerable.Empty<Type>();

        public bool GetUnityPose(ulong space, out Pose pose)
        {
            pose = default;
            var featureId = GetFeatureId();
            return !string.IsNullOrEmpty(featureId) && MagicLeapFeature.NativeBindings.MLOpenXRGetUnityPoseForFeature(featureId, space, out pose);
        }

#if UNITY_EDITOR
        protected override void GetValidationChecks(List<ValidationRule> rules, BuildTargetGroup targetGroup)
        {
            base.GetValidationChecks(rules, targetGroup);
            
            rules.Add(GetDependencyRule<MagicLeapFeature>(targetGroup));

            foreach (var depends in dependsOn)
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
#endif
