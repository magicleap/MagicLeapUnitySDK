using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public class MLVoiceTests
    {
        [Test]
        public void MLVoiceIntents_CheckPermission()
        {
            Assert.IsTrue(CheckVoiceIntentsPermissions());
        }

        [UnityTest]
        public IEnumerator MLVoiceIntents_IsStartedPositive()
        {
            yield return MLVoiceIntents_SetUp();
            Assert.IsTrue(MLVoice.IsStarted);
            //yield return MLVoiceIntents_TearDown();
        }

        [Test]
        public void MLVoiceIntents_IsStartedNegative()
        {
            Assert.IsFalse(MLVoice.IsStarted);
        }

        [UnityTest]
        public IEnumerator MLVoiceIntents_SetupVoiceIntents()
        {
            //yield return MLVoiceIntents_SetUp();
            CheckVoiceIntentsPermissions();
            bool isEnabled = MLVoice.VoiceEnabled;
            if (isEnabled)
            {
                MLVoiceIntentsConfiguration config = new();
                MLResult result = MLVoice.SetupVoiceIntents(config);
                yield return new WaitUntil(() => !MLVoice.IsStarted || !MLVoice.VoiceEnabled || result.IsOk);
                Assert.IsTrue(result.IsOk);
            }
            else
            {
                Assert.Fail();
            }
            //yield return MLVoiceIntents_TearDown();
        }

        [UnityTest]
        public IEnumerator MLVoiceIntents_Stop()
        {
            yield return MLVoiceIntents_SetUp();
            MLResult result = MLVoice.Stop();
            yield return new WaitUntil(() => !MLVoice.IsStarted);
            Assert.IsTrue(result.IsOk);
        }

        private IEnumerator MLVoiceIntents_SetUp()
        {
            CheckVoiceIntentsPermissions();
            MLVoice _ = new MLVoice();
            yield return new WaitUntil(() => MLVoice.IsStarted || MLVoice.VoiceEnabled);
        }

        private IEnumerator MLVoiceIntents_TearDown()
        {
            MLVoice.Stop();
            yield return new WaitUntil(() => !MLVoice.IsStarted || !MLVoice.VoiceEnabled);
        }

        private static bool CheckVoiceIntentsPermissions()
        {
            MLPermissions.RequestPermission(MLPermission.VoiceInput, new MLPermissions.Callbacks());

            return MLPermissions.CheckPermission(MLPermission.VoiceInput).IsOk;
        }
    }
}
