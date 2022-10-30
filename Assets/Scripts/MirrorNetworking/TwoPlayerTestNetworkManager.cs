using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

using Mirror;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    public class TwoPlayerTestNetworkManager : NetworkManager
    {
        // The player input prefab to spawn
        [SerializeField] [Required] private GameObject m_playerInputPrefab = null;
        [SerializeField] private Camera[] m_playerCameraArr = new Camera[1];


        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            GameObject temp_connectionObject =
                Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

            NetworkServer.AddPlayerForConnection(conn, temp_connectionObject);

            // Spawn a player object for each connected player device
            SpawnPlayerInputs();
        }


        /// <summary>
        /// Spawns PlayerInput in the battle scene for each player stored
        /// in the CurrentPlayerInputDevices.
        /// </summary>
        private void SpawnPlayerInputs()
        {
            // Spawn a PlayerInput
            foreach (KeyValuePair<int, ReadOnlyArray<InputDevice>> temp_kvp in
                CurrentPlayerInputDevices.GetAllPlayerInputDevices())
            {
                int temp_playerIndexValue = temp_kvp.Key;
                ReadOnlyArray<InputDevice> temp_inputDevices = temp_kvp.Value;

                // Spawn the player input
                PlayerInput temp_spawnedPlayerInp = PlayerInput.Instantiate(m_playerInputPrefab,
                    playerIndex: temp_playerIndexValue,
                    pairWithDevices: temp_inputDevices.ToArray());

                // Grab the player and team index components off the
                // spawned player input object
                PlayerIndex temp_spawnedPlayerIndex =
                    temp_spawnedPlayerInp.GetComponent<PlayerIndex>();
                ITeamIndex temp_spawnedTeamIndex =
                    temp_spawnedPlayerInp.GetComponent<ITeamIndex>();
                // Initialize the values of the player and team indices
                temp_spawnedPlayerIndex.playerIndex = (byte)temp_playerIndexValue;
                // TODO FIX. Find out team index
                temp_spawnedTeamIndex.teamIndex = (byte)0;

                // Grab the player's canvas
                Canvas temp_canvas = temp_spawnedPlayerInp.GetComponentInChildren<Canvas>();
                if (m_playerCameraArr.Length <= temp_playerIndexValue)
                {
                    // Not enough camera's for this player
                    Debug.LogError($"Not enough cameras for the amount of players" +
                        $" who have joined.");
                    continue;
                }
                temp_canvas.worldCamera = m_playerCameraArr[temp_playerIndexValue];
            }
        }
    }
}
