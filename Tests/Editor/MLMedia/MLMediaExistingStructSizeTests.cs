using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLMedia
    {
        public partial class Player
        {
            public partial class Track
            {
                public partial class DRM
                {
                    [Test]
                    public void NativeBindings_MLMediaDRMByteArray_struct_size()
                    {
                        var capiSize = NativeBindingsTestsProvider.GetMLMediaDRMByteArraySize();
                        var type = FindTypeByName("MLMediaDRMByteArray");
                        var sdkSize = Marshal.SizeOf(type);

                        Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
                    }

                    [Test]
                    public void NativeBindings_MLMediaDRMByteArrayList_struct_size()
                    {
                        var capiSize = NativeBindingsTestsProvider.GetMLMediaDRMByteArrayListSize();
                        var type = FindTypeByName("MLMediaDRMByteArrayList");
                        var sdkSize = Marshal.SizeOf(type);

                        Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
                    }

                    [Test]
                    public void NativeBindings_MLMediaDRMKeyValue_struct_size()
                    {
                        var capiSize = NativeBindingsTestsProvider.GetMLMediaDRMKeyValueSize();
                        var type = FindTypeByName("MLMediaDRMKeyValue");
                        var sdkSize = Marshal.SizeOf(type);

                        Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
                    }

                    [Test]
                    public void NativeBindings_MLMediaDRMKeyValueArray_struct_size()
                    {
                        var capiSize = NativeBindingsTestsProvider.GetMLMediaDRMKeyValueArraySize();
                        var type = FindTypeByName("MLMediaDRMKeyValueArray");
                        var sdkSize = Marshal.SizeOf(type);

                        Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
                    }

                    [Test]
                    public void NativeBindings_MLMediaDRMKeyRequestInputParam_struct_size()
                    {
                        var capiSize = NativeBindingsTestsProvider.GetMLMediaDRMKeyRequestInputParamSize();
                        var type = FindTypeByName("MLMediaDRMKeyRequestInputParam");
                        var sdkSize = Marshal.SizeOf(type);

                        Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
                    }

                    [Test]
                    public void NativeBindings_MLMediaDRMRequestMessage_struct_size()
                    {
                        var capiSize = NativeBindingsTestsProvider.GetMLMediaDRMRequestMessageSize();
                        var type = FindTypeByName("MLMediaDRMRequestMessage");
                        var sdkSize = Marshal.SizeOf(type);

                        Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
                    }

                    [Test]
                    public void NativeBindings_MLMediaDRMCryptoInputParam_struct_size()
                    {
                        var capiSize = NativeBindingsTestsProvider.GetMLMediaDRMCryptoInputParamSize();
                        var type = FindTypeByName("MLMediaDRMCryptoInputParam");
                        var sdkSize = Marshal.SizeOf(type);

                        Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
                    }

                    [Test]
                    public void NativeBindings_MLMediaDRMHMACInputParam_struct_size()
                    {
                        var capiSize = NativeBindingsTestsProvider.GetMLMediaDRMHMACInputParamSize();
                        var type = FindTypeByName("MLMediaDRMHMACInputParam");
                        var sdkSize = Marshal.SizeOf(type);

                        Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
                    }

                    [Test]
                    public void NativeBindings_MLMediaDRMRSAInputParam_struct_size()
                    {
                        var capiSize = NativeBindingsTestsProvider.GetMLMediaDRMRSAInputParamSize();
                        var type = FindTypeByName("MLMediaDRMRSAInputParam");
                        var sdkSize = Marshal.SizeOf(type);

                        Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
                    }

                    [Test]
                    public void NativeBindings_MLMediaDRMEventInfo_struct_size()
                    {
                        var capiSize = NativeBindingsTestsProvider.GetMLMediaDRMEventInfoSize();
                        var type = FindTypeByName("MLMediaDRMEventInfo");
                        var sdkSize = Marshal.SizeOf(type);

                        Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
                    }

                    [Test]
                    public void NativeBindings_MLMediaDRMExpirationUpdateInfo_struct_size()
                    {
                        var capiSize = NativeBindingsTestsProvider.GetMLMediaDRMExpirationUpdateInfoSize();
                        var type = FindTypeByName("MLMediaDRMExpirationUpdateInfo");
                        var sdkSize = Marshal.SizeOf(type);

                        Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
                    }

                    [Test]
                    public void NativeBindings_MLMediaDRMKeyStatus_struct_size()
                    {
                        var capiSize = NativeBindingsTestsProvider.GetMLMediaDRMKeyStatusSize();
                        var type = FindTypeByName("MLMediaDRMKeyStatus");
                        var sdkSize = Marshal.SizeOf(type);

                        Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
                    }

                    [Test]
                    public void NativeBindings_MLMediaDRMKeyStatusInfo_struct_size()
                    {
                        var capiSize = NativeBindingsTestsProvider.GetMLMediaDRMKeyStatusInfoSize();
                        var type = FindTypeByName("MLMediaDRMKeyStatusInfo");
                        var sdkSize = Marshal.SizeOf(type);

                        Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
                    }

                    [Test]
                    public void NativeBindings_MLMediaDRMEventCallbacks_struct_size()
                    {
                        var capiSize = NativeBindingsTestsProvider.GetMLMediaDRMEventCallbacksSize();
                        var type = FindTypeByName("MLMediaDRMEventCallbacks");
                        var sdkSize = Marshal.SizeOf(type);

                        Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
                    }
                }
            }
        }
    }
}