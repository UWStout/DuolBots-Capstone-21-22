using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

using NaughtyAttributes;

namespace DuolBots
{
    [RequireComponent(typeof(PlayerIndex))]
    public class PlayerCameraInput : MonoBehaviour
    {
        [SerializeField] [Tag] private string m_cameraTag = "PlayerCamera";

        private ITeamIndex m_teamIndex;
        private PlayerIndex m_playerIndex = null;

        private CinemachineFreeLookCameraController m_freeLookController = null;


        // Domestic Initialization
        private void Awake()
        {
            m_teamIndex = GetComponent<ITeamIndex>();
            m_playerIndex = GetComponent<PlayerIndex>();

            Assert.IsNotNull(m_teamIndex, $"{name}'s {nameof(PlayerCameraInput)} requires " +
                $"{nameof(ITeamIndex)} to be attached.");
            Assert.IsNotNull(m_playerIndex, $"{name}'s {nameof(PlayerCameraInput)} requires " +
                $"{nameof(PlayerIndex)} to be attached.");

            
        }
        private void Start()
        {
            GameObject temp_myCamera = FindMyCamera();
            m_freeLookController = temp_myCamera.GetComponent<CinemachineFreeLookCameraController>();
            Assert.IsNotNull(m_freeLookController, $"{nameof(PlayerCameraInput)} found a " +
                $"camera without {nameof(CinemachineFreeLookCameraController)} attached.");
        }


        private GameObject FindMyCamera()
        {
            GameObject[] temp_camerasFoundArr = GameObject.FindGameObjectsWithTag(m_cameraTag);
            foreach (GameObject temp_singleCameraObj in temp_camerasFoundArr)
            {
                ITeamIndex temp_cameraTeamIndex = temp_singleCameraObj.GetComponent<ITeamIndex>();
                PlayerIndex temp_cameraPlayerIndex = temp_singleCameraObj.GetComponent<PlayerIndex>();

                Assert.IsNotNull(temp_cameraTeamIndex, $"{temp_singleCameraObj.name}" +
                    $" should have a {nameof(ITeamIndex)} attached.");
                Assert.IsNotNull(temp_cameraPlayerIndex, $"{temp_singleCameraObj.name}" +
                    $" should have a {nameof(PlayerIndex)} attached.");

                if (m_teamIndex.teamIndex == temp_cameraTeamIndex.teamIndex &&
                    m_playerIndex.playerIndex == temp_cameraPlayerIndex.playerIndex)
                {
                    return temp_singleCameraObj;
                }
            }

            Debug.LogError($"{name}'s {nameof(PlayerCameraInput)} could not find any" +
                $" camera that matched their player ({m_playerIndex.playerIndex}) " +
                $" and team ({m_teamIndex.teamIndex}) indices");
            return null;
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
