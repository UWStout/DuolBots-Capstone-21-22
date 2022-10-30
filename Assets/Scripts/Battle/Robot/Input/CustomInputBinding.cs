using System;
using UnityEngine;
// Original Authors - Wyatt Senalik and Zachary Gross

namespace DuolBots
{
    /// <summary>
    /// Holds data for an binding an input to the control of a
    /// specific part.
    /// </summary>
    [Serializable]
    public class CustomInputBinding : IEquatable<CustomInputBinding>
    {    
        /// <summary>
        /// Which player is controlling the part.
        /// </summary>
        public byte playerIndex => m_playerIndex;
        [SerializeField] private readonly byte m_playerIndex = byte.MaxValue;
        /// <summary>
        /// Which action on the part the input is binded to.
        /// </summary>
        public byte actionIndex => m_actionIndex;
        [SerializeField] private readonly byte m_actionIndex = byte.MaxValue;
        /// <summary>
        /// The input that is binded to cause the action.
        /// </summary>
        public eInputType inputType => m_inputType;
        [SerializeField] private readonly eInputType m_inputType =
            eInputType.buttonEast;
        /// <summary>
        /// Slot ID that serves as the unique instance ID for the part.
        /// </summary>
        public byte partSlotID => m_partSlotID;
        [SerializeField] private readonly byte m_partSlotID = byte.MaxValue;
        /// <summary>
        /// Unique part ID that allows us to load the part from the database.
        /// </summary>
        public string partUniqueID => m_partUniqueID;
        [SerializeField] private readonly string m_partUniqueID = "INVALID ID";


        public CustomInputBinding() {}
        public CustomInputBinding(byte isPlayerOne, byte actionIndex,
            eInputType inputType, byte slotID, string uniqueID)
        {
            m_playerIndex = isPlayerOne;
            m_actionIndex = actionIndex;
            m_inputType = inputType;
            m_partSlotID = slotID;
            m_partUniqueID = uniqueID;
        }


        public override string ToString()
        {
            return $"CustomInputBinding (isPlayerOne={playerIndex}, " +
                $"actionIndex={actionIndex}, inputType={inputType}, " +
                $"partSlotID={partSlotID}, partUniqueID={partUniqueID})";
        }

        public bool Equals(CustomInputBinding other)
        {
            return playerIndex == other.playerIndex &&
                actionIndex == other.actionIndex &&
                inputType == other.inputType &&
                partSlotID == other.partSlotID &&
                partUniqueID == other.partUniqueID;
        }
    }
}
