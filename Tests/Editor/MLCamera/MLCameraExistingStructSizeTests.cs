using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLCamera : NativeBindingsTests
    {
        [Test]
        public void NativeBindings_MLCameraMetadataRational_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCameraMetadataRationalSize();
            var type = FindTypeByName("MLCameraMetadataRational");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLCameraMRConnectInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCameraMRConnectInfoSize();
            var type = FindTypeByName("MLCameraMRConnectInfo");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLCameraConnectContext_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCameraConnectContextSize();
            var type = FindTypeByName("MLCameraConnectContext");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLCameraCaptureStreamCaps_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCameraCaptureStreamCapsSize();
            var type = FindTypeByName("MLCameraCaptureStreamCaps");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLCameraCaptureStreamConfig_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCameraCaptureStreamConfigSize();
            var type = FindTypeByName("MLCameraCaptureStreamConfig");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLCameraCaptureConfig_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCameraCaptureConfigSize();
            var type = FindTypeByName("MLCameraCaptureConfig");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLCameraPlaneInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCameraPlaneInfoSize();
            var type = FindTypeByName("MLCameraPlaneInfo");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLCameraOutput_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCameraOutputSize();
            var type = FindTypeByName("MLCameraOutput");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLCameraIntrinsicCalibrationParameters_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCameraIntrinsicCalibrationParametersSize();
            var type = FindTypeByName("MLCameraIntrinsicCalibrationParameters");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLCameraResultExtras_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCameraResultExtrasSize();
            var type = FindTypeByName("MLCameraResultExtras");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLCameraDeviceAvailabilityInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCameraDeviceAvailabilityInfoSize();
            var type = FindTypeByName("MLCameraDeviceAvailabilityInfo");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLCameraDeviceAvailabilityStatusCallbacks_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCameraDeviceAvailabilityStatusCallbacksSize();
            var type = FindTypeByName("MLCameraDeviceAvailabilityStatusCallbacks");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLCameraDeviceStatusCallbacks_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCameraDeviceStatusCallbacksSize();
            var type = FindTypeByName("MLCameraDeviceStatusCallbacks");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLCameraCaptureCallbacks_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCameraCaptureCallbacksSize();
            var type = FindTypeByName("MLCameraCaptureCallbacks");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
    }
}