using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem.UI;
using NaughtyAttributes;
using System;

namespace DuolBots
{
    public class StartingUIArrows : MonoBehaviour
    {
        [SerializeField] [Required] private GameObject m_leftBeginElement = null;
        public GameObject leftBeginElement => m_leftBeginElement;
        [SerializeField] [Required] private GameObject m_rightBeginElement = null;
        public GameObject rightBeginElement => m_rightBeginElement;
        [SerializeField] [Tag] private string m_playerInputTag = "PlayerInput";

        void Start()
        {
            GameObject[] temp_playerInputs =
                                GameObject.FindGameObjectsWithTag(m_playerInputTag);
            if (temp_playerInputs.Length != 2)
            {
                CustomDebug.LogWarning($"Expected to find " +
                    $"2 players, instead found {temp_playerInputs.Length}");
            }

            foreach (GameObject temp_curPlayer in temp_playerInputs)
            {
                MultiplayerEventSystem temp_eventSys = temp_curPlayer.
                    GetComponentInChildren<MultiplayerEventSystem>();
                Assert.IsNotNull(temp_eventSys, $"{name}'s {GetType().Name} " +
                    $"requires {nameof(MultiplayerEventSystem)} in its children " +
                    $"but none was found.");
                PlayerIndex temp_playerIndex = temp_curPlayer.
                    GetComponent<PlayerIndex>();
                Assert.IsNotNull(temp_playerIndex, $"{name}'s {GetType().Name} " +
                    $"requires {nameof(PlayerIndex)} attached but none was found.");

                // Change which left and right element
                // to start on based on player index
                GameObject temp_beginElem = temp_playerIndex.playerIndex % 2 == 0 ?
                    m_leftBeginElement : m_rightBeginElement;

                temp_eventSys.SetSelectedGameObject(temp_beginElem);
            }
        }
    }
}
