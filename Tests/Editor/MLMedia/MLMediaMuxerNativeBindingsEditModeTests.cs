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
        public partial class Muxer : NativeBindingsTests
        {
            private MlSdkDllLoader lib;

            [OneTimeSetUp]
            public void Init()
            {
                lib = new MlSdkDllLoader();
                lib.Load("media_muxer.magicleap");
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                lib.Free();
            }

            [SetUp]
            public void SetupNativeBindings()
            {
                var apiType = typeof(UnityEngine.XR.MagicLeap.MLMedia.Muxer);
                nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
            }

            [Test]
            public void NativeBinding_MLMediaMuxerCreate_Exists()
            {
                AssertThatMethodExists("MLMediaMuxerCreate");
            }

            [Test]
            public void NativeBinding_MLMediaMuxerWriteSampleData_Exists()
            {
                AssertThatMethodExists("MLMediaMuxerWriteSampleData");
            }

            [Test]
            public void NativeBinding_MLMediaMuxerSetOrientationHint_Exists()
            {
                AssertThatMethodExists("MLMediaMuxerSetOrientationHint");
            }

            [Test]
            public void NativeBinding_MLMediaMuxerStop_Exists()
            {
                AssertThatMethodExists("MLMediaMuxerStop");
            }

            [Test]
            public void NativeBinding_MLMediaMuxerStart_Exists()
            {
                AssertThatMethodExists("MLMediaMuxerStart");
            }

            [Test]
            public void NativeBinding_MLMediaMuxerRelease_Exists()
            {
                AssertThatMethodExists("MLMediaMuxerRelease");
            }

            [Test]
            public void NativeBinding_MLMediaMuxerGetSupportedOutputFormats_Exists()
            {
                AssertThatMethodExists("MLMediaMuxerGetSupportedOutputFormats");
            }

            [Test]
            public void NativeBinding_MLMediaMuxerGetSupportedMimes_Exists()
            {
                AssertThatMethodExists("MLMediaMuxerGetSupportedMimes");
            }

            [Test]
            public void NativeBinding_MLMediaMuxerAddTrack_Exists()
            {
                AssertThatMethodExists("MLMediaMuxerAddTrack");
            }

            [Test]
            public void NativeBinding_MLMediaMuxerConfigure_Exists()
            {
                AssertThatMethodExists("MLMediaMuxerConfigure");
            }

            [Test]
            public void NativeBinding_MLMediaMuxerSetLocation_Exists()
            {
                AssertThatMethodExists("MLMediaMuxerSetLocation");
            }
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
