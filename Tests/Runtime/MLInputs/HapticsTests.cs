using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public partial class MLInputs
    {
        public partial class HapticsTests
        {
            private MagicLeapInputs mlInputs;
            private InputSubsystem.Extensions.Haptics.CustomPattern customHaptics = new InputSubsystem.Extensions.Haptics.CustomPattern();

            [Test]
            public void Haptics_Create()
            {
                try
                {
                    CreateCustomHapticsPattern();
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                mlInputs.Dispose();
            }

            [Test]
            public void Haptics_Dispose()
            {
                CreateCustomHapticsPattern();
                try
                {
                    mlInputs.Dispose();
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }

            private void CreateCustomHapticsPattern()
            {
                mlInputs = new MagicLeapInputs();
                mlInputs.Enable();

                var buzz1 = InputSubsystem.Extensions.Haptics.Buzz.Create(200, 800, 2000, 50);
                var preDefinedA = InputSubsystem.Extensions.Haptics.PreDefined.Create(InputSubsystem.Extensions.Haptics.PreDefined.Type.A);
                var preDefinedC = InputSubsystem.Extensions.Haptics.PreDefined.Create(InputSubsystem.Extensions.Haptics.PreDefined.Type.C);
                var buzz2 = InputSubsystem.Extensions.Haptics.Buzz.Create(800, 200, 2000, 20);

                customHaptics.Add(in buzz1);
                customHaptics.Add(in preDefinedA, 500);
                customHaptics.Add(in preDefinedC, 500);
                customHaptics.Add(in buzz2);
            }
        }
    }
}
