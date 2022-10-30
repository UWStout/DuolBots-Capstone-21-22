using UnityEngine;
// Original Authors - Wyatt Senalik.

namespace DuolBots
{
    /// <summary>
    /// Holds which slot the part has been placed into.
    /// </summary>
    public class PartSlotIndex : MonoBehaviour
    {
        /// <summary>254. One below max value.</summary>
        public const byte MOVEMENT_PART_SLOT_ID = byte.MaxValue - 1;

        // Here's my isChicken flag, sadly.
        [SerializeField] private bool m_isMovementPart = false;
        private byte m_slotIndex = byte.MaxValue;

        public byte slotIndex
        {
            get => m_slotIndex;
            private set => m_slotIndex = value;
        }


        // Domestic Initialization
        private void Awake()
        {
            if (m_isMovementPart)
            {
                m_slotIndex = MOVEMENT_PART_SLOT_ID;
                return;
            }

            // Look in parent for the slot index
            SlotIndex temp_slotIndex = GetComponentInParent<SlotIndex>();

            CustomDebug.AssertComponentInParentIsNotNull(temp_slotIndex, this);

            slotIndex = temp_slotIndex.slotIndex;
        }
    }
}
