using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using NaughtyAttributes;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    ///  Laser Cannon's implementation of IChargeProjectileSpawn where the projectile grows in size and increases in damage based on the amount of charge.
    /// </summary>
    [RequireComponent(typeof(DamageDealer))]
    [RequireComponent(typeof(LineRenderer))]
    public class LaserCannonChargeProjectile : MonoBehaviour, ISpawnedChargeProjectile,
        ITeamIndexSetter
    {
        // Constants
        private bool IS_DEBUGGING = false;

        [SerializeField] [Required] private Transform m_spawnPosition = null;
        private DamageDealer m_damageDealer = null;
        [Tag] [SerializeField] private string m_partTag = "PartDamageable";
        private LineRenderer m_laser = null;
        [SerializeField] private LayerMask m_hittableLayers = 1 << 32;

        private float m_charge = 0.0f;
        public float charge => m_charge;

        public byte teamIndex { get; set; } = byte.MaxValue;


        private void Awake()
        {
            if (m_damageDealer == null)
            {
                m_damageDealer = GetComponent<DamageDealer>();
            }
            m_laser = GetComponent<LineRenderer>();
            Assert.IsNotNull(m_laser, $"{nameof(m_laser)} was not found on {this.GetType().Name}");
            Assert.IsNotNull(m_damageDealer, $"{nameof(m_damageDealer)} was not specificed on {name}'s {this.GetType().Name}");
            Assert.IsNotNull(m_spawnPosition, $"{nameof(m_spawnPosition)} was not specificed on {name}'s {this.GetType().Name}");
        }

        private void Start()
        {
            SpawnLaser();
        }

        public void SetCharge(float charge)
        {
            m_charge = charge;
        }

        public void SpawnLaser()
        {
            // Positions for the line renderer
            Vector3 temp_startPos = m_spawnPosition.position;
            Vector3 temp_endPos = temp_startPos + (1000f * m_spawnPosition.forward);

            // Display a LineRenderer for debugging puposes
            if (IS_DEBUGGING)
            {
                Vector3[] temp_positions = new Vector3[2];
                temp_positions[0] = temp_startPos;
                temp_positions[1] = temp_endPos;

                m_laser.enabled = true;

                m_laser.startColor = Color.red;
                m_laser.endColor = Color.red;

                m_laser.SetPositions(temp_positions);
            }
            else { m_laser.enabled = false; }
            // Actual collision detection
            RaycastHit hit;
            Physics.Raycast(m_spawnPosition.position,
                m_spawnPosition.forward, out hit,  1000f, m_hittableLayers);
            if (hit.collider != null)
            {
                CustomDebug.Log($"{name} hit {hit} with tag {hit.collider.tag}", IS_DEBUGGING);
                if (hit.collider.CompareTag(m_partTag))
                {
                    CustomDebug.Log($"{name} hit {hit} and is attempting to deal {m_damageDealer.damageToDeal}", IS_DEBUGGING);
                    m_damageDealer.DealDamageToPart(hit.collider, teamIndex);
                }
            }
        }
    }
}
