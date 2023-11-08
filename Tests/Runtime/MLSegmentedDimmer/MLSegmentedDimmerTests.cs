using NUnit.Framework;
using System;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public class MLSegmentedDimmerTests
    {
        [Test]
        public void MLSegmentedDimmer_ExistsPositive()
        {
            MLSegmentedDimmer.Activate();
            Assert.IsTrue(MLSegmentedDimmer.Exists);
        }

        [Test]
        public void MLSegmentedDimmer_ExistsNegative()
        {
            MLSegmentedDimmer.Activate();
            Assert.IsFalse(MLSegmentedDimmer.Exists);
        }

        [Test]
        public void MLSegmentedDimmer_GetDefaultLayer()
        {
            try
            {
                MLSegmentedDimmer.Activate();
                MLSegmentedDimmer.GetDefaultLayer();
                Assert.Pass();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
