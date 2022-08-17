// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "ExampleEditModeTests.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using NUnit.Framework;
using UnityEngine.XR.MagicLeap;
#if UNITY_MAGICLEAP || UNITY_ANDROID
using UnityEngine.XR.MagicLeap.Native;
#endif

/// <summary>
/// "Edit-Mode Tests" are tests which can only be run at edit time in the Unity editor, 
/// e.g. they don't require running on device or in Playmode via Zero Iteration. 
/// 
/// Edit-mode tests can be ideal for test-driven-development (TDD). The examples below
/// are very rudimentary methods which check that certain simple methods in the Magic Leap
/// SDK return the expected values. 
/// 
/// Note that these methods don't perform any action that
/// depends on the app actually running on device (or via Zero Iteration).
/// </summary>

public class ExampleEditModeTests
{
    /// <summary>
    /// Test to make sure "result.IsOk" works as expected.
    /// </summary>
    [Test]
    public void MLResult_with_Code_Ok_Returns_IsOk()
    {
        MLResult result = MLResult.Create(MLResult.Code.Ok);

        Assert.IsTrue(result.IsOk);
    }

    /// <summary>
    /// Tests to make sure the MLResult.Create method behaves as expected.
    /// </summary>
    [Test]
    public void MLResult_Create_Returns_Result_with_Given_Code()
    {
        MLResult result = MLResult.Create(MLResult.Code.Ok);

        Assert.AreEqual(result.Result, MLResult.Code.Ok);
    }

    /// <summary>
    /// Tests to make sure invalid handles are recognized appropriately
    /// </summary>
    [Test]
    public void NativeBindings_Recognize_Invalid_Handle()
    {
        const ulong invalid = 0xFFFFFFFFFFFFFFFF; // (that's 16 F's)
#if UNITY_MAGICLEAP || UNITY_ANDROID
        Assert.IsFalse(MagicLeapNativeBindings.MLHandleIsValid(invalid));
        Assert.IsTrue(MagicLeapNativeBindings.MLHandleIsValid(0x1));
#else
        Assert.IsTrue(false);
#endif
    }
}
