using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using DuolBots.Mirror;

using NaughtyAttributes;
using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Singleton containing helper functions for robots.
    /// </summary>
    public class RobotHelpersSingleton :
        DynamicSingletonMonoBehaviourPersistant<RobotHelpersSingleton>
    {
        private const bool IS_DEBUGGING = true;

        [SerializeField] [Tag] private string m_botRootTag = "Robot";


        /// <summary>
        /// Finds all the bot roots in the scene and returns them.
        ///
        /// Pre Conditions - Assumes all the bot roots in the scene are tagged
        /// with m_botRootTag.
        /// </summary>
        public GameObject[] FindAllBotRoots(bool requiresFinding = true)
        {
            GameObject[] temp_botsInScene = GameObject.
                FindGameObjectsWithTag(m_botRootTag);
            if (requiresFinding)
            {
                CustomDebug.AssertIsTrueForComponent(temp_botsInScene.Length > 0,
                    $"At least 1 object with tag={m_botRootTag} to be in the " +
                    $"scene.", this);
            }

            return temp_botsInScene;
        }
        /// <summary>
        /// Finds a robot in the scene with the specified team index.
        ///
        /// Pre Conditions - Assumes every object with the tag m_botRootTag
        /// has an ITeamIndex attached to it. Assumes that there is exactly one
        /// object tagged m_botRootTag in the scene with the specified team index.
        /// Post Conditions - No local changes. Finds and returns the bot root
        /// with the specified team index.
        /// </summary>
        public GameObject FindBotRoot(byte teamIndex)
        {
            GameObject[] temp_botsInScene = FindAllBotRoots();

            foreach (GameObject temp_curBot in temp_botsInScene)
            {
                ITeamIndex temp_curBotTI = temp_curBot.GetComponent<ITeamIndex>();
                Assert.IsNotNull(temp_curBotTI, $"{name}'s {GetType().Name} " +
                    $"expected {temp_curBot.name} to have {nameof(ITeamIndex)} " +
                    $"attached, but none was found.");

                // If we found a bot with the specified team index.
                if (temp_curBotTI.teamIndex == teamIndex)
                {
                    return temp_curBot;
                }
            }

            Debug.LogError($"No robot with tag {m_botRootTag} could be found for " +
                $"team index {teamIndex}");
            return null;
        }
        /// <summary>
        /// Network helper to find which bot is owned by the current client.
        ///
        /// Pre Conditions - Assumes every bot root has BotRootOwnership attached.
        /// Assumes that at least one bot root is owned by the current client.
        /// Post Conditions - No local changes. Finds and returns the bot root
        /// that this client has ownership over.
        /// </summary>
        public GameObject FindOwnedBotRootNetwork()
        {
            GameObject[] temp_robots = FindAllBotRoots();
            // Determine which robot is owned by the current client.
            foreach (GameObject temp_curBot in temp_robots)
            {
                BotRootOwnership temp_botOwnership = temp_curBot.
                    GetComponent<BotRootOwnership>();
                Assert.IsNotNull(temp_botOwnership, $"{name}'s {GetType().Name} " +
                    $"requires {nameof(BotRootOwnership)} be attached to " +
                    $"{temp_curBot.name} but none was found.");

                if (temp_botOwnership.isMyBot)
                {
                    return temp_curBot;
                }
            }

            Debug.LogError($"There was no bot root in the scene " +
                $"that this client owns");
            return null;
        }
        /// <summary>
        /// Warning! Untested.
        ///
        /// If there is a network server calls
        /// <see cref="DetermineMyTeamIndexNetwork"/>.
        /// If there is no server, returns 0.
        /// </summary>
        public byte DetermineMyTeamIndex()
        {
            // If there is a server, that means this is networked.
            if (NetworkServer.active)
            {
                return DetermineMyTeamIndexNetwork();
            }

            // No server, so team index is 0
            return 0;
        }
        /// <summary>
        /// Network helper to find out what the current client's team index is.
        ///
        /// Pre Conditions - Assumes that the bot root has a ITeamIndex attached
        /// to it.
        /// Post Conditions - No local changes. Finds the owned bot and returns
        /// its team index.
        /// </summary>
        public byte DetermineMyTeamIndexNetwork()
        {
            GameObject temp_botRoot = FindOwnedBotRootNetwork();

            ITeamIndex temp_teamIndex = temp_botRoot.GetComponent<ITeamIndex>();
            Assert.IsNotNull(temp_teamIndex, $"{name}'s {GetType().Name} expected " +
                $"{temp_botRoot.name} to have {nameof(ITeamIndex)} attached but " +
                $"none was found");

            return temp_teamIndex.teamIndex;
        }
        /// <summary>
        /// Finds the robot with the least health. If there are multiple robots
        /// with the same health which also is the least, will return all those
        /// robots.
        ///
        /// Pre Conditions - Assumes that the bot root has <see cref="IRobotHealth"/>
        /// attached to it.
        /// Post Conditions - No local changes. Returns a list containing the bot with
        /// the least health (or if there is a tie, all the bots with
        /// the least health).
        /// </summary>
        public IReadOnlyList<IRobotHealth> FindRobotsWithLeastHealth()
        {
            GameObject[] temp_botRoots = FindAllBotRoots();
            // Hold the robots in a list in the case that there are multiple
            // bots with the same health.
            List<IRobotHealth> temp_curLowestHealthBotList = new List<IRobotHealth>();
            float temp_curLowestHealthPercentage = float.MaxValue;
            // Check every bot and store the lowest
            foreach (GameObject temp_singleBotRoot in temp_botRoots)
            {
                IRobotHealth temp_curBotHP = temp_singleBotRoot.
                    GetComponent<IRobotHealth>();

                Debug.Log($"{temp_singleBotRoot.GetComponent<ITeamIndex>().teamIndex} : {temp_curBotHP.maxHealth}");

                float temp_curHealthPercent = temp_curBotHP.currentHealth / temp_curBotHP.maxHealth;


                Assert.IsNotNull(temp_curBotHP, $"{name}'s {GetType().Name} " +
                    $"expected {temp_singleBotRoot.name} to have " +
                    $"{nameof(IRobotHealth)} attached but none was found");

                CustomDebug.Log($"Current bot has {temp_curHealthPercent}. " +
                    $"Current lowest is {temp_curLowestHealthPercentage}", IS_DEBUGGING);

                // If the current health is lower than current, replace the whole list
                if (temp_curHealthPercent < temp_curLowestHealthPercentage)
                {
                    temp_curLowestHealthPercentage = temp_curHealthPercent;
                    temp_curLowestHealthBotList.Clear();
                    temp_curLowestHealthBotList.Add(temp_curBotHP);
                }
                // If its tied for current, append it to the list.
                else if (temp_curHealthPercent == temp_curLowestHealthPercentage)
                {
                    temp_curLowestHealthBotList.Add(temp_curBotHP);
                }
            }

            return temp_curLowestHealthBotList;
        }
        /// <summary>
        /// Finds the robot with the most health. If there are multiple robots
        /// with the same health which also is the most, will return all those
        /// robots.
        ///
        /// Pre Conditions - Assumes that the bot root has <see cref="IRobotHealth"/>
        /// attached to it.
        /// Post Conditions - No local changes. Returns a list containing the bot with
        /// the most health (or if there is a tie, all the bots with
        /// the most health).
        /// </summary>
        public IReadOnlyList<IRobotHealth> FindRobotsWithMostHealth()
        {
            GameObject[] temp_botRoots = FindAllBotRoots();
            // Hold the robots in a list in the case that there are multiple
            // bots with the same health.
            List<IRobotHealth> temp_curHighestHealthBotList = new List<IRobotHealth>();
            float temp_curHighestHealthPercent = float.MinValue;
            // Check every bot and store the highest
            foreach (GameObject temp_singleBotRoot in temp_botRoots)
            {

                IRobotHealth temp_curBotHP = temp_singleBotRoot.
                    GetComponent<IRobotHealth>();

                float temp_curHealthPercent = temp_curBotHP.currentHealth / temp_curBotHP.maxHealth;

                #region Asserts
                CustomDebug.AssertIComponentOnOtherIsNotNull(temp_curBotHP, temp_singleBotRoot,
                    this);
                #endregion Asserts
                #region Logs
                CustomDebug.LogForComponent($"Current bot has {temp_curHealthPercent}. " +
                    $"Current highest is {temp_curHighestHealthPercent}", this, IS_DEBUGGING);
                #endregion Logs

                // If the current health is lower than current, replace the whole list
                if (temp_curHealthPercent > temp_curHighestHealthPercent)
                {
                    temp_curHighestHealthPercent = temp_curHealthPercent;
                    temp_curHighestHealthBotList.Clear();
                    temp_curHighestHealthBotList.Add(temp_curBotHP);
                }
                // If its tied for current, append it to the list.
                else if (temp_curHighestHealthPercent == temp_curHealthPercent)
                {
                    temp_curHighestHealthBotList.Add(temp_curBotHP);
                }
            }

            return temp_curHighestHealthBotList;
        }
        public IReadOnlyList<GameObject> FindLosingBots(IReadOnlyList<byte> winningTeamIndices)
        {
            #region Logs
            CustomDebug.LogForContainerElements<byte>($"", winningTeamIndices, IS_DEBUGGING);
            #endregion Logs

            List<GameObject> temp_winningBots = new List<GameObject>(winningTeamIndices.Count);
            for (int i = 0; i < winningTeamIndices.Count; ++i)
            {
                byte temp_curWinTeamIndex = winningTeamIndices[i];
                temp_winningBots.Add(FindBotRoot(temp_curWinTeamIndex));
                #region Logs
                CustomDebug.Log($"{name} has {temp_winningBots.Count} winners which are: {temp_winningBots}", IS_DEBUGGING);
                CustomDebug.LogForContainerGameObjects($"", temp_winningBots, IS_DEBUGGING);
                #endregion Logs
            }

            // Now find all the bots and then find the difference between the two sets
            // to figure out the losing bots.
            IReadOnlyList<GameObject> temp_allBots = FindAllBotRoots();
            #region Log
            CustomDebug.LogForContainerGameObjects($"", temp_allBots, IS_DEBUGGING);
            #endregion Log
            List<GameObject> temp_losingBots = new List<GameObject>(temp_allBots.Count
                - temp_winningBots.Count);
            for (int i = 0; i < temp_allBots.Count; ++i)
            {
                GameObject temp_curBot = temp_allBots[i];
                // If its not a winning bot root, then its a losing bot root
                if (!temp_winningBots.Contains(temp_curBot))
                {
                    temp_losingBots.Add(temp_curBot);
                }
                #region Logs
                CustomDebug.Log($"{name} has {temp_losingBots.Count} losers which are: {temp_losingBots}", IS_DEBUGGING);
                CustomDebug.LogForContainerGameObjects($"", temp_losingBots, IS_DEBUGGING);
                #endregion Logs
            }

            return temp_losingBots.Count == 0 ? temp_winningBots : temp_losingBots;
        }
    }
}
