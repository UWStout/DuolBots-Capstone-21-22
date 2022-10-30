using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
// Original Authors: Skyler Grabowsky

namespace DuolBots.Tests
{
    public class ByteArrayExtensionTests
    {
        void GeneralByteExtensionTest(object obj)
        {
            // Transfers the inserted object into a byte array.
            byte[] byteArray = obj.ToByteArray();

            // Changes the byte array back into an object type.
            object arrayedObject = byteArray.ToObject();

            // Tests for if the objects are still the same after being transferred.
            Assert.AreEqual(obj, arrayedObject, "Objects are not the same.");
        }

        // A Test behaves as an ordinary method
        [Test]
        public void ByteArrayExtensionTransferTest()
        {
            // Instantiating several basic values for transfer testing.
            bool testBool = true;
            bool otherTestBool = false;
            int testInt = 1;
            int secondTestInt = -1;
            int thirdTestInt = int.MaxValue;
            int fourthTestInt = int.MinValue;
            double testDouble = double.PositiveInfinity;
            double secondTestDouble = double.NegativeInfinity;
            double thirdTestDouble = double.MaxValue;
            double fourthTestDouble = double.MinValue;
            double fifthTestDouble = double.NaN;
            double sixthTestDouble = 0.0000000000001;
            double seventhTestDouble = -0.0000000000001;
            string testString = "";
            string secondTestString = "a";
            string thirdTestString = "1";
            string fourthTestString = "-1";
            string fifthTestString = "NULL";
            string sixthTestString = "\n";
            string seventhTestString = "Console.WriteLine('String')";
            string eighthTestString = "1101";
            char testChar = 'a';
            char secondTestChar = '1';

            // Tests the transfer of both basic booleans.
            GeneralByteExtensionTest(testBool);
            GeneralByteExtensionTest(otherTestBool);

            // Tests the transfer of basic ints, positive and negative.
            GeneralByteExtensionTest(testInt);
            GeneralByteExtensionTest(secondTestInt);

            // Tests the transfer of int type maxvalue and minvalue.
            GeneralByteExtensionTest(thirdTestInt);
            GeneralByteExtensionTest(fourthTestInt);

            // Tests the transfer of positive and negative infinity.
            GeneralByteExtensionTest(testDouble);
            GeneralByteExtensionTest(secondTestDouble);

            // Tests the transfer of double type maxvalue and minvalue.
            GeneralByteExtensionTest(thirdTestDouble);
            GeneralByteExtensionTest(fourthTestDouble);

            // Tests the transfer of NaN.
            GeneralByteExtensionTest(fifthTestDouble);

            // Tests the transfer of value small double values.
            GeneralByteExtensionTest(sixthTestDouble);
            GeneralByteExtensionTest(seventhTestDouble);

            // Tests the transfer of an empty string.
            GeneralByteExtensionTest(testString);

            // Tests the transfer of a single character string.
            GeneralByteExtensionTest(secondTestString);

            // Tests the transfer of strings with numerical values.
            GeneralByteExtensionTest(thirdTestString);
            GeneralByteExtensionTest(fourthTestString);

            // Tests the transfer of strings that could be interpreted as other values or object types.
            GeneralByteExtensionTest(fifthTestString);
            GeneralByteExtensionTest(sixthTestString);
            GeneralByteExtensionTest(seventhTestString);
            GeneralByteExtensionTest(eighthTestString);

            // Tests the transfer of characters.
            GeneralByteExtensionTest(testChar);
            GeneralByteExtensionTest(secondTestChar);
        }
    }
}
