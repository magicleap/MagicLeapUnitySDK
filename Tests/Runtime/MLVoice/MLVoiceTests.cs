using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public class MLVoiceTests
    {
        [SetUp]
        public void MLVoice_CheckIsEnabled()
        {
            Assert.That(MLVoice.VoiceEnabled, "Voice intents are not enabled in device settings");
        }
        
        [Test]
        public void MLVoice_SetupVoiceIntents()
        {
            MLVoiceIntentsConfiguration config = new();
            MLResult result = MLVoice.SetupVoiceIntents(config);
            Assert.That(result.IsOk, result.Result.ToString());
        }

        [Test]
        public void MLVoice_SetupVoiceIntents_WithJSON()
        {
            // TODO: supply a sample of valid VoiceIntents JSON and test against that
            MLResult result = MLVoice.SetupVoiceIntents("{}");
            Assert.Pass("Not yet tested at this time");
        }

        [Test]
        public void MLVoice_Stop()
        {
            MLResult result = MLVoice.Stop();
            Assert.That(result.IsOk, result.Result.ToString());
        }
    }
}
