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
        public class DataChannel : NativeBindingsTests
        {
            [SetUp]
            public void SetupNativeBindings()
            {
                var apiType = typeof(UnityEngine.XR.MagicLeap.MLWebRTC.DataChannel);
                nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
            }

            [Test]
            public void NativeBinding_MLWebRTCDataChannelCreate_Exists()
            {
                AssertThatMethodExists("MLWebRTCDataChannelCreate");
            }

            [Test]
            public void NativeBinding_MLWebRTCDataChannelGetLabel_Exists()
            {
                AssertThatMethodExists("MLWebRTCDataChannelGetLabel");
            }

            [Test]
            public void NativeBinding_MLWebRTCDataChannelReleaseLabelMemory_Exists()
            {
                AssertThatMethodExists("MLWebRTCDataChannelReleaseLabelMemory");
            }

            [Test]
            public void NativeBinding_MLWebRTCDataChannelIsOpen_Exists()
            {
                AssertThatMethodExists("MLWebRTCDataChannelIsOpen");
            }

            [Test]
            public void NativeBinding_MLWebRTCDataChannelSetEventCallbacks_Exists()
            {
                AssertThatMethodExists("MLWebRTCDataChannelSetEventCallbacks");
            }

            [Test]
            public void NativeBinding_MLWebRTCDataChannelSendMessage_Exists()
            {
                AssertThatMethodExists("MLWebRTCDataChannelSendMessage");
            }

            [Test]
            public void NativeBinding_MLWebRTCDataChannelDestroy_Exists()
            {
                AssertThatMethodExists("MLWebRTCDataChannelDestroy");
            }
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
