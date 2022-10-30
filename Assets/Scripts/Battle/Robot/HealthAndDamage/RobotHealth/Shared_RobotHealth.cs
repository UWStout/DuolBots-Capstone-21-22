using System;
using UnityEngine;
// Original Author - Wyatt Senalik, Skyler Grabowsky

namespace DuolBots
{
    /// <summary>
    /// Keeps track of the robots total health.
    /// </summary>
    public class Shared_RobotHealth : MonoBehaviour, IMonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;
        private const float CRITICAL_HEALTH_AMOUNT = 0.0f;

        // Max and current healths of the robot.
        private float m_maxHealth = 0.0f;
        private float m_currentHealth = 0.0f;
        private float m_criticalHealth = 0.0f;

        public float maxHealth => m_maxHealth;
        public float currentHealth => m_currentHealth;
        public float criticalHealth => m_criticalHealth;

        /// <summary>
        /// Event for when this robot's health is reduced.
        /// Parameter is current health.
        /// </summary>
        public event Action<float> onHealthChanged;
        /// <summary>
        /// Event for when this robot's health is zero.
        /// Only called once.
        /// </summary>
        public event Action onHealthReachedZero;
        /// <summary>
        /// Event for when this robot's health falls below the critical value.
        /// Only called once.
        /// </summary>
        public event Action onHealthReachedCritical;
        /// <summary>
        /// Event for when this robot's health is reduced with the team who dealt
        /// the damage as a parameter.
        /// Parameter is team index for the team that dealth damage.
        /// </summary>
        public event Action<float, byte> onTookDamageFromTeam;

        private PartHealth[] m_partHealthArr = new PartHealth[0];

        private bool m_wasOnHealthReachedZeroCalled = false;
        private bool m_wasOnHealthReachedCriticalCalled = false;


        /// <summary>
        /// Gets all the parts and subscribes to when each are damaged.
        /// </summary>
        public void GatherPartHealths()
        {
            m_partHealthArr = GetComponentsInChildren<PartHealth>();

            m_maxHealth = 0;
            foreach (PartHealth temp_partHealth in m_partHealthArr)
            {
                m_maxHealth += temp_partHealth.GetMaxHealth();
                temp_partHealth.onDamageTakenFromTeam += TakeDamage;
            }
            m_currentHealth = m_maxHealth;
            m_criticalHealth = m_maxHealth * CRITICAL_HEALTH_AMOUNT;

            CustomDebug.Log($"Gathered health from {m_partHealthArr.Length} " +
                $"parts. Max health is {m_maxHealth}. Critical health is " +
                $"{m_criticalHealth}.", IS_DEBUGGING);
        }


        /// <summary>
        /// Deals damage to the overall robot. 
        ///
        /// Pre Conditions - The damage taken is positive.
        /// Post Conditions - Current health is reduced by damage.
        /// Calls the onHealthChanged event.
        /// </summary>
        /// <param name="damageToTake">Amount of damage to take.
        /// Must be positive.</param>
        /// <param name="enemyTeamIndex">Team index of the team that is
        /// dealing the damage.</param>
        private void TakeDamage(float damageToTake, byte enemyTeamIndex)
        {
            if (damageToTake <= 0)
            {
                CustomDebug.Log($"Something tried to do negative " +
                    $"damage ({damageToTake}) to {name}", IS_DEBUGGING);
                return;
            }

            float temp_newCurHealth = m_currentHealth - damageToTake;
            m_currentHealth = Mathf.Clamp(temp_newCurHealth, 0.0f, m_maxHealth);

            onHealthChanged?.Invoke(m_currentHealth);
            onTookDamageFromTeam?.Invoke(damageToTake, enemyTeamIndex);

            if (m_currentHealth <= 0)
            {
                CustomDebug.Log("Zero health reached", IS_DEBUGGING);
                // Only call on health reached zero if it hasn't yet been called.
                if (!m_wasOnHealthReachedZeroCalled)
                {
                    onHealthReachedZero?.Invoke();
                    m_wasOnHealthReachedZeroCalled = true;
                }
            }
            if (m_currentHealth <= m_criticalHealth)
            {
                CustomDebug.Log("Critical health reached", IS_DEBUGGING);
                // Only call on health reached critical
                // if it hasn't yet been called.
                if (!m_wasOnHealthReachedCriticalCalled)
                {
                    onHealthReachedCritical?.Invoke();
                    m_wasOnHealthReachedCriticalCalled = true;
                }
            }

            CustomDebug.Log($"Robot took {damageToTake}, health is " +
                $"now {m_currentHealth}", IS_DEBUGGING);
        }

    }
}
