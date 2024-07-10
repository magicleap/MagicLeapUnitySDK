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
        public class ParserCEA708 : NativeBindingsTests
        {
            private MlSdkDllLoader lib;

            [OneTimeSetUp]
            public void Init()
            {
                lib = new MlSdkDllLoader();
                lib.Load("media_cea708parser.magicleap");
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                lib.Free();
            }

            [SetUp]
            public void SetupNativeBindings()
            {
                var apiType = typeof(UnityEngine.XR.MagicLeap.MLMedia.ParserCEA708);
                nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
            }

            [Test]
            public void NativeBinding_MLMediaCea708ParserCreate_Exists()
            {
                AssertThatMethodExists("MLMediaCea708ParserCreate");
            }

            [Test]
            public void NativeBinding_MLMediaCea708ParserDestroy_Exists()
            {
                AssertThatMethodExists("MLMediaCea708ParserDestroy");
            }

            [Test]
            public void NativeBinding_MLMediaCea708ParserSetEmitEventCallback_Exists()
            {
                AssertThatMethodExists("MLMediaCea708ParserSetEmitEventCallback");
            }

            [Test]
            public void NativeBinding_MLMediaCea708ParserParse_Exists()
            {
                AssertThatMethodExists("MLMediaCea708ParserParse");
            }
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
