using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

using Mirror;
using Steamworks;
// Original Authors - Wyatt Senalik
// Resources used: https://github.com/rlabrecque/Steamworks.NET/releases,
// https://mirror-networking.gitbook.io/docs/transports/fizzysteamworks-transport,
// https://partner.steamgames.com/doc/features/multiplayer/matchmaking, and
// https://partner.steamgames.com/doc/api
// Tutorials used: https://www.youtube.com/watch?v=QlbBC07dqnE and
// https://fatrodzianko.com/2021/08/02/gamedev-blog-using-mirror-steamworks-net-to-create-multiplayer-lobbies-in-unity/

namespace DuolBots.Mirror.Steam
{
    /// <summary>
    /// Manages hosting and joining a lobby using steam.
    /// </summary>
    [RequireComponent(typeof(NetworkManager))]
    public class SteamBattleLobby : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;
        private const string HOST_ADDRESS_KEY = "HostAddressKey";

        [SerializeField]
        private UnityEvent onJoinedLobbyViaSteamFriends = new UnityEvent();

        // References
        private NetworkManager m_networkManager = null;

        private bool m_isConnectingViaFriendsList = false;

        // Why are these protected? All the resources I found had these
        // callbacks marked protected, but I do not know the reason why.
        // To keep the magic working, I also used protected to avoid any
        // unnecessary errors.
        protected Callback<LobbyCreated_t> m_lobbyCreated = null;
        protected Callback<GameLobbyJoinRequested_t>
            m_gameLobbyJoinRequested = null;
        protected Callback<LobbyEnter_t> m_lobbyEnter = null;
        protected Callback<LobbyMatchList_t> m_lobbyList = null;


        // Called 0th
        private void Awake()
        {
            m_networkManager = GetComponent<NetworkManager>();
            Assert.IsNotNull(m_networkManager, $"{name}'s {GetType().Name} " +
                $"requires that {nameof(NetworkManager)} be attached " +
                $"but none was found.");
        }
        // Called 1st
        private void Start()
        {
            // Do nothing if steam manager is not initialized
            if (!SteamManager.Initialized)
            {
                Debug.LogError($"Steam is not initialized");
                return;
            }

            m_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            m_gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>
                .Create(OnGameLobbyJoinRequested);
            m_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
            m_lobbyList = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);
        }

        public void HostPrivateLobby()
        {
            // By setting friends only, this does not show up in lobby lists
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly,
                m_networkManager.maxConnections);

            CurrentLobbyData.currentLobbyType = eLobbyType.Private;
        }
        public void OpenSteamJoinOverlay()
        {
            // Here we are trying to join via the friends list.
            m_isConnectingViaFriendsList = true;

            SteamFriends.ActivateGameOverlay(SteamOverlayDialogue.FRIENDS);

            CurrentLobbyData.currentLobbyType = eLobbyType.Private;
        }
        public void JoinQueue()
        {
            // We are trying to join via queue her and not with the friends list.
            m_isConnectingViaFriendsList = false;

            // Filter the search to only show lobbies that are
            // still waiting for players.
            SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1);
            // This will call OnLobbyMatchList
            SteamAPICall_t temp_tryGetList = SteamMatchmaking.RequestLobbyList();

            CurrentLobbyData.currentLobbyType = eLobbyType.Queue;
        }


        /// <summary>
        /// StartHost using Mirror when a player creates a Steam lobby.
        /// </summary>
        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                CustomDebug.Log($"Failed to connect with result " +
                    $"{callback.m_eResult}", IS_DEBUGGING);
                return;
            }

            // Mirror start server
            m_networkManager.StartHost();

            // Set Steam lobby data
            CSteamID temp_lobbyID = new CSteamID(callback.m_ulSteamIDLobby);
            string temp_steamUserIDStr = SteamUser.GetSteamID().ToString();
            SteamMatchmaking.SetLobbyData(temp_lobbyID,
                HOST_ADDRESS_KEY, temp_steamUserIDStr);
        }
        /// <summary>
        /// Allow the client to join the lobby when they request.
        /// </summary>
        private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }
        /// <summary>
        /// StartClient using Mirror when SteamMatchingmaking says the client can
        /// enter the lobby.
        /// </summary>
        private void OnLobbyEnter(LobbyEnter_t callback)
        {
            // Protect against host calling this
            if (NetworkServer.active) { return; }

            // Get Steam lobby data
            CSteamID temp_lobbyID = new CSteamID(callback.m_ulSteamIDLobby);
            string temp_hostAddr = SteamMatchmaking.GetLobbyData(temp_lobbyID,
                HOST_ADDRESS_KEY);

            // If we just joined from the friends list.
            if (m_isConnectingViaFriendsList)
            {
                onJoinedLobbyViaSteamFriends.Invoke();
            }

            // Join using Mirror
            m_networkManager.networkAddress = temp_hostAddr;
            m_networkManager.StartClient();
        }
        private void OnLobbyMatchList(LobbyMatchList_t callback)
        {
            CustomDebug.Log($"Found {callback.m_nLobbiesMatching} lobbies.",
                IS_DEBUGGING);

            // If there exist lobbies that fit the criteria, join them.
            if (callback.m_nLobbiesMatching > 0)
            {
                // Get a lobby and attempt to join it.
                CSteamID temp_lobbyID = SteamMatchmaking.GetLobbyByIndex(0);
                SteamMatchmaking.JoinLobby(temp_lobbyID);
            }
            // Otherwise, no lobbies fit the criteria, so host a new lobby.
            else
            {
                // By setting invisible, it is will show up in lobby lists,
                // but not be visible to other friends.
                SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeInvisible,
                    m_networkManager.maxConnections);
            }
        }
    }
}
