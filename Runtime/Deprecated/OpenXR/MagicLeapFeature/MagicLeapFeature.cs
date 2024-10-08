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
using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine.LowLevel;
using MagicLeap;
using UnityEngine.XR.MagicLeap.Native;
using UnityEngine.XR.OpenXR.NativeTypes;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    using MagicLeapConvertTimeSpecNativeTypes;
    using MagicLeapViewConfigurationNativeTypes;
    using NativeInterop;
    using MagicLeapOpenXRNativeDelegates;
    using MagicLeapRenderingFeatureNativeTypes;

    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features")]
    public partial class MagicLeapFeature : MagicLeapOpenXRFeatureWithInterception<MagicLeapFeature>
    {
        private struct MLPerceptionSnapshotUpdate { }

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
            /// Do not restrict the Camera's near clip plane distance. 
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
        private bool perceptionSnapshots;

        [SerializeField]
        [Tooltip("Determines if the far clipping plane should be clamped, and to what maximum value.")]
        private FarClipMode farClipPolicy = FarClipMode.Recommended;

        [SerializeField]
        [Tooltip("Determines the minimum value the near clipping plane will be clamped to.")]
        private NearClipMode nearClipPolicy = NearClipMode.Recommended;

        public FarClipMode FarClipPolicy => farClipPolicy;
        public NearClipMode NearClipPolicy => nearClipPolicy;

		private IntPtr mlSnapshot = IntPtr.Zero;

        public float MinNearZ => viewConfigurationDepthRange.MinNearZ;
        public float RecommendedNearZ => viewConfigurationDepthRange.RecommendedNearZ;
        public float MaxFarZ => viewConfigurationDepthRange.MaxFarZ;
        public float RecommendedFarZ => viewConfigurationDepthRange.RecommendedFarZ;

        //This is used when DISABLE_MAGICLEAP_CLIP_ENFORCEMENT flag is toggled, for use with NearClipMode.None
        // Unity doesn't like its camera nearClip going below 0.01 and will lock up if it does. 
        private const float MinimumNearClip = 0.01f;

        private static readonly List<XRSessionSubsystemDescriptor> SessionSubsystemDesc = new();

        private XrSessionState sessionState = XrSessionState.Unknown;
        private XrGetInstanceProcAddr getInstanceProcAddr;

        private MagicLeapConvertTimeSpecNativeFunctions timeSpecNativeFunctions;

        private MagicLeapSystemInfoNativeFunctions systemInfoNativeFunctions;
        private MagicLeapViewConfigurationNativeFunctions magicLeapViewConfigurationNativeFunctions;
        private XrViewConfigurationDepthRange viewConfigurationDepthRange;
        private PlayerLoopSystem snapshotUpdatePlayerLoop;
        
        private FeatureLifecycleNativeListener lifecycleNativeListener;

        protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
        {
            NativeBindings.MLOpenXRGetLifecycleListener(ref lifecycleNativeListener);
            return base.HookGetInstanceProcAddr(func);
        }

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            var result  = base.OnInstanceCreate(xrInstance);
            if (!result)
            {
                return false;
            }
            getInstanceProcAddr = Marshal.GetDelegateForFunctionPointer<XrGetInstanceProcAddr>(xrGetInstanceProcAddr);
            timeSpecNativeFunctions = MagicLeapNativeFunctionsBase.Create<MagicLeapConvertTimeSpecNativeFunctions>(getInstanceProcAddr, xrInstance);
            magicLeapViewConfigurationNativeFunctions = MagicLeapNativeFunctionsBase.Create<MagicLeapViewConfigurationNativeFunctions>(getInstanceProcAddr, xrInstance);
            systemInfoNativeFunctions = MagicLeapNativeFunctionsBase.Create<MagicLeapSystemInfoNativeFunctions>(getInstanceProcAddr, xrInstance);
            EnumerateViewConfigurationViews();
            lifecycleNativeListener.InstanceCreated(xrInstance, xrGetInstanceProcAddr);
            return true;
        }

        internal override unsafe XrResult OnWaitFrame(ulong session, XrFrameWaitInfo* frameWaitInfo, XrFrameState* frameState, XrWaitFrame origWaitFrame)
        {
            var result = base.OnWaitFrame(session, frameWaitInfo, frameState, origWaitFrame);
            if (result != XrResult.Success)
            {
                return result;
            }

            if (frameState->PredictedDisplayPeriod != Utils.XrConstants.InfiniteDuration)
            {
                Interlocked.Exchange(ref PredictedDisplayTime, (frameState->PredictedDisplayTime + (long)frameState->PredictedDisplayPeriod));
            }
            else
            {
                Interlocked.Exchange(ref PredictedDisplayTime, frameState->PredictedDisplayTime);
            }
            lifecycleNativeListener.PredictedDisplayTimeChanged(PredictedDisplayTime);
            return result;
        }

        private void EnumerateViewConfigurationViews()
        {
            var xrResult = systemInfoNativeFunctions.GetSystemId(out var systemId);
            if (!Utils.DidXrCallSucceed(xrResult, nameof(systemInfoNativeFunctions.GetSystemId)))
            {
                return;
            }
            
            xrResult = magicLeapViewConfigurationNativeFunctions.EnumerateConfigurationViews(AppInstance, systemId, out viewConfigurationDepthRange);
            Utils.DidXrCallSucceed(xrResult, nameof(magicLeapViewConfigurationNativeFunctions.EnumerateConfigurationViews));
        }

        protected override void OnInstanceDestroy(ulong xrInstance)
        {
            base.OnInstanceDestroy(xrInstance);
            lifecycleNativeListener.InstanceDestroyed(xrInstance);
        }

        protected override void MarkFunctionsToIntercept()
        {
            InterceptWaitFrame = true;
        }

        protected override void OnAppSpaceChange(ulong xrSpace)
        {
            base.OnAppSpaceChange(xrSpace);
            lifecycleNativeListener.AppSpaceChanged(xrSpace);
        }

        protected override void OnSessionCreate(ulong xrSession)
        {
            base.OnSessionCreate(xrSession);
            lifecycleNativeListener.SessionCreated(xrSession);
            if (Application.isEditor)
            {
                return;
            }

            if (!perceptionSnapshots)
            {
                return;
            }

            snapshotUpdatePlayerLoop = new PlayerLoopSystem()
            {
                subSystemList = Array.Empty<PlayerLoopSystem>(),
                updateDelegate = PerformMLPerceptionSnapshot,
                type = typeof(MLPerceptionSnapshotUpdate)
            };
            var playerLoop = LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            if (!PlayerLoopUtil.InstallIntoPlayerLoop(ref playerLoop, snapshotUpdatePlayerLoop, PlayerLoopUtil.InstallPath))
                Debug.LogError("Unable to install snapshotting Update delegate into player loop!");
            else
                LowLevel.PlayerLoop.SetPlayerLoop(playerLoop);
        }

        protected override void OnSessionBegin(ulong xrSession)
        {
            base.OnSessionBegin(xrSession);
            if (!Application.isEditor)
            {
                Application.onBeforeRender += EnforceClippingPlanes;
            }
        }

        protected override void OnSessionEnd(ulong xrSession)
        {
            base.OnSessionEnd(xrSession);
            if (!Application.isEditor)
            {
                Application.onBeforeRender -= EnforceClippingPlanes;
            }
        }

        protected override void OnSessionDestroy(ulong xrSession)
        {
            base.OnSessionDestroy(xrSession);
            lifecycleNativeListener.SessionDestroyed(xrSession);
            if (Application.isEditor)
            {
                return;
            }

            if (!perceptionSnapshots)
            {
                return;
            }

            var playerLoop = LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopUtil.RemoveFromPlayerLoop(ref playerLoop, snapshotUpdatePlayerLoop, PlayerLoopUtil.InstallPath);
        }

        protected override void OnSessionStateChange(int oldState, int newState)
        {
            base.OnSessionStateChange(oldState, newState);
            sessionState = (XrSessionState)newState;
        }

        protected override void OnSubsystemCreate()
        {
            base.OnSubsystemCreate();
            
            CreateSubsystem<XRSessionSubsystemDescriptor, XRSessionSubsystem>(SessionSubsystemDesc, MagicLeapXrProvider.SessionSubsystemId);
        }
        
        protected override void OnSubsystemDestroy()
        {
            base.OnSubsystemDestroy();

            DestroySubsystem<XRSessionSubsystem>();
        }

		private void PerformMLPerceptionSnapshot()
        {
            if (!perceptionSnapshots || Application.isEditor || sessionState != XrSessionState.Focused)
                return;

            var result = MLResult.Code.Ok;
            if (mlSnapshot != IntPtr.Zero)
            {
                result = MagicLeapNativeBindings.MLPerceptionReleaseSnapshot(mlSnapshot);
                if(!MLResult.DidNativeCallSucceed(result, nameof(MagicLeapNativeBindings.MLPerceptionReleaseSnapshot)))
                {
                    mlSnapshot = IntPtr.Zero;
                    return;
                }
            }
            result = MagicLeapNativeBindings.MLPerceptionGetSnapshot(ref mlSnapshot);
            MLResult.DidNativeCallSucceed(result, nameof(MagicLeapNativeBindings.MLPerceptionGetSnapshot));
        }

        private void EnforceClippingPlanes() => ApplyToCamera(Camera.main);

        public void SetNearClipPolicy(NearClipMode mode)
        {
            nearClipPolicy = mode;
            EnumerateViewConfigurationViews();
            EnforceClippingPlanes();
        }

        private void ApplyFarClip(ref float zFar)
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
                // above the system recommendation.
                case NearClipMode.Minimum:
                    zNear = Mathf.Max(zNear, MinNearZ);
                    break;
                case NearClipMode.Recommended:
                    zNear = Mathf.Max(zNear, RecommendedNearZ);
                    break;
