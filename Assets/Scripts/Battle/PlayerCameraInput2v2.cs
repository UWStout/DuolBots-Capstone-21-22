using UnityEngine;
using UnityEngine.InputSystem;

using Cinemachine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    [RequireComponent(typeof(PlayerIndex))]
    public class PlayerCameraInput2v2 : MonoBehaviour
    {
        private BattleStateManager m_stateMan = null;
        private BattleCameraSystem m_camSys = null;

        private PlayerIndex m_playerIndex = null;

        private BattleStateChangeHandler m_battleHandler = null;
        private CinemachineFreeLookCameraController m_freeLookController = null;


        // Domestic Initialization
        private void Awake()
        {
            m_playerIndex = GetComponent<PlayerIndex>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_playerIndex, this);
            #endregion Asserts
        }
        // Foreign Initialization
        private void Start()
        {
            m_stateMan = BattleStateManager.instance;
            m_camSys = BattleCameraSystem.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_stateMan, this);
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_camSys, this);
            #endregion Asserts

            m_battleHandler = new BattleStateChangeHandler(m_stateMan,
                InitializeCameraInput, null, eBattleState.Battle);
        }
        private void OnDestroy()
        {
            m_battleHandler.ToggleActive(false);
        }


        private void InitializeCameraInput()
        {
            CinemachineFreeLook temp_myCamera = m_camSys.activeBattleCameras.
                GetPlayerCam(m_playerIndex.playerIndex);
            m_freeLookController = temp_myCamera.GetComponent
                <CinemachineFreeLookCameraController>();
            #region Asserts
            CustomDebug.AssertComponentOnOtherIsNotNull(m_freeLookController,
                temp_myCamera.gameObject, this);
            #endregion Asserts
        }


        /// <summary>
        /// Called by PlayerInput when the player tries to control the camera.
        /// Expects a Vector2.
        /// </summary>
        /// <param name="value"></param>
        private void OnCamera(InputValue value)
        {
            Vector2 temp_moveInput = value.Get<Vector2>();
            if (m_freeLookController == null)
            {
                return;
            }
            m_freeLookController.MoveCamera(temp_moveInput);
        }
    }
}
