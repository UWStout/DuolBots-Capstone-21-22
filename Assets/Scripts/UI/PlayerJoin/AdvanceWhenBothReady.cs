using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Function is called by a button. When the PlayerJoinScene has enough
    /// players ready, this will load a new scene.
    /// </summary>
    public class AdvanceWhenBothReady : MonoBehaviour
    {
        // Amount of players needed to be ready before loading the specified scene
        [SerializeField] private int m_readyPlayersNeeded = 2;
        // Scene to load when players are ready
        [SerializeField] [Scene] private string m_sceneNameToLoadOnReady = "PartSelection_SCENE";
        // Which players are ready
        private List<bool> m_playerReadyFlags = new List<bool>();


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            m_playerReadyFlags = new List<bool>(m_readyPlayersNeeded);
            for (int i = 0; i < m_readyPlayersNeeded; ++i)
            {
                m_playerReadyFlags.Add(false);
            }
        }


        /// <summary>
        /// Sets the player with the given index to be ready.
        /// If all players are ready after this change, loads the specified scene.
        /// 
        /// Called from button.
        /// </summary>
        public void OnReadyUp(int playerIndex)
        {
            if (playerIndex >= m_readyPlayersNeeded || playerIndex < 0)
            {
                Debug.LogError($"Specified player index is out of bounds ({playerIndex})");
                return;
            }
            SetReadyUpState(!m_playerReadyFlags[playerIndex], playerIndex);
        }

        public void SetReadyUpState(bool isReady, int playerIndex)
        {
            if (playerIndex >= m_readyPlayersNeeded || playerIndex < 0)
            {
                Debug.LogError($"Specified player index is out of bounds ({playerIndex})");
                return;
            }
            // Flip flag
            m_playerReadyFlags[playerIndex] = isReady;

            AdvanceSceneIfAllPlayersReady();
        }


        /// <summary>
        /// Checks if all players are ready and if they are,
        /// then we load the new scene.
        /// </summary>
        private void AdvanceSceneIfAllPlayersReady()
        {
            foreach (bool temp_singleReadyFlag in m_playerReadyFlags)
            {
                if (!temp_singleReadyFlag)
                {
                    return;
                }
            }
            SceneLoader.instance.LoadScene(m_sceneNameToLoadOnReady);
        }
    }
}
