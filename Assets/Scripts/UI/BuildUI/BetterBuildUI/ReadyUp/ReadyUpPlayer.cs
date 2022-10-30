using UnityEngine;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    [RequireComponent(typeof(PlayerIndex))]
    public class ReadyUpPlayer : MonoBehaviour
    {
        private PlayerIndex m_playerIndex = null;
        private ReadyUpManager m_readyUpMan = null;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            m_playerIndex = GetComponent<PlayerIndex>();
            CustomDebug.AssertComponentIsNotNull(m_playerIndex, this);
        }
        // Called 1st
        // Foreign Initialization
        private void Start()
        {
            m_readyUpMan = ReadyUpManager.instance;
            CustomDebug.AssertComponentIsNotNull(m_readyUpMan, this);
        }


        public void ReadyUp(bool state)
        {
            m_readyUpMan.ToggleReadyUpPlayer(m_playerIndex.playerIndex, state);
        }
    }
}
