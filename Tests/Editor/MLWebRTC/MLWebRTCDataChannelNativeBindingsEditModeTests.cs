using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
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
}