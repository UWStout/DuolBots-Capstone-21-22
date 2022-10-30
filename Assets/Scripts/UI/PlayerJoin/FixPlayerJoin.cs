using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DuolBots
{
    public class FixPlayerJoin : MonoBehaviour
    {

        [SerializeField] private List<GameObject> m_playerPrefabList = new List<GameObject>();
        [SerializeField] private GameObject m_newPlayerInputPrefab = null;

        private void OnPlayerJoined(PlayerInput player)
        {
            //m_playerList.Add(player.gameObject);

            MakingThePrefab(player);

            if (player.name == "PlayerJoinMenu_Prefab(Clone)")
                m_playerPrefabList.Add(player.gameObject);
        }

        private void MakingThePrefab(PlayerInput player)
        {
            InputDevice[] temp_inputDevices = player.GetComponent<PlayerInput>().devices.ToArray();
            PlayerInput temp_spawnPlayerInput = PlayerInput.Instantiate(m_newPlayerInputPrefab, playerIndex: player.GetComponent<PlayerInput>().playerIndex, pairWithDevices: temp_inputDevices);
            PlayerIndex temp_spawnPlayerIndex = temp_spawnPlayerInput.GetComponent<PlayerIndex>();
            temp_spawnPlayerIndex.playerIndex = player.GetComponent<PlayerIndex>().playerIndex;
        }

        /*
        private void BringingPlayerPrefabsBack()
        {
            foreach(GameObject temp_player in m_playerList)
            {
                
            }
        }*/

 
    }
}
