using UnityEngine;
using UnityEngine.InputSystem.UI;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Initializes the Player input prefab for the JoinPlayerScene.
    /// </summary>
    public class JoinPlayerSceneInputInitializer : MonoBehaviour
    {
        // Reference to the player's event system
        [SerializeField] private MultiplayerEventSystem m_myEventSystem = null;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            // THIS MUST BE IN AWAKE
            // Why, idk
            // Join the player
            PlayerJoinSceneController temp_playerJoinCont = FindObjectOfType<PlayerJoinSceneController>();
            temp_playerJoinCont.JoinPlayer(gameObject, m_myEventSystem);
        }
    }
}
