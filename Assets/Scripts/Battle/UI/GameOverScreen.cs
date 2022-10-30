using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
using NaughtyAttributes;
using TMPro;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    public class GameOverScreen : NetworkBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField] [NaughtyAttributes.Scene]
        private string m_mainMenuSceneName = "MainMenu";
        [SerializeField] [NaughtyAttributes.Scene]
        private string m_hostJoinSceneName = "Battle_HostJoin_SCENE";
        [SerializeField] [NaughtyAttributes.Tag]
        private string m_playerInputTag = "PlayerInput";

        [Space, Header("Rematch")]
        [SerializeField] [Required]
        private TextMeshProUGUI m_rematchTextMesh = null;
        [SerializeField] [ResizableTextArea]
        private string m_baseMyRequestText = "Your team has requested a rematch";
        [SerializeField] [ResizableTextArea] private string m_baseOtherRequestText
            = "Another team has requested a rematch";

        private BattleStateManager m_battleStateMan = null;
        private TeamConnectionManager m_teamConMan = null;
        private RematchController m_rematchCont = null;

        private BattleStateChangeHandler m_gameOverHandler = null;
        private bool m_didThisTeamRequestRematch = false;


        // Domestic Initialization
        private void Awake()
        {
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_rematchTextMesh,
                nameof(m_rematchTextMesh), this);
            #endregion Asserts
        }
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            #region Logs
            CustomDebug.Log($"{name}'s {GetType().Name}'s " +
                $"{nameof(OnStartLocalPlayer)}", IS_DEBUGGING);
            #endregion Logs


            m_battleStateMan = BattleStateManager.instance;
            m_rematchCont = RematchController.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_battleStateMan,
                this);
            CustomDebug.AssertIsTrueForComponent(m_rematchCont != null,
                $"{nameof(RematchController)} to be a singleton in the scene",
                this);
            #endregion Asserts
            m_gameOverHandler = new BattleStateChangeHandler(m_battleStateMan,
                HandleOnGameOverStart, HandleOnGameOverEnd, eBattleState.GameOver);
            m_rematchCont.ToggleSubscriptionToTeamsThatWantRematchUpdate(
                OnTeamsThatWantRematchUpdate, true);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();

            // OnStopLocalPlayer
            if (isLocalPlayer)
            {
                m_rematchCont.ToggleSubscriptionToTeamsThatWantRematchUpdate(
                    OnTeamsThatWantRematchUpdate, false);
            }
        }
        private void Start()
        {
            m_teamConMan = TeamConnectionManager.instance;
            m_battleStateMan = BattleStateManager.instance;
            m_rematchCont = RematchController.instance;

            #region Asserts
            CustomDebug.AssertIsTrueForComponent(m_teamConMan != null,
                $"{nameof(TeamConnectionManager)} to be a singleton in the scene",
                this);
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_battleStateMan,
                this);
            CustomDebug.AssertIsTrueForComponent(m_rematchCont != null,
                $"{nameof(RematchController)} to be a singleton in the scene",
                this);
            #endregion Asserts

        }


        public void OnRematchButton()
        {
            CustomDebug.Log($"{nameof(OnRematchButton)}", IS_DEBUGGING);

            RequestRematch();
        }
        public void OnMainMenuButton()
        {
            CancelRematchRequest();

            // Main menu is before players joined, so we need to kill
            // their main inputs.
            DestroyPlayerInputs();
            // Change to main menu scene.
            ChangeScene(m_mainMenuSceneName);
        }
        public void OnHostJoinButton()
        {
            CancelRematchRequest();

            // Change to host join scene.
            ChangeScene(m_hostJoinSceneName);
        }
        public void OnQuitButton()
        {
            CancelRematchRequest();

#if UNITY_EDITOR
            Debug.Log("Quitting Application");
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }


        private void HandleOnGameOverStart()
        {
            UpdateRematchText();
        }
        private void HandleOnGameOverEnd()
        {
            CancelRematchRequest();
        }
        #region Rematch
        private void RequestRematch()
        {
            byte temp_teamIndex = DetermineTeamIndex();

            // Host
            if (isClient && isServer)
            {
                RequestRematchServer(temp_teamIndex);
            }
            // Non-host client
            else if (hasAuthority)
            {
                RequestRematchCmd(temp_teamIndex);
            }
            else
            {
                Debug.LogError($"Unhandled RequestMatch");
            }
        }
        [Command]
        private void RequestRematchCmd(byte teamIndex)
        {
            RequestRematchServer(teamIndex);
        }
        [Server]
        private void RequestRematchServer(byte teamIndex)
        {
            m_rematchCont.RequestRematchServer(teamIndex);
        }
        private void CancelRematchRequest()
        {
            byte temp_teamIndex = DetermineTeamIndex();

            // Host
            if (isClient && isServer)
            {
                CancelRematchRequestServer(temp_teamIndex);
            }
            // Non-host client
            else if (hasAuthority)
            {
                CancelRematchRequestCmd(temp_teamIndex);
            }
            else
            {
                Debug.LogError($"Unhandled RequestMatch");
            }
        }
        [Command]
        private void CancelRematchRequestCmd(byte teamIndex)
        {
            CancelRematchRequestServer(teamIndex);
        }
        [Server]
        private void CancelRematchRequestServer(byte teamIndex)
        {
            m_rematchCont.CancelRematchRequestServer(teamIndex);
        }

        private void OnTeamsThatWantRematchUpdate(SyncList<byte>.Operation op,
            int itemIndex, byte oldItem, byte newItem)
        {
            byte temp_myTeamIndex = DetermineTeamIndex();
            switch (op)
            {
                case SyncList<byte>.Operation.OP_ADD:
                    // This team was the one who requested the rematch
                    if (temp_myTeamIndex == newItem)
                    {
                        m_didThisTeamRequestRematch = true;
                    }
                    break;
                case SyncList<byte>.Operation.OP_REMOVEAT:
                    // This team canceled their request for a rematch
                    if (temp_myTeamIndex == oldItem)
                    {
                        m_didThisTeamRequestRematch = false;
                    }
                    break;
                default:
                    CustomDebug.UnhandledEnum(op, this);
                    break;
            }
            UpdateRematchText();
        }
        private void UpdateRematchText()
        {
            string temp_baseText = m_didThisTeamRequestRematch ?
                m_baseMyRequestText : m_baseOtherRequestText;
            int temp_requestedTeams = m_rematchCont.amountTeamsThatWantRematch;
            int temp_totalTeamAmount = m_teamConMan.requiredTeamAmount;
            // Only show the rematch text if at least 1 team wants a rematch
            m_rematchTextMesh.gameObject.SetActive(temp_requestedTeams > 0);

            m_rematchTextMesh.text = $"{temp_baseText}\n({temp_requestedTeams}/" +
                $"{temp_totalTeamAmount})";
        }
        #endregion Rematch
        private void ChangeScene(string sceneToGoTo)
        {
            // Set the state manager to end briefly, to reset things.
            m_battleStateMan.SetState(eBattleState.End);
            NetworkManager.singleton.offlineScene = sceneToGoTo;
            DisconnectFromServer();
        }
        private void DestroyPlayerInputs()
        {
            // Need to destroy the player input objects
            GameObject[] temp_playerInpObjs =
                GameObject.FindGameObjectsWithTag(m_playerInputTag);
            foreach (GameObject temp_singleInp in temp_playerInpObjs)
            {
                Destroy(temp_singleInp);
            }
        }
        private void DisconnectFromServer()
        {
            CustomDebug.LogForComponent(nameof(DisconnectFromServer), this,
                IS_DEBUGGING);

            // We intend to disconnect
            BattleNetworkManager.SetDisconnectIntended();

            // Is host
            if (isServer && isClient)
            {
                // Maybe use NetworkServer.Shutdown() instead?
                NetworkManager.singleton.StopHost();
            }
            // Is client
            else if (isClientOnly)
            {
                NetworkManager.singleton.StopClient();
            }
            else
            {
                Debug.LogError($"Unhandled case for " +
                    $"{nameof(DisconnectFromServer)}");
                return;
            }

            /* I added a dummy scene as the offline scene and now it
             * should be doing it automatically?
            // NetworkManager is persistant, but we will be entering the scene it
            // originated from, so we don't need it to come with us.
            #region Logs
            CustomDebug.LogForComponent($"NetworkMan is " +
                $"{NetworkManager.singleton.gameObject}", this, IS_DEBUGGING);
            #endregion Logs
            Destroy(NetworkManager.singleton.gameObject);
            #region Logs
            CustomDebug.LogForComponent("Should have destroyed the NetworkManager",
                this, IS_DEBUGGING);
            CustomDebug.LogForComponent($"NetworkMan is " +
                $"{NetworkManager.singleton.gameObject}", this, IS_DEBUGGING);
            #endregion Logs
             */
        }
        private byte DetermineTeamIndex()
        {
            BattlePlayerNetworkObject temp_playObj
                = BattlePlayerNetworkObject.myPlayerInstance;
            #region Asserts
            CustomDebug.AssertIsTrueForComponent(temp_playObj != null,
                $"player instance of {nameof(BattlePlayerNetworkObject)} to exist",
                this);
            #endregion Asserts
            return temp_playObj.teamIndex.teamIndex;
        }
    }
}
