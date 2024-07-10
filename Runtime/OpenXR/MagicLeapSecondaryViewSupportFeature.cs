// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif
using System;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Features
{
    internal struct XrSecondaryViewConfigurationStateMSFT
    {
        public XrStructureType Type;
        public IntPtr Next;
        public XrViewConfigurationType ViewConfigurationType;
        public XrBool32 Active;
    }

    internal unsafe struct XrSecondaryViewConfigurationFrameStateMSFT
    {
        public XrStructureType Type;
        public IntPtr Next;
        public uint ViewConfigurationCount;
        public XrSecondaryViewConfigurationStateMSFT* ViewConfigurationStates;
    }

    public static class MLXrSecondaryViewState
    {
        public static bool IsActive { get; internal set; }
    }

#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Secondary View",
        Desc= "Support for Secondary View Configuration on the Magic Leap 2. Use to improve capture quality.",
        Company = "Magic Leap",
        Version = "1.0.0",
        BuildTargetGroups = new []{ BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        FeatureId = FeatureId,
        OpenxrExtensionStrings = ExtensionNames
    )]
#endif
    public class MagicLeapSecondaryViewSupportFeature : MagicLeapOpenXRFeatureBase
    {
        public const string FeatureId = "com.magicleap.openxr.feature.secondary_view_configuration";
        public const string ExtensionNames = "XR_MSFT_secondary_view_configuration XR_MSFT_first_person_observer";

        private bool secondaryViewActive;

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            foreach (var extName in ExtensionNames.Split(' '))
            {
                if (!OpenXRRuntime.IsExtensionEnabled(extName))
                {
                    Debug.LogError($"{extName} is not enabled. Disabling {nameof(MagicLeapSecondaryViewSupportFeature)}");
                    return false;
                }
            }
            var result = base.OnInstanceCreate(xrInstance);
            return result;
        }
    }
}
