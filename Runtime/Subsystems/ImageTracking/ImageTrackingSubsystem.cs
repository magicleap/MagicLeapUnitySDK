using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Lumin;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap.Internal;
#if UNITY_MAGICLEAP || UNITY_ANDROID
using UnityEngine.XR.MagicLeap.Native;
#endif

namespace UnityEngine.XR.MagicLeap
{
    using MLLog = UnityEngine.XR.MagicLeap.MagicLeapLogger;

    [Preserve]
    [UsesLuminPrivilege("CameraCapture")]
    public sealed class ImageTrackingSubsystem : XRImageTrackingSubsystem
    {
        public class Extensions
        {
            private static List<XRImageTrackingSubsystem> imgTrackingSubsystems = new List<XRImageTrackingSubsystem>();

            internal static ImageTrackingSubsystem GetMLSubsystem()
            {
                SubsystemManager.GetSubsystems<XRImageTrackingSubsystem>(imgTrackingSubsystems);

                if (imgTrackingSubsystems.Count > 0)
                {
                    foreach (var subsystem in imgTrackingSubsystems)
                    {
                        var mlImgSubsystem = subsystem as ImageTrackingSubsystem;
                        if (mlImgSubsystem != null)
                            return mlImgSubsystem;
                    }
                }

                return null;
            }

            internal static MagicLeapProvider GetMLProvider()
            {
                var mlSubsystem = GetMLSubsystem();
                if (mlSubsystem != null)
                    return mlSubsystem.provider as MagicLeapProvider;
                return null;
            }

            public static MLResult UpdateImageTarget(XRReferenceImage referenceImage, bool isEnabled, bool isStationary)
            {
                var mlProvider = GetMLProvider();
                if (mlProvider != null)
                {
                    if (mlProvider.imgDB == null)
                        return MLResult.Create(MLResult.Code.UnspecifiedFailure, "Image Database is null.");
                    return mlProvider.imgDB.UpdateImageTarget(referenceImage, isEnabled, isStationary);
                }

                return MLResult.Create(MLResult.Code.UnspecifiedFailure, "Could not find provider.");
            }

            public static bool EnforceMaxMovingImages
            {
                get
                {
                    var mlProvider = GetMLProvider();
                    if (mlProvider != null)
                    {
                        if (mlProvider.imgDB != null)
                            return mlProvider.imgDB.EnforceMovingImagesMax;
                    }

                    return false;
                }
                set
                {
                    var mlProvider = GetMLProvider();
                    if (mlProvider != null)
                    {
                        if (mlProvider.imgDB != null)
                            mlProvider.imgDB.EnforceMovingImagesMax = value;
                    }
                }
            }

            public static int MaxMovingImages
            {
                get
                {
                    var mlProvider = GetMLProvider();
                    if (mlProvider != null)
                    {
                        if (mlProvider.imgDB != null)
                            return mlProvider.imgDB.MaxMovingImages;
                    }

                    return 0;
                }
                set
                {
                    var mlProvider = GetMLProvider();
                    if (mlProvider != null)
                    {
                        if (mlProvider.imgDB != null)
                            mlProvider.imgDB.MaxMovingImages = value;
                    }
                }
            }


            /// <summary>
            /// Represents the settings of an Image Target.
            /// All fields are required for an Image Target to be tracked.
            /// </summary>
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public struct MLImageTrackerTargetSettings
            {
                /// <summary>
                /// Name of the target.
                /// This name has to be unique across all targets added to the Image Tracker.
                /// Caller should allocate memory for this.
                /// Encoding should be in UTF8.
                /// This will be copied internally.
                /// </summary>
                public string Name;

                /// <summary>
                /// LongerDimension refer to the size of the longer dimension of the physical Image.
                /// Target in Unity scene units.
                /// </summary>
                public float LongerDimension;

                /// <summary>
                /// Set this to \c true to improve detection for stationary targets.
                /// An Image Target is a stationary target if its position in the physical world does not change.
                /// This is best suited for cases where the target is stationary and when the local geometry (environment surrounding the target) is planar.
                /// When in doubt set this to false.
                /// </summary>
                [MarshalAs(UnmanagedType.I1)]
                public bool IsStationary;

                /// <summary>
                /// Set this to \c true to track the image target.
                /// Disabling the target when not needed marginally improves the tracker CPU performance.
                /// This is best suited for cases where the target is temporarily not needed.
                /// If the target no longer needs to be tracked it is best to use remove the target.
                /// </summary>
                [MarshalAs(UnmanagedType.I1)]
                public bool IsEnabled;
            }
        }

