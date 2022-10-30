using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Handles the HandShield's pushing functionality.
    /// </summary>

    public class HandShieldPushController : MonoBehaviour, IWeaponFireController
    {
        // Animation for the Handshield's middle extending outwards 
        private Animation m_middleExtendAnim = null;
        // Transform to move the hand (end part) of the HandShield
        private Transform m_handShield = null;
        private GameObject m_handShieldProjectile = null;
        private Transform m_spawnPosition = null;
        // Variables that should be attached to the part itself
        private TeamIndex m_teamIndex = null;
        private CooldownRemaining m_coolDownRemaining = null;
        private Specifications_HandShield m_specifications = null;
        // How long before the weapon can fire again
        private float m_coolDown = 3.0f;
        private float m_curCoolDown = 0.0f;
        // How long a weapon fire takes
        private float m_duration = 1.25f;
        private float m_curDuration = 1.25f;
        private bool m_isFiring = false;
        private float m_charge;
        public float charge { get => m_charge; set => m_charge = value; }

        
        private void Awake()
        {
            m_teamIndex = GetComponentInParent<TeamIndex>();
            Assert.IsNotNull(m_teamIndex, $"{this.name} does not have an attached {m_teamIndex.GetType()} but requires one.");

            m_coolDownRemaining = GetComponent<CooldownRemaining>();
            Assert.IsNotNull(m_coolDownRemaining, $"{this.name} does not have an attached {m_coolDown.GetType()} but requires one.");

            m_specifications = GetComponent<Specifications_HandShield>();
            if(m_specifications != null)
            {
                m_spawnPosition = m_specifications.spawnPosition;
                Assert.IsNotNull(m_spawnPosition, $"{this.name} does not have a serialized spawn position {m_spawnPosition.GetType()} but requires one.");
                m_handShield = m_specifications.handShieldTransform;
                Assert.IsNotNull(m_handShield, $"{this.name} does not have an serialized Hand Shield {m_handShield.GetType()} but requires one.");
                m_handShieldProjectile = m_specifications.projectile;
                Assert.IsNotNull(m_handShieldProjectile, $"{this.name} does not have an attached projectile {m_handShieldProjectile.GetType()} but requires one.");

                m_duration = m_specifications.duration;
                m_coolDown = m_specifications.cooldown;
            }
            else { Debug.LogError($"{this.name} did not have a {m_specifications.GetType()} but requires one."); }
        }

        private void Update()
        {
            if(m_curCoolDown > 0.0f)
            {
                m_curCoolDown -= Time.deltaTime;
                m_coolDownRemaining.UpdateCoolDown(m_coolDown, m_curCoolDown);
            }
        }

        public void Fire(bool isPressed, eInputType type)
        {
            m_isFiring = isPressed;
            m_coolDownRemaining.inputType = type;
            StartCoroutine(Push());
        }
        public void AlternateFire(bool value, eInputType type) { /*This controller does not utilize alternate firing.*/}

        private IEnumerator Push()
        {
            if (m_isFiring && m_curCoolDown <= 0.0f)
            {
                // Instantiating projectile and setting TeamIndex
                GameObject temp_projectile = Instantiate(m_handShieldProjectile, m_spawnPosition);
                HandShieldProjectile temp_projectileBehavior = temp_projectile.GetComponent<HandShieldProjectile>();
                if(temp_projectileBehavior != null)
                {
                    temp_projectileBehavior.SetTeamIndex(m_teamIndex);
                }

                // Play the animation of the middle part extending.
                if (!m_middleExtendAnim.isPlaying)
                {
                    m_middleExtendAnim.Play();
                }

                // Contains the movement of the HandShield (updated by the amount moved, subtracted at the end to return to default position).
                Vector3 temp_position = Vector3.zero;

                // Move handshield object smoothly in "pushing motion" (Uses Vector3.left because the position correction is 90 degrees to the right)
                while(m_curDuration > 0.0f)
                {
                    m_curDuration -= .1f;
                    yield return new WaitForSeconds(0.05f);
                    m_handShield.localPosition += 0.2f * Vector3.left;
                    temp_position += 0.1f * Vector3.left;
                }

                // Reset duration, cooldown, and position
                m_curDuration = m_duration;
                m_handShield.localPosition -= temp_position;
                m_curCoolDown = m_coolDown;
                m_coolDownRemaining.UpdateCoolDown(m_coolDown, m_curCoolDown);
                
                yield return null;
            }
        }
    }
}
