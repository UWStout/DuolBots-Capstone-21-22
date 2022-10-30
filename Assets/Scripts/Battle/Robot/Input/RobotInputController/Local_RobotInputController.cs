using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Wyatt Senalik and Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Local variant of the RobotInputController.
    /// 
    /// Mediator between the PlayerRobotInputController and PartInputs.
    /// Sends out the player's input to the associated parts.
    /// </summary>
    [RequireComponent(typeof(TeamIndex))]
    [RequireComponent(typeof(Shared_RobotInputController))]
    public class Local_RobotInputController : MonoBehaviour, IRobotInputController
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        private Shared_RobotInputController m_sharedController = null;

        public event Action<IReadOnlyList<CustomInputBinding>, CustomInputData> onPartInput;


        // Domestic Initialization
        private void Awake()
        {
            m_sharedController = GetComponent<Shared_RobotInputController>();
            Assert.IsNotNull(m_sharedController, $"{name} does not have " +
                $"{nameof(Shared_RobotInputController)}" +
                $" attached to it but {GetType().Name} requires it.");
        }
        // Foreign Initialization
        private void Start()
        {
            // This must stay in start. We instantiate the bot in start,
            // but unity will not call this start method until the end of this frame.
            // Because of this, we can keep this here and get references to all the parts
            // being spawned since they will be spawned all in the same frame.
            // Putting this in Awake would mean it is called instantly, so we would get none of the parts.
            m_sharedController.InitializeInputPartsMap();
            m_sharedController.InitializePlayerPartInputMaps();
        }


        /// <summary>
        /// Sends input data to parts that desire the given input type.
        ///
        /// Pre Conditions - Dictionaries are initialized.
        /// Post Conditions - All IPartInput that are listening for the given inputType have
        /// their DoPartAction function called.
        /// Note: Change isPlayerOne to a byte?
        /// </summary>
        /// <param name="playerIndex">Which player inputted.</param>
        /// <param name="inputType">What type of input.</param>
        /// <param name="slotIndex">Index of slot whose
        /// part should be controlled.</param>
        public void OnPlayerInput(byte playerIndex, eInputType inputType,
            byte slotIndex, CustomInputData inputValue)
        {
            // Check if this is a used input or not.
            if (!m_sharedController.CheckIfInputIsUsed(playerIndex, inputType,
                out IReadOnlyList<CustomInputBinding> temp_customInpList))
            {
                // Totally okay if we don't use this input.
                return;
            }

            onPartInput?.Invoke(temp_customInpList, inputValue);
            // Execute input for the player's input.
            m_sharedController.ExecuteInput(playerIndex, inputType,
                slotIndex, inputValue);
        }
    }
}
