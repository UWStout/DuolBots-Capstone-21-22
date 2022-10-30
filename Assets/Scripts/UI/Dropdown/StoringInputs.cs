using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Original Authors - Cole Woulf
namespace DuolBots
{
    /// <summary>
    /// Gets all the scripts with CustomInputBinding and stores all the values in a list
    /// </summary>
    public class StoringInputs : MonoBehaviour
    {
        // an array of PassingInput
        private PassingInput[] m_passingInputs;

        /// <summary>
        /// adds all the single CustomInputBinding to an array from all the scripts that
        /// have the PassingInput script
        /// (all parts that can rebind should have this script)
        /// </summary>
        void Start()
        {
             m_passingInputs = FindObjectsOfType<PassingInput>();
        }

        /// <summary>
        /// Pre-Condition: no controls have been past to the battle
        /// Post-Condition: Controls have been past to the battle team through players custom inputs
        /// adds the single CustomInputBinding objects into an inputList and them sets that list to our
        /// BuildSceneInputData class using the setter which then sends the info to the Battle
        /// </summary>
        public void SubmitBinding()
        {
            List<CustomInputBinding> inputList = new List<CustomInputBinding>();
            foreach (PassingInput passingInput in m_passingInputs)
            {
                //inputList.Add(passingInput.getInputBindings());
            }
            // TODO This is a hack. It just sets the data for the first team currently
            // PLEASE FIX - Sincerely Wyatt
            BuildSceneInputData.SetData(0, inputList);
            Debug.Log("Sent Input Info to Battle Team");
        }
    }
}
