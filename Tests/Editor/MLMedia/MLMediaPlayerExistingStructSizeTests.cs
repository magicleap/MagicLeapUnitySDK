using System.Runtime.InteropServices;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLMedia
    {
        public partial class Player : NativeBindingsTests
        {
            [Test]
            public void NativeBindings_MLMediaPlayerTrackDRMInfo_struct_size()
            {
                var capiSize = NativeBindingsTestsProvider.GetMLMediaPlayerTrackDRMInfoSize();
                var type = FindTypeByName("MLMediaPlayerTrackDRMInfo");
                var sdkSize = Marshal.SizeOf(type);

                Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
            }

            [Test]
            public void NativeBindings_MLMediaPlayerSubtitleData_struct_size()
            {
                var capiSize = NativeBindingsTestsProvider.GetMLMediaPlayerSubtitleDataSize();
                var type = FindTypeByName("MLMediaPlayerSubtitleData");
                var sdkSize = Marshal.SizeOf(type);

                Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
            }

            [Test]
            public void NativeBindings_MLMediaPlayerBufferingSettings_struct_size()
            {
                var capiSize = NativeBindingsTestsProvider.GetMLMediaPlayerBufferingSettingsSize();
                var type = FindTypeByName("MLMediaPlayerBufferingSettings");
                var sdkSize = Marshal.SizeOf(type);

                Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
            }

            [Test]
            public void NativeBindings_MLMediaPlayerOnBufferingUpdateInfo_struct_size()
            {
                var capiSize = NativeBindingsTestsProvider.GetMLMediaPlayerOnBufferingUpdateInfoSize();
                var type = FindTypeByName("MLMediaPlayerOnBufferingUpdateInfo");
                var sdkSize = Marshal.SizeOf(type);

                Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
            }

            [Test]
            public void NativeBindings_MLMediaPlayerOnCompletionInfo_struct_size()
            {
                var capiSize = NativeBindingsTestsProvider.GetMLMediaPlayerOnCompletionInfoSize();
                var type = FindTypeByName("MLMediaPlayerOnCompletionInfo");
                var sdkSize = Marshal.SizeOf(type);

                Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
            }

            [Test]
            public void NativeBindings_MLMediaPlayerOnErrorInfo_struct_size()
            {
                var capiSize = NativeBindingsTestsProvider.GetMLMediaPlayerOnErrorInfoSize();
                var type = FindTypeByName("MLMediaPlayerOnErrorInfo");
                var sdkSize = Marshal.SizeOf(type);

                Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
            }

            [Test]
            public void NativeBindings_MLMediaPlayerOnInfoInfo_struct_size()
            {
                var capiSize = NativeBindingsTestsProvider.GetMLMediaPlayerOnInfoInfoSize();
                var type = FindTypeByName("MLMediaPlayerOnInfoInfo");
                var sdkSize = Marshal.SizeOf(type);

                Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
            }

            [Test]
            public void NativeBindings_MLMediaPlayerOnPreparedInfo_struct_size()
            {
                var capiSize = NativeBindingsTestsProvider.GetMLMediaPlayerOnPreparedInfoSize();
                var type = FindTypeByName("MLMediaPlayerOnPreparedInfo");
                var sdkSize = Marshal.SizeOf(type);

                Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
            }

            [Test]
            public void NativeBindings_MLMediaPlayerOnSeekCompleteInfo_struct_size()
            {
                var capiSize = NativeBindingsTestsProvider.GetMLMediaPlayerOnSeekCompleteInfoSize();
                var type = FindTypeByName("MLMediaPlayerOnSeekCompleteInfo");
                var sdkSize = Marshal.SizeOf(type);

                Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
            }

            [Test]
            public void NativeBindings_MLMediaPlayerOnVideoSizeChangedInfo_struct_size()
            {
                var capiSize = NativeBindingsTestsProvider.GetMLMediaPlayerOnVideoSizeChangedInfoSize();
                var type = FindTypeByName("MLMediaPlayerOnVideoSizeChangedInfo");
                var sdkSize = Marshal.SizeOf(type);

                Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
            }

            [Test]
            public void NativeBindings_MLMediaPlayerOnTrackDRMInfo_struct_size()
            {
                var capiSize = NativeBindingsTestsProvider.GetMLMediaPlayerOnTrackDRMInfoSize();
                var type = FindTypeByName("MLMediaPlayerOnTrackDRMInfo");
                var sdkSize = Marshal.SizeOf(type);

                Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
            }

            [Test]
            public void NativeBindings_MLMediaPlayerOnResetCompleteInfo_struct_size()
            {
                var capiSize = NativeBindingsTestsProvider.GetMLMediaPlayerOnResetCompleteInfoSize();
                var type = FindTypeByName("MLMediaPlayerOnResetCompleteInfo");
                var sdkSize = Marshal.SizeOf(type);

                Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
            }

            [Test]
            public void NativeBindings_MLMediaPlayerOnFramePackingInfo_struct_size()
            {
                var capiSize = NativeBindingsTestsProvider.GetMLMediaPlayerOnFramePackingInfoSize();
                var type = FindTypeByName("MLMediaPlayerOnFramePackingInfo");
                var sdkSize = Marshal.SizeOf(type);

                Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
            }

            [Test]
            public void NativeBindings_MLMediaPlayerEventCallbacksEx_struct_size()
            {
                var capiSize = NativeBindingsTestsProvider.GetMLMediaPlayerEventCallbacksExSize();
                var type = FindTypeByName("MLMediaPlayerEventCallbacksEx");
                var sdkSize = Marshal.SizeOf(type);

                Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
            }

            [Test]
            public void NativeBindings_MLMediaPlayerTrackInfo_struct_size()
            {
                var capiSize = NativeBindingsTestsProvider.GetMLMediaPlayerTrackInfoSize();
                var type = FindTypeByName("MLMediaPlayerTrackInfo");
                var sdkSize = Marshal.SizeOf(type);

                Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
            }

            [Test]
            public void NativeBindings_MLMediaPlayerTrackDRMSessionInfo_struct_size()
            {
                var capiSize = NativeBindingsTestsProvider.GetMLMediaPlayerTrackDRMSessionInfoSize();
                var type = FindTypeByName("MLMediaPlayerTrackDRMSessionInfo");
                var sdkSize = Marshal.SizeOf(type);

                Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
            }

            [Test]
            public void NativeBindings_MLMediaPlayerMetrics_struct_size()
            {
                var capiSize = NativeBindingsTestsProvider.GetMLMediaPlayerMetricsSize();
                var type = FindTypeByName("MLMediaPlayerMetrics");
                var sdkSize = Marshal.SizeOf(type);

                Assert.IsTrue(capiSize == (ulong)sdkSize, Log(type.Name, capiSize, sdkSize));
            }

        }
    }
}