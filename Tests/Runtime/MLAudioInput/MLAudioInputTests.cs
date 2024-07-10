// Disabling deprecated warning for the internal project
#pragma warning disable 618

using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public class MLAudioInputTests
    {
        private const int AUDIO_CLIP_LENGTH_SECONDS = 60;

        private static MLPermissions.Callbacks permissionsCallbacks = new MLPermissions.Callbacks();
        private MLAudioInput.StreamingClip mlAudioStreamingClip;
        private MLAudioInput.BufferClip mlAudioBufferClip;

        private bool callbackReceived = false;
        private bool hasPermission = false;
        private float[] playbackSamples = null;

        [Test]
        public void MLAudioInput_CheckPermission()
        {
            Assert.IsTrue(CheckRecordAudioPermissions());
        }

        [UnityTest]
        public IEnumerator MLAudioInput_CreateStreamingAudioClip()
        {
            SetUp();
            yield return new WaitUntil(() => callbackReceived);
            var captureType = MLAudioInput.MicCaptureType.VoiceCapture;
            mlAudioStreamingClip = new MLAudioInput.StreamingClip(MLAudioInput.MicCaptureType.VoiceCapture, 3, MLAudioInput.GetSampleRate(captureType));
            Assert.IsNotNull(mlAudioStreamingClip.UnityAudioClip);
            TearDown();
        }

        [UnityTest]
        public IEnumerator MLAudioInput_CreateBufferAudioClip()
        {
            SetUp();
            yield return new WaitUntil(() => callbackReceived);
            var captureType = MLAudioInput.MicCaptureType.VoiceCapture;
            mlAudioBufferClip = new MLAudioInput.BufferClip(MLAudioInput.MicCaptureType.VoiceCapture, AUDIO_CLIP_LENGTH_SECONDS, MLAudioInput.GetSampleRate(captureType));
            Assert.GreaterOrEqual(0, mlAudioBufferClip.BufferPosition);
            TearDown();
        }

        // [UnityTest]
        public IEnumerator MLAudioInput_GetStreamingData()
        {
            SetUp();
            yield return new WaitUntil(() => callbackReceived);
            var captureType = MLAudioInput.MicCaptureType.VoiceCapture;
            mlAudioStreamingClip = new MLAudioInput.StreamingClip(MLAudioInput.MicCaptureType.VoiceCapture, 3, MLAudioInput.GetSampleRate(captureType));
            Assert.IsNotNull(mlAudioStreamingClip.UnityAudioClip);
            yield return new WaitForSeconds(1);
            if (hasPermission)
            {
                var result = mlAudioStreamingClip.UnityAudioClip.GetData(playbackSamples, 0);
                Assert.IsTrue(result);
            }
            else
            {
                Assert.Fail("MLAudioInput has no permission");
            }
            TearDown();
        }

        private void SetUp()
        {
            callbackReceived = false;
            hasPermission = false;
            permissionsCallbacks = new MLPermissions.Callbacks();
            permissionsCallbacks.OnPermissionGranted += OnPermissionGranted;
            permissionsCallbacks.OnPermissionDenied += OnPermissionDenied;
            CheckRecordAudioPermissions();
        }

        private void TearDown()
        {
            permissionsCallbacks.OnPermissionGranted -= OnPermissionGranted;
            permissionsCallbacks.OnPermissionDenied -= OnPermissionDenied;

            callbackReceived = false;
            hasPermission = false;

            permissionsCallbacks = null;
            mlAudioStreamingClip = null;
            mlAudioBufferClip = null;
        }

        private void OnPermissionGranted(string permission)
        {
            if (permission == MLPermission.RecordAudio)
            {
                callbackReceived = true;
                hasPermission = true;
            }
        }

        private void OnPermissionDenied(string permission)
        {
            if (permission == MLPermission.RecordAudio)
            {
                callbackReceived = true;
            }
        }

        private static bool CheckRecordAudioPermissions()
        {
            MLPermissions.RequestPermission(MLPermission.RecordAudio, permissionsCallbacks);

            return MLPermissions.CheckPermission(MLPermission.RecordAudio).IsOk;
        }
    }
}
