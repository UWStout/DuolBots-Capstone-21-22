using UnityEngine;

using Cinemachine;
// Orignal Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Used in conjunction with <see cref="SlotViewer"/>.
    /// Provides data about where the camera should be placed
    /// for all the slots.
    /// </summary>
    public class SlotViewPlacement : MonoBehaviour
    {
        // The dolly paths for each player to use. Doesn't need to be in
        // any special order, the 0th is given to player 0 and the 1st is
        // given to player 1. But they should be identical, we just need
        // one for each player.
        [SerializeField] private CinemachineSmoothPath[] m_dollyPath
            = new CinemachineSmoothPath[2];

        [SerializeField] private float[] m_targetValues = new float[3];
        [SerializeField] private int[] m_slotOrder = new int[3];

        public float[] targetValues => m_targetValues;
        public int[] slotOrder => m_slotOrder;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            CustomDebug.AssertListsAreSameSize(m_targetValues, m_slotOrder,
                nameof(m_targetValues), nameof(m_slotOrder), this);
        }


        public CinemachineSmoothPath GetDollyPathForPlayer(int playerIndex)
        {
            CustomDebug.AssertIndexIsInRange(playerIndex, m_dollyPath, this);

            return m_dollyPath[playerIndex];
        }
        /// <summary>
        /// Returns the index of the slot that should be viewed
        /// when viewing from the target value at the same index.
        /// </summary>
        /// <param name="viewIndex"></param>
        /// <returns></returns>
        public int GetSlotIndexFromViewIndex(int viewIndex)
        {
            CustomDebug.AssertIndexIsInRange(viewIndex, m_slotOrder, this);

            return m_slotOrder[viewIndex];
        }
    }
}
