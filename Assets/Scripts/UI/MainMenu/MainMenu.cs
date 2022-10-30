using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DuolBots
{
    public class MainMenu : MonoBehaviour
    {
        public void LoadPlayerJoin()
        {
            SceneManager.LoadScene("PlayerJoin_SCENE");
        }

        public void LoadMainMenu()
        {
            DeletingPlayers();
            SceneManager.LoadScene("NewMainMenu");
        }

        private void DeletingPlayers()
        {
            JoinPlayerSceneInputInitializer[] temp_list = FindObjectsOfType<JoinPlayerSceneInputInitializer>();
            foreach (JoinPlayerSceneInputInitializer controls in temp_list)
            {
                Destroy(controls.gameObject);
            }
        }

        public void LoadHostJoin()
        {
            SceneManager.LoadScene("HostJoinPlayer");
        }


        public void Quit()
        {
            Debug.Log("Quitting This Awesome Game");
            Application.Quit();
        }
    }
}
