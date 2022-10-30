using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using NaughtyAttributes;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// BurstLaser's implementation of IChargeProjectileSpawn where a burst of laser linerenderers are spawned after release
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(DamageDealer))]
    public class BurstLaserChargeProjectile : MonoBehaviour, ISpawnedChargeProjectile
    {
        // Constants
        const float LASER_DISPLAY_TIME = 0.1f;

        [SerializeField] [Required] private Transform m_spawnPosition = null;
        [Tag] [SerializeField] private string m_partTag = "Part";
        [SerializeField] [Range(0, 5)] private int m_numLasersPerBurst = 10;
        [SerializeField] [Range(0.1f, 1.0f)] private float m_delayBetweenLasers = 0.5f;
        private DamageDealer m_damageDealer = null;

        private LineRenderer m_laser = null;
        private float m_charge = 0.0f;
        public float charge => m_charge;

        private void Awake()
        {
            m_damageDealer = GetComponent<DamageDealer>();
            m_laser = GetComponent<LineRenderer>();
            Assert.IsNotNull(m_laser, $"{nameof(m_laser)} was not found on {this.GetType().Name}");
            Assert.IsNotNull(m_damageDealer, $"{nameof(m_damageDealer)} was not specificed on {name}'s {this.GetType().Name}");
            Assert.IsNotNull(m_spawnPosition, $"{nameof(m_spawnPosition)} was not specificed on {name}'s {this.GetType().Name}");
            StartCoroutine(SpawnLasers());
        }


        public void SetCharge(float charge)
        {
            m_charge = charge;
        }
        private IEnumerator SpawnLasers()
        {
            for(int i=0; i< m_numLasersPerBurst; ++i)
            {
                // Positions for the line renderer
                Vector3[] temp_positions = new Vector3[2];
                temp_positions[0] = m_spawnPosition.position;
                temp_positions[1] = temp_positions[0] + (1000f * m_spawnPosition.forward);

                m_laser.positionCount = 2;
                m_laser.SetPositions(temp_positions);
                m_laser.enabled = true;

                RaycastHit hit;
                Physics.Raycast(m_spawnPosition.position,
                    m_spawnPosition.forward, out hit, 1000f);
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag(m_partTag))
                    {
                        // TODO Fix: Use something else, or need to get team index.
                        //m_damageDealer.DealDamageToPart(hit.collider);
                    }
                }
                yield return new WaitForSecondsRealtime(LASER_DISPLAY_TIME);
                m_laser.enabled = false;
                yield return new WaitForSecondsRealtime(m_delayBetweenLasers);
            }
            yield return null;
        }

    }

}
