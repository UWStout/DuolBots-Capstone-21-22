using System;
using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Wyatt Senalik
// Modified by Aaron Duffey (added code for preventing damage
// which is used by HandShieldDamageAbsorber)

namespace DuolBots
{
    /// <summary>
    /// Keeps track of an individual part's health.
    /// </summary>
    [RequireComponent(typeof(PartSOReference))]
    public class PartHealth : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        private PartSOReference m_partSORef = null;

        private float m_maxHealth = 1.0f;
        private float m_currentHealth = 0.0f;
        private bool m_isDamagable = true;
        public bool isDamagable { get; set; }

        /// <summary>
        /// Event for when this part's health is changed.
        /// Parameter is current health.
        /// </summary>
        public event Action<float> onHealthChanged;
        /// <summary>
        /// Event for when this part takes damage.
        /// Parameter is the damage the part takes.
        /// May take damage even if destroyed.
        /// </summary>
        public event Action<float> onDamageTaken;
        /// <summary>
        /// Event for when this part would have taken damage.
        /// Parameter is the damage the part takes.
        /// This happens when m_isDamagable is false
        /// (the Hand Shield uses this to prevent damage).
        /// </summary>
        public event Action<float> onDamagePrevented;
        /// <summary>
        /// Event for when this part takes damage.
        /// Parameters are the damage the part takes (float)
        /// and the team index for the team that dealt the damage (byte).
        /// May take damage even if destroyed.
        /// </summary>
        public event Action<float, byte> onDamageTakenFromTeam;


        // Domestic Initialization
        private void Awake()
        {
            m_partSORef = GetComponent<PartSOReference>();
            Assert.IsNotNull(m_partSORef, $"{name}'s {nameof(PartHealth)} requires" +
                $" {nameof(PartSOReference)}, but none was attached");

            m_maxHealth = m_partSORef.partScriptableObject.health;
            m_currentHealth = m_maxHealth;
        }


        /// <summary>
        /// Reduces the health of the part and has the robot's total health decrease as well.
        ///
        /// Pre Conditions - The damage taken is positive.
        /// Post Conditions - Current health is reduced by damage. Calls the onHealthChanged event.
        /// </summary>
        /// <param name="damageToTake">Amount of damage to take. Must be positive.</param>
        /// <param name="enemyTeamIndex">Index of the team that dealt damage.</param>
        public void TakeDamage(float damageToTake, byte enemyTeamIndex)
        {
            if (damageToTake <= 0)
            {
                CustomDebug.Log($"Something tried to do negative damage ({damageToTake}) to {name}", IS_DEBUGGING);
                return;
            }
            if(!m_isDamagable)
            {
                CustomDebug.Log($"Something tried to do damage to {name} but {name} is not currently damagable.", IS_DEBUGGING);
                onDamagePrevented?.Invoke(damageToTake);
                return;
            }

            // Update current health
            float temp_newCurHealth = m_currentHealth - damageToTake;
            m_currentHealth = Mathf.Clamp(temp_newCurHealth, 0.0f, m_maxHealth);

            onDamageTaken?.Invoke(damageToTake);
            onHealthChanged?.Invoke(m_currentHealth);
            onDamageTakenFromTeam?.Invoke(damageToTake, enemyTeamIndex);

            // Pass the damage upwards
            CustomDebug.Log($"Part took {damageToTake}, health is now {m_currentHealth}", IS_DEBUGGING);
        }

        //get maxhealth for ui - Shelby Vian
        public float GetMaxHealth()
        {
            return m_maxHealth;
        }

        //get current health for ui - Shelby Vian
        public float GetCurrentHealth()
        {
            return m_currentHealth;
        }
    }
}
