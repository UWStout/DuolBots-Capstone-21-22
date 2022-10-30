using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using NaughtyAttributes;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Horn's implementation of IChargeProjectileSpawn which spawns an arcing rainbow
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(DamageDealer))]
    public class HornChargeProjectile : MonoBehaviour, ISpawnedChargeProjectile
    {
        // Constants
        private bool IS_DEBUGGING = false;
        private static Color[] COLORS = new Color[7] { Color.red, Color.green, Color.blue, Color.magenta, Color.yellow, Color.cyan, Color.yellow };
        private const float DAMAGE_DELAY = 1.25f;
        private const float TRAIL_SPAWN_DELAY = 0.1f;
        private const float TRAIL_DESTROY_DELAY = 0.25f;

        // Spawn position of the next point in the LineRenderer trail
        [SerializeField] [Required] private Transform m_spawnPosition = null;
        [SerializeField] [Range(0, 10)] private float m_chargeDamageMultiplier = 1f;
        [SerializeField] private LineRenderer m_trail = null;
        [SerializeField, Required] private PartImpactCollider m_partImpCol = null;


        // How long until the trail behind the HornChargeProjectile can deal damage again
        private float m_curDamageDelay = 0.0f;
        // How long until the next point in the trail will spawn
        private float m_curTrailSpawnDelay = 0.0f;
        // How long until the oldest point in the trail will be destroyed
        private float m_curTrailDestructionDelay = 0.25f;

        private float m_charge = 0.0f;
        public float charge => m_charge;

        private DamageDealer m_damageDealer = null;

        private void Awake()
        {
            Assert.IsNotNull(m_spawnPosition, $"{nameof(m_spawnPosition)} was not specificed on {name}'s {GetType().Name}");
            m_damageDealer = GetComponent<DamageDealer>();
        }

        private void Update()
        {
            InstantiatePoint();
            CollisionDetection();
        }

        /// <summary>
        /// Creates a new point in the LineRenderer trail behind the HornChargeProjectile object.
        /// </summary>
        private void InstantiatePoint()
        {
            CustomDebug.Log($"Current Charge: {m_charge}", IS_DEBUGGING);

            if (m_curTrailSpawnDelay > 0.0f)
            { m_curTrailSpawnDelay -= Time.deltaTime; }
            else
            {
                // Update LineRenderer vertices, and color
                Vector3[] m_positions = new Vector3[m_trail.positionCount + 1];
                m_trail.GetPositions(m_positions);

                if (IS_DEBUGGING)
                {
                    // Check what the points of the LineRenderer are
                    int i = 0;
                    foreach (Vector3 pos in m_positions)
                    {
                        Debug.LogError($"{i} {pos}");
                        CustomDebug.Log($"Point: {i} Positions: {pos}", IS_DEBUGGING);
                        ++i;
                    }
                }
                m_positions[m_trail.positionCount] = m_spawnPosition.transform.position;
                m_trail.positionCount++;
                m_trail.SetPositions(m_positions);

                int temp_random = UnityEngine.Random.Range(1, COLORS.Length);
                m_trail.material.color = COLORS[COLORS.Length % temp_random];

                // Reset delay between spawning damage trail projectiles
                m_curTrailSpawnDelay = TRAIL_SPAWN_DELAY;
            }
            if (m_curTrailDestructionDelay > 0.0f)
            { m_curTrailDestructionDelay -= Time.deltaTime; }
            else
            {
                DestroyPoint();
            }
            CollisionDetection();
        }

        /// <summary>
        /// Deletes the oldest point in the LineRenderer trail behind the ChargeHornProjectile and sets the positions accordingly.
        /// </summary>
        private void DestroyPoint()
        {
            Vector3[] temp_positions = new Vector3[m_trail.positionCount];
            m_trail.GetPositions(temp_positions);

            // Use a List to remove the first point and resize before converting back to a Vector3[]
            // and setting positions (SetPositions[] only takes Vector3[]'s]
            List<Vector3> temp_list = new List<Vector3>(temp_positions);
            temp_list.RemoveAt(0);
            temp_positions = new Vector3[m_trail.positionCount - 1];

            temp_positions = temp_list.ToArray();

            m_trail.SetPositions(temp_positions);
            m_trail.positionCount--;

            m_curTrailDestructionDelay = TRAIL_DESTROY_DELAY;
        }

        [Obsolete("Used in damage collision (less generic than PartImpactCollider)", false)]
        private void CollisionDetection()
        {
            if (m_curDamageDelay > 0.0f)
            { m_curDamageDelay -= Time.deltaTime; }
            else
            {
                RaycastHit[] hits = new RaycastHit[m_trail.positionCount-1];
                for (int i = 0; i < m_trail.positionCount - 2; ++i)
                {

                    Physics.Linecast(m_trail.GetPosition(i), m_trail.GetPosition(++i), out hits[i]);

                }
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider != null)
                    {
                        m_damageDealer.damageToDeal = Mathf.Round(m_charge * m_chargeDamageMultiplier);
                        m_damageDealer.damageToDeal = m_damageDealer.damageToDeal;

                        if (hit.collider.gameObject.CompareTag("Part") || hit.collider.gameObject.CompareTag("PartDamageable"))
                        {
                            m_damageDealer.DealDamageToPart(hit.collider,
                                m_partImpCol.teamIndex);
                            m_curDamageDelay = DAMAGE_DELAY;
                        }
                    }
                }
            }
        }

        public void SetCharge(float charge)
        {
            m_charge = charge;
            // Scale charge by the charge damage multiplier and clamp before passing into DamageDealer
            DamageDealer temp_damageDealer = GetComponent<DamageDealer>();
            float temp_chargeDamage = charge * m_chargeDamageMultiplier;
            temp_damageDealer.damageToDeal = temp_chargeDamage;

            CustomDebug.Log($"{name}'s charge is {charge}, it's damage multiplier is {m_chargeDamageMultiplier}, " +
                $"and it's damage to deal has been clamped to {temp_chargeDamage}", IS_DEBUGGING);
        }
    }
}
