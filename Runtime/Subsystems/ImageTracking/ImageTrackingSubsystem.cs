using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// Subsystem for Image Tracking
    /// TODO: Implement when Image Tracking is available from the platform
    /// </summary>
    [Preserve]
    public sealed class ImageTrackingSubsystem : XRImageTrackingSubsystem
    {

#if !UNITY_2020_2_OR_NEWER
        /// <summary>
        /// Create the generic XR Provider object from the magic leap Provider
        /// </summary>
        /// <returns>A Provider</returns>
        protected override Provider CreateProvider() => new MagicLeapProvider();
#endif

        class MagicLeapProvider : Provider
        {
            internal static bool IsSubsystemStateValid()
            {
                // TODO: Implement when Image Tracking is available from the platform
                return false;
            }

            /// <summary>
            /// Allows the user to re-request privileges
            /// </summary>
            /// <returns>
            /// <c>true</c> if the Color Camera privileges were granted and <c>false</c> otherwise.
            /// </returns>
            public bool RequestPermissionIfNecessary()
            {
                // TODO: Implement when Image Tracking is available from the platform
                return false;
            }

            public MagicLeapProvider()
            {
                // TODO: Implement when Image Tracking is available from the platform
            }

#if UNITY_2020_2_OR_NEWER
            public override void Start() { }
            public override void Stop() { }
#endif

            /// <summary>
            /// Destroy the image tracking subsystem.
            /// </summary>
            public override void Destroy()
            {
                // TODO: Implement when Image Tracking is available from the platform
            }

            /// <summary>
            /// The current <c>RuntimeReferenceImageLibrary</c>.  If <c>null</c> then the subsystem will be set to "off".
            /// </summary>
            public override RuntimeReferenceImageLibrary imageLibrary
            {
                set
                {
                    // TODO: Implement when Image Tracking is available from the platform
                }
            }

            public unsafe override TrackableChanges<XRTrackedImage> GetChanges(XRTrackedImage defaultTrackedImage, Allocator allocator)
            {
                // TODO: Implement when Image Tracking is available from the platform
                return default(TrackableChanges<XRTrackedImage>);
            }

            /// <summary>
            /// Stores the requested maximum number of concurrently tracked moving images.
            /// </summary>
            /// <remarks>
            /// Magic Leap Image Tracking has the ability to set an enforcement policy on the maximum number of tracked
            /// moving images.  If the policy has been explicitly set to <c>false</c> by using
            /// <see cref="ImageTrackingSubsystem.SetAutomaticImageStationarySettingsEnforcementPolicy"/> then
            /// this value will not be honored by the native provider until the policy is set to <c>true</c>.
            /// </remarks>
            public override int requestedMaxNumberOfMovingImages
            {
                get => m_RequestedNumberOfMovingImages;
                set
                {
                    m_RequestedNumberOfMovingImages = value;
                }
            }
            int m_RequestedNumberOfMovingImages = 25;

            /// <summary>
            /// Stores the current maximum number of moving images that can be tracked by the native platform.
            /// </summary>
            /// <remarks>
            /// Magic Leap Image Tracking has the ability to set an enforcement policy on the maximum number of tracked
            /// moving images.  If the policy has been explicitly set to <c>false</c> by using
            /// <see cref="ImageTrackingSubsystem.SetAutomaticImageStationarySettingsEnforcementPolicy"/> then
            /// this value will indicate the current number of explicitly declared moving images in the current library
            /// otherwise it will return the same value as <see cref="requestedMaxNumberOfMovingImages"/>.
            /// </remarks>
            public override int currentMaxNumberOfMovingImages => m_RequestedNumberOfMovingImages;

            /// <summary>
            /// Creates a <c>RuntimeReferenceImageLibrary</c> from the passed in <c>XRReferenceImageLibrary</c> passed in.
            /// </summary>
            /// <param name="serializedLibrary">The <c>XRReferenceImageLibrary</c> that is used to create the <c>RuntimeReferenceImageLibrary</c></param>
            /// <returns>A new <c>RuntimeReferenceImageLibrary</c> created from the old  </returns>
            public override RuntimeReferenceImageLibrary CreateRuntimeLibrary(XRReferenceImageLibrary serializedLibrary)
            {
                // TODO: Implement when Image Tracking is available from the platform
                return null;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
            XRImageTrackingSubsystemDescriptor.Create(new XRImageTrackingSubsystemDescriptor.Cinfo
            {
                id = MagicLeapXrProvider.ImageTrackingSubsystemId,
#if UNITY_2020_2_OR_NEWER
                providerType = typeof(MagicLeapProvider),
                subsystemTypeOverride = typeof(ImageTrackingSubsystem),
#else
                subsystemImplementationType = typeof(ImageTrackingSubsystem),
#endif
                // TODO: Update when Image Tracking is available from the platform
                supportsMovingImages = false,
                supportsMutableLibrary = false
            });
        }
    }
}
