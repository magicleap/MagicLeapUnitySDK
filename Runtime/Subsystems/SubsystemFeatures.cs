// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.MagicLeap
{
    static class SubsystemFeatures
    {
        const Feature kUniversallySupportedFeatures = Feature.AnyTrackingMode
            | Feature.WorldFacingCamera
            | Feature.PlaneTracking;

        // Features rank in order of power usage typically
        static readonly ConfigurationDescriptor[] s_ConfigurationsDescriptors = {
            new ConfigurationDescriptor(IntPtr.Zero, kUniversallySupportedFeatures | Feature.Meshing, 0),
            new ConfigurationDescriptor((IntPtr)1, kUniversallySupportedFeatures | Feature.PointCloud, 1)
        };

        public static Feature requestedFeatures { get; private set; } = Feature.None;

        public static void SetFeatureRequested(Feature feature, bool value)
        {
            requestedFeatures = requestedFeatures.SetEnabled(feature, value);
        }

        public static Feature currentFeatures { get; private set; } = Feature.None;

        public static void SetCurrentFeatureEnabled(Feature feature, bool value)
        {
            currentFeatures = currentFeatures.SetEnabled(feature, value);
        }

        public static unsafe NativeArray<ConfigurationDescriptor> AcquireConfigurationDescriptors(Allocator allocator)
            => new NativeArray<ConfigurationDescriptor>(s_ConfigurationsDescriptors, allocator);
    }
}
