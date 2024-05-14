using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

using MagicLeap.Android;
//using Is = UnityEngine.TestTools.Constraints.Is;

namespace UnitySDKPlayTests
{
    public class ArrayUtilityTests
    {
        [Test]
        public void ArrayUtilityResizeAndInsertWorks()
        {
            // Using a null array param throws an exception.
            int[] array = null;
            Assert.That(() => ArrayUtility.ResizeAndInsert(ref array, 1, 0), Throws.ArgumentNullException);

            // the index argument must have an absolute value that's less than the size of the array.
            array = new[] { 1, 2, 3 };
            Assert.That(() => ArrayUtility.ResizeAndInsert(ref array, 4, 4), Throws.InstanceOf<ArgumentOutOfRangeException>());

            // positive indices insert from the beginning of the array.
            Assert.That(() => ArrayUtility.ResizeAndInsert(ref array, 2, 2), Throws.Nothing);
            Assert.That(array, Is.EquivalentTo(new[] { 1,2,2,3 }));

            // negative indices insert from the end of the array.
            Assert.That(() => ArrayUtility.ResizeAndInsert(ref array, 4, -1), Throws.Nothing);
            Assert.That(array, Is.EquivalentTo(new[] { 1,2,2,3,4}));
        }


    }
}
