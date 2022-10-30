using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik and Eslis Vang

namespace DuolBots
{
    /// <summary>
    /// Helper to generates a list of <see cref="CustomInputBinding"/>s.
    /// </summary>
    public class GenerateInputBindings : MonoBehaviour
    {
        private const string IGNORE_ACTION_NAME = "DUMMY";
        [SerializeField] private byte m_playerAmount = 2;


        /// <summary>
        /// Generates a list of <see cref="CustomInputBinding"/>s for the movement
        /// part and the slotted parts.
        /// </summary>
        /// <param name="botData">BotData to generate inputs for.</param>
        /// <returns><see cref="CustomInputBinding"/> for the movement part and
        /// each slotted part.</returns>
        public List<CustomInputBinding> GenerateInputs(BuiltBotData botData)
        {
            return GenerateInputs(botData.movementPartID,
                botData.slottedPartIDList);
        }
        /// <summary>
        /// Generates a list of <see cref="CustomInputBinding"/>s for the movement
        /// part and the slotted parts.
        /// </summary>
        /// <param name="movementPartID">Movement part that was selected.</param>
        /// <param name="slottedParts">Slotted parts that were selected.</param>
        /// <returns><see cref="CustomInputBinding"/> for the movement part and
        /// each slotted part.</returns>
        public List<CustomInputBinding> GenerateInputs(string movementPartID,
            IReadOnlyCollection<PartInSlot> slottedParts)
        {
            PartDatabase temp_database = PartDatabase.instance;
            #region Asserts
            CustomDebug.AssertDynamicSingletonMonoBehaviourPersistantIsNotNull(
                temp_database, this);
            #endregion Asserts

            List<CustomInputBinding> temp_bindings = new List<CustomInputBinding>();
            // Generate bindings for movement parts.
            temp_bindings.Add(new CustomInputBinding(0, 0, eInputType.leftStick_Y,
                PartSlotIndex.MOVEMENT_PART_SLOT_ID, movementPartID));
            temp_bindings.Add(new CustomInputBinding(1, 1, eInputType.leftStick_Y,
                PartSlotIndex.MOVEMENT_PART_SLOT_ID, movementPartID));

            // Generate bindings for each slotted part.
            byte temp_playerIndex = 0;
            foreach (PartInSlot temp_part in slottedParts)
            {
                PartScriptableObject temp_partSO =
                    temp_database.GetPartScriptableObject(temp_part.partID);

                int temp_amountActions = temp_partSO.actionList.Count;
                for (byte i = 0; i < temp_amountActions; ++i)
                {
                    actionInfo temp_info = temp_partSO.actionList[i];
                    // If it is an ignored action, do nothing and move on.
                    if (temp_info.action == IGNORE_ACTION_NAME) { continue; }

                    // Get the input type (button(s)) for the action
                    eInputType temp_inpType = GetInputTypeForAction(temp_info);
                    // Create and add the binding
                    temp_bindings.Add(new CustomInputBinding(temp_playerIndex, i,
                        temp_inpType, temp_part.slotIndex, temp_part.partID));

                    // Increment the player index so a different player will
                    // have the next input.
                    temp_playerIndex = IncrementPlayerIndex(temp_playerIndex);
                }
            }

            return temp_bindings;
        }


        /// <summary>
        /// Returns the input type that matches the action type.
        /// Analog is always buttonEast.
        /// Vector1 is always buttons_y.
        /// </summary>
        private eInputType GetInputTypeForAction(actionInfo info)
        {
            switch (info.actionType)
            {
                case eActionType.Analog:
                    return eInputType.buttonEast;
                case eActionType.Vector1:
                    return eInputType.buttons_Y;
                default:
                    CustomDebug.UnhandledEnum(info.actionType, this);
                    return eInputType.buttonWest;
            }
        }
        private byte IncrementPlayerIndex(byte prevIndex)
        {
            return (byte)((prevIndex + 1) % m_playerAmount);
        }
    }
}
