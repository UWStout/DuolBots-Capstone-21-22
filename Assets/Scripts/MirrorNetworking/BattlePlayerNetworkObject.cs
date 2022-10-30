using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

using Mirror;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Controller script for the player's (team's) network object.
    /// Sets up players (team) after spawning and incites the start of
    /// the game once every player (team) has joined.
    /// We call the team a player here because player here refers to a
    /// single connection to the server and not each individual player in
    /// terms of the people playing.
    /// </summary>
    [RequireComponent(typeof(NetworkTeamIndex))]
    public class BattlePlayerNetworkObject : NetworkBehaviour
    {
        private const bool IS_DEBUGGING = false;
        
        [SerializeField] [Required] private GameObject m_playerInputPrefab = null;

        private TeamConnectionManager m_teamConMan = null;

        private NetworkTeamIndex m_teamIndex = null;

        public static BattlePlayerNetworkObject myPlayerInstance
        { get; private set; }
        public ITeamIndex teamIndex => m_teamIndex;


        // Called on both client and server
        private void Awake()
        {
            m_teamIndex = GetComponent<NetworkTeamIndex>();

            #region Asserts
            Assert.IsNotNull(m_teamIndex, $"{nameof(NetworkTeamIndex)} was " +
                $"requried to be attached to {name} but was not found");
            #endregion Asserts
        }
        public override void OnStartServer()
        {
            base.OnStartServer();

            // Determine the team index for this team's object
            m_teamIndex.teamIndex = BattleTeamIndexProvider.instance.
                RequestNewTeamIndex();
            #region Logs
            CustomDebug.Log($"TeamIndex for this local team has " +
                $"been determined to be {m_teamIndex.teamIndex}", IS_DEBUGGING);
            #endregion Logs

            m_teamConMan = TeamConnectionManager.instance;
            #region Asserts
            CustomDebug.AssertIsTrueForComponent(m_teamConMan != null,
                $"{nameof(TeamConnectionManager)} to exist in the scene, but " +
                $"none was found.", this);
            #endregion Asserts
        }
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            CustomDebug.Log($"OnStartLocalPlayer", IS_DEBUGGING);

            if (myPlayerInstance == null)
            {
                myPlayerInstance = this;
            }
            else
            {
                Debug.LogError($"There are multiple local players " +
                    $"for {m_teamIndex.teamIndex}");
            }

            SpawnPlayerInputs();

            // Is Host
            if (isClient && isServer)
            {
                CustomDebug.Log($"This player is the host", IS_DEBUGGING);
                // Bot data is already set on the host, no
                // need to send the data over since its already here.
            }
            // Is Client (not host)
            else if (isClient)
            {
                CustomDebug.Log($"This player is the client", IS_DEBUGGING);
                // Send the data to the host for them to instantiate
                // Must get the bot data using the team index it was saved with
                byte temp_originalTeamIndex = 0;
                CmdSendBotDataToServer(
                    BuildSceneBotData.GetBotData(temp_originalTeamIndex));
            }

            // Connect the team after we've set the bot data.
            ConnectTeamCmd();
        }
        public override void OnStopServer()
        {
            base.OnStopServer();

            // Connected on the server, so we need to disconnect on
            // the server too.
            m_teamConMan.DisconnectTeam(m_teamIndex.teamIndex);
        }


        /// <summary>
        /// Spawns PlayerInput in the battle scene for each player stored
        /// in the CurrentPlayerInputDevices.
        /// </summary>
        [Client]
        private void SpawnPlayerInputs()
        {
            // Spawn a PlayerInput
            foreach (KeyValuePair<int, ReadOnlyArray<InputDevice>> temp_kvp in
                CurrentPlayerInputDevices.GetAllPlayerInputDevices())
            {
                int temp_playerIndexValue = temp_kvp.Key;
                ReadOnlyArray<InputDevice> temp_inputDevices = temp_kvp.Value;

                // Spawn the player input
                PlayerInput temp_spawnedPlayerInp = PlayerInput.Instantiate(
                    m_playerInputPrefab, playerIndex: temp_playerIndexValue,
                    pairWithDevices: temp_inputDevices.ToArray());

                // Grab the player and team index components off the
                // spawned player input object
                PlayerIndex temp_spawnedPlayerIndex =
                    temp_spawnedPlayerInp.GetComponent<PlayerIndex>();
                ITeamIndex temp_spawnedTeamIndex =
                    temp_spawnedPlayerInp.GetComponent<ITeamIndex>();
                // Initialize the values of the player and team indices
                temp_spawnedPlayerIndex.playerIndex = (byte)temp_playerIndexValue;
                temp_spawnedTeamIndex.teamIndex = m_teamIndex.teamIndex;
            }
        }

        /// <summary>
        /// Tells the server to connect the team.
        /// </summary>
        [Command]
        private void ConnectTeamCmd()
        {
            m_teamConMan.ConnectTeam(m_teamIndex.teamIndex);
        }
        /// <summary>
        /// Sends bot data from the client to the server to store.
        ///
        /// Pre Conditions - None.
        /// Post Conditions - Saves the given bot data to the server's
        /// BuildSceneBotData to be used to spawn the bots later.
        /// </summary>
        /// <param name="botData">Bot data to save.</param>
        [Command]
        private void CmdSendBotDataToServer(BuiltBotData botData)
        {
            byte temp_teamIndex = m_teamIndex.teamIndex;
            // Send data to server from client
            BuildSceneBotData.SetData(temp_teamIndex, botData);
            // Relay updated BuilSceneBotData to clients
            BuiltBotDataWithTeamIndex[] temp_allBotData =
                BuildSceneBotData.GetAllData();

            #region Logs
            CustomDebug.Log($"Setting data for team {temp_teamIndex} on SERVER",
                IS_DEBUGGING);
            foreach (BuiltBotDataWithTeamIndex temp_dataWithIndex
                in temp_allBotData)
            {
                CustomDebug.Log($"Server is sending data for team" +
                    $"{temp_dataWithIndex.teamIndex}", IS_DEBUGGING);
            }
            #endregion Logs

            UpdateBotSceneDataClientRpc(temp_allBotData);
        }

        /// <summary>
        /// Sets the data for the client's BuildSceneBotData to match
        /// the specified values (from the server).
        /// </summary>
        [ClientRpc]
        private void UpdateBotSceneDataClientRpc(BuiltBotDataWithTeamIndex[]
            dataWithIndexArray)
        {
            #region Logs
            CustomDebug.Log($"Updating client with bot scene data for " +
                $"{dataWithIndexArray.Length} teams", IS_DEBUGGING);
            #endregion Logs
            foreach (BuiltBotDataWithTeamIndex temp_dataWithIndex
                in dataWithIndexArray)
            {
                #region Logs
                CustomDebug.Log($"Client receives data for team" +
                    $"{temp_dataWithIndex.teamIndex}", IS_DEBUGGING);
                #endregion Logs
                BuildSceneBotData.SetData(temp_dataWithIndex);
            }
        }
    }
}
