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
    public partial class MLMedia
    {
        public partial class Player : NativeBindingsTests
        {
            private MlSdkDllLoader lib;

            [OneTimeSetUp]
            public void Init()
            {
                lib = new MlSdkDllLoader();
                lib.Load("media_player.magicleap");
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                lib.Free();
            }

            [SetUp]
            public void SetupNativeBindings()
            {
                var apiType = typeof(UnityEngine.XR.MagicLeap.MLMedia.Player);
                nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
            }

            [Test]
            public void NativeBinding_MLMediaPlayerSetEventCallbacksEx_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerSetEventCallbacksEx");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerCreate_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerCreate");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerReset_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerReset");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerSetNextPlayer_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerSetNextPlayer");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerStop_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerStop");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerSetLooping_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerSetLooping");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerGetCurrentPosition_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerGetCurrentPosition");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerGetAudioHandle_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerGetAudioHandle");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerPrepareTrackDRM_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerPrepareTrackDRM");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerAddTimedTextSourceForURI_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerAddTimedTextSourceForURI");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerDestroy_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerDestroy");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerSetDataSourceForFD_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerSetDataSourceForFD");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerSetOffsetDataSourceForFD_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerSetOffsetDataSourceForFD");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerSetDataSourceForPath_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerSetDataSourceForPath");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerSetDataSourceForURI_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerSetDataSourceForURI");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerGetInfo_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerGetInfo");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerSetRemoteDataSourceForURI_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerSetRemoteDataSourceForURI");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerSetMediaStreamSource_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerSetMediaStreamSource");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerSetMediaDataSource_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerSetMediaDataSource");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerAddTimedTextSourceForFD_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerAddTimedTextSourceForFD");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerGetTimedText_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerGetTimedText");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerResetAsync_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerResetAsync");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerPrepare_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerPrepare");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerPrepareAsync_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerPrepareAsync");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerStart_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerStart");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerPause_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerPause");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerSeekTo_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerSeekTo");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerGetVideoSize_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerGetVideoSize");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerGetDuration_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerGetDuration");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerGetBufferingUpdate_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerGetBufferingUpdate");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerGetError_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerGetError");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerGetSubtitleEx_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerGetSubtitleEx");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerReleaseSubtitleEx_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerReleaseSubtitleEx");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerSetOnMediaSubtitleUpdateCallback_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerSetOnMediaSubtitleUpdateCallback");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerGetMetadata_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerGetMetadata");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerSetVideoScalingMode_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerSetVideoScalingMode");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerSetVolume_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerSetVolume");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerPollStates_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerPollStates");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerGetTrackDRMInfo_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerGetTrackDRMInfo");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerGetWebVTTData_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerGetWebVTTData");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerReleaseDRM_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerReleaseDRM");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerGetDefaultBufferingSettings_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerGetDefaultBufferingSettings");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerGetBufferingSettings_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerGetBufferingSettings");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerSetBufferingSettings_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerSetBufferingSettings");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerSetOnMediaTimedTextUpdateCallback_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerSetOnMediaTimedTextUpdateCallback");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerGetMetrics_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerGetMetrics");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerSelectTrack_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerSelectTrack");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerUnselectTrack_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerUnselectTrack");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerGetTrackCount_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerGetTrackCount");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerGetTTMLData_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerGetTTMLData");
            }

            [Test]
            public void NativeBinding_MLMediaPlayerSetSurface_Exists()
            {
                AssertThatMethodExists("MLMediaPlayerSetSurface");
            }
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
