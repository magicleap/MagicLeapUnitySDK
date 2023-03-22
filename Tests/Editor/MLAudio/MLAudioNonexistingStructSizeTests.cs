using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLAudio
    {
        [Test]
        public void NativeBindings_MLAudioMultibandLevel_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLAudioMultibandLevelSize();
            var type = FindTypeByName("MLAudioMultibandLevel");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLAudioDispersionProperties_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLAudioDispersionPropertiesSize();
            var type = FindTypeByName("MLAudioDispersionProperties");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLAudioReverbProperties_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLAudioReverbPropertiesSize();
            var type = FindTypeByName("MLAudioReverbProperties");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLAudioRaycast_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLAudioRaycastSize();
            var type = FindTypeByName("MLAudioRaycast");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLAudioObstruction_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLAudioObstructionSize();
            var type = FindTypeByName("MLAudioObstruction");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLAudioRaycastRequest_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLAudioRaycastRequestSize();
            var type = FindTypeByName("MLAudioRaycastRequest");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLAudioRaycastResult_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLAudioRaycastResultSize();
            var type = FindTypeByName("MLAudioRaycastResult");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLAudioSpectralAnalysis_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLAudioSpectralAnalysisSize();
            var type = FindTypeByName("MLAudioSpectralAnalysis");

            Assert.IsNull(type);
        }

    }
}