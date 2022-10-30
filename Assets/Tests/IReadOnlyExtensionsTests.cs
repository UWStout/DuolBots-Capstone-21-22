using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DuolBots.Tests
{
    public class IReadOnlyExtensionsTests
    {
        [Test]
        public void ListExtensionTransferTest()
        {
            // Generic Test
            object[] testListThree = { 10, 5, 7, 8 };

            // Small/large values Test
            object[] testListFour = { 0.001, -0.001, 100000000, -100000000 };

            // Assorted datatypes Test
            object[] testListFive = { true, 5, "string", 0.001 };

            // Infinities/null/empty string Test
            object[] testListSix = { double.PositiveInfinity, double.NegativeInfinity, null, "" };

            // Special command/large value/small value/ints in strings Test
            object[] testListSeven = { "\n", int.MaxValue, int.MinValue, "1", "-1" };

            // Single value Test
            object[] testListEight = { 1 };

            // Empty list Test
            object[] testListNine = { };

            // Executing tests
            CompareLists(testListThree);
            CompareLists(testListFour);
            CompareLists(testListFive);
            CompareLists(testListSix);
            CompareLists(testListSeven);
            CompareLists(testListEight);
            CompareLists(testListNine);
        }

        /// <summary>
        /// Transfers the entered list into an array, then compares the values from each in order to make sure they are equal.
        /// </summary>
        /// <param name="readList">The list to read values from</param>
        private void CompareLists(IReadOnlyList<object> readList) {
            List<object> testListOne = readList.ToList<object>();
            object[] testListTwo = readList.ToArray<object>();

            for (int i = 0; i < testListTwo.Length ; i++)
            {
                Assert.AreEqual(testListOne[i], testListTwo[i]);
            }
        }
    }
}
