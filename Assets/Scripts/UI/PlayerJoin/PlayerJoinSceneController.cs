using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
// Original Authors - Aaron Duffey and Wyatt Senalik
// Modifying Author - Cole Woulf

namespace DuolBots
{
    /// <summary>
    /// Controller for the PlayerJoinScene. Holds PlayerUI for each player
    /// and switches them on as players join/leave.
    /// </summary>
    public class PlayerJoinSceneController : MonoBehaviour
    {
        // UI that is set up for each player
        [SerializeField] private List<PlayerUI> m_playerUIList = new List<PlayerUI>();
        

        // A reference to the advance when ready script to see of both players are ready
        // to move on to the next scene
        private AdvanceWhenBothReady m_advance;

        private PlayingSounds m_playingSounds = null;

        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            UpdatePlayerUI();
            m_advance = FindObjectOfType<AdvanceWhenBothReady>();

            m_playingSounds = FindObjectOfType<PlayingSounds>();
            if (m_playingSounds == null) { return; }
        }


        /// <summary>
        /// Assigns the player with the given objects a PlayerUI.
        /// </summary>
        /// <param name="playerInputGameObject">GameObject that holds
        /// the player's PlayerInput script.</param>
        /// <param name="eventSystem">Player's event system.</param>
        /// <returns>Index assigned to the player.</returns>
        public int JoinPlayer(GameObject playerInputGameObject,
            MultiplayerEventSystem eventSystem)
        {
            // Find the first available PlayerUI
            for (int i = 0; i < m_playerUIList.Count; ++i)
            {
                PlayerUI temp_curPlayerUI = m_playerUIList[i];

                // Unavailable, taken already
                if (temp_curPlayerUI.isJoined) { continue; }

                // Not yet taken, so take it
                temp_curPlayerUI.isJoined = true;
                // Initialize some necessary stuff
                temp_curPlayerUI.playerInputGameObject = playerInputGameObject;
                temp_curPlayerUI.multiplayerEventSystem = eventSystem;
                eventSystem.playerRoot = temp_curPlayerUI.playerRoot;
                eventSystem.firstSelectedGameObject = temp_curPlayerUI.
                    firstSelected;

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

                // Detects the players device joining with
                var device = temp_curPlayerInput.devices[0];
                //Debug.Log("The Device is: " + device);
                if (device.ToString() == "Keyboard:/Keyboard")
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
            if(m_playerUIList == null) { return; }
            if(m_playerUIList[playerIndex] == null) { return; } 
            m_playerUIList[playerIndex].isJoined = false;
            m_playerUIList[playerIndex].isReady = false;
            if (m_playerUIList[playerIndex].multiplayerEventSystem == null) { return; }
            m_playerUIList[playerIndex].multiplayerEventSystem.
                SetSelectedGameObject(null);
            //m_playerUIList[playerIndex].multiplayerEventSystem.UpdateModules();
            Destroy(m_playerUIList[playerIndex].playerInputGameObject);
            PlayerUI temp_curPlayerUI = m_playerUIList[playerIndex];
            temp_curPlayerUI.keyboard.SetActive(false);
            temp_curPlayerUI.gamepad.SetActive(false);

            UpdatePlayerUI();
            m_advance.SetReadyUpState(m_playerUIList[playerIndex].isReady, playerIndex);
            m_playingSounds.ReadyUpSound();
        }

        /// <summary>
        /// Either readys up the player but if the player is already ready, then up readys them
        ///  then updates the coresponding UI
        /// </summary>
        /// <param name="playerIndex"></param>
        public void ReadyPlayer(int playerIndex)
        {
            m_playerUIList[playerIndex].isReady = !m_playerUIList[playerIndex].isReady;
            UpdatePlayerUI();
            m_advance.OnReadyUp(playerIndex);
            m_playingSounds.ReadyUpSound();
        }

        /// <summary>
        /// Turns UI on for Joined and turns a prompt on for the
        /// first unjoined UI.
        /// </summary>
        private void UpdatePlayerUI()
        {
            bool temp_unjoinedFoundAlready = false;
            foreach (PlayerUI temp_curPlayerUI in m_playerUIList)
            {
                // Active
                if (temp_curPlayerUI.isJoined)
                {
                    temp_curPlayerUI.joinedPortion.SetActive(true);
                    temp_curPlayerUI.unjoinedPortion.SetActive(false);
                }
                // Next-to-be active
                else if (!temp_unjoinedFoundAlready)
                {
                    temp_curPlayerUI.joinedPortion.SetActive(false);
                    temp_curPlayerUI.unjoinedPortion.SetActive(true);
                    temp_unjoinedFoundAlready = true;
                }
                // Far off active
                else
                {
                    temp_curPlayerUI.joinedPortion.SetActive(false);
                    temp_curPlayerUI.unjoinedPortion.SetActive(false);
                }
            }
        }

        /// <summary>
        /// UI for a single player. Holds serializefields for their UI objects.
        /// Also holds if the UI belongs to a player right now or not.
        /// Holds some references belonging to that player for ease of use.
        /// </summary>
        [Serializable]
        class PlayerUI
        {
            // Joined portion is active when a player is connected to this UI
            public GameObject joinedPortion => m_joinedPortion;
            [SerializeField] private GameObject m_joinedPortion = null;
            // Unjoined portion is active when this is the first unjoined UI
            public GameObject unjoinedPortion => m_unjoinedPortion;
            [SerializeField] private GameObject m_unjoinedPortion = null;
            // Canvas root of the PlayerUI
            public GameObject playerRoot => m_playerRoot;
            [SerializeField] private GameObject m_playerRoot = null;
            // First selected UI element that will be the
            // player's default initial selection
            public GameObject firstSelected => m_firstSelected;
            [SerializeField] private GameObject m_firstSelected = null;

            [SerializeField] private GameObject m_keyboard = null;
            public GameObject keyboard => m_keyboard;

            [SerializeField] private GameObject m_gamepad = null;
            public GameObject gamepad => m_gamepad;

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
            // The player's event system
            public MultiplayerEventSystem multiplayerEventSystem
            {
                set => m_multiplayerEventSystem = value;
                get => m_multiplayerEventSystem;
            }
            private MultiplayerEventSystem m_multiplayerEventSystem = null;
        }
    }
}
