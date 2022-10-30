using System;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Holds which PartScriptableObject is currently being selected.
    /// </summary>
    public class PartSelectPlayerSelection : MonoBehaviour
    {
        [SerializeField] private List<PartScriptableObject> m_excludedParts
            = new List<PartScriptableObject>();

        private BetterBuildSceneStateManager m_stateMan = null;

        private BetterBuildSceneStateChangeHandler m_partHandler = null;
        // Slotted parts
        private List<PartScriptableObject> m_partSOList = null;
        // Index of the PartScriptableObject that is currently selected.
        private int m_selPartIndex = 0;

        public IReadOnlyList<PartScriptableObject> partSOList => m_partSOList;

        public event Action<int> onSelectedIndexChanged;


        // Foreign Initialization
        private void Start()
        {
            PartDatabase temp_partDatabase = PartDatabase.instance;
            #region Asserts
            CustomDebug.AssertDynamicSingletonMonoBehaviourPersistantIsNotNull(
                temp_partDatabase, this);
            #endregion Asserts
            m_partSOList = temp_partDatabase.GetSlottedPartScriptableObjects(
                m_excludedParts);

            m_stateMan = BetterBuildSceneStateManager.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_stateMan, this);
            #endregion Asserts
            m_partHandler = new BetterBuildSceneStateChangeHandler(m_stateMan,
                BeginPartHandler, null, eBetterBuildSceneState.Part);
        }
        private void OnDestroy()
        {
            m_partHandler.ToggleActive(false);
        }


        /// <summary>
        /// Gets which <see cref="PartScriptableObject"/> is currently selected.
        /// </summary>
        public PartScriptableObject GetCurrentlySelectedPartSO()
        {
            #region Asserts
            CustomDebug.AssertIndexIsInRange(m_selPartIndex, m_partSOList, this);
            #endregion Asserts
            return m_partSOList[m_selPartIndex];
        }
        /// <summary>
        /// Gets the <see cref="PartScriptableObject"/> in the list that is
        /// the amount specified away from the currently selected.
        /// </summary>
        /// <param name="amountAway">Distance away from the currently selected
        /// part in the list.</param>
        public PartScriptableObject GetPartSOAwayFromCurrentlySelected(
            int amountAway)
        {
            int temp_newIndex = m_partSOList.WrapIndex(m_selPartIndex + amountAway);
            #region Asserts
            CustomDebug.AssertIndexIsInRange(temp_newIndex, m_partSOList, this);
            #endregion Asserts
            return m_partSOList[temp_newIndex];
        }


        private void BeginPartHandler()
        {
            onSelectedIndexChanged?.Invoke(m_selPartIndex);
        }


        #region AnimationEvents
        /// <summary>
        /// Called after the move left animation.
        /// Decrements <see cref="m_selPartIndex"/> and updates images.
        /// </summary>
        private void OnMoveLeftAnimEnd()
        {
            m_selPartIndex = m_partSOList.WrapIndex(m_selPartIndex - 1);
            onSelectedIndexChanged?.Invoke(m_selPartIndex);
        }
        /// <summary>
        /// Called after the move right animation.
        /// Increments <see cref="m_selPartIndex"/> and updates images.
        /// </summary>
        private void OnMoveRightAnimEnd()
        {
            m_selPartIndex = m_partSOList.WrapIndex(m_selPartIndex + 1);
            onSelectedIndexChanged?.Invoke(m_selPartIndex);
        }
        #endregion AnimationEvents
    }
}