        internal ulong handle => MagicLeapProvider.trackerHandle;

        const string kLogTag = LuminXrProvider.ImageTrackingSubsystemId;

        [Conditional("DEVELOPMENT_BUILD")]
        static void DebugError(string msg) => LogError(msg);

        static void LogWarning(string msg) => MLLog.Warning(kLogTag, $"{msg}");
        static void LogError(string msg) => MLLog.Error(kLogTag, $"{msg}");
        static void Log(string msg) => MLLog.Debug(kLogTag, $"{msg}");

        internal static readonly string k_StreamingAssetsPath = Path.Combine(Application.streamingAssetsPath, "MLImageLibraries");
        internal static readonly string k_ImageTrackingDependencyPath = Path.Combine("Library", "MLImageTracking");

        internal static string GetDatabaseFilePathFromLibrary(XRReferenceImageLibrary library) => Path.Combine(k_StreamingAssetsPath, $"{library.name}_{library.guid}.imgpak");

#if !UNITY_2020_2_OR_NEWER
        protected override Provider CreateProvider() => new MagicLeapProvider();
#endif

        /// <summary>
        /// Checks to see whether the native provider is valid and whether permission has been granted.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the native provider has been instantiated and has a valid native resource.
        /// </returns>
        /// <remarks>
        /// There are a number of reasons for false.  Either permissions were denied or the device experienced
        /// and internal error and was not able to create the native tracking resource.  Should the latter be
        /// the case, native error logs will have more information.
        /// </remarks>
        public bool IsValid() => MagicLeapProvider.IsSubsystemStateValid();

        /// <summary>
        /// The <see cref="JobHandle"/> that refers to the native tracker handle creation job.
        /// </summary>
        /// <remarks>
        /// The creation of the native image tracker handle that enables image tracking on
        /// Lumin devices has an average startup time of anywhere between ~1500ms - ~6000ms
        /// depending on the state of the device and is blocking.  This subsystem opts to
        /// perform this operation asynchronously because of this.
        /// </remarks>
        public static JobHandle nativeTrackerCreationJobHandle => MagicLeapProvider.s_NativeTrackerCreationJobHandle;

        internal class MagicLeapProvider : Provider
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            internal static ulong trackerHandle = MagicLeapNativeBindings.InvalidHandle;
#else
            internal static ulong trackerHandle = 0;
#endif
            internal static IntPtr s_NativeProviderPtr = IntPtr.Zero;
            internal static JobHandle s_NativeTrackerCreationJobHandle = default(JobHandle);

            internal ImageDatabase imgDB;

            private XRTrackedImage[] currentDatabaseImages = new XRTrackedImage[0];
            private XRTrackedImage[] previousDatabaseImages = new XRTrackedImage[0];

            private List<XRTrackedImage> addedImages = new List<XRTrackedImage>();
            private List<XRTrackedImage> updatedImages = new List<XRTrackedImage>();
            private List<TrackableId> removedTrackedImageIds = new List<TrackableId>();

            private HashSet<TrackableId> currentTrackedImageIds = new HashSet<TrackableId>();
            private HashSet<TrackableId> previousTrackedImageIds = new HashSet<TrackableId>();

            internal static bool IsSubsystemStateValid()
            {
                return !s_NativeTrackerCreationJobHandle.Equals(default(JobHandle))
                    && s_NativeTrackerCreationJobHandle.IsCompleted;
            }

            // This job is used to gate the creation of image libraries while also bypassing
            // the 6000ms wait time it takes to start a tracking system.  The job handle is then
            // passed with the handle to allow other asynchronous jobs to use it as a dependency.
            struct CreateNativeImageTrackerJob : IJob
            {

                public void Execute()
                {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                    // moved to Start() - SDKUNITY-5063
#endif
                }
            }

            /// <summary>
            /// Allows the user to re-request permissions
            /// </summary>
            /// <returns>
            /// <c>true</c> if the Color Camera permissions were granted and <c>false</c> otherwise.
            /// </returns>
            public bool RequestPermissionIfNecessary()
            {
                return false;
                /*
                Permission request causing rendering bug, don't request for now. 
                if (Android.Permission.HasUserAuthorizedPermission("android.permission.CAMERA"))
                {
                    return true;
                }
                else
                {
                    Android.Permission.RequestUserPermission("android.permission.CAMERA");
                    return Android.Permission.HasUserAuthorizedPermission("android.permission.CAMERA");
                }
                */
            }

