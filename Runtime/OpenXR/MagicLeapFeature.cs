// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
#if UNITY_OPENXR_1_9_0_OR_NEWER
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap;
using System;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    /// <summary>
    /// Enables the Magic Leap OpenXR Loader for Android, and modifies the AndroidManifest to be compatible with ML2.
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Support",
        Desc="Necessary to deploy a Magic Leap 2 compatible application.",
        Company = "Magic Leap",
        Version = "1.0.0",
        BuildTargetGroups = new []{ BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        FeatureId = FeatureId,
        OpenxrExtensionStrings = "XR_ML_compat XR_KHR_convert_timespec_time XR_EXT_view_configuration_depth_range"
    )]
#endif
    public partial class MagicLeapFeature : OpenXRFeature
    {
        private struct MLPerceptionSnapshotUpdate { }

        public struct TimeSpec
        {
            public long Seconds;
            public long Nanoseconds;
        }

        public enum FarClipMode : byte
        {
            /// <summary>
            /// Do not restrict the Camera's far clip plane distance.
            /// </summary>
            None,

            /// <summary>
            /// Restrict the Camera's far clip plane distance to no more than the maximum allowed by the device.
            /// </summary>
            Maximum,

            /// <summary>
            /// Restrict the Camera's far clip plane to no more than the distance recommended by Magic Leap.
            /// </summary>
            Recommended,
        }

        public enum NearClipMode : byte
        {
            /// <summary>
            /// Restrict the Camera's near clip plane to no less than the absolute minimum allowed (25cm).
            /// </summary>
            Minimum,

            /// <summary>
            /// Restrict the Camera's near clip plane to no less than the distance configured in the system's settings.
            /// </summary>
            Recommended,
#if DISABLE_MAGICLEAP_CLIP_ENFORCEMENT
            /// <summary>
            /// Unsupported
            /// </summary>
            None,
#endif
        }

        /// <summary>
        /// The feature id string. This is used to give the feature a well known id for reference.
        /// </summary>
        public const string FeatureId = "com.magicleap.openxr.feature.ml2";

        [SerializeField]
        [Tooltip("Should MLPerception snapshots be performed? This is used to support certain legacy ML APIs.")]
        private bool perceptionSnapshots = true;

        [SerializeField]
        [Tooltip("Determines if the far clipping plane should be clamped, and to what maximum value.")]
        private FarClipMode farClipPolicy = FarClipMode.Recommended;

        [SerializeField]
        [Tooltip("Determines the minimum value the near clipping plane will be clamped to.")]
        private NearClipMode nearClipPolicy = NearClipMode.Recommended;

        public FarClipMode FarClipPolicy => farClipPolicy;
        public NearClipMode NearClipPolicy => nearClipPolicy;

		private IntPtr mlSnapshot = IntPtr.Zero;

        public float MinNearZ => NativeBindings.MLOpenXRGetMinNearClippingPlane();
        public float RecommendedNearZ => NativeBindings.MLOpenXRGetRecommendedNearClippingPlane();
        public float MaxFarZ => NativeBindings.MLOpenXRGetMaxFarClippingPlane();
        public float RecommendedFarZ => NativeBindings.MLOpenXRGetRecommendedFarClippingPlane();

        private static List<XRMeshSubsystemDescriptor> meshSubsysDesc = new List<XRMeshSubsystemDescriptor>();
        private static List<XRSessionSubsystemDescriptor> sessionSubsysDesc = new List<XRSessionSubsystemDescriptor>();

        protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
        {
            return NativeBindings.MLOpenXRInterceptFunctions(func);
        }

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            return NativeBindings.MLOpenXROnInstanceCreate(xrGetInstanceProcAddr, xrInstance);
        }

        protected override void OnInstanceDestroy(ulong xrInstance)
        {
            NativeBindings.MLOpenXROnInstanceDestroy(xrInstance);
        }

        protected override void OnAppSpaceChange(ulong xrSpace)
        {
            NativeBindings.MLOpenXROnAppSpaceChange(xrSpace);
        }

        protected override void OnSessionCreate(ulong xrSession)
        {
            NativeBindings.MLOpenXROnSessionCreate(xrSession);

            if (perceptionSnapshots)
            {
                var update = new PlayerLoopSystem()
                {
                    subSystemList = Array.Empty<PlayerLoopSystem>(),
                    updateDelegate = PerformMLPerceptionSnapshot,
                    type = typeof(MLPerceptionSnapshotUpdate)
                };
                var playerLoop = LowLevel.PlayerLoop.GetCurrentPlayerLoop();
                if (!Utils.InstallIntoPlayerLoop(ref playerLoop, update, Utils.InstallPath))
                    Debug.LogError("Unable to install snapshotting Update delegate into player loop!");
                else
                    LowLevel.PlayerLoop.SetPlayerLoop(playerLoop);
            }
        }

        protected override void OnSessionBegin(ulong xrSession)
        {
            base.OnSessionBegin(xrSession);

            Application.onBeforeRender += EnforceClippingPlanes;
        }

        protected override void OnSessionEnd(ulong xrSession)
        {
            base.OnSessionEnd(xrSession);

            Application.onBeforeRender -= EnforceClippingPlanes;
        }

        protected override void OnSessionDestroy(ulong xrSession)
        {
            NativeBindings.MLOpenXROnSessionDestroy(xrSession);
        }

        protected override void OnSessionStateChange(int oldState, int newState)
        {
            NativeBindings.MLOpenXRUpdateDepthRangeValues();
        }

        protected override void OnSubsystemCreate()
        {
            base.OnSubsystemCreate();
            
            CreateSubsystem<XRMeshSubsystemDescriptor, XRMeshSubsystem>(meshSubsysDesc, MagicLeapXrProvider.MeshingSubsystemId);
            CreateSubsystem<XRSessionSubsystemDescriptor, XRSessionSubsystem>(sessionSubsysDesc, MagicLeapXrProvider.SessionSubsystemId);
        }
        
        protected override void OnSubsystemDestroy()
        {
            base.OnSubsystemDestroy();
            DestroySubsystem<XRMeshSubsystem>();
            DestroySubsystem<XRSessionSubsystem>();
        }

        public void StartMeshSubsystem()
        {
            StartSubsystem<XRMeshSubsystem>();
        }

        public void StopMeshSubsystem()
        {
            StopSubsystem<XRMeshSubsystem>();
        }

		private void PerformMLPerceptionSnapshot()
        {
            if (!perceptionSnapshots)
                return;

            var result = MLResult.Code.Ok;
            if (mlSnapshot != IntPtr.Zero)
            {
                result = MagicLeapXrProviderNativeBindings.MLPerceptionReleaseSnapshot(mlSnapshot);
                if(!MLResult.DidNativeCallSucceed(result, nameof(MagicLeapXrProviderNativeBindings.MLPerceptionReleaseSnapshot)))
                {
                    mlSnapshot = IntPtr.Zero;
                    return;
                }
            }
            result = MagicLeapXrProviderNativeBindings.MLPerceptionGetSnapshot(ref mlSnapshot);
            MLResult.DidNativeCallSucceed(result, nameof(MagicLeapXrProviderNativeBindings.MLPerceptionGetSnapshot));
        }

        private void EnforceClippingPlanes() => ApplyToCamera(Camera.main);

        public void ApplyFarClip(ref float zFar)
        {
            switch (farClipPolicy)
            {
                case FarClipMode.Maximum:
                    zFar = Mathf.Min(zFar, Mathf.Min(zFar, MaxFarZ));
                    break;
                case FarClipMode.Recommended:
                    zFar = Mathf.Min(zFar, RecommendedFarZ);
                    break;
                case FarClipMode.None:
                default:
                    break;
            }
        }

        public void ApplyNearClip(ref float zNear)
        {
            switch (nearClipPolicy)
            {
                // Whatever is set in the system settings menu is our new minimum, even if it is
                // above the system recommendation
                case NearClipMode.Minimum:
                    zNear = Mathf.Max(zNear, MinNearZ);
                    break;
                case NearClipMode.Recommended:
                    zNear = Mathf.Max(zNear, RecommendedNearZ);
                    break;
#if DISABLE_MAGICLEAP_CLIP_ENFORCEMENT
                case NearClipMode.None:
                default:
                    break;
#endif
            }
        }

        public void ApplyToCamera(Camera camera, bool warnIfNearClipChanged = true)
        {
            if (!camera)
                return;

            var zFar = camera.farClipPlane;
            var zNear = camera.nearClipPlane;

            ApplyFarClip(ref zFar);
            ApplyNearClip(ref zNear);

            if (warnIfNearClipChanged && zNear > camera.nearClipPlane)
                Debug.LogWarning($"Main Camera's nearClipPlane value is less than the minimum value for this device. Increasing to {zNear}");

            camera.farClipPlane = zFar;
            camera.nearClipPlane = zNear;
        }

        #region Time
        private static MLResult.Code InternalConvertSystemTimeToMLTime(long timestampNs, out MLTime mlTime)
        {
            TimeSpec timeSpec;
            timeSpec.Seconds = timestampNs / 1_000_000_000;
            timeSpec.Nanoseconds = timestampNs - timeSpec.Seconds;

            MLResult.Code resultCode = MLResult.Code.UnspecifiedFailure;

            resultCode = NativeBindings.MLOpenXRConvertTimespecTimeToXrTime(timeSpec, out long nanoseconds);
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLOpenXRConvertTimespecTimeToXrTime)))
            {
                mlTime = new MLTime(nanoseconds);
            }
            else
            {
                mlTime = new MLTime(0);
            }

            return resultCode;

        }

        private static MLResult.Code InternalConvertMLTimeToSystemTime(MLTime mlTime, out long timestampNs)
        {
            var timeSpec = new TimeSpec();
            MLResult.Code resultCode = MLResult.Code.UnspecifiedFailure;
            resultCode = NativeBindings.MLOpenXRConvertXrTimeToTimespecTime(mlTime, out timeSpec);
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLOpenXRConvertXrTimeToTimespecTime)))
            {
                timestampNs = (timeSpec.Seconds * 1000 * 1000 * 1000) + timeSpec.Nanoseconds;
            }
            else
            {
                timestampNs = (long)mlTime;
            }
            return resultCode;
        }

        /// <summary>
        /// Converts a timestamp from a value captured by the system clock to an equivalent XrTime value
        /// </summary>
        /// <param name="timestampNs">The system's monotonic clock timestamp represented in nanoseconds. An example might be <see cref="DateTime.Ticks"/><c>/100</c>.</param>
        /// <param name="xrTime">The system time converted to respective XrTime.</param>
        /// <returns></returns>
        public static MLResult ConvertSystemTimeToXrTime(long timestampNs, out long xrTime)
        {
            var result = MLResult.Create(InternalConvertSystemTimeToMLTime(timestampNs, out MLTime mlTime));
            xrTime = mlTime;
            return result;
        }

        /// <summary>
        /// Converts a timestamp from a value captured by the system clock to an equivalent XrTime value
        /// </summary>
        /// <param name="timeSpec">The system's monotonic clock timestamp represented as a C timespec.</param>
        /// <param name="xrTime">The system time converted to respective XrTime.</param>
        /// <returns></returns>
        public static MLResult ConvertSystemTimeToXrTime(TimeSpec timeSpec, out long xrTime) => MLResult.Create(NativeBindings.MLOpenXRConvertTimespecTimeToXrTime(timeSpec, out xrTime));

        /// <summary>
        /// Converts a timestamp provided by OpenXR to the equivalent monotonic system clock value. 
        /// </summary>
        /// <param name="xrTime">An <c>XrTime</c> value, such as provided by another API, measured in nanoseconds.</param>
        /// <param name="timestampNs">Converted equivalent timestamp, calculated using <c>clock_gettime()</c> with <c>CLOCK_MONOTONIC</c>.</param>
        /// <returns></returns>
        public static MLResult ConvertXrTimeToSystemTime(long xrTime, out long timestampNs) => MLResult.Create(InternalConvertMLTimeToSystemTime((MLTime)xrTime, out timestampNs));

        /// <summary>
        /// Converts a timestamp provided by OpenXR to the equivalent monotonic system clock value. 
        /// </summary>
        /// <param name="xrTime">An <c>XrTime</c> value, such as provided by another API, measured in nanoseconds.</param>
        /// <param name="timeSpec">Converted equivalent timestamp, calculated using <c>clock_gettime()</c> with <c>CLOCK_MONOTONIC</c>.</param>
        /// <returns></returns>
        public static MLResult ConvertXrTimeToSystemTime(long xrTime, out TimeSpec timeSpec) => MLResult.Create(NativeBindings.MLOpenXRConvertXrTimeToTimespecTime(xrTime, out timeSpec));
        #endregion
    }
}
#endif
