using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Authors- Aaron Duffey and Ben Lussman

namespace DuolBots
{
    /// <summary>
    /// Gets the current cooldown of a Weapon (used for Icons)
    /// </summary>
    public class CooldownRemaining : MonoBehaviour
    {
        private float m_coolDown = 0.0f;
        public float coolDown => m_coolDown;
        private eInputType m_inputType = eInputType.buttonEast;
        public eInputType inputType { get => m_inputType; set => m_inputType = value; }

        public void UpdateCoolDown(float max, float current)
        {
            m_coolDown = Mathf.Clamp01(current / max);
        }

    }
}
