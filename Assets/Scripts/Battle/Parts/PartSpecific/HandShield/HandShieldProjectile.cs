using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Script containing behavior for the Handshield's "projectile"
    /// </summary>
    public class HandShieldProjectile : MonoBehaviour
    {
        // Constants
        private const string PART = "Part";
        private const string PART_DAMAGEABLE = "PartDamageable";
        private const bool IS_DEBUGGING = false;

        [SerializeField] private float m_forceModifier = 1000.0f;

        private bool m_indexHit = false;
        // TeamIndex of the bot that "fired" this projectile
        private ITeamIndex m_teamIndex = null;
        /// <summary>
        /// Sets the TeamIndex that the projectile associates with it's own team. Used to avoid self-damage.
        /// </summary>
        /// <param name="index">TeamIndex to assign.</param>
        public void SetTeamIndex(TeamIndex index)
        {
            m_teamIndex = index;
        }

        private void OnEnable()
        {
            m_indexHit = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check that the collider inside the trigger is a damagable part
            if (other.CompareTag(PART) || other.CompareTag(PART_DAMAGEABLE))
            {
                // Check that the bot's TeamIndex is different from the the one that "fired" this projectile
                ITeamIndex temp_index = other.GetComponentInParent<TeamIndex>();
                if (temp_index != null)
                {
                    if (temp_index.teamIndex != m_teamIndex.teamIndex && !m_indexHit)
                    {
                        m_indexHit = true;
                        // Check that there is a Rigidbody to apply a force to
                        Rigidbody temp_rigidbody = other.attachedRigidbody;
                        if (temp_rigidbody != null)
                        {
                            CustomDebug.Log($"Projectile TI: {m_teamIndex.teamIndex}, hit TI: {temp_index.teamIndex}", IS_DEBUGGING);
                            other.attachedRigidbody.AddForce(transform.forward * m_forceModifier);
                        }
                        else
                        {
                            CustomDebug.Log($"{this.name} hit an enemy part {other.name} that did not have an attached Rigidbody to apply a force to.", IS_DEBUGGING);
                        }
                    }
                }
            }
        }
    }
}
