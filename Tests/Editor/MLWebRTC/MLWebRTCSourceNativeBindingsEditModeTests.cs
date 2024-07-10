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
    public partial class MLWebRTC
    {
        public class Source : NativeBindingsTests
        {
            [SetUp]
            public void SetupNativeBindings()
            {
                var apiType = typeof(UnityEngine.XR.MagicLeap.MLWebRTC.Source);
                nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceCreateLocalSourceForCamera_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceCreateLocalSourceForCamera");
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceCreateLocalSourceForMicrophoneEx_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceCreateLocalSourceForMicrophoneEx");
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceCreateAppDefinedAudioSourceEx_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceCreateAppDefinedAudioSourceEx");
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceIsEnabled_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceIsEnabled");
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceSetEnabled_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceSetEnabled");
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceGetType_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceGetType");
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceGetTrackId_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceGetTrackId");
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceReleaseTrackId_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceReleaseTrackId");
            }

            [Test]
            public void NativeBinding_MLWebRTCSourceDestroy_Exists()
            {
                AssertThatMethodExists("MLWebRTCSourceDestroy");
            }
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
