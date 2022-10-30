using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace DuolBots
{
    public class PlayerJoinMenuController : MonoBehaviour
    {
        // UI that is set up for each player
        [SerializeField] private List<JoinMenuPlayerUI> m_playerUIList = new List<JoinMenuPlayerUI>();
        private AdvanceWhenBothReady m_advance;

        [SerializeField] private GameObject m_newPlayerInputPrefab = null;
        [SerializeField] private List<GameObject> m_playerPrefabList = new List<GameObject>();

        private void Awake()
        {
            UpdatePlayerUI();
            m_advance = FindObjectOfType<AdvanceWhenBothReady>();
        }

        /// <summary>
        /// Assigns the player with the given objects a PlayerUI.
        /// </summary>
        /// <param name="playerInputGameObject">GameObject that holds
        /// the player's PlayerInput script.</param>
        /// <param name="eventSystem">Player's event system.</param>
        /// <returns>Index assigned to the player.</returns>
        public int JoinPlayer(GameObject playerInputGameObject)
        {
            // Find the first available PlayerUI
            for (int i = 0; i < m_playerUIList.Count; ++i)
            {
                JoinMenuPlayerUI temp_curPlayerUI = m_playerUIList[i];

                // Unavailable, taken already
                if (temp_curPlayerUI.isJoined) { continue; }

                // Not yet taken, so take it
                temp_curPlayerUI.isJoined = true;

                temp_curPlayerUI.playerInputGameObject = playerInputGameObject;

                // Save the player's input devices
                PlayerInput temp_curPlayerInput =
                    playerInputGameObject.GetComponent<PlayerInput>();
                Assert.IsNotNull(temp_curPlayerInput, $"{name}'s {GetType().Name} " +
                    $"requires {playerInputGameObject.name} to have a " +
                    $"{typeof(PlayerInput)} attached. None was found");
                CurrentPlayerInputDevices.AddReplaceInputDevice(i,
                    temp_curPlayerInput.devices);

                // Set the player's index
                PlayerIndex temp_curPlayerIndex =
                    playerInputGameObject.GetComponent<PlayerIndex>();
                Assert.IsNotNull(temp_curPlayerIndex, $"{name}'s {GetType().Name} " +
                    $"requires {playerInputGameObject.name} to have a " +
                    $"{typeof(PlayerIndex)} attached. None was found");
                temp_curPlayerIndex.playerIndex = (byte)i;

                m_playerPrefabList.Add(temp_curPlayerInput.gameObject);

                // instantiated and delete the prefab
                AddDeletePrefabs(temp_curPlayerInput, temp_curPlayerIndex);



                 // Detects the players device joining with
                 var device = temp_curPlayerInput.devices[0];
                //Debug.Log("The Device is: " + device);
                if(device.ToString() == "Keyboard:/Keyboard")
                {
                    temp_curPlayerUI.keyboard.SetActive(true);
                    temp_curPlayerUI.gamepad.SetActive(false);
                }
                else
                {
                    temp_curPlayerUI.keyboard.SetActive(false);
                    temp_curPlayerUI.gamepad.SetActive(true);
                }

                // Update the player UI to reflect the joined change
                UpdatePlayerUI();
                return i;
            }

            // No ui is available. If this is happening, check the
            // max player amount being allowed by the PlayerInputManager.
            Debug.LogError("No PlayerUI available for player to join");
            return -1;
        }
        /// <summary>
        /// Called from disconnect button.
        /// </summary>
        /// <param name="playerIndex"></param>
        public void DisconnectPlayer(int playerIndex)
        {
            m_playerUIList[playerIndex].isJoined = false;
            m_playerUIList[playerIndex].isReady = false;
            Destroy(m_playerUIList[playerIndex].playerInputGameObject);
            JoinMenuPlayerUI temp_curPlayerUI = m_playerUIList[playerIndex];
            temp_curPlayerUI.keyboard.SetActive(false);
            temp_curPlayerUI.gamepad.SetActive(false);

            foreach(GameObject temp_playerInput in m_playerPrefabList)
            {
                Destroy(temp_playerInput);
            }

            UpdatePlayerUI();
            m_advance.SetReadyUpState(m_playerUIList[playerIndex].isReady, playerIndex);
        }

        public void AddDeletePrefabs(PlayerInput input, PlayerIndex index)
        {
            Debug.Log("Starting deletion");
            MainMenuControls[] temp_list = FindObjectsOfType<MainMenuControls>();
            foreach (MainMenuControls temp_controls in temp_list)
            {
                Destroy(temp_controls.gameObject);
            }

            Debug.Log("Starting Instantiating");
            if(m_playerPrefabList.Count == 0)
            {
                InputDevice[] temp_inputDevices = input.devices.ToArray();
                PlayerInput temp_spawnPlayerInput = PlayerInput.Instantiate(m_newPlayerInputPrefab, playerIndex: input.playerIndex, pairWithDevices: temp_inputDevices);
                PlayerIndex temp_spawnPlayerIndex = temp_spawnPlayerInput.GetComponent<PlayerIndex>();
                temp_spawnPlayerIndex.playerIndex = index.playerIndex;

                m_playerPrefabList.Add(temp_spawnPlayerInput.gameObject);
            }
            else
            {
                foreach (GameObject temp in m_playerPrefabList)
                {
                    InputDevice[] temp_inputDevices = temp.GetComponent<PlayerInput>().devices.ToArray();
                    PlayerInput.Instantiate(m_newPlayerInputPrefab, playerIndex: temp.GetComponent<PlayerInput>().playerIndex, pairWithDevices: temp_inputDevices);
                    PlayerIndex temp_spawnPlayerIndex = temp.GetComponent<PlayerIndex>();
                    temp_spawnPlayerIndex.playerIndex = temp.GetComponent<PlayerIndex>().playerIndex;
                }
            }
            
        }

        /// <summary>
        /// Turns UI on for Joined and turns a prompt on for the
        /// first unjoined UI.
        /// </summary>
        private void UpdatePlayerUI()
        {
            bool temp_unjoinedFoundAlready = false;
            foreach (JoinMenuPlayerUI temp_curPlayerUI in m_playerUIList)
            {
                //Debug.Log("isJoined: " + temp_curPlayerUI.isJoined);
                //Debug.Log("isReady: " + temp_curPlayerUI.isReady);

                if (temp_curPlayerUI.isReady)
                {
                    temp_curPlayerUI.joinedPortion.SetActive(false);
                    temp_curPlayerUI.unjoinedPortion.SetActive(false);
                    temp_curPlayerUI.readyPortion.SetActive(true);
                }
                // Active and is joined
                else if (temp_curPlayerUI.isJoined)
                {
                    temp_curPlayerUI.joinedPortion.SetActive(true);
                    temp_curPlayerUI.unjoinedPortion.SetActive(false);
                    temp_curPlayerUI.readyPortion.SetActive(false);
                }
                // pressing A or Enter to join
                else if (!temp_unjoinedFoundAlready)
                {
                    temp_curPlayerUI.joinedPortion.SetActive(false);
                    temp_curPlayerUI.unjoinedPortion.SetActive(true);
                    temp_curPlayerUI.readyPortion.SetActive(false);
                    temp_unjoinedFoundAlready = true;
                }
                // Far off active
                else
                {
                    temp_curPlayerUI.joinedPortion.SetActive(false);
                    temp_curPlayerUI.unjoinedPortion.SetActive(false);
                    temp_curPlayerUI.readyPortion.SetActive(false);
                }
            }
        }

        public void ReadyPlayer(int playerIndex)
        {
            m_playerUIList[playerIndex].isReady = !m_playerUIList[playerIndex].isReady;
            UpdatePlayerUI();
            m_advance.OnReadyUp(playerIndex);
        }
    }

    /// <summary>
    /// UI for a single player. Holds serializefields for their UI objects.
    /// Also holds if the UI belongs to a player right now or not.
    /// Holds some references belonging to that player for ease of use.
    /// </summary>
    [Serializable]
    class JoinMenuPlayerUI
    {
        [SerializeField] private GameObject m_joinedPortion = null;
        public GameObject joinedPortion => m_joinedPortion;

        // Unjoined portion is active when this is the first unjoined UI
        [SerializeField] private GameObject m_unjoinedPortion = null;
        public GameObject unjoinedPortion => m_unjoinedPortion;
        // Player has readied up and is awaiting the other player
        // or to advance to next scene
        [SerializeField] private GameObject m_readyPortion = null;
        public GameObject readyPortion => m_readyPortion;

        [SerializeField] private GameObject m_keyboard = null;
        public GameObject keyboard => m_keyboard;

        [SerializeField] private GameObject m_gamepad = null;
        public GameObject gamepad => m_gamepad;

        /*
        public JoinMenuPlayerUI()
        {
            keyboard.SetActive(false);
            gamepad.SetActive(false);
        }
        */



        // If there is a player connected to this UI
        public bool isJoined
        {
            set => m_isJoined = value;
            get => m_isJoined;
        }
        private bool m_isJoined = false;

        public bool isReady
        {
            set => m_isReady = value;
            get => m_isReady;
        }
        private bool m_isReady = false;

        // References to connected player objects
        // GameObject that holds the player's PlayerInput script
        public GameObject playerInputGameObject
        {
            set => m_playerInputGameObject = value;
            get => m_playerInputGameObject;
        }
        private GameObject m_playerInputGameObject = null;
    }
}

