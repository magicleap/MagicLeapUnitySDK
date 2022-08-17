// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="DeviceLayouts.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2021 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine.Scripting;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;

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
                /// Enum representing the eye calibration status for a MagicLeapLightwear headset.
                /// For use with IntegerControl MagicLeapLightwear.eyeCalibrationStatus
                /// </summary>
                public enum CalibrationStatus
                {
                    None = 0,
                    Bad,
                    Good
                }

                /// <summary>
                /// Enum representing the eye calibration status for a MagicLeapLightwear headset.
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
                public class MagicLeapHandDevice : XRController
                {
                    [Preserve]
                    [InputControl]
                    public AxisControl handConfidence { get; private set; }
                    [Preserve]
                    [InputControl]
                    public Vector3Control normalizeCenter { get; private set; }
                    [Preserve]
                    [InputControl]
                    public Vector3Control wristCenter { get; private set; }
                    [Preserve]
                    [InputControl]
                    public Vector3Control wristUlnar { get; private set; }
                    [Preserve]
                    [InputControl]
                    public Vector3Control wristRadial { get; private set; }
                    [Preserve]
                    [InputControl]
                    public IntegerControl keyPose { get; private set; }

                    //Need Bone control and Hand Control

                    protected override void FinishSetup()
                    {
                        base.FinishSetup();

                        handConfidence = GetChildControl<AxisControl>("handConfidence");
                        normalizeCenter = GetChildControl<Vector3Control>("normalizeCenter");
                        wristCenter = GetChildControl<Vector3Control>("wristCenter");
                        wristUlnar = GetChildControl<Vector3Control>("wristUlnar");
                        wristRadial = GetChildControl<Vector3Control>("wristRadial");
                        keyPose = GetChildControl<IntegerControl>("keyPose");
                    }
                }

                [Preserve]
                [InputControlLayout(displayName = "MagicLeapController")]
                public class MagicLeapController : XRController
                {
                    [Preserve]
                    [InputControl]
                    public ButtonControl touchpad1Pressed { get; private set; }
                    [Preserve]
                    [InputControl]
                    public Vector2Control touchpad1Position { get; private set; }
                    [Preserve]
                    [InputControl]
                    public AxisControl touchpad1Force { get; private set; }

                    [Preserve]
                    [InputControl]
                    public ButtonControl touchpad2Pressed { get; private set; }
                    [Preserve]
                    [InputControl]
                    public Vector2Control touchpad2Position { get; private set; }
                    [Preserve]
                    [InputControl]
                    public AxisControl touchpad2Force { get; private set; }

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
                    [Preserve]
                    [InputControl]
                    public IntegerControl dof { get; private set; }
                    [Preserve]
                    [InputControl]
                    public IntegerControl calibrationAccuracy { get; private set; }
                    [Preserve]
                    [InputControl]
                    public IntegerControl type { get; private set; }


                    //Need Discrete State for DOF and Type and CalibrationAccuracy

                    public bool StartVibe(VibePattern pattern, VibeIntensity vibeIntensity)
                    {
                        var command = SendControllerVibe.Create(pattern, vibeIntensity);
                        return ExecuteCommand(ref command) >= 0;
                    }

                    public bool StartLEDPattern(LEDPattern ledPattern, LEDColor ledColor, uint durationMs)
                    {
                        var command = SendLEDPattern.Create(ledPattern, ledColor, durationMs);
                        return ExecuteCommand(ref command) >= 0;
                    }

                    public bool StartLEDEffect(LEDEffect ledEffect, LEDSpeed ledSpeed, LEDPattern ledPattern, LEDColor ledColor, uint durationMs)
                    {
                        var command = SendLEDEffect.Create(ledEffect, ledSpeed, ledPattern, ledColor, durationMs);
                        return ExecuteCommand(ref command) >= 0;
                    }

                    protected override void FinishSetup()
                    {
                        base.FinishSetup();

                        touchpad1Pressed = GetChildControl<ButtonControl>("touchpad1Pressed");
                        touchpad1Position = GetChildControl<Vector2Control>("touchpad1Position");
                        touchpad1Force = GetChildControl<AxisControl>("touchpad1Force");

                        touchpad2Pressed = GetChildControl<ButtonControl>("touchpad2Pressed");
                        touchpad2Position = GetChildControl<Vector2Control>("touchpad2Position");
                        touchpad2Force = GetChildControl<AxisControl>("touchpad2Force");

                        triggerButton = GetChildControl<ButtonControl>("triggerButton");
                        trigger = GetChildControl<AxisControl>("trigger");
                        bumperButton = GetChildControl<ButtonControl>("gripButton");
                        bumper = GetChildControl<AxisControl>("grip");
                        menu = GetChildControl<ButtonControl>("menu");

                        dof = GetChildControl<IntegerControl>("dof");
                        calibrationAccuracy = GetChildControl<IntegerControl>("calibrationAccuracy");
                        type = GetChildControl<IntegerControl>("type");

                    }
                }
            }

        }
    }
}
