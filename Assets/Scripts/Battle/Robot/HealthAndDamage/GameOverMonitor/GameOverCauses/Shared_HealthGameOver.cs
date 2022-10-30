using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    public class Shared_HealthGameOver : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = true;

        [SerializeField] [Required]
        private GameOverMonitor m_gameOverMonitor = null;
        private RobotHelpersSingleton robotHelpersSingleton
        {
            get
            {
                if (m_robotHelpersSingleton == null)
                {
                    m_robotHelpersSingleton = RobotHelpersSingleton.instance;
                }
                Assert.IsNotNull(m_robotHelpersSingleton, $"No " +
                    $"{nameof(RobotHelpersSingleton)} found");
                return m_robotHelpersSingleton;
            }
            set
            {
                m_robotHelpersSingleton = value;
            }
        }
        private RobotHelpersSingleton m_robotHelpersSingleton = null;

        private List<IRobotHealth> m_robotHealths = new List<IRobotHealth>();

        private bool m_wasInitialized = false;

        /// <summary>
        /// Event invoked when a bot is determined to have reached
        /// critical health and listeners like the Local and Network
        /// versions of GameOverMonitor should handle actually killing
        /// the bot.
        ///
        /// Parameters is bot that should die.
        /// </summary>
        public event Action<GameObject> onBotShouldDie;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            Assert.IsNotNull(m_gameOverMonitor, $"{nameof(GameOverMonitor)} " +
                $"is required but none was specified");
        }
        private void OnDestroy()
        {
            // Don't unsub if we never subbed int he first place
            if (!m_wasInitialized) { return; }

            // Clean up subscription event from RobotHealth
            UnsubscribeFromRobotEvents();
        }


        /// <summary>
        /// Finds the robots and stores their healths.
        /// Also subscribes to robot health events.
        /// 
        /// PRECONDITION: Expecting robots to have the tag "Robot" and that their
        /// RobotHealth scripts are attached directly to them.
        /// POSTCONDITION: Subscribed to robot health events and
        /// have a list of RobotHealths.
        /// </summary>
        public void InitializeRobotHealths()
        {
           
            GameObject[] temp_robotObjects =
                robotHelpersSingleton.FindAllBotRoots();
            #region Logs
            CustomDebug.LogForComponent($"Found {temp_robotObjects.Length} " +
                $"robots in the scene", this, IS_DEBUGGING);
            #endregion Logs
            m_robotHealths = new List<IRobotHealth>(temp_robotObjects.Length);
            foreach (GameObject temp_robot in temp_robotObjects)
            {
                // Create the robot health and its death callback
                IRobotHealth temp_robotHealth =
                    temp_robot.GetComponent<IRobotHealth>();
                #region Asserts
                CustomDebug.AssertIComponentOnOtherIsNotNull(temp_robotHealth,
                    temp_robot, this);
                #endregion Asserts
                m_robotHealths.Add(temp_robotHealth);
                temp_robotHealth.onHealthReachedCritical += SingleRobotDied;
                #region Logs
                CustomDebug.LogForComponent($"Initializing health for " +
                    $"{temp_robotHealth.name} at position " +
                    $"{temp_robotHealth.transform.position}", this, IS_DEBUGGING);
                #endregion Logs
            }

            m_wasInitialized = true;
        }


        /// <summary>
        /// Cleans up subscriptions to events.
        ///
        /// PRECONDITION: List of RobotHealths has been initialized.
        /// POSTCONDITION: Unsubscribed from all robot events.
        /// </summary>
        private void UnsubscribeFromRobotEvents()
        {
            foreach (IRobotHealth temp_curRobotHealth in m_robotHealths)
            {
                temp_curRobotHealth.onHealthReachedCritical -= SingleRobotDied;
            }
        }
        /// <summary>
        /// TODO Play some death animation and have the game be over.
        /// </summary>
        private void SingleRobotDied(IRobotHealth robotToDie)
        {
            #region Logs
            CustomDebug.LogForComponent($"{nameof(SingleRobotDied)}. " +
                $"{robotToDie} is the robot who died.", this, IS_DEBUGGING);
            #endregion Logs

            // Clean up
            // Unsubscribe from the event
            robotToDie.onHealthReachedCritical -= SingleRobotDied;
            // Get the dead robot references out of here
            m_robotHealths.Remove(robotToDie);
            #region Logs
            CustomDebug.LogForComponent($"A robot ({robotToDie.name}) should die.",
                this, IS_DEBUGGING);
            #endregion Logs
            // All bots
            IReadOnlyList<GameObject> temp_allBots = robotHelpersSingleton.FindAllBotRoots();
            int temp_amountAllBots = temp_allBots.Count;

            // Remaining bots
            List<GameObject> temp_remainingBots = new List<GameObject>();
            for (int i = 0; i < temp_amountAllBots; ++i)
            {
                GameObject temp_curBot = temp_allBots[i];
                IRobotHealth temp_curBotHp = temp_curBot.GetComponent<IRobotHealth>();
                #region Asserts
                CustomDebug.AssertIComponentOnOtherIsNotNull(temp_curBotHp, temp_curBot, this);
                #endregion Asserts

                // If the current bot is not one of the dying ones, then add it remains
                if (temp_curBotHp != robotToDie)
                {
                    temp_remainingBots.Add(temp_curBot);
                }
            }
            int temp_amountRemainingBots = temp_remainingBots.Count;
            #region Asserts
            CustomDebug.AssertIsTrueForComponent(temp_amountRemainingBots ==
                temp_amountAllBots - 1, $"the remaining bots list to have a " +
                $"length equal to the amount of all bots minus 1", this);
            #endregion Asserts

            // One bot remaining, so it wins
            if (temp_amountRemainingBots == 1)
            {
                ITeamIndex temp_curBotTeam
                    = temp_remainingBots[0].GetComponent<ITeamIndex>();
                #region Asserts
                CustomDebug.AssertIComponentOnOtherIsNotNull(temp_curBotTeam,
                    temp_remainingBots[0].gameObject, this);
                #endregion Asserts
                #region Logs
                CustomDebug.LogForComponent($"Ending game with single winner. " +
                    $"Winning team index is {temp_curBotTeam.teamIndex}",
                    this, IS_DEBUGGING);
                #endregion Logs
                m_gameOverMonitor.EndGame(eGameOverCause.Health,
                    temp_curBotTeam.teamIndex);
            }
            // If there are no bots remaining (probably shouldn't happen)
            // but lets handle it anyway
            else if (temp_amountRemainingBots <= 0)
            {
                #region Logs
                CustomDebug.LogForComponent($"Ending game with no winner",
                    this, IS_DEBUGGING);
                #endregion Logs
                m_gameOverMonitor.EndGameWithNoWinner(eGameOverCause.Health);
            }
            // Otherwise, there is more than 1 and not 0, so there are multiple bots
            // left, which doesn't happen in 

            onBotShouldDie?.Invoke(robotToDie.gameObject);
        }
    }
}
