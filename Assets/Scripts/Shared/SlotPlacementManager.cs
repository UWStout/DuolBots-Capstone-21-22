using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Manages the slots on a Chassis and helps add parts into the slots.
    /// Meant to be attached to a Chassis.
    /// </summary>
    public class SlotPlacementManager : MonoBehaviour
    {
        // Serialized transforms for the position/rotation information for the slots
        public Transform[] slotTransforms => m_slotTransforms;
        [SerializeField] private Transform[] m_slotTransforms = new Transform[0];

        private Dictionary<int, GameObject> m_slottedPartDict = new Dictionary<int, GameObject>();


        /// <summary>
        /// The amount of slots on this chassis.
        /// </summary>
        public int GetSlotAmount()
        {
            return m_slotTransforms.Length;
        }
        /// <summary>
        /// Adds the given part prefab to the slot with the given index.
        ///
        /// Pre Conditions - Slot index is in bounds. Another part does not already exist
        /// in the slot with specified index. The given part object should be a prefab, not an instance.
        /// Post Conditions - Places the given part at the slot with given index with proper rotation and scale.
        /// </summary>
        /// <param name="partPrefab">Part prefab to move to the specified slot.</param>
        /// <param name="slotIndex">Index of the slot to move the part to.</param>
        /// <returns>Part Instantiated.</returns>
        public GameObject AddPartToSlot(GameObject partPrefab, byte slotIndex)
        {
            GameObject temp_partObj = AddPartToSlotHelper(partPrefab, slotIndex);
            // Reset the part's transform to be located at the slot, facing the correct direction,
            // and scaled appropriately
            temp_partObj.transform.ResetLocal();

            return temp_partObj;
        }
        /// <summary>
        /// Destroys the part at the given index.
        ///
        /// Pre Conditions - Slot index must be valid. Must be only one part
        /// attached to this slot.
        /// Post Conditions - Part in the slot is removed if there is one.
        /// </summary>
        /// <param name="slotIndex"></param>
        /// <returns>True if the part was deleted. False if there was no part to delete.</returns>
        public bool RemovePartFromSlot(int slotIndex)
        {
            // Index out of bounds check
            if (slotIndex < 0 || slotIndex > m_slotTransforms.Length)
            {
                Debug.LogError($"Invalid slot index of {slotIndex} specified for chassis {name}");
                return false;
            }

            Transform temp_desiredSlotTrans = m_slotTransforms[slotIndex];
            // If there is no part assigned to this slot check
            if (!m_slottedPartDict.TryGetValue(slotIndex, out GameObject temp_existingPart))
            {
                Debug.LogWarning($"Chassis {name} has no part in slot index {slotIndex}");
                return false;
            }

            Assert.AreEqual(temp_desiredSlotTrans.childCount, 1, $"There should not be more than" +
                $" one child in the slot transform");

            // Destroy the part
            Destroy(temp_existingPart);
            return true;
        }
        public Transform GetSlotTransform(int slotIndex)
        {
            // Index out of bounds check
            if (slotIndex < 0 || slotIndex >= m_slotTransforms.Length)
            {
                Debug.LogError($"Invalid slot index of {slotIndex} specified for chassis {name}");
                return null;
            }

            return m_slotTransforms[slotIndex];
        }
        public bool GetSlotTransform(int slotIndex, out Transform slotTrans)
        {
            slotTrans = GetSlotTransform(slotIndex);
            if (slotTrans.childCount > 0)
            {
                return true;
            }
            return false;
        }
        public int GetAmountSlotsThatHaveChildren()
        {
            int temp_amountSlotsWithChildren = 0;
            foreach (Transform temp_slotTrans in m_slotTransforms)
            {
                if (temp_slotTrans.childCount > 0)
                {
                    ++temp_amountSlotsWithChildren;
                }
            }
            return temp_amountSlotsWithChildren;
        }
        /// <summary>
        /// Returns the dictionary of slotted parts on the bot.
        /// 
        /// Pre Conditions - The slotted part dictionary is initialized.
        /// Post Conditions - Returns the slotted parts in a dictionary. Changes no internal values.
        /// </summary>
        /// <returns>Dictionary with slot index as the key and the part's
        /// instantiated GameObject as the value.</returns>
        public IReadOnlyDictionary<int, GameObject> GetSlottedParts()
        {
            return m_slottedPartDict;
        }
        /// <summary>
        /// Returns a list of all the indicies of slots with parts in it.
        ///
        /// Pre Conditions - The slotted part dictionary is initialized.
        /// Post Conditions - Changes no internal values.
        /// </summary>
        public IReadOnlyList<int> GetUsedSlotsIndices()
        {
            // Can't just use the dictionary because
            // the client needs to know this too.
            //
            // Create a list  to hold the indices
            List<int> temp_usedSlotIndicies = new List<int>();
            // Determine which slots have parts using the transforms
            for (int i = 0; i < m_slotTransforms.Length; ++i)
            {
                Transform temp_slotTrans = m_slotTransforms[i];
                if (temp_slotTrans.childCount > 0)
                {
                    temp_usedSlotIndicies.Add(i);
                }
            }
            // Sort in ascending order
            temp_usedSlotIndicies.Sort();

            return temp_usedSlotIndicies;
        }


        /// <summary>
        /// Helper function for adding a part to a slot.
        /// Adds the given part prefab to the slot with the given index (Networking variant).
        ///
        /// Pre Conditions - Slot index is in bounds. Another part does not already exist
        /// in the slot with specified index. The given part object should be a prefab, not an instance.
        /// Post Conditions - Places the given part at the slot with given index with proper rotation and scale.
        /// </summary>
        /// <param name="partPrefab">Part prefab to move to the specified slot.</param>
        /// <param name="slotIndex">Index of the slot to move the part to.</param>
        /// <returns>Part Instantiated.</returns>
        private GameObject AddPartToSlotHelper(GameObject partPrefab, byte slotIndex)
        {
            Transform temp_desiredSlotTrans = GetSlotTransform(slotIndex);
            // If a part is already assigned to this slot check
            if (m_slottedPartDict.ContainsKey(slotIndex))
            {
                Debug.LogError($"Chassis {name} already has a part in slot index {slotIndex}");
                return null;
            }

            GameObject temp_partObj = Instantiate(partPrefab, temp_desiredSlotTrans.transform);
            m_slottedPartDict.Add(slotIndex, temp_partObj);

            // We no longer do this here, PartSlotIndex now initializes itself.
            //// Set the SlotIndex
            //PartSlotIndex temp_partSlotIndex = temp_partObj.GetComponent<PartSlotIndex>();
            //Assert.IsNotNull(temp_partSlotIndex, $"{name}'s {GetType().Name} " +
            //    $"expected {partPrefab.name} to have a {nameof(PartSlotIndex)} " +
            //    $"attached, but none was found");
            //temp_partSlotIndex.slotIndex = slotIndex;

            return temp_partObj;
        }
    }
}
