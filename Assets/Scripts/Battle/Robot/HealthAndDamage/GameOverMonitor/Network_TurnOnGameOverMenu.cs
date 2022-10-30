using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Mirror;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    public class Network_TurnOnGameOverMenu : NetworkBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField] [Required]
        private Shared_TurnOnGameOverMenu m_sharedController = null;

        private BattleStateManager m_battleStateMan = null;

        private BattleStateChangeHandler m_battleHandler = null;
        private BattleStateChangeHandler m_gameOverHandler = null;
        private byte m_connectionsTeamIndex = byte.MaxValue;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_sharedController,
                nameof(m_sharedController), this);
            #endregion Asserts
        }
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            m_battleStateMan = BattleStateManager.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(
                m_battleStateMan, this);
            #endregion Asserts

            // Battle
            m_battleHandler = new BattleStateChangeHandler(m_battleStateMan,
                HandleBattleBegin, null, eBattleState.Battle);
            // GameOver
            m_gameOverHandler = new BattleStateChangeHandler(m_battleStateMan,
                HandleGameOverBegin, HandleGameOverEnd, eBattleState.GameOver);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();

            // OnStopLocalPlayer
            if (isLocalPlayer)
            {
                // Clean up state change handlers
                m_battleHandler.ToggleActive(false);
                m_gameOverHandler.ToggleActive(false);
            }
        }


        #region Battle
        private void HandleBattleBegin()
        {
            m_connectionsTeamIndex = RobotHelpersSingleton.
                   instance.DetermineMyTeamIndexNetwork();
            #region Logs
            CustomDebug.Log($"{GetType().Name} determined team index to be " +
                $"{m_connectionsTeamIndex}", IS_DEBUGGING);
            #endregion Logs
        }
        #endregion Battle
        #region Game Over
        private void HandleGameOverBegin()
        {
            #region Logs
            CustomDebug.Log($"{GetType().Name}'s {nameof(HandleGameOverBegin)}",
                IS_DEBUGGING);
            #endregion Logs
            GameOverMonitor temp_goMon = GameOverMonitor.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(temp_goMon, this);
            #endregion Asserts
            GameOverData temp_goData = temp_goMon.gameOverData;
            OnGameOver(temp_goData);
        }
        private void HandleGameOverEnd()
        {
            m_sharedController.TurnOffMenu();
        }
        #endregion Game Over


        private void OnGameOver(GameOverData gameOverData)
        {
            string temp_gameOverMsg;
            IReadOnlyList<byte> temp_winnerIndices =
                gameOverData.winningTeamIndices;

            #region Logs
            CustomDebug.Log($"{GetType().Name}'s {nameof(OnGameOver)}",
                IS_DEBUGGING);
            CustomDebug.Log($"There are {temp_winnerIndices.Count} winners",
                IS_DEBUGGING);
            #endregion Logs

            if (temp_winnerIndices.Contains(m_connectionsTeamIndex))
            {
                if (temp_winnerIndices.Count == 1)
                {
                    temp_gameOverMsg = "Congrats, You Win!";
                }
                else
                {
                    temp_gameOverMsg = "Tie";
                }
            }
            else
            {
                temp_gameOverMsg = "You Lose";
            }

            m_sharedController.TurnOnMenu(temp_gameOverMsg);
        }
    }
}
