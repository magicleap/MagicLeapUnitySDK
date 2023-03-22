using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLMedia 
    {
        [Test]
        public void NativeBindings_MLMediaCodecBufferInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMediaCodecBufferInfoSize();
            var type = FindTypeByName("MLMediaCodecBufferInfo");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLMediaCodecInputBufferInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMediaCodecInputBufferInfoSize();
            var type = FindTypeByName("MLMediaCodecInputBufferInfo");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLMediaCodecOutputBufferInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMediaCodecOutputBufferInfoSize();
            var type = FindTypeByName("MLMediaCodecOutputBufferInfo");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLMediaCodecOutputFormatInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMediaCodecOutputFormatInfoSize();
            var type = FindTypeByName("MLMediaCodecOutputFormatInfo");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLMediaCodecErrorInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMediaCodecErrorInfoSize();
            var type = FindTypeByName("MLMediaCodecErrorInfo");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLMediaCodecCallbacksEx_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMediaCodecCallbacksExSize();
            var type = FindTypeByName("MLMediaCodecCallbacksEx");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLMediaCodecFrameRenderedInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMediaCodecFrameRenderedInfoSize();
            var type = FindTypeByName("MLMediaCodecFrameRenderedInfo");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLMediaCodecOutputFrameListener_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMediaCodecOutputFrameListenerSize();
            var type = FindTypeByName("MLMediaCodecOutputFrameListener");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLMediaCodecCryptoPattern_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMediaCodecCryptoPatternSize();
            var type = FindTypeByName("MLMediaCodecCryptoPattern");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLMediaCodecCryptoInfo_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMediaCodecCryptoInfoSize();
            var type = FindTypeByName("MLMediaCodecCryptoInfo");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLMediaCodecListQueryResults_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMediaCodecListQueryResultsSize();
            var type = FindTypeByName("MLMediaCodecListQueryResults");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLMediaCodecListProfileLevel_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMediaCodecListProfileLevelSize();
            var type = FindTypeByName("MLMediaCodecListProfileLevel");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLMediaExtractorDRMSchemeInitData_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLMediaExtractorDRMSchemeInitDataSize();
            var type = FindTypeByName("MLMediaExtractorDRMSchemeInitData");

            Assert.IsNull(type);
        }
        
        [Test]
        public void NativeBindings_MLCea608CaptionStyleColor_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCea608CaptionStyleColorSize();
            var type = FindTypeByName("MLCea608CaptionStyleColor");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLCea608CaptionPAC_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCea608CaptionPACSize();
            var type = FindTypeByName("MLCea608CaptionPAC");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLCea608CaptionLineEx_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCea608CaptionLineExSize();
            var type = FindTypeByName("MLCea608CaptionLineEx");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLCea608CaptionSegmentEx_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCea608CaptionSegmentExSize();
            var type = FindTypeByName("MLCea608CaptionSegmentEx");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLCea708CaptionColor_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCea708CaptionColorSize();
            var type = FindTypeByName("MLCea708CaptionColor");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLCea708CaptionPenAttr_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCea708CaptionPenAttrSize();
            var type = FindTypeByName("MLCea708CaptionPenAttr");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLCea708CaptionPenColor_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCea708CaptionPenColorSize();
            var type = FindTypeByName("MLCea708CaptionPenColor");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLCea708CaptionPenLocation_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCea708CaptionPenLocationSize();
            var type = FindTypeByName("MLCea708CaptionPenLocation");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLCea708CaptionWindowAttr_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCea708CaptionWindowAttrSize();
            var type = FindTypeByName("MLCea708CaptionWindowAttr");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLCea708CaptionWindow_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCea708CaptionWindowSize();
            var type = FindTypeByName("MLCea708CaptionWindow");

            Assert.IsNull(type);
        }

        [Test]
        public void NativeBindings_MLCea708CaptionEvent_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLCea708CaptionEventSize();
            var type = FindTypeByName("MLCea708CaptionEvent");

            Assert.IsNull(type);
        }
        
        [Test]
        public void NativeBindings_MLPSSHEntry_struct_size()
        {
            var capiSize = NativeBindingsTestsProvider.GetMLPSSHEntrySize();
            var type = FindTypeByName("MLPSSHEntry");

            Assert.IsNull(type);
        }

    }
}