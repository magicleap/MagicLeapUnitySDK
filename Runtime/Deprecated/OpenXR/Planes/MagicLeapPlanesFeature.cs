// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.Planes")]
    public class MagicLeapPlanesFeature : MagicLeapOpenXRFeatureBase
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_planes";
        private const string PlaneExtensionName = "XR_EXT_plane_detection";
        
        private readonly List<XRPlaneSubsystemDescriptor> planeSubsystemDescriptors = new();
        
        internal MagicLeapPlanesNativeFunctions PlanesNativeFunctions;

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            if (OpenXRRuntime.IsExtensionEnabled(PlaneExtensionName))
            {
                var instanceCreateResult = base.OnInstanceCreate(xrInstance);
                if (instanceCreateResult)
                {
                    MLXrPlaneSubsystem.RegisterDescriptor();
                }

                PlanesNativeFunctions = CreateNativeFunctions<MagicLeapPlanesNativeFunctions>();
                return instanceCreateResult;
            }
            Debug.LogError($"{PlaneExtensionName} is not enabled. Disabling {nameof(MagicLeapPlanesFeature)}");
            return false;
        }

        protected override void OnSubsystemCreate()
        {
            base.OnSubsystemCreate();
            CreateSubsystem<XRPlaneSubsystemDescriptor, XRPlaneSubsystem>(planeSubsystemDescriptors, MagicLeapXrProvider.PlanesSubsystemId);
        }

        protected override void OnSubsystemStart()
        {
            base.OnSubsystemStart();
            StartSubsystem<XRPlaneSubsystem>();
        }

        protected override void OnSubsystemStop()
        {
            base.OnSubsystemStop();
            StopSubsystem<XRPlaneSubsystem>();
        }

        protected override void OnSubsystemDestroy()
        {
            base.OnSubsystemDestroy();
            DestroySubsystem<XRPlaneSubsystem>();
        }
    }
}
