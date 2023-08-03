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
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;
#if UNITY_XR_MAGICLEAP_PROVIDER
using UnityEngine.XR.MagicLeap.Internal;
#endif
using UnityEngine.XR.Management;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// The Magic Leap implementation of the <c>XRSessionSubsystem</c>. Do not create this directly.
    /// Use <c>SessionSubsystemDescriptor.Create()</c> instead.
    /// </summary>
    [Preserve]
    public sealed class SessionSubsystem : XRSessionSubsystem
    {
#if !UNITY_2020_2_OR_NEWER
        protected override Provider CreateProvider() => new MagicLeapProvider();
#endif

        class MagicLeapProvider : Provider
        {
            public override Promise<SessionAvailability> GetAvailabilityAsync()
            {
                var availability = SessionAvailability.Installed | SessionAvailability.Supported;

                return Promise<SessionAvailability>.CreateResolvedPromise(availability);
            }

            public override TrackingState trackingState
            {
                get
                {
                    var device = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
                    if (device.TryGetFeatureValue(CommonUsages.trackingState, out InputTrackingState inputTrackingState))
                    {
                        if (inputTrackingState == InputTrackingState.None)
                            return TrackingState.None;
                        else if (inputTrackingState == (InputTrackingState.Position | InputTrackingState.Rotation))
                            return TrackingState.Tracking;
                        else
                            return TrackingState.Limited;
                    }
                    else
                    {
                        return TrackingState.None;
                    }
                }
            }

            public override Feature requestedFeatures => SubsystemFeatures.requestedFeatures;

            public override NativeArray<ConfigurationDescriptor> GetConfigurationDescriptors(Allocator allocator)
                => SubsystemFeatures.AcquireConfigurationDescriptors(allocator);

            public override Feature requestedTrackingMode
            {
                get => SubsystemFeatures.requestedFeatures.Intersection(Feature.AnyTrackingMode);
                set
                {
                    SubsystemFeatures.SetFeatureRequested(Feature.AnyTrackingMode, false);
                    SubsystemFeatures.SetFeatureRequested(value, true);
                }
            }

            /// <summary>
            /// The current tracking mode feature flag.
            /// </summary>
            /// <remarks>
            /// Magic Leap will always try to use 6DoF tracking but will automatically switch to
            /// 3DoF if it doesn't have a sufficient tracking environment. This will report which
            /// of the two modes is currently active and
            /// <c>UnityEngine.XR.ARSubsystems.Feature.None</c> otherwise.
            /// </remarks>
            public override Feature currentTrackingMode
            {
                get
                {
                    switch (trackingState)
                    {
                        case TrackingState.Tracking:
                            return Feature.PositionAndRotation;
                        case TrackingState.Limited:
                            return Feature.RotationOnly;
                        default:
                            return Feature.None;
                    }
                }
            }

            private Feature previousConfigurationFeatures = Feature.None;

            public override void Update(XRSessionUpdateParams updateParams, Configuration configuration)
            {
                // Magic Leap supports almost everything working at the same time except Point Clouds and Meshing
                if (configuration.features.HasFlag(Feature.Meshing | Feature.PointCloud))
                {
                    /// TODO (5/26/2020): Move MLSpatialMapper specific features to shared XRMeshSubsystem extensions
                    // Currently, the MeshingSubsystemComponent is required to do PointClouds on magic leap.  So
                    // if meshing is detected at all then simply request a start to the subsystem because it will be
                    // handled either by ARMeshManager or the MeshingSubsystemComponent.
#if UNITY_XR_MAGICLEAP_PROVIDER
                    var loader = (MagicLeapLoader)XRGeneralSettings.Instance.Manager.activeLoader;
                    if (loader.meshSubsystem != null && !loader.meshSubsystem.running)
                    {
                        loader.StartMeshSubsystem();
                    }
#endif
                }
                else if (previousConfigurationFeatures.HasFlag(Feature.Meshing | Feature.PointCloud))
                {
#if UNITY_XR_MAGICLEAP_PROVIDER
                    var loader = (MagicLeapLoader)XRGeneralSettings.Instance.Manager.activeLoader;
                    if (loader.meshSubsystem != null && loader.meshSubsystem.running)
                    {
                        loader.StopMeshSubsystem();
                    }
#endif
                }

                // Only disable and enable features that are managed by the passed in configurations.
                // In some cases features are managed outside of the AR Foundation and explicitly
                // setting all features based on the configuration may override those settings. 
                SubsystemFeatures.SetCurrentFeatureEnabled(previousConfigurationFeatures, false);
                SubsystemFeatures.SetCurrentFeatureEnabled(configuration.features, true);
                previousConfigurationFeatures = configuration.features;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
            XRSessionSubsystemDescriptor.RegisterDescriptor(new XRSessionSubsystemDescriptor.Cinfo
            {
                id = MagicLeapXrProvider.SessionSubsystemId,
#if UNITY_2020_2_OR_NEWER
                providerType = typeof(SessionSubsystem.MagicLeapProvider),
                subsystemTypeOverride = typeof(SessionSubsystem),
#else
                subsystemImplementationType = typeof(SessionSubsystem),
#endif
                supportsInstall = false
            });
        }
    }
}
