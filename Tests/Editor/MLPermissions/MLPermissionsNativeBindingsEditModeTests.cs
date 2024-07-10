// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
#pragma warning disable CS0618 // Type or member is obsolete
    public partial class MLPermissions : NativeBindingsTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLPermissions);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLZIPermissionsStart_Exists()
        {
            AssertThatMethodExists("MLZIPermissionsStart");
        }

        [Test]
        public void NativeBinding_MLZIPermissionsStop_Exists()
        {
            AssertThatMethodExists("MLZIPermissionsStop");
        }

        [Test]
        public void NativeBinding_MLZIPermissionsIsGranted_Exists()
        {
            AssertThatMethodExists("MLZIPermissionsIsGranted");
        }

        [Test]
        public void NativeBinding_MLZIPermissionsRequest_Exists()
        {
            AssertThatMethodExists("MLZIPermissionsRequest");
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
