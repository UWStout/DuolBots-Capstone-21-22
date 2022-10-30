using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DuolBots.Tests
{
    public class BuiltBotInputDataTests
    {
        [Test]
        public void BuiltBotInputDataGeneralTest()
        {
            BuiltBotInputData emptyData = new BuiltBotInputData();

            CustomInputBinding testBindingOne = new CustomInputBinding((byte)(Random.Range(0, 1)), (byte)(Random.Range(0, 5)), eInputType.rightShoulder, (byte)(Random.Range(0, 3)), "IDONE");
            CustomInputBinding testBindingTwo = new CustomInputBinding((byte)(Random.Range(0, 1)), (byte)(Random.Range(0, 5)), eInputType.buttonSouth, (byte)(Random.Range(0, 3)), "IDTWO");
            CustomInputBinding testBindingThree = new CustomInputBinding((byte)(Random.Range(0, 1)), (byte)(Random.Range(0, 5)), eInputType.buttonWest, (byte)(Random.Range(0, 3)), "IDTHREE");
            CustomInputBinding testBindingFour = new CustomInputBinding((byte)(Random.Range(0, 1)), (byte)(Random.Range(0, 5)), eInputType.dPad_Y, (byte)(Random.Range(0, 3)), "IDFOUR");
            CustomInputBinding testBindingFive = new CustomInputBinding((byte)(Random.Range(0, 1)), (byte)(Random.Range(0, 5)), eInputType.select, (byte)(Random.Range(0, 3)), "IDFIVE");

            // IReadOnlyList<CustomInputBinding> testBindingList = new IReadOnlyList<CustomInputBinding>();

            /* testBindingList.Add(testBindingOne);
            testBindingList.Add(testBindingTwo);
            testBindingList.Add(testBindingThree);
            testBindingList.Add(testBindingFour);
            testBindingList.Add(testBindingFive); */
        }
    }
}
