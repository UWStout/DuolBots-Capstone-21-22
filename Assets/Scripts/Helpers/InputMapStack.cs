using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
// Original Authors - Wyatt Senalik and Eslis Vang
// Borrowed code from SquaredUp

namespace DuolBots
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputMapStack : MonoBehaviour
    {
        // Name of the default map
        [SerializeField] private string m_defaultMapName = "Player";
        // Reference to the player input
        public PlayerInput playerInput => m_playerInputRef;
        private PlayerInput m_playerInputRef = null;

        // Stack of active input maps.
        private List<string> m_activeMapNames = new List<string>();


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            m_playerInputRef = GetComponent<PlayerInput>();
            Assert.IsNotNull(m_playerInputRef, $"{name}'s {GetType().Name} requires " +
                $"{nameof(PlayerInput)} be attached but none was found");

            ResetInputMap();
        }


        /// <summary>Adds the given input map to the stack and swaps to it.</summary>
        /// <param name="inputMapName">Name of the input map to add to the stack.</param>
        public void SwitchInputMap(string inputMapName)
        {
            m_activeMapNames.Add(inputMapName);
            UpdateActiveInputMap();
        }

        /// <summary>Pops the input map with the given name off the stack and updates the input map to the one before it.</summary>
        /// <param name="inputMapName">Name of the input map to remove from the stack.</param>
        public void PopInputMap(string inputMapName)
        {
            m_activeMapNames.Remove(inputMapName);
            UpdateActiveInputMap();
        }

        /// <summary>Resets the input map to just be the starting input map.</summary>
        public void ResetInputMap()
        {
            // Initialize active map names to have the default map in it.
            m_activeMapNames = new List<string>();
            m_activeMapNames.Add(m_defaultMapName);
            UpdateActiveInputMap();
        }

        /// <summary>Updates the action map to be the map at the top of the stack.</summary>
        private void UpdateActiveInputMap()
        {
            if (m_activeMapNames.Count > 0)
            {
                string temp_mapToChangeTo = m_activeMapNames[m_activeMapNames.Count - 1];
                m_playerInputRef.SwitchCurrentActionMap(temp_mapToChangeTo);
                Assert.AreEqual(m_playerInputRef.currentActionMap.name, temp_mapToChangeTo,
                    $"Was not able to change action map to {temp_mapToChangeTo}");
            }
            else
            {
                Debug.LogError("Last input map was removed from the stack");
            }
        }
    }
}
