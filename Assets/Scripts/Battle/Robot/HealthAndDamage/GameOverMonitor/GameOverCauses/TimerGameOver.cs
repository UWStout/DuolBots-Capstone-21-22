using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    [RequireComponent(typeof(Shared_TimerManager))]
    public class TimerGameOver : MonoBehaviour
    {
        private const bool IS_DEBUGGING = true;

        [SerializeField] [Required]
        private GameOverMonitor m_gameOverMonitor = null;
        private Shared_TimerManager m_timerManager = null;


        // Domestic Initialization
        private void Awake()
        {
            m_timerManager = GetComponent<Shared_TimerManager>();
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_timerManager,
                nameof(m_timerManager), this);
            CustomDebug.AssertComponentIsNotNull(m_timerManager, this);
            #endregion Asserts
        }
        private void OnEnable()
        {
            m_timerManager.onTimerReachedZero += OnTimerReachedZero;
        }
        private void OnDisable()
        {
            // Account for timer manager potentially being destroyed
            // this on scene change.
            if (m_timerManager != null)
            {
                m_timerManager.onTimerReachedZero -= OnTimerReachedZero;
            }
        }


        private void OnTimerReachedZero()
        {
            IReadOnlyList<IRobotHealth> temp_highestHPBotList =
                RobotHelpersSingleton.instance.FindRobotsWithMostHealth();

            byte[] temp_highestHPTeams = new byte[temp_highestHPBotList.Count];
            for (int i = 0; i < temp_highestHPTeams.Length; ++i)
            {
                IRobotHealth temp_curBotHP = temp_highestHPBotList[i];
                ITeamIndex temp_curBotTeam
                    = temp_curBotHP.GetComponent<ITeamIndex>();
                #region Asserts
                CustomDebug.AssertIComponentOnOtherIsNotNull(temp_curBotTeam,
                    temp_curBotHP.gameObject, this);
                #endregion Asserts
                temp_highestHPTeams[i] = temp_curBotTeam.teamIndex;
            }

            // Single winner
            if (temp_highestHPBotList.Count == 1)
            {
                #region Logs
                CustomDebug.LogForComponent($"Ending game with single winner. " +
                    $"Winning team index is {temp_highestHPTeams[0]}.", this,
                    IS_DEBUGGING);
                #endregion Logs
                m_gameOverMonitor.EndGame(eGameOverCause.Time,
                    temp_highestHPTeams[0]);
            }
            // Tie
            else if (temp_highestHPBotList.Count > 1)
            {
                #region Logs
                string temp_winningTeamIndicies = "";
                foreach (byte temp_singleTeamIndex in temp_highestHPTeams)
                {
                    temp_winningTeamIndicies += temp_singleTeamIndex + ", ";
                }
                temp_winningTeamIndicies = temp_winningTeamIndicies.Substring(0,
                    temp_winningTeamIndicies.Length - 2);
                CustomDebug.LogForComponent($"Ending game with as tie. " +
                    $"Tied team indices are {temp_winningTeamIndicies}.", this,
                    IS_DEBUGGING);
                #endregion Logs

                // Convert the collection of IRobotHealths to a collection
                // of GameObjects.
                GameObject[] temp_highestHPBotObjArr =
                    new GameObject[temp_highestHPBotList.Count];
                for (int i = 0; i < temp_highestHPBotList.Count; ++i)
                {
                    temp_highestHPBotObjArr[i] = temp_highestHPBotList[i].gameObject;
                }

                m_gameOverMonitor.EndGameAsTie(eGameOverCause.Time,
                    temp_highestHPTeams);
            }
            // No winners?
            else
            {
                Debug.LogError($"There were no bots found when searching " +
                    $"for the bot with the least health.");
                m_gameOverMonitor.EndGameWithNoWinner(eGameOverCause.Time);
            }
        }
    }
}
