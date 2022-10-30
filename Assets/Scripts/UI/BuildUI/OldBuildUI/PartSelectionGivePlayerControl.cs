using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Gives only the first player control of the CreateServer scene.
    /// </summary>
    public class PartSelectionGivePlayerControl : MonoBehaviour
    {
        // References to UI that will be controlled
        // Root UI object (Canvas)
        [SerializeField] [Required] private GameObject m_uiRoot = null;
        // Which UI element will be the default selection
        [SerializeField] [Required] private GameObject m_uiFirstSelected = null;

        private MultiplayerEventSystem m_activeEventSystem = null;

        [ReadOnly][SerializeField] private PlayerIndex m_currentPlayer = null;
        [ReadOnly][SerializeField] private PlayerIndex m_otherPlayer = null;

        // Called 1st
        // Foreign Initialization
        private void Start()
        {
            // Find the first player and all non first players
            FindPlayers(0, out PlayerIndex temp_firstPlayer,
                out PlayerIndex temp_otherPlayer);

            m_currentPlayer = temp_firstPlayer;
            m_otherPlayer = temp_otherPlayer;

            GiveFirstPlayerControl();
        }


        /// <summary>
        /// Called from buttons.
        /// </summary>
        /// <param name="uiToSelect"></param>
        public void SetSelectedUIComponent(GameObject uiToSelect)
        {
            m_activeEventSystem.SetSelectedGameObject(uiToSelect);
        }

        public void SwapPlayerControl()
        {
            m_otherPlayer.gameObject.SetActive(true);
            GivePlayerControl(m_otherPlayer.gameObject);

            RestrictPlayerControl(m_currentPlayer.gameObject);

            PlayerIndex temp_playerIndexHolder = m_currentPlayer;
            m_currentPlayer = m_otherPlayer;
            m_otherPlayer = temp_playerIndexHolder;
        }


        /// <summary>
        /// Gives control to the first player only
        ///
        /// Pre Conditions - A first player exists in the scene (signified
        /// by a player index equal to 0).
        /// Post Conditions - All non-first players interact with the UI at all
        /// but the first player can.
        /// </summary>
        public void GiveFirstPlayerControl()
        {
            // Give the first player control
            GivePlayerControl(m_currentPlayer.gameObject);
            // Restrict control from every other player
            RestrictPlayerControl(m_otherPlayer.gameObject);
        }
        /// <summary>
        /// Finds the players in the scene.
        /// 
        /// Pre Conditions - There exists a player in the scene with player index 0.
        /// Post Conditions - Returns references to the first player and other players.
        /// </summary>
        /// <param name="firstPlayer">Player with PlayerIndex 0.</param>
        /// <param name="otherPlayers">All other players who are not the first player.</param>
        /// <returns>A list of all the players, including the first player.</returns>
        private void FindPlayers(byte index, out PlayerIndex firstPlayer,
            out PlayerIndex otherPlayer)
        {
            firstPlayer = null;

            // Find all players in the scene
            IReadOnlyList<PlayerIndex> temp_playerIndices = FindObjectsOfType<PlayerIndex>();
            // Create a list that will hold all the non-first-player players
            List<PlayerIndex> temp_otherPlayers = new List<PlayerIndex>(temp_playerIndices);
            foreach (PlayerIndex temp_singlePlayerIndex in temp_playerIndices)
            {
                Debug.LogError($"Player obj index: {temp_singlePlayerIndex.playerIndex}\nLooking for index: {index}");
                // We've found the first player
                if (temp_singlePlayerIndex.playerIndex == index)
                {
                    Debug.Log($"Found the player with index: {index}");
                    firstPlayer = temp_singlePlayerIndex;
                    break;
                }
            }
            if (firstPlayer == null)
            {
                Debug.LogError("No FirstPlayer was found in the scene");
            }
            // Remove the first player from the list of other players
            else
            {
                temp_otherPlayers.Remove(firstPlayer);
            }
            otherPlayer = temp_otherPlayers[0];
        }
        /// <summary>
        /// Gives the specified player control of the UI scene.
        /// 
        /// Pre Conditions - The given player object must have a MultiplayerEventSystem
        /// attached to itself or its children. That MultiplayerEventSystem must be the
        /// event system for the player's PlayerInput as well. The given player object must
        /// also have a PlayerInput attached to itself.
        /// Post Conditions - Gives the player control over the CreateServer UI.
        /// </summary>
        /// <param name="playerObj">GameObject for a player.</param>
        private void GivePlayerControl(GameObject playerObj)
        {
            PlayerInput temp_playerInput = playerObj.GetComponent<PlayerInput>();
            Assert.IsNotNull(temp_playerInput, $"{name}'s {GetType().Name} requires " +
                $"a {nameof(PlayerInput)} to be attached to {playerObj.name}, " +
                $"but none was found.");

            m_activeEventSystem = playerObj.GetComponentInChildren<MultiplayerEventSystem>();
            Assert.IsNotNull(m_activeEventSystem, $"{name}'s {GetType().Name} requires " +
                $"a {nameof(MultiplayerEventSystem)} to be attached to {playerObj.name} " +
                $"or its children, but none was found.");

            m_activeEventSystem.playerRoot = m_uiRoot;
            m_activeEventSystem.firstSelectedGameObject = m_uiFirstSelected;
            m_activeEventSystem.SetSelectedGameObject(m_uiFirstSelected);
        }
        /// <summary>
        /// Disallows the specified player from interacting with the CreateServer UI.
        /// Most players won't be able to interact regardless, unless using a mouse
        /// or touch controls.
        ///
        /// Pre Conditions - None.
        /// Post Conditions - Restricts the player's control until the CreateServer UI
        /// is exited.
        /// </summary>
        /// <param name="playerObj">GameObject for a player.</param>
        private void RestrictPlayerControl(GameObject playerObj)
        {
            playerObj.SetActive(false);

            // Set them active again once the scene changes.
            SceneManager.activeSceneChanged += RestorePlayerControl;
            void RestorePlayerControl(Scene curScene, Scene newScene)
            {
                playerObj.SetActive(true);
                SceneManager.activeSceneChanged -= RestorePlayerControl;
            }
        }
    }
}
