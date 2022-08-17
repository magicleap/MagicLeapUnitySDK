// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="SubsystemFeatures.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2021 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
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
            | Feature.PlaneTracking
            | Feature.ImageTracking;

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
