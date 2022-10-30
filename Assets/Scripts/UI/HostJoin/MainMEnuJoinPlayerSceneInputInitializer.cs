using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DuolBots
{
    public class MainMEnuJoinPlayerSceneInputInitializer : MonoBehaviour
    {
        private void Awake()
        {
            // THIS MUST BE IN AWAKE
            // Join the player
            PlayerJoinMenuController temp_playerJoinCont = FindObjectOfType<PlayerJoinMenuController>();
            temp_playerJoinCont.JoinPlayer(gameObject);
        }
    }
}