#if DISABLE_MAGICLEAP_CLIP_ENFORCEMENT
                case NearClipMode.None:
                    zNear = MinimumNearClip;
                    break;
#endif
                default:
                    break;
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

        public long ConvertSystemTimeToXrTime(long systemTime)
        {
            var timeSpec = new TimeSpec
            {
                Seconds = systemTime / 1000000000,
                NanoSeconds = systemTime % 1000000000
            };
            unsafe
            {
                var xrResult = timeSpecNativeFunctions.XrConvertTimeSpecTimeToTime(AppInstance, in timeSpec, out var xrTime);
                Utils.DidXrCallSucceed(xrResult, nameof(timeSpecNativeFunctions.XrConvertTimeSpecTimeToTime));
                return xrTime;
            }
        }

        public long ConvertXrTimeToSystemTime(long xrTime)
        {
            unsafe
            {
                var xrResult = timeSpecNativeFunctions.XrConvertTimeToTimeSpecTime(AppInstance, xrTime, out var xrTimeSpec);
                if (!Utils.DidXrCallSucceed(xrResult, nameof(timeSpecNativeFunctions.XrConvertTimeToTimeSpecTime)))
                {
                    return 0;
                }
                return xrTimeSpec.Seconds * 1000000000 + xrTimeSpec.NanoSeconds;
            }
        }
        
        [Obsolete("MagicLeapFeature.ConvertTimestampToXrTime will be deprecated. Use MagicLeapFeature.ConvertTimeStampToXrTime instead")]
        public static XrTime ConvertTimestampToXrTime(long timestampNs)
        {
            return default;
        }
        
        [Obsolete("MagicLeapFeature.ConvertXrTimeToTimestamp will be deprecated. Use MagicLeapFeature.ConvertXRTimeToTimeNanoseconds instead")]
        public static long ConvertXrTimeToTimestamp(XrTime xrTime)
        {
            return 0;
        }
    }
}
