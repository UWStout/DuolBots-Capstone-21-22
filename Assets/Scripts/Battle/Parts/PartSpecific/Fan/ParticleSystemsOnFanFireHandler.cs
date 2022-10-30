using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Handles VFX for the Fan weapon on charge/fire.
    /// Acts the same as VFXOnChargeHandler but scales the play rate of the
    /// Particle Systems to the amount of time charge is held.
    /// </summary>
    [Obsolete("Fan VFX is now handled on ParticleSystemsOnInstantiatedFanFire \n" +
        "because the Fan VFX are being spawned on instantiated projectiles to \n" +
        "better replicate wind particles.", false)]
    public class ParticleSystemsOnFanFireHandler : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;
        private const float BASE_PLAY_RATE = 1.0f;
        private const float MAX_PLAY_RATE = 3.0f;
        private const float PLAY_RATE_RANGE = (MAX_PLAY_RATE - BASE_PLAY_RATE);

        [SerializeField] private FanProjectileFireController m_fanProjectileFireController = null;
        [SerializeField] private List<ParticleSystem> m_pSystems = null;

        
        private float m_curVFXPlayRate = 1.0f;

        private void Awake()
        {
            // Check that VisualEffect is not null and that the shared controller is not null and then subscribe to its charging events
            Assert.IsNotNull(m_fanProjectileFireController, $"{name} does not have a " +
                $"{m_fanProjectileFireController.GetType()} but requires one.");
            Assert.IsNotNull(m_pSystems, $"{name} does not have a " +
                $"{m_pSystems.GetType()} but requires one.");

            m_fanProjectileFireController.onStartedCharging += StartCharging;

            m_fanProjectileFireController.onFinishedCharging += StopCharging;
        }
        private void OnDisable()
        {
            // In case the object is destroyed before OnDisable() gets called.
            if (m_fanProjectileFireController != null)
            {
                m_fanProjectileFireController.onFinishedCharging -= StartCharging;
                m_fanProjectileFireController.onFinishedCharging -= StopCharging;
            }
        }


        public void SetPlayBackSpeed(float percentageOfMaCharge)
        {
            m_curVFXPlayRate = percentageOfMaCharge * PLAY_RATE_RANGE + BASE_PLAY_RATE;
        }


        private void StartCharging()
        {
            CustomDebug.Log($"StartCharging called.", IS_DEBUGGING);
            foreach (ParticleSystem pSystem in m_pSystems)
            {
                // Play the ParticleSystem at the current play rate given the charge.
                if (pSystem != null)
                {
                    ParticleSystem.MainModule temp_pModule = pSystem.main;
                    temp_pModule.simulationSpeed =  m_curVFXPlayRate;
                    pSystem.Play();
                }
                CustomDebug.Log($"{pSystem}'s simulation speed was set to {m_curVFXPlayRate}", IS_DEBUGGING);
            }
        }
        private void StopCharging()
        {
            CustomDebug.Log($"StopCharging called.", IS_DEBUGGING);
            m_curVFXPlayRate = BASE_PLAY_RATE;
            foreach (ParticleSystem pSystem in m_pSystems)
            {
                // Stop the ParticleSystem and reset the play rate.
                if (pSystem != null)
                {
                    ParticleSystem.MainModule temp_pModule = pSystem.main;
                    temp_pModule.simulationSpeed = BASE_PLAY_RATE;
                    pSystem.Stop();
                }
                CustomDebug.Log($"{pSystem} has been stopped.", IS_DEBUGGING);
            }
        }
    }
}
