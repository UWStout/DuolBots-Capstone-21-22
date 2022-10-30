using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DuolBots
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerIndex))]
    public class SpawnNewPlayerInput : MonoBehaviour
    {
        private PlayerInput m_playerInput;
        [SerializeField] private GameObject m_newPlayerObject = null;
        private PlayerIndex m_playerIndex;
 

        private void Awake()
        {
            m_playerInput = GetComponent<PlayerInput>();
            CustomDebug.AssertComponentIsNotNull(m_playerInput, this);
            m_playerIndex = GetComponent<PlayerIndex>();
        }

        void Start()
        {
            SpawnPlayerInput();
        }

        public void SpawnPlayerInput()
        {
            InputDevice[] temp_inputDevices = m_playerInput.devices.ToArray();
            PlayerInput temp_spawnPlayerInput = PlayerInput.Instantiate(m_newPlayerObject, playerIndex: m_playerInput.playerIndex, pairWithDevices: temp_inputDevices);
            PlayerIndex temp_spawnPlayerIndex = temp_spawnPlayerInput.GetComponent<PlayerIndex>();
            temp_spawnPlayerIndex.playerIndex = m_playerIndex.playerIndex;
        }
    }
}
