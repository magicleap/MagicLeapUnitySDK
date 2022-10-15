// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;
using UnityEngine.XR.MagicLeap;

/// <summary>
/// Runtime Tests (a.k.a. PlayMode Tests) are tests which require the game to be running
/// either on device or in PlayMode. These will tend to be more involved than simple Edit-Mode tests
/// as they might have dependencies on subsystems or other libraries being loaded. These
/// can be thought of more like integration tests as opposed to unit tests, although
/// Unity doesn't use that terminology or make that distinction anywhere.
/// 
/// For us, running these tests in PlayMode will usually require first connecting to a Magic Leap App Simulator 
/// session. Which modes (Simulator vs Device) are suitable might take some experimenting. 
/// </summary>

public class ExampleRuntimeTests
{
    private InputDevice headDevice;

    /// <summary>
    /// A [Setup] method will execute BEFORE each and every test in this class. They're useful
    /// for preparing a scene state with certain components that will be needed by the tests.
    /// </summary>
    [SetUp]
    public void MLDeviceSetup()
    {
        Debug.Log("setup: MLDevice.RegisterGestureSubsystem");

        // Note that this method requires a reference to the UnityEngine.SpatialTracking assembly
        // You'll this on the LuminUnity-Tests.asmdef asset. 
        // It might be useful in the future to further subdivide our tests into smaller ASMDEFs according
        // to which extra Unity or 3rd party assemblies are required.
#if UNITY_MAGICLEAP || UNITY_ANDROID
        MLDevice.RegisterGestureSubsystem();
#endif
    }

    /// <summary>
    /// A [TearDown] method will execute AFTER each and every test in this class. They're useful
    /// for cleanup, where you don't want something leftover from one test to possibly affect the next.
    /// May be unnecessary if you only have one test. 
    /// </summary>
    [TearDown]
    public void MLDeviceTearDown()
    {
        Debug.Log("teardown: MLDevice.UnregisterGestureSubsystem");
#if UNITY_MAGICLEAP || UNITY_ANDROID
        MLDevice.UnregisterGestureSubsystem();
#endif
    }

    [Test]
    public void MLDevice_has_Gesture_Subsystem_Component()
    {
#if UNITY_MAGICLEAP || UNITY_ANDROID
        Assert.IsNotNull(MLDevice.GestureSubsystemComponent);
#else
        Assert.IsTrue(false);
#endif
    }

    // A [UnityTest] behaves like a coroutine (Notice the IEnumerator return type). Use yield statements
    // to test something over time. Here, we wait an arbitrary 1 second before attempting to acquire the
    // headtracking device. 
    [UnityTest]
    public IEnumerator InputSubsystem_Can_Acquire_Head_Device()
    {
        if(!headDevice.isValid)
        {
            yield return new WaitForSeconds(1.0f);
            headDevice = InputSubsystem.Utils.FindMagicLeapDevice(InputDeviceCharacteristics.HeadMounted | InputDeviceCharacteristics.TrackedDevice);
        }

        Assert.IsTrue(headDevice.isValid);
    }
}
