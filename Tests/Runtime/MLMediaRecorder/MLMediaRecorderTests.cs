using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Profiling;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;
using static UnityEngine.XR.MagicLeap.MeshingSubsystem.Extensions.MLMeshing;

namespace UnitySDKPlayTests
{
    // We separate this from camera because its large enough, even if its not used 
    // solely by itself
    public class MLMediaRecorderTests
    {
        MLMediaRecorder recorder;
        MLMediaFormat format;
        MLResult result;
        string filePath = Path.Combine(Application.temporaryCachePath,"out_file_test");
        
        #region Test Parameters
        private static IEnumerable OutputFormatsToTest = new[]
        {
            MLMediaRecorder.OutputFormat.Default,
            MLMediaRecorder.OutputFormat.THREE_GPP,
            MLMediaRecorder.OutputFormat.MPEG_4,
            MLMediaRecorder.OutputFormat.AMR_NB,
            MLMediaRecorder.OutputFormat.AMR_WB,
            MLMediaRecorder.OutputFormat.AAC_ADIF,
            MLMediaRecorder.OutputFormat.AAC_ADTS,
            MLMediaRecorder.OutputFormat.RTP_AVP,
            MLMediaRecorder.OutputFormat.MPEG2TS,
            MLMediaRecorder.OutputFormat.WEBM,
            MLMediaRecorder.OutputFormat.HEIF,
            MLMediaRecorder.OutputFormat.OGG,
        };

        private static IEnumerable AudioSourcesToTest = new[]
        {
            MLMediaRecorder.AudioSource.Voice,
            MLMediaRecorder.AudioSource.World,
            MLMediaRecorder.AudioSource.Virtual,
            MLMediaRecorder.AudioSource.Mixed,
        };

        private static IEnumerable VideoEncodersToTest = new[]
        { 
            MLMediaRecorder.VideoEncoder.Default,
            MLMediaRecorder.VideoEncoder.H263,
            MLMediaRecorder.VideoEncoder.H264,
            MLMediaRecorder.VideoEncoder.MPEG_4_SP,
            MLMediaRecorder.VideoEncoder.VP8,
            MLMediaRecorder.VideoEncoder.HEVC,
        };

        private static IEnumerable AudioEncodersToTest = new[]
        { 
            MLMediaRecorder.AudioEncoder.Default,
            MLMediaRecorder.AudioEncoder.AMR_NB,
            MLMediaRecorder.AudioEncoder.AMR_WB,
            MLMediaRecorder.AudioEncoder.AAC,
            MLMediaRecorder.AudioEncoder.HE_AAC,
            MLMediaRecorder.AudioEncoder.AAC_ELD,
            MLMediaRecorder.AudioEncoder.VORBIS,
            MLMediaRecorder.AudioEncoder.OPUS,

        };
        #endregion

        [SetUp]
        public void SetUp()
        {
            //Condensing create and setvideosource into this for later tests convenience
            recorder = MLMediaRecorder.Create();
            Assert.IsNotNull(recorder, "Setup:MLMediaRecorder.Create failed; recorder null");
            result = recorder.SetVideoSource(MLMediaRecorder.VideoSource.Camera);
            Assert.IsTrue(result.IsOk, "Setup:SetVideoSource fail");
            format = MLMediaFormat.CreateEmpty();

            //gently lifted from CameraRecorder.cs, used for prepare and start tests
            //some later tests may overwrite these specific configs; this is a general setup 
            format.SetValue(MLMediaFormatKey.Width, 1920);
            format.SetValue(MLMediaFormatKey.Height, 1080);
            format.SetValue(MLMediaFormatKey.Frame_Rate, 30);
            format.SetValue(MLMediaFormatKey.Parameter_Video_Bitrate, 20000000);
            format.SetValue(MLMediaFormatKey.Bit_Rate, 96000);
            format.SetValue(MLMediaFormatKey.Channel_Count, 1);
            format.SetValue(MLMediaFormatKey.Sample_Rate, 16000);
        }

        [UnityTest]
        public IEnumerator MLMediaRecorder_SetAudioSource(
            [ValueSource(nameof(AudioSourcesToTest))] MLMediaRecorder.AudioSource source)
        {
            result = recorder.SetAudioSource(source);
            Assert.IsTrue(result.IsOk, string.Format("SetAudioSource fail: {0}", source.ToString()));
            yield return null;
        }

        [UnityTest]
        public IEnumerator MLMediaRecorder_SetOutputFormat(
            [ValueSource(nameof(OutputFormatsToTest))] MLMediaRecorder.OutputFormat outputFormat)
        {
            result = recorder.SetOutputFormat(outputFormat);
            Assert.IsTrue(result.IsOk, string.Format("SetOutputFormat fail: {0}", outputFormat.ToString()));
            yield return null;
        }

        [Test]
        public void MLMediaRecorder_SetOutputFileForFD()
        {
            // Not really sure how we're supposed to get an unmanaged FD for this method, so 
            // we have it present but leave it as unimplemented.
            Assert.Ignore("Not implemented");
        }

        [UnityTest]
        public IEnumerator MLMediaRecorder_SetVideoEncoder(
            [ValueSource(nameof(VideoEncodersToTest))] MLMediaRecorder.VideoEncoder encoder)
        {
            result = recorder.SetOutputFormat(MLMediaRecorder.OutputFormat.Default);
            Assert.IsTrue(result.IsOk, "SetOutputFormat fail");
            result = recorder.SetVideoEncoder(encoder);
            Assert.IsTrue(result.IsOk, string.Format("SetVideoEncoder fail: {0}", encoder.ToString()));
            yield return null;
        }

