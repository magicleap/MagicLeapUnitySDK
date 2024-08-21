// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if USE_INPUT_SYSTEM_POSE_CONTROL
using PoseControl = UnityEngine.InputSystem.XR.PoseControl;
#else
using PoseControl = UnityEngine.XR.OpenXR.Input.PoseControl;
#endif

using System.Collections.Generic;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;
using UnityEngine.XR.OpenXR.Input;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.InteractionProfiles")]
    public class MagicLeapControllerProfile : OpenXRInteractionFeature
    {
        /// <summary>
        /// The feature id string. This is used to give the feature a well known id for reference.
        /// </summary>
        public const string FeatureId = "com.magicleap.openxr.feature.input.ml2controllerprofile";

        /// <summary>
        /// Extension required for enable the Magic Leap 2 Controller Interaction Profile.
        /// </summary>
        public const string ExtensionString = "XR_ML_ml2_controller_interaction";

        /// <summary>
        /// An Input System device based off the <see href="https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#_khronos_simple_controller_profile">Magic Leap Controller interaction profile</see>. This device contains one haptic output motor.
        /// </summary>
        [Preserve, InputControlLayout(displayName = "Magic Leap Controller (OpenXR)", commonUsages = new[] { "LeftHand", "RightHand" })]
        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MagicLeapController")]
        public class MagicLeapController : XRControllerWithRumble
        { 
            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents information from the <see cref="HTCViveControllerProfile.squeeze"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "GripButton", "shoulderClicked"}, usage = "GripButton")]
            public ButtonControl gripPressed { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents the <see cref="MagicLeapControllerProfile.menu"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "Primary", "menubutton" }, usage ="MenuButton")]
            public ButtonControl menu { get; private set; }
            
            /// <summary>
            /// A <see cref="PoseControl"/> that represents information from the <see cref="MagicLeapControllerProfile.grip"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(offset = 0, aliases = new[] { "device", "gripPose" }, usage = "Device")]
            public PoseControl devicePose { get; private set; }

            /// <summary>
            /// A <see cref="PoseControl"/> that represents information from the <see cref="MagicLeapControllerProfile.aim"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(offset = 0, alias = "aimPose", usage = "Pointer")]
            public PoseControl pointer { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) required for backwards compatibility with the XRSDK layouts. This represents the overall tracking state of the device. This value is equivalent to mapping devicePose/isTracked.
            /// </summary>
            [Preserve, InputControl(offset = 2)]
            public new ButtonControl isTracked { get; private set; }

            /// <summary>
            /// A [IntegerControl](xref:UnityEngine.InputSystem.Controls.IntegerControl) required for backwards compatibility with the XRSDK layouts. This represents the bit flag set indicating what data is valid. This value is equivalent to mapping devicePose/trackingState.
            /// </summary>
            [Preserve, InputControl(offset = 4)]
            public new IntegerControl trackingState { get; private set; }

            /// <summary>
            /// A [Vector3Control](xref:UnityEngine.InputSystem.Controls.Vector3Control) required for backwards compatibility with the XRSDK layouts. This is the device position, or grip position. This value is equivalent to mapping devicePose/position.
            /// </summary>
            [Preserve, InputControl(offset = 8, alias = "gripPosition")]
            public new Vector3Control devicePosition { get; private set; }

            /// <summary>
            /// A [QuaternionControl](xref:UnityEngine.InputSystem.Controls.QuaternionControl) required for backwards compatibility with the XRSDK layouts. This is the device orientation, or grip orientation. This value is equivalent to mapping devicePose/rotation.
            /// </summary>
            [Preserve, InputControl(offset = 20, alias = "gripOrientation")]
            public new QuaternionControl deviceRotation { get; private set; }

            /// <summary>
            /// A [Vector3Control](xref:UnityEngine.InputSystem.Controls.Vector3Control) required for backwards compatibility with the XRSDK layouts. This is the pointer position. This value is equivalent to mapping pointerPose/position.
            /// </summary>
            [Preserve, InputControl(offset = 68)]
            public Vector3Control pointerPosition { get; private set; }

            /// <summary>
            /// A [QuaternionControl](xref:UnityEngine.InputSystem.Controls.QuaternionControl) required for backwards compatibility with the XRSDK layouts. This is the pointer rotation. This value is equivalent to mapping pointerPose/rotation.
            /// </summary>
            [Preserve, InputControl(offset = 80, alias = "pointerOrientation")]
            public QuaternionControl pointerRotation { get; private set; }

            /// <summary>
            /// A [AxisControl](xref:UnityEngine.InputSystem.Controls.AxisControl) that represents information from the <see cref="MagicLeapControllerProfile.trigger"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(alias = "triggeraxis", usage = "Trigger")]
            public AxisControl trigger { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents information from the <see cref="MagicLeapControllerProfile.triggerClick"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(alias = "triggerbutton", usage = "TriggerButton")]
            public ButtonControl triggerPressed { get; private set; }

            /// <summary>
            /// A [Vector2Control](xref:UnityEngine.InputSystem.Controls.Vector2Control) that represents information from the <see cref="HTCViveControllerProfile.trackpad"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "Primary2DAxis", "touchpadaxes", "touchpad" }, usage = "Primary2DAxis")]
            public Vector2Control trackpad { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents information from the <see cref="HTCViveControllerProfile.trackpadClick"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "joystickorpadpressed", "touchpadpressed" }, usage = "Primary2DAxisClick")]
            public ButtonControl trackpadClicked { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents information from the <see cref="HTCViveControllerProfile.trackpadTouch"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "joystickorpadtouched", "touchpadtouched" }, usage = "Primary2DAxisTouch")]
            public ButtonControl trackpadTouched { get; private set; }

            /// <summary>
            /// A [AxisControl](xref:UnityEngine.InputSystem.Controls.AxisControl) that represents the <see cref="ValveIndexControllerProfile.trackpadForce"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(alias = "touchpadForce", usage = "Secondary2DAxisForce")]
            public AxisControl trackpadForce { get; private set; }

            /// <summary>
            /// A <see cref="HapticControl"/> that represents the <see cref="MagicLeapControllerProfile.haptic"/> binding.
            /// </summary>
            [Preserve, InputControl(usage = "Haptic")]
            public HapticControl haptic { get; private set; }

            /// <inheritdoc  cref="OpenXRDevice"/>
            protected override void FinishSetup()
            {
                base.FinishSetup();
                gripPressed = GetChildControl<ButtonControl>("gripPressed");
                menu = GetChildControl<ButtonControl>("menu");
                devicePose = GetChildControl<PoseControl>("devicePose");
                pointer = GetChildControl<PoseControl>("pointer");

                isTracked = GetChildControl<ButtonControl>("isTracked");
                trackingState = GetChildControl<IntegerControl>("trackingState");
                devicePosition = GetChildControl<Vector3Control>("devicePosition");
                deviceRotation = GetChildControl<QuaternionControl>("deviceRotation");
                pointerPosition = GetChildControl<Vector3Control>("pointerPosition");
                pointerRotation = GetChildControl<QuaternionControl>("pointerRotation");

                trigger = GetChildControl<AxisControl>("trigger");
                triggerPressed = GetChildControl<ButtonControl>("triggerPressed");
                trackpad = GetChildControl<Vector2Control>("trackpad");
                trackpadClicked = GetChildControl<ButtonControl>("trackpadClicked");
                trackpadTouched = GetChildControl<ButtonControl>("trackpadTouched");
                trackpadForce = GetChildControl<AxisControl>("trackpadForce");

                haptic = GetChildControl<HapticControl>("haptic");
            }
        }

        /// <summary>
        /// OpenXR string that represents the <see href="https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#semantic-path-interaction-profiles">Interaction Profile.</see>
        /// </summary>
        public const string profile = "/interaction_profiles/ml/ml2_controller";

        /// <summary>
        /// Constant for a boolean interaction binding '.../input/menu/click' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string menu = "/input/menu/click";

        /// <summary>
        /// Constant for a boolean interaction binding '.../input/trigger/click' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string triggerClick = "/input/trigger/click";

        /// <summary>
        /// Constant for a boolean interaction binding '.../input/trigger/value' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string trigger = "/input/trigger/value";

        /// <summary>
        /// Constant for a Vector2 interaction binding '.../input/trackpad' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string trackpad = "/input/trackpad";

        /// <summary>
        /// Constant for a boolean interaction binding '.../input/trackpad/click' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string trackpadClick = "/input/trackpad/click";

        /// <summary>
        /// Constant for a boolean interaction binding '.../input/trackpad/force' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string trackpadForce = "/input/trackpad/force";

        /// <summary>
        /// Constant for a boolean interaction binding '.../input/trackpad/touch' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string trackpadTouch = "/input/trackpad/touch";

        /// <summary>
        /// Constant for a pose interaction binding '.../input/grip/pose' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string grip = "/input/grip/pose";

        /// <summary>
        /// Constant for a pose interaction binding '.../input/aim/pose' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string aim = "/input/aim/pose";

        /// <summary>
        /// Constant for a boolean interaction binding '.../input/shoulder/click' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string shoulderClick = "/input/shoulder/click";

        /// <summary>
        /// Constant for a haptic interaction binding '.../input/output/haptic' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string haptic = "/output/haptic";

        private const string kDeviceLocalizedName = "Magic Leap Controller OpenXR";

        /// <summary>
        /// Registers the <see cref="MagicLeapController"/> layout with the Input System.
        /// </summary>
        protected override void RegisterDeviceLayout()
        {
            InputSystem.InputSystem.RegisterLayout(typeof(MagicLeapController),
                        matches: new InputDeviceMatcher()
                        .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                        .WithProduct(kDeviceLocalizedName));
        }

        /// <summary>
        /// Unregisters the <see cref="MagicLeapControllerProfile"/> layout from the Input System.
        /// </summary>
        protected override void UnregisterDeviceLayout()
        {
            InputSystem.InputSystem.RemoveLayout(nameof(MagicLeapController));
        }

        protected override string GetDeviceLayoutName()
        {
            return nameof(MagicLeapController);
        }

        protected override InteractionProfileType GetInteractionProfileType()
        {
            return base.GetInteractionProfileType();
        }

        /// <inheritdoc/>
        protected override void RegisterActionMapsWithRuntime()
        {
            ActionMapConfig actionMap = new ActionMapConfig()
            {
                name = nameof(MagicLeapController),
                localizedName = kDeviceLocalizedName,
                desiredInteractionProfile = profile,
                manufacturer = "Magic Leap",
                deviceInfos = new List<DeviceConfig>()
                {
                    new()
                    {
                        characteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left,
                        userPath = UserPaths.leftHand
                    },
                    new()
                    {
                        characteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right,
                        userPath = UserPaths.rightHand
                    }
                },
                actions = new List<ActionConfig>()
                {
                    // Bumper
                    new()
                    {
                        name = nameof(MagicLeapController.gripPressed),
                        localizedName = "Grip Pressed",
                        type = ActionType.Binary,
                        usages = new List<string>(){ "GripButton" },
                        bindings = new List<ActionBinding>()
                        {
                            new()
                            {
                                interactionPath = shoulderClick,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    // Menu
                    new()
                    {
                        name =nameof(MagicLeapController.menu),
                        localizedName = "Menu",
                        type = ActionType.Binary,
                        usages = new List<string>() { "MenuButton" },
                        bindings = new List<ActionBinding>()
                        {
                            new()
                            {
                                interactionPath = menu,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    // Device Pose
                    new()
                    {
                        name = nameof(MagicLeapController.devicePose),
                        localizedName = "Device Pose",
                        type = ActionType.Pose,
                        usages = new List<string>() { "Device" },
                        bindings = new List<ActionBinding>()
                        {
                            new()
                            {
                                interactionPath = grip,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    // Pointer Pose
                    new()
                    {
                        name = nameof(MagicLeapController.pointer),
                        localizedName = "Pointer Pose",
                        type = ActionType.Pose,
                        usages = new List<string>() { "Pointer" },
                        bindings = new List<ActionBinding>()
                        {
                            new()
                            {
                                interactionPath = aim,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    // Trigger 
                    new()
                    {
                        name = nameof(MagicLeapController.trigger),
                        localizedName = "Trigger",
                        type = ActionType.Axis1D,
                        usages = new List<string>() { "Trigger" },
                        bindings = new List<ActionBinding>()
                        {
                            new()
                            {
                                interactionPath = trigger,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    // Trigger Pressed
                    new()
                    {
                        name = nameof(MagicLeapController.triggerPressed),
                        localizedName = "Trigger Pressed",
                        type = ActionType.Binary,
                        usages = new List<string>() { "TriggerButton" },
                        bindings = new List<ActionBinding>()
                        {
                            new()
                            {
                                interactionPath = triggerClick,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    new()
                    {
                        name = nameof(MagicLeapController.trackpad),
                        localizedName = "Trackpad",
                        type = ActionType.Axis2D,
                        usages = new List<string>() { "Primary2DAxis" },
                        bindings = new List<ActionBinding>()
                        {
                            new()
                            {
                                interactionPath = trackpad,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    new()
                    {
                        name = nameof(MagicLeapController.trackpadTouched),
                        localizedName = "Trackpad Touched",
                        type = ActionType.Binary,
                        usages = new List<string>() { "Primary2DAxisTouch" },
                        bindings = new List<ActionBinding>()
                        {
                            new()
                            {
                                interactionPath = trackpadTouch,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    new()
                    {
                        name = nameof(MagicLeapController.trackpadClicked),
                        localizedName = "Trackpad Clicked",
                        type = ActionType.Binary,
                        usages = new List<string>() { "Primary2DAxisClick" },
                        bindings = new List<ActionBinding>()
                        {
                            new()
                            {
                                interactionPath = trackpadClick,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    new()
                    {
                        name = nameof(MagicLeapController.trackpadForce),
                        localizedName = "Trackpad Force",
                        type = ActionType.Axis1D,
                        usages = new List<string>() { "Secondary2DAxisForce" },
                        bindings = new List<ActionBinding>()
                        {
                            new()
                            {
                                interactionPath = trackpadForce,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    // Haptics
                    new()
                    {
                        name = nameof(MagicLeapController.haptic),
                        localizedName = "Haptic Output",
                        type = ActionType.Vibrate,
                        usages = new List<string>() { "Haptic" },
                        bindings = new List<ActionBinding>()
                        {
                            new()
                            {
                                interactionPath = haptic,
                                interactionProfileName = profile,
                            }
                        }
                    }
                }
            };

            AddActionMap(actionMap);
        }

        #region Raycast subsystem
        private static List<XRRaycastSubsystemDescriptor> subsystemDescriptors = new List<XRRaycastSubsystemDescriptor>();

        protected override void OnSubsystemCreate()
        {
            CreateSubsystem<XRRaycastSubsystemDescriptor, XRRaycastSubsystem>(subsystemDescriptors, RaycastSubsystem.Id);
        }

        protected override void OnSubsystemDestroy()
        {
            DestroySubsystem<XRRaycastSubsystem>();
        }

        // We don't actually need a Raycast Subsystem with OpenXR; however, because we use AR Foundation, the XRRayInteractor component
        // on our XR Origin rig's Controller object will instantiate an ARRaycastManager, which in turn will log an annoying warning if it can't find a corresponding subsytem.
        // By creating an instance of this subsystem, we can avoid those warnings. Note that this implementation doesn't actually do anything!
        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.RaycastSubsystem")]
        private class RaycastSubsystem : XRRaycastSubsystem
        {
            public const string Id = "MagicLeap-Raycast";
            public override TrackableChanges<XRRaycast> GetChanges(Unity.Collections.Allocator allocator) => default;

            [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MagicLeapProvider")]

            private class MagicLeapProvider : Provider { }

            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
            private static void RegisterDescriptor()
            {
                XRRaycastSubsystemDescriptor.RegisterDescriptor(new XRRaycastSubsystemDescriptor.Cinfo
                {
                    id = RaycastSubsystem.Id,
                    providerType = typeof(MagicLeapProvider),
                    subsystemTypeOverride = typeof(RaycastSubsystem),
                    supportsTrackedRaycasts = false,
                    supportsViewportBasedRaycast = false,
                    supportsWorldBasedRaycast = false,
                    supportedTrackableTypes = TrackableType.None
                });
            }
        }
        #endregion
    }
}
