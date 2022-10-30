using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Holder for the player's index to their input devices.
    /// </summary>
    /// 
    public static class CurrentPlayerInputDevices 
    {
        private const bool IS_DEBUGGING = true;

        // Dictionary that relates index to input devices.
        private static Dictionary<int, ReadOnlyArray<InputDevice>>
            m_playerInputDeviceDict = new Dictionary<int, ReadOnlyArray<InputDevice>>();


        /// <summary>
        /// Adds the given input devices to the dictionary for the player
        /// with the specified index.
        /// </summary>
        public static void AddReplaceInputDevice(int playerIndex,
            ReadOnlyArray<InputDevice> inputDevices)
        {
            string temp_inputDeviceString = "Add/Repalce Input Device for " + playerIndex;
            foreach (InputDevice device in inputDevices)
            {
                temp_inputDeviceString += " " + device.ToString();
            }
            CustomDebug.Log(temp_inputDeviceString, IS_DEBUGGING);
            if (m_playerInputDeviceDict.ContainsKey(playerIndex))
            {
                m_playerInputDeviceDict[playerIndex] = inputDevices;
            }
            else
            {
                m_playerInputDeviceDict.Add(playerIndex, inputDevices);
            }
        }
        /// <summary>
        /// Gets the input devices for the player with the given index.
        /// </summary>
        public static ReadOnlyArray<InputDevice> GetPlayerInputDevices(int playerIndex)
        {
            if (!m_playerInputDeviceDict.ContainsKey(playerIndex))
            {
                Debug.LogError($"No InputDevice was specified for player with " +
                    $"index {playerIndex}");
                return null;
            }

            return m_playerInputDeviceDict[playerIndex];
        }
        /// <summary>
        /// Gets all the player input devices and player indices.
        /// </summary>
        public static IReadOnlyList<KeyValuePair<int, ReadOnlyArray<InputDevice>>>
            GetAllPlayerInputDevices()
        {
            return new List<KeyValuePair<int,
                ReadOnlyArray<InputDevice>>>(m_playerInputDeviceDict);
        }
        /// <summary>
        /// Spawns a player input prefab for each connected device.
        /// </summary>
        /// <param name="playerPrefab">Player input prefab to spawn.
        /// Must have a <see cref="PlayerIndex"/> and a <see cref="ITeamIndex"/>
        /// attached.</param>
        public static IReadOnlyList<PlayerInput>
            SpawnPlayerInputForEachDevice(GameObject playerPrefab)
        {
            Assert.IsNotNull(playerPrefab, "Given Prebad is null for " + nameof(CurrentPlayerInputDevices));
            CustomDebug.Log("Spawning Prefab for " + GetAllPlayerInputDevices().Count + " Players", IS_DEBUGGING);

            List<PlayerInput> temp_spawnedPlayerList = new List<PlayerInput>();

            // Spawn a PlayerInput
            foreach (KeyValuePair<int, ReadOnlyArray<InputDevice>> temp_kvp in
                GetAllPlayerInputDevices())
            {
                int temp_playerIndex = temp_kvp.Key;
                ReadOnlyArray<InputDevice> temp_inputDevices = temp_kvp.Value;

                string temp_inputDeviceString = "Spawning Input Devices for " + temp_playerIndex;
                foreach(InputDevice device in temp_inputDevices)
                {
                    temp_inputDeviceString += " " + device.ToString();
                }
                CustomDebug.Log(temp_inputDeviceString, IS_DEBUGGING);

                // Spawn the player input
                PlayerInput temp_spawnedPlayerInp = PlayerInput.Instantiate(playerPrefab,
                    playerIndex: temp_playerIndex,
                    pairWithDevices: temp_inputDevices.ToArray());

                PlayerIndex temp_spawnedPlayerIndex = temp_spawnedPlayerInp.
                    GetComponent<PlayerIndex>();
                CustomDebug.AssertComponentIsNotNull(temp_spawnedPlayerIndex,
                    typeof(CurrentPlayerInputDevices));

                ITeamIndex temp_spawnedTeamIndex = temp_spawnedPlayerInp.
                    GetComponent<ITeamIndex>();
                CustomDebug.AssertComponentIsNotNull(temp_spawnedTeamIndex,
                    typeof(ITeamIndex),
                    typeof(CurrentPlayerInputDevices));

                temp_spawnedPlayerIndex.playerIndex = (byte)temp_playerIndex;
                // TODO FIX. Find out team index
                temp_spawnedTeamIndex.teamIndex = (byte)0;

                temp_spawnedPlayerList.Add(temp_spawnedPlayerInp);
            }

            return temp_spawnedPlayerList;
        }
    }
}
