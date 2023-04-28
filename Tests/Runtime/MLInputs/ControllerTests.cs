using System;
using System.Numerics;
using NUnit.Framework;
using UnityEngine.InputSystem;

namespace UnitySDKPlayTests
{
    public partial class MLInputs
    {
        public partial class ControllerTests
        {
            private MagicLeapInputs mlInputs;
            private MagicLeapInputs.ControllerActions controllerActions;

            /// <summary>
            /// A [Setup] method will execute BEFORE each and every test in this class. They're useful
            /// for preparing a scene state with certain components that will be needed by the tests.
            /// </summary>
            [SetUp]
            public void MLControlSetup()
            {
                mlInputs = new MagicLeapInputs();
                mlInputs.Enable();
                controllerActions = new MagicLeapInputs.ControllerActions(mlInputs);
            }

            /// <summary>
            /// A [TearDown] method will execute AFTER each and every test in this class. They're useful
            /// for cleanup, where you don't want something leftover from one test to possibly affect the next.
            /// May be unnecessary if you only have one test. 
            /// </summary>
            [TearDown]
            public void MLControlTearDown()
            {
                mlInputs.Dispose();
            }

            [Test]
            public void MLInput_ControllerActions()
            {
                try
                {
                    controllerActions.Position.ReadValue<Vector3>();
                    controllerActions.Velocity.ReadValue<Vector3>();
                    controllerActions.AngularVelocity.ReadValue<Vector3>();
                    controllerActions.Acceleration.ReadValue<Vector3>();
                    controllerActions.AngularAcceleration.ReadValue<Vector3>();
                    controllerActions.Rotation.ReadValue<Quaternion>();
                    controllerActions.Menu.IsPressed();
                    controllerActions.Trigger.ReadValue<float>();
                    InputActionPhase phase = controllerActions.TriggerHold.phase;
                    controllerActions.Bumper.IsPressed();
                    controllerActions.TouchpadPosition.ReadValue<Vector2>();
                    controllerActions.TouchpadForce.ReadValue<float>();
                }
                catch (Exception e)
                {
                    Assert.Fail(e.Message);
                }
            }
        }
    }
}