            public MagicLeapProvider()
            {
                if (RequestPermissionIfNecessary())
                {
                    if (s_NativeTrackerCreationJobHandle.Equals(default(JobHandle)))
                    {
                        // see SDKUNITY-5063
                        //s_NativeTrackerCreationJobHandle = new CreateNativeImageTrackerJob().Schedule();
                    }
                }
                else
                {
                    LogWarning($"Could not start the image tracking subsystem because permissions were denied.");
                }
            }

#if UNITY_2020_2_OR_NEWER
            public override void Start()
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                var trackerSettingsNative = MLImageTracker.NativeBindings.MLImageTrackerSettingsNative.Create();
                MLResult.Code resultCode = MLImageTracker.NativeBindings.MLImageTrackerInitSettings(ref trackerSettingsNative);

                resultCode = MLImageTracker.NativeBindings.MLImageTrackerCreate(ref trackerSettingsNative, ref MagicLeapProvider.trackerHandle);
                if (!MLResult.IsOK(resultCode))
                {
                    LogError($"Unable to create native tracker due to internal device error.  Subsystem will be set to invalid.  See native output for more details.");
                }
#endif
            }

            public override void Stop() { }
#endif

            /// <summary>
            /// Destroy the image tracking subsystem.
            /// </summary>
            public override void Destroy()
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                if (MagicLeapNativeBindings.MLHandleIsValid(trackerHandle))
                {
                    s_NativeTrackerCreationJobHandle = default(JobHandle);
                }
                var resultCode = MLImageTracker.NativeBindings.MLImageTrackerDestroy(trackerHandle);
#endif
            }

            /// <summary>
            /// The current <c>RuntimeReferenceImageLibrary</c>.  If <c>null</c> then the subsystem will be set to "off".
            /// </summary>
            public override RuntimeReferenceImageLibrary imageLibrary
            {
                set
                {
                    if (RequestPermissionIfNecessary())
                    {
                        if (value == null)
                        {
                            SubsystemFeatures.SetFeatureRequested(Feature.ImageTracking, false);
                        }
                        else if (value is ImageDatabase database)
                        {
                            SubsystemFeatures.SetFeatureRequested(Feature.ImageTracking, true);
                            // reset lists and images
                        }
                        else
                        {
                            throw new ArgumentException($"The {value.GetType().Name} is not a valid Magic Leap image library.");
                        }
                    }
                }
            }

            public unsafe override TrackableChanges<XRTrackedImage> GetChanges(XRTrackedImage defaultTrackedImage, Allocator allocator)
            {
                if (!IsSubsystemStateValid())
                    return default(TrackableChanges<XRTrackedImage>);

                addedImages.Clear();
                updatedImages.Clear();
                removedTrackedImageIds.Clear();
                currentTrackedImageIds.Clear();

                if (imgDB.Changed)
                {
                    foreach (var img in previousDatabaseImages)
                    {
                        if (!previousTrackedImageIds.Contains(img.trackableId))
                        {
                            removedTrackedImageIds.Add(img.trackableId);
                        }
                    }

                    imgDB.Changed = false;
                    previousDatabaseImages = currentDatabaseImages;
                    currentDatabaseImages = imgDB.TrackedImagesArray;
                }
#if UNITY_MAGICLEAP || UNITY_ANDROID
                for (int i = 0; i < currentDatabaseImages.Length; ++i)
                {
                    XRTrackedImage trackedImage = currentDatabaseImages[i];
                    var nativeTrackingResult = new MLImageTracker.NativeBindings.MLImageTrackerTargetResultNative();
                    var resultCode = MLImageTracker.NativeBindings.MLImageTrackerGetTargetResult(MagicLeapProvider.trackerHandle, trackedImage.trackableId.subId1, ref nativeTrackingResult);

                    if (MLResult.IsOK(resultCode))
                    {
                        TrackingState state = TrackingState.None;
                        switch (nativeTrackingResult.Status)
                        {
                            case MLImageTracker.Target.TrackingStatus.NotTracked: state = TrackingState.None; break;
                            case MLImageTracker.Target.TrackingStatus.Tracked: state = TrackingState.Tracking; break;
                            case MLImageTracker.Target.TrackingStatus.Unreliable: state = TrackingState.Limited; break;
                        }

                        var nativeStaticData = new MLImageTracker.NativeBindings.MLImageTrackerTargetStaticDataNative();
                        resultCode = MLImageTracker.NativeBindings.MLImageTrackerGetTargetStaticData(MagicLeapProvider.trackerHandle, trackedImage.trackableId.subId1, ref nativeStaticData);

                        resultCode = LuminXrProviderNativeBindings.GetUnityPose(nativeStaticData.CoordFrameTarget, out Pose pose);

                        trackedImage = new XRTrackedImage(trackedImage.trackableId, trackedImage.sourceImageId, pose, trackedImage.size, state, IntPtr.Zero);
                        if (previousTrackedImageIds.Contains(trackedImage.trackableId))
                        {
                            updatedImages.Add(trackedImage);
                        }
                        else
                        {
                            addedImages.Add(trackedImage);
                        }

                        currentTrackedImageIds.Add(trackedImage.trackableId);
                    }
                    else if (!previousTrackedImageIds.Contains(trackedImage.trackableId) && !removedTrackedImageIds.Contains(trackedImage.trackableId))
                    {
                        removedTrackedImageIds.Add(trackedImage.trackableId);
                    }
                }
#endif

                previousTrackedImageIds.Clear();
                foreach (var id in currentTrackedImageIds)
                {
                    previousTrackedImageIds.Add(id);
                }

                var added = new NativeArray<XRTrackedImage>(addedImages.ToArray(), allocator);
                var updated = new NativeArray<XRTrackedImage>(updatedImages.ToArray(), allocator);
                var removed = new NativeArray<TrackableId>(removedTrackedImageIds.ToArray(), allocator);

                return TrackableChanges<XRTrackedImage>.CopyFrom(added, updated, removed, allocator);
            }

            /// <summary>
            /// Stores the requested maximum number of concurrently tracked moving images.
            /// </summary>
            public override int requestedMaxNumberOfMovingImages
            {
                get
                {
                    if (this.imgDB != null)
                        return this.imgDB.MaxMovingImages;
                    return 0;
                }
                set
                {
                    if (this.imgDB != null)
                        this.imgDB.MaxMovingImages = value;
                }
            }

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
            public override int currentMaxNumberOfMovingImages
            {
                get
                {
                    var mlProvider = Extensions.GetMLProvider();
                    if (mlProvider != null)
                    {
                        if (mlProvider.imgDB != null)
                            return mlProvider.imgDB.MaxMovingImages;
                    }

                    return 0;
                }
            }

            /// <summary>
            /// Creates a <c>RuntimeReferenceImageLibrary</c> from the passed in <c>XRReferenceImageLibrary</c> passed in.
            /// </summary>
            /// <param name="serializedLibrary">The <c>XRReferenceImageLibrary</c> that is used to create the <c>RuntimeReferenceImageLibrary</c></param>
            /// <returns>A new <c>RuntimeReferenceImageLibrary</c> created from the old  </returns>
            public override RuntimeReferenceImageLibrary CreateRuntimeLibrary(XRReferenceImageLibrary serializedLibrary)
            {
                if (s_NativeTrackerCreationJobHandle.Equals(default(JobHandle)))
                {
                    return null;
                }

#if UNITY_MAGICLEAP || UNITY_ANDROID
                foreach (var image in currentDatabaseImages)
                {
                    var resultCode = MLImageTracker.NativeBindings.MLImageTrackerRemoveTarget(trackerHandle, image.trackableId.subId1);
                }
#endif

                this.imgDB = new ImageDatabase(serializedLibrary, s_NativeTrackerCreationJobHandle);
                this.imgDB.MaxMovingImages = 25;
                this.imageLibrary = imgDB;
                previousDatabaseImages = currentDatabaseImages;
                currentDatabaseImages = imgDB.TrackedImagesArray;
                return imgDB;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            XRImageTrackingSubsystemDescriptor.Create(new XRImageTrackingSubsystemDescriptor.Cinfo
            {
                id = LuminXrProvider.ImageTrackingSubsystemId,
#if UNITY_2020_2_OR_NEWER
                providerType = typeof(ImageTrackingSubsystem.MagicLeapProvider),
                subsystemTypeOverride = typeof(ImageTrackingSubsystem),
#else
                subsystemImplementationType = typeof(ImageTrackingSubsystem),
#endif
                supportsMovingImages = true,
                supportsMutableLibrary = true
            });
#endif 
        }
    }
}
