// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine.Scripting;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
#if !UNITY_XR_MAGICLEAP_PROVIDER
using VibePattern = UnityEngine.XR.MagicLeap.InputSubsystem.Extensions.DeviceCommands.VibePattern;
using VibeIntensity = UnityEngine.XR.MagicLeap.InputSubsystem.Extensions.DeviceCommands.VibeIntensity;
using SendControllerVibe = UnityEngine.XR.MagicLeap.InputSubsystem.Extensions.DeviceCommands.SendControllerVibe;
using QueryControllerEnabled = UnityEngine.XR.MagicLeap.InputSubsystem.Extensions.DeviceCommands.QueryControllerEnabled;
using SetControllerEnabled = UnityEngine.XR.MagicLeap.InputSubsystem.Extensions.DeviceCommands.SetControllerEnabled;
using QueryEyesEnabled = UnityEngine.XR.MagicLeap.InputSubsystem.Extensions.DeviceCommands.QueryEyesEnabled;
using SetEyesEnabled = UnityEngine.XR.MagicLeap.InputSubsystem.Extensions.DeviceCommands.SetEyesEnabled;
#endif

namespace UnityEngine.XR.MagicLeap
{
    public static partial class InputSubsystem
    {
        public static partial class Extensions
        {
            public static class DeviceLayouts
            {
                [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
                public static void RegisterInputLayouts()
                {
                    UnityEngine.InputSystem.InputSystem.RegisterLayout<MagicLeapHeadset>(
                        matches: new InputDeviceMatcher()
                            .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                            .WithProduct("MagicLeap Headset")
                    );
                    UnityEngine.InputSystem.InputSystem.RegisterLayout<MagicLeapEyes>(
                        matches: new InputDeviceMatcher()
                            .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                            .WithProduct("MagicLeap Eyes")
                    );
                    UnityEngine.InputSystem.InputSystem.RegisterLayout<MagicLeapController>(
                        matches: new InputDeviceMatcher()
                            .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                            .WithProduct("MagicLeap Controller")
                    );
                    UnityEngine.InputSystem.InputSystem.RegisterLayout<MagicLeapHandDevice>(
                        matches: new InputDeviceMatcher()
                            .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                            .WithProduct("^(MagicLeapHand - )")
                    );
                }

                /// <summary>
                /// Enum representing the eye calibration status for a MagicLeap headset.
                /// For use with IntegerControl MagicLeapEyes.eyeCalibrationStatus
                /// </summary>
                public enum CalibrationStatus
                {
                    None = 0,
                    Bad,
                    Good
                }

                /// <summary>
                /// Enum representing the eye calibration status for a MagicLeap headset.
                /// For use with MagicLeapController.dof IntegerControl
                /// </summary>
                public enum ControllerDoF
                {
                    None = 0,
                    DoF3,
                    DoF6
                }

                /// <summary>
                /// Enum representing the current controller calibration accuracy
                /// For use with MagicLeapController.calibrationAccuracy IntegerControl
                /// </summary>
                public enum ControllerCalibrationAccuracy
                {
                    Bad = 0,
                    Low,
                    Medium,
                    High
                }

                /// <summary>
                /// Enum representing the type of Magic Leap Controller being used.
                /// For use with MagicLeapController.type IntegerControl
                /// </summary>
                public enum ControllerType
                {
                    None = 0,
                    Device,
                    MobileApp
                }

                [Preserve]
                [InputControlLayout(displayName = "MagicLeapHeadset")]
                public class MagicLeapHeadset : XRHMD
                {
                    public bool ControllerEnabled
                    {
                        get
                        {
                            var command = QueryControllerEnabled.Create();
                            if (ExecuteCommand(ref command) >= 0)
                                return command.isEnabled;
                            return false;
                        }
                        set
                        {
                            var command = SetControllerEnabled.Create(value);
                            ExecuteCommand(ref command);
                        }
                    }

                    public bool EyesEnabled
                    {
                        get
                        {
                            var command = QueryEyesEnabled.Create();
                            if (ExecuteCommand(ref command) >= 0)
                                return command.isEnabled;
                            return false;
                        }
                        set
                        {
                            var command = SetEyesEnabled.Create(value);
                            ExecuteCommand(ref command);
                        }
                    }

                    [Preserve]
                    [InputControl]
                    public AxisControl confidence { get; private set; }

                    protected override void FinishSetup()
                    {
                        base.FinishSetup();

                        confidence = GetChildControl<AxisControl>("confidence");
                        EyesEnabled = true;
                    }
                }

                [Preserve]
                [InputControlLayout(displayName = "MagicLeapEyes")]
                public class MagicLeapEyes : UnityEngine.InputSystem.InputDevice
                {
                    [Preserve]
                    [InputControl]
                    public AxisControl confidence { get; private set; }
                    [Preserve]
                    [InputControl]
                    public AxisControl fixationPointConfidence { get; private set; }
                    [Preserve]
                    [InputControl]
                    public AxisControl eyeLeftCenterConfidence { get; private set; }
                    [Preserve]
                    [InputControl]
                    public AxisControl eyeRightCenterConfidence { get; private set; }
                    [Preserve]
                    [InputControl]
                    public IntegerControl eyeCalibrationStatus { get; private set; }

                    protected override void FinishSetup()
                    {
                        base.FinishSetup();
                        confidence = GetChildControl<AxisControl>("confidence");
                        fixationPointConfidence = GetChildControl<AxisControl>("fixationPointConfidence");
                        eyeLeftCenterConfidence = GetChildControl<AxisControl>("eyeLeftCenterConfidence");
                        eyeRightCenterConfidence = GetChildControl<AxisControl>("eyeRightCenterConfidence");
                        eyeCalibrationStatus = GetChildControl<IntegerControl>("eyeCalibrationStatus");
                    }
                }

                [Preserve]
                [InputControlLayout(commonUsages = new[] { "LeftHand", "RightHand" }, displayName = "MagicLeapHand")]
                public class MagicLeapHandDevice : UnityEngine.InputSystem.InputDevice
                {
                    [Preserve]
                    [InputControl]
                    public AxisControl handConfidence { get; private set; }
                    [Preserve]
                    [InputControl]
                    public Vector3Control normalizeCenter { get; private set; }
                    [Preserve]
                    [InputControl]
                    public BoneControl wristCenter { get; private set; }
                    [Preserve]
                    [InputControl]
                    public BoneControl wristUlnar { get; private set; }
                    [Preserve]
                    [InputControl]
                    public BoneControl wristRadial { get; private set; }
                    [Preserve]
                    [InputControl]
                    public IntegerControl keyPose { get; private set; }

                    protected override void FinishSetup()
                    {
                        base.FinishSetup();

                        var capabilities = description.capabilities;
                        var deviceDescriptor = XRDeviceDescriptor.FromJson(capabilities);

                        if (deviceDescriptor != null)
                        {
                            if ((deviceDescriptor.characteristics & InputDeviceCharacteristics.Left) != 0)
                                InputSystem.InputSystem.SetDeviceUsage(this, InputSystem.CommonUsages.LeftHand);
                            else if ((deviceDescriptor.characteristics & InputDeviceCharacteristics.Right) != 0)
                                InputSystem.InputSystem.SetDeviceUsage(this, InputSystem.CommonUsages.RightHand);
                        }

                        handConfidence = GetChildControl<AxisControl>("handConfidence");
                        normalizeCenter = GetChildControl<Vector3Control>("normalizeCenter");
                        wristCenter = GetChildControl<BoneControl>("wristCenter");
                        wristUlnar = GetChildControl<BoneControl>("wristUlnar");
                        wristRadial = GetChildControl<BoneControl>("wristRadial");
                        keyPose = GetChildControl<IntegerControl>("keyPose");
                    }
                }

                [Preserve]
                [InputControlLayout(displayName = "MagicLeapController")]
                public class MagicLeapController : XRController
                {
                    [Preserve]
                    [InputControl]
                    public Vector2Control touchpadPosition { get; private set; }
                    [Preserve]
                    [InputControl]
                    public ButtonControl touchpadClick { get; private set; }
                    [Preserve]
                    [InputControl]
                    public ButtonControl touchpadTouch { get; private set; }

                    [Preserve]
                    [InputControl]
                    public ButtonControl triggerButton { get; private set; }
                    [Preserve]
                    [InputControl]
                    public AxisControl trigger { get; private set; }
                    [Preserve]
                    [InputControl]
                    public ButtonControl bumperButton { get; private set; }

                    [Preserve]
                    [InputControl]
                    public AxisControl bumper { get; private set; }

                    [Preserve]
                    [InputControl]
                    public ButtonControl menu { get; private set; }


                    //Need Discrete State for DOF and Type and CalibrationAccuracy

                    public bool StartVibe(VibePattern pattern, VibeIntensity vibeIntensity)
                    {
                        var command = SendControllerVibe.Create(pattern, vibeIntensity);
                        return ExecuteCommand(ref command) >= 0;
                    }

                    //dd
                    protected override void FinishSetup()
                    {
                        base.FinishSetup();

                        touchpadPosition = GetChildControl<Vector2Control>("primary2DAxis");
                        touchpadClick = GetChildControl<ButtonControl>("primary2DAxisClick");
                        touchpadTouch = GetChildControl<ButtonControl>("primary2DAxisTouch");

                        triggerButton = GetChildControl<ButtonControl>("triggerButton");
                        trigger = GetChildControl<AxisControl>("trigger");
                        bumperButton = GetChildControl<ButtonControl>("gripButton");
                        bumper = GetChildControl<AxisControl>("grip");
                        menu = GetChildControl<ButtonControl>("menuButton");
                    }
                }
            }

        }
    }
}
