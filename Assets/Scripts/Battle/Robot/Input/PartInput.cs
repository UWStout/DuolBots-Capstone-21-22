using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
// Original Authors - Wyatt Senalik, Aaron Duffey, and Zachary Gross

namespace DuolBots
{
    /// <summary>
    /// Input for a single part.
    /// Serializes callbacks for input using this script's UnityEvents.
    /// </summary>
    public class PartInput : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        // Unity Events for callback serialization
        [SerializeField] private UnityEvent<byte, InputValue> m_inputEvent = default;
        // TODO: We will need to kill these two and build them based on the bot building scene.
        // We only have these in order to build the dictionaries based off of them
        [SerializeField] private List<TempInputMapping>
            m_inputTypeIndexMapTempHackSerializablePlayerOne = new List<TempInputMapping>();
        [SerializeField] private List<TempInputMapping>
            m_inputTypeIndexMapTempHackSerializablePlayerTwo = new List<TempInputMapping>();

        // Maps from an input type to an index of the ability.
        // These need to be initialized from the build scene eventually.
        // The build scene will have to give us mapping from eInputType to a byte (index).
        // This index is an abstracted way for representing what on the part will be controlled
        // by the given input.
        // Example:
        //   index=1: left and right trigger for player A make the turret spin left and right
        //   index=2: left and right trigger for player B make the turret raise and lower.
        //   index=3: (x) for player B fires the turret.
        private Dictionary<eInputType, byte>
            m_inputTypeIndexMapPlayerOne = new Dictionary<eInputType, byte>();
        private Dictionary<eInputType, byte>
            m_inputTypeIndexMapPlayerTwo = new Dictionary<eInputType, byte>();

        /// <summary>
        /// The input types that player one can use for this part.
        /// 
        /// Precondition: Assumes m_inputTypeIndexMapPlayerOne is initialized.
        /// Postcondition: None.
        /// </summary>
        public IReadOnlyList<eInputType> inputTypeListPlayerOne =>
            new List<eInputType>(m_inputTypeIndexMapPlayerOne.Keys);
        /// <summary>
        /// The input types that player two can use for this part.
        /// 
        /// Pre Conditions: Assumes m_inputTypeIndexMapPlayerTwo is initialized.
        /// Post Conditions: None.
        /// </summary>
        public IReadOnlyList<eInputType> inputTypeListPlayerTwo =>
            new List<eInputType>(m_inputTypeIndexMapPlayerTwo.Keys);


        /// <summary>
        /// Called from RobotInputController when the player inputs some eInputType
        /// that is store in either player's inputTypeList.
        ///
        /// Pre Conditions: Assumes player input maps are initialized and the given
        ///   inputType is in the specified player's dictionary.
        /// Post Conditions: inputEvent (UnityEvent) is invoked.
        /// </summary>
        /// <param name="isPlayerOne">Which player made the input.</param>
        /// <param name="inputType">Type of the input.</param>
        public void OnInput(bool isPlayerOne, eInputType inputType, InputValue inputValue)
        {
            // Which player's input type index map
            Dictionary<eInputType, byte> temp_inputTypeIndexMap =
                isPlayerOne ? m_inputTypeIndexMapPlayerOne : m_inputTypeIndexMapPlayerTwo;

            // If the dictionary did not contain the input type
            if (!temp_inputTypeIndexMap.TryGetValue(inputType, out byte temp_index))
            {
                // If the dictionary does not contain the inputType.
                // We should never reach here.
                DebugCatchUnknownInputType(isPlayerOne, inputType);
                return;
            }

            // Invoke the input event
            m_inputEvent.Invoke(temp_index, inputValue);
            // Debug
            CustomDebug.Log($"Player {(isPlayerOne ? "1" : "2")}'s input was seen by part {name}" +
                    $" for input type {inputType}", IS_DEBUGGING);
        }


        #region Debugging
        /// <summary>
        /// Called from OnInput if the inputType is not in the player's dictionary,
        /// which we would hope would never happen.
        /// </summary>
        /// <param name="isPlayerOne"></param>
        /// <param name="inputType"></param>
        private void DebugCatchUnknownInputType(bool isPlayerOne, eInputType inputType)
        {
            // Do not use CustomDebug. This is intended to be Debug.LogError.
            // If we are seeing this, it means something wrong is happening.
            // It's probably one of these two things:
            //   1) Either we didn't initialize the dictionaries correctly.
            //   2) We aren't verifying correctly that given inputType is valid before calling.
            // If its neither of these, it may be that we are changing inputs during runtime
            // by some kind of utility part's functionality.
            // If its none of those, good luck to you.
            Debug.LogError($"PartInput's OnInput was called with an unhandled" +
                $" inputType of {inputType} for player {(isPlayerOne ? "1" : "2")}");
        }
        #endregion Debugging
    }


    /// <summary>
    /// Please kill this eventually.
    /// This exists only for serializing temporary data.
    /// The data in here will be need to be given by the build scene eventually.
    /// </summary>
    [Serializable]
    struct TempInputMapping
    {
        // Input type that controls the part control with the given index
        [SerializeField] private eInputType m_inputType;
        [SerializeField] private byte m_index;

        public eInputType inputType => m_inputType;
        public byte index => m_index;
    }
}
