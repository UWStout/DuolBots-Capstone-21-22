using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Mirror;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Spawns the bots over the server acting as an inbetween between
    /// the BattlePlayerNetworkObject and the BattleBotInstantiator since
    /// the BattleBotInstantiator was not designed with server-clients in
    /// mind.
    /// </summary>
    [RequireComponent(typeof(BattleBotInstantiator))]
    public class NetworkBotInstantiator : NetworkBehaviour
    {
        private const bool IS_DEBUGGING = false;

        private static event Action onAllBotsInstantiated;
        private static bool areAllBotsInstantiated = false;

        [SerializeField, Required] private TeamSpawnPoints m_teamSpawnPoints = null;

        private BattleBotInstantiator m_botInstantiator = null;


        // Called on both client and server
        private void Awake()
        {
            m_botInstantiator = GetComponent<BattleBotInstantiator>();

            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_botInstantiator, this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_teamSpawnPoints,
                nameof(m_teamSpawnPoints), this);
            #endregion Asserts
        }


        public static void ToggleSubscriptionToOnAllBotsInstantiated(
            Action subFunc, bool subOrUnsub)
        {
            #region Logs
            CustomDebug.Log($"{nameof(ToggleSubscriptionToOnAllBotsInstantiated)} " +
                $"{subFunc} for {subFunc}", IS_DEBUGGING);
            #endregion Logs
            // Sub (true)
            if (subOrUnsub)
            {
                if (areAllBotsInstantiated)
                {
                    subFunc.Invoke();
                }
                onAllBotsInstantiated += subFunc;
            }
            // Unsub (false)
            else
            {
                onAllBotsInstantiated -= subFunc;
            }
        }

        /// <summary>
        /// Instantiates the bots for all teams.
        /// </summary>
        [Server]
        public void SpawnBots()
        {
            InstantiateBots();
        }


        /// <summary>
        /// Creates bots for the two teams on the server and then has the clients
        /// spawn them as well.
        /// </summary>
        [Server]
        private void InstantiateBots()
        {
            #region Asserts
            Assert.IsNotNull(m_botInstantiator, $"{nameof(m_botInstantiator)} " +
                $"is null when trying to instantiate bots");
            #endregion Asserts

            // Spawn bots on the server to create the bot roots
            BuiltBotDataWithTeamIndex[] temp_allBotData
                = BuildSceneBotData.GetAllData();
            #region Logs
            CustomDebug.Log($"There is built bot data for " +
                $"{temp_allBotData.Length} teams", IS_DEBUGGING);
            #endregion Logs
            foreach (BuiltBotDataWithTeamIndex temp_singleBotData in temp_allBotData)
            {
                byte temp_curTeamIndex = temp_singleBotData.teamIndex;
                #region Logs
                CustomDebug.Log($"Instantiating bot for team with index " +
                    $"{temp_singleBotData.teamIndex}.", IS_DEBUGGING);
                #endregion Logs
                InstantiateSingleBotServer(temp_curTeamIndex);
            }

            // Invoke the all bots instantiated event on the server
            #region Logs
            CustomDebug.Log($"Invoking event {nameof(onAllBotsInstantiated)}",
                IS_DEBUGGING);
            #endregion Logs
            areAllBotsInstantiated = true;
            onAllBotsInstantiated?.Invoke();
            // Make a client rpc invoking the all bots instantiated event on the server
            InvokeAllBotsInstantiatedClientRpc();
        }
        /// <summary>
        /// Instantiates the bot for the team corresponding to the specified team index.
        /// Creates a new bot root and syncs it to the clients with ownership to the team
        /// whose bot it is.
        ///
        /// Pre Conditions - Assumes m_teamSpawnPoints and m_botInstantiator are not null.
        /// Assumes that a BattlePlayerNetworkObject for the given, corresponding team
        /// exists. Assumes that the constructed bot root has a NetworkTeamIndex on it.
        /// Post Conditions - Spawns a bot for the corresponding team to the given index
        /// on the server and relays it to the clients, giving the corresponding team's
        /// connection ownership.
        /// </summary>
        /// <param name="teamIndex">Team to instantiate the bot for.</param>
        [Server]
        private void InstantiateSingleBotServer(byte teamIndex)
        {
            #region Asserts
            Assert.IsNotNull(m_teamSpawnPoints, $"{nameof(m_teamSpawnPoints)} is null in " +
                $"{GetType()}'s {nameof(InstantiateSingleBotServer)}");
            Assert.IsNotNull(m_botInstantiator, $"{nameof(m_botInstantiator)} is null in " +
                $"{GetType()}'s {nameof(InstantiateSingleBotServer)}");
            #endregion Asserts

            BotUnderConstruction temp_botConstructed = m_botInstantiator.CreateBot(
                teamIndex, m_teamSpawnPoints.GetSpawnLocation(teamIndex).position);

            // Get the player (team) object to spawn the bot with ownership
            BattlePlayerNetworkObject temp_teamNetworkObject =
                FindBattlePlayerNetworkObjectWithTeamIndex(teamIndex);
            #region Asserts
            Assert.IsNotNull(temp_teamNetworkObject, $"Could not find a " +
                $"{nameof(BattlePlayerNetworkObject)} with team index {teamIndex}");
            #endregion Asserts

            // Get the bot root
            GameObject temp_currentBotRoot = temp_botConstructed.currentBotRoot;
            #region Asserts
            Assert.IsNotNull(temp_currentBotRoot, $"BotRoot is null when " +
                $"trying to relay it's spawn to clients from the server");
            #endregion Asserts
            // Set the bot root's team index
            ITeamIndex temp_teamIndex = temp_currentBotRoot.GetComponent<ITeamIndex>();
            #region Asserts
            CustomDebug.AssertIComponentOnOtherIsNotNull(temp_teamIndex,
                temp_currentBotRoot, this);
            #endregion Asserts
            temp_teamIndex.teamIndex = teamIndex;

            // Get the network child manager
            NetworkChildManager temp_netChildMan =
                temp_currentBotRoot.GetComponent<NetworkChildManager>();
            #region Asserts
            CustomDebug.AssertComponentOnOtherIsNotNull(temp_netChildMan,
                temp_currentBotRoot, this);
            #endregion Asserts
            // Get the bot parts
            GameObject temp_botChassis = temp_botConstructed.currentChassis;
            GameObject temp_botMovement = temp_botConstructed.currentMovementPart;
            IReadOnlyDictionary<int, GameObject> temp_botSlottedParts =
                temp_botConstructed.currentSlottedParts;

            #region Asserts
            CustomDebug.AssertIsTrueForComponent(
                temp_teamNetworkObject.connectionToClient != null,
                $"{temp_teamNetworkObject} to have a connectionToClient, " +
                $"but its null", this);
            #endregion Asserts
            // Relay the spawn of the bot root to the clients
            NetworkServer.Spawn(temp_currentBotRoot,
                temp_teamNetworkObject.connectionToClient);
            // Relay spawn of bot aprts to the clients
            temp_netChildMan.Spawn(temp_botChassis);
            temp_netChildMan.Spawn(temp_botMovement);
            foreach (KeyValuePair<int, GameObject> temp_kvp in temp_botSlottedParts)
            {
                temp_netChildMan.Spawn(temp_kvp.Value);
            }
        }

        /// <summary>
        /// Instantiates the bot for the team corresponding to the specified team index.
        /// Does not create a new bot root. Tries to find an existing bot root for that
        /// team that should have been spawned on the server.
        ///
        /// Pre Conditions - Assumes that a bot root for the corresponding team has been
        /// spawned on the server and relayed to this client already. Assumes
        /// m_teamSpawnPoints and m_botInstantiator are not null.
        /// Post Conditions - Spawns new instances of all the parts on the bot root. These
        /// are new instances and are not the same instances as on the server (don't have
        /// a NetworkIdentity). Also changes what is stored in BuildSceneBotData to the
        /// given bot data (to match the server).
        /// </summary>
        /// <param name="teamIndex">Team index for who to spawn the bot for.</param>
        /// <param name="botData">Data for the bot to spawn.</param>
        [ClientRpc]
        private void InstantiateSingleBotClientRpc(byte teamIndex, BuiltBotData botData)
        {
            // If Host, don't spawn the bots twice
            if (isServer) { return; }

            // Set the data for the bot before instantiating it.
            CustomDebug.Log($"Setting data for team {teamIndex} on CLIENT", IS_DEBUGGING);
            BuildSceneBotData.SetData(teamIndex, botData);

            // Find the bot root with the given team index.
            RobotHelpersSingleton temp_botHelpers = RobotHelpersSingleton.instance;
            GameObject temp_botRootInstance = temp_botHelpers.FindBotRoot(teamIndex);

            #region Asserts
            Assert.IsNotNull(m_teamSpawnPoints, $"{nameof(m_teamSpawnPoints)} is null in " +
                $"{GetType()}'s {nameof(InstantiateSingleBotClientRpc)}");
            Assert.IsNotNull(m_botInstantiator, $"{nameof(m_botInstantiator)} is null in " +
                $"{GetType()}'s {nameof(InstantiateSingleBotClientRpc)}");
            #endregion Asserts

            // Create the bot using the already instantiated bot root as the root.
            Vector3 temp_spawnPos = m_teamSpawnPoints.GetSpawnLocation(teamIndex).position;
            BotUnderConstruction temp_botConstructed = m_botInstantiator.CreateBot(
                teamIndex, temp_spawnPos, temp_botRootInstance);
        }
        /// <summary>
        /// Simply invokes onAllBotsInstantiated on each client.
        /// </summary>
        [ClientRpc]
        private void InvokeAllBotsInstantiatedClientRpc()
        {
            // Protect against calling it twice on host
            if (isServer) { return; }

            onAllBotsInstantiated?.Invoke();
        }

        /// <summary>
        /// Finds a BattlePlayerNetworkObject with the given team index in the scene.
        /// 
        /// Pre Conditions - There must be exactly 2 BattlePlayerNetworkObjects in the
        /// scene, 1 for each team. One of them must have a team index qual to the
        /// specified team index.
        /// Post Conditions - Returns the found BattlePlayerNetworkObject. Returns
        /// null if no BattlePlayerNetworkObjects with the specified team index were
        /// found.
        /// </summary>
        /// <param name="teamIndex">Team index of the BattlePlayerNetworkObject
        /// to find.</param>
        /// <returns>The found BattlePlayerNetworkObject. Or null if
        /// no BattlePlayerNetworkObjects with the specified team index were
        /// found.</returns>
        private BattlePlayerNetworkObject FindBattlePlayerNetworkObjectWithTeamIndex(
            byte teamIndex)
        {
            BattlePlayerNetworkObject[] temp_playerObjArr =
                FindObjectsOfType<BattlePlayerNetworkObject>();

            foreach (BattlePlayerNetworkObject temp_singlePlayerObj in temp_playerObjArr)
            {
                NetworkTeamIndex temp_netTeamIndex =
                    temp_singlePlayerObj.GetComponent<NetworkTeamIndex>();
                Assert.IsNotNull(temp_netTeamIndex, $"Expected to find a " +
                    $"{nameof(NetworkTeamIndex)} attched to {temp_singlePlayerObj.name}, " +
                    $"but none was found");
                if (temp_netTeamIndex.teamIndex == teamIndex)
                {
                    return temp_singlePlayerObj;
                }
            }
            Debug.LogError($"No {nameof(BattlePlayerNetworkObject)} with team index " +
                $"{teamIndex} was found");
            return null;
        }
    }
}
