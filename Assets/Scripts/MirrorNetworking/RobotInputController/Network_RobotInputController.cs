using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Network variant of the RobotInputController.
    ///
    /// Mediator between the PlayerRobotInputController and PartInputs.
    /// Sends out the player's input to the associated parts.
    /// </summary>
    [RequireComponent(typeof(NetworkTeamIndex))]
    [RequireComponent(typeof(Shared_RobotInputController))]
    public class Network_RobotInputController : NetworkBehaviour, IRobotInputController
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        private Shared_RobotInputController m_sharedController = null;
        private NetworkTeamIndex m_teamIndex = null;

        public event Action<IReadOnlyList<CustomInputBinding>, CustomInputData> onPartInput;


        // Domestic Initialization
        private void Awake()
        {
            m_teamIndex = GetComponent<NetworkTeamIndex>();
            Assert.IsNotNull(m_teamIndex, $"{name} does not have {nameof(NetworkTeamIndex)}" +
                $" attached to it but {GetType().Name} requires it.");
            m_sharedController = GetComponent<Shared_RobotInputController>();
            Assert.IsNotNull(m_sharedController, $"{name} does not have " +
                $"{nameof(Shared_RobotInputController)}" +
                $" attached to it but {GetType().Name} requires it.");
        }
        public override void OnStartServer()
        {
            base.OnStartServer();
            CustomDebug.Log($"{GetType().Name}'s " +
                $"{nameof(OnStartServer)}", IS_DEBUGGING);
            m_sharedController.InitializeInputPartsMap();
        }
        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            CustomDebug.Log($"{GetType().Name}'s " +
                $"{nameof(OnStartAuthority)}", IS_DEBUGGING);

            // Before we try to access using our team index, we need to update our
            // BuildSceneInputData to reflect our real team index instead of the
            // team index we used when we were local (0).
            byte temp_originalTeamIndex = 0;
            BuiltBotInputData temp_teamInputData =
                BuildSceneInputData.GetBuiltBotInputData(temp_originalTeamIndex);
            Assert.IsNotNull(temp_teamInputData, $"There was no specified input " +
                $"data for team {temp_originalTeamIndex}'s bot");
            Assert.AreNotEqual(0, temp_teamInputData.customInputBindings.Count,
                $"The input data list of custom inputs was empty");
            BuildSceneInputData.SetData(m_teamIndex.teamIndex, temp_teamInputData);

            // Send the input binding data to the server.
            CmdSendBuildSceneInputData(temp_teamInputData);

            m_sharedController.InitializePlayerPartInputMaps();
        }


        /// <summary>
        /// Requests to the server that the player's input be accepted.
        /// </summary>
        /// <param name="playerIndex">Which player inputted.</param>
        /// <param name="inputType">What type of input.</param>
        /// <param name="slotIndex">Index of slot whose
        /// part should be controlled.</param>
        /// <param name="inputValue">Value of the player's input.</param>
        public void OnPlayerInput(byte playerIndex, eInputType inputType,
            byte slotIndex, CustomInputData inputValue)
        {
            //CustomDebug.Log($"OnPlayerInput {nameof(playerIndex)}={playerIndex}, " +
            //    $"{nameof(inputType)}={inputType}, {nameof(inputValue)}={inputValue}",
            //    IS_DEBUGGING);
            // Before we send the request to the server, confirm that the input
            // is one that is being used by the team.

            // Check if this is a used input or not.
            if (!m_sharedController.CheckIfInputIsUsed(playerIndex, inputType,
                out IReadOnlyList<CustomInputBinding> temp_customInpList))
            {
                // Totally okay if we don't use this input.
                return;
            }

            #region Logs
            CustomDebug.LogForComponent($"InputValue to request: " +
                $"{inputValue}", this, IS_DEBUGGING);
            #endregion Logs
            onPartInput?.Invoke(temp_customInpList, inputValue);
            CmdClientRequestInput(m_teamIndex.teamIndex, playerIndex, inputType,
                slotIndex, inputValue);
        }


        [Command]
        private void CmdSendBuildSceneInputData(BuiltBotInputData inputData)
        {
            CustomDebug.Log($"CmdSendBuildSceneInputData for " +
                $"{m_teamIndex.teamIndex}", IS_DEBUGGING);
            Assert.IsNotNull(inputData, $"The sent input data for " +
                $"team {m_teamIndex.teamIndex} is null");
            Assert.AreNotEqual(0, inputData.customInputBindings.Count,
                $"The sent input data for team {m_teamIndex.teamIndex} has " +
                $"an empty list of input bindings.");
            
            BuildSceneInputData.SetData(m_teamIndex.teamIndex, inputData);
            // After receiving the data, initialize the player part input maps with it.
            m_sharedController.InitializePlayerPartInputMaps();
        }
        /// <summary>
        /// Sends input data to parts that desire the given input type.
        ///
        /// Pre Conditions - Dictionaries are initialized. The given combination of player
        /// and inputType are used for the team.
        /// Post Conditions - All IPartInput that are listening for the given inputType have
        /// their DoPartAction function called.
        /// Note: Change isPlayerOne to a byte?
        /// </summary>
        /// <param name="teamIndex">Which team inputted.</param>
        /// <param name="playerIndex">Which player inputted.</param>
        /// <param name="inputType">What type of input.</param>
        /// <param name="slotIndex">Index of slot whose
        /// part should be controlled.</param>
        /// <param name="inputValue">Value of the input.</param>
        [Command]
        private void CmdClientRequestInput(byte teamIndex,
            byte playerIndex, eInputType inputType, byte slotIndex,
            CustomInputData inputValue)
        {
            CustomDebug.Log($"{nameof(CmdClientRequestInput)} with " +
                $"({nameof(teamIndex)}={teamIndex}) ({nameof(playerIndex)}=" +
                $"{playerIndex}) ({nameof(inputType)}={inputType}) " +
                $"({nameof(inputValue)}={inputType})", IS_DEBUGGING);

            if (teamIndex != m_teamIndex.teamIndex)
            {
                CustomDebug.Log($"Invalid team index {teamIndex} when " +
                    $"{m_teamIndex.teamIndex} was expected.", IS_DEBUGGING);
                return;
            }

            // Do all the actions for the current binding.
            bool temp_wasValidInputForPlayer = m_sharedController.ExecuteInput(
                playerIndex, inputType, slotIndex, inputValue);
            // Input should have been validated before being sent to the server,
            // so hopefully this if is never executed.
            if (!temp_wasValidInputForPlayer)
            {
                // Should never reach here, we should be validating if the input is used
                // client side.
                Debug.LogError($"An unused input ({inputType}) was sent " +
                    $"to the server for player {playerIndex} on team " +
                    $"{m_teamIndex.teamIndex}.");
                return;
            }
        }
    }
}
