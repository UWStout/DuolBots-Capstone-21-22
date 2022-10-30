using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using NaughtyAttributes;


namespace DuolBots
{
    [RequireComponent(typeof(PlayerIndex))]
    public class PlayerHostJoinMenu : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        private PlayerIndex m_playerIndex;
        MultiplayerEventSystem m_eventSystem;
        private bool m_isInsideTest = false;
        public bool isInsideTest => m_isInsideTest;
        [SerializeField] [Tag] private string m_playerInputTag = "PlayerInput";

        private MenuStack m_menuStack;

        private void Awake()
        {
            m_playerIndex = gameObject.GetComponent<PlayerIndex>();
            m_menuStack = FindObjectOfType<MenuStack>();
        }

        private void Start()
        {
            GameObject[] temp_allPlayerInputObjects = GameObject.FindGameObjectsWithTag(m_playerInputTag);
            foreach (GameObject temp in temp_allPlayerInputObjects)
            {
                PlayerIndex temp_playerIndex = temp.GetComponent<PlayerIndex>();
                if(temp_playerIndex.playerIndex == m_playerIndex.playerIndex)
                {
                    m_eventSystem = temp.GetComponentInChildren<MultiplayerEventSystem>();
                    return;
                }
            }
        }

        private void OnToggleTest(InputValue value)
        {
            if (value.isPressed)
            {
                CustomDebug.Log("OnToggleTest for " + m_playerIndex.playerIndex, IS_DEBUGGING);
                PlayerHostJoinController.instance.TogglePlayerInputActive(isInsideTest,m_playerIndex.playerIndex);
                if(m_isInsideTest)
                {
                    GameObject temp = CurrentDuolPlayerInput.instance.currentMenu.GetCorrespondingElement(m_playerIndex.playerIndex);
                    m_eventSystem.SetSelectedGameObject(temp);
                }
                else
                {
                    m_eventSystem.SetSelectedGameObject(null);
                }
                m_isInsideTest = !m_isInsideTest;
            }
        }

        private void OnBackMenu(InputValue value)
        {
            if(value.isPressed)
            {
                m_menuStack.CloseCurrentMenu();
            }
        }
    }
}
