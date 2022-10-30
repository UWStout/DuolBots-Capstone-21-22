using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class IReadOnly
{
    // A Test behaves as an ordinary method
    [Test]
    public void IReadOnlyCollectionTesting()
    {
        // Creates a test list with many object types for WrapIndex to iterate through.
        List<object> testList = new List<object> { 3, 20, 5, "orange", true };

        // Testing of a large value in WrapIndex to make sure that it iterates correctly and efficiently at high values.
        Assert.AreEqual(0, testList.WrapIndex(200));

        // Testing of negative values in WrapIndex to make sure that it iterates correctly going backwards.
        Assert.AreEqual(4, testList.WrapIndex(-1));
        Assert.AreEqual(1, testList.WrapIndex(-4));
        Assert.AreEqual(0, testList.WrapIndex(-5));
        Assert.AreEqual(0, testList.WrapIndex(-10));

        // Testing of large negative values to make sure that the goal of both previous tests is met at once.
        Assert.AreEqual(0, testList.WrapIndex(-25));
        Assert.AreEqual(0, testList.WrapIndex(-50));
        Assert.AreEqual(0, testList.WrapIndex(-100));

        // Testing of a large number which is not a multiple of five to balance testing parameters.
        Assert.AreEqual(3, testList.WrapIndex(203));
    }
}
