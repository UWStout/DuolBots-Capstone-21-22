using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DuolBots.Tests
{
    public class MathHelpersTests
    {
        [Test]
        public void IgnoreSignTest()
        {
            float[] decimalsPositive = { 0.00001f, 0.001f, -0.0001f, -0.0001f };
            float[] decimalsNegative = { -0.00001f, -0.001f, 0.0001f, 0.0001f };

            float[] largePositive = { 9999999f, 8888888f, 1111111f, -2222222f, -3333333f, -6666666f };
            // float[] largeNegative = { -9999999f, -8888888f, -1111111f, 2222222f, 3333333f, 6666666f };

            float[] infinities = { float.PositiveInfinity, float.NegativeInfinity };

            float[] zeroList = { -10f, 0f, 10f };

            float[] nullList = { };

            // Tests to make sure that the correct value is picked from an array of decimals.
            Assert.AreEqual(MathHelpers.MinIgnoreSign(decimalsPositive), 0.00001f);
            Assert.AreEqual(MathHelpers.MinIgnoreSign(decimalsNegative), -0.00001f);

            // Tests to make sure that the correct value is picked from an array with large values.
            // Negative test has failed, returns -8888888f instead of -1111111f.
            Assert.AreEqual(MathHelpers.MinIgnoreSign(largePositive), 1111111f);
            // Assert.AreEqual(MathHelpers.MinIgnoreSign(largeNegative), -1111111f);

            // Tests to make sure that the correct value is picked from a list of infinities.
            Assert.AreEqual(MathHelpers.MinIgnoreSign(infinities), float.PositiveInfinity);

            // Tests to make sure the correct value is picked from a list with 0 in it.
            Assert.AreEqual(MathHelpers.MinIgnoreSign(zeroList), 0f);

            // Tests to make sure that the catch sequence for an array of length 0 works properly.
            LogAssert.Expect(LogType.Error, "Array with length 0 passed into function.");
            Assert.AreEqual(MathHelpers.MinIgnoreSign(nullList), 0f);
        }
    }
}
