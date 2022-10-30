using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DuolBots.Tests
{
    public class CustomInputBindingTests
    {
        [Test]
        public void generalBindingTests()
        {
            // Initial construction of three bindings with parameters: playerIndex, actionIndex, inputType,
            // partSlotID, partUniqueID. Two are equal, one is unequal from the others.
            CustomInputBinding newCustom = new CustomInputBinding(1, 2, eInputType.buttonSouth, 2, "partID");
            CustomInputBinding secondCustom = new CustomInputBinding(1, 2, eInputType.buttonSouth, 2, "partID");
            CustomInputBinding thirdCustom = new CustomInputBinding(3, 5, eInputType.rightShoulder, 2, "partIDD");

            // Test to make sure objects are equal after construction
            Assert.AreEqual(newCustom, secondCustom);

            // Test to make sure objects are equal using their class method, tested both ways
            Assert.AreEqual(newCustom.Equals(secondCustom), true);
            Assert.AreEqual(secondCustom.Equals(newCustom), true);

            // Test to make sure that objects are equal even after one is reconstructed
            secondCustom = new CustomInputBinding(1, 2, eInputType.buttonSouth, 2, "partID");

            Assert.AreEqual(newCustom.Equals(secondCustom), true);
            Assert.AreEqual(newCustom, secondCustom);

            // Test to make sure that objects are not equal after one is reconstructed
            secondCustom = new CustomInputBinding(4, 10, eInputType.buttonSouth, 1, "partID");

            Assert.AreEqual(newCustom.Equals(secondCustom), false);
            Assert.AreNotEqual(newCustom, secondCustom);

            // Test to make sure that objects are not equal after initial constructions
            Assert.AreEqual(newCustom.Equals(thirdCustom), false);
            Assert.AreNotEqual(newCustom, thirdCustom);

            // Test to make sure that unequal objects become equal after reconstruction
            thirdCustom = new CustomInputBinding(1, 2, eInputType.buttonSouth, 2, "partID");

            Assert.AreEqual(newCustom.Equals(thirdCustom), true);
            Assert.AreEqual(newCustom, thirdCustom);
        }
    }
}
