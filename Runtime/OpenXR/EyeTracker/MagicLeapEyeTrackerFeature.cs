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
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;
using UnityEngine.XR.OpenXR.Input;
using MagicLeap.OpenXR.NativeDelegates;

#if USE_INPUT_SYSTEM_POSE_CONTROL
using PoseControl = UnityEngine.InputSystem.XR.PoseControl;
#else
using PoseControl = UnityEngine.XR.OpenXR.Input.PoseControl;
#endif

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace MagicLeap.OpenXR.Features.EyeTracker
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Eye Tracker",
        Desc = "Necessary to deploy a Magic Leap 2 compatible application with Eye Tracker events.",
        Company = "Magic Leap",
        Version = "1.0.0",
        Priority = -1,
        BuildTargetGroups = new[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        FeatureId = FeatureId,
        OpenxrExtensionStrings = "XR_ML_eye_tracker"
    )]
#endif
    public partial class MagicLeapEyeTrackerFeature : MagicLeapOpenXRInteractionFeatureBase
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_eyetracker";
        public const string ExtensionName = "XR_ML_eye_tracker";
        public const string DeviceLocalizedName = "Magic Leap Eye Tracker OpenXR";
        private const string UserPath = "/user/eyes_ml";
        private const string Profile = "/interaction_profiles/ml/eye_tracker_interaction_ml";
        private const string GazePose = "/input/gaze_ml/pose";
        private const string LeftPose = "/input/left_center_ml/pose";
        private const string RightPose = "/input/right_center_ml/pose";
        private const string VergencePose = "/input/vergence_ml/pose";

        private EyeTrackerNativeFunctions nativeFunctions;

        protected override bool UsesExperimentalExtensions => true;
        protected override string LayoutName => "EyeTracker";

        [Preserve, InputControlLayout(displayName = "Eye Tracker (Magic Leap)", isGenericTypeOfDevice = true)]
        public class EyeTrackerDevice : OpenXRDevice
        {
            [Preserve, InputControl(offset = 0, usages = new[] { "gaze", "gazePose" })]
            public PoseControl gazePose { get; private set; }

            [Preserve, InputControl(offset = 1, usages = new[] { "left", "leftPose" })]
            public PoseControl leftPose { get; private set; }

            [Preserve, InputControl(offset = 2, usages = new[] { "right", "rightPose" })]
            public PoseControl rightPose { get; private set; }

            [Preserve, InputControl(offset = 3, usages = new[] { "vergence", "vergencePose" })]
            public PoseControl vergencePose { get; private set; }

            protected override void FinishSetup()
            {
                base.FinishSetup();
                gazePose = GetChildControl<PoseControl>("gazePose");
                leftPose = GetChildControl<PoseControl>("leftPose");
                rightPose = GetChildControl<PoseControl>("rightPose");
                vergencePose = GetChildControl<PoseControl>("vergencePose");
            }
        }

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            if (OpenXRRuntime.IsExtensionEnabled(ExtensionName))
            {
                var result = base.OnInstanceCreate(xrInstance);
                if (result)
                {
                    nativeFunctions = CreateNativeFunctions<EyeTrackerNativeFunctions>();
                }
                return result;
            }
            Debug.LogError($"{ExtensionName} is not enabled. Disabling {nameof(MagicLeapEyeTrackerFeature)}");
            return false;
        }

        protected override void RegisterDeviceLayout()
        {
            base.RegisterDeviceLayout();

            UnityEngine.InputSystem.InputSystem.RegisterLayout(typeof(EyeTrackerDevice),
                LayoutName,
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct(DeviceLocalizedName));
        }

        protected override InteractionProfileType GetInteractionProfileType()
        {
            base.GetInteractionProfileType();

            return typeof(EyeTrackerDevice).IsSubclassOf(typeof(XRController)) ? InteractionProfileType.XRController : InteractionProfileType.Device;
        }

        protected override void RegisterActionMapsWithRuntime()
        {
            base.RegisterActionMapsWithRuntime();

            ActionMapConfig actionMap = new ActionMapConfig()
            {
                name = "eyetracker",
                localizedName = DeviceLocalizedName,
                desiredInteractionProfile = Profile,
                manufacturer = "",
                serialNumber = "",
                deviceInfos = new List<DeviceConfig>()
                {
                    new DeviceConfig()
                    {
                        characteristics = InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.EyeTracking | InputDeviceCharacteristics.HeadMounted,
                        userPath = UserPath
                    }
                },
                actions = new List<ActionConfig>()
                {
                    // Gaze Pose
                    new ActionConfig()
                    {
                        name = "gazePose",
                        localizedName = "GazePose",
                        type = ActionType.Pose,
                        usages = new List<string>()
                        {
                            "gaze",
                            "gazePose"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = GazePose,
                                interactionProfileName = Profile
                            }
                        }
                    },
                    // Left Pose
                    new ActionConfig()
                    {
                        name = "leftPose",
                        localizedName = "LeftPose",
                        type = ActionType.Pose,
                        usages = new List<string>()
                        {
                            "left",
                            "leftPose"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = LeftPose,
                                interactionProfileName = Profile
                            }
                        }
                    },
                    // Right Pose
                    new ActionConfig()
                    {
                        name = "rightPose",
                        localizedName = "RightPose",
                        type = ActionType.Pose,
                        usages = new List<string>()
                        {
                            "right",
                            "rightPose"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = RightPose,
                                interactionProfileName = Profile
                            }
                        }
                    },
                    // Vergence Pose
                    new ActionConfig()
                    {
                        name = "vergencePose",
                        localizedName = "VergencePose",
                        type = ActionType.Pose,
                        usages = new List<string>()
                        {
                            "vergence",
                            "vergencePose"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = VergencePose,
                                interactionProfileName = Profile
                            }
                        }
                    }
                }
            };

            AddActionMap(actionMap);
        }
    }

    public static class EyeTrackerUsages
    {
        public static InputFeatureUsage<Vector3> gazePosition = new InputFeatureUsage<Vector3>("gazePosition");
        public static InputFeatureUsage<Quaternion> gazeRotation = new InputFeatureUsage<Quaternion>("gazeRotation");
        public static InputFeatureUsage<Vector3> leftPosition = new InputFeatureUsage<Vector3>("leftPosition");
        public static InputFeatureUsage<Quaternion> leftRotation = new InputFeatureUsage<Quaternion>("leftRotation");
        public static InputFeatureUsage<Vector3> rightPosition = new InputFeatureUsage<Vector3>("rightPosition");
        public static InputFeatureUsage<Quaternion> rightRotation = new InputFeatureUsage<Quaternion>("rightRotation");
        public static InputFeatureUsage<Vector3> vergencePosition = new InputFeatureUsage<Vector3>("vergencePosition");
        public static InputFeatureUsage<Quaternion> vergenceRotation = new InputFeatureUsage<Quaternion>("vergenceRotation");
    }
}
