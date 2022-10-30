using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace DuolBots
{
    public class NavigatingMainMenu : MonoBehaviour
    {
        [SerializeField] private PlayingSounds m_playingSounds = null;
        [SerializeField] private EventSystem m_eventSystem = null;
        [SerializeField] private GameObject m_resolutionButton = null;
        [SerializeField] private WindowManager m_windowManager = null;

        private void OnNavigate(InputValue value)
        {
            Vector2 temp = value.Get<Vector2>();
            if (Mathf.Abs(temp.y) > 0.1f)
                m_playingSounds.MoveMenuSound();

            if (m_eventSystem.currentSelectedGameObject == m_resolutionButton)
            {
                if(temp.x > 0.1f)
                {
                    m_windowManager.SetNextResolution();
                }
                else if (temp.x < -0.1f)
                {
                    m_windowManager.SetPreviousResolution();
                }
            }
        }
    }
}
