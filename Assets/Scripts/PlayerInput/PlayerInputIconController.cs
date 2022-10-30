using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// UNUSED
    /// </summary>
    public class PlayerInputIconController : MonoBehaviour
    {
        public PlayerInputIcon playerInputIcon
        {
            get => m_playerInpIcon;
            set => m_playerInpIcon = value;
        }
        private PlayerInputIcon m_playerInpIcon = null;

        private Vector2 m_curMoveValue = Vector2.zero;


        private void Update()
        {
            // Its fine if we don't always have an icon.
            if (m_playerInpIcon == null) { return; }

            UpdateMoveIcon();
        }


        private void UpdateMoveIcon()
        {
            m_playerInpIcon.Move(m_curMoveValue);
        }


        private void OnNavigate(InputValue inputValue)
        {
            m_curMoveValue = inputValue.Get<Vector2>();
        }
    }
}
