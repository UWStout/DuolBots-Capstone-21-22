using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Shared functions for variants of the RobotInputController.
    /// </summary>
    public class Shared_RobotInputController : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        // Dictionary for identifying parts by their slot index (instance id)
        private Dictionary<byte, IPartInput> m_inputPartsMap =
            new Dictionary<byte, IPartInput>();

        // Map of parts for player one based on input
        private Dictionary<eInputType, List<CustomInputBinding>>
            m_inputBindingMapPlayerOne = new Dictionary<eInputType,
                List<CustomInputBinding>>();
        // Map of parts for player two based on input
        private Dictionary<eInputType, List<CustomInputBinding>>
            m_inputBindingMapPlayerTwo = new Dictionary<eInputType,
                List<CustomInputBinding>>();

        private ITeamIndex m_teamIndex = null;

        public Action<CustomInputBinding> onPartInput;


        // Domestic Initialization
        private void Awake()
        {
            m_teamIndex = GetComponent<ITeamIndex>();
            Assert.IsNotNull(m_teamIndex, $"{name} does not have " +
                $"{nameof(ITeamIndex)} attached to it but {GetType().Name} " +
                $"requires it.");
        }


        /// <summary>
        /// Initializes the dictionary for identifying parts by their unique id.
        ///
        /// Pre Conditions - All parts are already attached to child objects of
        /// this GameObject and already have their IPartInputs initialized.
        /// Post Conditions - m_inputPartsMap is filled with IPartInput's with
        /// their unique ID as the key.
        /// </summary>
        public void InitializeInputPartsMap()
        {
            // new up the dictionary
            m_inputPartsMap = new Dictionary<byte, IPartInput>();

            // Gather the input parts
            IPartInput[] temp_botInputs = GetComponentsInChildren<IPartInput>();
            Assert.AreNotEqual(0, temp_botInputs.Length, $"No " +
                $"{nameof(IPartInput)}'s were found as a child of {name}");

            // Add each part to the dictionary
            foreach (IPartInput temp_currentPartInp in temp_botInputs)
            {
                // Pull off the slot index. Assumes that the part slot index
                // is either on the same object, or a parent of the IPartInput
                PartSlotIndex temp_slotIndex = temp_currentPartInp.
                    GetComponentInParent<PartSlotIndex>();
                Assert.IsNotNull(temp_slotIndex, $"{temp_currentPartInp.name} " +
                    $"is supposed to have a {nameof(PartSlotIndex)} attached to " +
                    $"its parent, but none was found.");

                // There should not be multiple parts in the same slot index.
                if (m_inputPartsMap.TryGetValue(temp_slotIndex.slotIndex,
                    out IPartInput temp_partInp))
                {
                    Debug.LogError($"Found multiple parts with the slot index " +
                        $"{temp_slotIndex.slotIndex}");
                }
                // Add the part to the dictionary.
                else
                {
                    m_inputPartsMap.Add(temp_slotIndex.slotIndex,
                        temp_currentPartInp);
                }
            }

            // Debug the initialized input parts
            DebugInputPartsMap();
        }
        /// <summary>
        /// Creates the maps for each player from an eInputType to all the
        /// CustomInputBindings that use that eInputType.
        /// 
        /// Pre Conditions - Dictionary for storing IPartInputs based on their
        /// inputType is initialized. There exists a BuildSceneInputData in the
        /// scene and it has been initialzied.
        /// Post Conditions - Both player's input binding maps are initialized
        /// with every custom input binding.
        /// </summary>
        public void InitializePlayerPartInputMaps()
        {
            CustomDebug.Log($"{nameof(InitializePlayerPartInputMaps)}",
                IS_DEBUGGING);
            // new up the dictionaries
            m_inputBindingMapPlayerOne =
                new Dictionary<eInputType, List<CustomInputBinding>>();
            m_inputBindingMapPlayerTwo =
                new Dictionary<eInputType, List<CustomInputBinding>>();


            // Get the data for our robot
            BuiltBotInputData temp_robotInputData =
                BuildSceneInputData.GetBuiltBotInputData(m_teamIndex.teamIndex);
            Assert.IsNotNull(temp_robotInputData, $"No Input Data was specified " +
                $"for team {m_teamIndex.teamIndex}");
            Assert.AreNotEqual(0, temp_robotInputData.customInputBindings.Count,
                $"The InputBindingsList stored in BuiltBotInputData was empty " +
                $"for team {m_teamIndex.teamIndex}");
            // Iterate over its input bindings data
            foreach (CustomInputBinding temp_currentInpBinding
                in temp_robotInputData.customInputBindings)
            {
                Assert.IsNotNull(temp_currentInpBinding, $"VERY BAD! There was a " +
                    $"null CustomInputBinding stored in BuildSceneInputData for " +
                    $"team {m_teamIndex.teamIndex}");
                // TODO fix this
                // Player 1
                if (temp_currentInpBinding.playerIndex == 0)
                {
                    AddCustomBindingToDictionary(ref m_inputBindingMapPlayerOne,
                        temp_currentInpBinding);
                }
                // Player 2
                else
                {
                    AddCustomBindingToDictionary(ref m_inputBindingMapPlayerTwo,
                        temp_currentInpBinding);
                }
            }

            // Debug the input maps after creating them
            DebugPlayerInputBindingsMaps();
        }
        /// <summary>
        /// Checks if the input is used for the given player.
        /// 
        /// Pre Conditions - Dictionaries are initialized.
        /// Post Conditions - No changes are made. Returns if the input is
        /// used or not.
        /// </summary>
        /// <param name="playerIndex">Which player inputted.</param>
        /// <param name="inputType">What type of input.</param>
        /// <returns>If the input is used for the specified player.</returns>
        public bool CheckIfInputIsUsed(byte playerIndex, eInputType inputType)
        {
            return CheckIfInputIsUsed(playerIndex, inputType, out _);
        }
        /// <summary>
        /// Checks if the input is used for the given player.
        /// 
        /// Pre Conditions - Dictionaries are initialized.
        /// Post Conditions - No changes are made. Returns if the input is
        /// used or not.
        /// </summary>
        /// <param name="playerIndex">Which player inputted.</param>
        /// <param name="inputType">What type of input.</param>
        /// <returns>If the input is used for the specified player.</returns>
        public bool CheckIfInputIsUsed(byte playerIndex, eInputType inputType,
            out IReadOnlyList<CustomInputBinding> customInputBindings)
        {
            // Determine whose input it was
            Dictionary<eInputType, List<CustomInputBinding>> temp_curPlayerInputs =
                playerIndex == 0 ?
                m_inputBindingMapPlayerOne : m_inputBindingMapPlayerTwo;

            // Check if this is a used input or not
            if (!temp_curPlayerInputs.ContainsKey(inputType))
            {
                customInputBindings = null;
                // Unused action.
                return false;
            }

            customInputBindings = temp_curPlayerInputs[inputType];
            // Action is used.
            return true;
        }
        /// <summary>
        /// Checks if the input is used for the given player for the
        /// given slot.
        /// 
        /// Pre Conditions - Dictionaries are initialized.
        /// Post Conditions - No changes are made. Returns if the input is
        /// used or not.
        /// </summary>
        /// <param name="playerIndex">Which player inputted.</param>
        /// <param name="inputType">What type of input.</param>
        /// <param name="slotIndex">Slot to check if the input is used for.</param>
        /// <returns>If the input is used for the specified player
        /// for the specified slot.</returns>
        public bool CheckIfInputIsUsedBySpecificSlot(byte playerIndex,
            eInputType inputType, byte slotIndex)
        {
            return CheckIfInputIsUsedBySpecificSlot(playerIndex, inputType,
                slotIndex, out _);
        }
        /// <summary>
        /// Checks if the input is used for the given player for the
        /// given slot.
        /// 
        /// Pre Conditions - Dictionaries are initialized.
        /// Post Conditions - No changes are made. Returns if the input is
        /// used or not.
        /// </summary>
        /// <param name="playerIndex">Which player inputted.</param>
        /// <param name="inputType">What type of input.</param>
        /// <param name="slotIndex">Slot to check if the input is used for.</param>
        /// <param name="customInputBindings">Bindings used for the given input
        /// for the player index and slot index.</param>
        /// <returns>If the input is used for the specified player
        /// and for the specified slot.</returns>
        public bool CheckIfInputIsUsedBySpecificSlot(byte playerIndex,
            eInputType inputType, byte slotIndex,
            out IReadOnlyList<CustomInputBinding> customInputBindings)
        {
            List<CustomInputBinding> temp_bindsForSlot
                = new List<CustomInputBinding>();

            // If its simply not used at all, then it can't be used by the slot.
            if (!CheckIfInputIsUsed(playerIndex, inputType,
                out IReadOnlyList<CustomInputBinding> temp_rawInpBinds))
            {
                customInputBindings = temp_bindsForSlot;
                return false;
            }

            
            // Check if the part is in any input binding
            foreach (CustomInputBinding temp_bind in temp_rawInpBinds)
            {
                // Slot index matches, so a binding does use it.
                if (temp_bind.partSlotID == slotIndex)
                {
                    temp_bindsForSlot.Add(temp_bind);
                }
                // If the current part binding slotID is for movement,
                // add it, since you don't need to select movement parts to move.
                else if (temp_bind.partSlotID
                    == PartSlotIndex.MOVEMENT_PART_SLOT_ID)
                {
                    temp_bindsForSlot.Add(temp_bind);
                }
            }

            customInputBindings = temp_bindsForSlot;
            // Return true if there is at least 1 input binding for the specified
            // slot and player. Otherwise, false.
            return customInputBindings.Count > 0;
        }
        /// <summary>
        /// Sends input data to parts that desire the given input type.
        ///
        /// Pre Conditions - Dictionaries are initialized.
        /// Post Conditions - All IPartInput that are listening for the given
        /// inputType have their DoPartAction function called.
        /// </summary>
        /// <param name="playerIndex">Which player inputted.</param>
        /// <param name="inputType">What type of input.</param>
        /// <param name="inputValue">Data for the input.</param>
        /// <returns>If the input is used for the specified player.</returns>
        public bool ExecuteInput(byte playerIndex, eInputType inputType,
            byte slotIndex, CustomInputData inputValue)
        {
            // Determine whose input it was
            Dictionary<eInputType, List<CustomInputBinding>> temp_curPlayerInputs =
                playerIndex == 0 ?
                m_inputBindingMapPlayerOne : m_inputBindingMapPlayerTwo;

            // Check if this is a used input or not
            if (!CheckIfInputIsUsedBySpecificSlot(playerIndex, inputType,
                slotIndex, out IReadOnlyList<CustomInputBinding> temp_bindingsList))
            {
                // Unused action.
                return false;
            }

            CustomDebug.Log($"Debugging bindings list for " +
                $"{inputType}", IS_DEBUGGING);
            DebugCustomInputBindingsList(ref temp_bindingsList);

            // After the rework, we are only controlling 1 part at a time.
            // However, this is still a for loop.
            // Even though, it is literally impossible for us to be doing
            // input on more than 1 part now. Its just easier to keep it as is
            // and instead add additional restrictions on top.
            //
            // Do all the actions for the current bindings
            foreach (CustomInputBinding temp_binding in temp_bindingsList)
            {
                // Get the IPartInput from it's unique string
                if (!m_inputPartsMap.TryGetValue(temp_binding.partSlotID,
                    out IPartInput temp_curPartInput))
                {
                    // Intended to be an Debug.LogError, we should not reach here.
                    Debug.LogError($"Could not find the part with unique ID " +
                        $"{temp_binding.partUniqueID} and slot ID " +
                        $"{temp_binding.partSlotID} in {name}'s " +
                        $"RobotInputController");
                    continue;
                }
                // Have the parts take its actions
                temp_curPartInput.DoPartAction(temp_binding.actionIndex, inputValue);
            }

            return true;
        }

        /// <summary>
        /// Returns a list of bindings that the specified player has for the part in the specified slot
        /// </summary>
        /// <param name="slotID"></param>
        /// <param name="playerIndex"></param>
        /// <returns></returns>
        public List<CustomInputBinding> GetBindingByID(byte slotID, byte playerIndex)
        {
            List<CustomInputBinding> bindingsForSlot = new List<CustomInputBinding>();

            Dictionary<eInputType, List<CustomInputBinding>> temp_dictionary = (playerIndex == 0) ? m_inputBindingMapPlayerOne : m_inputBindingMapPlayerTwo;
            
            foreach(KeyValuePair<eInputType, List<CustomInputBinding>> temp_bindingList in temp_dictionary)
            {
                foreach(CustomInputBinding temp_binding in temp_bindingList.Value)
                {
                    if (slotID == temp_binding.partSlotID)
                    {
                        bindingsForSlot.Add(temp_binding);
                    }
                }
            }

            return bindingsForSlot;
        }

        /// <summary>
        /// Returns the movement binding for the specified player
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <returns></returns>
        public CustomInputBinding GetMovementBinding(byte playerIndex)
        {
            List<CustomInputBinding> bindings = GetBindingByID(254, playerIndex);

            if (bindings.Count > 0)
            {
                return bindings[0];
            }
            else
            {
                Debug.LogError($"Unable to find movement binding for {name}");
                return null;
            }
        }


        /// <summary>
        /// Adds a CustomInputBinding to the given
        /// dictionary based on its inputType.
        ///
        /// Pre Conditions - The playerBindingMap has been new-ed up.
        /// Post Conditions - The CustomInputBinding has been added to the
        /// dictionary's list located at the key of inputType.
        /// </summary>
        /// <param name="playerBindingMap">Dictionary to add the
        /// CustomInputBinding to.</param>
        /// <param name="customInputBinding">CustomInputBinding to
        /// add to the dictionary.</param>
        private void AddCustomBindingToDictionary(ref Dictionary<eInputType,
            List<CustomInputBinding>> playerBindingMap,
            CustomInputBinding customInputBinding)
        {
            // If the list already exists in the map.
            if (playerBindingMap.TryGetValue(customInputBinding.inputType,
                out List<CustomInputBinding> temp_existingList))
            {
                temp_existingList.Add(customInputBinding);
            }
            // If there is no list in the map for the current input type,
            // create a new one with the given binding in it.
            else
            {
                playerBindingMap.Add(customInputBinding.inputType,
                    new List<CustomInputBinding>() { customInputBinding });
            }
        }


        #region Debugging
        /// <summary>
        /// Prints out the inputPartsMap to the console.
        /// </summary>
        private void DebugInputPartsMap()
        {
            if (!IS_DEBUGGING) { return; }

            CustomDebug.Log($"Debug Input Part Map", IS_DEBUGGING);
            foreach (KeyValuePair<byte, IPartInput> temp_pair in m_inputPartsMap)
            {
                CustomDebug.Log($"InputPartSlotID: {temp_pair.Key} " +
                    $"has part {temp_pair.Value.name}.", IS_DEBUGGING);
            }
        }
        /// <summary>
        /// Prints out both player's input maps to the console.
        /// </summary>
        private void DebugPlayerInputBindingsMaps()
        {
            if (!IS_DEBUGGING) { return; }

            CustomDebug.Log("Player One's Dictionaries", IS_DEBUGGING);
            DebugDictionary(ref m_inputBindingMapPlayerOne);
            CustomDebug.Log("Player Two's Dictionaries", IS_DEBUGGING);
            DebugDictionary(ref m_inputBindingMapPlayerTwo);
        }
        /// <summary>
        /// Prints out the specified player input map to the console.
        /// </summary>
        private void DebugDictionary(ref Dictionary<eInputType,
            List<CustomInputBinding>> playerInputMap)
        {
            if (!IS_DEBUGGING) { return; }

            CustomDebug.Log($"DebugDictionary", IS_DEBUGGING);

            if (playerInputMap.Count <= 0)
            {
                CustomDebug.Log($"The Dictionary is empty", IS_DEBUGGING);
                return;
            }

            foreach (KeyValuePair<eInputType, List<CustomInputBinding>>
                temp_pair in playerInputMap)
            {
                CustomDebug.Log($"Input Type: {temp_pair.Key}.", IS_DEBUGGING);
                foreach (CustomInputBinding temp_binding in temp_pair.Value)
                {
                    CustomDebug.Log(temp_binding.ToString(), IS_DEBUGGING);
                }
            }
        }
        /// <summary>
        /// Prints out the specified customInputBindingList.
        /// </summary>
        private void DebugCustomInputBindingsList(ref IReadOnlyList<CustomInputBinding>
            customInputBingingList)
        {
            if (!IS_DEBUGGING) { return; }

            foreach (CustomInputBinding temp_binding in customInputBingingList)
            {
                CustomDebug.Log(temp_binding.ToString(), IS_DEBUGGING);
            }
        }
        #endregion Debugging
    }
}
