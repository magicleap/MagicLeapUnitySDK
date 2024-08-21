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
using MagicLeap.OpenXR.NativeDelegates;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.MagicLeap.Unsafe;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.NativeTypes;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace MagicLeap.OpenXR.Features.Reprojection
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Reprojection", 
        Desc = "Enables the application to provide additional reprojection information to the projection composition layer", 
        Company = "Magic Leap", 
        Version = "1.0.0", 
        FeatureId = FeatureId, 
        BuildTargetGroups = new[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone }, 
        OpenxrExtensionStrings = ReprojectionExtensions
    )]
#endif

    public unsafe class MagicLeapReprojectionFeature : MagicLeapOpenXRFeatureWithInterception<MagicLeapReprojectionFeature>
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_reprojection";
        private const string ReprojectionExtensions = "XR_MSFT_composition_layer_reprojection";

        private XrCompositionLayerReprojectionInfo* compositionLayerReprojectionInfo;
        private XrCompositionLayerReprojectionPlaneOverride* compositionLayerReprojectionPlaneOverride;

        private MagicLeapReprojectionNativeFunctions reprojectionNativeFunctions;
        private XrReprojectionMode[] reprojectionModes = Array.Empty<XrReprojectionMode>();
        
        public enum ReprojectionMode
        {
            Depth = 1,
            PlanarFromDepth,
            PlanarManual,
            OrientationOnly
        }

        public bool IsReprojectionModeSupported(ReprojectionMode reprojectionMode) => reprojectionModes.Contains((XrReprojectionMode)reprojectionMode);

        private bool enableReprojection;
        public bool EnableReprojection
        {
            get => enableReprojection;
            set
            {
                enableReprojection = value;
                shouldAppendPlaneInfo = false;
                UpdateReprojectionChain();
            }
        }

        private bool shouldAppendPlaneInfo;

        public void SetReprojectionMode(ReprojectionMode reprojectionMode)
        {
            var internalReprojectionMode = (XrReprojectionMode)reprojectionMode;
            compositionLayerReprojectionInfo->ReprojectionMode = internalReprojectionMode;
            UpdateReprojectionChain();
        }

        public void SetReprojectionPlaneInfo(Vector3 position, Vector3 normal, Vector3 velocity)
        {
            compositionLayerReprojectionPlaneOverride->Normal = normal.ConvertBetweenUnityOpenXr();
            compositionLayerReprojectionPlaneOverride->Position = position.ConvertBetweenUnityOpenXr();
            compositionLayerReprojectionPlaneOverride->Velocity = velocity.ConvertBetweenUnityOpenXr();
            shouldAppendPlaneInfo = true;
            UpdateReprojectionChain();
        }

        public void ResetPlaneInfo()
        {
            shouldAppendPlaneInfo = false;
            UpdateReprojectionChain();
        }

        private void UpdateReprojectionChain()
        {
            if (shouldAppendPlaneInfo)
            {
                compositionLayerReprojectionInfo->Next = new IntPtr(compositionLayerReprojectionPlaneOverride);
            }
            else
            {
                compositionLayerReprojectionInfo->Next = IntPtr.Zero;
            }
        }


        protected override void MarkFunctionsToIntercept()
        {
            InterceptEndFrame = true;
        }

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            var result = base.OnInstanceCreate(xrInstance);
            if (!result)
            {
                return false;
            }

            if (!OpenXRRuntime.IsExtensionEnabled(ReprojectionExtensions))
            {
                return false;
            }

            reprojectionNativeFunctions = CreateNativeFunctions<MagicLeapReprojectionNativeFunctions>();
            var xrResult = reprojectionNativeFunctions.GetSystemId(out var systemId);
            if (!Utils.DidXrCallSucceed(xrResult, nameof(reprojectionNativeFunctions.GetSystemId)))
            {
                return false;
            }
            
            xrResult = reprojectionNativeFunctions.GetSupportedReprojectionModes(AppInstance, systemId, out reprojectionModes);
            if (!Utils.DidXrCallSucceed(xrResult, nameof(reprojectionNativeFunctions.GetSupportedReprojectionModes)))
            {
                return false;
            }
            AllocateOrFreeResources(true);
            return true;
        }

        protected override void OnInstanceDestroy(ulong xrInstance)
        {
            base.OnInstanceDestroy(xrInstance);
            AllocateOrFreeResources(false);
        }

        private void AllocateOrFreeResources(bool shouldAllocate)
        {
            if (shouldAllocate)
            {
                compositionLayerReprojectionInfo = UnsafeUtilityEx.CallocTracked<XrCompositionLayerReprojectionInfo>(Allocator.Persistent);
                compositionLayerReprojectionInfo->Initialize();
                compositionLayerReprojectionPlaneOverride = UnsafeUtilityEx.CallocTracked<XrCompositionLayerReprojectionPlaneOverride>(Allocator.Persistent);
                compositionLayerReprojectionPlaneOverride->Initialize();
                compositionLayerReprojectionInfo->Next = new IntPtr(compositionLayerReprojectionPlaneOverride);
            }
            else
            {
                UnsafeUtility.FreeTracked(compositionLayerReprojectionInfo, Allocator.Persistent);
                UnsafeUtility.FreeTracked(compositionLayerReprojectionPlaneOverride, Allocator.Persistent);
            }
        }

        internal override XrResult OnEndFrame(ulong session, XrFrameEndInfo* frameEndInfo, XrEndFrame origEndFrame)
        {
            if (!EnableReprojection || compositionLayerReprojectionInfo->ReprojectionMode == XrReprojectionMode.None)
            {
                return base.OnEndFrame(session, frameEndInfo, origEndFrame);
            }
            
            frameEndInfo->AppendToProjectionLayer(new IntPtr(compositionLayerReprojectionInfo), 100, true);
            return base.OnEndFrame(session, frameEndInfo, origEndFrame);
        }
        
#if UNITY_EDITOR
        protected override void GetValidationChecks(List<ValidationRule> rules, BuildTargetGroup targetGroup)
        {
            rules.Add(Utils.GetDepthSubmissionValidationRule(this, targetGroup));
            base.GetValidationChecks(rules, targetGroup);
        }
#endif
    }
}
