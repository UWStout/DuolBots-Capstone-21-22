using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Handles attached ParticleSystems on Weapon charge.
    /// </summary>
    public class ParticleSystemsOnChargeHandler : MonoBehaviour
    {
        [SerializeField] private Shared_ChargeSpawnProjectileFireController m_sharedSpawnProjectileFireController = null;
        // ParticleSystems used for charging (will always be set to play while charging and to stop while not charging).
        [SerializeField] private List<ParticleSystem> m_chargingParticleSystems = null;

        private void Awake()
        {
            // Check that VisualEffect is not null and that the shared controller is not null and then subscribe to its charging events
            Assert.IsNotNull(m_sharedSpawnProjectileFireController, $"{name} does not have a " +
                $"{m_sharedSpawnProjectileFireController.GetType()} but requires one.");
            Assert.IsNotNull(m_chargingParticleSystems, $"{name} does not have a " +
                $"{m_chargingParticleSystems.GetType()} but requires one.");

            m_sharedSpawnProjectileFireController.onStartedCharging += StartCharging;
            m_sharedSpawnProjectileFireController.onFinishedCharging += StopCharging;
        }

        private void OnDisable()
        {
            // In case the object is destroyed before OnDisable() gets called.
            if (m_sharedSpawnProjectileFireController != null)
            {
                m_sharedSpawnProjectileFireController.onFinishedCharging -= StartCharging;
                m_sharedSpawnProjectileFireController.onFinishedCharging -= StopCharging;
            }
        }

        private void StartCharging()
        {
            if (m_chargingParticleSystems != null && m_chargingParticleSystems.Count > 0)
            {
                foreach (ParticleSystem pSystem in m_chargingParticleSystems)
                {
                    if (!pSystem.isPlaying)
                    {
                        pSystem.Play();
                    }
                }
            }
        }

        private void StopCharging()
        {
            if (m_chargingParticleSystems != null && m_chargingParticleSystems.Count > 0)
            {
                foreach (ParticleSystem pSystem in m_chargingParticleSystems)
                {
                    if (!pSystem.isStopped)
                    {
                        pSystem.Stop();
                    }
                }
            }
        }
    }
}
