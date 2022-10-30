using System;
using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Script that controls the attached ParticleSystem on the LaserCannon weapon.
    /// This is assumed to do nothing on the clients during networking. Because
    /// input is sent to the server
    /// </summary>
    [RequireComponent(typeof(Shared_ChargeSpawnProjectileFireController))]
    public class LaserCannonParticleController : MonoBehaviour
    {
        [SerializeField] [Min(0.01f)] private float m_particleSpeedMultiplier = 1.0f;

        [SerializeField] [Required] private ParticleSystem m_particleSystem = null;
        private Shared_ChargeSpawnProjectileFireController
            m_sharedChargeSpawnProjectileFireController = null;


        // Domestic initialization
        private void Awake()
        {
            Assert.IsNotNull(m_particleSystem, $"{typeof(ParticleSystem).Name} " +
                $"was not found on {this.name} but is required.");
            m_sharedChargeSpawnProjectileFireController = GetComponent
                <Shared_ChargeSpawnProjectileFireController>();
            Assert.IsNotNull(m_sharedChargeSpawnProjectileFireController,
                $"{typeof(Shared_ChargeSpawnProjectileFireController).Name} " +
                $"was not found on {this.name} but is required.");
        }
        // Update is called once per frame
        private void Update()
        {
            // If the charge button is being held, start the particle system
            if (m_sharedChargeSpawnProjectileFireController.isCharging)
            {
                // Set the speed of the particle system to correspond to the
                // current charge of the laser.
                ParticleSystem.MainModule temp_main = m_particleSystem.main;
                temp_main.startSpeed = m_sharedChargeSpawnProjectileFireController.curCharge
                    * m_particleSpeedMultiplier;

                // Play particle system
                if (!m_particleSystem.isPlaying)
                {
                    m_particleSystem.Play();
                }
            }
            // If the charge button is not being held
            else
            {
                // Stop playing particle system
                if (!m_particleSystem.isStopped)
                {
                    m_particleSystem.Stop();
                }
            }
        }
    }
}
