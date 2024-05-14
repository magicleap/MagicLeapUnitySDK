using System.Collections;
using MagicLeap.Android;
using NUnit.Framework;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using UnityEngine.TestTools;

namespace UnitySDKPlayTests
{
    public class PlayerLoopUtilTests
    {
        struct NonExistentSystem { }

        [Test]
        public void GetInstallPathForSystemWorks()
        {
            // a return value of null indicates a system wasn't found.
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            Assert.That(() => AndroidCameraPlayerLoopUtility.GetInstallPathForSystem(playerLoop, typeof(NonExistentSystem)), Is.Null);

            // a return value of arity 1 (ie, a single item in the array) indicates a "top-level" system
            Assert.That(() => AndroidCameraPlayerLoopUtility.GetInstallPathForSystem(playerLoop, typeof(Initialization)), Is.EquivalentTo(new[] { typeof(Initialization)}));

            // otherwise, the return value is an array of types representing the path down the player loop tree.
            Assert.That(() => AndroidCameraPlayerLoopUtility.GetInstallPathForSystem(playerLoop, typeof(Initialization.XREarlyUpdate)), Is.EquivalentTo(new[]
            {
                typeof(Initialization), typeof(Initialization.XREarlyUpdate)
            }));

        }
    }
}
