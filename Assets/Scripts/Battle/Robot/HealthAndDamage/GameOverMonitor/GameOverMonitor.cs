using System.Collections.Generic;
using UnityEngine;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Keeps track of the game over conditions and handles ending the game.
    /// </summary>
    public class GameOverMonitor : NetworkBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = true;

        public static GameOverMonitor instance => s_instance;
        private static GameOverMonitor s_instance = null;
        

        /// <summary>
        /// Event invoked when the game is over.
        /// Either when a bot is destroyed or the timer runs out.
        /// </summary>
        public IEventPrimer<GameOverData> onGameOver => m_onGameOver;
        public IEventPrimer onGameOverNoParam => m_onGameOverNoParam;
        private CatchupEvent<GameOverData> m_onGameOver
            = new CatchupEvent<GameOverData>();
        private CatchupEvent m_onGameOverNoParam = new CatchupEvent();

        public GameOverData gameOverData => m_gameOverData;
        private GameOverData m_gameOverData = new GameOverData();



        private void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this;
            }
            else
            {
                Debug.LogError($"Multiple {GetType().Name} found in the scene. " +
                    $"Destroying the newer one.");
                Destroy(gameObject);
            }
        }
        // Foreign Initialization
        public override void OnStartServer()
        {
            base.OnStartServer();

            CatchupEventResetter temp_eventResetter = CatchupEventResetter.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(temp_eventResetter,
                this);
            #endregion Asserts
            temp_eventResetter.AddCatchupEventForReset(m_onGameOver);
            temp_eventResetter.AddCatchupEventForReset(m_onGameOverNoParam);
        }


        [Server]
        public void EndGame(eGameOverCause cause, byte winningTeamIndex)
        {
            #region Logs
            CustomDebug.Log($"Game End. <color=green>Team {winningTeamIndex}" +
                $"has won</color>. Cause was {cause}", IS_DEBUGGING);
            #endregion Logs
            m_gameOverData = new GameOverData(cause, winningTeamIndex);
            SetGameOverDataClientRpc(m_gameOverData);
            InvokeEvents();
        }
        [Server]
        public void EndGameAsTie(eGameOverCause cause, byte[] tiedTeams)
        {
            #region Logs
            CustomDebug.Log($"Game End. <color=yellow>Tied</color>. " +
                $"Cause was {cause}", IS_DEBUGGING);
            #endregion Logs
            m_gameOverData = new GameOverData(cause, tiedTeams);
            SetGameOverDataClientRpc(m_gameOverData);
            InvokeEvents();
        }
        [Server]
        /// <summary>
        /// Should hopefully never happen, but handle it nonetheless.
        /// </summary>
        public void EndGameWithNoWinner(eGameOverCause cause)
        {
            #region Logs
            CustomDebug.Log($"Game End. <color=red>No Winner</color>. " +
                $"Cause was {cause}", IS_DEBUGGING);
            #endregion Logs
            m_gameOverData = new GameOverData(cause);
            SetGameOverDataClientRpc(m_gameOverData);
            InvokeEvents();
        }


        [ClientRpc]
        private void SetGameOverDataClientRpc(GameOverData newData)
        {
            m_gameOverData = newData;
        }
        [Server]
        private void InvokeEvents()
        {
            m_onGameOver.Invoke(m_gameOverData);
            m_onGameOverNoParam.Invoke();
        }
    }
}
