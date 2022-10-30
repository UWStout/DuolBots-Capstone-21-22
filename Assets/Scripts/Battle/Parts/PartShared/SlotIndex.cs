using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Meant to be attached to each slot with the slot index serialized.
    ///
    /// Created this script because when a part is spawned over the network,
    /// its slot index on <see cref="PartSlotIndex"/> is not synced,
    /// which some things need to know. So now instead of initializing
    /// <see cref="PartSlotIndex"/>  from <see cref="BotUnderConstruction"/>,
    /// we will now have <see cref="PartSlotIndex"/> search for this script
    /// in its parents and set itself.
    /// </summary>
    public class SlotIndex : MonoBehaviour
    {
        [SerializeField] private byte m_slotIndex = byte.MaxValue;

        public byte slotIndex => m_slotIndex;
    }
}
