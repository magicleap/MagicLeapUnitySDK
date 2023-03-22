using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLAudio : NativeBindingsTests
    {
        [Test]
        public void NativeBindings_MLAudioSpatialSoundDistanceProperties_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLAudioSpatialSoundDistancePropertiesSize();
            var type = FindTypeByName("MLAudioSpatialSoundDistanceProperties");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLAudioSpatialSoundRadiationProperties_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLAudioSpatialSoundRadiationPropertiesSize();
            var type = FindTypeByName("MLAudioSpatialSoundRadiationProperties");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLAudioBufferFormat_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLAudioBufferFormatSize();
            var type = FindTypeByName("MLAudioBufferFormat");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }

        [Test]
        public void NativeBindings_MLAudioBuffer_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLAudioBufferSize();
            var type = FindTypeByName("MLAudioBuffer");
            var sdkSize = Marshal.SizeOf(type);

            Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
        }
    }
}