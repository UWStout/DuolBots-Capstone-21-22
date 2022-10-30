using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace DuolBots
{
    public class MainMenuControls : MonoBehaviour
    {

        private PlayerInput m_playerInput = null;
        private PlayerJoinMenuController m_playerController;
        private PlayerIndex m_playerIndex;
        private MenuStack m_menuStack;
        private MenuController m_menuController;

        private void Awake()
        {
            m_playerInput = this.GetComponent<PlayerInput>();
            m_playerController = FindObjectOfType<PlayerJoinMenuController>();
            m_playerIndex = this.GetComponent<PlayerIndex>();
            m_menuStack = FindObjectOfType<MenuStack>();
            m_menuController = FindObjectOfType<MenuController>();
        }

        // PlayerJoin Scene Buttons

        public void OnReady(InputValue value)
        {
            if (value.isPressed)
                m_playerController.ReadyPlayer(m_playerIndex.playerIndex);
        }

        // not unreadying up with visuals and backend
        public void OnUnready(InputValue value)
        {
            if (value.isPressed)
                m_playerController.ReadyPlayer(m_playerIndex.playerIndex);
        }

        // not deleting correctly
        public void OnLeave(InputValue value)
        {
            if (value.isPressed)
            {
                m_playerController.DisconnectPlayer(m_playerIndex.playerIndex);
                Destroy(this.gameObject);
            }
        }

        public void OnBack(InputValue value)
        {
            if (value.isPressed)
            {
                if (m_playerController != null)
                {
                    m_playerController.DisconnectPlayer(0);
                    m_playerController.DisconnectPlayer(1);
                    MainMenuControls [] temp_list = FindObjectsOfType<MainMenuControls>();
                    foreach(MainMenuControls controls in temp_list)
                    {
                        Destroy(controls.gameObject);
                    }
                    
                }
                m_menuStack.CloseCurrentMenu();
                m_menuController.BackToMainMenu();
            }
        }
    }
}
