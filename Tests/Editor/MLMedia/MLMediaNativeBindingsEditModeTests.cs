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
    public partial class MLMedia 
    {
        private MlSdkDllLoader lib;
    
        [OneTimeSetUp]
        public void Init()
        {
            lib = new MlSdkDllLoader();
            lib.Load("media_error.magicleap");
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            lib.Free();
        } 
    
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLMedia);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLMediaResultGetString_Exists()
        {
            AssertThatMethodExists("MLMediaResultGetString");
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
