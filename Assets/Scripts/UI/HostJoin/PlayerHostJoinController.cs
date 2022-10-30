using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DuolBots;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;
using UnityEngine.InputSystem.UI;
using System;

// Original Authors - Cole Woulf
namespace DuolBots
{
    public class PlayerHostJoinController : SingletonMonoBehaviour<PlayerHostJoinController>
    {
        [SerializeField] private BattlePlayerInputSetup m_battlePlayerInput;
        private IReadOnlyList<PlayerInput> m_playerInputList;
        [SerializeField] private string m_hostJoinInputMapName = "HostJoin";
        [SerializeField] private string m_battleInputMapName = "Battle";

        [SerializeField] private GameObject m_startTestP1 = null;
        [SerializeField] private GameObject m_startTestP2 = null;

        /// <summary>
        /// Enables the OnPlayerInputSpawn in order to set the input spawn on when needed
        /// </summary>
        private void OnEnable()
        {
            m_battlePlayerInput.onPlayerInputSpawn += OnPlayerInputSpawn;
        }

        /// <summary>
        /// Setting the player input object to our m_playerInputList IReadOnlyList<PlayerInput> object
        /// </summary>
        /// <param name="obj"></param>
        private void OnPlayerInputSpawn(IReadOnlyList<PlayerInput> obj)
        {
            m_playerInputList = obj;
        }

        /// <summary>
        /// Disables the OnPlayerInputSpawn so we can turn it off when need be
        /// </summary>
        private void OnDisable()
        {
            if (m_battlePlayerInput != null)
                m_battlePlayerInput.onPlayerInputSpawn -= OnPlayerInputSpawn;
        }

        /// <summary>
        /// The actual toggle that changes the player from testing your built bot, to the Host/Join UI
        /// </summary>
        /// <param name="isInTest"></param>
        /// <param name="playerIndex"></param>
        public void TogglePlayerInputActive(bool isInTest, byte playerIndex)
        {
            var temp_playerToToggle = m_playerInputList[playerIndex];
            if (isInTest)
            {
                temp_playerToToggle.SwitchCurrentActionMap(m_hostJoinInputMapName);
                if (playerIndex == 0)
                {
                    m_startTestP1.SetActive(true);
                }
                else
                {
                    m_startTestP2.SetActive(true);
                }
            }
            else
            {
                temp_playerToToggle.SwitchCurrentActionMap(m_battleInputMapName);
                if (playerIndex == 0)
                {
                    m_startTestP1.SetActive(false);
                }
                else
                {
                    m_startTestP2.SetActive(false);
                }
            }
        }

    }
}
