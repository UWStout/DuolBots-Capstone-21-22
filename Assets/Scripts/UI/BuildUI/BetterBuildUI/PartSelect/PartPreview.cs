using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik and Eslis Vang

namespace DuolBots
{
    public class PartPreview : MonoBehaviour
    {
        [SerializeField] [Required]
        private GameObject m_playerZeroPreviewObj = null;
        [SerializeField] [Required]
        private GameObject m_playerOnePreviewObj = null;
        [SerializeField] [Layer] private int m_pZeroLayer = 13;
        [SerializeField] [Layer] private int m_pOneLayer = 14;

        private bool m_isPlayerZeroPreviewing = false;
        private bool m_isPlayerOnePreviewing = false;


        private void Awake()
        {
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_playerZeroPreviewObj,
                nameof(m_playerZeroPreviewObj), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_playerOnePreviewObj,
                nameof(m_playerOnePreviewObj), this);
            #endregion Asserts

            m_playerZeroPreviewObj.SetActive(false);
            m_playerZeroPreviewObj.SetLayerRecursively(m_pZeroLayer);
            m_playerOnePreviewObj.SetActive(false);
            m_playerOnePreviewObj.SetLayerRecursively(m_pOneLayer);
        }


        public void ToggleActive(int playerIndex, bool cond)
        {
            if (playerIndex == 0)
            {
                m_playerZeroPreviewObj.SetActive(cond);
                m_isPlayerZeroPreviewing = cond;
            }
            else if (playerIndex == 1)
            {
                m_playerOnePreviewObj.SetActive(cond);
                m_isPlayerOnePreviewing = cond;
            }
            else
            {
                Debug.LogError($"Unhandled player index {playerIndex} for " +
                    $"{name}'s {GetType().Name}");
            }

            DestroyIfNotPreviewed();
        }


        private void DestroyIfNotPreviewed()
        {
            if (m_isPlayerZeroPreviewing) { return; }
            if (m_isPlayerOnePreviewing) { return; }

            Destroy(gameObject);
        }
    }
}
