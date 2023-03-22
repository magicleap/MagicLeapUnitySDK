using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLMedia : NativeBindingsTests
    {
        public partial class Player
        {
            public partial class Track
            {
                public partial class DRM : NativeBindingsTests
                {
                    private MlSdkDllLoader lib;

                    [OneTimeSetUp]
                    public void Init()
                    {
                        lib = new MlSdkDllLoader();
                        lib.Load("media_drm.magicleap");
                    }

                    [OneTimeTearDown]
                    public void Cleanup()
                    {
                        lib.Free();
                    }

                    [SetUp]
                    public void SetupNativeBindings()
                    {
                        var apiType = typeof(UnityEngine.XR.MagicLeap.MLMedia.Player.Track.DRM);
                        nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMIsCryptoSchemeSupported_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMIsCryptoSchemeSupported");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMRequestMessageRelease_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMRequestMessageRelease");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMVerify_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMVerify");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMSign_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMSign");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMRemoveKeys_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMRemoveKeys");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMByteArrayAllocate_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMByteArrayAllocate");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMByteArrayAllocAndCopy_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMByteArrayAllocAndCopy");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMKeyValueArrayAllocate_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMKeyValueArrayAllocate");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMCreate_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMCreate");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMRelease_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMRelease");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMByteArrayRelease_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMByteArrayRelease");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMByteArrayListRelease_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMByteArrayListRelease");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMKeyValueArrayRelease_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMKeyValueArrayRelease");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMKeyValueArrayAdd_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMKeyValueArrayAdd");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMSetOnEventListenerEx_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMSetOnEventListenerEx");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMOpenSession_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMOpenSession");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMCloseSession_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMCloseSession");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMGetKeyRequest_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMGetKeyRequest");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMProvideKeyResponse_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMProvideKeyResponse");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMRestoreKeys_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMRestoreKeys");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMQueryKeyStatus_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMQueryKeyStatus");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMGetProvisionRequest_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMGetProvisionRequest");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMProvideProvisionResponse_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMProvideProvisionResponse");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMGetSecureStops_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMGetSecureStops");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMGetSecureStop_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMGetSecureStop");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMReleaseSecureStops_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMReleaseSecureStops");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMReleaseAllSecureStops_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMReleaseAllSecureStops");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMGetPropertyString_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMGetPropertyString");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMGetPropertyByteArray_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMGetPropertyByteArray");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMSetPropertyString_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMSetPropertyString");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMSetPropertyByteArray_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMSetPropertyByteArray");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMEncrypt_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMEncrypt");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMDecrypt_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMDecrypt");
                    }

                    [Test]
                    public void NativeBinding_MLMediaDRMSignRSA_Exists()
                    {
                        AssertThatMethodExists("MLMediaDRMSignRSA");
                    }
                }
            }
        }
    }
}