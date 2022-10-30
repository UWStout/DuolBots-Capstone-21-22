using System;
using UnityEngine;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    public class ReadyUpManager : SingletonMonoBehaviour<ReadyUpManager>
    {
        [SerializeField] private int m_numPlayers = 2;

        private BetterBuildSceneStateChangeHandler m_chassisHandler = null;
        private BetterBuildSceneStateChangeHandler m_movementHandler = null;
        private BetterBuildSceneStateChangeHandler m_partHandler = null;
        private bool[] m_readyStates = new bool[2];

        public event Action onAllPlayersReady;


        // Called 0th
        // Domestic Initialization
        protected override void Awake()
        {
            base.Awake();

            m_readyStates = new bool[m_numPlayers];
        }
        // Foreign Initialization
        private void Start()
        {
            m_chassisHandler = BetterBuildSceneStateChangeHandler.CreateNew(
                ResetReadyStates, null, eBetterBuildSceneState.Chassis);
            m_movementHandler = BetterBuildSceneStateChangeHandler.CreateNew(
                ResetReadyStates, null, eBetterBuildSceneState.Movement);
            m_partHandler = BetterBuildSceneStateChangeHandler.CreateNew(
                ResetReadyStates, null, eBetterBuildSceneState.Part);
        }
        private void OnDestroy()
        {
            m_chassisHandler.ToggleActive(false);
            m_movementHandler.ToggleActive(false);
            m_partHandler.ToggleActive(false);
        }


        public void ToggleReadyUpPlayer(int playerIndex, bool readyState)
        {
            if (playerIndex < 0)
            {
                Debug.LogError($"{playerIndex} is negative and outside " +
                    $"the collection for {nameof(ReadyUpManager)}");
                return;
            }
            if (playerIndex >= m_readyStates.Length)
            {
                Debug.LogError($"{playerIndex} is too large " +
                    $"(max={m_readyStates.Length}) and outside " +
                    $"the collection for {nameof(ReadyUpManager)}");
                return;
            }

            m_readyStates[playerIndex] = readyState;

            // If someone was set to true, its possible
            // that everyone is readied up now.
            if (readyState)
            {
                CheckIfAllReadiedUp();
            }
        }


        private void ResetReadyStates()
        {
            for (int i = 0; i < m_readyStates.Length; ++i)
            {
                m_readyStates[i] = false;
            }
        }
        private void CheckIfAllReadiedUp()
        {
            for (int i = 0; i < m_readyStates.Length; ++i)
            {
                // At least 1 player was not ready
                if (!m_readyStates[i]) { return; }
            }

            // If made it here, all players are ready.
            onAllPlayersReady?.Invoke();
        }
    }
}
