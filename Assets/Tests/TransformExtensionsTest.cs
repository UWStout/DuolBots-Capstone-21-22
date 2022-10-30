using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
// Original Authors - Skyler and Prof Berrier

namespace DuolBots.Tests
{
    public class TransformExtensionsTests
    {
        void GeneralResetTransformTest (Vector3 position, Quaternion rotation, Vector3 scale)
        {
            // Create new object
            GameObject myObject = new GameObject();

            // Set to identity transformation
            myObject.transform.SetPositionAndRotation(position, rotation);
            myObject.transform.localScale = scale;

            // Call function we are testing
            myObject.transform.ResetLocal();

            // Assert the expected changes
            Assert.AreEqual(myObject.transform.localPosition, Vector3.zero, "Position is not zero");
            Assert.AreEqual(myObject.transform.localRotation, Quaternion.identity, "Rotation is not identity");
            Assert.AreEqual(myObject.transform.localScale, Vector3.one, "Scale is not one");
        }

        [Test]
        public void BasicResetTransformTest()
        {
            GeneralResetTransformTest(Vector3.zero, Quaternion.identity, Vector3.one);
        }

        [Test]
        public void RandomizedResetTransformTest()
        {
            GeneralResetTransformTest(Random.insideUnitSphere, Random.rotation, Random.insideUnitSphere);
        }

        [Test]
        public void InfinityResetTransformTest()
        {
            GeneralResetTransformTest(Vector3.positiveInfinity, Quaternion.identity, Vector3.zero);
            GeneralResetTransformTest(Vector3.negativeInfinity, Quaternion.identity, Vector3.zero);
        }
    }
}
