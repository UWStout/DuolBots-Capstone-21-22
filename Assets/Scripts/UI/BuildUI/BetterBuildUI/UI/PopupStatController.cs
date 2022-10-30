using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using NaughtyAttributes;
// Original Authors - Eslis Vang

namespace DuolBots
{
    public class PopupStatController : MonoBehaviour
    {
        private const float MAX_HEALTH = 16.0f;
        private const float MAX_WEIGHT = 25.0f;
        private const float MAX_SLOT_COUNT = 5.0f;

        [SerializeField, Min(0.0f), MaxValue(MAX_HEALTH)]
        private float m_healthOffset = 0.0f;
        [SerializeField, Min(0.0f), MaxValue(MAX_WEIGHT)]
        private float m_weightOffset = 0.0f;
        [SerializeField, Min(0.0f), MaxValue(MAX_SLOT_COUNT)]
        private float m_difficultyOffset = 0.0f;

        [ReadOnly, Min(0.0f), MaxValue(1.0f)]
        private float m_healthFillAmount = 0.0f;
        [ReadOnly, Min(0.0f), MaxValue(1.0f)]
        private float m_weightFillAmount = 0.0f;
        [ReadOnly, Min(0.0f), MaxValue(1.0f)]
        private float m_difficultyFillAmount = 0.0f;

        [SerializeField] private Image[] m_statBars = new Image[3];

        private ChassisMoveSelectOnReadyUp m_readyUp = null;
        private IReadOnlyList<SingleChassisMoveOption> m_optionList = null;
        private PartDatabase m_partDatabase = null;

        private void Start()
        {
            m_partDatabase = PartDatabase.instance;
            CustomDebug.AssertDynamicSingletonMonoBehaviourPersistantIsNotNull(
                m_partDatabase, this);
            m_readyUp = FindObjectOfType<ChassisMoveSelectOnReadyUp>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_readyUp, this);
            #endregion
            m_optionList = m_readyUp.optionList;
            this.gameObject.SetActive(false);
        }

        public void CalculateChassisStats(int partSOIndex)
        {
            CustomDebug.LogWarning($"Calculating stats of chassis at index: " +
                $"<color=green>{partSOIndex}</color>.");
            string temp_chassisID = m_optionList[partSOIndex].chassisID;

            PartScriptableObject temp_partSO = m_partDatabase.
                GetPartScriptableObject(temp_chassisID);

            int temp_slotAmount = temp_partSO.battleNetworkPrefab.
                GetComponent<SlotPlacementManager>().GetSlotAmount();

            m_statBars[0].fillAmount = CalculateHealth(temp_partSO.health);
            m_statBars[1].fillAmount = CalculateWeight(temp_partSO.weight);
            m_statBars[2].fillAmount = CalculateDifficulty(temp_slotAmount);
        }

        private float CalculateHealth(float health)
        {
            m_healthFillAmount = (health - m_healthOffset) / MAX_HEALTH;

            return m_healthFillAmount;
        }

        private float CalculateWeight(float weight)
        {
            m_weightFillAmount = 1f - ((weight - m_weightOffset) / MAX_WEIGHT);

            return m_weightFillAmount;
        }

        private float CalculateDifficulty(int slotAmount)
        {
            m_difficultyFillAmount = (slotAmount - m_difficultyOffset)
                / MAX_SLOT_COUNT;

            return m_difficultyFillAmount;
        }
    }
}
