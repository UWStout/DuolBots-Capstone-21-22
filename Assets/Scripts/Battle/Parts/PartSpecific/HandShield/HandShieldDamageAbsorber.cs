using System;
using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Script that negates damage (up to the given threshold) that would happen to a part.
    /// </summary>
    [RequireComponent(typeof(DamageDealer))]
    public class HandShieldDamageAbsorber : MonoBehaviour
    {
        // Constants
        private bool IS_DEBUGGING = false;

        [SerializeField] private PartHealth m_partToProtect = null;
        [SerializeField] private float m_maximumDamageToBlock = 10.0f;

        private float m_curDamageToBlock = 0.0f;
        private DamageDealer m_damageDealer = null;

        // Domestic Initialization and assertion
        private void Awake()
        {
            m_damageDealer = GetComponent<DamageDealer>();
            Assert.IsNotNull(m_partToProtect, $"{name} does not have an attached {m_partToProtect.GetType()} but requires one.");
            if(m_partToProtect != null)
            {
                m_partToProtect.isDamagable = false;
                m_partToProtect.onDamagePrevented += PreventDamage;
            }
        }

        private void OnDisable()
        {
            // In case the part is destroyed before OnDisable() gets called on this.
            if (m_partToProtect != null)
            {
                m_partToProtect.onDamagePrevented -= PreventDamage;
            }
        }

        private void PreventDamage(float damage)
        {
            // If damage is 0 or negative (should not happen) don't do anything, else block damage and pass remaining damage back to part.
            if (damage <= 0)
            {
                CustomDebug.Log($"Something is trying to do negative {damage} damage" +
                    $" to {m_partToProtect}.", IS_DEBUGGING);
            }
            else
            {
                // Handle damage if PartHealth is not null
                if (m_partToProtect != null)
                {
                    // Update curDamageToBlock (debug damage values if applicable)
                    if (damage <= m_curDamageToBlock)
                    {
                        m_curDamageToBlock -= damage;
                        CustomDebug.Log($"{name} prevented {damage} damage on {m_partToProtect.name}," +
                            $" damage protection left on {name} is {m_curDamageToBlock}", IS_DEBUGGING);
                    }
                    else
                    {
                        m_curDamageToBlock = 0;
                        CustomDebug.Log($"{name} prevented {damage} damage on {m_partToProtect.name}," +
                             $" but {damage - m_curDamageToBlock} was left.", IS_DEBUGGING);
                    }
                    // Deal damage
                    m_damageDealer.damageToDeal = damage - m_curDamageToBlock;
                    // TODO Fix: needs to know team index.
                    //m_damageDealer.DealDamageToPart(m_partToProtect);
                }
                else
                {
                    Debug.LogError($"{name}'s {m_partToProtect.GetType()} did not have" +
                      $"an attached {m_partToProtect.GetType()} but requires one.");
                }


            }
        }
    }
}
