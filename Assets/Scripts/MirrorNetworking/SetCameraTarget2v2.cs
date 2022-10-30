using UnityEngine;

using Cinemachine;
using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    [RequireComponent(typeof(NetworkTeamIndex))]
    public class SetCameraTarget2v2 : ClientNetworkBehaviour
    {
        private BattleStateManager m_stateMan = null;
        private BattleCameraSystem m_camSys = null;

        private ITeamIndex m_teamIndex = null;
        private BattleStateChangeHandler m_battleHandler = null;


        // Domestic Initialization
        private void Awake()
        {
            m_teamIndex = GetComponent<ITeamIndex>();

            #region Asserts
            CustomDebug.AssertIComponentIsNotNull(m_teamIndex, this);
            #endregion Asserts
        }
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            m_stateMan = BattleStateManager.instance;
            m_camSys = BattleCameraSystem.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_stateMan, this);
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_camSys, this);
            #endregion Asserts

            m_battleHandler = new BattleStateChangeHandler(m_stateMan,
                InitializeCamera, null, eBattleState.Battle);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();

            if (!isLocalPlayer) { return; }

            m_battleHandler.ToggleActive(false);
        }


        /// <summary>
        /// Sets the specified camera to be looking at the robot on my team.
        /// 
        /// Pre Conditions - Assumes FindRobotOnMyTeam's Pre Conditions are met.
        /// Assumes m_freeLookCam is not null.
        /// Post Conditions - Sets the specified camera to be looking at and
        /// following the robot.
        /// </summary>
        [Client]
        private void InitializeCamera()
        {
            if (!isLocalPlayer) { return; }

            // Assumes that this executes after the BotInstantiation
            RobotHelpersSingleton temp_botHelpers = RobotHelpersSingleton.instance;
            #region Asserts
            CustomDebug.AssertDynamicSingletonMonoBehaviourPersistantIsNotNull(
                temp_botHelpers, this);
            #endregion Asserts
            GameObject temp_myTeamRobotRoot = RobotHelpersSingleton.instance.
                FindBotRoot(m_teamIndex.teamIndex);
            SetCamerasToFocusOnTransform(temp_myTeamRobotRoot.transform);
        }
        [Client]
        private void SetCamerasToFocusOnTransform(Transform trans)
        {
            // Get the cameras.
            PairCamera<CinemachineFreeLook> temp_freeLookPairCam
                = m_camSys.activeBattleCameras;

            // Set both cameras to look at and follow the bot
            foreach (CinemachineFreeLook temp_freeLook
                in temp_freeLookPairCam.bothCameras)
            {
                temp_freeLook.LookAt = trans;
                temp_freeLook.Follow = trans;
            }
        }
    }
}
