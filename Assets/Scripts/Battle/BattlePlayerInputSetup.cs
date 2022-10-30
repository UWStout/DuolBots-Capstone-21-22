using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

using NaughtyAttributes;
using System;
// Original Authors - Wyatt Senalik and Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Spawns player input prefabs based on what was created in the JoinPlayerScene.
    /// </summary>
    public class BattlePlayerInputSetup : MonoBehaviour
    {
        // The player input prefab to spawn
        [SerializeField] [Required] private GameObject m_playerPrefab = null;


        // Called 1st
        // Foreign Initialization
        private void Start()
        {
            SpawnPlayerInputs();
        }


        /// <summary>
        /// Spawns PlayerInput in the battle scene for each player stored
        /// in the CurrentPlayerInputDevices.
        /// </summary>
        private void SpawnPlayerInputs()
        {
            var temp = CurrentPlayerInputDevices.SpawnPlayerInputForEachDevice(m_playerPrefab);
            onPlayerInputSpawn?.Invoke(temp);
        }

        public event Action<IReadOnlyList<PlayerInput>> onPlayerInputSpawn;
    }
}