        [UnityTest]
        public IEnumerator MLMediaRecorder_SetAudioEncoder(
            [ValueSource(nameof(AudioEncodersToTest))] MLMediaRecorder.AudioEncoder encoder)
        {
            result = recorder.SetOutputFormat(MLMediaRecorder.OutputFormat.THREE_GPP);
            Assert.IsTrue(result.IsOk, "SetOutputFormat fail");
            result = recorder.SetVideoEncoder(MLMediaRecorder.VideoEncoder.Default);
            Assert.IsTrue(result.IsOk, "SetVideoEncoder fail");
            result = recorder.SetAudioEncoder(encoder);
            Assert.IsTrue(result.IsOk, string.Format("SetAudioEncoder fail: {0}", encoder.ToString()));
            yield return null;
        }

        [Test]
        public void MLMediaRecorder_SetMaxDuration()
        {
            recorder.SetOutputFormat(MLMediaRecorder.OutputFormat.Default);
            result = recorder.SetMaxDuration(0);
            Assert.IsTrue(result.IsOk, string.Format("SetMaxDuration fail: {0}", result));
        }

        [Test]
        public void MLMediaRecorder_SetOutputFilePath()
        {
            recorder.SetOutputFormat(MLMediaRecorder.OutputFormat.Default);
            result = recorder.SetOutputFileForPath(filePath);
            Assert.IsTrue(result.IsOk, string.Format("SetOutputFormat fail: {0}; path is {1}", result, filePath));
        }

        [Test]
        public void MLMediaRecorder_SetMaxFileSize()
        {
            recorder.SetOutputFormat(MLMediaRecorder.OutputFormat.Default);
            result = recorder.SetMaxFileSize(0);
            Assert.IsTrue(result.IsOk, string.Format("SetMaxFileSize fail: {0}", result));
        }

        [Test]
        public void MLMediaRecorder_SetGeoLocation()
        {
            result = recorder.SetGeoLocation(0, 0); //Null island -- I had to make sure this existed
            Assert.IsTrue(result.IsOk, "SetGeoLocation failed with 0, 0");
        }

        [Test]
        public void MLMediaRecorder_Prepare()
        {
            ExtraSetup();
            result = recorder.Prepare(format);
            Assert.IsTrue(result.IsOk, string.Format("Prepare fail: {0}", result));
        }

        [Test]
        public void MLMediaRecorder_Start()
        {
            ExtraSetup();
            Assert.IsTrue(result.IsOk, string.Format("SetOutputFileForPath fail: {0}, path is {1}", result, filePath));
            result = recorder.Prepare(format);
            Assert.IsTrue(result.IsOk, string.Format("Prepare fail: {0}", result));
            result = recorder.Start();
            Assert.IsTrue(result.IsOk, string.Format("Start fail: {0}", result));
        }

        [Test]
        public void MLMediaRecorder_GetInputSurface()
        {
            ExtraSetup();
            result = recorder.Prepare(format);
            Assert.IsTrue(result.IsOk, string.Format("Prepare fail: {0}", result));
            result = recorder.Start();
            Assert.IsTrue(result.IsOk, string.Format("Start fail: {0}", result));
            result = recorder.GetInputSurface();
            Assert.IsTrue(result.IsOk, string.Format("GetInputSurface fail: {0}", result));
        }

        [Test]
        public void MLMediaRecorder_GetMaxAmplitude()
        {
            ExtraSetup();
            result = recorder.Prepare(format);
            Assert.IsTrue(result.IsOk, string.Format("Prepare fail: {0}", result));
            result = recorder.Start();
            Assert.IsTrue(result.IsOk, string.Format("Start fail: {0}", result));
            result = recorder.GetMaxAmplitude(out int _);
            Assert.IsTrue(result.IsOk, string.Format("GetMaxAmplitude fail: {0}", result));
        }

        [Test]
        public void MLMediaRecorder_Stop()
        {
            ExtraSetup();
            result = recorder.Prepare(format);
            Assert.IsTrue(result.IsOk, string.Format("Prepare fail: {0}", result));
            result = recorder.Start();
            Assert.IsTrue(result.IsOk, string.Format("Start fail: {0}", result));
            result = recorder.Stop();
            Assert.IsTrue(result.IsOk, string.Format("Stop fail: {0}", result));
        }

        [Test]
        public void MLMediaRecorder_Reset()
        {
            MLResult result = recorder.Reset();
            Assert.IsTrue(result.IsOk);
        }

        private void ExtraSetup()
        {
            // This method is specifically used when doing tests that need to call
            // MLMediaRecorder.Start()
            result = recorder.SetOutputFormat(MLMediaRecorder.OutputFormat.Default);
            Assert.IsTrue(result.IsOk, string.Format("SetOutputFormat fail: {0}", result));
            result = recorder.SetVideoEncoder(MLMediaRecorder.VideoEncoder.Default);
            Assert.IsTrue(result.IsOk, string.Format("SetVideoEncoder fail: {0}", result));
            result = recorder.SetOutputFileForPath(filePath);
            Assert.IsTrue(result.IsOk, string.Format("SetOutputFileForPath fail: {0}, path is {1}", result, filePath));
        }

    }

}
