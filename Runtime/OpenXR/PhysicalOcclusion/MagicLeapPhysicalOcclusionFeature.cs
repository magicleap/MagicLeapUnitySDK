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
using MagicLeap.OpenXR.NativeDelegates;
using MagicLeap.OpenXR.SystemInfo;
using MagicLeap.OpenXR.ViewConfiguration;
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

namespace MagicLeap.OpenXR.Features.PhysicalOcclusion
{
    /// <summary>
    ///     Enables the Magic Leap OpenXR Loader for Android, and modifies the AndroidManifest to be compatible with ML2.
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Physical Occlusion", 
        Desc = "Necessary to deploy a Magic Leap 2 compatible application with physical world occlusion", 
        Company = "Magic Leap", 
        Version = "1.0.0", 
        FeatureId = FeatureId, 
        BuildTargetGroups = new[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone }, 
        OpenxrExtensionStrings = OcclusionExtensionName
    )]
#endif
    public unsafe class MagicLeapPhysicalOcclusionFeature : MagicLeapOpenXRFeatureWithInterception<MagicLeapPhysicalOcclusionFeature>
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_physical_occlusion";
        private const string OcclusionExtensionName = "XR_ML_physical_world_occlusion";

        protected override bool UsesExperimentalExtensions => true;
        
        /// <summary>
        /// The Occlusion sources to use for physical occlusion
        /// </summary>
        [Flags]
        public enum OcclusionSource
        {
            Environment = 1 << 1,
            DepthSensor = 1 << 2,
            Hands = 1 << 3,
            Controller = 1 << 4,
        }
        
        private OcclusionSource enabledOcclusionSources;
        
        /// <summary>
        /// Occlusion Sources to enable
        /// </summary>
        public OcclusionSource EnabledOcclusionSource
        {
            get => enabledOcclusionSources;
            set
            {
                enabledOcclusionSources = value;
                UpdateOcclusionSources();
            }
        }
        
        /// <summary>
        /// Whether to enable or disable occlusion
        /// </summary>
        public bool EnableOcclusion
        {
            get;
            set;
        }
        
        /// <summary>
        /// The near range of the depth sensor, if the depth sensor is used as an occlusion source
        /// </summary>
        public float DepthSensorNearRange
        {
            get => sourcesHolder->DepthSensor.NearRange;
            set
            {
                var clampedValue = Mathf.Clamp(value, occlusionPropertiesHolder.DepthSensorProperties.MinNearRange, occlusionPropertiesHolder.DepthSensorProperties.MaxNearRange);
                sourcesHolder->DepthSensor.NearRange = clampedValue;
            }
        }
        
        /// <summary>
        /// The far range of the depth sensor, if the depth sensor is used as an occlusion source
        /// </summary>
        public float DepthSensorFarRange
        {
            get => sourcesHolder->DepthSensor.FarRange;
            set
            {
                var clampedValue = Mathf.Clamp(value, occlusionPropertiesHolder.DepthSensorProperties.MinFarRange, occlusionPropertiesHolder.DepthSensorProperties.MaxFarRange);
                sourcesHolder->DepthSensor.FarRange = clampedValue;
            }
        }

        private XrPhysicalOcclusionPropertiesHolder occlusionPropertiesHolder;
        private ViewConfigNativeFunctions viewConfigNativeFunctions;
        private SystemInfoNativeFunctions systemInfoNativeFunctions;

        private XrPhysicalOcclusionSourcesHolder* sourcesHolder;
        private NativeList<IntPtr> occlusionSources;
        private XrCompositionLayerPhysicalWorldOcclusion* physicalWorldOcclusionCompositionLayer;
        
        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            var creationResult =  base.OnInstanceCreate(xrInstance);
            if (!OpenXRRuntime.IsExtensionEnabled(OcclusionExtensionName))
            {
                Debug.LogError($"{OcclusionExtensionName} extension was not enabled");
                return false;
            }
            if (!creationResult)
            {
                return false;
            }

            viewConfigNativeFunctions = CreateNativeFunctions<ViewConfigNativeFunctions>();
            systemInfoNativeFunctions = CreateNativeFunctions<SystemInfoNativeFunctions>();

            var xrResult = systemInfoNativeFunctions.GetSystemId(out var systemId);
            if (!Utils.DidXrCallSucceed(xrResult, nameof(systemInfoNativeFunctions.GetSystemId)))
            {
                return false;
            }

            Utils.OpenXRStructHelpers<XrViewConfigurationProperties>.Create(StructType.ViewConfigurationProperties, out var viewConfigurationProperties);
            var propertiesHolder = XrPhysicalOcclusionPropertiesHolder.Create();
            viewConfigurationProperties.Next = new IntPtr(&propertiesHolder.ControllerProperties);
            propertiesHolder.ControllerProperties.Next = new IntPtr(&propertiesHolder.EnvironmentProperties);
            propertiesHolder.EnvironmentProperties.Next = new IntPtr(&propertiesHolder.HandsProperties);
            propertiesHolder.HandsProperties.Next = new IntPtr(&propertiesHolder.DepthSensorProperties);

            xrResult = viewConfigNativeFunctions.GetViewConfigurationProperties(AppInstance, systemId, ref viewConfigurationProperties);
            if (!Utils.DidXrCallSucceed(xrResult, nameof(viewConfigNativeFunctions.GetViewConfigurationProperties)))
            {
                return false;
            }
            occlusionPropertiesHolder = propertiesHolder;
            AllocateOrFreeResources(true);
            return true;
        }

        private void UpdateOcclusionSources()
        {
            occlusionSources.Clear();
            foreach (var occlusionSource in (OcclusionSource[])Enum.GetValues(typeof(OcclusionSource)))
            {
                if (enabledOcclusionSources.HasFlag(occlusionSource))
                {
                    switch (occlusionSource)
                    {
                        case OcclusionSource.Environment:
                            occlusionSources.Add(new IntPtr(&sourcesHolder->Environment));
                            break;
                        case OcclusionSource.DepthSensor:
                            occlusionSources.Add(new IntPtr(&sourcesHolder->DepthSensor));
                            break;
                        case OcclusionSource.Hands:
                            occlusionSources.Add(new IntPtr(&sourcesHolder->Hands));
                            break;
                        case OcclusionSource.Controller:
                            occlusionSources.Add(new IntPtr(&sourcesHolder->Controller));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            physicalWorldOcclusionCompositionLayer->OcclusionSourceCount = (uint)occlusionSources.Length;
            physicalWorldOcclusionCompositionLayer->OcclusionSources = occlusionSources.GetUnsafePtr();
        }

        protected override void OnInstanceDestroy(ulong xrInstance)
        {
            base.OnInstanceDestroy(xrInstance);
            AllocateOrFreeResources(false);
        }

        protected override void MarkFunctionsToIntercept()
        {
            InterceptEndFrame = true;
        }

        private void AllocateOrFreeResources(bool shouldAllocate)
        {
            if (shouldAllocate)
            {
                occlusionSources = new NativeList<IntPtr>(4, Allocator.Persistent);
                sourcesHolder = UnsafeUtilityEx.CallocTracked<XrPhysicalOcclusionSourcesHolder>(Allocator.Persistent);
                sourcesHolder->Initialize(in occlusionPropertiesHolder);
                physicalWorldOcclusionCompositionLayer = UnsafeUtilityEx.CallocTracked<XrCompositionLayerPhysicalWorldOcclusion>(Allocator.Persistent);
                physicalWorldOcclusionCompositionLayer->Type = XrPhysicalOcclusionStructTypes.XRTypeCompositionLayerPhysicalWorldOcclusion;
            }
            else
            {
                occlusionSources.Dispose();
                UnsafeUtility.FreeTracked(sourcesHolder, Allocator.Persistent);
                UnsafeUtility.FreeTracked(physicalWorldOcclusionCompositionLayer, Allocator.Persistent);
            }
        }
        
        /// <summary>
        /// Checks if an occlusion source is supported
        /// </summary>
        /// <param name="source">The occlusion source to check</param>
        /// <returns>True if the occlusion source is supported</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws exception if an invalid occlusion source is passed</exception>
        public bool IsOcclusionSourceSupported(OcclusionSource source)
        {
            return source switch
            {
                OcclusionSource.Environment => occlusionPropertiesHolder.EnvironmentProperties.SupportsOcclusion,
                OcclusionSource.DepthSensor => occlusionPropertiesHolder.DepthSensorProperties.SupportsOcclusion,
                OcclusionSource.Hands => occlusionPropertiesHolder.HandsProperties.SupportsOcclusion,
                OcclusionSource.Controller => occlusionPropertiesHolder.ControllerProperties.SupportsOcclusion,
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, "Invalid Occlusion Source")
            };
        }
        
        /// <summary>
        /// Get the near and far range ranges for the depth sensor occlusion
        /// </summary>
        /// <returns>A tuple of named-tuple pairs for the near-range and the far-range values</returns>
        public ((float min, float max) nearRange, (float min, float max) farRange) GetDepthSensorProperties()
        {
            ref var depthSensorProperties = ref occlusionPropertiesHolder.DepthSensorProperties;
            return ((depthSensorProperties.MinNearRange, depthSensorProperties.MaxNearRange), (depthSensorProperties.MinFarRange, depthSensorProperties.MaxFarRange));
        }
        
        internal override XrResult OnEndFrame(ulong session, XrFrameEndInfo* frameEndInfo, XrEndFrame origEndFrame)
        {
            if (EnableOcclusion && occlusionSources.Length > 0)
            {
                frameEndInfo->AppendToProjectionLayer(new IntPtr(physicalWorldOcclusionCompositionLayer), includeSecondaryViews: true);
            }
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
