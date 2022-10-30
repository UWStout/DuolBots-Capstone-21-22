using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using NaughtyAttributes;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    [RequireComponent(typeof(PlayerIndex))]
    public class Input_ReadyUpPartSelect : MonoBehaviour
    {
        private const bool IS_DEBUGGING = true;

        [SerializeField] [Required]
        private ReadyImage_PartSelect m_readyImg = null;

        private PlayerIndex m_playerIndex = null;
        private ReadyUpManager m_readyUpManager = null;

        private bool m_isReady = false;


        // Domestic Initialization
        private void Awake()
        {
            m_playerIndex = GetComponent<PlayerIndex>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_playerIndex, this);
            #endregion Asserts
        }
        // Foreign Initialization
        private void Start()
        {
            m_readyUpManager = ReadyUpManager.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_readyUpManager,
                this);
            #endregion Asserts
        }


        #region InputMessages
        private void OnReadyUp(InputValue value)
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(OnReadyUp), this, IS_DEBUGGING);
            #endregion Logs

            if (!value.isPressed) { return; }

            m_isReady = !m_isReady;

            m_readyUpManager.ToggleReadyUpPlayer(m_playerIndex.playerIndex,
                m_isReady);
            m_readyImg.ToggleReady(m_isReady);
        }
        #endregion InputMessages

        public void SetIsReady(bool cond)
        {
            m_isReady = cond;

            m_readyUpManager.ToggleReadyUpPlayer(m_playerIndex.playerIndex,
                m_isReady);
            m_readyImg.ToggleReady(m_isReady);
        }
    }
}
