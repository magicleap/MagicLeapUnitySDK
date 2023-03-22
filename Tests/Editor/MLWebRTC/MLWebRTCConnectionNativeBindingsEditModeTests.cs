using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLWebRTC
    {
        public class PeerConnection : NativeBindingsTests
        {
            [SetUp]
            public void SetupNativeBindings()
            {
                var apiType = typeof(UnityEngine.XR.MagicLeap.MLWebRTC.PeerConnection);
                nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
            }

            [Test]
            public void NativeBinding_MLWebRTCConnectionCreate_Exists()
            {
                AssertThatMethodExists("MLWebRTCConnectionCreate");
            }

            [Test]
            public void NativeBinding_MLWebRTCConnectionProcessEvents_Exists()
            {
                AssertThatMethodExists("MLWebRTCConnectionProcessEvents");
            }

            [Test]
            public void NativeBinding_MLWebRTCConnectionCreateOffer_Exists()
            {
                AssertThatMethodExists("MLWebRTCConnectionCreateOffer");
            }

            [Test]
            public void NativeBinding_MLWebRTCConnectionSetRemoteOffer_Exists()
            {
                AssertThatMethodExists("MLWebRTCConnectionSetRemoteOffer");
            }

            [Test]
            public void NativeBinding_MLWebRTCConnectionSetRemoteAnswer_Exists()
            {
                AssertThatMethodExists("MLWebRTCConnectionSetRemoteAnswer");
            }

            [Test]
            public void NativeBinding_MLWebRTCConnectionAddRemoteIceCandidate_Exists()
            {
                AssertThatMethodExists("MLWebRTCConnectionAddRemoteIceCandidate");
            }

            [Test]
            public void NativeBinding_MLWebRTCConnectionIsConnected_Exists()
            {
                AssertThatMethodExists("MLWebRTCConnectionIsConnected");
            }

            [Test]
            public void NativeBinding_MLWebRTCConnectionHasFailed_Exists()
            {
                AssertThatMethodExists("MLWebRTCConnectionHasFailed");
            }

            [Test]
            public void NativeBinding_MLWebRTCConnectionAddLocalSourceTrackEx_Exists()
            {
                AssertThatMethodExists("MLWebRTCConnectionAddLocalSourceTrackEx");
            }

            [Test]
            public void NativeBinding_MLWebRTCConnectionRemoveLocalSourceTrack_Exists()
            {
                AssertThatMethodExists("MLWebRTCConnectionRemoveLocalSourceTrack");
            }

            [Test]
            public void NativeBinding_MLWebRTCConnectionDestroy_Exists()
            {
                AssertThatMethodExists("MLWebRTCConnectionDestroy");
            }
        }
    }
}