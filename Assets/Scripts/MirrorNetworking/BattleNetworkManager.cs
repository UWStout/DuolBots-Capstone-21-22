using UnityEngine;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    public class BattleNetworkManager : NetworkManager
    {
        private const bool IS_DEBUGGING = false;

        public static void SetDisconnectIntended() => s_isDisconnectIntended = true;
        private static bool s_isDisconnectIntended = false;

        // Supposed to be WAHt the offline scene is for, but thats not working
        // for some reason.
        [SerializeField] [NaughtyAttributes.Scene]
        private string m_disconnectSceneName = "Battle_HostJoin_SCENE";


        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            GameObject temp_connectionObject =
                Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

            NetworkServer.AddPlayerForConnection(conn, temp_connectionObject);
        }
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);

            // If we just had max connections (everyone was here),
            // then reduce the max connetions so that no one else
            // can join after losing someone.
            if (numPlayers + 1 >= maxConnections)
            {
                maxConnections = numPlayers;
            }

            // If the disconnect is intentional, then the one
            // who disconnected is the host and not their opponent.
            // We need to reset the disconnect intended, but we assume that
            // this is the host, so that will be done in client.
            if (s_isDisconnectIntended) { return; }

            // Client has disconnected mid game. Just say the game is over.
            // The host has won since their opponent left.
            GameOverMonitor temp_gameOverMon = GameOverMonitor.instance;
            // Pass in team index instead of robot.
            BattlePlayerNetworkObject temp_myPlayer
                = BattlePlayerNetworkObject.myPlayerInstance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(temp_gameOverMon,
                this);
            CustomDebug.AssertIsTrueForComponent(temp_myPlayer != null,
                $"a player instance for this connection to exist.", this);
            #endregion Asserts
            ITeamIndex temp_myTeamIndex = temp_myPlayer.teamIndex;
            #region Asserts
            CustomDebug.AssertIComponentOnOtherIsNotNull(temp_myTeamIndex,
                temp_myPlayer.gameObject, this);
            #endregion Asserts
            temp_gameOverMon.EndGame(eGameOverCause.Disconnect,
                temp_myTeamIndex.teamIndex);
        }
        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            CustomDebug.LogForComponent(nameof(OnClientDisconnect), this,
                IS_DEBUGGING);

            // If the client is disconnected from the server, take them to the
            // disconnect scene. However, the disconnect scene expects to have
            // the network manager there, so we no longer need this network manager.
            //Destroy(gameObject);
            // Also, tell the host join scene that we were disconnected if it was
            // unintentional.
            if (!s_isDisconnectIntended)
            {
                // TODO - if the host leaving caused this, instead show some kind of
                // game over screen where this player wins.
                HostJoinSceneController.wasDisconnected = true;
            }
            // Reset the static flag
            else
            {
                s_isDisconnectIntended = false;
            }

            /*
            // Don't load a scene for the host, they already do so
            // by menuing.
            if (temp_myPlayer.isServer) { return; }

            SceneLoader temp_sceneLoader = SceneLoader.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(temp_sceneLoader,
                this);
            #endregion Asserts
            if (offlineScene == "") { return; }
            SceneLoader.instance.LoadScene(offlineScene);
            */
        }
    }
}
